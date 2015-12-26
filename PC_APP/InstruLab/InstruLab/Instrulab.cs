﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ZedGraph;
using System.Threading;
using System.Timers;

namespace InstruLab
{
    public partial class Instrulab : Form
    {
        Thread comm_th;
        Comms_thread comms;


        System.Timers.Timer GUITimer;

        public Instrulab()
        {
            InitializeComponent();
            comms = new Comms_thread();
            comm_th = new Thread(new ThreadStart(comms.run));
            comm_th.Priority = System.Threading.ThreadPriority.Highest;
            comm_th.Start();
            GUITimer = new System.Timers.Timer(50);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            
        }

        //invoke updating of GUI during searching of devices
        private void Update_GUI(object sender, ElapsedEventArgs e)
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            switch (comms.get_comms_state())
            {
                case Comms_thread.CommsStates.FINDING:
                    this.toolStripProgressBar.Value = comms.get_progress();
                    this.toolStripStatusLabel.Text = "Searching in progress";
                    break;
                case Comms_thread.CommsStates.NO_DEV_FOUND:
                    this.toolStripProgressBar.Value = 0;
                    this.toolStripStatusLabel.Text = "No device was found";
                    break;
                case Comms_thread.CommsStates.FOUND_DEVS:
                    this.toolStripProgressBar.Value = 0;
                    this.toolStripStatusLabel.Text = "Searching done";
                    this.btn_connect.Enabled = true;
                    this.listBox_devices.Items.Clear();

                    for (int i = 0; i < comms.get_num_of_devices(); i++)
                    {
                        this.listBox_devices.Items.Add(comms.get_dev_names()[i]);
                    }
                    this.listBox_devices.SelectedIndex = 0;
                    break;
                case Comms_thread.CommsStates.CONNECTING:
                    this.toolStripStatusLabel.Text = "Connecting";
                    this.toolStripProgressBar.Value = 80;
                    break;
                case Comms_thread.CommsStates.CONNECTED:
                    this.toolStripProgressBar.Value = 0;
                    this.toolStripStatusLabel.Text = "Connected to "+ comms.get_connected_device_port();
                    this.btn_scope_open.Enabled = comms.get_connected_device().scopeCfg.isScope ? true : false;
                    this.btn_gen_open.Enabled = comms.get_connected_device().genCfg.isGen ? true : false;
                    this.btn_connect.Text = "Disconnect";
                    this.btn_scan.Enabled = false;
                    GUITimer.Stop();

                    string tmpStr = "";
                    
                    // Show device params
                    this.label_device.Text = comms.get_connected_device().get_name();
                    this.label_MCU.Text=comms.get_connected_device().getSystemCfg().MCU;
                    this.label_Freq.Text = (comms.get_connected_device().getSystemCfg().CoreClock/1000000).ToString()+ "MHz";
                    this.label_con1.Text = "UART (" + comms.get_connected_device().getCommsCfg().UartSpeed.ToString() + " baud)";
                    tmpStr = "RX-" + comms.get_connected_device().getCommsCfg().RX_pin + " TX-" + comms.get_connected_device().getCommsCfg().TX_pin;
                    this.label_con2.Text = tmpStr.Replace("_", "");
                    if (comms.get_connected_device().getCommsCfg().useUsb)
                    {
                        this.label_con3.Text = "USB";
                        tmpStr = "DP-" + comms.get_connected_device().getCommsCfg().DP_pin + " DM-" + comms.get_connected_device().getCommsCfg().DM_pin;
                         this.label_con4.Text = tmpStr.Replace("_", "");
                    }
                    else {
                        this.label_con3.Text = "";
                        this.label_con4.Text = "";
                    }

                    this.label_fw.Text = comms.get_connected_device().systemCfg.FW_Version;
                    this.label_RTOS.Text = comms.get_connected_device().systemCfg.FREE_RTOS_Version;
                    this.label_HAL.Text = comms.get_connected_device().systemCfg.HAL_Version;


                    if (comms.get_connected_device().genCfg.isGen)
                    {
                        if (comms.get_connected_device().genCfg.maxSamplingFrequency > 1000000)
                        {
                            this.label_gen_smpl.Text = (comms.get_connected_device().genCfg.maxSamplingFrequency / 1000000).ToString() + " Msps";
                        }
                        else
                        {
                            this.label_gen_smpl.Text = (comms.get_connected_device().genCfg.maxSamplingFrequency / 1000).ToString() + " ksps";
                        }
                        this.label_gen_data_depth.Text = comms.get_connected_device().genCfg.dataDepth.ToString() + " bits";
                        if (comms.get_connected_device().genCfg.BufferLength > 1000)
                        {
                            this.label_gen_buff_len.Text = (comms.get_connected_device().genCfg.BufferLength / 1000).ToString() + "k bytes";
                        }
                        else
                        {
                            this.label_gen_buff_len.Text = (comms.get_connected_device().genCfg.BufferLength).ToString() + " bytes";
                        }
                        this.label_gen_vref.Text = comms.get_connected_device().genCfg.VRef.ToString() + " mV";
                        this.label_gen_channs.Text = comms.get_connected_device().genCfg.numChannels.ToString();
                        tmpStr = "";
                        for (int i = 0; i < comms.get_connected_device().genCfg.numChannels; i++)
                        {
                            tmpStr += comms.get_connected_device().genCfg.pins[i] + ",";
                        }
                        tmpStr = tmpStr.Replace("_", "");
                        this.label_gen_pins.Text = tmpStr.Substring(0, tmpStr.Length - 1);
                    }

                    if (comms.get_connected_device().scopeCfg.isScope)
                    {
                        if (comms.get_connected_device().scopeCfg.maxSamplingFrequency > 1000000)
                        {
                            this.label_scope_smpl.Text = (comms.get_connected_device().scopeCfg.maxSamplingFrequency / 1000000).ToString() + " Msps";
                        }
                        else
                        {
                            this.label_scope_smpl.Text = (comms.get_connected_device().scopeCfg.maxSamplingFrequency / 1000).ToString() + " ksps";
                        }
                        if (comms.get_connected_device().scopeCfg.maxBufferLength > 1000)
                        {
                            this.label_scope_buff_len.Text = (comms.get_connected_device().scopeCfg.maxBufferLength / 1000).ToString() + "k bytes";
                        }
                        else
                        {
                            this.label_scope_buff_len.Text = (comms.get_connected_device().scopeCfg.maxBufferLength).ToString() + " bytes";
                        }

                        this.label_scope_vref.Text = comms.get_connected_device().scopeCfg.VRef.ToString() + " mV";
                        this.label_scope_channs.Text = comms.get_connected_device().scopeCfg.maxNumChannels.ToString();
                        tmpStr = "";
                        for (int i = 0; i < comms.get_connected_device().scopeCfg.maxNumChannels; i++)
                        {
                            tmpStr += comms.get_connected_device().scopeCfg.pins[i] + ",";
                        }
                        tmpStr = tmpStr.Replace("_", "");
                        this.label_scope_pins.Text = tmpStr.Substring(0, tmpStr.Length - 1);
                    }
                    break;
                case Comms_thread.CommsStates.DISCONNECTED:
                    this.btn_connect.Text = "Connect";
                    this.toolStripStatusLabel.Text = "Device was disconnected";
                    this.label_device.Text = "No device connected";
                    this.label_MCU.Text = "--";
                    this.label_Freq.Text = "--";
                    this.label_con1.Text = "--";
                    this.label_con2.Text ="";
                    this.label_con3.Text = "";
                    this.label_con4.Text = "";
                    this.label_gen_smpl.Text = "--";
                    this.label_gen_data_depth.Text = "--";
                    this.label_gen_buff_len.Text = "--";
                    this.label_gen_vref.Text = "--";
                    this.label_gen_channs.Text = "--";
                    this.label_gen_pins.Text = "--";
                    this.label_scope_smpl.Text = "--";
                    this.label_scope_buff_len.Text = "--";
                    this.label_scope_vref.Text = "--";
                    this.label_scope_channs.Text = "--";
                    this.label_scope_pins.Text = "--";
                    this.label_fw.Text = "--";
                    this.label_RTOS.Text = "--";
                    this.label_HAL.Text = "--";

                    this.btn_scope_open.Enabled = false;
                    this.btn_gen_open.Enabled = false;
                    this.btn_scan.Enabled = true;

                    break;
                case Comms_thread.CommsStates.ERROR:
                    this.toolStripStatusLabel.Text = "Some error ocured";
                    break;
            }
        }

        private void btn_scope_open_Click(object sender, EventArgs e)
        {
            comms.get_connected_device().open_scope();
        }

        private void btn_gen_open_Click(object sender, EventArgs e)
        {
            comms.get_connected_device().open_gen();
        }

        private void Instrulab_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (comms.get_connected_device() != null)
            {
                comms.get_connected_device().close_scope();
                comms.get_connected_device().close_gen();
            }


            if (comm_th.IsAlive) {
                comms.stop();
                while (comm_th.IsAlive) {
                    Thread.Yield();
                }
            }
            if (comms.get_comms_state() == Comms_thread.CommsStates.CONNECTED)
            {
                comms.get_connected_device().close_port();
            }
        }

        private void exitInstrulabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_scan_Click(object sender, EventArgs e)
        {
            comms.add_message(new Message(Message.MsgRequest.FIND_DEVICES));
            this.btn_connect.Enabled = false;
            GUITimer.Start();
        }

        private void btn_connect_Click(object sender, EventArgs e)
        {
            if (this.btn_connect.Text.Equals("Connect"))
            {
                string dev = (string)this.listBox_devices.SelectedItem;

                if (dev == null)
                {
                    MessageBox.Show("You have to select device first", "No device selected", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    if (dev[4] == ':')
                    {
                        dev = dev.Substring(0, 4);
                    }
                    else
                    {
                        dev = dev.Substring(0, 5);
                    }
                    comms.add_message(new Message(Message.MsgRequest.CONNECT_DEVICE, dev));
                  //  this.toolStripStatusLabel_status.Text = "Connecting to " + dev;
                   // this.mode = Paint_mode.Mode.CONNECTING;

                }
            }
            else if (this.btn_connect.Text.Equals("Disconnect"))
            {
                comms.disconnect_device();
                GUITimer.Start();
                this.Invalidate();
            }
        }
    }
}
