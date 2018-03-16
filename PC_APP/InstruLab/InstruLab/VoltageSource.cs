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
    public partial class VoltageSource : Form
    {

        private Device device;
        int semaphoreTimeout = 2000;
        double voltChann1 = 0;
        double voltChann2 = 0;

        double voltChann1_old = 0;
        double voltChann2_old = 0;

        double voltActual1 = 0;
        double voltActual2 = 0;

        int numChannels = 0;
        int tmpData;
        int tmpData_ch1;
        int tmpData_ch2;
        int usedVdda = 0;

        bool updated=false;

        System.Timers.Timer refreshTimer;

        private Queue<Message> source_q = new Queue<Message>();
        Message messg;



        public VoltageSource(Device dev)
        {
            //initialized = false;
            device = dev;
            InitializeComponent();

            this.trackBar_chann_1.Maximum = dev.genCfg.VRefMax;
            this.trackBar_chann_2.Maximum = dev.genCfg.VRefMax;
            this.trackBar_chann_1.Minimum = dev.genCfg.VRefMin;
            this.trackBar_chann_2.Minimum = dev.genCfg.VRefMin;
            this.trackBar_chann_1.Value = 0;
            this.trackBar_chann_2.Value = 0;

            refreshTimer = new System.Timers.Timer(200);
            refreshTimer.Elapsed += new ElapsedEventHandler(RefreshDAC);
            refreshTimer.Start();

            numChannels = device.genCfg.numChannels;

            this.groupBox_ch_1.Enabled = numChannels >= 1 ? true : false;
            this.groupBox_ch_2.Enabled = numChannels >= 2 ? true : false;

        }

        private void RefreshDAC(object sender, ElapsedEventArgs e)
        {
            if (source_q.Count > 0)
            {
                messg = source_q.Dequeue();
                if (messg == null)
                {
                    return;
                }
                switch (messg.GetRequest())
                {
                    case Message.MsgRequest.GEN_OK:
                        updated = true;
                        this.Invalidate();
                        break;
                }
            }
            if (voltChann1_old == voltChann1 && voltChann2_old == voltChann2) {
                if (voltActual1 != voltChann1 || voltActual2 != voltChann2) {
                    
                    device.takeCommsSemaphore(semaphoreTimeout + 103);
                    device.send(Commands.GENERATOR + ":" + Commands.GEN_DAC_VAL + " ");
                    usedVdda = device.systemCfg.VDDA_actual;
                    tmpData_ch1 = (int)Math.Round(voltChann1 / (device.genCfg.VRefMax - device.genCfg.VRefMin) * (Math.Pow(2, device.genCfg.dataDepth)) * device.systemCfg.VDDA_target / device.systemCfg.VDDA_actual + (Math.Pow(2, device.genCfg.dataDepth - 1)) - (device.genCfg.VRefMax + device.genCfg.VRefMin) / (device.genCfg.VRefMax - device.genCfg.VRefMin) / (Math.Pow(2, device.genCfg.dataDepth - 1)));
                    tmpData_ch2 = (int)Math.Round(voltChann2 / (device.genCfg.VRefMax - device.genCfg.VRefMin) * (Math.Pow(2, device.genCfg.dataDepth)) * device.systemCfg.VDDA_target / device.systemCfg.VDDA_actual + (Math.Pow(2, device.genCfg.dataDepth - 1)) - (device.genCfg.VRefMax + device.genCfg.VRefMin) / (device.genCfg.VRefMax - device.genCfg.VRefMin) / (Math.Pow(2, device.genCfg.dataDepth - 1)));

                    // invert whe shield present because it has inverting amplifier
                    if (device.systemCfg.isShield) {
                        tmpData_ch1 = (int)(Math.Pow(2, device.genCfg.dataDepth) - 1) - tmpData_ch1;
                        tmpData_ch2 = (int)(Math.Pow(2, device.genCfg.dataDepth) - 1) - tmpData_ch2;
                    }

                    if (tmpData_ch1 > (Math.Pow(2, device.genCfg.dataDepth) - 1)) {
                        tmpData_ch1 = (int)(Math.Pow(2, device.genCfg.dataDepth) - 1);
                    }
                    if (tmpData_ch2 > (Math.Pow(2, device.genCfg.dataDepth) - 1))
                    {
                        tmpData_ch2 = (int)(Math.Pow(2, device.genCfg.dataDepth) - 1);
                    }
                    tmpData = tmpData_ch1 + tmpData_ch2 * (int)(Math.Pow(2, 16));
                    device.send_int((int)(tmpData));
                    device.send(";");

                    device.giveCommsSemaphore();

                    voltActual1 = voltChann1;
                    voltActual2 = voltChann2;
                    this.Invalidate();
                }
            }
            voltChann1_old = voltChann1;
            voltChann2_old = voltChann2;
        }

        public void add_message(Message msg)
        {
            this.source_q.Enqueue(msg);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            if (updated)
            {
                this.panel_status.BackColor = Color.LightGreen;
                this.label_used_vdda.Text = "Used Vdda " + usedVdda.ToString() + " mV";
                this.label_ch1_volt.Text = voltActual1 + " mV";
                this.label_ch2_volt.Text = voltActual2 + " mV";
            }
            base.OnPaint(e);
        }

        private void VoltageSource_FormClosing(object sender, FormClosingEventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.GENERATOR + ":" + Commands.STOP + ";");
            device.giveCommsSemaphore();
            device.sourceClosed();   
        }

        private void trackBar_chann_1_ValueChanged(object sender, EventArgs e)
        {

            voltChann1 = ((double)(this.trackBar_chann_1.Value));
            this.textBox_volt_1.Text = voltChann1.ToString();
            this.panel_status.BackColor = Color.Red;
            updated = false;
        }

        private void textBox_volt_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            validate_text_volt_ch1();
            this.panel_status.BackColor = Color.Red;
            updated = false;
        }

        private void textBox_volt_1_Leave(object sender, EventArgs e)
        {
            validate_text_volt_ch1();
            this.panel_status.BackColor = Color.Red;
            updated = false;
        }

        private void validate_text_volt_ch1()
        {
            try
            {
                Double val = Double.Parse(this.textBox_volt_1.Text);

                this.trackBar_chann_1.Value = (int)(val);
                voltChann1 = val;

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            finally
            {
                this.trackBar_chann_1.Text = voltChann1.ToString();
            }
        }

        private void textBox_volt_2_KeyPress(object sender, KeyPressEventArgs e)
        {
            validate_text_volt_ch2();
            this.panel_status.BackColor = Color.Red;
            updated = false;
        }

        private void textBox_volt_2_Leave(object sender, EventArgs e)
        {
            validate_text_volt_ch2();
            this.panel_status.BackColor = Color.Red;
            updated = false;
        }

        private void trackBar_chann_2_ValueChanged(object sender, EventArgs e)
        {

            voltChann2 = ((double)(this.trackBar_chann_2.Value));
            this.textBox_volt_2.Text = voltChann2.ToString();
            this.panel_status.BackColor = Color.Red;
            updated = false;

        }

        private void validate_text_volt_ch2()
        {
            try
            {
                Double val = Double.Parse(this.textBox_volt_2.Text);

                this.trackBar_chann_2.Value = (int)(val);
                voltChann2 = val;

            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
            finally
            {
                this.trackBar_chann_2.Text = voltChann2.ToString();
            }
        }


    }
}
