using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text.RegularExpressions;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using ZedGraph;

namespace LEO
{
    public partial class LogicAnalyzer : Form
    {
        Device device;
        
        System.Timers.Timer GUITimer;
        System.Timers.Timer SignalTimer;
        static Semaphore logAnlysSemaphore = new Semaphore(1, 1);
        int semaphoreTimeout = 4000;

        private Queue<Message> logAnlys_q = new Queue<Message>();
        Message message;
        Message.MsgRequest req;
//        double cntPaint;

//        LineItem curve_ch1, curve_ch2, curve_ch3, curve_ch4, curve_ch5, curve_ch6, curve_ch7, curve_ch8;
        public GraphPane logAnlysPane;

        double[] timeAxis;
        double[] signal_ch1, signal_ch2, signal_ch3, signal_ch4, signal_ch5, signal_ch6, signal_ch7, signal_ch8;

        private static string[] logAnlysPins = new string[8];
        private static uint posttrigPeriphClock; // 
        private static uint timeBasePeriphClock;
        private int triggerPointer;    

        private double last_sum1 = 0;
//        private double last_sum2 = 0;
        public uint dataLength = 1000;
        public uint samplingFreq = 10000;
        public uint pretrig = 50; // [%]
        public uint lastPretrig = 50;
        private double realSamplingFreq = 10000;
        private ushort firstGpioPin;       
        public uint syncPwmArr = 0;
        public uint syncPwmPsc = 0;

        public enum TRIGGER_MODE { AUTO, NORMAL, SINGLE, };
        TRIGGER_MODE triggerMode;

        public LogicAnalyzer(Device dev)
        {
            InitializeComponent();
            InitializeZedGraphComponent();
            ShowPinsInComponent();
            /* PC Logic Analyzer app works only if all GPIO pins of LA are on the same port, 
            are next to each other on the port and also the MCU firmware sends pins ascending
            in config request.  */
            getFirstGPIOPin();
            device = dev;

            SignalTimer = new System.Timers.Timer(100);
            SignalTimer.Elapsed += new ElapsedEventHandler(Update_signal);
            SignalTimer.Start();

            GUITimer = new System.Timers.Timer(20);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            GUITimer.Start();

            LogAnlys_Init();
            Thread.Sleep(100);
            LogAnlys_Start();
        }

        public void ShowPinsInComponent()
        {
            this.label_color_ch1.Text = "Ch 1 (" + logAnlysPins[0] + ")";
            this.label_color_ch2.Text = "Ch 2 (" + logAnlysPins[1] + ")";
            this.label_color_ch3.Text = "Ch 3 (" + logAnlysPins[2] + ")";
            this.label_color_ch4.Text = "Ch 4 (" + logAnlysPins[3] + ")";
            this.label_color_ch5.Text = "Ch 5 (" + logAnlysPins[4] + ")";
            this.label_color_ch6.Text = "Ch 6 (" + logAnlysPins[5] + ")";
            this.label_color_ch7.Text = "Ch 7 (" + logAnlysPins[6] + ")";
            this.label_color_ch8.Text = "Ch 8 (" + logAnlysPins[7] + ")";            
            //this.label_color_ch1.Text = device.logAnlysCfg.pins[0];
        }

        private void InitializeZedGraphComponent()
        {
            logAnlysPane = zedGraphControl_logAnlys.GraphPane;

            logAnlysPane.XAxis.Title.Text = "Time [s]";
            logAnlysPane.XAxis.Title.FontSpec.Size = this.Size.Width / 80;
            logAnlysPane.XAxis.Scale.FontSpec.Size = 10;

            zedGraphControl_logAnlys.MasterPane[0].IsFontsScaled = false;
            zedGraphControl_logAnlys.MasterPane[0].Title.IsVisible = false;

            zedGraphControl_logAnlys.MasterPane[0].XAxis.Title.IsVisible = true;
            zedGraphControl_logAnlys.MasterPane[0].YAxis.Title.IsVisible = false;

            zedGraphControl_logAnlys.MasterPane[0].XAxis.MajorGrid.IsVisible = true;
            zedGraphControl_logAnlys.MasterPane[0].YAxis.MajorGrid.IsVisible = false;

            zedGraphControl_logAnlys.MasterPane[0].XAxis.IsVisible = true;
            zedGraphControl_logAnlys.MasterPane[0].YAxis.IsVisible = false;
        }        

        private void Update_signal(object sender, ElapsedEventArgs e)
        {
            double sum = samplingFreq.GetHashCode() + dataLength;        

            if (sum != last_sum1)
            {
                //LogAnlys_Stop();
                last_sum1 = sum;
                calculateAndSend_AllParameters();                
                this.Invalidate();
                //LogAnlys_Start();
            }
            else if(pretrig != lastPretrig)
            {
                //LogAnlys_Stop();
                lastPretrig = pretrig;
                calculateAndSend_PretrigPosttrig();
                this.Invalidate();
                //LogAnlys_Start();
            }
        }

        public void LogAnlys_Init()
        {
            sendCommand(Commands.LOG_ANLYS_INIT);
        }

        public void LogAnlys_Deinit()
        {
            sendCommand(Commands.LOG_ANLYS_DEINIT);
        }

        public void LogAnlys_Start()
        {            
            sendCommand(Commands.LOG_ANLYS_START);
        }

        public  void LogAnlys_Stop()
        {
            sendCommand(Commands.LOG_ANLYS_STOP);
        }

        void logAnlys_next()
        {
            LogAnlys_Start();
        }

        private void LogicAnalyzer_FormClosing(object sender, FormClosingEventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.LOG_ANLYS + ":" + Commands.LOG_ANLYS_STOP + ";");
            device.send(Commands.LOG_ANLYS + ":" + Commands.LOG_ANLYS_DEINIT + ";");
            device.giveCommsSemaphore();
        }

        void calculateAndSend_AllParameters()
        {
            /* Send number of samples to be taken */
            sendCommandNumber(Commands.LOG_ANLYS_SAMPLES_NUM, dataLength);            
            /* Calculate and send sampling frequency */
            realSamplingFreq = processFrequency(samplingFreq, timeBasePeriphClock);
            sendCommandNumber(Commands.LOG_ANLYS_SAMPLING_FREQ, make32BitFromArrPsc((ushort)(syncPwmArr - 1), (ushort)(syncPwmPsc - 1)));            
            /* Set pretrigger and posttrigger */
            calculateAndSend_PretrigPosttrig();
        }

        void calculateAndSend_PretrigPosttrig()
        {            
            double samplingTime = dataLength / (double)samplingFreq;
            /* Calculate pretrigger in milliseconds */
            uint pretriggerTime = (uint)Math.Round(samplingTime * pretrig / (double)100 * 1000);
            double posttriggerFreq = 1 / (samplingTime * (1 - pretrig / (double)100));
            processFrequency(posttriggerFreq, posttrigPeriphClock);

            /* Send pretrigger */
            sendCommandNumber(Commands.LOG_ANLYS_PRETRIG, pretriggerTime);            
            /* Send posttrigger */
            sendCommandNumber(Commands.LOG_ANLYS_POSTTRIG, make32BitFromArrPsc((ushort)(syncPwmArr - 1), (ushort)(syncPwmPsc - 1)));            
        }

        public void sendCommand(string generalComm)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);            
            device.send(Commands.LOG_ANLYS + ":" + generalComm + ";");            
            device.giveCommsSemaphore();
        }

        public void sendCommand(string generalComm, string specificComm)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.LOG_ANLYS + ":" + generalComm + " ");
            device.send(specificComm + ";");
            device.giveCommsSemaphore();
        }

        public void sendCommandNumber(string generalComm, uint number)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.LOG_ANLYS + ":" + generalComm + " ");
            device.send_int((int)number);
            device.send(";");
            device.giveCommsSemaphore();
        }

        public static void setPins(string[] pins)
        {
            for (int i = 0; i < pins.Length; i++)
            {
                logAnlysPins[i] = pins[i].Replace("_", "");
            }            
        }
        
        public void getFirstGPIOPin()
        {
            string pinNumber = Regex.Match(logAnlysPins[0], @"\d+").Value;

            try
            {
                firstGpioPin = ushort.Parse(pinNumber);
            }
            catch (Exception ex)
            {
                device.logRecieved("Logic Analyzer - first GPIO pin not parsed.");
            }
        }

        public static void setPosttrigPeriphClock(uint periphClock)
        {
            posttrigPeriphClock = periphClock;
        }

        public static void setTimeBasePeriphClock(uint periphClock)
        {
            timeBasePeriphClock = periphClock;
        }

        private void Update_GUI(object sender, ElapsedEventArgs e)
        {
            if (logAnlys_q.Count > 0)
            {
                message = logAnlys_q.Dequeue();
                if (message == null)
                {
                    return;
                }
                switch (req = message.GetRequest())
                {
                    case Message.MsgRequest.LOG_ANLYS_TRIGGER_POINTER:
                        triggerPointer = message.GetNum();
                        break;
                    case Message.MsgRequest.LOG_ANLYS_DATA:
                        retrieveData();
                        this.Invalidate();
                        break;
                }
            }
        }

        public void add_message(Message msg)
        {
            this.logAnlys_q.Enqueue(msg);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                if (req == Message.MsgRequest.LOG_ANLYS_DATA)
                {
                    /* There might be a data coming from MCU after MCU was stopped. This is to prevent asking for another data pack.*/
                    if (checkBox_trig_single.Text != "Single")
                    {
                        label_logAnlys_status.ForeColor = (label_logAnlys_status.ForeColor == Color.Red) ? Color.Blue : Color.Red;
                        label_logAnlys_status.Text = "Trig";
                        paint_signals();

                        switch (triggerMode)
                        {
                            case TRIGGER_MODE.SINGLE:
                                checkBox_trig_single.Text = "Single";
                                break;
                            case TRIGGER_MODE.AUTO:
                                logAnlys_next();
                                break;
                            case TRIGGER_MODE.NORMAL:
                                logAnlys_next();
                                break;
                        }
                    }
                }

                base.OnPaint(e);
                req = Message.MsgRequest.NULL_MESSAGE;
            }
            catch (Exception ex)
            {
                this.Close();
                throw new System.ArgumentException("Logic Analyzer painting went wrong");
            }

        }

        public void paint_signals()
        {
            logAnlysPane.CurveList.Clear();

            paint_one_signal(timeAxis, signal_ch1, 1, Color.Red);
            paint_one_signal(timeAxis, signal_ch2, 2, Color.Blue);
            paint_one_signal(timeAxis, signal_ch3, 3, Color.Green);
            paint_one_signal(timeAxis, signal_ch4, 4, Color.Black);
            paint_one_signal(timeAxis, signal_ch5, 5, Color.Magenta);
            paint_one_signal(timeAxis, signal_ch6, 6, Color.DarkOrange);
            paint_one_signal(timeAxis, signal_ch7, 7, Color.Indigo);
            paint_one_signal(timeAxis, signal_ch8, 8, Color.Maroon);

            zedGraphControl_logAnlys.AxisChange();
            zedGraphControl_logAnlys.Invalidate();
        }

        public void paint_one_signal(double[] timeAxis, double[] valueAxis, uint channel, Color color)
        {            
            LineItem logAnlysCurve;

            //switch (channel)
            //{
            //    case 1: logAnlysCurve = curve_ch1; break;
            //    case 2: logAnlysCurve = curve_ch2; break;
            //    case 3: logAnlysCurve = curve_ch3; break;
            //    case 4: logAnlysCurve = curve_ch4; break;
            //    case 5: logAnlysCurve = curve_ch5; break;
            //    case 6: logAnlysCurve = curve_ch6; break;
            //    case 7: logAnlysCurve = curve_ch7; break;
            //    case 8: logAnlysCurve = curve_ch8; break;
            //    default: break;
            //}

            logAnlysCurve = logAnlysPane.AddCurve("", timeAxis, valueAxis, color, SymbolType.Square);
            logAnlysCurve.Line.StepType = StepType.ForwardStep;
            logAnlysCurve.Line.IsSmooth = false;
            logAnlysCurve.Line.Width = 1.5F;
            logAnlysCurve.Line.IsAntiAlias = false;
            logAnlysCurve.Line.IsOptimizedDraw = true;
            logAnlysCurve.Symbol.Size = 0;
            logAnlysPane.YAxis.Scale.Max = 18;
            //logAnlysPane.XAxis.Scale.Max = dataLength / samplingFreq;
            logAnlysPane.XAxis.MajorGrid.IsVisible = true;

            //logAnlysPane.XAxis.Scale.MaxAuto = true;
            //logAnlysPane.XAxis.Scale.MinAuto = true;
            //logAnlysPane.YAxis.Scale.MaxAuto = true;
            //logAnlysPane.YAxis.Scale.MinAuto = true;
            //logAnlysPane.XAxis.Scale.Min = 0;
            //logAnlysPane.YAxis.Scale.Max = 5;
            //logAnlysPane.YAxis.Scale.Min = 0.5f;            
        }

        void retrieveData()
        {            
            uint length = (uint)device.logAnlysCfg.samples.Length;

            timeAxis = timAxis(new double[length]);

            signal_ch1 = valueAxis(new double[length], 8);
            signal_ch2 = valueAxis(new double[length], 7);
            signal_ch3 = valueAxis(new double[length], 6);
            signal_ch4 = valueAxis(new double[length], 5);
            signal_ch5 = valueAxis(new double[length], 4);
            signal_ch6 = valueAxis(new double[length], 3);
            signal_ch7 = valueAxis(new double[length], 2);
            signal_ch8 = valueAxis(new double[length], 1);
        }

        public double[] valueAxis(double[] array, uint channel)
        {
            //ushort chan = (ushort)(channel - 1);
            ushort chan = (ushort)(8 - channel);            

            /* Required trigger point. Defined by pretrigger trackbar value. */
            int requiredTrigger = (int)(pretrig / (double)100 * array.Length);
            /* Actual trigger point - triggerPointer represents the value of CNDTR register in time of trigger event. */
            int actualTrigger = array.Length - triggerPointer;
            /* Calculate the number of array elements to be shifted. */
            int shiftNum = (actualTrigger > requiredTrigger) ? array.Length - actualTrigger + requiredTrigger : requiredTrigger - actualTrigger;
            /* Shift the array to align it for painting. */
            rotateArrayForward(array, shiftNum);            

            /* Extract zeroes and ones of required GPIO pin from received buffer. */
            for (int j = 0; j < array.Length; j++)
            {
                /* Set the n-th element to predefined level for graph painting. */
                array[j] = ((device.logAnlysCfg.samples[j] & (ushort)(1 << (firstGpioPin + chan))) == 0) ? ((channel - 1) * 2) + 1 : ((channel - 1) * 2) + 2;
            }

            return array;
        }

        public double[] timAxis(double[] array)
        {
            double samplingPeriod = 1 / realSamplingFreq;            

            array[0] = 0;            
            for (int j = 1; j < array.Length; j++)
            {
                array[j] = samplingPeriod;
                samplingPeriod += samplingPeriod;
            }

            return array;
        }

        public static void rotateArrayForward(double[] array, int numOfShifts)
        {
            int shift = array.Length - numOfShifts;
            arrayReverse(array, 0, shift - 1);
            arrayReverse(array, shift, array.Length - 1);
            arrayReverse(array, 0, array.Length - 1);
        }

        public static void arrayReverse(double[] array, int start, int end)
        {
            while(start < end)
            {
                double temp = array[start];
                array[start] = array[end];
                array[end] = temp;
                start++;
                end--;
            }
        }

        private double processFrequency(double freq, uint periphClock)
        {
            UInt32 psc = 0, arr = 1;  // MCU TIM Prescaler and Auto Reload Register
            UInt32 arrMultipliedByPsc = 0;
            arrMultipliedByPsc = (uint)Math.Round(periphClock / freq);

            if (arrMultipliedByPsc <= 65536)
            {
                arr = arrMultipliedByPsc;
                psc = 1;
            }
            else
            {
                /* Test how the inserted frequency can be devided to set ARR and PSC registers. */
                for (; psc == 0; arrMultipliedByPsc--)
                {
                    for (UInt32 pscTemp = 65536; pscTemp > 1; pscTemp--)
                    {
                        if ((arrMultipliedByPsc % pscTemp) == 0)
                        {
                            if (arrMultipliedByPsc / pscTemp <= 65536)
                            {
                                psc = pscTemp;
                                break;
                            }
                        }
                    }

                    if (psc != 0)
                    {
                        if (arrMultipliedByPsc / psc <= 65536)
                        {
                            break;
                        }
                    }
                }

                arr = arrMultipliedByPsc / psc;

                if (arr < psc)
                {
                    UInt32 swapVar = arr;
                    arr = psc;
                    psc = swapVar;
                }
            }

            syncPwmArr = (ushort)arr;
            syncPwmPsc = (ushort)psc;

            double realPwmFreq = periphClock / (double)(arr * psc);
            return realPwmFreq;
        }

        public uint make32BitFromArrPsc(ushort arr, ushort psc)
        {
            uint arrPsc = 0;
            arrPsc = (uint)(psc << 16) | arr;

            return arrPsc;
        }

        private static bool isPrimeNumber(UInt32 number)
        {
            if ((number & 1) == 0)
            {
                return (number == 2) ? true : false;
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

        /* ------------------------------------------------------------------------------------------------------------------ */
        /* ---------------------------------------- LOGIC ANALYZER EVENTS HANDLERS ------------------------------------------ */
        /* ------------------------------------------------------------------------------------------------------------------ */
        private void trackBar_pretrig_Scroll(object sender, EventArgs e)
        {
            pretrig = (uint)trackBar_pretrig.Value;
            this.maskedTextBox_pretrig.Text = pretrig.ToString();
        }

        private void maskedTextBox_pretrig_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_pretrigger();
            }
        }

        private void maskedTextBox_pretrig_Leave(object sender, EventArgs e)
        {
            validate_pretrigger();
        }

        private void validate_pretrigger()
        {
            /* In Auto trigger mode the trigger may not be received. In that case its value is
               given by user settings as the data is sent to PC app after a period. Moreover, triggerPointer
               value represents CNDTR register of DMA in the time of trigger, i.e. pointer is decreasing
               from initial value. */
            triggerPointer = (int)dataLength - (int)dataLength * (int)(trackBar_pretrig.Value / (double)100);

            try
            {
                ushort val = ushort.Parse(this.maskedTextBox_pretrig.Text);
                if (val > 100)
                {
                    throw new System.ArgumentException("Parameter cannot be greather then 100", "original");
                }
                this.trackBar_pretrig.Value = (int)(val);
                pretrig = val;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                this.maskedTextBox_pretrig.Text = pretrig.ToString();
            }
        }

        private void radioButton_trig_ch1_CheckedChanged(object sender, EventArgs e)
        {
            sendCommandNumber(Commands.LOG_ANLYS_TRIGGER_CHANNEL, 1);
        }

        private void radioButton_trig_ch2_CheckedChanged(object sender, EventArgs e)
        {
            sendCommandNumber(Commands.LOG_ANLYS_TRIGGER_CHANNEL, 2);
        }

        private void checkBox_trig_rise_CheckedChanged(object sender, EventArgs e)
        {
            sendCommand(Commands.LOG_ANLYS_TRIGGER_EVENT, Commands.LOG_ANLYS_TRIGGER_EDGE_RISING);
            checkBox_trig_rise.Checked = true;
            checkBox_trig_fall.Checked = false;
        }

        private void checkBox_trig_fall_CheckedChanged(object sender, EventArgs e)
        {
            sendCommand(Commands.LOG_ANLYS_TRIGGER_EVENT, Commands.LOG_ANLYS_TRIGGER_EDGE_FALLING);
            checkBox_trig_fall.Checked = true;
            checkBox_trig_rise.Checked = false;
        }

        private void checkBox_trig_single_CheckedChanged(object sender, EventArgs e)
        {            
            checkBox_trig_single.Checked = true;
            if (checkBox_trig_single.Text.Equals("Stop"))
            {
                LogAnlys_Stop();
                checkBox_trig_single.Text = "Single";
                label_logAnlys_status.Text = "";
            }
            else if (checkBox_trig_single.Text.Equals("Single"))
            {                
                triggerMode = TRIGGER_MODE.SINGLE;
                sendCommand(Commands.LOG_ANLYS_TRIGGER_MODE, Commands.LOG_ANLYS_TRIGGER_MODE_SINGLE);
                logAnlys_next();
                checkBox_trig_single.Text = "Stop";
                label_logAnlys_status.Text = "Wait";                
            }
            this.checkBox_trig_auto.Checked = false;
            this.checkBox_trig_normal.Checked = false;
        }

        private void checkBox_trig_normal_CheckedChanged(object sender, EventArgs e)
        {            
            if (this.checkBox_trig_normal.Checked)
            {                          
                triggerMode = TRIGGER_MODE.NORMAL;
                sendCommand(Commands.LOG_ANLYS_TRIGGER_MODE, Commands.LOG_ANLYS_TRIGGER_MODE_NORMAL);
                logAnlys_next();
                this.checkBox_trig_auto.Checked = false;
                this.checkBox_trig_single.Checked = false;
                this.checkBox_trig_single.Text = "Stop";
                label_logAnlys_status.Text = "";
            }
        }

        private void checkBox_trig_auto_CheckedChanged(object sender, EventArgs e)
        {            
            if (this.checkBox_trig_auto.Checked)
            {
                triggerMode = TRIGGER_MODE.AUTO;
                sendCommand(Commands.LOG_ANLYS_TRIGGER_MODE, Commands.LOG_ANLYS_TRIGGER_MODE_AUTO);
                logAnlys_next();                
                this.checkBox_trig_normal.Checked = false;
                this.checkBox_trig_single.Checked = false;
                this.checkBox_trig_single.Text = "Stop";
                label_logAnlys_status.Text = "";
            }
        }

        /* ---------------------------------------- DATA LENGTH EVENTS HANDLERS ------------------------------------------ */
        private void radioButton_100_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_100.Checked)
            {
                dataLength = 100;
            }
        }

        private void radioButton_200_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_200.Checked)
            {
                dataLength = 200;
            }
        }

        private void radioButton_500_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_500.Checked)
            {
                dataLength = 500;
            }
        }

        private void radioButton_1x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_1x.Checked)
            {
                dataLength = 1000;
            }
        }

        private void radioButton_2x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_2x.Checked)
            {
                dataLength = 2000;
            }
        }

        private void radioButton_5x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_5x.Checked)
            {
                dataLength = 5000;
            }
        }

        private void radioButton_10x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_10x.Checked)
            {
                dataLength = 10000;
            }
        }

        private void radioButton_20x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_20x.Checked)
            {
                dataLength = 20000;
            }
        }

        private void radioButton_50x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_50x.Checked)
            {
                dataLength = 50000;
            }
        }

        private void radioButton_100x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_100x.Checked)
            {
                dataLength = 100000;
            }
        }

        private void radioButton_200x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_200x.Checked)
            {
                dataLength = 200000;
            }
        }

        private void radioButton_500x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_500x.Checked)
            {
                dataLength = 500000;
            }
        }

        private void button_max_possible_Click(object sender, EventArgs e)
        {
            radioButton_500x.Checked = true;            
        }

        /* ------------------------------ TIME BASE (SAMPLING FREQUENCY) EVENTS HANDLERS -------------------------------- */
        private void radioButton_1k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_1k.Checked)
            {
                samplingFreq = 1000;
                this.label_freq.Text = "1 kS";
            }
        }

        private void radioButton_2k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_2k.Checked)
            {
                samplingFreq = 2000;
                this.label_freq.Text = "2 kS";
            }
        }

        private void radioButton_5k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_5k.Checked)
            {
                samplingFreq = 5000;
                this.label_freq.Text = "5 kS";
            }
        }

        private void radioButton_10k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_10k.Checked)
            {
                samplingFreq = 10000;
                this.label_freq.Text = "10 kS";
            }
        }

        private void radioButton_20k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_20k.Checked)
            {
                samplingFreq = 20000;
                this.label_freq.Text = "20 Ks";
            }
        }

        private void radioButton_50k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_50k.Checked)
            {
                samplingFreq = 50000;
                this.label_freq.Text = "50 kS";
            }
        }

        private void radioButton_100k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_100k.Checked)
            {
                samplingFreq = 100000;
                this.label_freq.Text = "100 Ks";
            }
        }

        private void radioButton_200k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_200k.Checked)
            {
                samplingFreq = 200000;
                this.label_freq.Text = "200 kS";
            }
        }

        private void radioButton_500k_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_500k.Checked)
            {
                samplingFreq = 500000;
                this.label_freq.Text = "500 kS";
            }
        }

        private void radioButton_1m_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_1m.Checked)
            {
                samplingFreq = 1000000;
                this.label_freq.Text = "1 MS";
            }
        }

        private void radioButton_2m_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_2m.Checked)
            {
                samplingFreq = 2000000;
                this.label_freq.Text = "2 MS";
            }
        }

        private void radioButton_5m_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_5m.Checked)
            {
                samplingFreq = 5000000;
                this.label_freq.Text = "5 MS";
            }
        }

        private void radioButton_10m_CheckedChanged(object sender, EventArgs e)
        {           
            if (radioButton_10m.Checked)
            {
                samplingFreq = 10000000;
                this.label_freq.Text = "10 MS";
            }
        }

        private void radioButton_20m_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_20m.Checked)
            {
                samplingFreq = 20000000;
                this.label_freq.Text = "20 MS";
            }
        }
    }
}
