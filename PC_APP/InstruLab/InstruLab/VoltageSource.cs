﻿using InstruLab;
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
        bool initialized = false;
        int tmpData;

        System.Timers.Timer refreshTimer;



        public VoltageSource(Device dev)
        {
            initialized = false;
            device = dev;
            InitializeComponent();

            this.trackBar_chann_1.Maximum = dev.genCfg.VRef;
            this.trackBar_chann_2.Maximum = dev.genCfg.VRef;
            this.trackBar_chann_1.Value = 0;
            this.trackBar_chann_2.Value = 0;

            refreshTimer = new System.Timers.Timer(100);
            refreshTimer.Elapsed += new ElapsedEventHandler(RefreshDAC);
            refreshTimer.Start();

            numChannels = device.genCfg.numChannels;

            this.groupBox_ch_1.Enabled = numChannels >= 1 ? true : false;
            this.groupBox_ch_2.Enabled = numChannels >= 2 ? true : false;

        }

        private void RefreshDAC(object sender, ElapsedEventArgs e)
        {
            if (voltChann1_old == voltChann1 && voltChann2_old == voltChann2) {
                if (voltActual1 != voltChann1 || voltActual2 != voltChann2) {
                    
                    device.takeCommsSemaphore(semaphoreTimeout + 103);
                    
                    if (!initialized) {
                        if (numChannels >= 2)
                        {
                            device.send(Commands.GENERATOR + ":" + Commands.STOP + ";");
                            device.send(Commands.GENERATOR + ":" + Commands.DATA_LENGTH_CH1 + " ");
                            device.send_short((int)(2));
                            device.send(";");
                            device.send(Commands.GENERATOR + ":" + Commands.CHANNELS + " " + Commands.CHANNELS_2 + ";");
                            device.send(Commands.GENERATOR + ":" + Commands.DATA_LENGTH_CH2 + " ");
                            device.send_short((int)(2));
                            device.send(";");
                        }
                        else {
                            device.send(Commands.GENERATOR + ":" + Commands.STOP + ";");
                            device.send(Commands.GENERATOR + ":" + Commands.DATA_LENGTH_CH1 + " ");
                            device.send_short((int)(2));
                            device.send(";");
                            device.send(Commands.GENERATOR + ":" + Commands.CHANNELS + " " + Commands.CHANNELS_1 + ";");
                        }
                        initialized = true;
                    }
                    else
                    {
                        device.send(Commands.GENERATOR + ":" + Commands.STOP + ";");
                    }        

                    device.send(Commands.GENERATOR + ":" + Commands.SAMPLING_FREQ + " ");
                    device.send_int(10 * 256 + 1);
                    device.send(";");

                    device.send(Commands.GENERATOR + ":" + Commands.GEN_DATA + " ");
                    
                    device.send_int((0 / 256) + (0 % 256) * 256 + (2 * 256 * 256) + (1 * 256 * 256 * 256));
                    device.send(":");
                    
                    tmpData = (int)Math.Round(voltChann1 / device.genCfg.VRef * (Math.Pow(2, device.genCfg.dataDepth) - 1));
                    device.send_short_2byte(tmpData);
                    device.send_short_2byte(tmpData);
                    device.send(";");

                    if (numChannels >= 2) {

                        device.send(Commands.GENERATOR + ":" + Commands.SAMPLING_FREQ + " ");
                        device.send_int(10 * 256 + 2);
                        device.send(";");

                        device.send(Commands.GENERATOR + ":" + Commands.GEN_DATA + " ");

                        device.send_int((0 / 256) + (0 % 256) * 256 + (2 * 256 * 256) + (2 * 256 * 256 * 256));
                        device.send(":");

                        tmpData = (int)Math.Round(voltChann2 / device.genCfg.VRef * (Math.Pow(2, device.genCfg.dataDepth) - 1));
                        device.send_short_2byte(tmpData);
                        device.send_short_2byte(tmpData);
                        device.send(";");
                    
                    }

                    Thread.Sleep(50);
                    device.send(Commands.GENERATOR + ":" + Commands.START + ";");

                    device.giveCommsSemaphore();

                    voltActual1 = voltChann1;
                    voltActual2 = voltChann2;

                }
            }

            voltChann1_old = voltChann1;
            voltChann2_old = voltChann2;
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
        }

        private void textBox_volt_1_KeyPress(object sender, KeyPressEventArgs e)
        {
            validate_text_volt_ch1();
        }

        private void textBox_volt_1_Leave(object sender, EventArgs e)
        {
            validate_text_volt_ch1();
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
        }

        private void textBox_volt_2_Leave(object sender, EventArgs e)
        {
            validate_text_volt_ch2();
        }

        private void trackBar_chann_2_ValueChanged(object sender, EventArgs e)
        {

            voltChann2 = ((double)(this.trackBar_chann_2.Value));
            this.textBox_volt_2.Text = voltChann2.ToString();

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
