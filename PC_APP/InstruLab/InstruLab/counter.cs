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

        double cntPaint;
        Message.MsgRequest req;

        System.Timers.Timer GUITimer;
        private static System.Timers.Timer timer = new System.Timers.Timer(160);

        private Queue<Message> cnt_q = new Queue<Message>();
        Message messg;

        public LinkedList<double> avrgList = new LinkedList<double>();
        LinkedListNode<double> nodeCurrent;
        double average;

        UInt32 refSamples = 0;

        public enum CNT_MODES { IC = 0, ETR, REF };
        public enum CNT_TIMER { TIM_SCROLL1 = 0, TIM_SCROLL2, TIM_ELAPSE1, TIM_ELAPSE2 };
        CNT_TIMER timScroll;

        public counter(Device dev)
        {
            InitializeComponent();
            timerConfig();

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
                        circBuffer();
                        sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_REF_DATA:
                        cntPaint = messg.GetNum();
                        this.Invalidate();
                        sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_IC1_DATA:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_IC2_DATA:
                        cntPaint = messg.GetFlt();
                        this.Invalidate();
                        sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_IC1_BUFF:
                        cntPaint = messg.GetNum();
                        this.Invalidate();
                        sendDataReques();
                        break;
                    case Message.MsgRequest.COUNTER_IC2_BUFF:
                        cntPaint = messg.GetNum();
                        this.Invalidate();
                        sendDataReques();
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
                if (req == Message.MsgRequest.COUNTER_ETR_DATA)
                {
                    label_cnt_etr_value.ForeColor = SystemColors.ControlText;
                    label_cnt_etr_avrg.ForeColor = SystemColors.ControlText;
                    this.label_cnt_etr_value.Text = cntPaint.ToString("F6");
                    this.label_cnt_etr_avrg.Text = average.ToString("F6");

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
                else if (req == Message.MsgRequest.COUNTER_IC1_DATA)
                {
                    this.label_cnt_ic1_value.Text = cntPaint.ToString("F6");
                    //if (label_cnt_ic1_value.Text == "1E-05")
                    //{
                    //    label_cnt_ic1_value.Text = "0.0000";
                    //}
                }
                else if (req == Message.MsgRequest.COUNTER_IC2_DATA)
                {
                    this.label_cnt_ic2_value.Text = cntPaint.ToString("F6");                    
                    //if (label_cnt_ic2_value.Text == "1E-05")
                    //{
                    //    label_cnt_ic2_value.Text = "0.0000";
                    //}
                }
                else if (req == Message.MsgRequest.COUNTER_IC1_BUFF)
                {
                    this.cnt_ic1_buffRecalc_textBox.Text = cntPaint.ToString();
                }
                else if (req == Message.MsgRequest.COUNTER_IC2_BUFF)
                {
                    this.cnt_ic2_buffRecalc_textBox.Text = cntPaint.ToString();
                }
                else if (req == Message.MsgRequest.COUNTER_REF_DATA)
                {
                    this.label_cnt_ref_value.Text = (refSamples / cntPaint).ToString();
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

                    radioButton_1s.Checked = true;
                    radioButton_1s_CheckedChanged(null, null);
                    cnt_etr_trackBar.Value = 2;
                    cnt_etr_trackBar_Scroll(null, null);
                    cnt_etr_avrg_textBox.Text = "Enter a number";
                    label_cnt_etr_avrg.Text = "0.000000";
                    label_cnt_etr_value.Text = "0.000000";
                    break;
                case 1:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.IC);
                    cnt_start();

                    cnt_ic1_buffer_textBox.ForeColor = SystemColors.WindowFrame;
                    cnt_ic2_buffer_textBox.ForeColor = SystemColors.WindowFrame;
                    cnt_ic1_buffer_textBox.Text = "4";
                    cnt_ic2_buffer_textBox.Text = "4";
                    cnt_ic1_buffRecalc_textBox.Text = "";
                    cnt_ic2_buffRecalc_textBox.Text = "";
                    label_cnt_ic1_value.Text = "0.000000";
                    label_cnt_ic2_value.Text = "0.000000";
                    break;
                case 2:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.REF);
                    cnt_start();

                    cnt_ref_sampleCount_textBox.Text = "1 000 000";                    
                    cnt_ref_trackBar1.Value = 1000;
                    cnt_ref_trackBar2.Value = 1000;
                    cnt_ref_count_textBox1.Text = "1000";
                    cnt_ref_count_textBox2.Text = "1000";
                    break;
            }
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* -------------------------------------------------- COUNTER IC1/IC2 FUNCTIONS --------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* Robusting textBox "IC1 buffer" */
        //this.cnt_ic1_buffer_textBox.KeyPress += this.cnt_ic1_buffer_textBox_KeyPress;
        private void cnt_ic1_buffer_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ic1_buffer_textBox.ForeColor = SystemColors.WindowFrame;

            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(cnt_ic1_buffer_textBox.Text))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    int ic1buffer = Convert.ToInt32(textBox.Text) + 1;

                    if (ic1buffer < 2)
                    {
                        cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic1_buffer_textBox.Text = "Min val. 1";
                        this.ActiveControl = null;
                    }
                    else if (ic1buffer > 10001)
                    {
                        cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic1_buffer_textBox.Text = "Max val. 10 k";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        device.takeCommsSemaphore(semaphoreTimeout + 120);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT1 + " ");
                        device.send_short((ushort)ic1buffer);
                        device.send(";");
                        device.giveCommsSemaphore();
                    }
                }
            }

            if (cnt_ic1_buffer_textBox.Text.Length > 5 && cnt_ic1_buffer_textBox.Text != "Min val. 1")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ic1_buffer_textBox.ForeColor = Color.DarkRed;
                cnt_ic1_buffer_textBox.Text = "Max val. 10 k";
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

        //this.cnt_ic2_buffer_textBox.KeyPress += this.cnt_ic2_buffer_textBox_KeyPress;
        private void cnt_ic2_buffer_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ic2_buffer_textBox.ForeColor = SystemColors.WindowFrame;

            TextBox textBox = (TextBox)sender;

            if (!string.IsNullOrEmpty(cnt_ic2_buffer_textBox.Text))
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    int ic2buffer = Convert.ToInt32(textBox.Text) + 1;

                    if (ic2buffer < 2)
                    {
                        cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic2_buffer_textBox.Text = "Min val. 1";
                        this.ActiveControl = null;
                    }
                    else if (ic2buffer > 10001)
                    {
                        cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                        cnt_ic2_buffer_textBox.Text = "Max val. 10 k";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        device.takeCommsSemaphore(semaphoreTimeout + 121);
                        device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT2 + " ");
                        device.send_short((ushort)ic2buffer);
                        device.send(";");
                        device.giveCommsSemaphore();
                    }
                }
            }

            if (cnt_ic2_buffer_textBox.Text.Length > 5 && cnt_ic2_buffer_textBox.Text != "Min val. 1")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_ic2_buffer_textBox.ForeColor = Color.DarkRed;
                cnt_ic2_buffer_textBox.Text = "Max val. 10 k";
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


        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ---------------------------------------------------- COUNTER ETR FUNCTIONS ----------------------------------------------------- */
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
                    else if (etrbuffer > 10000)
                    {
                        cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                        cnt_etr_avrg_textBox.Text = "Max val. 10 k";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                        cnt_etr_trackBar.Value = etrbuffer;
                    }
                }
            }

            if (cnt_etr_avrg_textBox.Text.Length > 5 && cnt_etr_avrg_textBox.Text != "Min val. 2")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                cnt_etr_avrg_textBox.Text = "Max val. 10 k";
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

        private void radioButton_1s_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 105);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_1s + ";");
            device.giveCommsSemaphore();
        }

        private void radioButton_10s_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 106);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_10s + ";");
            device.giveCommsSemaphore();
        }

        private void radioButton_10m_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 107);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_10m + ";");
            device.giveCommsSemaphore();
        }

        private void radioButton_100m_CheckedChanged(object sender, EventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 108);
            device.send(Commands.COUNTER + ":" + Commands.CNT_GATE + " ");
            device.send(Commands.CNT_GATE_100m + ";");
            device.giveCommsSemaphore();
        }

        private void circBuffer()
        {
            int circBuffSize = Convert.ToInt32(cnt_etr_avrg_textBox.Text);

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

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ---------------------------------------------------- COUNTER REF FUNCTIONS ----------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */          
        /* Function splitting 32 bit number into two 16 bit values that represent ARR and PSC register values send to MCU timer */
        private void splitAndSend(UInt32 sampleCount)
        {
            UInt32 psc = 0, arr = 0;
                        
            if (isPrimeNumber(sampleCount) && sampleCount > 64000)
            {
                cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                cnt_ref_sampleCount_textBox.Text = "A prime number > 64000";
                this.ActiveControl = null;
            }
            else if (sampleCount <= 64000)
            {
                psc = (ushort)sampleCount;
                arr = 1;
            }
            else
            {
                for (int arrTemp = 64000; arrTemp > 1; arrTemp--)
                {
                    if((sampleCount % arrTemp) == 0 && arrTemp <= 64000)
                    {
                        arr = (ushort)arrTemp;
                        break;
                    }
                }              
                                              
                psc = (sampleCount / arr);

                if (psc > 64000)
                {
                    cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                    cnt_ref_sampleCount_textBox.Text = "Can't be devided.";
                    this.ActiveControl = null;
                }
            }

            if ((arr <= 64000) && (psc <= 64000) && (arr > 0) && (psc > 0)) {

                device.takeCommsSemaphore(semaphoreTimeout + 121);

                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_PSC + " ");
                device.send_short((ushort)psc);
                device.send(";");

                Thread.Sleep(100);

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

        private void timerConfig()
        {                      
            timer.Elapsed += new ElapsedEventHandler(timeElapseEvent);            
            timer.AutoReset = false;            
        }

        private void timeElapseEvent(Object source, ElapsedEventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 121);

            if (timScroll == CNT_TIMER.TIM_SCROLL1)
            {
                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_PSC + " ");
                device.send_short(ushort.Parse(cnt_ref_count_textBox1.Text));
                device.send(";");
                timScroll = CNT_TIMER.TIM_ELAPSE1;
                timer.Stop();
            }
            else if (timScroll == CNT_TIMER.TIM_SCROLL2)
            {
                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLES_ARR + " ");
                device.send_short(ushort.Parse(cnt_ref_count_textBox2.Text));
                device.send(";");
                timScroll = CNT_TIMER.TIM_ELAPSE2;
                timer.Stop();
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

            if (cnt_ref_count_textBox1.Text.Length > 10 && cnt_ref_count_textBox1.Text != "Min val. 1")
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

                    if (refSampleCount < 1)
                    {
                        cnt_ref_count_textBox1.ForeColor = Color.DarkRed;
                        cnt_ref_count_textBox1.Text = "Min val. 1";
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
                        cnt_ref_sampleCount_textBox.Text = "" + (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
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
                        cnt_ref_sampleCount_textBox.Text = "" + (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);
                    }
                }
            }
        }

        private void cnt_ref_sampleCount_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            cnt_ref_sampleCount_textBox.ForeColor = SystemColors.WindowFrame;

            TextBox textBox = (TextBox)sender;

            if (cnt_ref_sampleCount_textBox.Text.Length > 10 && cnt_ref_sampleCount_textBox.Text != "Min val. 1")
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

                    if (refSampleCount < 1)
                    {
                        cnt_ref_sampleCount_textBox.ForeColor = Color.DarkRed;
                        cnt_ref_sampleCount_textBox.Text = "Min val. 1";
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

            cnt_ref_sampleCount_textBox.Text = "" + (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);

            timer.Stop();
            timer.Start();

            timScroll = CNT_TIMER.TIM_SCROLL1;
        }

        private void cnt_ref_trackBar2_Scroll(object sender, EventArgs e)
        {           
            cnt_ref_count_textBox1.ForeColor = SystemColors.GrayText;
            cnt_ref_count_textBox1.Text = cnt_ref_trackBar1.Value.ToString();

            cnt_ref_count_textBox2.ForeColor = SystemColors.GrayText;
            cnt_ref_sampleCount_textBox.ForeColor = SystemColors.GrayText;
            cnt_ref_count_textBox2.Text = "" + cnt_ref_trackBar2.Value;

            cnt_ref_sampleCount_textBox.Text = "" + (UInt32)(cnt_ref_trackBar1.Value * cnt_ref_trackBar2.Value);

            timer.Stop();
            timer.Start();            

            timScroll = CNT_TIMER.TIM_SCROLL2;
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
            //cnt_ref_sampleCount_textBox.ForeColor = SystemColors.WindowFrame;
            if (cnt_ref_count_textBox1.Text == "Enter a number")
            {
                cnt_ref_count_textBox1.Text = "";
                cnt_ref_count_textBox1.ForeColor = SystemColors.WindowFrame;
            }
        }

        private void cnt_ref_count_textBox2_Enter(object sender, EventArgs e)
        {
            //cnt_ref_sampleCount_textBox.ForeColor = SystemColors.WindowFrame;
            if (cnt_ref_count_textBox2.Text == "Enter a number")
            {
                cnt_ref_count_textBox2.Text = "";
                cnt_ref_count_textBox2.ForeColor = SystemColors.WindowFrame;
            }
        }

        private void cnt_ref_sampleCount_textBox_Enter(object sender, EventArgs e)
        {
            //cnt_ref_count_textBox1.ForeColor = SystemColors.WindowFrame;
            //cnt_ref_count_textBox2.ForeColor = SystemColors.WindowFrame;
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
            //cnt_ref_sampleCount_textBox.ForeColor = SystemColors.WindowFrame;
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
