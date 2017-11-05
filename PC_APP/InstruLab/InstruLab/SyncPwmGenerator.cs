using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Timers;
using System.Drawing;
using System.Windows.Forms;
using ZedGraph;

namespace LEO
{
    public partial class SyncPwmGenerator : Form
    {
        Device device;
        int semaphoreTimeout = 4000;
        double cntPaint;
        const int messageDelay = 5;

        System.Timers.Timer GUITimer;
        static Semaphore syncPwmSemaphore = new Semaphore(1, 1);

        private Queue<Message> cnt_q = new Queue<Message>();
        Message.MsgRequest req;
        Message messg;

        private double last_sum = 0;
        private bool generating = false;

        static int syncPwmTimPeriphClock;
        static string[] syncPwmPins = new string[4];
        double syncPwmFreq = 1;   // Hz               
        double syncPwmRealFreq = 1;   // Hz     
        uint syncPwmDuty = 25;      // %
        uint syncPwmPhase = 90;     // °
        ushort syncPwmPsc = 10000;
        ushort syncPwmArr = 14400;
        uint channelsActiveNum = 4;
        ulong stepNum = 0;

        /* Advanced */
        double phaseChan1 = 0;
        double phaseChan2 = 90;
        double phaseChan3 = 180;
        double phaseChan4 = 270;
        double dutyChan1 = 25;
        double dutyChan2 = 25;
        double dutyChan3 = 25;
        double dutyChan4 = 25;

        /* Basic */
        ushort[] toggleChan1 = new ushort[2];
        ushort[] toggleChan2 = new ushort[2];
        ushort[] toggleChan3 = new ushort[2];
        ushort[] toggleChan4 = new ushort[2];

        LineItem syncPwmCurve_ch1, syncPwmCurve_ch2, syncPwmCurve_ch3, syncPwmCurve_ch4;
        public GraphPane syncPwmPane;

        public double[] syncPwmTimeAxis_ch1, syncPwmTimeAxis_ch2, syncPwmTimeAxis_ch3, syncPwmTimeAxis_ch4;
        public double[] syncPwmSignal_ch1, syncPwmSignal_ch2, syncPwmSignal_ch3, syncPwmSignal_ch4;

        //System.Timers.Timer scrollTimer;
        //public enum SYNC_PWM_TIMER { SCROLL_BASIC_FREQ = 0, SCROLL_NONE };
        //SYNC_PWM_TIMER timScroll;

        public enum MODE { BASIC, ADVANCED };
        MODE mode;


        public SyncPwmGenerator(Device dev)
        {
            InitializeComponent();
            ShowPinsInComponent(); 
            InitializeZedGraphComponent();            

            device = dev;

            //scrollTimerConfig();

            GUITimer = new System.Timers.Timer(120);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_signal);
            GUITimer.Start();

            SyncPwmGenerator_Init();
            BasicModeComponent_Init();
        }

        private void InitializeZedGraphComponent()
        {
            syncPwmPane = zedGraphControl_syncPwm.GraphPane;

            syncPwmPane.XAxis.Title.Text = "Time [s]";
            syncPwmPane.XAxis.Title.FontSpec.Size = this.Size.Width / 80;
            syncPwmPane.XAxis.Scale.FontSpec.Size = 10;

            zedGraphControl_syncPwm.MasterPane[0].IsFontsScaled = false;
            zedGraphControl_syncPwm.MasterPane[0].Title.IsVisible = false;

            zedGraphControl_syncPwm.MasterPane[0].XAxis.Title.IsVisible = true;
            zedGraphControl_syncPwm.MasterPane[0].YAxis.Title.IsVisible = false;

            zedGraphControl_syncPwm.MasterPane[0].XAxis.MajorGrid.IsVisible = false;
            zedGraphControl_syncPwm.MasterPane[0].YAxis.MajorGrid.IsVisible = false;

            zedGraphControl_syncPwm.MasterPane[0].XAxis.IsVisible = true;
            zedGraphControl_syncPwm.MasterPane[0].YAxis.IsVisible = false;
        }

        public void ShowPinsInComponent()
        {
            this.groupBox_syncPwm_ch1.Text = "Channel 1 (" + syncPwmPins[0] + ")";
            this.groupBox_syncPwm_ch2.Text = "Channel 2 (" + syncPwmPins[1] + ")";
            this.groupBox_syncPwm_ch3.Text = "Channel 3 (" + syncPwmPins[2] + ")";
            this.groupBox_syncPwm_ch4.Text = "Channel 4 (" + syncPwmPins[3] + ")";
        }

        public void BasicModeComponent_Init()
        {
            this.ClientSize = new Size(670, 320);
            this.tableLayoutPanel_basic_pwmComponents.Visible = true;
            this.tableLayoutPanel_advanced.Visible = false;

            this.trackBar_basic_freq.Value = trackBar_advanced_generalFreq.Value;
            this.textBox_basic_freq.Text = textBox_advanced_generalFreq.Text;
            this.textBox_basic_realFreq.Text = textBox_advanced_generalRealFreq.Text;

            this.tableLayoutPanel_basic_pwmComponents.Enabled = (this.button_syncPwm.Text == "Stop") ? false : true;

            generate_signals_mcu_basic();
            mode = MODE.BASIC;
        }

        public void AdvancedModeComponent_Init()
        {
            this.ClientSize = new Size(850, 390);
            this.tableLayoutPanel_basic_pwmComponents.Visible = false;
            this.tableLayoutPanel_advanced.Visible = true;

            this.trackBar_advanced_generalFreq.Value = trackBar_basic_freq.Value;
            this.textBox_advanced_generalFreq.Text = textBox_basic_freq.Text;
            this.textBox_advanced_generalRealFreq.Text = textBox_basic_realFreq.Text;

            this.tableLayoutPanel_advanced.Enabled = (this.button_syncPwm.Text == "Stop") ? false : true;

            generate_signals_mcu_advanced();
            mode = MODE.ADVANCED;
        }

        void SyncPwmGenerator_Start()
        {
            sendCommand(Commands.SYNC_PWM_COMMAND, Commands.SYNC_PWM_START);
        }

        void SyncPwmGenerator_Stop()
        {
            sendCommand(Commands.SYNC_PWM_COMMAND, Commands.SYNC_PWM_STOP);            
        }

        void SyncPwmGenerator_Init()
        {
            sendCommand(Commands.SYNC_PWM_COMMAND, Commands.SYNC_PWM_INIT);
        }

        private void Update_signal(object sender, ElapsedEventArgs e)
        {
            double sum;

            if (mode == MODE.BASIC)
            {
                sum = syncPwmFreq.GetHashCode() + channelsActiveNum + syncPwmDuty + syncPwmPhase;
            }
            else
            {
                sum = syncPwmFreq.GetHashCode() + channelsActiveNum + dutyChan1 + dutyChan2 + dutyChan3 + dutyChan4 + 
                                                                      phaseChan1 + phaseChan2 + phaseChan3 + phaseChan4;
            }

            if (sum != last_sum)
            {
                last_sum = sum;
                generate_signals();
                paint_signals();
                this.Invalidate();
            }
        }

        public void generate_signals()
        {
            if (mode == MODE.BASIC)
            {
                generate_signals_gui_basic();
                generate_signals_mcu_basic();
            }
            else
            {
                generate_signals_gui_advanced();
                generate_signals_mcu_advanced();
            }
        }

        public void generate_signals_mcu_advanced()
        {
            sendCommandNumber(Commands.SYNC_PWM_FREQ, make32BitFromArrPsc((ushort)(syncPwmArr - 1), (ushort)(syncPwmPsc - 1)));
            Thread.Sleep(messageDelay);

            uint temp;
            /* syncPwmTimPeriphClock / 4 = DMA2 max freq.    ->    4 channels    -> syncPwmTimPeriphClock / (4 * 4) = 144 MHz / 16 = 9MHz */
            ushort firstEdge = (syncPwmTimPeriphClock / syncPwmPsc > 9000000) ? (ushort)(16 / syncPwmPsc) : (ushort)2;
            ushort secondEdge = (syncPwmTimPeriphClock / syncPwmPsc > 9000000) ? (ushort)(8 / syncPwmPsc) : (ushort)1;


            /* Channel 1 */
            temp = (uint)Math.Round(phaseChan1 / (double)360 * syncPwmArr);
            toggleChan1[0] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - firstEdge) : (ushort)temp;
            temp = (uint)Math.Round(dutyChan1 / (double)100 * syncPwmArr) + toggleChan1[0];
            toggleChan1[1] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - secondEdge) : (ushort)temp;

            if (toggleChan1[0] >= syncPwmArr - firstEdge & toggleChan1[1] >= syncPwmArr - secondEdge)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL1 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
            }
            else if (checkBox_syncPwm_ch1.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL1 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_NUM, 1);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_CONFIG, (uint)(toggleChan1[0] << 16) | toggleChan1[1]);
                Thread.Sleep(messageDelay);
            }


            /* Channel 2 */
            temp = (uint)Math.Round(phaseChan2 / (double)360 * syncPwmArr);
            toggleChan2[0] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - firstEdge) : (ushort)temp;
            temp = (uint)(Math.Round(dutyChan2 / (double)100 * syncPwmArr) + toggleChan2[0]);
            toggleChan2[1] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - secondEdge) : (ushort)temp;

            if (toggleChan2[0] >= syncPwmArr - firstEdge & toggleChan2[1] >= syncPwmArr - secondEdge)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL2 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
            }
            else if (checkBox_syncPwm_ch2.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL2 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_NUM, 2);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_CONFIG, (uint)(toggleChan2[0] << 16) | toggleChan2[1]);
                Thread.Sleep(messageDelay);
            }

            /* Channel 3 */
            temp = (uint)Math.Round(phaseChan3 / (double)360 * syncPwmArr);
            toggleChan3[0] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - firstEdge) : (ushort)temp;
            temp = (uint)(Math.Round(dutyChan3 / (double)100 * syncPwmArr) + toggleChan3[0]);
            toggleChan3[1] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - secondEdge) : (ushort)temp;

            if (toggleChan3[0] >= syncPwmArr - firstEdge & toggleChan3[1] >= syncPwmArr - secondEdge)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL3 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
            }
            else if (checkBox_syncPwm_ch3.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL3 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_NUM, 3);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_CONFIG, (uint)(toggleChan3[0] << 16) | toggleChan3[1]);
                Thread.Sleep(messageDelay);
            }


            /* Channel 4 */
            temp = (uint)Math.Round(phaseChan4 / (double)360 * syncPwmArr);
            toggleChan4[0] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - firstEdge) : (ushort)temp;
            temp = (uint)(Math.Round(dutyChan4 / (double)100 * syncPwmArr) + toggleChan4[0]);
            toggleChan4[1] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - secondEdge) : (ushort)temp;

            if (toggleChan4[0] >= syncPwmArr - firstEdge & toggleChan4[1] >= syncPwmArr - secondEdge)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL4 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
            }
            else if (checkBox_syncPwm_ch4.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL4 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_NUM, 4);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_CONFIG, (uint)(toggleChan4[0] << 16) | toggleChan4[1]);
                Thread.Sleep(messageDelay);
            }
        }

        public void generate_signals_mcu_basic()
        {
            sendCommandNumber(Commands.SYNC_PWM_FREQ, make32BitFromArrPsc((ushort)(syncPwmArr - 1), (ushort)(syncPwmPsc - 1)));
            Thread.Sleep(messageDelay);

            uint temp;
            /* syncPwmTimPeriphClock / 4 = DMA2 max freq.    ->    4 channels    -> syncPwmTimPeriphClock / (4 * 4) = 144 MHz / 16 = 9MHz */
            ushort firstEdge = (syncPwmTimPeriphClock / syncPwmPsc > 9000000) ? (ushort)(16 / syncPwmPsc) : (ushort)2;
            ushort secondEdge = (syncPwmTimPeriphClock / syncPwmPsc > 9000000) ? (ushort)(8 / syncPwmPsc) : (ushort)1;

            toggleChan1[0] = 0;
            temp = (uint)Math.Round(syncPwmDuty / (double)100 * syncPwmArr);
            toggleChan1[1] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - secondEdge) : (ushort)temp;

            if (toggleChan1[0] >= syncPwmArr - firstEdge & toggleChan1[1] >= syncPwmArr - secondEdge)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL1 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
            }
            else if (checkBox_syncPwm_ch1.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL1 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_NUM, 1);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_CONFIG, (uint)(toggleChan1[0] << 16) | toggleChan1[1]);
                Thread.Sleep(messageDelay);
            }


            /* Channel 2 */
            temp = (uint)Math.Round(syncPwmPhase / (double)360 * syncPwmArr);
            toggleChan2[0] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - firstEdge) : (ushort)temp;
            temp = (uint)(Math.Round(syncPwmDuty / (double)100 * syncPwmArr) + toggleChan2[0]);
            toggleChan2[1] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - secondEdge) : (ushort)temp;

            if (toggleChan2[0] >= syncPwmArr - firstEdge & toggleChan2[1] >= syncPwmArr - secondEdge)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL2 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
            }
            else if (checkBox_syncPwm_ch2.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL2 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_NUM, 2);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_CONFIG, (uint)(toggleChan2[0] << 16) | toggleChan2[1]);
                Thread.Sleep(messageDelay);
            }


            /* Channel 3 */
            temp = (uint)Math.Round(2 * syncPwmPhase / (double)360 * syncPwmArr);
            toggleChan3[0] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - firstEdge) : (ushort)temp;
            temp = (uint)(Math.Round(syncPwmDuty / (double)100 * syncPwmArr) + toggleChan3[0]);
            toggleChan3[1] = (temp >= syncPwmArr) ? (ushort)(syncPwmArr - secondEdge) : (ushort)temp;

            if (toggleChan3[0] >= syncPwmArr - firstEdge & toggleChan3[1] >= syncPwmArr - secondEdge)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL3 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
            }
            else if (checkBox_syncPwm_ch3.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL3 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_NUM, 3);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_CONFIG, (uint)(toggleChan3[0] << 16) | toggleChan3[1]);
                Thread.Sleep(messageDelay);
            }


            /* Channel 4 */
            temp = (uint)Math.Round(3 * syncPwmPhase / (double)360 * syncPwmArr);
            toggleChan4[0] = (temp >= syncPwmArr - firstEdge) ? (ushort)(syncPwmArr - firstEdge) : (ushort)temp;
            temp = (uint)(Math.Round(syncPwmDuty / (double)100 * syncPwmArr) + toggleChan4[0]);
            toggleChan4[1] = (temp >= syncPwmArr - secondEdge) ? (ushort)(syncPwmArr - secondEdge) : (ushort)temp;

            if (toggleChan4[0] >= syncPwmArr - firstEdge & toggleChan4[1] >= syncPwmArr - secondEdge)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL4 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
            }
            else if (checkBox_syncPwm_ch4.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL4 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
                Thread.Sleep(messageDelay);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_NUM, 4);
                sendCommandNumber(Commands.SYNC_PWM_CHAN_CONFIG, (uint)(toggleChan4[0] << 16) | toggleChan4[1]);
                Thread.Sleep(messageDelay);
            }
        }

        public void channelsActive()
        {
            channelsActiveNum = 0;
            channelsActiveNum = (checkBox_syncPwm_ch1.Checked) ? channelsActiveNum + 1 : channelsActiveNum;
            channelsActiveNum = (checkBox_syncPwm_ch2.Checked) ? channelsActiveNum + 1 : channelsActiveNum;
            channelsActiveNum = (checkBox_syncPwm_ch3.Checked) ? channelsActiveNum + 1 : channelsActiveNum;
            channelsActiveNum = (checkBox_syncPwm_ch4.Checked) ? channelsActiveNum + 1 : channelsActiveNum;
        }

        public void generate_signals_gui_advanced()
        {
            syncPwmTimeAxis_ch1 = timAxis_advanced(new double[4], 1);
            syncPwmTimeAxis_ch2 = timAxis_advanced(new double[4], 2);
            syncPwmTimeAxis_ch3 = timAxis_advanced(new double[4], 3);
            syncPwmTimeAxis_ch4 = timAxis_advanced(new double[4], 4);

            syncPwmSignal_ch1 = valueAxis(new double[4], 1);
            syncPwmSignal_ch2 = valueAxis(new double[4], 2);
            syncPwmSignal_ch3 = valueAxis(new double[4], 3);
            syncPwmSignal_ch4 = valueAxis(new double[4], 4);
        }

        public void generate_signals_gui_basic()
        {
            syncPwmTimeAxis_ch1 = timAxis_basic(new double[4], 1);
            syncPwmTimeAxis_ch2 = timAxis_basic(new double[4], 2);
            syncPwmTimeAxis_ch3 = timAxis_basic(new double[4], 3);
            syncPwmTimeAxis_ch4 = timAxis_basic(new double[4], 4);

            syncPwmSignal_ch1 = valueAxis(new double[4], 1);
            syncPwmSignal_ch2 = valueAxis(new double[4], 2);
            syncPwmSignal_ch3 = valueAxis(new double[4], 3);
            syncPwmSignal_ch4 = valueAxis(new double[4], 4);
        }

        public double[] timAxis_advanced(double[] array, uint channel)
        {
            double timePeriod = 1 / syncPwmRealFreq;
            array[0] = 0;
            array[3] = timePeriod;

            switch (channel)
            {
                case 1:
                    array[1] = Math.Round(syncPwmArr * (phaseChan1 / 360)) * timePeriod / syncPwmArr;
                    array[2] = array[1] + (Math.Round(syncPwmArr * ((double)dutyChan1 / 100)) * timePeriod / syncPwmArr);
                    break;
                case 2:
                    array[1] = Math.Round(syncPwmArr * (phaseChan2 / 360)) * timePeriod / syncPwmArr;
                    array[2] = array[1] + (Math.Round(syncPwmArr * ((double)dutyChan2 / 100)) * timePeriod / syncPwmArr);
                    break;
                case 3:
                    array[1] = Math.Round(syncPwmArr * (phaseChan3 / 360)) * timePeriod / syncPwmArr;
                    array[2] = array[1] + (Math.Round(syncPwmArr * ((double)dutyChan3 / 100)) * timePeriod / syncPwmArr);
                    break;
                case 4:
                    array[1] = Math.Round(syncPwmArr * (phaseChan4 / 360)) * timePeriod / syncPwmArr;
                    array[2] = array[1] + (Math.Round(syncPwmArr * ((double)dutyChan4 / 100)) * timePeriod / syncPwmArr);
                    break;
                default:
                    break;
            }

            return array;
        }

        public double[] timAxis_basic(double[] array, uint channel)
        {
            double timePeriod = 1 / syncPwmRealFreq;
            array[0] = 0;
            array[1] = (channel != 1) ? Math.Round(syncPwmArr * ((double)(channel - 1) * syncPwmPhase / 360)) * timePeriod / syncPwmArr : 0;
            array[2] = array[1] + (Math.Round(syncPwmArr * ((double)syncPwmDuty / 100)) * timePeriod / syncPwmArr);
            array[3] = timePeriod;

            return array;
        }

        public double[] valueAxis(double[] array, uint channel)
        {
            double chan = 5 - channel;
            array[0] = chan;
            array[1] = chan + 0.5;
            array[2] = chan;
            array[3] = chan;

            return array;
        }

        public void paint_signals()
        {
            syncPwmPane.CurveList.Clear();

            if (checkBox_syncPwm_ch1.Checked)
            {
                paint_one_signal(syncPwmTimeAxis_ch1, syncPwmSignal_ch1, 1, Color.Blue);
            }

            if (checkBox_syncPwm_ch2.Checked)
            {
                paint_one_signal(syncPwmTimeAxis_ch2, syncPwmSignal_ch2, 2, Color.Green);
            }

            if (checkBox_syncPwm_ch3.Checked)
            {
                paint_one_signal(syncPwmTimeAxis_ch3, syncPwmSignal_ch3, 3, Color.Red);
            }

            if (checkBox_syncPwm_ch4.Checked)
            {
                paint_one_signal(syncPwmTimeAxis_ch4, syncPwmSignal_ch4, 4, Color.DarkSlateGray);
            }

            zedGraphControl_syncPwm.AxisChange();
            zedGraphControl_syncPwm.Invalidate();
        }

        public void paint_one_signal(double[] timeAxis, double[] valueAxis, uint channel, Color color)
        {
            LineItem syncPwmCurve;

            switch (channel)
            {
                case 1:
                    syncPwmCurve = syncPwmCurve_ch1;
                    break;
                case 2:
                    syncPwmCurve = syncPwmCurve_ch2;
                    break;
                case 3:
                    syncPwmCurve = syncPwmCurve_ch3;
                    break;
                case 4:
                    syncPwmCurve = syncPwmCurve_ch4;
                    break;
                default:
                    break;
            }

            syncPwmCurve = syncPwmPane.AddCurve("", timeAxis, valueAxis, color, SymbolType.Square);
            syncPwmCurve.Line.StepType = StepType.ForwardStep;
            syncPwmCurve.Line.IsSmooth = false;
            syncPwmCurve.Line.Width = 1.5F;
            //syncPwmCurve.Line.SmoothTension = 0.5F;
            syncPwmCurve.Line.IsAntiAlias = false;
            syncPwmCurve.Line.IsOptimizedDraw = true;
            syncPwmCurve.Symbol.Size = 0;

            syncPwmPane.XAxis.Scale.MaxAuto = false;
            syncPwmPane.XAxis.Scale.MinAuto = false;
            syncPwmPane.YAxis.Scale.MaxAuto = false;
            syncPwmPane.YAxis.Scale.MinAuto = false;

            syncPwmPane.XAxis.Scale.Max = 1 / syncPwmRealFreq;
            syncPwmPane.XAxis.Scale.Min = 0;
            syncPwmPane.YAxis.Scale.Max = 5;
            syncPwmPane.YAxis.Scale.Min = 0.5f;
            syncPwmPane.XAxis.MajorGrid.IsVisible = true;
        }

        private double processPwmFrequency(double freq)
        {
            UInt32 psc = 0, arr = 1;  // MCU TIM Prescaler and Auto Reload Register
            UInt32 arrMultipliedByPsc = 0;
            arrMultipliedByPsc = (uint)Math.Round(syncPwmTimPeriphClock / freq);

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
                            if (pscTemp <= 65536 && (arrMultipliedByPsc / pscTemp <= 65536))
                            {
                                psc = pscTemp;
                                break;
                            }
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

            double realPwmFreq = syncPwmTimPeriphClock / (double)(arr * psc);
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

        public void sendCommand(string generalComm, string specificComm)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.SYNC_PWM_GEN + ":" + generalComm + " ");
            device.send(specificComm + ";");
            device.giveCommsSemaphore();
        }

        public void sendCommandNumber(string generalComm, uint number)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.SYNC_PWM_GEN + ":" + generalComm + " ");
            device.send_int((int)number);
            device.send(";");
            device.giveCommsSemaphore();
        }


        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ------------------------------------------------------- EVENT HANDLERS --------------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */
        private void button_syncPwm_Click(object sender, EventArgs e)
        {
            if (radioButton_syncPwm_continuous_mode.Checked)
            {
                if (button_syncPwm.Text.Equals("Start"))
                {
                    SyncPwmGenerator_Start();
                    button_syncPwm.Text = "Stop";
                    label_status_syncPwmIndicator.BackColor = Color.Green;
                    label_status_syncPwmText.Text = "Generating";

                    tableLayoutPanel_basic_pwmComponents.Enabled = false;
                    tableLayoutPanel_advanced.Enabled = false;
                    tableLayoutPanel_channelsActive.Enabled = false;
                    groupBox_syncPwm_mode.Enabled = false;
                }
                else
                {
                    SyncPwmGenerator_Stop();
                    button_syncPwm.Text = "Start";
                    label_status_syncPwmIndicator.BackColor = Color.Red;
                    label_status_syncPwmText.Text = "Idle";

                    tableLayoutPanel_basic_pwmComponents.Enabled = true;
                    tableLayoutPanel_advanced.Enabled = true;
                    tableLayoutPanel_channelsActive.Enabled = true;
                    groupBox_syncPwm_mode.Enabled = true;
                }
            }
            else if (radioButton_syncPwm_step_mode.Checked)
            {
                SyncPwmGenerator_Start();
                button_syncPwm.Enabled = false;
                stepNum += 1;    
                                         
                if(syncPwmRealFreq <= 500)
                {
                    Thread.Sleep((int)Math.Round(1 / syncPwmRealFreq * 1000));
                }
                else
                {
                    Thread.Sleep(2);
                }

                SyncPwmGenerator_Stop();
                label_status_syncPwmIndicator.BackColor = Color.LightGreen;
                label_status_syncPwmText.Text = "Step #" + stepNum;
                button_syncPwm.Enabled = true;
            }
        }

        private void radioButton_syncPwm_continuous_mode_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_syncPwm_continuous_mode.Checked)
            {
                SyncPwmGenerator_Stop();
                sendCommand(Commands.SYNC_PWM_STEP_MODE, Commands.SYNC_PWM_STEP_DISABLE);
                button_syncPwm.Text = "Start";
                label_status_syncPwmIndicator.BackColor = Color.Red;
                label_status_syncPwmText.Text = "Idle";
            }
        }

        private void radioButton_syncPwm_step_mode_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_syncPwm_step_mode.Checked)
            {            
                stepNum = 0;
                SyncPwmGenerator_Stop();
                sendCommand(Commands.SYNC_PWM_STEP_MODE, Commands.SYNC_PWM_STEP_ENABLE);
                button_syncPwm.Text = "Step";
            }
        }

        private void basicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.basicToolStripMenuItem.Checked = true;
            this.advancedToolStripMenuItem.Checked = false;
            BasicModeComponent_Init();
        }

        private void advancedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.basicToolStripMenuItem.Checked = false;
            this.advancedToolStripMenuItem.Checked = true;
            AdvancedModeComponent_Init();
        }

        private void trackBar_basic_freq_Scroll(object sender, EventArgs e)
        {
            syncPwmFreq = getExponencialTrackBarValue((double)(this.trackBar_basic_freq.Value));
            this.textBox_basic_freq.Text = syncPwmFreq.ToString("F2");

            syncPwmRealFreq = processPwmFrequency(syncPwmFreq);
            this.textBox_basic_realFreq.Text = syncPwmRealFreq.ToString("F2");

            //scrollTimer.Stop();
            //scrollTimer.Start();

            //timScroll = SYNC_PWM_TIMER.SCROLL_BASIC_FREQ;
        }

        private void trackBar_basic_duty_Scroll(object sender, EventArgs e)
        {
            syncPwmDuty = (ushort)trackBar_basic_duty.Value;
            this.textBox_basic_duty.Text = syncPwmDuty.ToString();
        }

        private void trackBar_basic_phase_Scroll(object sender, EventArgs e)
        {
            syncPwmPhase = (ushort)trackBar_basic_phase.Value;
            this.textBox_basic_phase.Text = syncPwmPhase.ToString();
        }

        private void textBox_basic_freq_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_basic(textBox);
            }
        }

        private void textBox_basic_duty_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_basic(textBox);
            }
        }

        private void textBox_basic_phase_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_basic(textBox);
            }
        }

        private void validate_text_syncPwm_basic(TextBox textBox)
        {
            try
            {
                Double val = Double.Parse(textBox.Text);
                if (val > 100000)
                {
                    throw new System.ArgumentException("Parameter cannot be greather then ", "original");
                }

                if (textBox.Equals(textBox_basic_freq))
                {
                    syncPwmRealFreq = processPwmFrequency(val);
                    syncPwmFreq = val;
                    textBox.Text = syncPwmFreq.ToString("F2");
                    this.textBox_basic_realFreq.Text = syncPwmRealFreq.ToString("F2");
                    trackBar_basic_freq.Value = (int)setExponencialTrackBarValue(syncPwmFreq);
                }
                else if (textBox.Equals(textBox_basic_duty))
                {
                    /* needs to be assigned to syncPwmDuty since zedGraph update is based on the parameter change */
                    trackBar_basic_duty.Value = (int)val;
                    syncPwmDuty = (ushort)val;
                }
                else if (textBox.Equals(textBox_basic_phase))
                {
                    trackBar_basic_phase.Value = (int)val;
                    syncPwmPhase = (ushort)val;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        private void checkBox_syncPwm_ch1_CheckedChanged(object sender, EventArgs e)
        {
            channelsActive();

            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL1 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
            }
            else
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL1 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
            }
        }

        private void checkBox_syncPwm_ch2_CheckedChanged(object sender, EventArgs e)
        {
            channelsActive();

            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL2 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
            }
            else
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL2 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
            }
        }

        private void checkBox_syncPwm_ch3_CheckedChanged(object sender, EventArgs e)
        {
            channelsActive();

            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL3 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
            }
            else
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL3 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
            }
        }

        private void checkBox_syncPwm_ch4_CheckedChanged(object sender, EventArgs e)
        {
            channelsActive();

            CheckBox checkBox = (CheckBox)sender;

            if (checkBox.Checked)
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL4 << 8) | Commands.SYNC_PWM_CHANNEL_ENABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
            }
            else
            {
                ushort channelState = (Commands.SYNC_PWM_CHANNEL4 << 8) | Commands.SYNC_PWM_CHANNEL_DISABLE;
                sendCommandNumber(Commands.SYNC_PWM_CHANNEL_STATE, channelState);
            }
        }

        public bool takeSyncPwmSemaphore(int ms)
        {
            bool result = false;
            result = syncPwmSemaphore.WaitOne(ms);
            if (!result)
            {
                throw new Exception("Unable to take semaphore");
            }
            return result;
        }

        public void giveSyncPwmSemaphore()
        {
            syncPwmSemaphore.Release();
        }

        public double getExponencialTrackBarValue(double trackVal)
        {
            return Math.Pow(10.0, trackVal / 10000);
        }

        public double setExponencialTrackBarValue(double trackVal)
        {
            return Math.Log10(trackVal) * 10000;
        }

        /* -------------------------------------------------------------------------------------------------------------------------------- */
        /* ----------------------------------------------- ADVANCED MODE EVENT HANDLERS --------------------------------------------------- */
        /* -------------------------------------------------------------------------------------------------------------------------------- */

        private void trackBar_advanced_generalFreq_Scroll(object sender, EventArgs e)
        {
            syncPwmFreq = getExponencialTrackBarValue((double)(this.trackBar_advanced_generalFreq.Value));
            this.textBox_advanced_generalFreq.Text = syncPwmFreq.ToString("F2");

            syncPwmRealFreq = processPwmFrequency(syncPwmFreq);
            this.textBox_advanced_generalRealFreq.Text = syncPwmRealFreq.ToString("F2");
        }

        private void trackBar_advanced_dutyCycle_channel1_Scroll(object sender, EventArgs e)
        {
            dutyChan1 = (ushort)trackBar_advanced_dutyCycle_channel1.Value;
            this.textBox_advanced_dutyCycle_channel1.Text = dutyChan1.ToString();
        }

        private void trackBar_advanced_phase_channel1_Scroll(object sender, EventArgs e)
        {
            phaseChan1 = (ushort)trackBar_advanced_phase_channel1.Value;
            this.textBox_advanced_phase_channel1.Text = phaseChan1.ToString();
        }

        private void trackBar_advanced_dutyCycle_channel2_Scroll(object sender, EventArgs e)
        {
            dutyChan2 = (ushort)trackBar_advanced_dutyCycle_channel2.Value;
            this.textBox_advanced_dutyCycle_channel2.Text = dutyChan2.ToString();
        }

        private void trackBar_advanced_phase_channel2_Scroll(object sender, EventArgs e)
        {
            phaseChan2 = (ushort)trackBar_advanced_phase_channel2.Value;
            this.textBox_advanced_phase_channel2.Text = phaseChan2.ToString();
        }

        private void trackBar_advanced_dutyCycle_channel3_Scroll(object sender, EventArgs e)
        {
            dutyChan3 = (ushort)trackBar_advanced_dutyCycle_channel3.Value;
            this.textBox_advanced_dutyCycle_channel3.Text = dutyChan3.ToString();
        }

        private void trackBar_advanced_phase_channel3_Scroll(object sender, EventArgs e)
        {
            phaseChan3 = (ushort)trackBar_advanced_phase_channel3.Value;
            this.textBox_advanced_phase_channel3.Text = phaseChan3.ToString();
        }

        private void trackBar_advanced_dutyCycle_channel4_Scroll(object sender, EventArgs e)
        {
            dutyChan4 = (ushort)trackBar_advanced_dutyCycle_channel4.Value;
            this.textBox_advanced_dutyCycle_channel4.Text = dutyChan4.ToString();
        }

        private void trackBar_advanced_phase_channel4_Scroll(object sender, EventArgs e)
        {
            phaseChan4 = (ushort)trackBar_advanced_phase_channel4.Value;
            this.textBox_advanced_phase_channel4.Text = phaseChan4.ToString();
        }

        private void textBox_advanced_generalFreq_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox_advanced_dutyCycle_channel1_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }

        private void textBox_advanced_phase_channel1_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }

        private void textBox_advanced_dutyCycle_channel2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }

        private void textBox_advanced_phase_channel2_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }

        private void textBox_advanced_dutyCycle_channel3_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }

        private void textBox_advanced_phase_channel3_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }

        private void textBox_advanced_dutyCycle_channel4_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }

        private void textBox_advanced_phase_channel4_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;

            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_text_syncPwm_advanced(textBox);
            }
        }


        private void validate_text_syncPwm_advanced(TextBox textBox)
        {
            try
            {
                Double val = Double.Parse(textBox.Text);
                if (val > 100000)
                {
                    throw new System.ArgumentException("Parameter cannot be greather then ", "original");
                }

                if (textBox.Equals(textBox_advanced_generalFreq))
                {
                    syncPwmRealFreq = processPwmFrequency(val);
                    syncPwmFreq = val;
                    textBox.Text = syncPwmFreq.ToString("F2");
                    this.textBox_advanced_generalRealFreq.Text = syncPwmRealFreq.ToString("F2");
                    trackBar_advanced_generalFreq.Value = (int)setExponencialTrackBarValue(syncPwmFreq);
                }
                else if (textBox.Equals(textBox_advanced_dutyCycle_channel1))
                {
                    trackBar_advanced_dutyCycle_channel1.Value = (int)val;
                    dutyChan1 = (ushort)val;
                }
                else if (textBox.Equals(textBox_advanced_dutyCycle_channel2))
                {
                    trackBar_advanced_dutyCycle_channel2.Value = (int)val;
                    dutyChan2 = (ushort)val;
                }
                else if (textBox.Equals(textBox_advanced_dutyCycle_channel3))
                {
                    trackBar_advanced_dutyCycle_channel3.Value = (int)val;
                    dutyChan3 = (ushort)val;
                }
                else if (textBox.Equals(textBox_advanced_dutyCycle_channel4))
                {
                    trackBar_advanced_dutyCycle_channel4.Value = (int)val;
                    dutyChan4 = (ushort)val;
                }
                else if (textBox.Equals(textBox_advanced_phase_channel1))
                {
                    trackBar_advanced_phase_channel1.Value = (int)val;
                    phaseChan1 = (ushort)val;
                }
                else if (textBox.Equals(textBox_advanced_phase_channel2))
                {
                    trackBar_advanced_phase_channel2.Value = (int)val;
                    phaseChan2 = (ushort)val;
                }
                else if (textBox.Equals(textBox_advanced_phase_channel3))
                {
                    trackBar_advanced_phase_channel3.Value = (int)val;
                    phaseChan3 = (ushort)val;
                }
                else if (textBox.Equals(textBox_advanced_phase_channel4))
                {
                    trackBar_advanced_phase_channel4.Value = (int)val;
                    phaseChan4 = (ushort)val;
                }
            }
            catch (Exception ex)
            {
                ex.Data.Clear();
            }
        }

        public static void setPins(string[] pins)
        {
            syncPwmPins[0] = pins[0].Replace("_", "");
            syncPwmPins[1] = pins[1].Replace("_", "");
            syncPwmPins[2] = pins[2].Replace("_", "");
            syncPwmPins[3] = pins[3].Replace("_", "");
        }

        public static void setPeriphClock(int periphClock)
        {
            syncPwmTimPeriphClock = periphClock;
        }
    }
}

