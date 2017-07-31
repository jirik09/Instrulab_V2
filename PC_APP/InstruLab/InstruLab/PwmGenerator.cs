using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Timers;
using System.Windows.Forms;

namespace LEO
{
    public partial class PwmGenerator : Form
    {
        Device device;
        int semaphoreTimeout = 4000;
        double cntPaint;

        System.Timers.Timer GUITimer;

        private Queue<Message> cnt_q = new Queue<Message>();
        Message.MsgRequest req;
        Message messg;

        public PwmGenerator(Device dev)
        {
            InitializeComponent();

            device = dev;

            GUITimer = new System.Timers.Timer(50);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            GUITimer.Start();

            PwmGenerator_start();
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
                    //case Message.MsgRequest.XX:
                    //    cntPaint = messg.GetFlt();
                    //    this.Invalidate();
                    //    break;
                }
            }
        }

        void PwmGenerator_start()
        {

        }
    }
}