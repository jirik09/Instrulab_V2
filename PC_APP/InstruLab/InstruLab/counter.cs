using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows.Forms;

namespace LEO
{
    public partial class counter : Form
    {

        Device device;
        int semaphoreTimeout = 4000;

        double cntPaint;
        //double freqIc1;
        //double freqIc2;
        //double freqRef;
        Message.MsgRequest req;

        System.Timers.Timer GUITimer;

        private Queue<Message> cnt_q = new Queue<Message>();
        Message messg;


        public enum CNT_MODES {IC=0, ETR, REF };              
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
                        nextData();
                        break;
                    case Message.MsgRequest.COUNTER_REF_DATA:
                        nextData();
                        break;
                    case Message.MsgRequest.COUNTER_IC1_DATA:
                        nextData();
                        break;
                    case Message.MsgRequest.COUNTER_IC2_DATA:
                        nextData();
                        break;
                    case Message.MsgRequest.COUNTER_IC1_BUFF:
                        nextData();
                        break;
                    case Message.MsgRequest.COUNTER_IC2_BUFF:
                        nextData();
                        break;
                }
            }
        }

        private void nextData()
        {
            cntPaint = messg.GetFlt();
            this.Invalidate();
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
                    this.label_cnt_etr_value.Text = cntPaint.ToString();
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


        public void cnt_start() {
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


        public void cnt_init_mode(CNT_MODES mode) {
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
                    break;
                case 1:
                    cnt_stop();
                    cnt_init_mode(CNT_MODES.IC);
                    cnt_start();
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

        //this.cnt_ic1_buffer_textBox.KeyPress += this.cnt_ic1_buffer_textBox_KeyPress;
        private void cnt_ic1_buffer_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ushort ic1buffer = Convert.ToUInt16(textBox.Text);

                device.takeCommsSemaphore(semaphoreTimeout + 120);
                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT1 + " ");
                device.send_short(ic1buffer);
                device.send(";");
                device.giveCommsSemaphore();
            }
        }

        //this.cnt_ic2_buffer_textBox.KeyPress += this.cnt_ic2_buffer_textBox_KeyPress;
        private void cnt_ic2_buffer_textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                ushort ic2buffer = Convert.ToUInt16(textBox.Text);

                device.takeCommsSemaphore(semaphoreTimeout + 121);
                device.send(Commands.COUNTER + ":" + Commands.CNT_SAMPLE_COUNT2 + " ");
                device.send_short(ic2buffer);
                device.send(";");
                device.giveCommsSemaphore();
            }
        }
    }    
}
