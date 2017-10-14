using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using System.Windows.Forms;

namespace LEO
{
    public partial class counter : Form
    {

        Device device;
        int semaphoreTimeout = 4000;

        UInt32 refSamples = 1000000;
        double cntPaint;
        int circBuffSizeTest;
        const UInt32 timEtrPeriphClock = 144000000;
        static string[] refPins = new string[2];
        ushort cntIcPresc1 = 1, cntIcPresc2 = 1;
        int trackVal1, trackVal2;
        bool trackIc1Rec = false, trackIc2Rec = false;
        uint ic2buffer;

        private const string ETR_MIN = "Min val. 2";
        private const string ETR_MAX = "Max val. 200";

        private const string IC_MIN = "Min val. ";
        private const string IC_MAX = "Max val. ";

        private const string REF1_MIN = "Min val. 2";
        private const string REF2_MIN = "Min val. 1";
        private const string REF1_MAX = "Max val. 64000";
        private const string REF2_MAX = "Max val. 64000";
        private const string REF_MAX = "Max val. 4.096 G";
        private const string REF_MIN = "Min val. 2";

        private const string NUMS_ONLY = "Numbers only";
        private const string NUM_ENTER = "Enter a number";

        public enum CNT_MODES { ETR = 0, IC, REF, TI };
        CNT_MODES counterMode;

        /* REF/ETR/IC trackBars scrolling */
        public enum CNT_TIMER { TIM_SCROLL_REF1 = 0, TIM_SCROLL_REF2, TIM_SCROLL_IC1, TIM_SCROLL_IC2, TIM_SCROLL_ETR, TIM_SCROLL_TI };
        CNT_TIMER timScroll;

        /* IC pulse mode */
        public enum CNT_DUTY_CYCLE { DUTY_CYCLE_DISABLED = 0, DUTY_CYCLE_ENABLED };
        CNT_DUTY_CYCLE ic1DutyCycle, ic2DutyCycle;

        public enum KEY_PRESS { YES = 0, NO };
        KEY_PRESS keyPress;

        System.Timers.Timer GUITimer;
        System.Timers.Timer scrollTimer;

        private Queue<Message> cnt_q = new Queue<Message>();
        Message.MsgRequest req;
        Message messg;

        public LinkedList<double> avrgList = new LinkedList<double>();
        LinkedListNode<double> nodeCurrent;
        double average;
          

        public counter(Device dev)
        {
            InitializeComponent();
            InitializeTIComponent();
            
            device = dev;

            GUITimer = new System.Timers.Timer(50);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            GUITimer.Start();

            scrollTimerConfig();
            mcuTypeSetLabels();

            cnt_init_mode(CNT_MODES.ETR);
            cnt_start();
        }

        private void mcuTypeSetLabels()
        {
            /* ETR labels */
            label_cnt_etr_maxFreq.Text = (timEtrPeriphClock / 4 * 8 / 1000000).ToString() + " MHz";            // 1/4 periph clock = max ETR input frequency * 8 (prescaler)

            /* REF labels */
            label_cnt_refRatio_pins.Text = "Frequency ratio (" + refPins[0] + " / " + refPins[1] + ")" + " [-]";
            label_cnt_ref_input.Text = "Max. freq. reference input (" + refPins[1].ToString() + ") :";
            label_cnt_ref_sec_input.Text = "Max. freq. second input (" + refPins[0].ToString() + ") :";
            label_cnt_ref_maxFreq.Text = (timEtrPeriphClock / 4 * 8 / 1000000).ToString() + " MHz";
        }

        private void InitializeTIComponent()
        {
            this.richTextBox_it_ab.Font = new System.Drawing.Font(richTextBox_it_ab.Font.FontFamily, 10);
            this.richTextBox_it_ab.SelectionStart = 2;
            this.richTextBox_it_ab.SelectionLength = 2;
            this.richTextBox_it_ab.SelectionCharOffset = -4;
            this.richTextBox_it_ab.SelectionLength = 0;

            this.richTextBox_it_ba.Font = new System.Drawing.Font(richTextBox_it_ba.Font.FontFamily, 10);
            this.richTextBox_it_ba.SelectionStart = 2;
            this.richTextBox_it_ba.SelectionLength = 2;
            this.richTextBox_it_ba.SelectionCharOffset = -4;
            this.richTextBox_it_ba.SelectionLength = 0;
        }

        public void setRefRatioLabel(string text)
        {            
            this.label_cnt_refRatio_pins.Text = text;
        }

        private static List<T> CreateList<T>(int size)
        {
            return Enumerable.Repeat(default(T), size).ToList();
        }

        private void Update_GUI(object sender, ElapsedEventArgs e)
        {
            if (cnt_q.Count > 0)
            {
                messg = cnt_q.Dequeue();
                if (messg == null)
                {
                    return;
                }
                switch (req = messg.GetRequest())
                {
                    /* ETR messages */
                    case Message.MsgRequest.COUNTER_ETR_DATA:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        break;

                    /* REF messages */
                    case Message.MsgRequest.COUNTER_REF_DATA:
                        cntPaint = messg.GetNum();
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.COUNTER_REF_WARN:
                        this.Invalidate();
                        break;

                    /* IC messages */
                    case Message.MsgRequest.COUNTER_IC1_DATA:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.COUNTER_IC2_DATA:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.COUNTER_IC1_DUTY_CYCLE:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.COUNTER_IC1_PULSE_WIDTH:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        break;

                    /* TI messages */
                    case Message.MsgRequest.COUNTER_TI_TIMEOUT:
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.COUNTER_TI_EQUAL:
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.COUNTER_TI_BIGGER_BUF1:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.COUNTER_TI_BIGGER_BUF2:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        break;
                }
            }
        }
        
        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                /************************************** ETR DATA PRINT **************************************/
                if (req == Message.MsgRequest.COUNTER_ETR_DATA)
                {
                    avrgCircBuffer();
                    label_cnt_etr_value.ForeColor = SystemColors.ControlText;                    
                    label_cnt_etr_value.Text = String.Format("{0:n6}", cntPaint);                    

                    if(cntPaint != 0)
                    {
                        uint prescaler = 1;
                        if (cntPaint < timEtrPeriphClock / 4)
                        {
                            prescaler = 1;                            
                        }
                        else if ((cntPaint >= timEtrPeriphClock / 4) && (cntPaint < timEtrPeriphClock / 2))
                        {
                            prescaler = 2;                            
                        }
                        else if ((cntPaint >= timEtrPeriphClock / 2) && (cntPaint < timEtrPeriphClock))
                        {
                            prescaler = 4;                            
                        }
                        else if ((cntPaint >= timEtrPeriphClock) && (cntPaint < timEtrPeriphClock * 2))
                        {
                            prescaler = 8;                            
                        }

                        label_cnt_etr_prescaler.Text = prescaler.ToString();

                        /* Nucleo On-board crystal 20 ppm accuracy, Higher the frequency - Quantiz. error masked by Time Base error (and vice versa) */
                        /* Accuracy:
                        Relative Frequency Quantization Error = 1 / freq (-+ 1), but if ETRP input prescaler is set, then: prescaler / freq
                        Time Base Error = (ppm / 1000000) * freq */

                        label_cnt_etr_period.Text = String.Format("{0:0.##e-00}", 1 / cntPaint);
                        double relQuantError = (prescaler * (double)(1000 / getRadioNumber())) / cntPaint;
                        double timBaseError = (double)((20 * cntPaint) / 1000000);
                        double freqError = relQuantError + timBaseError;
                        label_cnt_etr_acc_freq.Text = String.Format("∓ {0:n6}", freqError);
                        label_cnt_etr_acc_period.Text = String.Format("∓ {0:0.##e-00}", (freqError / (cntPaint * cntPaint)));
                        label_cnt_etr_acc_average.Text = String.Format("∓ {0:n6}", relQuantError / cnt_etr_trackBar.Value + timBaseError);                    
                    }
                    else
                    {
                        label_cnt_etr_period.Text = "0.000000";
                        label_cnt_etr_acc_freq.Text = "∓ 0.000000";
                        label_cnt_etr_acc_period.Text = "∓ 0.000000";
                    }

                    etrPainting();
                    com_etr_indication();                  
                }
                /************************************** IC1 DATA PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC1_DATA)
                {
                    if (cntPaint > 10000000)
                    {
                        label_cnt_ic1_value.ForeColor = SystemColors.WindowFrame;
                        label_cnt_ic1_value.Font = new Font("Times New Roman", 13);
                        label_cnt_ic1_value.Text = "Frequency too high. Select\nHigh Frequency measurement tab.";
                    }
                    /* Hysterezis */
                    //else if ((cntPaint < 12000000) && (cntPaint > 9800000) && (label_cnt_ic1_value.Text == "Frequency > 10 MHz"))
                    //{
                    //    label_cnt_ic1_value.Text = "Frequency > 10 MHz";
                    //}
                    else
                    {
                        if (cntPaint != 0)
                        {
                            label_cnt_ic1_value.ForeColor = Color.Black;
                            label_cnt_ic1_value.Font = new Font("Times New Roman", 21.75F);
                            label_cnt_ic1_value.Text = String.Format("{0:n9}", cntPaint);
                            label_cnt_ic1_period.Text = (cntPaint != 0) ? String.Format("{0:n9}", 1 / cntPaint) : "0.000000";
                        }
                        com_ic1_indication();
                    }
                }
                /************************************** IC2 DATA PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC2_DATA)
                {
                    if (cntPaint > 10000000)
                    {
                        label_cnt_ic2_value.ForeColor = SystemColors.WindowFrame;
                        label_cnt_ic2_value.Font = new Font("Times New Roman", 13);
                        label_cnt_ic2_value.Text = "Frequency too high. Select\nHigh Frequency measurement tab.";
                    }
                    /* Hysterezis */
                    //else if ((cntPaint < 12000000) && (cntPaint > 9800000) && (label_cnt_ic2_value.Text == "Frequency > 10 MHz"))
                    //{
                    //    label_cnt_ic2_value.Text = "Frequency > 10 MHz";
                    //}
                    else
                    {
                        if (cntPaint != 0)
                        {
                            label_cnt_ic2_value.ForeColor = Color.Black;
                            label_cnt_ic2_value.Font = new Font("Times New Roman", 21.75F);
                            label_cnt_ic2_value.Text = String.Format("{0:n9}", cntPaint);
                            label_cnt_ic2_period.Text = (cntPaint != 0) ? String.Format("{0:n9}", 1 / cntPaint) : "0.000000";
                        }
                        com_ic2_indication();
                    }
                }
                /************************************** IC1 DUTY CYCLE PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC1_DUTY_CYCLE)
                {
                    label_cnt_ic_dutyCycle.Text = cntPaint.ToString("F3");
                    com_ic_dutyCycle_indication();
                }
                /************************************** IC1 PULSE WIDTH PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC1_PULSE_WIDTH)
                {
                    label_cnt_ic_pulseWidth.Text = cntPaint.ToString("F12");
                    com_ic_pulseWidth_indication();
                }
                /************************************** REF DATA PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_REF_DATA)
                {
                    label_cnt_ref_value.ForeColor = SystemColors.ControlText;
                    label_cnt_ref_value.Font = new Font("Times New Roman", 21.75F);
                    label_cnt_ref_value.Text = String.Format("{0:n10}", (refSamples / cntPaint));

                    com_ref_indication();
                }
                /************************************** REF WAIT PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_REF_WAIT)
                {
                    label_cnt_ref_value.ForeColor = SystemColors.WindowFrame;
                    label_cnt_ref_value.Font = new Font("Times New Roman", 14);
                    label_cnt_ref_value.Text = "Sampling!";
                }
                /************************************** REF WARNING PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_REF_WARN)
                {
                    label_cnt_ref_value.ForeColor = SystemColors.WindowFrame;
                    label_cnt_ref_value.Font = new Font("Times New Roman", 14);
                    label_cnt_ref_value.Text = "Sample count too low!";
                }
                /************************************** TI TIMEOUT PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_TI_TIMEOUT)
                {
                    label_cnt_it_value.ForeColor = SystemColors.WindowFrame;
                    label_cnt_it_value.Font = new Font("Times New Roman", 13.4F);
                    label_cnt_it_value.Text = "Timeout occurred!";
                    tiEnableGroupBoxes();
                    com_ti_indication();
                }
                /************************************** TI EQUAL PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_TI_EQUAL)
                {
                    label_cnt_it_value.ForeColor = SystemColors.WindowFrame;
                    label_cnt_it_value.Font = new Font("Times New Roman", 13);
                    label_cnt_it_value.Text = "Delay too short to differentiate!";
                    tiEnableGroupBoxes();
                    com_ti_indication();
                }
                /************************************** TI BUFF1 BIGGER PRINT **************************************/
                /* Buffer 1 is bigger - meaning, the event A on channel 1 occurred after event B on channel 2 (B came first) */
                else if (req == Message.MsgRequest.COUNTER_TI_BIGGER_BUF1)
                {
                    if (radioButton_it_eventSequence_ba.Checked == true)
                    {
                        label_cnt_it_value.ForeColor = Color.Black;
                        label_cnt_it_value.Font = new Font("Times New Roman", 21.75F);
                        label_cnt_it_value.Text = String.Format("{0:n12}", cntPaint);
                    }
                    else
                    {
                        label_cnt_it_value.ForeColor = SystemColors.WindowFrame;
                        label_cnt_it_value.Font = new Font("Times New Roman", 12);
                        label_cnt_it_value.Text = "Event B on channel 2 occurred first.\nData does not match the request.\nCheck Tba or swap wires.";
                    }
                    tiEnableGroupBoxes();
                    com_ti_indication();
                }
                /************************************** TI BUFF2 BIGGER PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_TI_BIGGER_BUF2)
                {
                    if (radioButton_it_eventSequence_ab.Checked == true)
                    {
                        label_cnt_it_value.ForeColor = Color.Black;
                        label_cnt_it_value.Font = new Font("Times New Roman", 21.75F);
                        label_cnt_it_value.Text = String.Format("{0:n12}", cntPaint);
                    }
                    else
                    {
                        label_cnt_it_value.ForeColor = SystemColors.WindowFrame;
                        label_cnt_it_value.Font = new Font("Times New Roman", 12);
                        label_cnt_it_value.Text = "Event A on channel 1 occurred first.\nData does not match the request.\nCheck Tab or swap wires.";
                    }
                    tiEnableGroupBoxes();
                    com_ti_indication();
                }


                base.OnPaint(e);
                req = Message.MsgRequest.NULL_MESSAGE;
            }
            catch (Exception ex)
            {
                this.Close();
                throw new System.ArgumentException("Counter painting went wrong");
            }

        }

        private void tiEnableGroupBoxes()
        {
            button_cnt_ti_enable.Text = "Start";
            groupBox_ti_event_A.Enabled = true;
            groupBox_ti_event_B.Enabled = true;
            groupBox_ti_eventSeq.Enabled = true;
            groupBox_ti_timeout.Enabled = true;
        }

        /************************************** ETR DATA PRINT **************************************/
        private void etrPainting()
        {
            ushort gateTime = getRadioNumber();
            Int16 trackBar = (Int16)cnt_etr_trackBar.Value;

            if (((avrgList.Count < trackBar) && (trackBar != 2)) && (keyPress == KEY_PRESS.YES))
            {
                label_cnt_etr_avrg.ForeColor = SystemColors.WindowFrame;
                label_cnt_etr_avrg.Font = new Font("Times New Roman", 15);

                Int32 difference = cnt_etr_trackBar.Value - (int)(avrgList.Count);
                if ((gateTime != 5000) && (gateTime != 10000))
                {
                    label_cnt_etr_avrg.Text = "Wait " + ((difference * gateTime / 1000) + 1).ToString() + " seconds.";
                }
                else
                {
                    label_cnt_etr_avrg.Text = "Wait " + (difference * gateTime / 1000).ToString() + " seconds.";
                }
            }
            else
            {
                label_cnt_etr_avrg.ForeColor = SystemColors.ControlText;
                label_cnt_etr_avrg.Font = new Font("Times New Roman", 21.75F);                
                label_cnt_etr_avrg.Text = String.Format("{0:n6}", average);
                keyPress = KEY_PRESS.NO;
            }
        }

        public void com_etr_indication()
        {
            cnt_etr_indication.BackColor = (cnt_etr_indication.BackColor == Color.Teal) ? Color.Maroon : Color.Teal;
        }

        public void com_ic1_indication()
        {
            cnt_ic_indication_ch1.BackColor = (cnt_ic_indication_ch1.BackColor == Color.Teal) ? Color.Maroon : Color.Teal;
        }

        public void com_ic_dutyCycle_indication()
        {
            cnt_ic_pulse_indic_ch1.BackColor = (cnt_ic_pulse_indic_ch1.BackColor == Color.Teal) ? Color.Maroon : Color.Teal;
        }

        public void com_ic_pulseWidth_indication()
        {
            cnt_ic_pulse_indic_ch2.BackColor = (cnt_ic_pulse_indic_ch2.BackColor == Color.Teal) ? Color.Maroon : Color.Teal;
        }

        public void com_ic2_indication()
        {
            cnt_ic_indication_ch2.BackColor = (cnt_ic_indication_ch2.BackColor == Color.Teal) ? Color.Maroon : Color.Teal;
        }

        public void com_ref_indication()
        {
            cnt_ref_indication.BackColor = (cnt_ref_indication.BackColor == Color.Teal) ? Color.Maroon : Color.Teal;
        }

        public void com_ti_indication()
        {
            cnt_ti_indication.BackColor = (cnt_ti_indication.BackColor == Color.Teal) ? Color.Maroon : Color.Teal;
        }

        private void exitCounterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void counter_FormClosing(object sender, FormClosingEventArgs e)
        {
            cnt_stop();
        }

        public void cnt_start()
        {
            device.takeCommsSemaphore(semaphoreTimeout + 102);
            device.send(Commands.COUNTER + ":" + Commands.START + ";");
            device.giveCommsSemaphore();
        }

        public void cnt_stop()
        {
            device.takeCommsSemaphore(semaphoreTimeout + 103);
            device.send(Commands.COUNTER + ":" + Commands.STOP + ";");
            device.giveCommsSemaphore();
        }

        public void cnt_deinit()
        {
            device.takeCommsSemaphore(semaphoreTimeout + 103);
            device.send(Commands.COUNTER + ":" + Commands.DEINIT + ";");
            device.giveCommsSemaphore();
        }

        public void cnt_init_mode(CNT_MODES mode)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 104);
            device.send(Commands.COUNTER + ":" + Commands.CNT_MODE + " ");
            if (mode == CNT_MODES.ETR)
            {
                device.send(Commands.CNT_ETR + ";");
            }
            else if (mode == CNT_MODES.IC)
            {
                device.send(Commands.CNT_IC + ";");
            }
            else if (mode == CNT_MODES.REF)
            {
                device.send(Commands.CNT_REF + ";");
            }
            else if (mode == CNT_MODES.TI)
            {
                device.send(Commands.CNT_TI + ";");
            }
            device.giveCommsSemaphore();
        }

        public void add_message(Message msg)
        {
            this.cnt_q.Enqueue(msg);
        }

        private void cnt_mode_tabControl_SelectedIndexchanged(object sender, EventArgs e)
        {
            /* Set IC values to default */
            if (counterMode == CNT_MODES.IC)
            {                
                icGuiReinit();
                restoreIcMode();
            }

            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:                  
                    cnt_stop();                    
                    cnt_init_mode(CNT_MODES.ETR);
                    counterMode = CNT_MODES.ETR;
                    cnt_start();
                    ETR_appReinit();                    
                    break;
                case 1:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.IC);
                    counterMode = CNT_MODES.IC;
                    cnt_start();
                    IC_appReinit();                    
                    break;
                case 2:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.REF);
                    counterMode = CNT_MODES.REF;
                    cnt_start();
                    REF_appReinit();                    
                    break;
                case 3:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.TI);
                    counterMode = CNT_MODES.TI;
                    TI_appReinit();                    
                    break;
            }
        }

        private void ETR_appReinit()
        {
            average = 0;
            avrgList.Clear();

            radioButton_100m.Checked = true;
            radioButton_100m_CheckedChanged(null, null);
            cnt_etr_trackBar.Value = 2;
            cnt_etr_trackBar_Scroll(null, null);
            cnt_etr_avrg_textBox.Text = "0";
            cnt_etr_avrg_textBox.Text = "2";
            label_cnt_etr_avrg.Text = "0.000000";
            label_cnt_etr_value.Text = "0.000000";
            label_cnt_etr_period.Text = "0.000000";
            label_cnt_etr_acc_freq.Text = "∓ 0.000000";
            label_cnt_etr_acc_average.Text = "∓ 0.000000";
            label_cnt_etr_acc_period.Text = "∓ 0.000000";
            label_cnt_etr_prescaler.Text = "1";
            label_cnt_etr_minFreq.Text = "10 Hz";
        }

        private void REF_appReinit()
        {

            cnt_ref_sampleCount_textBox.Text = "10 000 000";
            cnt_ref_trackBar1.Value = 10000;
            cnt_ref_trackBar2.Value = 1000;
            cnt_ref_count_textBox1.Text = "10000";
            cnt_ref_count_textBox2.Text = "1000";
            refSamples = 10000000;
            label_cnt_ref_value.ForeColor = SystemColors.WindowFrame;
            label_cnt_ref_value.Font = new Font("Times New Roman", 14);
            label_cnt_ref_value.Text = "Change the sample count first.";            
            label_cnt_ref_acc.Text = "∓ 0.000000";

            cnt_etr_trackBar.Value = 2; 
        }

        private void IC_appReinit()
        {
            cnt_ic1_buffer_textBox.ForeColor = Color.Black;
            cnt_ic2_buffer_textBox.ForeColor = Color.Black;
            cnt_ic1_buffer_textBox.Text = "1";
            cnt_ic2_buffer_textBox.Text = "1";
            cnt_ic1_trackBar.Value = 1;
            cnt_ic2_trackBar.Value = 1;
            cnt_etr_trackBar.Value = 2;

            label_cnt_ic1_value.Text = "0.000000";
            label_cnt_ic2_value.Text = "0.000000";
            label_cnt_ic1_period.Text = "0.000000";
            label_cnt_ic2_period.Text = "0.000000";
            label_cnt_ic1_acc_freq.Text = "∓ 0.000000";
            label_cnt_ic2_acc_freq.Text = "∓ 0.000000";
            label_cnt_ic1_acc_period.Text = "∓ 0.000000";
            label_cnt_ic2_acc_period.Text = "∓ 0.000000";
            label_cnt_ic_acc_dutyCycle.Text = "∓ 0.000000";
            label_cnt_ic_acc_pulseWidth.Text = "∓ 0.000000";

            groupBox_cnt_ic1_freq.Visible = true;
            groupBox_cnt_ic1_period.Visible = true;
            groupBox_cnt_ic_dutyCycle.Visible = false;

            groupBox_cnt_ic2_freq.Visible = true;
            groupBox_cnt_ic2_period.Visible = true;
            groupBox_cnt_ic_pulseWidth.Visible = false;

            xToolStripMenu_icCh1_1x.Checked = true;
            xToolStripMenu_icCh2_1x.Checked = true;

            channel1ToolStripMenuItem.Visible = true;
            channel2ToolStripMenuItem.Visible = true;       
        }

        private void TI_appReinit()
        {
            button_cnt_ti_enable.Text = "Start";
            groupBox_ti_event_A.Enabled = true;
            groupBox_ti_event_B.Enabled = true;
            groupBox_ti_eventSeq.Enabled = true;
            groupBox_ti_timeout.Enabled = true;

            radioButton_it_rising_ch1.Checked = true;
            radioButton_it_rising_ch2.Checked = true;
            radioButton_it_eventSequence_ab.Checked = true;

            this.pictureBox_ti_sequence.Image = Properties.Resources.ab_rising_rising;

            trackBar_cnt_ti_timeout.Value = 500;
            textBox_cnt_ti_timeout.Text = "500";

            label_cnt_it_value.ForeColor = Color.Black;
            label_cnt_it_value.Font = new Font("Times New Roman", 21.75F);

            label_cnt_it_value.Text = "0.000000";
            label_cnt_ti_acc.Text = "∓ 0.000000";
        }

        private void icGuiReinit()
        {            
            cntIcPresc1 = 1;
            label_icCh1_min.Text = "1";
            label_icCh1_max.Text = "100";
            cnt_ic1_trackBar.Minimum = cntIcPresc1;
            cnt_ic1_trackBar.Maximum = cntIcPresc1 * 100;

            cntIcPresc2 = 1;
            label_icCh2_min.Text = "1";
            label_icCh2_max.Text = "100";
            cnt_ic2_trackBar.Minimum = cntIcPresc2;
            cnt_ic2_trackBar.Maximum = cntIcPresc2 * 100;            

            icMenuHideChannels();

            negateToolStrip1Check();
            negateToolStrip2Check();
        }
        
        public void icMenuHideChannels()
        {
            channel1ToolStripMenuItem.Visible = false;
            channel2ToolStripMenuItem.Visible = false;
        }

        /* Any counter mode tab can be selected when Duty Cycle measurement is still 
           selected, so have to be reconfigured back to IC frequency measurement mode. */
        public void restoreIcMode()
        {
            if(ic1DutyCycle == CNT_DUTY_CYCLE.DUTY_CYCLE_ENABLED)
            {
                checkBox_icMode_dutyCycle_ch1.Checked = false;
            }
            else
            {
                dutyCycleDisable_ch1();                                     
            }

            if (ic2DutyCycle == CNT_DUTY_CYCLE.DUTY_CYCLE_ENABLED)
            {
                checkBox_icMode_dutyCycle_ch2.Checked = false;
            }
            else
            {
                dutyCycleDisable_ch2();
            }

            ic1DutyCycle = CNT_DUTY_CYCLE.DUTY_CYCLE_DISABLED;
            ic2DutyCycle = CNT_DUTY_CYCLE.DUTY_CYCLE_DISABLED;
        }

        public static void setRefPins(string[] pins)
        {
            refPins[0] = pins[3].Replace("_", "");
            refPins[1] = pins[4].Replace("_", "");
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* --------------------------------------------------- COUNTER IC1/IC2 EVENTS ----------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* Robusting textBox "IC1 buffer" */
        private void cnt_ic1_buffer_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ic1_buffer_textBox.ForeColor = Color.Black;

            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(cnt_ic1_buffer_textBox.Text))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;                    
                    int ic1buffer = Convert.ToInt32(textBox.Text);
                    ic1buffer = (int)Math.Round(ic1buffer / (double)cntIcPresc1) * cntIcPresc1;

                    if (ic1buffer < cntIcPresc1)
                    {
                        cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic1_buffer_textBox.Text = IC_MIN + cntIcPresc1;
                        this.ActiveControl = null;
                    }
                    else if (ic1buffer > cntIcPresc1 * 100)
                    {
                        cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic1_buffer_textBox.Text = IC_MAX + cntIcPresc1 * 100;
                        this.ActiveControl = null;
                    }
                    else
                    {
                        cnt_ic1_trackBar.Value = ic1buffer;
                        cnt_ic1_buffer_textBox.Text = ic1buffer.ToString();
                        device.takeCommsSemaphore(semaphoreTimeout + 120);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT_CH1 + " ");
                        device.send_short((ushort)(ic1buffer / cntIcPresc1));
                        device.send(";");
                        device.giveCommsSemaphore();
                        req = Message.MsgRequest.COUNTER_IC1_BUFF_WAIT;
                        this.Invalidate();
                    }
                }
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                cnt_ic1_buffer_textBox.Text = NUMS_ONLY;
                this.ActiveControl = null;
            }
        }

        private void cnt_ic2_buffer_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ic2_buffer_textBox.ForeColor = Color.Black;

            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(cnt_ic2_buffer_textBox.Text))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    int ic2buffer = Convert.ToInt32(textBox.Text);
                    ic2buffer = (int)Math.Round(ic2buffer / (double)cntIcPresc2) * cntIcPresc2;

                    if (ic2buffer < cntIcPresc2)
                    {
                        cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic2_buffer_textBox.Text = IC_MIN + cntIcPresc2;
                        this.ActiveControl = null;
                    }
                    else if (ic2buffer > cntIcPresc2 * 100)
                    {
                        cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic2_buffer_textBox.Text = IC_MAX + cntIcPresc2 * 100;
                        this.ActiveControl = null;
                    }
                    else
                    {                        
                        cnt_ic2_trackBar.Value = ic2buffer;
                        cnt_ic2_buffer_textBox.Text = ic2buffer.ToString();
                        device.takeCommsSemaphore(semaphoreTimeout + 121);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT_CH2 + " ");
                        device.send_short((ushort)ic2buffer /  cntIcPresc2);
                        device.send(";");
                        device.giveCommsSemaphore();
                        req = Message.MsgRequest.COUNTER_IC2_BUFF_WAIT;
                        this.Invalidate();
                    }
                }
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                cnt_ic2_buffer_textBox.Text = NUMS_ONLY;
                this.ActiveControl = null;
            }
        }

        private void cnt_ic1_buffer_textBox_Click(object sender, EventArgs e)
        {
            int boole;
            bool isNumeric = int.TryParse(cnt_ic1_buffer_textBox.Text, out boole);
            if (string.IsNullOrEmpty(cnt_ic1_buffer_textBox.Text) || !isNumeric)
            {
                cnt_ic1_buffer_textBox.Text = "";
            }
        }

        private void cnt_ic2_buffer_textBox_Click(object sender, EventArgs e)
        {
            int boole;
            bool isNumeric = int.TryParse(cnt_ic2_buffer_textBox.Text, out boole);
            if (string.IsNullOrEmpty(cnt_ic2_buffer_textBox.Text) || !isNumeric)
            {
                cnt_ic2_buffer_textBox.Text = "";
            }
        }

        private void cnt_ic1_buffer_textBox_Leave(object sender, EventArgs e)
        {
            int boole;
            int numero = 0;
            bool isNumeric = int.TryParse(cnt_ic1_buffer_textBox.Text, out boole);

            if (cnt_ic1_buffer_textBox.Text.Length == 0)
            {
                cnt_ic1_buffer_textBox.ForeColor = Color.Black;
                cnt_ic1_buffer_textBox.Text = cnt_ic1_trackBar.Value.ToString();
            }

            if (isNumeric)
            {
                numero = Convert.ToUInt16(cnt_ic1_buffer_textBox.Text);
                numero = (int)Math.Round(numero / (double)cntIcPresc1) * cntIcPresc1;

                if (numero < cntIcPresc1)
                {
                    cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                    cnt_ic1_buffer_textBox.Text = IC_MIN + cntIcPresc1;
                }
                else if (numero > cntIcPresc1 * 100)
                {
                    cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                    cnt_ic1_buffer_textBox.Text = IC_MAX + cntIcPresc1 * 100;
                }
                else if (!cnt_ic1_buffer_textBox.Text.Equals(cnt_ic1_trackBar.Value.ToString()))
                {
                    cnt_ic1_trackBar.Value = Convert.ToInt16(cnt_ic1_buffer_textBox.Text);
                    cnt_ic1_buffer_textBox.Text = numero.ToString();
                    device.takeCommsSemaphore(semaphoreTimeout + 120);
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT_CH1 + " ");
                    device.send_short((ushort)(cnt_ic1_trackBar.Value / cntIcPresc1));
                    device.send(";");
                    device.giveCommsSemaphore();
                }
            }
        }

        private void cnt_ic2_buffer_textBox_Leave(object sender, EventArgs e)
        {
            int boole;
            int numero = 0;
            bool isNumeric = int.TryParse(cnt_ic2_buffer_textBox.Text, out boole);

            if (cnt_ic2_buffer_textBox.Text.Length == 0)
            {
                cnt_ic2_buffer_textBox.ForeColor = Color.Black;
                cnt_ic2_buffer_textBox.Text = cnt_ic2_trackBar.Value.ToString();
            }

            if (isNumeric)
            {
                numero = Convert.ToUInt16(cnt_ic2_buffer_textBox.Text);
                numero = (int)Math.Round(numero / (double)cntIcPresc2) * cntIcPresc2;

                if (numero < cntIcPresc2)
                {
                    cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                    cnt_ic2_buffer_textBox.Text = IC_MIN + cntIcPresc2;
                }
                else if (numero > cntIcPresc2 * 100)
                {
                    cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                    cnt_ic2_buffer_textBox.Text = IC_MAX + cntIcPresc2 * 100;
                }
                else if (!cnt_ic2_buffer_textBox.Text.Equals(cnt_ic2_trackBar.Value.ToString()))
                {
                    cnt_ic2_trackBar.Value = Convert.ToInt16(cnt_ic2_buffer_textBox.Text);
                    cnt_ic2_buffer_textBox.Text = numero.ToString();
                    device.takeCommsSemaphore(semaphoreTimeout + 120);
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT_CH2 + " ");
                    device.send_short((ushort)(cnt_ic2_trackBar.Value / cntIcPresc2));
                    device.send(";");
                    device.giveCommsSemaphore();
                }
            }
        }

        private void cnt_ic2_buffer_textBox_Enter(object sender, EventArgs e)
        {
            if (cnt_ic2_buffer_textBox.Text == NUM_ENTER)
            {
                cnt_ic2_buffer_textBox.Text = "";
                cnt_ic2_buffer_textBox.ForeColor = Color.Black;
            }
        }

        private void cnt_ic1_buffer_textBox_Enter(object sender, EventArgs e)
        {
            if (cnt_ic1_buffer_textBox.Text == NUM_ENTER)
            {
                cnt_ic1_buffer_textBox.Text = "";
                cnt_ic1_buffer_textBox.ForeColor = Color.Black;
            }
        }

        private void cnt_ic1_trackBar_Scroll(object sender, EventArgs e)
        {
            cnt_ic1_buffer_textBox.ForeColor = Color.Black;

            /* Set trackBar step due to Input Capture prescaler */
            if (trackIc1Rec)
            {
                return;
            }            
            trackVal1 = cnt_ic1_trackBar.Value;
            if (trackVal1 % cntIcPresc1 != 0)
            {
                trackVal1 = (trackVal1 / cntIcPresc1) * cntIcPresc1;                
                trackIc1Rec = true;
                cnt_ic1_trackBar.Value = trackVal1;
                trackIc1Rec = false;
            }

            cnt_ic1_buffer_textBox.Text = cnt_ic1_trackBar.Value.ToString();

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_IC1;
        }

        private void cnt_ic2_trackBar_Scroll(object sender, EventArgs e)
        {
            cnt_ic2_buffer_textBox.ForeColor = Color.Black;

            /* Set trackBar step due to Input Capture prescaler */
            if (trackIc2Rec)
            {
                return;
            }
            trackVal2 = cnt_ic2_trackBar.Value;
            if (trackVal2 % cntIcPresc2 != 0)
            {
                trackVal2 = (trackVal2 / cntIcPresc2) * cntIcPresc2;
                trackIc2Rec = true;
                cnt_ic2_trackBar.Value = trackVal2;
                trackIc2Rec = false;
            }

            cnt_ic2_buffer_textBox.Text = cnt_ic2_trackBar.Value.ToString();

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_IC2;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ---------------------------------------------- COUNTER IC1 MENU STRIPS HANDLERS ------------------------------------------------ */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void xToolStripMenu_icCh1_1x_Click(object sender, EventArgs e)
        {
            cntIcPresc1 = 1;
            negateToolStrip1Check();
            xToolStripMenu_icCh1_1x.Checked = true;
            setIc1GuiConfiguration();            
            sendIc1SampleCount(1);
            sendIc1Multiplier(Commands.CNT_IC_PSC_1x); // division of CCR (capture compare reg.) value counted
        }

        private void xToolStripMenu_icCh1_2x_Click(object sender, EventArgs e)
        {
            cntIcPresc1 = 2;
            negateToolStrip1Check();
            xToolStripMenu_icCh1_2x.Checked = true;
            setIc1GuiConfiguration();            
            sendIc1SampleCount(1);
            sendIc1Multiplier(Commands.CNT_IC_PSC_2x);
        }

        private void xToolStripMenu_icCh1_4x_Click(object sender, EventArgs e)
        {            
            cntIcPresc1 = 4;
            negateToolStrip1Check();
            xToolStripMenu_icCh1_4x.Checked = true;
            setIc1GuiConfiguration();            
            sendIc1SampleCount(1);
            sendIc1Multiplier(Commands.CNT_IC_PSC_4x);
        }

        private void xToolStripMenu_icCh1_8x_Click(object sender, EventArgs e)
        {
            cntIcPresc1 = 8;
            negateToolStrip1Check();
            xToolStripMenu_icCh1_8x.Checked = true;
            setIc1GuiConfiguration();            
            sendIc1SampleCount(1);
            sendIc1Multiplier(Commands.CNT_IC_PSC_8x);
        }

        void sendIc1Multiplier(string com)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 121);
            device.send(Commands.COUNTER + ":" + Commands.CNT_IC_PSC_CH1 + " ");
            device.send(com + ";");
            device.giveCommsSemaphore();
        }

        void sendIc1SampleCount(ushort sampleCount)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 101);
            device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT_CH1 + " ");
            device.send_short(sampleCount);
            device.send(";");
            device.giveCommsSemaphore();                        
        }

        private void setIc1GuiConfiguration()
        {
            string prescStringMin = cntIcPresc1.ToString();
            string prescStringMax = (cntIcPresc1 * 100).ToString();            

            cnt_ic1_trackBar.Minimum = cntIcPresc1;
            cnt_ic1_trackBar.Maximum = cntIcPresc1 * 100;
            label_icCh1_min.Text = prescStringMin;
            label_icCh1_max.Text = prescStringMax;

            cnt_ic1_buffer_textBox.Text = prescStringMin;
            cnt_ic1_trackBar.Value = cntIcPresc1;
        }

        private void negateToolStrip1Check()
        {
            xToolStripMenu_icCh1_1x.Checked = false;
            xToolStripMenu_icCh1_2x.Checked = false;
            xToolStripMenu_icCh1_4x.Checked = false;
            xToolStripMenu_icCh1_8x.Checked = false;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ---------------------------------------------- COUNTER IC2 MENU STRIPS HANDLERS ------------------------------------------------ */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void xToolStripMenu_icCh2_1x_Click(object sender, EventArgs e)
        {
            cntIcPresc2 = 1;
            negateToolStrip2Check();
            xToolStripMenu_icCh2_1x.Checked = true;
            setIc2GuiConfiguration();
            sendIc2Multiplier(Commands.CNT_IC_PSC_1x);
            sendIc2SampleCount(1);
        }

        private void xToolStripMenu_icCh2_2x_Click(object sender, EventArgs e)
        {
            cntIcPresc2 = 2;
            negateToolStrip2Check();
            xToolStripMenu_icCh2_2x.Checked = true;
            setIc2GuiConfiguration();
            sendIc2Multiplier(Commands.CNT_IC_PSC_2x);
            sendIc2SampleCount(1);
        }

        private void xToolStripMenu_icCh2_4x_Click(object sender, EventArgs e)
        {
            cntIcPresc2 = 4;
            negateToolStrip2Check();
            xToolStripMenu_icCh2_4x.Checked = true;
            setIc2GuiConfiguration();
            sendIc2Multiplier(Commands.CNT_IC_PSC_4x);
            sendIc2SampleCount(1);
        }

        private void xToolStripMenu_icCh2_8x_Click(object sender, EventArgs e)
        {
            cntIcPresc2 = 8;
            negateToolStrip2Check();
            xToolStripMenu_icCh2_8x.Checked = true;
            setIc2GuiConfiguration();
            sendIc2Multiplier(Commands.CNT_IC_PSC_8x);
            sendIc2SampleCount(1);
        }

        void sendIc2Multiplier(string com)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 121);
            device.send(Commands.COUNTER + ":" + Commands.CNT_IC_PSC_CH2 + " ");
            device.send(com + ";");
            device.giveCommsSemaphore();
        }

        void sendIc2SampleCount(ushort sampleCount)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 121);
            device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT_CH2 + " ");
            device.send_short(sampleCount);
            device.send(";");
            device.giveCommsSemaphore();
        }

        private void setIc2GuiConfiguration()
        {
            string prescStringMin = cntIcPresc2.ToString();
            string prescStringMax = (cntIcPresc2 * 100).ToString();

            cnt_ic2_trackBar.Minimum = cntIcPresc2;
            cnt_ic2_trackBar.Maximum = cntIcPresc2 * 100;
            label_icCh2_min.Text = prescStringMin;
            label_icCh2_max.Text = prescStringMax;

            cnt_ic2_buffer_textBox.Text = prescStringMin;
            cnt_ic2_trackBar.Value = cntIcPresc2;
        }

        private void negateToolStrip2Check()
        {
            xToolStripMenu_icCh2_1x.Checked = false;
            xToolStripMenu_icCh2_2x.Checked = false;
            xToolStripMenu_icCh2_4x.Checked = false;
            xToolStripMenu_icCh2_8x.Checked = false;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ------------------------------------------------ COUNTER IC1/IC2 CHECK BOXES --------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void checkBox_icMode_dutyCycle_ch1_CheckedChanged(object sender, EventArgs e)
        {
            bool checkCheck = (checkBox_icMode_dutyCycle_ch1.Checked == true) ? false : true;

            checkCheckDutyCycle_ch1(checkCheck);

            /* Duty Cycle disable : restore the previous configuration */
            if (checkCheck)
            {
                dutyCycleDisable_ch1();
            }
            /* Duty Cycle enable : set duty cycle measuring mode configuration */
            else
            {
                cnt_stop();
                dutyCycleEnable_ch1();
            }
        }

        private void checkCheckDutyCycle_ch1(bool checkCheck)
        {
            groupBox_icSampleCount1.Enabled = checkCheck;
            cnt_ic1_trackBar.Enabled = checkCheck;
            cnt_ic1_buffer_textBox.Enabled = checkCheck;
            label_icCh1_min.Enabled = checkCheck;
            label_icCh1_max.Enabled = checkCheck;

            groupBox_icSampleCount2.Enabled = checkCheck;
            cnt_ic2_trackBar.Enabled = checkCheck;
            cnt_ic2_buffer_textBox.Enabled = checkCheck;
            label_icCh2_min.Enabled = checkCheck;
            label_icCh2_max.Enabled = checkCheck;

            groupBox_cnt_ic_dutyCycle_ch2.Enabled = checkCheck;

            channel1ToolStripMenuItem.Enabled = checkCheck;
            channel2ToolStripMenuItem.Enabled = checkCheck;

            //signalDividersToolStripMenuItem_ch1.Enabled = checkCheck;
            //signalDividersToolStripMenuItem_ch2.Enabled = checkCheck;
        }

        private void dutyCycleEnable_ch1()
        {
            /* Show duty cycle and pulse width measurement groupBox */
            groupBox_cnt_ic_dutyCycle.Visible = true;
            groupBox_cnt_ic_pulseWidth.Visible = true;
            label_dutyCycle_groupBoxName.Text = "Duty cycle channel 1 [%]";
            label_pulseWidth_groupBoxName.Text = "Pulse width channel 1 [s]";
            label_cnt_ic_dutyCycle.Text = "0.000";
            label_cnt_ic_pulseWidth.Text = "0.000000";
            label_cnt_ic_acc_dutyCycle.Text = "∓ 0.000000";
            label_cnt_ic_acc_pulseWidth.Text = "∓ 0.000000";

            /* Initialize duty cycle and pulse width measurement  */
            sendCounterCommand(Commands.CNT_DUTY_CYCLE, Commands.CNT_DUTY_CYCLE_INIT_CH1);
            /* Initiate measuring */
            sendCounterCommand(Commands.CNT_DUTY_CYCLE, Commands.CNT_DUTY_CYCLE_ENABLE);

            ic1DutyCycle = CNT_DUTY_CYCLE.DUTY_CYCLE_ENABLED;
        }

        private void dutyCycleDisable_ch1()
        {
            /* Initiate measuring */
            sendCounterCommand(Commands.CNT_DUTY_CYCLE, Commands.CNT_DUTY_CYCLE_DISABLE);

            /* Set the previous value of IC prescaler */
            switch (cntIcPresc1)
            {
                case 1:
                    sendIc1Multiplier(Commands.CNT_IC_PSC_1x);
                    break;
                case 2:
                    sendIc1Multiplier(Commands.CNT_IC_PSC_2x);
                    break;
                case 4:
                    sendIc1Multiplier(Commands.CNT_IC_PSC_4x);
                    break;
                case 8:
                    sendIc1Multiplier(Commands.CNT_IC_PSC_8x);
                    break;
            }

            /* Set the previous sample count */
            sendIc1SampleCount((ushort)(cnt_ic1_trackBar.Value / cntIcPresc1));            
            sendIc2SampleCount((ushort)(cnt_ic2_trackBar.Value / cntIcPresc2));

            /* Deinitialize duty cycle and pulse width measurement  */
            sendCounterCommand(Commands.CNT_DUTY_CYCLE, Commands.CNT_DUTY_CYCLE_DEINIT_CH1);

            /* Hide duty cycle and pulse width measurement groupBox */
            groupBox_cnt_ic_dutyCycle.Visible = false;
            groupBox_cnt_ic_pulseWidth.Visible = false;

            ic1DutyCycle = CNT_DUTY_CYCLE.DUTY_CYCLE_DISABLED;
        }

        private void checkBox_icMode_dutyCycle_ch2_CheckedChanged(object sender, EventArgs e)
        {
            bool checkCheck = (checkBox_icMode_dutyCycle_ch2.Checked == true) ? false : true;

            checkCheckDutyCycle_ch2(checkCheck);

            /* Duty Cycle disable : restore the previous configuration */
            if (checkCheck)
            {
                dutyCycleDisable_ch2();
            }
            /* Duty Cycle enable : set duty cycle measuring mode configuration */
            else
            {
                cnt_stop();
                dutyCycleEnable_ch2();
            }
        }

        private void checkCheckDutyCycle_ch2(bool checkCheck)
        {
            groupBox_icSampleCount1.Enabled = checkCheck;
            cnt_ic1_trackBar.Enabled = checkCheck;
            cnt_ic1_buffer_textBox.Enabled = checkCheck;
            label_icCh1_min.Enabled = checkCheck;
            label_icCh1_max.Enabled = checkCheck;

            groupBox_icSampleCount2.Enabled = checkCheck;
            cnt_ic2_trackBar.Enabled = checkCheck;
            cnt_ic2_buffer_textBox.Enabled = checkCheck;
            label_icCh2_min.Enabled = checkCheck;
            label_icCh2_max.Enabled = checkCheck;

            groupBox_cnt_ic_dutyCycle_ch1.Enabled = checkCheck;

            channel1ToolStripMenuItem.Enabled = checkCheck;
            channel2ToolStripMenuItem.Enabled = checkCheck;

            //signalDividersToolStripMenuItem_ch1.Enabled = checkCheck;
            //signalDividersToolStripMenuItem_ch2.Enabled = checkCheck;
        }

        private void dutyCycleEnable_ch2()
        {
            /* Show duty cycle and pulse width measurement groupBox */
            groupBox_cnt_ic_dutyCycle.Visible = true;
            groupBox_cnt_ic_pulseWidth.Visible = true;
            label_dutyCycle_groupBoxName.Text = "Duty cycle channel 2 [%]";
            label_pulseWidth_groupBoxName.Text = "Pulse width channel 2 [s]";
            label_cnt_ic_dutyCycle.Text = "0.000";
            label_cnt_ic_pulseWidth.Text = "0.000000";
            label_cnt_ic_acc_dutyCycle.Text = "∓ 0.000000";
            label_cnt_ic_acc_pulseWidth.Text = "∓ 0.000000";

            /* Initialize duty cycle and pulse width measurement  */
            sendCounterCommand(Commands.CNT_DUTY_CYCLE, Commands.CNT_DUTY_CYCLE_INIT_CH2);            
            /* Initiate measuring */
            sendCounterCommand(Commands.CNT_DUTY_CYCLE, Commands.CNT_DUTY_CYCLE_ENABLE);

            ic2DutyCycle = CNT_DUTY_CYCLE.DUTY_CYCLE_ENABLED;
        }

        private void dutyCycleDisable_ch2()
        {
            /* Initiate measuring */
            sendCounterCommand(Commands.CNT_DUTY_CYCLE, Commands.CNT_DUTY_CYCLE_DISABLE);

            /* Set the previous value of IC prescaler */
            switch (cntIcPresc2)
            {
                case 1:
                    sendIc2Multiplier(Commands.CNT_IC_PSC_1x);
                    break;
                case 2:
                    sendIc2Multiplier(Commands.CNT_IC_PSC_2x);
                    break;
                case 4:
                    sendIc2Multiplier(Commands.CNT_IC_PSC_4x);
                    break;
                case 8:
                    sendIc2Multiplier(Commands.CNT_IC_PSC_8x);
                    break;
            }

            /* Set the previous sample count */
            sendIc1SampleCount((ushort)(cnt_ic1_trackBar.Value / cntIcPresc1));
            sendIc2SampleCount((ushort)(cnt_ic2_trackBar.Value / cntIcPresc2));

            /* Deinitialize duty cycle and pulse width measurement  */
            sendCounterCommand(Commands.CNT_DUTY_CYCLE, Commands.CNT_DUTY_CYCLE_DEINIT_CH2);

            /* Hide duty cycle and pulse width measurement groupBox */
            groupBox_cnt_ic_dutyCycle.Visible = false;
            groupBox_cnt_ic_pulseWidth.Visible = false;

            ic2DutyCycle = CNT_DUTY_CYCLE.DUTY_CYCLE_DISABLED;
        }

        public void sendCounterCommand(string generalComm, string specificComm)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.COUNTER + ":" + generalComm + " ");
            device.send(specificComm + ";");
            device.giveCommsSemaphore();
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ----------------------------------------------------- COUNTER ETR EVENTS ------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void cnt_etr_avrg_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_etr_avrg_textBox.ForeColor = Color.Black;

            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(cnt_etr_avrg_textBox.Text))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    int etrbuffer = Convert.ToInt32(textBox.Text);

                    if (etrbuffer < 2)
                    {
                        cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                        cnt_etr_avrg_textBox.Text = ETR_MIN;
                        this.ActiveControl = null;
                    }
                    else if (etrbuffer > 200)
                    {
                        cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                        cnt_etr_avrg_textBox.Text = ETR_MAX;
                        this.ActiveControl = null;
                    }
                    else
                    {                        
                        cnt_etr_trackBar.Value = etrbuffer;
                        keyPress = KEY_PRESS.YES;
                        req = Message.MsgRequest.COUNTER_ETR_DATA;
                        this.Invalidate();
                    }
                }
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                cnt_etr_avrg_textBox.Text = NUMS_ONLY;
                this.ActiveControl = null;
            }
        }

        private void cnt_etr_avrg_textBox_Click(object sender, EventArgs e)
        {
            int boole;
            bool isNumeric = int.TryParse(cnt_etr_avrg_textBox.Text, out boole);
            if (string.IsNullOrEmpty(cnt_etr_avrg_textBox.Text) || !isNumeric)
            {
                cnt_etr_avrg_textBox.Text = "";
            }
        }

        private void cnt_etr_trackBar_Scroll(object sender, EventArgs e)
        {
            cnt_etr_avrg_textBox.ForeColor = Color.Black;
            cnt_etr_avrg_textBox.Text = "" + cnt_etr_trackBar.Value;
            keyPress = KEY_PRESS.YES;

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_ETR;
        }

        private void cnt_etr_avrg_textBox_Enter(object sender, EventArgs e)
        {
            if (cnt_etr_avrg_textBox.Text == NUM_ENTER)
            {
                cnt_etr_avrg_textBox.Text = "";
                cnt_etr_avrg_textBox.ForeColor = Color.Black;
            }
        }

        private void cnt_etr_avrg_textBox_Leave(object sender, EventArgs e)
        {
            int boole;
            UInt16 numero = 0;
            bool isNumeric = int.TryParse(cnt_etr_avrg_textBox.Text, out boole);

            if (cnt_etr_avrg_textBox.Text.Length == 0)
            {
                cnt_etr_avrg_textBox.ForeColor = Color.Black;
                cnt_etr_avrg_textBox.Text = cnt_etr_trackBar.Value.ToString();
            }

            if (isNumeric)
            {
                numero = Convert.ToUInt16(cnt_etr_avrg_textBox.Text);

                if (numero < 2)
                {
                    cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                    cnt_etr_avrg_textBox.Text = ETR_MIN;
                }
                else if (numero > 200)
                {
                    cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                    cnt_etr_avrg_textBox.Text = ETR_MAX;
                }
                else if (!cnt_etr_avrg_textBox.Text.Equals(cnt_etr_trackBar.Value.ToString()))
                {
                    cnt_etr_trackBar.Value = Convert.ToInt16(cnt_etr_avrg_textBox.Text);
                }
            }
        }

        private void radioButton_100m_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 108);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_100m + ";");
            device.giveCommsSemaphore();
            label_cnt_avrg_info.Text = "";
            label_cnt_etr_minFreq.Text = "10 Hz";
            keyPress = KEY_PRESS.YES;
            etrPainting();
        }

        private void radioButton_500m_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 107);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_500m + ";");
            device.giveCommsSemaphore();
            label_cnt_avrg_info.Text = "";
            label_cnt_etr_minFreq.Text = "2 Hz";
            keyPress = KEY_PRESS.YES;
            etrPainting();
        }

        private void radioButton_1s_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 105);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_1s + ";");
            device.giveCommsSemaphore();
            label_cnt_avrg_info.Text = "";
            label_cnt_etr_minFreq.Text = "1 Hz";
            keyPress = KEY_PRESS.YES;
            etrPainting();
        }

        private void radioButton_5s_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 108);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_5s + ";");
            device.giveCommsSemaphore();
            label_cnt_avrg_info.Text = "5 sec. update";
            label_cnt_etr_minFreq.Text = "200 mHz";
            keyPress = KEY_PRESS.YES;
            etrPainting();
        }

        private void radioButton_10s_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 106);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_10s + ";");
            device.giveCommsSemaphore();
            label_cnt_avrg_info.Text = "10 sec. update";
            label_cnt_etr_minFreq.Text = "100 mHz";
            keyPress = KEY_PRESS.YES;
            etrPainting();
        }


        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* -------------------------------------------------- COUNTER ETR FUNCTIONS ------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void avrgCircBuffer()
        {
            int circBuffSize = cnt_etr_trackBar.Value;
            circBuffSizeTest = circBuffSize;
            if (avrgList.Count == 0)
            {
                nodeCurrent = avrgList.AddFirst(cntPaint);
            }
            else if ((avrgList.Count < circBuffSize) && (avrgList.Count != 0))
            {
                nodeCurrent = avrgList.AddAfter(nodeCurrent, cntPaint);
            }
            /* If user changes previous buffer size to lower value -> delete all first nodes */
            else if (avrgList.Count > circBuffSize)
            {
                int toDelete = avrgList.Count - circBuffSize;
                for (int i = 0; i < toDelete; i++)
                {
                    avrgList.RemoveFirst();
                }
            }
            else if (avrgList.Count == circBuffSize)
            {
                avrgList.RemoveFirst();
                nodeCurrent = avrgList.AddAfter(nodeCurrent, cntPaint);
                average = avrgList.Sum() / circBuffSize;
            }
        }

        ushort getRadioNumber()
        {
            foreach (Control control in groupBox1.Controls)
            {
                if (control.GetType() == typeof(RadioButton))
                {
                    RadioButton radio = control as RadioButton;
                    if (radio.Checked)
                    {
                        switch (radio.Name)
                        {
                            case "radioButton_100m":
                                return 100;
                            case "radioButton_500m":
                                return 500;
                            case "radioButton_1s":
                                return 1000;
                            case "radioButton_5s":
                                return 5000;
                            case "radioButton_10s":
                                return 10000;
                        }
                    }
                }
            }
            return 0;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ---------------------------------------------------- COUNTER REF FUNCTIONS ----------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* Function splitting 32 bit number into two 16 bit values that represent ARR and PSC register values send to MCU timer */
        private void splitAndSend(UInt32 sampleCount)
        {
            UInt32 psc = 1, arr = 1;

            if (isPrimeNumber(sampleCount) && sampleCount > 64000)
            {
                cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                cnt_ref_sampleCount_textBox.Text = "A prime number > 64000";
                this.ActiveControl = null;
                return;
            }
            else if (sampleCount <= 64000)
            {
                arr = (ushort)sampleCount;
                psc = 1;
            }
            else
            {
                for (int pscTemp = 64000; pscTemp > 1; pscTemp--)
                {
                    if ((sampleCount % pscTemp) == 0 && pscTemp <= 64000)
                    {
                        psc = (ushort)pscTemp;
                        break;
                    }
                }

                arr = (sampleCount / psc);

                if (arr > 64000)
                {
                    cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                    cnt_ref_sampleCount_textBox.Text = "Can't be devided.";                    
                    this.ActiveControl = null;
                    return;
                }
            }

            if ((arr <= 64000) && (psc <= 64000) && (arr > 0) && (psc > 0))
            {

                device.takeCommsSemaphore(semaphoreTimeout + 121);

                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_PSC + " ");
                device.send_short((ushort)psc);
                device.send(";");

                //Thread.Sleep(120);

                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_ARR + " ");
                device.send_short((ushort)arr);
                device.send(";");

                device.giveCommsSemaphore();

                cnt_ref_count_textBox1.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                cnt_ref_count_textBox2.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                cnt_ref_count_textBox1.Text = arr.ToString();
                cnt_ref_count_textBox2.Text = psc.ToString();
                cnt_ref_trackBar1.Value = (int)arr;
                cnt_ref_trackBar2.Value = (int)psc;
            }
        }

        private static bool isPrimeNumber(UInt32 number)
        {
            if ((number & 1) == 0)
            {
                if (number == 2)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            for (int i = 3; (i * i) <= number; i = i + 2)
            {
                if ((number % i) == 0)
                {
                    return false;
                }
            }
            return true;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ------------------------------------------------ COUNTER ETR/REF/IC TIMER ------------------------------------------------------ */
        /* -------------------------------------------------------------------------------------------------------------------------------- */

        private void scrollTimerConfig()
        {
            scrollTimer = new System.Timers.Timer(100);
            scrollTimer.Elapsed += new ElapsedEventHandler(scrollTimeElapseEvent);
            scrollTimer.AutoReset = false;
        }

        private void scrollTimeElapseEvent(Object source, ElapsedEventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 121);

            switch (timScroll)
            {
                case CNT_TIMER.TIM_SCROLL_IC1:
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT_CH1 + " ");
                    device.send_short(Convert.ToUInt16(cnt_ic1_buffer_textBox.Text) / cntIcPresc1);
                    device.send(";");
                    req = Message.MsgRequest.COUNTER_IC1_BUFF_WAIT;
                    this.Invalidate();
                    break;
                case CNT_TIMER.TIM_SCROLL_IC2:
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT_CH2 + " ");
                    device.send_short(Convert.ToUInt16(cnt_ic2_buffer_textBox.Text) / cntIcPresc2);
                    device.send(";");
                    req = Message.MsgRequest.COUNTER_IC2_BUFF_WAIT;
                    this.Invalidate();
                    break;
                case CNT_TIMER.TIM_SCROLL_REF1:
                    scrollTimer.Stop();
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_ARR + " ");
                    device.send_short(Convert.ToUInt16(cnt_ref_count_textBox1.Text));
                    device.send(";");
                    req = Message.MsgRequest.COUNTER_REF_WAIT;
                    this.Invalidate();
                    break;
                case CNT_TIMER.TIM_SCROLL_REF2:
                    scrollTimer.Stop();
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_PSC + " ");
                    device.send_short(Convert.ToUInt16(cnt_ref_count_textBox2.Text));
                    device.send(";");
                    req = Message.MsgRequest.COUNTER_REF_WAIT;
                    this.Invalidate();
                    break;
                case CNT_TIMER.TIM_SCROLL_TI:
                    scrollTimer.Stop();
                    device.send(Commands.COUNTER + ":" + Commands.CNT_TI_TIMEOUT_VALUE + " ");
                    device.send_short(Convert.ToUInt16(textBox_cnt_ti_timeout.Text));
                    device.send(";");                                        
                    break;
                case CNT_TIMER.TIM_SCROLL_ETR:
                    req = Message.MsgRequest.COUNTER_ETR_DATA;
                    this.Invalidate();
                    break;
            }

            device.giveCommsSemaphore();
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ----------------------------------------------------- COUNTER REF EVENTS ------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void cnt_ref_count_textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ref_count_textBox1.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

            TextBox textBox = (TextBox)sender;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                cnt_ref_count_textBox1.Text = NUMS_ONLY;
                this.ActiveControl = null;
            }

            if (!string.IsNullOrEmpty(cnt_ref_count_textBox1.Text) && !(cnt_ref_count_textBox1.Text.Length > 10))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    UInt32 refSampleCount = Convert.ToUInt32(textBox.Text);

                    if (refSampleCount < 2)
                    {
                        cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                        cnt_ref_count_textBox1.Text = REF1_MIN;
                        this.ActiveControl = null;
                    }
                    else if (refSampleCount > 64000)
                    {
                        cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                        cnt_ref_count_textBox1.Text = REF1_MAX;
                        this.ActiveControl = null;
                    }
                    else
                    {
                        cnt_ref_trackBar1.Value = int.Parse(cnt_ref_count_textBox1.Text);
                        cnt_ref_sampleCount_textBox.ForeColor = Color.Black;

                        cnt_ref_count_textBox2.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                        cnt_ref_count_textBox2.Text = cnt_ref_trackBar2.Value.ToString();

                        refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
                        cnt_ref_sampleCount_textBox.Text = "" + refSamples;

                        device.takeCommsSemaphore(semaphoreTimeout + 121);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_ARR + " ");
                        device.send_short((ushort)cnt_ref_trackBar1.Value);
                        device.send(";");
                        device.giveCommsSemaphore();

                        req = Message.MsgRequest.COUNTER_REF_WAIT;
                        this.Invalidate();
                    }
                }
            }
        }

        private void cnt_ref_count_textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ref_count_textBox2.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));

            TextBox textBox = (TextBox)sender;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_count_textBox2.ForeColor = Color.DarkRed;
                cnt_ref_count_textBox2.Text = NUMS_ONLY;
                this.ActiveControl = null;
            }

            if (!string.IsNullOrEmpty(cnt_ref_count_textBox2.Text) && !(cnt_ref_count_textBox2.Text.Length > 10))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    UInt32 refSampleCount = Convert.ToUInt32(textBox.Text);

                    if (refSampleCount < 1)
                    {
                        cnt_ref_count_textBox2.ForeColor = Color.DarkRed;
                        cnt_ref_count_textBox2.Text = REF2_MIN;
                        this.ActiveControl = null;
                    }
                    else if (refSampleCount > 64000)
                    {
                        cnt_ref_count_textBox2.ForeColor = Color.DarkRed;
                        cnt_ref_count_textBox2.Text = REF2_MAX;
                        this.ActiveControl = null;
                    }
                    else
                    {
                        cnt_ref_trackBar2.Value = int.Parse(cnt_ref_count_textBox2.Text);
                        cnt_ref_sampleCount_textBox.ForeColor = Color.Black;

                        cnt_ref_count_textBox1.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                        cnt_ref_count_textBox1.Text = cnt_ref_trackBar1.Value.ToString();

                        refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);                        
                        cnt_ref_sampleCount_textBox.Text = "" + refSamples;

                        device.takeCommsSemaphore(semaphoreTimeout + 121);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_PSC + " ");
                        device.send_short((ushort)cnt_ref_trackBar2.Value);
                        device.send(";");
                        device.giveCommsSemaphore();

                        req = Message.MsgRequest.COUNTER_REF_WAIT;
                        this.Invalidate();
                    }
                }
            }
        }

        private void cnt_ref_sampleCount_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ref_sampleCount_textBox.ForeColor = Color.Black;

            TextBox textBox = (TextBox)sender;

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                cnt_ref_sampleCount_textBox.Text = NUMS_ONLY;
                this.ActiveControl = null;
            }

            if (!string.IsNullOrEmpty(cnt_ref_sampleCount_textBox.Text) && !(cnt_ref_sampleCount_textBox.Text.Length > 10))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    UInt64 refSampleCount = Convert.ToUInt64(textBox.Text);

                    if (refSampleCount < 2)
                    {
                        cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                        cnt_ref_sampleCount_textBox.Text = REF_MIN;
                        this.ActiveControl = null;
                    }
                    else if (refSampleCount > 4096000000)
                    {
                        cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                        cnt_ref_sampleCount_textBox.Text = REF_MAX;
                        this.ActiveControl = null;
                    }
                    else
                    {
                        cnt_ref_sampleCount_textBox.ForeColor = Color.Black;
                        splitAndSend((UInt32)refSampleCount);
                        if (!cnt_ref_sampleCount_textBox.Text.Equals("Can't be devided.") && !cnt_ref_sampleCount_textBox.Text.Equals("A prime number > 64000"))
                        {                            
                            refSamples = (UInt32)refSampleCount;
                            req = Message.MsgRequest.COUNTER_REF_WAIT;
                            this.Invalidate();
                        }
                    }
                }
            }
        }

        private void cnt_ref_trackBar1_Scroll(object sender, EventArgs e)
        {
            cnt_ref_count_textBox2.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            cnt_ref_count_textBox2.Text = cnt_ref_trackBar2.Value.ToString();

            cnt_ref_count_textBox1.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            cnt_ref_sampleCount_textBox.ForeColor = Color.Black;
            cnt_ref_count_textBox1.Text = "" + cnt_ref_trackBar1.Value;

            refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
            cnt_ref_sampleCount_textBox.Text = "" + refSamples;

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_REF1;
        }

        private void cnt_ref_trackBar2_Scroll(object sender, EventArgs e)
        {
            cnt_ref_count_textBox1.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            cnt_ref_count_textBox1.Text = cnt_ref_trackBar1.Value.ToString();

            cnt_ref_count_textBox2.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            cnt_ref_sampleCount_textBox.ForeColor = Color.Black;
            cnt_ref_count_textBox2.Text = "" + cnt_ref_trackBar2.Value;

            refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
            cnt_ref_sampleCount_textBox.Text = "" + refSamples;

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_REF2;
        }

        private void cnt_ref_count_textBox1_Click(object sender, EventArgs e)
        {
            int boole;
            bool isNumeric = int.TryParse(cnt_ref_count_textBox1.Text, out boole);
            if (string.IsNullOrEmpty(cnt_ref_count_textBox1.Text) || !isNumeric)
            {
                cnt_ref_count_textBox1.Text = "";
            }
        }

        private void cnt_ref_count_textBox2_Click(object sender, EventArgs e)
        {
            int boole;
            bool isNumeric = int.TryParse(cnt_ref_count_textBox2.Text, out boole);
            if (string.IsNullOrEmpty(cnt_ref_count_textBox2.Text) || !isNumeric)
            {
                cnt_ref_count_textBox2.Text = "";
            }
        }

        private void cnt_ref_sampleCount_textBox_Click(object sender, EventArgs e)
        {
            int boole;
            bool isNumeric = int.TryParse(cnt_ref_sampleCount_textBox.Text, out boole);
            if (string.IsNullOrEmpty(cnt_ref_sampleCount_textBox.Text) || !isNumeric)
            {
                cnt_ref_sampleCount_textBox.Text = "";
            }
        }

        private void cnt_ref_count_textBox1_Enter(object sender, EventArgs e)
        {
            if (cnt_ref_count_textBox1.Text == NUM_ENTER)
            {
                cnt_ref_count_textBox1.Text = "";
                cnt_ref_count_textBox1.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            }
        }

        private void cnt_ref_count_textBox2_Enter(object sender, EventArgs e)
        {
            if (cnt_ref_count_textBox2.Text == NUM_ENTER)
            {
                cnt_ref_count_textBox2.Text = "";
                cnt_ref_count_textBox2.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            }
        }

        private void cnt_ref_sampleCount_textBox_Enter(object sender, EventArgs e)
        {
            if (cnt_ref_sampleCount_textBox.Text == NUM_ENTER)
            {
                cnt_ref_sampleCount_textBox.Text = "";
                cnt_ref_sampleCount_textBox.ForeColor = Color.Black;
            }
        }

        private void cnt_ref_count_textBox1_Leave(object sender, EventArgs e)
        {
            int boole;
            UInt32 numero = 0;
            bool isNumeric = int.TryParse(cnt_ref_count_textBox1.Text, out boole);

            if (cnt_ref_count_textBox1.Text.Length == 0)
            {
                cnt_ref_count_textBox1.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                cnt_ref_count_textBox1.Text = cnt_ref_trackBar1.Value.ToString();
            }

            if (isNumeric)
            {
                numero = Convert.ToUInt32(cnt_ref_count_textBox1.Text);

                if (numero < 2)
                {
                    cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                    cnt_ref_count_textBox1.Text = REF1_MIN;
                }
                else if (numero > 64000)
                {
                    cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                    cnt_ref_count_textBox1.Text = REF1_MAX;
                }
                else if (!cnt_ref_count_textBox1.Text.Equals(cnt_ref_trackBar1.Value.ToString()))
                {
                    cnt_ref_trackBar1.Value = Convert.ToUInt16(cnt_ref_count_textBox1.Text);                    
                    refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
                    cnt_ref_sampleCount_textBox.ForeColor = Color.Black;
                    cnt_ref_sampleCount_textBox.Text = "" + refSamples;

                    device.takeCommsSemaphore(semaphoreTimeout + 120);
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_ARR + " ");
                    device.send_short((ushort)cnt_ref_trackBar1.Value);
                    device.send(";");
                    device.giveCommsSemaphore();

                    req = Message.MsgRequest.COUNTER_REF_WAIT;
                    this.Invalidate();
                }
            }
        }

        private void cnt_ref_count_textBox2_Leave(object sender, EventArgs e)
        {
            int boole;
            UInt32 numero = 0;
            bool isNumeric = int.TryParse(cnt_ref_count_textBox2.Text, out boole);

            if (cnt_ref_count_textBox2.Text.Length == 0)
            {
                cnt_ref_count_textBox2.ForeColor = Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
                cnt_ref_count_textBox2.Text = cnt_ref_trackBar2.Value.ToString();
            }

            if (isNumeric)
            {
                numero = Convert.ToUInt32(cnt_ref_count_textBox2.Text);

                if (numero < 1)
                {
                    cnt_ref_count_textBox2.ForeColor = Color.DarkRed;
                    cnt_ref_count_textBox2.Text = REF2_MIN;
                }
                else if (numero > 64000)
                {
                    cnt_ref_count_textBox2.ForeColor = Color.DarkRed;
                    cnt_ref_count_textBox2.Text = REF2_MAX;
                }
                else if (!cnt_ref_count_textBox2.Text.Equals(cnt_ref_trackBar2.Value.ToString()))
                {
                    cnt_ref_trackBar2.Value = Convert.ToUInt16(cnt_ref_count_textBox2.Text);                    
                    refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
                    cnt_ref_sampleCount_textBox.ForeColor = Color.Black;
                    cnt_ref_sampleCount_textBox.Text = "" + refSamples;

                    device.takeCommsSemaphore(semaphoreTimeout + 120);
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_PSC + " ");
                    device.send_short((ushort)cnt_ref_trackBar2.Value);
                    device.send(";");
                    device.giveCommsSemaphore();

                    req = Message.MsgRequest.COUNTER_REF_WAIT;
                    this.Invalidate();                    
                }
            }
        }

        private void cnt_ref_sampleCount_textBox_Leave(object sender, EventArgs e)
        {
            int boole;
            UInt32 numero = 0;
            bool isNumeric = int.TryParse(cnt_ref_sampleCount_textBox.Text, out boole);

            if (cnt_ref_sampleCount_textBox.Text.Length == 0)
            {
                cnt_ref_sampleCount_textBox.ForeColor = Color.Black;
                cnt_ref_sampleCount_textBox.Text = ((UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value)).ToString();
            }

            if (isNumeric)
            {
                numero = Convert.ToUInt32(cnt_ref_sampleCount_textBox.Text);

                if (numero < 2)
                {
                    cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                    cnt_ref_sampleCount_textBox.Text = REF_MIN;
                }
                else if (numero > 4096000000)
                {
                    cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                    cnt_ref_sampleCount_textBox.Text = REF_MAX;
                }
                else
                {
                    refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
                    cnt_ref_sampleCount_textBox.ForeColor = Color.Black;
                    cnt_ref_sampleCount_textBox.Text = "" + refSamples;               
                }
            }
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ----------------------------------------------------- COUNTER TI EVENTS -------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void radioButton_it_rising_ch1_CheckedChanged(object sender, EventArgs e)
        {
            selectPicture();
            sendEventCommand(Commands.CNT_EVENT_RISING_ONLY_CH1);
        }

        private void radioButton_it_falling_ch1_CheckedChanged(object sender, EventArgs e)
        {
            selectPicture();
            sendEventCommand(Commands.CNT_EVENT_FALLING_ONLY_CH1);
        }

        private void radioButton_it_rising_ch2_CheckedChanged(object sender, EventArgs e)
        {
            selectPicture();
            sendEventCommand(Commands.CNT_EVENT_RISING_ONLY_CH2);
        }

        private void radioButton_it_falling_ch2_CheckedChanged(object sender, EventArgs e)
        {
            selectPicture();
            sendEventCommand(Commands.CNT_EVENT_FALLING_ONLY_CH2);
        }

        private void radioButton_it_eventSequence_ab_CheckedChanged(object sender, EventArgs e)
        {
            selectPicture();
        }

        private void radioButton_it_eventSequence_ba_CheckedChanged(object sender, EventArgs e)
        {
            selectPicture();
        }

        private void selectPicture()
        {
            bool evA_ris = (radioButton_it_rising_ch1.Checked) ? true : false;
            bool evA_fall = (radioButton_it_falling_ch1.Checked) ? true : false;
            bool evB_ris = (radioButton_it_rising_ch2.Checked) ? true : false;
            bool evB_fall = (radioButton_it_falling_ch2.Checked) ? true : false;
            bool AB_seq = (radioButton_it_eventSequence_ab.Checked) ? true : false;
            bool BA_seq = (radioButton_it_eventSequence_ba.Checked) ? true : false;

            if(AB_seq && evA_ris && evB_ris)
            {
                this.pictureBox_ti_sequence.Image = Properties.Resources.ab_rising_rising;
            }
            else if (AB_seq && evA_ris && evB_fall)
            {
                this.pictureBox_ti_sequence.Image = Properties.Resources.ab_rising_falling;
            }
            else if(AB_seq && evA_fall && evB_fall)
            {
                this.pictureBox_ti_sequence.Image = Properties.Resources.ab_falling_falling;
            }
            else if (AB_seq && evA_fall && evB_ris)
            {
                this.pictureBox_ti_sequence.Image = Properties.Resources.ab_falling_rising;
            }
            else if (BA_seq && evB_ris && evA_ris)
            {
                this.pictureBox_ti_sequence.Image = Properties.Resources.ba_rising_rising;
            }
            else if (BA_seq && evB_ris && evA_fall)
            {
                this.pictureBox_ti_sequence.Image = Properties.Resources.ba_rising_falling;
            }
            else if (BA_seq && evB_fall && evA_fall)
            {
                this.pictureBox_ti_sequence.Image = Properties.Resources.ba_falling_falling;
            }
            else if (BA_seq && evB_fall && evA_ris)
            {
                this.pictureBox_ti_sequence.Image = Properties.Resources.ba_falling_rising;
            }
        }

        private void trackBar_cnt_ti_timeout_Scroll(object sender, EventArgs e)
        {
            textBox_cnt_ti_timeout.ForeColor = Color.Black;
            textBox_cnt_ti_timeout.Text = trackBar_cnt_ti_timeout.Value.ToString();

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_TI;
        }

        private void textBox_cnt_ti_timeout_KeyPress(object sender, KeyPressEventArgs e)
        {
            textBox_cnt_ti_timeout.ForeColor = Color.Black;

            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(textBox_cnt_ti_timeout.Text))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    int timeout = Convert.ToInt32(textBox.Text);                    

                    if (timeout < 500)
                    {
                        textBox_cnt_ti_timeout.ForeColor = Color.DarkRed;
                        textBox_cnt_ti_timeout.Text = IC_MIN + "500";
                        this.ActiveControl = null;
                    }
                    else if (timeout > 28000)
                    {
                        textBox_cnt_ti_timeout.ForeColor = Color.DarkRed;
                        textBox_cnt_ti_timeout.Text = IC_MAX + "28000";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        trackBar_cnt_ti_timeout.Value = timeout;
                        textBox_cnt_ti_timeout.Text = timeout.ToString();

                        device.takeCommsSemaphore(semaphoreTimeout + 120);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_TI_TIMEOUT_VALUE + " ");
                        device.send_short((ushort)timeout);
                        device.send(";");
                        device.giveCommsSemaphore();
                        
                        //req = Message.MsgRequest.COUNTER_IC1_BUFF_WAIT;
                        //this.Invalidate();
                    }
                }
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                textBox_cnt_ti_timeout.ForeColor = Color.DarkRed;
                textBox_cnt_ti_timeout.Text = NUMS_ONLY;
                this.ActiveControl = null;
            }
        }

        private void textBox_cnt_ti_timeout_Leave(object sender, EventArgs e)
        {
            int boole;
            int timeout = 0;
            bool isNumeric = int.TryParse(textBox_cnt_ti_timeout.Text, out boole);

            if (textBox_cnt_ti_timeout.Text.Length == 0)
            {
                textBox_cnt_ti_timeout.ForeColor = Color.Black;
                textBox_cnt_ti_timeout.Text = trackBar_cnt_ti_timeout.Value.ToString();
            }

            if (isNumeric)
            {
                timeout = Convert.ToUInt16(textBox_cnt_ti_timeout.Text);                

                if (timeout < 500)
                {
                    textBox_cnt_ti_timeout.ForeColor = Color.DarkRed;
                    textBox_cnt_ti_timeout.Text = IC_MIN + "500";
                }
                else if (timeout > 28000)
                {
                    textBox_cnt_ti_timeout.ForeColor = Color.DarkRed;
                    textBox_cnt_ti_timeout.Text = IC_MAX + "28000";
                }
                else if (!textBox_cnt_ti_timeout.Text.Equals(trackBar_cnt_ti_timeout.Value.ToString()))
                {
                    trackBar_cnt_ti_timeout.Value = Convert.ToInt16(textBox_cnt_ti_timeout.Text);
                    textBox_cnt_ti_timeout.Text = timeout.ToString();

                    device.takeCommsSemaphore(semaphoreTimeout + 120);
                    device.send(Commands.COUNTER + ":" + Commands.CNT_TI_TIMEOUT_VALUE + " ");
                    device.send_short((ushort)timeout);
                    device.send(";");
                    device.giveCommsSemaphore();
                }
            }
        }

        private void textBox_cnt_ti_timeout_Click(object sender, EventArgs e)
        {
            int boole;
            bool isNumeric = int.TryParse(textBox_cnt_ti_timeout.Text, out boole);
            if (string.IsNullOrEmpty(textBox_cnt_ti_timeout.Text) || !isNumeric)
            {
                textBox_cnt_ti_timeout.Text = "";
            }
        }

        private void button_cnt_ti_enable_Click(object sender, EventArgs e)
        {
            if (button_cnt_ti_enable.Text == "Start")
            {                
                cnt_start();
                button_cnt_ti_enable.Text = "Stop";
                groupBox_ti_event_A.Enabled = false;
                groupBox_ti_event_B.Enabled = false;
                groupBox_ti_eventSeq.Enabled = false;
                groupBox_ti_timeout.Enabled = false;

                label_cnt_it_value.ForeColor = SystemColors.WindowFrame;
                label_cnt_it_value.Font = new Font("Times New Roman", 14);
                label_cnt_it_value.Text = "Waiting for events.";
            }
            else
            {
                cnt_stop();
                button_cnt_ti_enable.Text = "Start";
                groupBox_ti_event_A.Enabled = true;
                groupBox_ti_event_B.Enabled = true;
                groupBox_ti_eventSeq.Enabled = true;
                groupBox_ti_timeout.Enabled = true;

                label_cnt_it_value.ForeColor = Color.Black;
                label_cnt_it_value.Font = new Font("Times New Roman", 21.75F);
                label_cnt_it_value.Text = "0.000000";
            }
        }

        private void sendEventCommand(String command)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 105);
            device.send(Commands.COUNTER + ":" + Commands.CNT_EVENT + " ");
            device.send(command + ";");
            device.giveCommsSemaphore();
        }
    }
}
