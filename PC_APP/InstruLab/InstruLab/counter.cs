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
        Message.MsgRequest req;
        int circBuffSizeTest;

        System.Timers.Timer GUITimer;
        private static System.Timers.Timer scrollTimer = new System.Timers.Timer(200);

        private Queue<Message> cnt_q = new Queue<Message>();
        Message messg;

        public LinkedList<double> avrgList = new LinkedList<double>();
        LinkedListNode<double> nodeCurrent;
        double average;

        public enum CNT_MODES { IC = 0, ETR, REF };
        public enum CNT_TIMER { TIM_SCROLL_REF1 = 0, TIM_SCROLL_REF2, TIM_SCROLL_IC1, TIM_SCROLL_IC2 };
        CNT_TIMER timScroll;
        public enum KEY_PRESS { YES = 0, NO };
        KEY_PRESS keyPress;

        public counter(Device dev)
        {
            InitializeComponent();
            scrollTimerConfig();

            device = dev;

            GUITimer = new System.Timers.Timer(50);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            GUITimer.Start();

            cnt_init_mode(CNT_MODES.ETR);
            cnt_start();
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
                    case Message.MsgRequest.COUNTER_ETR_DATA:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        //                      sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_REF_DATA:
                        cntPaint = messg.GetNum();
                        this.Invalidate();
                        //                      sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_IC1_DATA:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        //                      sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_IC2_DATA:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        //                      sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_IC1_BUFF:
                        cntPaint = messg.GetNum();
                        this.Invalidate();
                        //                      sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_IC2_BUFF:
                        cntPaint = messg.GetNum();
                        this.Invalidate();
                        //                      sendDataReques();
                        break;
                }
            }
        }

        private void sendDataReques()
        {
            device.takeCommsSemaphore(semaphoreTimeout * 2 + 108);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_NEXT + ";");
            device.giveCommsSemaphore();
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
                    this.label_cnt_etr_value.Text = cntPaint.ToString("F6");

                    ushort gateTime = getRadioNumber();
                    if (gateTime == 5000)
                    {
                        label_cnt_avrg_info.Text = "5 sec. update";
                    }
                    else if (gateTime == 10000)
                    {
                        label_cnt_avrg_info.Text = "10 sec. update";
                    }
                    else
                    {
                        label_cnt_avrg_info.Text = "";
                    }

                    if ((cnt_etr_avrg_textBox.Text != "") && (cnt_etr_avrg_textBox.Text != "Numbers only") && (cnt_etr_avrg_textBox.Text != "Max val. 200") && (cnt_etr_avrg_textBox.Text != "Min val. 2"))
                    {
                        Int16 textBox = Convert.ToInt16(cnt_etr_avrg_textBox.Text);

                        if (((avrgList.Count < textBox) && (textBox != 2)) && (keyPress == KEY_PRESS.YES))
                        {
                            label_cnt_etr_avrg.ForeColor = SystemColors.WindowFrame;
                            label_cnt_etr_avrg.Font = new Font("Times New Roman", 15);

                            Int32 difference = Convert.ToInt32(cnt_etr_avrg_textBox.Text) - (int)(avrgList.Count);
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
                            label_cnt_etr_avrg.Font = new Font("Times New Roman", 24);
                            this.label_cnt_etr_avrg.Text = average.ToString("F6");
                            keyPress = KEY_PRESS.NO;
                        }
                    }

                    if (label_cnt_etr_value.Text == "1E-05")
                    {
                        //label_cnt_etr_avrg.ForeColor = SystemColors.WindowFrame;
                        label_cnt_etr_value.Text = "0.0000";
                    }

                    if (label_cnt_etr_avrg.Text == "1E-05" || label_cnt_etr_avrg.Text == "0")
                    {
                        label_cnt_etr_avrg.ForeColor = SystemColors.WindowFrame;
                        label_cnt_etr_avrg.Text = "NA";
                    }
                }
                /************************************** IC BUFF WAIT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC1_BUFF_WAIT)
                {
                    cnt_ic1_buffRecalc_textBox.Text = "Wait";
                }
                else if (req == Message.MsgRequest.COUNTER_IC2_BUFF_WAIT)
                {
                    cnt_ic2_buffRecalc_textBox.Text = "Wait";
                }
                /************************************** IC1 DATA PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC1_DATA)
                {
                    if (cntPaint > 10100000)
                    {
                        label_cnt_ic1_value.ForeColor = Color.DarkRed;
                        label_cnt_ic1_value.Font = new Font("Times New Roman", 13);
                        label_cnt_ic1_value.Text = "Frequency > 10 MHz";
                    }
                    else if ((cntPaint < 12000000) && (cntPaint > 9800000) && (label_cnt_ic1_value.Text == "Frequency > 10 MHz"))
                    {
                        label_cnt_ic1_value.Text = "Frequency > 10 MHz";
                    }
                    else
                    {
                        if (cntPaint != 0)
                        {
                            label_cnt_ic1_value.ForeColor = SystemColors.ControlText;
                            label_cnt_ic1_value.Font = new Font("Times New Roman", 24);
                            this.label_cnt_ic1_value.Text = cntPaint.ToString("F6");
                        }
                    }

                }
                /************************************** IC2 DATA PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC2_DATA)
                {
                    if (cntPaint > 10100000)
                    {
                        label_cnt_ic2_value.ForeColor = Color.DarkRed;
                        label_cnt_ic2_value.Font = new Font("Times New Roman", 13);
                        label_cnt_ic2_value.Text = "Frequency > 10 MHz";
                    }
                    else if ((cntPaint < 12000000) && (cntPaint > 9800000) && (label_cnt_ic2_value.Text == "Frequency > 10 MHz"))
                    {
                        label_cnt_ic2_value.Text = "Frequency > 10 MHz";
                    }
                    else
                    {
                        if (cntPaint != 0)
                        {
                            label_cnt_ic2_value.ForeColor = SystemColors.ControlText;
                            label_cnt_ic2_value.Font = new Font("Times New Roman", 24);
                            this.label_cnt_ic2_value.Text = cntPaint.ToString("F6");
                        }
                    }
                }
                /************************************** IC1 SAMPLE COUNT PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC1_BUFF)
                {
                    this.cnt_ic1_buffRecalc_textBox.Text = cntPaint.ToString();
                }
                /************************************** IC2 SAMPLE COUNT PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_IC2_BUFF)
                {
                    this.cnt_ic2_buffRecalc_textBox.Text = cntPaint.ToString();
                }
                /************************************** REF DATA PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_REF_DATA)
                {
                    int boole;
                    bool isNumeric = int.TryParse(cnt_ref_sampleCount_textBox.Text, out boole);
                    if (!string.IsNullOrEmpty(cnt_ref_sampleCount_textBox.Text) && isNumeric)
                    {
                        label_cnt_ref_value.ForeColor = SystemColors.ControlText;
                        label_cnt_ref_value.Font = new Font("Times New Roman", 24);
                        this.label_cnt_ref_value.Text = (refSamples / cntPaint).ToString("F6");
                    }
                }
                /************************************** REF WAIT PRINT **************************************/
                else if (req == Message.MsgRequest.COUNTER_REF_WAIT)
                {
                    int boole;
                    bool isNumeric = int.TryParse(cnt_ref_sampleCount_textBox.Text, out boole);
                    if (!string.IsNullOrEmpty(cnt_ref_sampleCount_textBox.Text) && isNumeric)
                    {
                        label_cnt_ref_value.ForeColor = SystemColors.WindowFrame;
                        label_cnt_ref_value.Font = new Font("Times New Roman", 18);
                        label_cnt_ref_value.Text = "Sampling!";
                    }
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
            device.send(Commands.COUNTER + ":" + Commands.MODE + " ");
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
            device.giveCommsSemaphore();
        }

        public void add_message(Message msg)
        {
            this.cnt_q.Enqueue(msg);
        }

        private void cnt_mode_tabControl_SelectedIndexchanged(object sender, EventArgs e)
        {
            switch ((sender as TabControl).SelectedIndex)
            {
                case 0:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.ETR);
                    cnt_start();
                    ETR_appReinit();
                    break;
                case 1:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.IC);
                    cnt_start();
                    IC_appReinit();
                    break;
                case 2:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.REF);
                    cnt_start();
                    REF_appReinit();
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
        }

        private void REF_appReinit()
        {
            cnt_ref_sampleCount_textBox.Text = "1000000";
            cnt_ref_trackBar1.Value = 1000;
            cnt_ref_trackBar2.Value = 1000;
            cnt_ref_count_textBox1.Text = "1000";
            cnt_ref_count_textBox2.Text = "1000";
            refSamples = 1000000;
            label_cnt_ref_value.ForeColor = SystemColors.ControlText;
            label_cnt_ref_value.Font = new Font("Times New Roman", 24);
            label_cnt_ref_value.Text = "0.000000";
        }

        private void IC_appReinit()
        {
            cnt_ic1_buffer_textBox.ForeColor = SystemColors.WindowFrame;
            cnt_ic2_buffer_textBox.ForeColor = SystemColors.WindowFrame;
            cnt_ic1_buffer_textBox.Text = "1";
            cnt_ic2_buffer_textBox.Text = "1";
            cnt_ic1_buffRecalc_textBox.Text = "Recalculated";
            cnt_ic2_buffRecalc_textBox.Text = "Recalculated";
            label_cnt_ic1_value.Text = "0.000000";
            label_cnt_ic2_value.Text = "0.000000";
            cnt_ic1_trackBar.Value = 1;
            cnt_ic2_trackBar.Value = 1;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* --------------------------------------------------- COUNTER IC1/IC2 EVENTS ----------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* Robusting textBox "IC1 buffer" */
        private void cnt_ic1_buffer_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ic1_buffer_textBox.ForeColor = SystemColors.WindowFrame;

            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(cnt_ic1_buffer_textBox.Text))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    int ic1buffer = Convert.ToInt32(textBox.Text);

                    if (ic1buffer < 1)
                    {
                        cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic1_buffer_textBox.Text = "Min val. 1";
                        this.ActiveControl = null;
                    }
                    else if (ic1buffer > 100)
                    {
                        cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic1_buffer_textBox.Text = "Max val. 100";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        cnt_ic1_trackBar.Value = ic1buffer;
                        device.takeCommsSemaphore(semaphoreTimeout + 120);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT1 + " ");
                        device.send_short((ushort)ic1buffer);
                        device.send(";");
                        device.giveCommsSemaphore();
                        req = Message.MsgRequest.COUNTER_IC1_BUFF_WAIT;
                        this.Invalidate();
                    }
                }
            }

            if (cnt_ic1_buffer_textBox.Text.Length > 5 && cnt_ic1_buffer_textBox.Text != "Min val. 1")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                cnt_ic1_buffer_textBox.Text = "Max val. 100";
                this.ActiveControl = null;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                cnt_ic1_buffer_textBox.Text = "Numbers only";
                this.ActiveControl = null;
            }
        }

        private void cnt_ic2_buffer_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ic2_buffer_textBox.ForeColor = SystemColors.WindowFrame;

            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(cnt_ic2_buffer_textBox.Text))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    int ic2buffer = Convert.ToInt32(textBox.Text);

                    if (ic2buffer < 1)
                    {
                        cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic2_buffer_textBox.Text = "Min val. 1";
                        this.ActiveControl = null;
                    }
                    else if (ic2buffer > 100)
                    {
                        cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic2_buffer_textBox.Text = "Max val. 100";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        cnt_ic2_trackBar.Value = ic2buffer;
                        device.takeCommsSemaphore(semaphoreTimeout + 121);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT2 + " ");
                        device.send_short((ushort)ic2buffer);
                        device.send(";");
                        device.giveCommsSemaphore();
                        req = Message.MsgRequest.COUNTER_IC2_BUFF_WAIT;
                        this.Invalidate();
                    }
                }
            }

            if (cnt_ic2_buffer_textBox.Text.Length > 5 && cnt_ic2_buffer_textBox.Text != "Min val. 1")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                cnt_ic2_buffer_textBox.Text = "Max val. 100";
                this.ActiveControl = null;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                cnt_ic2_buffer_textBox.Text = "Numbers only";
                this.ActiveControl = null;
            }
        }

        private void cnt_ic1_buffer_textBox_Click(object sender, EventArgs e)
        {
            cnt_ic1_buffer_textBox.Text = "";
        }

        private void cnt_ic2_buffer_textBox_Click(object sender, EventArgs e)
        {
            cnt_ic2_buffer_textBox.Text = "";
        }

        private void cnt_ic1_buffer_textBox_Leave(object sender, EventArgs e)
        {
            if (cnt_ic1_buffer_textBox.Text.Length == 0)
            {
                cnt_ic1_buffer_textBox.ForeColor = SystemColors.GrayText;
                cnt_ic1_buffer_textBox.Text = "Enter a number";
            }
        }

        private void cnt_ic2_buffer_textBox_Leave(object sender, EventArgs e)
        {
            if (cnt_ic2_buffer_textBox.Text.Length == 0)
            {
                cnt_ic2_buffer_textBox.ForeColor = SystemColors.GrayText;
                cnt_ic2_buffer_textBox.Text = "Enter a number";
            }
        }

        private void cnt_ic2_buffer_textBox_Enter(object sender, EventArgs e)
        {
            if (cnt_ic2_buffer_textBox.Text == "Enter a number")
            {
                cnt_ic2_buffer_textBox.Text = "";
                cnt_ic2_buffer_textBox.ForeColor = SystemColors.WindowFrame;
            }
        }

        private void cnt_ic1_buffer_textBox_Enter(object sender, EventArgs e)
        {
            if (cnt_ic1_buffer_textBox.Text == "Enter a number")
            {
                cnt_ic1_buffer_textBox.Text = "";
                cnt_ic1_buffer_textBox.ForeColor = SystemColors.WindowFrame;
            }
        }

        private void cnt_ic1_trackBar_Scroll(object sender, EventArgs e)
        {
            cnt_ic1_buffer_textBox.ForeColor = SystemColors.GrayText;
            cnt_ic1_buffer_textBox.Text = cnt_ic1_trackBar.Value.ToString();

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_IC1;
        }

        private void cnt_ic2_trackBar_Scroll(object sender, EventArgs e)
        {
            cnt_ic2_buffer_textBox.ForeColor = SystemColors.GrayText;
            cnt_ic2_buffer_textBox.Text = cnt_ic2_trackBar.Value.ToString();

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_IC2;
        }


        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ----------------------------------------------------- COUNTER ETR EVENTS ------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void cnt_etr_avrg_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_etr_avrg_textBox.ForeColor = SystemColors.WindowFrame;

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
                        cnt_etr_avrg_textBox.Text = "Min val. 2";
                        this.ActiveControl = null;
                    }
                    else if (etrbuffer > 200)
                    {
                        cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                        cnt_etr_avrg_textBox.Text = "Max val. 200";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        cnt_etr_trackBar.Value = etrbuffer;
                        keyPress = KEY_PRESS.YES;
                        req = Message.MsgRequest.COUNTER_ETR_DATA;
                        this.Invalidate();
                    }
                }
            }

            if (cnt_etr_avrg_textBox.Text.Length > 5 && cnt_etr_avrg_textBox.Text != "Min val. 2")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                cnt_etr_avrg_textBox.Text = "Max val. 200";
                this.ActiveControl = null;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                cnt_etr_avrg_textBox.Text = "Numbers only";
                this.ActiveControl = null;
            }
        }

        private void cnt_etr_avrg_textBox_Click(object sender, EventArgs e)
        {
            cnt_etr_avrg_textBox.Text = "";
        }

        private void cnt_etr_trackBar_Scroll(object sender, EventArgs e)
        {
            cnt_etr_avrg_textBox.ForeColor = SystemColors.GrayText;
            cnt_etr_avrg_textBox.Text = "" + cnt_etr_trackBar.Value;
            keyPress = KEY_PRESS.YES;
            avrgCircBuffer();
            req = Message.MsgRequest.COUNTER_ETR_DATA;
            this.Invalidate();
        }

        private void cnt_etr_avrg_textBox_Enter(object sender, EventArgs e)
        {
            if (cnt_etr_avrg_textBox.Text == "Enter a number")
            {
                cnt_etr_avrg_textBox.Text = "";
                cnt_etr_avrg_textBox.ForeColor = SystemColors.WindowFrame;
            }
        }

        private void cnt_etr_avrg_textBox_Leave(object sender, EventArgs e)
        {
            if (cnt_etr_avrg_textBox.Text.Length == 0)
            {
                cnt_etr_avrg_textBox.ForeColor = SystemColors.GrayText;
                cnt_etr_avrg_textBox.Text = "Enter a number";
            }
        }

        private void radioButton_100m_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 108);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_100m + ";");
            device.giveCommsSemaphore();
            req = Message.MsgRequest.COUNTER_ETR_DATA;
            this.Invalidate();
        }
        private void radioButton_500m_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 107);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_500m + ";");
            device.giveCommsSemaphore();
            req = Message.MsgRequest.COUNTER_ETR_DATA;
            this.Invalidate();
        }

        private void radioButton_1s_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 105);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_1s + ";");
            device.giveCommsSemaphore();
            req = Message.MsgRequest.COUNTER_ETR_DATA;
            this.Invalidate();
        }

        private void radioButton_5s_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 108);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_5s + ";");
            device.giveCommsSemaphore();
            req = Message.MsgRequest.COUNTER_ETR_DATA;
            this.Invalidate();
        }

        private void radioButton_10s_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 106);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_10s + ";");
            device.giveCommsSemaphore();
            req = Message.MsgRequest.COUNTER_ETR_DATA;
            this.Invalidate();
        }


        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* -------------------------------------------------- COUNTER ETR FUNCTIONS ------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void avrgCircBuffer()
        {
            int circBuffSize = Convert.ToInt32(cnt_etr_trackBar.Value);
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
                }
            }

            if ((arr <= 64000) && (psc <= 64000) && (arr > 0) && (psc > 0))
            {

                device.takeCommsSemaphore(semaphoreTimeout + 121);

                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_PSC + " ");
                device.send_short((ushort)psc);
                device.send(";");

                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_ARR + " ");
                device.send_short((ushort)arr);
                device.send(";");

                device.giveCommsSemaphore();

                cnt_ref_count_textBox1.ForeColor = SystemColors.WindowFrame;
                cnt_ref_count_textBox2.ForeColor = SystemColors.WindowFrame;
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
        /* ------------------------------------------------- COUNTER REF & IC TIMERS ------------------------------------------------------ */
        /* -------------------------------------------------------------------------------------------------------------------------------- */

        private void scrollTimerConfig()
        {
            scrollTimer.Elapsed += new ElapsedEventHandler(scrollTimeElapseEvent);
            scrollTimer.AutoReset = false;
        }

        private void scrollTimeElapseEvent(Object source, ElapsedEventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 121);

            switch (timScroll)
            {
                case CNT_TIMER.TIM_SCROLL_IC1:
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT1 + " ");
                    device.send_short(Convert.ToUInt16(cnt_ic1_buffer_textBox.Text));
                    device.send(";");
                    req = Message.MsgRequest.COUNTER_IC1_BUFF_WAIT;
                    this.Invalidate();
                    break;
                case CNT_TIMER.TIM_SCROLL_IC2:
                    device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT2 + " ");
                    device.send_short(Convert.ToUInt16(cnt_ic2_buffer_textBox.Text));
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
            }

            device.giveCommsSemaphore();
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ----------------------------------------------------- COUNTER REF EVENTS ------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void cnt_ref_count_textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ref_count_textBox1.ForeColor = SystemColors.WindowFrame;

            TextBox textBox = (TextBox)sender;

            if (cnt_ref_count_textBox1.Text.Length > 10 && cnt_ref_count_textBox1.Text != "Min val. 2")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                cnt_ref_count_textBox1.Text = "Max val. 64000";
                this.ActiveControl = null;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                cnt_ref_count_textBox1.Text = "Numbers only";
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
                        cnt_ref_count_textBox1.Text = "Min val. 2";
                        this.ActiveControl = null;
                    }
                    else if (refSampleCount > 64000)
                    {
                        cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                        cnt_ref_count_textBox1.Text = "Max val. 64000";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        cnt_ref_trackBar1.Value = int.Parse(cnt_ref_count_textBox1.Text);
                        cnt_ref_sampleCount_textBox.ForeColor = SystemColors.WindowFrame;

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
            cnt_ref_count_textBox2.ForeColor = SystemColors.WindowFrame;

            TextBox textBox = (TextBox)sender;

            if (cnt_ref_count_textBox2.Text.Length > 10 && cnt_ref_count_textBox2.Text != "Min val. 1")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_count_textBox2.ForeColor = Color.DarkRed;
                cnt_ref_count_textBox2.Text = "Max val. 64000";
                this.ActiveControl = null;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_count_textBox2.ForeColor = Color.DarkRed;
                cnt_ref_count_textBox2.Text = "Numbers only";
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
                        cnt_ref_count_textBox2.Text = "Min val. 1";
                        this.ActiveControl = null;
                    }
                    else if (refSampleCount > 64000)
                    {
                        cnt_ref_count_textBox2.ForeColor = Color.DarkRed;
                        cnt_ref_count_textBox2.Text = "Max val. 64000";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        cnt_ref_trackBar2.Value = int.Parse(cnt_ref_count_textBox2.Text);
                        cnt_ref_sampleCount_textBox.ForeColor = SystemColors.WindowFrame;

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
            cnt_ref_sampleCount_textBox.ForeColor = SystemColors.WindowFrame;

            TextBox textBox = (TextBox)sender;

            if (cnt_ref_sampleCount_textBox.Text.Length > 10 && cnt_ref_sampleCount_textBox.Text != "Min val. 2")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                cnt_ref_sampleCount_textBox.Text = "Max val. 4.096 G";
                this.ActiveControl = null;
            }

            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                cnt_ref_sampleCount_textBox.Text = "Numbers only";
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
                        cnt_ref_sampleCount_textBox.Text = "Min val. 2";
                        this.ActiveControl = null;
                    }
                    else if (refSampleCount > 4096000000)
                    {
                        cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                        cnt_ref_sampleCount_textBox.Text = "Max val. 4.096 G";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        splitAndSend((UInt32)refSampleCount);
                        refSamples = (UInt32)refSampleCount;

                        req = Message.MsgRequest.COUNTER_REF_WAIT;
                        this.Invalidate();
                    }
                }
            }
        }

        private void cnt_ref_trackBar1_Scroll(object sender, EventArgs e)
        {
            cnt_ref_count_textBox2.ForeColor = SystemColors.GrayText;
            cnt_ref_count_textBox2.Text = cnt_ref_trackBar2.Value.ToString();

            cnt_ref_count_textBox1.ForeColor = SystemColors.GrayText;
            cnt_ref_sampleCount_textBox.ForeColor = SystemColors.GrayText;
            cnt_ref_count_textBox1.Text = "" + cnt_ref_trackBar1.Value;

            refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
            cnt_ref_sampleCount_textBox.Text = "" + refSamples;

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_REF1;
        }

        private void cnt_ref_trackBar2_Scroll(object sender, EventArgs e)
        {
            cnt_ref_count_textBox1.ForeColor = SystemColors.GrayText;
            cnt_ref_count_textBox1.Text = cnt_ref_trackBar1.Value.ToString();

            cnt_ref_count_textBox2.ForeColor = SystemColors.GrayText;
            cnt_ref_sampleCount_textBox.ForeColor = SystemColors.GrayText;
            cnt_ref_count_textBox2.Text = "" + cnt_ref_trackBar2.Value;

            refSamples = (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
            cnt_ref_sampleCount_textBox.Text = "" + refSamples;

            scrollTimer.Stop();
            scrollTimer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL_REF2;
        }

        private void cnt_ref_count_textBox1_Click(object sender, EventArgs e)
        {
            cnt_ref_count_textBox1.Text = "";
        }

        private void cnt_ref_count_textBox2_Click(object sender, EventArgs e)
        {
            cnt_ref_count_textBox2.Text = "";
        }

        private void cnt_ref_sampleCount_textBox_Click(object sender, EventArgs e)
        {
            cnt_ref_sampleCount_textBox.Text = "";
        }

        private void cnt_ref_count_textBox1_Enter(object sender, EventArgs e)
        {
            if (cnt_ref_count_textBox1.Text == "Enter a number")
            {
                cnt_ref_count_textBox1.Text = "";
                cnt_ref_count_textBox1.ForeColor = SystemColors.WindowFrame;
            }
        }

        private void cnt_ref_count_textBox2_Enter(object sender, EventArgs e)
        {
            if (cnt_ref_count_textBox2.Text == "Enter a number")
            {
                cnt_ref_count_textBox2.Text = "";
                cnt_ref_count_textBox2.ForeColor = SystemColors.WindowFrame;
            }
        }

        private void cnt_ref_sampleCount_textBox_Enter(object sender, EventArgs e)
        {
            if (cnt_ref_sampleCount_textBox.Text == "Enter a number")
            {
                cnt_ref_sampleCount_textBox.Text = "";
                cnt_ref_sampleCount_textBox.ForeColor = SystemColors.WindowFrame;
            }
        }

        private void cnt_ref_count_textBox1_Leave(object sender, EventArgs e)
        {
            if (cnt_ref_count_textBox1.Text.Length == 0)
            {
                cnt_ref_count_textBox1.ForeColor = SystemColors.GrayText;
                cnt_ref_count_textBox1.Text = "Enter a number";
            }
        }

        private void cnt_ref_count_textBox2_Leave(object sender, EventArgs e)
        {
            if (cnt_ref_count_textBox2.Text.Length == 0)
            {
                cnt_ref_count_textBox2.ForeColor = SystemColors.GrayText;
                cnt_ref_count_textBox2.Text = "Enter a number";
            }
        }

        private void cnt_ref_sampleCount_textBox_Leave(object sender, EventArgs e)
        {
            if (cnt_ref_sampleCount_textBox.Text.Length == 0)
            {
                cnt_ref_sampleCount_textBox.ForeColor = SystemColors.GrayText;
                cnt_ref_sampleCount_textBox.Text = "Enter a number";
            }
        }
    }
}
