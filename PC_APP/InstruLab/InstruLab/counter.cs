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

        private Queue<Message> cnt_q = new Queue<Message>();
        Message messg;

        public LinkedList<double> avrgList = new LinkedList<double>();
        LinkedListNode<double> nodeCurrent;
        double average;


        public enum CNT_MODES { IC = 0, ETR, REF };

        public counter(Device dev)
        {
            InitializeComponent();
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
                        cntPaint = messg.GetFlt();
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

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (req == Message.MsgRequest.COUNTER_ETR_DATA)
                {
                    this.label_cnt_etr_value.Text = cntPaint.ToString();
                    this.label_cnt_etr_avrg.Text = average.ToString();
                }
                else if (req == Message.MsgRequest.COUNTER_IC1_DATA)
                {
                    this.label_cnt_ic1_value.Text = cntPaint.ToString();
                }
                else if (req == Message.MsgRequest.COUNTER_IC2_DATA)
                {
                    this.label_cnt_ic2_value.Text = cntPaint.ToString();
                }
                else if (req == Message.MsgRequest.COUNTER_IC1_BUFF)
                {
                    this.cnt_ic1_buffRecalc_textBox.Text = cntPaint.ToString();
                }
                else if (req == Message.MsgRequest.COUNTER_IC2_BUFF)
                {
                    this.cnt_ic2_buffRecalc_textBox.Text = cntPaint.ToString();
                }
                base.OnPaint(e);
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

        // Function handling event from tab selection
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
                    break;
                case 2:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.REF);
                    cnt_start();
                    break;
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
                        cnt_ic1_buffer_textBox.Text = "Max val. 10k";
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
                cnt_ic1_buffer_textBox.Text = "Max val. 10k";
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
                        cnt_ic2_buffer_textBox.Text = "Max val. 10k";
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
                cnt_ic2_buffer_textBox.Text = "Max val. 10k";
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

        private void cnt_ic1_buffer_textBox_Leave(object sender, EventArgs e)
        {
            if (cnt_ic1_buffer_textBox.Text.Length == 0)
            {
                cnt_ic1_buffer_textBox.ForeColor = SystemColors.GrayText;
                cnt_ic1_buffer_textBox.Text = "Enter a number";
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

        private void cnt_etr_avrg_textBox_Click(object sender, EventArgs e)
        {
            cnt_etr_avrg_textBox.Text = "";
        }

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
                    else if (etrbuffer > 10001)
                    {
                        cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                        cnt_etr_avrg_textBox.Text = "Max val. 10k";
                        this.ActiveControl = null;
                    }
                    else
                    {
                        this.ActiveControl = null;
                    }
                }
            }

            if (cnt_etr_avrg_textBox.Text.Length > 5 && cnt_etr_avrg_textBox.Text != "Min val. 2")
            {
                e.KeyChar = (char)Keys.Enter;
                cnt_etr_avrg_textBox.ForeColor = Color.DarkRed;
                cnt_etr_avrg_textBox.Text = "Max val. 10k";
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
    }
}
