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
using ZedGraph;

namespace LEO
{
    public partial class BodePlot : Form
    {
        private Device device;
        Device.ScopeConfig_def ScopeDevice;
        System.Timers.Timer GUITimer;
        System.Timers.Timer ZedTimer;
        Message messg;
        public GraphPane freqAnalysisPane;
        Measurements meas = new Measurements(5);

        double fMin = 10;
        double fMax = 100000;
        double numofPoints = 20;
        double offset = 1600;
        double amplitude = 500;

        Generator Gen_form;
        Scope Scope_form;

       // Thread processSignal_th;
       // Thread calcSignal_th;

        public BodePlot(Device dev)
        {
            InitializeComponent();
            Gen_form = new Generator(dev);
            Scope_form = new Scope(dev);
            Scope_form.scope_stop();

            this.device = dev;
            zedGraphControl_freq_analysis.MasterPane[0].IsFontsScaled = false;
            zedGraphControl_freq_analysis.MasterPane[0].Title.IsVisible = false;
            zedGraphControl_freq_analysis.MasterPane[0].XAxis.MajorGrid.IsVisible = true;
            zedGraphControl_freq_analysis.MasterPane[0].XAxis.Title.IsVisible = false;

            zedGraphControl_freq_analysis.IsEnableZoom = false;

            zedGraphControl_freq_analysis.MasterPane[0].YAxis.MajorGrid.IsVisible = true;
            zedGraphControl_freq_analysis.MasterPane[0].YAxis.Title.IsVisible = false;

            ScopeDevice = device.scopeCfg;
            

            
            this.Text = "Scope - (" + device.get_port() + ") " + device.get_name();

            //validate_channel_colors();
            //validate_radio_btns();
            //validate_menu();

            freqAnalysisPane = zedGraphControl_freq_analysis.GraphPane;

            GUITimer = new System.Timers.Timer(5);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            GUITimer.Start();

            ZedTimer = new System.Timers.Timer(50);
            ZedTimer.Elapsed += new ElapsedEventHandler(Zed_update);
            ZedTimer.Start();

           // processSignal_th = new Thread(process_signals);
            meas.clearMeasurements();
        }

        private void Update_GUI(object sender, ElapsedEventArgs e) { 
        
        }

        private void Zed_update(object sender, ElapsedEventArgs e)
        { 
        
        }

        private void textBox_f_min_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                fMin = validateValue(this.textBox_f_min.Text);
                if (fMin == 0) {
                    this.textBox_f_min.Text = "0";
                }
            }
        }

        private void textBox_f_min_Leave(object sender, EventArgs e)
        {
            fMin = validateValue(this.textBox_f_min.Text);
            if (fMin == 0)
            {
                this.textBox_f_min.Text = "0";
            }
        }



        private void textBox_f_max_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                fMax = validateValue(this.textBox_f_max.Text);
                if (fMax == 0)
                {
                    this.textBox_f_max.Text = "0";
                }
            }
        }

        private void textBox_f_max_Leave(object sender, EventArgs e)
        {
            fMax = validateValue(this.textBox_f_max.Text);
            if (fMax == 0)
            {
                this.textBox_f_max.Text = "0";
            }
        }

        private void textBox_num_points_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                numofPoints = validateValue(this.textBox_num_points.Text);
                if (numofPoints == 0)
                {
                    this.textBox_num_points.Text = "0";
                }
            }
        }

        private void textBox_num_points_Leave(object sender, EventArgs e)
        {
            numofPoints = validateValue(this.textBox_num_points.Text);
            if (numofPoints == 0)
            {
                this.textBox_num_points.Text = "0";
            }
        }

        private void textBox_amplitude_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                amplitude = validateValue(this.textBox_amplitude.Text);
                if (amplitude == 0)
                {
                    this.textBox_amplitude.Text = "0";
                }
            }
        }

        private void textBox_amplitude_Leave(object sender, EventArgs e)
        {
            amplitude = validateValue(this.textBox_amplitude.Text);
            if (amplitude == 0)
            {
                this.textBox_amplitude.Text = "0";
            }

        }

        private void textBox_offset_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                offset = validateValue(this.textBox_offset.Text);
                if (offset == 0)
                {
                    this.textBox_offset.Text = "0";
                }
            }
        }

        private void textBox_offset_Leave(object sender, EventArgs e)
        {
                offset = validateValue(this.textBox_offset.Text);
                if (offset == 0)
                {
                    this.textBox_offset.Text = "0";
                }
        }

        private double validateValue(string input) { 
            try
            {
                Double val = Double.Parse(input);
                return val;
            }
            catch (Exception ex)
            {
            }
            return 0;
        }

        private void button_run_Click(object sender, EventArgs e)
        {
            if (button_run.Text.Equals("Run")) {
                Gen_form.setParams(Generator.SIGNAL_TYPE.SINE, amplitude, offset, 200);
                Gen_form.run();
                this.button_run.Text="Update";
                Thread.Sleep(2000);
                Scope_form.setParams(20000, Commands.FREQ_20K, 1000, Scope.TRIG_MODE.AUTO, 2, Commands.CHANNELS_2, 1);
                Scope_form.scope_start();


            }else if (button_run.Text.Equals("Update")) {
                Gen_form.updatefreq(100);
                Thread.Sleep(2000);
                Scope_form.setParams(20000, Commands.FREQ_20K, 1000, Scope.TRIG_MODE.AUTO, 2, Commands.CHANNELS_2, 1);
                Scope_form.scope_start();
                
                this.button_run.Text = "Stop";
            }else if (button_run.Text.Equals("Stop")) {
                Gen_form.stop();
                this.button_run.Text = "Run";
            }
        }

        public void genMessage(Message msg){
            Gen_form.add_message(msg);
        }

        public void scopeMessage(Message msg) {
            Scope_form.add_message(msg);
            if (msg.GetRequest().Equals(Message.MsgRequest.SCOPE_NEW_DATA)) {
                Scope_form.scope_stop();
                Thread.Sleep(1000);
                Console.WriteLine(Scope_form.getMeasFreq(0).ToString());
            }

        }
    }
}
