using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private Message messg_bode;
        public GraphPane freqAnalysisPane;
        Measurements meas = new Measurements(12);
        Thread calcSignal_th;
        int semaphoreTimeout = 3400;

        private Queue<Message> bode_q = new Queue<Message>();
        Message messg;

        private double fMin = 1;
        private double fMax = 100000;
        private int numofPoints = 5;
        private double offset = 1600;
        private double amplitude = 500;

        private double[] frequencies;
        private double[] amplitudes_Input;
        private double[] amplitudes_Output1;
        private double[] amplitudes_Output2;
        private double[] amplitudes_Output3;
        private double[] frequency_input;
        private double[] frequency_output;
        private double[] phase_output;
        private double[] amplitude_decibles1;
        private int measurement_index;
        private int average_index;
        private bool measuring = false;
        private double real_freq;
        private double freq_err = 5; //max freq error in %
        Generator Gen_form;

        private int VerticalCursorSrc = 0;
        private double VerticalCursorA = 0;
        private double VerticalCursorB = 0;

        private double VerticalCursorA_last = 0;
        private double VerticalCursorB_last = 0;

        double minY;
        double maxY;
        double freqA;
        double freqB;
        double phaseA;
        double phaseB;

        private string str_freqDif;
        private string str_freqA;
        private string str_freqB;
        private string str_amplDif;
        private string str_amplA;
        private string str_amplB;
        private string str_phaseDif;
        private string str_phaseA;
        private string str_phaseB;


       // Thread processSignal_th;
       // Thread calcSignal_th;

        public BodePlot(Device dev)
        {
            InitializeComponent();
            Gen_form = new Generator(dev);
            measurement_index = numofPoints;

            this.textBox_amplitude.Text = amplitude.ToString();
            this.textBox_offset.Text = offset.ToString();
            this.textBox_f_max.Text=fMax.ToString();
            this.textBox_f_min.Text = fMin.ToString();
            this.textBox_num_points.Text = numofPoints.ToString();

            this.device = dev;
            zedGraphControl_freq_analysis.MasterPane[0].IsFontsScaled = false;
            zedGraphControl_freq_analysis.MasterPane[0].Title.IsVisible = false;
            zedGraphControl_freq_analysis.MasterPane[0].XAxis.MajorGrid.IsVisible = true;
            zedGraphControl_freq_analysis.MasterPane[0].XAxis.Title.IsVisible = false;

            zedGraphControl_freq_analysis.IsEnableZoom = false;

            zedGraphControl_freq_analysis.MasterPane[0].YAxis.MajorGrid.IsVisible = true;
            zedGraphControl_freq_analysis.MasterPane[0].YAxis.Title.IsVisible = false;

            ScopeDevice = device.scopeCfg;
            

            
            this.Text = "Freq. analysis - (" + device.get_port() + ") " + device.get_name();

            meas.clearMeasurements();
            meas.addMeasurement(0, Measurements.MeasurementTypes.FREQUENCY);
            meas.addMeasurement(0, Measurements.MeasurementTypes.PKPK);
            meas.addMeasurement(0, Measurements.MeasurementTypes.PHASE);
            meas.addMeasurement(1, Measurements.MeasurementTypes.FREQUENCY);
            meas.addMeasurement(1, Measurements.MeasurementTypes.PKPK);
            meas.addMeasurement(1, Measurements.MeasurementTypes.PHASE);
            meas.addMeasurement(2, Measurements.MeasurementTypes.FREQUENCY);
            meas.addMeasurement(2, Measurements.MeasurementTypes.PKPK);
            meas.addMeasurement(2, Measurements.MeasurementTypes.PHASE);
            meas.addMeasurement(3, Measurements.MeasurementTypes.FREQUENCY);
            meas.addMeasurement(3, Measurements.MeasurementTypes.PKPK);

            freqAnalysisPane = zedGraphControl_freq_analysis.GraphPane;

            GUITimer = new System.Timers.Timer(5);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            GUITimer.Start();

            ZedTimer = new System.Timers.Timer(50);
            ZedTimer.Elapsed += new ElapsedEventHandler(Zed_update);
            //ZedTimer.Start();



            device.takeCommsSemaphore(semaphoreTimeout + 109);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_ADC_CHANNEL_DEAFULT + ";");
            //device.send(Commands.SCOPE + ":" + Commands.SAMPLING_FREQ + " " + Commands.FREQ_5K + ";");
            //device.send(Commands.SCOPE + ":" + Commands.DATA_LENGTH + " " + Commands.SAMPLES_200 + ";");
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_MODE + " " + Commands.MODE_AUTO + ";");
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_DATA_DEPTH + " " + Commands.DATA_DEPTH_12B + ";");

            switch (device.scopeCfg.maxNumChannels)
            {
                case 2:
                    device.send(Commands.SCOPE + ":" + Commands.CHANNELS + " " + Commands.CHANNELS_2 + ";");
                    break;
                case 3:
                    device.send(Commands.SCOPE + ":" + Commands.CHANNELS + " " + Commands.CHANNELS_3 + ";");
                    break;
                case 4:
                    device.send(Commands.SCOPE + ":" + Commands.CHANNELS + " " + Commands.CHANNELS_4 + ";");
                    break;
            }

            Thread.Sleep(100);

            device.giveCommsSemaphore();
        }

        private void Update_GUI(object sender, ElapsedEventArgs e) {
            if (bode_q.Count > 0)
            {
                messg_bode = bode_q.Dequeue();
                if (messg_bode == null)
                {
                    return;
                }
                switch (messg_bode.GetRequest())
                {
                    case Message.MsgRequest.BODE_NEW_DATA:
                        if (calcSignal_th != null && calcSignal_th.IsAlive)
                        {
                            calcSignal_th.Join();
                        }

                            //calcSignal_th = new Thread(() => meas.calculateMeasurements(device.scopeCfg.samples, rangeMax, rangeMin, device.scopeCfg.actualChannels, device.scopeCfg.realSmplFreq, device.scopeCfg.timeBase.Length, device.scopeCfg.actualRes));
                            calcSignal_th = new Thread(() => meas.calculateMeasurements(device.scopeCfg.samples, 1000, 0, device.scopeCfg.actualChannels, device.scopeCfg.realSmplFreq, device.scopeCfg.timeBase.Length, device.scopeCfg.actualRes));

                            calcSignal_th.Start();
                            Thread.Sleep(1);
                            if (calcSignal_th.IsAlive)
                            {
                                calcSignal_th.Join();
                            }


                            amplitudes_Input[measurement_index] = meas.getPkPk(0,1000,0,12);
                            amplitudes_Output1[measurement_index] = meas.getPkPk(1, 1000, 0, 12);
                            amplitudes_Output2[measurement_index] = meas.getPkPk(2, 1000, 0, 12);
                            amplitudes_Output3[measurement_index] = device.scopeCfg.realSmplFreq;
                            frequency_input[measurement_index] = meas.getFreq(0);
                            frequency_output[measurement_index] = meas.getFreq(1);
                            phase_output[measurement_index] = -meas.getPhaseDeg(0,1,device.scopeCfg.realSmplFreq,meas.getFreq(0));
                            amplitude_decibles1[measurement_index] = Math.Log10(amplitudes_Output1[measurement_index] / amplitudes_Input[measurement_index]) * 20.0; 

                            //channels = device.scopeCfg.actualChannels;

                            //vdda[0] = device.scopeCfg.VRefInt / (meas.getMean(0) * 1000) * device.scopeCfg.VRef / 1000;
                            //vdda[1] = device.scopeCfg.VRefInt / (meas.getMean(1) * 1000) * device.scopeCfg.VRef / 1000;
                            //vdda[2] = device.scopeCfg.VRefInt / (meas.getMean(2) * 1000) * device.scopeCfg.VRef / 1000;
                            //vdda[3] = device.scopeCfg.VRefInt / (meas.getMean(3) * 1000) * device.scopeCfg.VRef / 1000;

                            //device.scopeCfg.VDDA = (int)(vdda[0] * 1000);
                            //device.genCfg.VDDA = (int)(vdda[0] * 1000);
                            
                            if (measurement_index > 0)
                            {
                                measurement_index--;
                                Gen_form.updatefreq(frequencies[measurement_index]);
                                Thread.Sleep(50);
                                real_freq=Gen_form.getfrequency();
                                double frac = real_freq / frequencies[measurement_index];

                                if (frac > (1 + freq_err / 100) || frac < (1 - freq_err / 100) || Gen_form.getsignallength()<100)
                                {
                                    Gen_form.stop();
                                    Gen_form.setParams(Generator.SIGNAL_TYPE.SINE, amplitude, offset, frequencies[measurement_index]);
                                    Gen_form.run();
                                }
                                while (!Gen_form.is_generating())
                                {
                                    Thread.Sleep(10);
                                }
                                scope_stop();
                                scope_update_sampling(frequencies[measurement_index]);
                                scope_start();
                                Zed_update(null, null);
                                this.Invalidate();
                            }
                            else
                            {
                                Gen_form.stop();
                                scope_stop();
                                measuring = false;
                                Zed_update(null, null);
                                this.Invalidate();
                            }

                        break;
                    case Message.MsgRequest.BODE_START_MEAS:
                        measuring = true;
                        Gen_form.stop();
                        Gen_form.setParams(Generator.SIGNAL_TYPE.SINE, amplitude, offset, frequencies[measurement_index]);
                        Gen_form.run();
                        while (!Gen_form.is_generating()) {
                            Thread.Sleep(10);
                        }
                        scope_stop();
                        scope_update_sampling(frequencies[measurement_index]);
                        scope_start();
                        
                        break;
                }
                Thread.Sleep(10);
            }

            if (VerticalCursorA != VerticalCursorA_last || VerticalCursorB != VerticalCursorB_last)
            {
                VerticalCursorA_last = VerticalCursorA;
                VerticalCursorB_last = VerticalCursorB;
                Zed_update(null, null);
                this.Invalidate();
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {

            if (measuring) { 
                this.label_info.Text = "Measurement " + (numofPoints - measurement_index) + "/" + numofPoints;
                this.button_run.Text = "Stop";
                zedGraphControl_freq_analysis.Refresh();
            }
            else if (!measuring && measurement_index <= 0)
            {
                this.button_run.Text = "Run";
                this.label_info.Text = "Done";
                controls(true);
                zedGraphControl_freq_analysis.Refresh();
            }
            else {
                this.label_info.Text = "Estimated time:" + numofPoints * 3 + "s";
            }

            if (VerticalCursorSrc != 0)
            {
                this.label_cur_a_ampl.Text = str_amplA;
                this.label_cur_b_ampl.Text = str_amplB;
                this.label_cur_da.Text = str_amplDif;

                this.label_cur_a_freq.Text = str_freqA;
                this.label_cur_b_freq.Text = str_freqB;
                this.label_cur_df.Text = str_freqDif;

                this.label_cur_a_phase.Text = str_phaseA;
                this.label_cur_b_phase.Text = str_phaseB;
                this.label_cur_dphi.Text = str_phaseDif;
                zedGraphControl_freq_analysis.Refresh();
            }



            base.OnPaint(e);
        }

        private void Zed_update(object sender, ElapsedEventArgs e)
        {
            freqAnalysisPane.CurveList.Clear();
            LineItem curve;

            curve = freqAnalysisPane.AddCurve("", frequencies, amplitude_decibles1, Color.Red, SymbolType.Diamond);
            curve.Line.IsSmooth = false;
            curve.Line.SmoothTension = 0.5F;
            curve.Line.IsAntiAlias = false;
            curve.Line.IsOptimizedDraw = true;
            curve.Symbol.Size = 4;

            curve = freqAnalysisPane.AddCurve("", frequencies, phase_output, Color.Red, SymbolType.Diamond);
            curve.IsY2Axis = true;
            curve.Line.Style = DashStyle.Custom;
            curve.Line.DashOn = 10;
            curve.Line.DashOff = 5;
            curve.Line.IsSmooth = false;
            curve.Line.SmoothTension = 0.5F;
            curve.Line.IsAntiAlias = false;
            curve.Line.IsOptimizedDraw = true;
            curve.Symbol.Size = 4;

            

            freqAnalysisPane.XAxis.Scale.MaxAuto = false;
            freqAnalysisPane.XAxis.Scale.MinAuto = false;
            freqAnalysisPane.YAxis.Scale.MaxAuto = false;
            freqAnalysisPane.YAxis.Scale.MinAuto = false;
            freqAnalysisPane.Y2Axis.IsVisible = true;
            freqAnalysisPane.Y2Axis.Scale.MaxAuto = false;
            freqAnalysisPane.Y2Axis.Scale.MinAuto = false;
            //freqAnalysisPane.Y2Axis.MajorGrid.IsZeroLine = false;
            freqAnalysisPane.YAxis.MinorGrid.IsVisible = true;
            freqAnalysisPane.XAxis.MinorGrid.IsVisible = true;

            freqAnalysisPane.XAxis.MinorGrid.DashOn = 2;
            freqAnalysisPane.XAxis.MinorGrid.DashOff = 10;
            freqAnalysisPane.YAxis.MinorGrid.DashOn = 2;
            freqAnalysisPane.YAxis.MinorGrid.DashOff = 10;


            freqAnalysisPane.XAxis.MajorGrid.DashOn = 10;
            freqAnalysisPane.XAxis.MajorGrid.DashOff = 5;
            freqAnalysisPane.YAxis.MajorGrid.DashOn = 10;
            freqAnalysisPane.YAxis.MajorGrid.DashOff = 5;

            freqAnalysisPane.XAxis.Scale.Max = frequencies.Max();
            freqAnalysisPane.XAxis.Scale.Min = frequencies.Min();
            freqAnalysisPane.YAxis.Scale.Max = amplitude_decibles1.Max() + 1;
            freqAnalysisPane.YAxis.Scale.Min = amplitude_decibles1.Min() - 1;
            minY = amplitude_decibles1.Min() - 1;
            maxY = amplitude_decibles1.Max() + 1;
            freqAnalysisPane.Y2Axis.Scale.Max = phase_output.Max() + 15;
            freqAnalysisPane.Y2Axis.Scale.Min = phase_output.Min() - 15;

            freqAnalysisPane.Y2Axis.Scale.Align = AlignP.Inside;
            freqAnalysisPane.YAxis.Scale.Align = AlignP.Inside;

            freqAnalysisPane.XAxis.Type = AxisType.Log;
            freqAnalysisPane.AxisChange();

            paint_cursors();
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
                numofPoints = (int)validateValue(this.textBox_num_points.Text);
                if (numofPoints == 0)
                {
                    this.textBox_num_points.Text = "0";
                }
            }
            this.Invalidate();
        }

        private void textBox_num_points_Leave(object sender, EventArgs e)
        {
            numofPoints = (int)validateValue(this.textBox_num_points.Text);
            if (numofPoints == 0)
            {
                this.textBox_num_points.Text = "0";
            }
            this.Invalidate();
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
            if (button_run.Text.Equals("Run"))
            {

                measurement_index = numofPoints + 1;
                bool logaritmic = radioButton_log.Checked;

                frequencies = new double[measurement_index];
                amplitudes_Input = new double[measurement_index];
                amplitudes_Output1 = new double[measurement_index];
                phase_output = new double[measurement_index];
                amplitudes_Output2 = new double[measurement_index];
                amplitudes_Output3 = new double[measurement_index];
                frequency_input = new double[measurement_index];
                frequency_output = new double[measurement_index];
                amplitude_decibles1 = new double[measurement_index];


                double q = Math.Log10(fMax / fMin) / (measurement_index - 1);

                for (int i = 0; i < measurement_index; i++)
                {
                    if (logaritmic)
                    {
                        frequencies[i] = fMin * Math.Pow(10, q * i);
                    }
                    else
                    {
                        frequencies[i] = fMin + (fMax - fMin) * i / (measurement_index - 1);
                    }
                }
                measurement_index--;

                add_message(new Message(Message.MsgRequest.BODE_START_MEAS));
                controls(false);

            }
            else if (button_run.Text.Equals("Stop"))
            {
                measurement_index = 0;
            }
        }

        public void genMessage(Message msg){
            Gen_form.add_message(msg);
        }

        public void add_message(Message msg)
        {
            this.bode_q.Enqueue(msg);
        }

        //set scope params to catch >10 periods of signal
        public void scope_update_sampling(double freq)
        {
            double time = (1 / freq) * 10;
            string scope_smapling = get_sampling_freq_string((int)freq * 10 * 200);

            device.takeCommsSemaphore(semaphoreTimeout + 109);
            device.send(Commands.SCOPE + ":" + Commands.SAMPLING_FREQ + " " + scope_smapling + ";");
            device.send(Commands.SCOPE + ":" + Commands.DATA_LENGTH + " " + Commands.SAMPLES_2K + ";");
            device.giveCommsSemaphore();

        }

        public string get_sampling_freq_string(int freq) {
            string result = "";
            if (freq > device.scopeCfg.maxSamplingFrequency) {
                freq = device.scopeCfg.maxSamplingFrequency;
            }

            if (freq > 10000000)
            {
                result = Commands.FREQ_10M;
            }
            else if (freq > 5000000)
            {
                result = Commands.FREQ_5M;
            }
            else if (freq > 2000000)
            {
                result = Commands.FREQ_2M;
            }
            else if (freq > 1000000)
            {
                result = Commands.FREQ_1M;
            }
            else if (freq > 500000)
            {
                result = Commands.FREQ_500K;
            }
            else if (freq > 200000)
            {
                result = Commands.FREQ_200K;
            }
            else if (freq > 100000)
            {
                result = Commands.FREQ_100K;
            }
            else if (freq > 50000)
            {
                result = Commands.FREQ_50K;
            }
            else if (freq > 20000)
            {
                result = Commands.FREQ_20K;
            }
            else if (freq > 10000)
            {
                result = Commands.FREQ_10K;
            }
            else if (freq > 5000)
            {
                result = Commands.FREQ_5K;
            }
            else if (freq > 2000)
            {
                result = Commands.FREQ_2K;
            }
            else
            {
                result = Commands.FREQ_1K;
            }
            

            return result;
        }


        public void scope_start()
        {
            device.takeCommsSemaphore(semaphoreTimeout + 109);
            device.send(Commands.SCOPE + ":" + Commands.START + ";");
            device.giveCommsSemaphore();
        }

        public void scope_stop()
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.SCOPE + ":" + Commands.STOP + ";");
            device.giveCommsSemaphore();
        }
        public void scope_next()
        {
            device.takeCommsSemaphore(semaphoreTimeout + 99);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_NEXT + ";");
            device.giveCommsSemaphore();
        }

        public void controls(bool en) {
            this.textBox_amplitude.Enabled = en;
            this.textBox_f_max.Enabled = en;
            this.textBox_f_min.Enabled = en;
            this.textBox_num_points.Enabled = en;
            this.textBox_offset.Enabled = en;
            this.radioButton_avg1.Enabled = en;
            this.radioButton_avg2.Enabled = en;
            this.radioButton_avg4.Enabled = en;
            this.radioButton_avg8.Enabled = en;

            this.radioButton_lin.Enabled = en;
            this.radioButton_log.Enabled = en;
            
        }

        private void radioButton_ver_cur_on_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_ver_cur_on.Checked)
            {
                VerticalCursorSrc = 1;
                vertical_cursor_update();
                validate_vertical_curr();
                Zed_update(null, null);
                this.Invalidate();
            }
        }

        private void radioButton_ver_cur_off_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_ver_cur_off.Checked)
            {
                VerticalCursorSrc = 0;
                vertical_cursor_update();
                validate_vertical_curr();
                Zed_update(null, null);
                this.Invalidate();
            }
        }

        public void vertical_cursor_update()
        {
            if (VerticalCursorSrc != 0)
            {
                //vypocet casu
                freqA = VerticalCursorA * fMax + (1 - VerticalCursorA) * fMin;
                freqB = VerticalCursorB * fMax + (1 - VerticalCursorB) * fMin;

                int indexAA = 0;
                int indexAB = 0;

                for (int i = 0; i < numofPoints; i++)
                {
                    if (freqA >= frequencies[i])
                    {
                        indexAA = i;
                    }
                    if (freqB >= frequencies[i])
                    {
                        indexAB = i;
                    }
                }

                double AmplA = 0;
                double AmplB = 0;
                if (indexAA >= numofPoints)
                {
                    indexAA = numofPoints - 1;
                }
                if (indexAB >= numofPoints)
                {
                    indexAB = numofPoints - 1;
                }
                //vypocet linearni interpolace napeti kurzoru  //nefunguje ptz je to logaritmicke

                if (indexAA < numofPoints - 1)
                {
                    AmplA = (amplitude_decibles1[indexAA]) + (freqA - frequencies[indexAA]) / (frequencies[indexAA + 1] - frequencies[indexAA]) * ((amplitude_decibles1[indexAA + 1]) - (amplitude_decibles1[indexAA]));
                    phaseA = (phase_output[indexAA]) + (phaseA - frequencies[indexAA]) / (frequencies[indexAA + 1] - frequencies[indexAA]) * ((phase_output[indexAA + 1]) - (phase_output[indexAA]));
                }
                else
                {
                    AmplA = (amplitude_decibles1[indexAA - 1]);
                    phaseA = (phase_output[indexAA - 1]);
                }

                if (indexAB < numofPoints - 1)
                {
                    AmplB = (amplitude_decibles1[indexAB]) + (freqB - frequencies[indexAB]) / (frequencies[indexAB + 1] - frequencies[indexAB]) * ((amplitude_decibles1[indexAB + 1]) - (amplitude_decibles1[indexAB]));
                    phaseB = (phase_output[indexAB]) + (phaseB - frequencies[indexAB]) / (frequencies[indexAB + 1] - frequencies[indexAB]) * ((phase_output[indexAB + 1]) - (phase_output[indexAB]));
                }
                else
                {
                    AmplB = (amplitude_decibles1[indexAB - 1]);
                    phaseB = (phase_output[indexAB - 1]);
                }



                double freq_diff = freqA - freqB;
                double ampl_diff = AmplA - AmplB;
                double phase_diff = phaseA - phaseB;

                //formatovani stringu
                if (freq_diff >= 1000 || freq_diff <= -1000)
                {
                    this.str_freqDif = "df " + (Math.Round(freq_diff / 1000, 3)).ToString() + " kHz";
                }
                else
                {
                    this.str_freqDif = "df " + (Math.Round(freq_diff, 3)).ToString() + " Hz";
                }

                if (freqA >= 1000 || freqA <= -1000)
                {
                    this.str_freqA = "f " + (Math.Round(freqA / 1000, 3)).ToString() + " kHz";
                }
                else
                {
                    this.str_freqA = "f " + (Math.Round(freqA, 3)).ToString() + " Hz";
                }

                if (freqB >= 1000 || freqB <= -1000)
                {
                    this.str_freqB = "f " + (Math.Round(freqB / 1000, 3)).ToString() + " kHz";
                }
                else
                {
                    this.str_freqB = "f " + (Math.Round(freqB, 3)).ToString() + " Hz";
                }

                this.str_amplA = "A " + (Math.Round(AmplA, 3)).ToString() + " dB";
                this.str_amplB = "A " + (Math.Round(AmplB, 3)).ToString() + " dB";
                this.str_amplDif = "A " + (Math.Round(ampl_diff, 3)).ToString() + " dB";


                this.str_phaseA = "Phi " + (Math.Round(phaseA, 3)).ToString() + " °";
                this.str_phaseB = "Phi " + (Math.Round(phaseB, 3)).ToString() + " °";
                this.str_phaseDif = "Phi " + (Math.Round(phase_diff, 3)).ToString() + " °";
            }
        }


        public void validate_vertical_curr()
        {
            this.trackBar_ver_cur_a.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.trackBar_ver_cur_b.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_b_freq.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_b_ampl.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_b_phase.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_a_freq.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_a_ampl.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_a_phase.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_da.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_df.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_dphi.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label5.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label6.Enabled = VerticalCursorSrc == 0 ? false : true;
        }

        private void trackBar_ver_cur_a_ValueChanged(object sender, EventArgs e)
        {
            //VerticalCursorA = Math.Log10((double)(this.trackBar_ver_cur_a.Value)) / 4;
            double q = Math.Log10(fMax / fMin);
            VerticalCursorA = fMin * Math.Pow(10, q * (double)(this.trackBar_ver_cur_a.Value) / 10000) / fMax;
            Console.WriteLine(VerticalCursorA.ToString());
            vertical_cursor_update();
        }

        private void trackBar_ver_cur_b_ValueChanged(object sender, EventArgs e)
        {
            double q = Math.Log10(fMax / fMin);
            VerticalCursorB = fMin * Math.Pow(10, q * (double)(this.trackBar_ver_cur_b.Value) / 10000) / fMax;
            vertical_cursor_update();
        }

        public void paint_cursors()
        {

            if (VerticalCursorSrc != 0)
            {
                Color col;
                switch (VerticalCursorSrc)
                {
                    case 1:
                        col = Color.Red;
                        break;
                    case 2:
                        col = Color.Blue;
                        break;
                    case 3:
                        col = Color.DarkGreen;
                        break;
                    case 4:
                        col = Color.Magenta;
                        break;
                    default:
                        col = Color.Black;
                        break;
                }
                LineItem curve;
                PointPairList list1 = new PointPairList();

                list1 = new PointPairList();
                list1.Add(freqA, minY);
                list1.Add(freqA, maxY);
                curve = freqAnalysisPane.AddCurve("", list1, col, SymbolType.HDash);
                curve.Symbol.Size = 0;
                curve.Line.IsSmooth = true;


                list1 = new PointPairList();
                list1.Add(freqB, minY);
                list1.Add(freqB, maxY);
                curve = freqAnalysisPane.AddCurve("", list1, col, SymbolType.HDash);
                curve.Symbol.Size = 0;
                curve.Line.IsSmooth = true;
            }
        }





    }
}
