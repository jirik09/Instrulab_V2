using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Timers;
using ZedGraph;
using System.IO;

namespace InstruLab
{

    public partial class Scope : Form
    {
        //Thread scope_th;
        //Scope_thread scope;
        Device device;
        InstruLab.Device.ScopeConfig_def ScopeDevice;
        System.Timers.Timer GUITimer;
        System.Timers.Timer ZedTimer;

        int semaphoreTimeout = 4000;

        private Queue<Message> scope_q = new Queue<Message>();
        Message messg;
        Measurements meas = new Measurements(5);

        Thread processSignal_th;
        Thread calcSignal_th;
        
        public enum triggerEdge_def { RISE, FALL };
        public enum mode_def {IDLE, NORMAL, AUTO, SINGLE };

        public enum math_def {NONE, SUM,DIFF_A,DIFF_B,MULT };

        math_def math = math_def.NONE;
        math_def last_math = math_def.NONE;

        public triggerEdge_def triggeredge = triggerEdge_def.RISE;
        public double triggerLevel;
        public double last_trigger_level;
        public double pretrigger;
        public double last_pretrigger;
        public int adcRes;
        public int numSamples;
        public int actualCahnnels;
        private int triggerChannel;
        private int measChann = 1;

        private double[] signal_ch1;
        private double[] signal_ch2;
        private double[] signal_ch3;
        private double[] signal_ch4;
        private double[] signal_math;

        private double[] gain = new double[5] { 1, 1, 1, 1, 1 };
        public int[] offset = new int[5] { 0, 0, 0, 0, 0 };

        private double[] last_gain = new double[5] { 1, 1, 1, 1, 1 };
        public int[] last_offset = new int[5] { 0, 0, 0, 0, 0 };

        private int selectedRange = 0;
        private int selectedChannelVolt = 0;




        //cursors
        private int VerticalCursorSrc = 0;
        private int last_ver_cur_src=0;
        private double VerticalCursorA = 0;
        private double VerticalCursorB = 0;
        double tA = 0;
        double tB = 0;
        double last_tA = 0;
        double last_tB = 0;

        private string timeDif;
        private string timeA;
        private string timeB;
        private string frequency;
        private string UA;
        private string UB;
        private string DiffU;


        private int HorizontalCursorSrc = 0;
        private int last_hor_cur_src = 0;
        private double HorizontalCursorA = 0;
        private double HorizontalCursorB = 0;
        double uA = 0;
        double uB = 0;
        double last_uA = 0;
        double last_uB = 0;
        private string voltDif;
        private string voltA;
        private string voltB;


        //signal plot params
        private bool last_interpolation = true;
        private bool last_showPoints = false;
        private bool last_antialiasing = true;
        private float smoothing = 0.5F;

        private bool XYplot = false;
        private bool last_XYplot = false;
        
        private bool interpolation = true;
        private bool showPoints = false;
        private bool antialiasing=true;

        private double maxX = 0;
        private double maxY = 0;
        private double minX = 0;
        private double minY = 0;
        private double last_maxX = 0;
        private double last_maxY = 0;
        private double last_minX = 0;
        private double last_minY = 0;

        private bool measValid = false;
        private bool last_measValid = false; 


        public GraphPane scopePane;

        private string status_text="";


        double scale=1;
        double horPosition=0.5;
        double LastHorPosition = 0.5;

        
    

        public Scope(Device dev)
        {
            InitializeComponent();
            zedGraphControl_scope.MasterPane[0].IsFontsScaled = false;
            zedGraphControl_scope.MasterPane[0].Title.IsVisible = false;
            zedGraphControl_scope.MasterPane[0].XAxis.MajorGrid.IsVisible = true;
            zedGraphControl_scope.MasterPane[0].XAxis.Title.IsVisible = false;

            zedGraphControl_scope.IsEnableZoom = false;

            zedGraphControl_scope.MasterPane[0].YAxis.MajorGrid.IsVisible = true;
            zedGraphControl_scope.MasterPane[0].YAxis.Title.IsVisible = false;
            this.device = dev;
            ScopeDevice=device.scopeCfg;
            set_scope_default();

            label_meas1.Text = "";
            label_meas2.Text = "";
            label_meas3.Text = "";
            label_meas4.Text = "";
            label_meas5.Text = "";

            this.toolStripMenuItem_XY_plot.Enabled = false;

            this.Text = "Scope - (" + device.get_port() + ") " + device.get_name(); 

            validate_channel_colors();
            validate_radio_btns();
            validate_menu();

            scopePane = zedGraphControl_scope.GraphPane;
            
            GUITimer = new System.Timers.Timer(5);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            GUITimer.Start();

            ZedTimer = new System.Timers.Timer(50);
            ZedTimer.Elapsed += new ElapsedEventHandler(Zed_update);
            ZedTimer.Start();

            processSignal_th = new Thread(process_signals);
            meas.clearMeasurements();
            Thread.Sleep(10);
            scope_start();

      
        }

        private void Zed_update(object sender, ElapsedEventArgs e)
        {
            bool update = false;

            if (last_XYplot != XYplot)
            {
                update = true;
                paint_signals();
                paint_markers();
                vertical_cursor_update();
                horizontal_cursor_update();
                update_X_axe();
                last_XYplot = XYplot;
            }

            if (maxX != last_maxX || minX != last_minX)
            {
                scopePane.XAxis.Scale.MaxAuto = false;
                scopePane.XAxis.Scale.MinAuto = false;

                scopePane.XAxis.Scale.Max = maxX;
                scopePane.XAxis.Scale.Min = minX;

                if (XYplot)
                {
                    scopePane.XAxis.Scale.Max = maxY;
                    scopePane.XAxis.Scale.Min = minY;
                }

                last_maxX = maxX;
                last_minX = minX;

                update = true;

                scopePane.CurveList.Clear();
                paint_signals();
                paint_markers();
                vertical_cursor_update();
                horizontal_cursor_update();
                paint_cursors();
                try
                {
                    scopePane.AxisChange();
                }
                catch (Exception ex) { }
            }
            if (maxY != last_maxY || minY != last_minY || horPosition!=LastHorPosition)
            {

                scopePane.YAxis.Scale.MaxAuto = false;
                scopePane.YAxis.Scale.MinAuto = false;

                scopePane.YAxis.Scale.Max = maxY;
                scopePane.YAxis.Scale.Min = minY;


                last_maxY = maxY;
                last_minY = minY;
                LastHorPosition = horPosition;
                update = true;
                process_signals();
                scopePane.CurveList.Clear();
                paint_signals();
                paint_markers();
                vertical_cursor_update();
                horizontal_cursor_update();
                paint_cursors();
                try
                {
                    scopePane.AxisChange();
                }
                catch (Exception ex) { }
            }

            if (gain[0] != last_gain[0] || gain[1] != last_gain[1] || gain[2] != last_gain[2] || gain[3] != last_gain[3] || gain[4] != last_gain[4] || offset[0] != last_offset[0] || offset[1] != last_offset[1] || offset[2] != last_offset[2] || offset[3] != last_offset[3] || offset[4] != last_offset[4])
            {
                update = true;
                process_signals();
                scopePane.CurveList.Clear();
                paint_signals();
                paint_markers();
                vertical_cursor_update();
                horizontal_cursor_update();
                paint_cursors();
                Array.Copy(gain, last_gain, 5);
                Array.Copy(offset, last_offset, 5);
            }

            if (interpolation != last_interpolation || showPoints != last_showPoints || last_antialiasing != antialiasing)
            {
                update = true;
                process_signals();
                scopePane.CurveList.Clear();
                paint_signals();
                paint_markers();
                paint_cursors();
                last_interpolation = interpolation;
                last_showPoints = showPoints;
                last_antialiasing = antialiasing;
            }

            if (last_tA != tA || last_tB != tB || last_ver_cur_src != VerticalCursorSrc)
            {
                update = true;
                scopePane.CurveList.Clear();
                paint_signals();
                paint_markers();
                vertical_cursor_update();
                paint_cursors();
                last_tA = tA;
                last_tB = tB;
                last_ver_cur_src = VerticalCursorSrc;
            }

            if (last_uA != uA || last_uB != uB || last_hor_cur_src != HorizontalCursorSrc)
            {
                update = true;
                scopePane.CurveList.Clear();
                paint_signals();
                paint_markers();
                horizontal_cursor_update();
                paint_cursors();
                last_uA = uA;
                last_uB = uB;
                last_hor_cur_src = HorizontalCursorSrc;
            }

            if (last_measValid != measValid)
            {
                update = true;
                meas.calculateMeasurements(device.scopeCfg.samples, device.scopeCfg.ranges[1, selectedRange], device.scopeCfg.ranges[0, selectedRange], device.scopeCfg.actualChannels, device.scopeCfg.realSmplFreq, device.scopeCfg.timeBase.Length, device.scopeCfg.actualRes);
                last_measValid = measValid;
            }

            if (last_math != math) {
                update = true;
                scopePane.CurveList.Clear();
                process_signals();
                paint_signals();
                paint_markers();
                vertical_cursor_update();
                horizontal_cursor_update();
                paint_cursors();
                last_math = math;
            }

            if (last_pretrigger != pretrigger)
            {
                set_prettriger(pretrigger);
                last_pretrigger = pretrigger;
            }

            if (last_trigger_level != triggerLevel) {
                set_trigger_level(triggerLevel);
                last_trigger_level = triggerLevel;
                scopePane.CurveList.Clear();
                paint_signals();
                paint_markers();
                update = true;
            }


            if (update)
            {
                this.Invalidate();
            }
        }

        public void update_Y_axe() {
            maxY = (double)device.scopeCfg.ranges[1, selectedRange] / 1000;
            minY = (double)device.scopeCfg.ranges[0, selectedRange] / 1000;
        }

        public void update_X_axe()
        {
            if (XYplot)
            {
                maxX = (double)device.scopeCfg.ranges[1, selectedRange] / 1000;
                minY = (double)device.scopeCfg.ranges[0, selectedRange] / 1000;
            }
            else
            {
                double maxTime = device.scopeCfg.maxTime;
                double interval = scale * maxTime;
                double posmin = (interval / 2);
                double posScale = (maxTime - interval) / maxTime;
                maxX = (double)(maxTime) * horPosition * posScale + posmin + interval / 2;
                minX = (double)(maxTime) * horPosition * posScale + posmin - interval / 2;
            }
        }

        public void vertical_cursor_update() {
            if (VerticalCursorSrc != 0)
            {
                if (!XYplot)
                {
                    double scale = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 / Math.Pow(2, (double)device.scopeCfg.actualRes);
                    double off = (double)device.scopeCfg.ranges[0, selectedRange] / 1000;

                    //vypocet casu
                    tA = VerticalCursorA * maxX + (1 - VerticalCursorA) * minX;
                    tB = VerticalCursorB * maxX + (1 - VerticalCursorB) * minX;
                    int indexUA = (int)(tA / device.scopeCfg.maxTime * signal_ch1.Length);
                    int indexUB = (int)(tB / device.scopeCfg.maxTime * signal_ch1.Length);
                    double VA = 0;
                    double VB = 0;
                    if (indexUA >= signal_ch1.Length)
                    {
                        indexUA = signal_ch1.Length - 1;
                    }
                    if (indexUB >= signal_ch1.Length)
                    {
                        indexUB = signal_ch1.Length - 1;
                    }
                    //vypocet linearni interpolace napeti kurzoru
                    if (VerticalCursorSrc <= device.scopeCfg.actualChannels)
                    {
                        if (indexUA < signal_ch1.Length - 1)
                        {

                            VA = (device.scopeCfg.samples[VerticalCursorSrc - 1, indexUA] * scale + off) + (tA - device.scopeCfg.timeBase[indexUA]) / (device.scopeCfg.timeBase[indexUA + 1] - device.scopeCfg.timeBase[indexUA]) * ((device.scopeCfg.samples[VerticalCursorSrc - 1, indexUA + 1] * scale + off) - (device.scopeCfg.samples[VerticalCursorSrc - 1, indexUA] * scale + off));
                        }
                        else
                        {
                            VA = (device.scopeCfg.samples[VerticalCursorSrc - 1, indexUA - 1] * scale + off);
                        }

                        if (indexUB < signal_ch1.Length - 1)
                        {
                            VB = (device.scopeCfg.samples[VerticalCursorSrc - 1, indexUB] * scale + off) + (tB - device.scopeCfg.timeBase[indexUB]) / (device.scopeCfg.timeBase[indexUB + 1] - device.scopeCfg.timeBase[indexUB]) * ((device.scopeCfg.samples[VerticalCursorSrc - 1, indexUB + 1] * scale + off) - (device.scopeCfg.samples[VerticalCursorSrc - 1, indexUB] * scale + off));
                        }
                        else
                        {
                            VB = (device.scopeCfg.samples[VerticalCursorSrc - 1, indexUB - 1] * scale + off);
                        }
                    }
                    double td = tA - tB;
                    double ud = VA - VB;
                    double f = Math.Abs(1 / td);

                    //formatovani stringu
                    if (td >= 1 || td <= -1)
                    {
                        this.timeDif = "dt " + (Math.Round(td, 3)).ToString() + " s";
                    }
                    else if (td >= 0.001 || td <= -0.001)
                    {
                        this.timeDif = "dt " + (Math.Round(td * 1000, 3)).ToString() + " ms";
                    }else
                    {
                        this.timeDif = "dt " + (Math.Round(td * 1000000, 3)).ToString() + " us";
                    }

                    if (tA >= 1 || tA <= -1)
                    {
                        this.timeA = "t " + (Math.Round(tA, 3)).ToString() + " s";
                    }
                    else
                    {
                        this.timeA = "t " + (Math.Round(tA * 1000, 3)).ToString() + " ms";
                    }
                    if (tB >= 1 || tB <= -1)
                    {
                        this.timeB = "t " + (Math.Round(tB, 3)).ToString() + " s";
                    }
                    else
                    {
                        this.timeB = "t " + (Math.Round(tB * 1000, 3)).ToString() + " ms";
                    }
                    if (Double.IsInfinity(f))
                    {
                        this.frequency = "f Inf";
                    }
                    else if (f >= 1000000)
                    {
                        this.frequency = "f " + (Math.Round(f / 1000000, 3)).ToString() + " MHz";
                    }
                    else if (f >= 1000)
                    {
                        this.frequency = "f " + (Math.Round(f / 1000, 3)).ToString() + " kHz";
                    }
                    else
                    {
                        this.frequency = "f " + (Math.Round(f, 3)).ToString() + " Hz";
                    }

                    if (VA >= 1 || VA <= -1)
                    {
                        this.UA = "U " + (Math.Round(VA, 3)).ToString() + " V";
                    }
                    else
                    {
                        this.UA = "U " + (Math.Round(VA * 1000, 2)).ToString() + " mV";
                    }

                    if (VB >= 1 || VB <= -1)
                    {
                        this.UB = "U " + (Math.Round(VB, 3)).ToString() + " V";
                    }
                    else
                    {
                        this.UB = "U " + (Math.Round(VB * 1000, 2)).ToString() + " mV";
                    }

                    if (ud >= 1 || ud <= -1)
                    {
                        this.DiffU = "dU " + (Math.Round(ud, 3)).ToString() + " V";
                    }
                    else
                    {
                        this.DiffU = "dU " + (Math.Round(ud * 1000, 2)).ToString() + " mV";
                    }
                }
                else
                {

                    double off = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 * (double)offset[VerticalCursorSrc - 1] / 1000 * gain[VerticalCursorSrc - 1] * 2;

                    tA = ((VerticalCursorA * maxX + (1 - VerticalCursorA) * minX));
                    tB = ((VerticalCursorB * maxX + (1 - VerticalCursorB) * minX));
                    double ud = (tA - tB) / gain[VerticalCursorSrc - 1];
                    double tmpA = (tA - off) / gain[VerticalCursorSrc - 1];
                    double tmpB = (tB - off) / gain[VerticalCursorSrc - 1];
                    if (ud >= 1 || ud <= -1)
                    {
                        this.DiffU = "dU " + (Math.Round(ud, 3)).ToString() + " V";
                    }
                    else
                    {
                        this.DiffU = "dU " + (Math.Round(ud * 1000, 2)).ToString() + " mV";
                    }
                    if (tmpA >= 1 || tmpA <= -1)
                    {
                        this.UA = "UA " + (Math.Round(tmpA, 3)).ToString() + " V";
                    }
                    else
                    {
                        this.UA = "UA " + (Math.Round(tmpA * 1000, 2)).ToString() + " mV";
                    }
                    if (tmpB >= 1 || tmpB <= -1)
                    {
                        this.UB = "UB " + (Math.Round(tmpB, 3)).ToString() + " V";
                    }
                    else
                    {
                        this.UB = "UB " + (Math.Round(tmpB * 1000, 2)).ToString() + " mV";
                    }
                    this.timeDif = "";
                    this.frequency = "";
                    this.timeA = "";
                    this.timeB = "";

                }
            }
        }

        public void horizontal_cursor_update() {
            if (HorizontalCursorSrc!=0)
            {
                double off = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 * (double)offset[HorizontalCursorSrc - 1] / 1000 * gain[HorizontalCursorSrc - 1] * 2;

                uA = ((HorizontalCursorA * maxY + (1 - HorizontalCursorA) * minY));
                uB = ((HorizontalCursorB * maxY + (1 - HorizontalCursorB) * minY));
                double ud = (uA - uB) / gain[HorizontalCursorSrc - 1];
                double tmpA = (uA - off) / gain[HorizontalCursorSrc - 1];
                double tmpB = (uB - off) / gain[HorizontalCursorSrc - 1];
                if (ud >= 1 || ud <= -1)
                {
                    this.voltDif = "dU " + (Math.Round(ud, 3)).ToString() + " V";
                }
                else
                {
                    this.voltDif = "dU " + (Math.Round(ud * 1000, 2)).ToString() + " mV";
                }
                if (tmpA >= 1 || tmpA <= -1)
                {
                    this.voltA = "U " + (Math.Round(tmpA, 3)).ToString() + " V";
                }
                else
                {
                    this.voltA = "U " + (Math.Round(tmpA * 1000, 2)).ToString() + " mV";
                }
                if (tmpB >= 1 || tmpB <= -1)
                {
                    this.voltB = "U " + (Math.Round(tmpB, 3)).ToString() + " V";
                }
                else
                {
                    this.voltB = "U " + (Math.Round(tmpB * 1000, 2)).ToString() + " mV";
                }
            }
        }

        private void Update_GUI(object sender, ElapsedEventArgs e)
        {
            if (scope_q.Count > 0)
            {
                messg = scope_q.Dequeue();
                if (messg == null) {
                    return;
                }
                switch (messg.GetRequest())
                {
                    case Message.MsgRequest.SCOPE_NEW_DATA:
                        status_text = "";
                        //meas.calculateMeasurements(device.scopeCfg.samples, device.scopeCfg.ranges[1, selectedRange], device.scopeCfg.ranges[0, selectedRange], device.scopeCfg.actualChannels, device.scopeCfg.sampligFreq, device.scopeCfg.timeBase.Length,device.scopeCfg.actualRes);
                        if (processSignal_th!=null && processSignal_th.IsAlive)
                        {
                            processSignal_th.Join();
                        }
                        if (calcSignal_th!=null && calcSignal_th.IsAlive)
                        {
                            calcSignal_th.Join();
                        }
                        calcSignal_th = new Thread(() => meas.calculateMeasurements(device.scopeCfg.samples, device.scopeCfg.ranges[1, selectedRange], device.scopeCfg.ranges[0, selectedRange], device.scopeCfg.actualChannels, device.scopeCfg.realSmplFreq, device.scopeCfg.timeBase.Length,device.scopeCfg.actualRes));
                        calcSignal_th.Start();
                        processSignal_th = new Thread(process_signals);
                        processSignal_th.Start();
                        Thread.Sleep(1);
                        if (processSignal_th.IsAlive)
                        {
                            processSignal_th.Join();
                        }
                        if (calcSignal_th.IsAlive)
                        {
                            calcSignal_th.Join();
                        }
                        scopePane.CurveList.Clear();
                        //process_signals();
                        paint_signals();
                        update_Y_axe();
                        update_X_axe();
                        paint_markers();
                        vertical_cursor_update();
                        paint_cursors();
                        
                        this.Invalidate();

                        if (device.scopeCfg.mode == Scope.mode_def.AUTO || device.scopeCfg.mode == Scope.mode_def.NORMAL)
                        {
                            Thread.Sleep(100);
                            device.takeCommsSemaphore(semaphoreTimeout*2 + 108);
                            device.send(Commands.SCOPE + ":" + Commands.SCOPE_NEXT + ";");
                            device.giveCommsSemaphore();
                        }
                        break;

                    case Message.MsgRequest.SCOPE_TRIGGERED:
                        status_text = "Trig";
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.SCOPE_WAIT:
                        scopePane.CurveList.Clear();
                        status_text = "Wait";
                        this.Invalidate();
                        break;
                    case Message.MsgRequest.SCOPE_FREQ:
                        device.scopeCfg.realSmplFreq = messg.GetNum();
                        break;
                }
                
            }
            
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            zedGraphControl_scope.Refresh();
            if (VerticalCursorSrc != 0) {
                this.label_cur_freq.Text = this.frequency;
                this.label_cur_time_a.Text = this.timeA;
                this.label_cur_time_b.Text = this.timeB;
                this.label_cur_ua.Text = this.UA;
                this.label_cur_ub.Text = this.UB;
                this.label_time_diff.Text = this.timeDif;
                this.label_ver_cur_du.Text = this.DiffU;
            }
            if (HorizontalCursorSrc != 0) {
                this.label_hor_cur_volt_diff.Text = voltDif;
                this.label_cur_u_a.Text = voltA;
                this.label_cur_u_b.Text = voltB;
            }
            if (meas.getMeasCount() > 0) {
                for (int i = 0; i < meas.getMeasCount(); i++)
                {
                    switch (i) { 
                        case 0:
                            this.label_meas1.Text = meas.getMeas(0);
                            this.label_meas1.ForeColor = meas.getColor(0);
                            break;
                        case 1:
                            this.label_meas2.Text = meas.getMeas(1);
                            this.label_meas2.ForeColor = meas.getColor(1);
                            break;
                        case 2:
                            this.label_meas3.Text = meas.getMeas(2);
                            this.label_meas3.ForeColor = meas.getColor(2);
                            break;
                        case 3:
                            this.label_meas4.Text = meas.getMeas(3);
                            this.label_meas4.ForeColor = meas.getColor(3);
                            break;
                        case 4:
                            this.label_meas5.Text = meas.getMeas(4);
                            this.label_meas5.ForeColor = meas.getColor(4);
                            break;
                    }
                }
            }
            this.label_scope_status.Text = status_text;
            if (device.scopeCfg.realSmplFreq >= 1000000) {
                this.label_samplingfreq.Text = Math.Round((double)device.scopeCfg.realSmplFreq / 1000000, 3).ToString() + " MSPS";
            }
            else if (device.scopeCfg.realSmplFreq >= 1000)
            {
                this.label_samplingfreq.Text = Math.Round((double)device.scopeCfg.realSmplFreq / 1000, 3).ToString() + " kSPS";
            }
            else {
                this.label_samplingfreq.Text = Math.Round((double)device.scopeCfg.realSmplFreq, 3).ToString() + " SPS";
            }
            
            base.OnPaint(e);
        }

        private void Scope_FormClosing(object sender, FormClosingEventArgs e)
        {
            scope_stop();
            GUITimer.Stop();
            device.scopeClosed();

        }

        public void add_message(Message msg)
        {
            this.scope_q.Enqueue(msg);
        }


        //
        // sending commands to scope
        //

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
            device.takeCommsSemaphore(semaphoreTimeout+99);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_NEXT + ";");
            device.giveCommsSemaphore();
        }


        public void set_data_depth(string dataDepth) {
            device.takeCommsSemaphore(semaphoreTimeout+111);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_DATA_DEPTH + " " + dataDepth + ";");
            device.giveCommsSemaphore();
        }

        public void set_num_of_samples(string numSmp) {
            device.takeCommsSemaphore(semaphoreTimeout + 112);
            device.send(Commands.SCOPE + ":" + Commands.DATA_LENGTH + " " + numSmp + ";");
            device.giveCommsSemaphore();
        }


        public void set_sampling_freq(string smpFreq)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 113);
            device.send(Commands.SCOPE + ":" + Commands.SAMPLING_FREQ + " " + smpFreq+ ";");
            device.giveCommsSemaphore();
        }

        public void set_num_of_channels(string chann) {
            device.takeCommsSemaphore(semaphoreTimeout + 114);
            device.send(Commands.SCOPE + ":" + Commands.CHANNELS + " " + chann + ";");
            device.giveCommsSemaphore();
        }

        public void set_trigger_channel(string chann)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 115);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_CHANNEL + " " + chann + ";");
            device.giveCommsSemaphore();
        }

        public void set_trigger_mode(mode_def mod)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 116);
            switch (mod)
            {
                case mode_def.AUTO:
                    device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_MODE + " " + Commands.MODE_AUTO + ";");
                    break;
                case mode_def.NORMAL:
                    device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_MODE + " " + Commands.MODE_NORMAL + ";");
                    break;
                case mode_def.SINGLE:
                    device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_MODE + " " + Commands.MODE_SINGLE + ";");
                    break;
            }
            device.set_scope_mode(mod);
            device.giveCommsSemaphore();
        }

        public void set_trigger_edge_fall() {
            device.takeCommsSemaphore(semaphoreTimeout + 117);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_EDGE + " " + Commands.EDGE_FALLING + ";");
            device.giveCommsSemaphore();
        }
        public void set_trigger_edge_rise() {
            device.takeCommsSemaphore(semaphoreTimeout + 118);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_EDGE + " " + Commands.EDGE_RISING + ";");
            device.giveCommsSemaphore();
        }

        public void set_prettriger(double pre) {
            device.takeCommsSemaphore(semaphoreTimeout + 119);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_PRETRIGGER + " ");
            device.send_short((int)(pre*65536));
            device.send(";");
            device.giveCommsSemaphore();
        }

        public void set_trigger_level(double level) {
            device.takeCommsSemaphore(semaphoreTimeout + 120);
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_LEVEL + " ");
            device.send_short((int)(level * 65536));
            device.send(";");
            device.giveCommsSemaphore();
        }


        public bool validate_buffer_usage() {
            if (adcRes > 8)
            {
                if (2 * numSamples * actualCahnnels <= ScopeDevice.maxBufferLength)
                {
                    return true;
                }
            }
            else {
                if (numSamples * actualCahnnels <= ScopeDevice.maxBufferLength)
                {
                    return true;
                }
            }
            return false;
        }

        public void set_scope_default()
        {
            //must be same as in window designer!!!
            set_trigger_mode(mode_def.AUTO);

            set_sampling_freq(Commands.FREQ_1K);
            device.scopeCfg.sampligFreq = 1000;

            triggeredge = triggerEdge_def.RISE;
            set_trigger_edge_rise();

            triggerLevel = 0.5;
            last_trigger_level = 0.5;
            set_trigger_level(triggerLevel);

            pretrigger = 0.5;
            last_pretrigger = 0.5;
            set_prettriger(pretrigger);

            actualCahnnels = 1;
            set_num_of_channels(Commands.CHANNELS_1);

            numSamples=100;
            set_num_of_samples(Commands.SAMPLES_100);

            adcRes = 12;
            set_data_depth(Commands.DATA_DEPTH_12B);


            triggerChannel = 1;
            set_trigger_channel(Commands.CHANNELS_1);

        }


        public void reset_volt_set() {
            gain = new double[5] { 1, 1, 1, 1, 1 };
            offset = new int[5] { 0, 0, 0, 0, 0 };
            this.trackBar_vol_level.Value = 500;
            redrawVolt();
        }


        //
        // Callbacks
        //

        private void bitsToolStripMenuItem_12bit_Click(object sender, EventArgs e)
        {
            if (this.bitsToolStripMenuItem_12bit.Checked)
            {
                int tmpAdcRes = adcRes;
                this.adcRes = 12;
                if (validate_buffer_usage())
                {
                    set_data_depth(Commands.DATA_DEPTH_12B);
                    if (device.scopeCfg.sampligFreq == int.MaxValue) {
                        set_sampling_freq(Commands.FREQ_MAX);
                    }
                }
                else
                {
                    adcRes = tmpAdcRes;
                    show_buffer_err_message();
                }
                update_data_depth_menu();
            }else{
                this.bitsToolStripMenuItem_12bit.Checked = true;
            }
        }
        private void bitsToolStripMenuItem_8bit_Click(object sender, EventArgs e)
        {
            if (this.bitsToolStripMenuItem_8bit.Checked)
            {
                int tmpAdcRes = adcRes;
                this.adcRes = 8;
                if (validate_buffer_usage())
                {
                    set_data_depth(Commands.DATA_DEPTH_8B);
                    if (device.scopeCfg.sampligFreq == int.MaxValue)
                    {
                        set_sampling_freq(Commands.FREQ_MAX);
                    }
                }
                else
                {
                    adcRes = tmpAdcRes;
                    show_buffer_err_message();
                }
                update_data_depth_menu();
            }else {
                this.bitsToolStripMenuItem_8bit.Checked = true;
            }
        }

        
        private void checkBox_trig_normal_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_trig_normal.Checked)
            {
                set_trigger_mode(mode_def.NORMAL);
                scope_next();
                this.checkBox_trig_auto.Checked = false;
                this.checkBox_trig_single.Checked = false;
                this.checkBox_trig_single.Text = "Stop";
                
            }
        }

        private void checkBox_trig_auto_CheckedChanged(object sender, EventArgs e)
        {
            if (this.checkBox_trig_auto.Checked)
            {
                set_trigger_mode(mode_def.AUTO);
                scope_next();
                this.checkBox_trig_normal.Checked = false;
                this.checkBox_trig_single.Checked = false;
                this.checkBox_trig_single.Text = "Stop";
                
            }
        }
        private void checkBox_trig_single_Click(object sender, EventArgs e)
        {
            this.checkBox_trig_single.Checked = true;
            if (this.checkBox_trig_single.Text.Equals("Stop"))
            {
                this.checkBox_trig_single.Text = "Single";
                device.set_scope_mode(mode_def.SINGLE);
            }
            else if (this.checkBox_trig_single.Text.Equals("Single"))
            {
                set_trigger_mode(mode_def.SINGLE);
                scope_next();
                add_message(new Message(Message.MsgRequest.SCOPE_WAIT));
            }          
            this.checkBox_trig_auto.Checked = false;
            this.checkBox_trig_normal.Checked = false;
        }

        private void checkBox_trig_rise_Click(object sender, EventArgs e)
        {
            if (this.checkBox_trig_rise.Checked)
            {
                this.checkBox_trig_fall.Checked = false;
                set_trigger_edge_rise();
            }
            this.checkBox_trig_rise.Checked = true;
        }
        private void checkBox_trig_fall_Click(object sender, EventArgs e)
        {
            if (this.checkBox_trig_fall.Checked)
            {
                this.checkBox_trig_rise.Checked = false;
                set_trigger_edge_fall();
            }
            this.checkBox_trig_fall.Checked = true;
        }

        //signal zoom
        private void trackBar_zoom_ValueChanged(object sender, EventArgs e)
        {
            scale = 1.0 - (double)(this.trackBar_zoom.Value) / (this.trackBar_zoom.Maximum - this.trackBar_zoom.Minimum + 10);
            update_X_axe();
        }
        private void trackBar_position_ValueChanged(object sender, EventArgs e)
        {
            horPosition = (double)(this.trackBar_position.Value) / (this.trackBar_position.Maximum - this.trackBar_position.Minimum);
            update_X_axe();
        }

        private void radioButton_1k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_1k.Checked)
            {
                set_sampling_freq(Commands.FREQ_1K);
                device.scopeCfg.sampligFreq = 1000;
            }
        }

        private void radioButton_2k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_2k.Checked)
            {
                set_sampling_freq(Commands.FREQ_2K);
                device.scopeCfg.sampligFreq = 2000;
            }
        }

        private void radioButton_5k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_5k.Checked)
            {
                set_sampling_freq(Commands.FREQ_5K);
                device.scopeCfg.sampligFreq = 5000;
            }
        }

        private void radioButton_10k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_10k.Checked)
            {
                set_sampling_freq(Commands.FREQ_10K);
                device.scopeCfg.sampligFreq = 10000;
            }
        }

        private void radioButton_20k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_20k.Checked)
            {
                set_sampling_freq(Commands.FREQ_20K);
                device.scopeCfg.sampligFreq = 20000;
            }
        }

        private void radioButton_50k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_50k.Checked)
            {
                set_sampling_freq(Commands.FREQ_50K);
                device.scopeCfg.sampligFreq = 50000;
            }
        }

        private void radioButton_100k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_100k.Checked)
            {
                set_sampling_freq(Commands.FREQ_100K);
                device.scopeCfg.sampligFreq = 100000;
            }
        }

        private void radioButton_200k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_200k.Checked)
            {
                set_sampling_freq(Commands.FREQ_200K); ;
                device.scopeCfg.sampligFreq = 200000;
            }
        }

        private void radioButton_500k_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_500k.Checked)
            {
                set_sampling_freq(Commands.FREQ_500K);
                device.scopeCfg.sampligFreq = 500000;
            }
        }

        private void radioButton_1m_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_1m.Checked)
            {
                set_sampling_freq(Commands.FREQ_1M);
                device.scopeCfg.sampligFreq = 1000000;
            }
        }

        private void radioButton_2m_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_2m.Checked)
            {
                set_sampling_freq(Commands.FREQ_2M);
                device.scopeCfg.sampligFreq = 2000000;
            }
        }

        private void radioButton_5m_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_5m.Checked)
            {
                set_sampling_freq(Commands.FREQ_5M);
                device.scopeCfg.sampligFreq = 5000000;
            }
        }

        private void radioButton_sampl_max_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_sampl_max.Checked)
            {
                set_sampling_freq(Commands.FREQ_MAX);
                device.scopeCfg.sampligFreq = int.MaxValue;
            }
        }




        private void radioButton_01x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_01x.Checked)
            {
                this.gain[selectedChannelVolt] = 0.1;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }
        private void radioButton_02x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_02x.Checked)
            {
                this.gain[selectedChannelVolt] = 0.2;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }
        private void radioButton_05x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_05x.Checked)
            {
                this.gain[selectedChannelVolt] = 0.5;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }
        private void radioButton_1x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_1x.Checked)
            {
                this.gain[selectedChannelVolt] = 1;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void radioButton_2x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_2x.Checked)
            {
                this.gain[selectedChannelVolt] = 2;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void radioButton_5x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_5x.Checked)
            {
                this.gain[selectedChannelVolt] = 5;
                //(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void radioButton_10x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_10x.Checked)
            {
                this.gain[selectedChannelVolt] = 10;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void radioButton_20x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_20x.Checked)
            {
                this.gain[selectedChannelVolt] = 20;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void radioButton_50x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_50x.Checked)
            {
                this.gain[selectedChannelVolt] = 50;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void radioButton_100x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_100x.Checked)
            {
                this.gain[selectedChannelVolt] = 100;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void radioButton_200x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_200x.Checked)
            {
                this.gain[selectedChannelVolt] = 200;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void radioButton_500x_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_500x.Checked)
            {
                this.gain[selectedChannelVolt] = 500;
                //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
            }
        }

        private void trackBar_vol_level_ValueChanged(object sender, EventArgs e)
        {
            offset[selectedChannelVolt] = trackBar_vol_level.Value - 500;
            //add_message(new Message(Message.MsgRequest.CHANGE_GAIN));
        }

        private void channelToolStripMenuItem_1ch_Click(object sender, EventArgs e)
        {
            if (this.channelToolStripMenuItem_1ch.Checked)
            {
                int tmpActualCahnnels = actualCahnnels;
                this.actualCahnnels = 1;
                if (validate_buffer_usage())
                {
                    validate_radio_btns();
                    if (triggerChannel >= 1) {
                        triggerChannel = 1;
                        set_trigger_channel(Commands.CHANNELS_1);
                        this.radioButton_trig_ch1.Checked = true;
                    }
                    set_num_of_channels(Commands.CHANNELS_1);
                }
                else
                {
                    actualCahnnels = tmpActualCahnnels;
                    show_buffer_err_message();
                }
            }
            validate_channel_colors();
            update_channels_menu();
        }

        private void channelToolStripMenuItem_2ch_Click(object sender, EventArgs e)
        {
            if (this.channelToolStripMenuItem_2ch.Checked)
            {
                int tmpActualCahnnels = actualCahnnels;
                this.actualCahnnels = 2;
                if (validate_buffer_usage())
                {
                    validate_radio_btns();
                    if (triggerChannel >= 2)
                    {
                        triggerChannel = 2;
                        set_trigger_channel(Commands.CHANNELS_2);
                        this.radioButton_trig_ch2.Checked = true;
                    }
                    set_num_of_channels(Commands.CHANNELS_2);
                }
                else
                {
                    actualCahnnels = tmpActualCahnnels;
                    show_buffer_err_message();
                }
            }
            validate_channel_colors();
            update_channels_menu();
        }

        private void channelToolStripMenuItem_3ch_Click(object sender, EventArgs e)
        {
            if (this.channelToolStripMenuItem_3ch.Checked)
            {
                int tmpActualCahnnels = actualCahnnels;
                this.actualCahnnels = 3;
                if (validate_buffer_usage())
                {
                    validate_radio_btns();
                    if (triggerChannel >= 3)
                    {
                        triggerChannel = 3;
                        set_trigger_channel(Commands.CHANNELS_3);
                        this.radioButton_trig_ch3.Checked = true;
                    }
                    set_num_of_channels(Commands.CHANNELS_3);
                }
                else
                {
                    actualCahnnels = tmpActualCahnnels;
                    show_buffer_err_message();
                }
            }
            validate_channel_colors();
            update_channels_menu();
        }

        private void channelToolStripMenuItem_4ch_Click(object sender, EventArgs e)
        {
            if (this.channelToolStripMenuItem_4ch.Checked)
            {
                int tmpActualCahnnels = actualCahnnels;
                this.actualCahnnels = 4;
                if (validate_buffer_usage())
                {
                    validate_radio_btns();
                    if (triggerChannel >= 4)
                    {
                        triggerChannel = 4;
                        set_trigger_channel(Commands.CHANNELS_4);
                        this.radioButton_trig_ch4.Checked = true;
                    }
                    set_num_of_channels(Commands.CHANNELS_4);
                }
                else
                {
                    actualCahnnels = tmpActualCahnnels;
                    show_buffer_err_message();
                }
            }
            validate_channel_colors();
            update_channels_menu();
        }

        private void radioButton_volt_ch1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_volt_ch1.Checked)
            {
                this.selectedChannelVolt = 0;
                redrawVolt();
            }
        }

        private void radioButton_volt_ch2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_volt_ch2.Checked)
            {
                this.selectedChannelVolt = 1;
                redrawVolt();
            }
        }

        private void radioButton_volt_ch3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_volt_ch3.Checked)
            {
                this.selectedChannelVolt = 2;
                redrawVolt();
            }
        }

        private void radioButton_volt_ch4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_volt_ch4.Checked)
            {
                this.selectedChannelVolt = 3;
                redrawVolt();
            }
        }

        private void radioButton_volt_math_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_volt_math.Checked)
            {
                this.selectedChannelVolt = 4;
                redrawVolt();
            }
        }

        private void range0ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.range0ToolStripMenuItem.Checked)
            {
                selectedRange = 0;
                update_Y_axe();
            }
            update_ranges_menu();
        }

        private void range1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.range1ToolStripMenuItem.Checked)
            {
                selectedRange = 1;
                update_Y_axe();
            }
            update_ranges_menu();
        }

        private void range2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.range2ToolStripMenuItem.Checked)
            {
                selectedRange = 2;
                update_Y_axe();
            }
            update_ranges_menu();
        }

        private void range3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.range3ToolStripMenuItem.Checked)
            {
                selectedRange = 3;
                update_Y_axe();
            }
            update_ranges_menu();
        }

        private void radioButton_trig_ch1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_trig_ch1.Checked)
            {
                triggerChannel = 1;
                set_trigger_channel(Commands.CHANNELS_1);
            }
        }

        private void radioButton_trig_ch2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_trig_ch2.Checked)
            {
                triggerChannel = 2;
                set_trigger_channel(Commands.CHANNELS_2);
            }
        }

        private void radioButton_trig_ch3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_trig_ch3.Checked)
            {
                triggerChannel = 3;
                set_trigger_channel(Commands.CHANNELS_3);
            }
        }

        private void radioButton_trig_ch4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_trig_ch4.Checked)
            {
                triggerChannel = 4;
                set_trigger_channel(Commands.CHANNELS_4);
            }
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

        private void trackBar_pretrig_ValueChanged(object sender, EventArgs e)
        {
            pretrigger = ((double)(this.trackBar_pretrig.Value)/100);
            this.maskedTextBox_pretrig.Text = (pretrigger*100).ToString();
        }
        private void validate_pretrigger()
        {
            try
            {
                Double val = Double.Parse(this.maskedTextBox_pretrig.Text);
                if (val > 100)
                {
                    throw new System.ArgumentException("Parameter cannot be greather then 100", "original");
                }
                this.trackBar_pretrig.Value = (int)(val);
                pretrigger = val / 100;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                this.maskedTextBox_pretrig.Text = (pretrigger*100).ToString();
            }
        }

        private void maskedTextBox_trig_level_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(Keys.Enter))
            {
                validate_trigger_level();
            }
        }

        private void maskedTextBox_trig_level_Leave(object sender, EventArgs e)
        {
            validate_trigger_level();
        }

        private void trackBar_trig_level_ValueChanged(object sender, EventArgs e)
        {
            triggerLevel = ((double)(this.trackBar_trig_level.Value)/100);
            this.maskedTextBox_trig_level.Text = (triggerLevel*100).ToString();
        }

        private void validate_trigger_level()
        {
            try
            {
                Double val = Double.Parse(this.maskedTextBox_trig_level.Text);
                if (val > 100)
                {
                    throw new System.ArgumentException("Parameter cannot be greather then 100", "original");
                }
                this.trackBar_trig_level.Value = (int)(val);
                triggerLevel = val / 100;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                this.maskedTextBox_trig_level.Text = (triggerLevel * 100).ToString();
            }
        }

        private void interpolateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            interpolation = this.interpolateToolStripMenuItem.Checked;
        }

        private void showPointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            showPoints = this.showPointsToolStripMenuItem.Checked;
        }

        private void ToolStripMenuItem_100smp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_100smp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 100;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_100);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
               
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_200smp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_200smp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 200;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_200);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_500smp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_500smp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 500;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_500);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_1ksmp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_1ksmp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 1000;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_1K);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_2ksmp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_2ksmp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 2000;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_2K);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_5ksmp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_5ksmp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 5000;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_5K);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_10ksmp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_10ksmp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 10000;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_10K);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_20ksmp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_20ksmp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 20000;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_20K);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_50ksmp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_50ksmp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 50000;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_50K);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void ToolStripMenuItem_100ksmp_Click(object sender, EventArgs e)
        {
            if (this.ToolStripMenuItem_100ksmp.Checked)
            {
                int tmpNumSamples = numSamples;
                this.numSamples = 100000;
                if (validate_buffer_usage())
                {
                    set_num_of_samples(Commands.SAMPLES_100K);
                }
                else
                {
                    numSamples = tmpNumSamples;
                    show_buffer_err_message();
                }
            }
            update_data_len_menu();
        }

        private void button_volt_reset_chan_Click(object sender, EventArgs e)
        {
            gain[selectedChannelVolt] = 1;
            offset[selectedChannelVolt] = 0;
            this.trackBar_vol_level.Value = 500;
            redrawVolt();
        }

        private void button_volt_reset_all_Click(object sender, EventArgs e)
        {
            reset_volt_set();
        }

        private void radioButton_ver_cur_off_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_ver_cur_off.Checked)
            {
                VerticalCursorSrc = 0;
                vertical_cursor_update();
                validate_vertical_curr();
            }
        }
        private void radioButton_ver_cur_ch1_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_ver_cur_ch1.Checked)
            {
                VerticalCursorSrc = 1;
                vertical_cursor_update();
                validate_vertical_curr();
            }
        }
        private void radioButton_ver_cur_ch2_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_ver_cur_ch2.Checked)
            {
                VerticalCursorSrc = 2;
                vertical_cursor_update();
                validate_vertical_curr();
            }
        }
        private void radioButton_ver_cur_ch3_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_ver_cur_ch3.Checked)
            {
                VerticalCursorSrc = 3;
                vertical_cursor_update();
                validate_vertical_curr();
            }
        }
        private void radioButton_ver_cur_ch4_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_ver_cur_ch4.Checked)
            {
                VerticalCursorSrc = 4;
                vertical_cursor_update();
                validate_vertical_curr();
            }
        }
        private void radioButton_ver_cur_math_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_ver_cur_math.Checked)
            {
                VerticalCursorSrc = 5;
                vertical_cursor_update();
                validate_vertical_curr();
            }
        }
        private void trackBar_ver_cur_a_ValueChanged(object sender, EventArgs e)
        {
            VerticalCursorA = (double)(this.trackBar_ver_cur_a.Value) / (this.trackBar_ver_cur_a.Maximum - this.trackBar_ver_cur_a.Minimum);
            vertical_cursor_update();
        }
        private void trackBar_ver_cur_b_ValueChanged(object sender, EventArgs e)
        {
            VerticalCursorB = (double)(this.trackBar_ver_cur_b.Value) / (this.trackBar_ver_cur_b.Maximum - this.trackBar_ver_cur_b.Minimum);
            vertical_cursor_update();
        }

        private void radioButton_hor_cur_off_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_hor_cur_off.Checked)
            {
                HorizontalCursorSrc = 0;
                horizontal_cursor_update();
                validate_horizontal_curr();
            }
        }

        private void radioButton_hor_cur_math_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_hor_cur_math.Checked)
            {
                HorizontalCursorSrc = 5;
                horizontal_cursor_update();
                validate_horizontal_curr();
            }
        }

        private void radioButton_hor_cur_ch1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_hor_cur_ch1.Checked)
            {
                HorizontalCursorSrc = 1;
                horizontal_cursor_update();
                validate_horizontal_curr();
            }
        }

        private void radioButton_hor_cur_ch2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_hor_cur_ch2.Checked)
            {
                HorizontalCursorSrc = 2;
                horizontal_cursor_update();
                validate_horizontal_curr();
            }
        }

        private void radioButton_hor_cur_ch3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_hor_cur_ch3.Checked)
            {
                HorizontalCursorSrc = 3;
                horizontal_cursor_update();
                validate_horizontal_curr();
            }
        }

        private void radioButton_hor_cur_ch4_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton_hor_cur_ch4.Checked)
            {
                HorizontalCursorSrc = 4;
                horizontal_cursor_update();
                validate_horizontal_curr();
            }
        }

        private void trackBar_hor_cur_a_ValueChanged(object sender, EventArgs e)
        {
            HorizontalCursorA = (double)(this.trackBar_hor_cur_a.Value) / (this.trackBar_hor_cur_a.Maximum - this.trackBar_hor_cur_a.Minimum);
            horizontal_cursor_update();
        }

        private void trackBar_hor_cur_b_ValueChanged(object sender, EventArgs e)
        {
            HorizontalCursorB = (double)(this.trackBar_hor_cur_b.Value) / (this.trackBar_hor_cur_b.Maximum - this.trackBar_hor_cur_b.Minimum);
            horizontal_cursor_update();
        }

        private void antiAliasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            antialiasing = this.antiAliasToolStripMenuItem.Checked;
        }


        //end callbacks



        //
        // methodts to keep window actual
        //

        public void redrawVolt() {
            this.trackBar_vol_level.Value = offset[selectedChannelVolt] + 500;
            this.radioButton_01x.Checked = gain[selectedChannelVolt] == 0.1 ? true : false;
            this.radioButton_02x.Checked = gain[selectedChannelVolt] == 0.2 ? true : false;
            this.radioButton_05x.Checked = gain[selectedChannelVolt] == 0.5 ? true : false;
            this.radioButton_1x.Checked = gain[selectedChannelVolt] == 1 ? true : false;
            this.radioButton_2x.Checked = gain[selectedChannelVolt] == 2 ? true : false;
            this.radioButton_5x.Checked = gain[selectedChannelVolt] == 5 ? true : false;
            this.radioButton_10x.Checked = gain[selectedChannelVolt] == 10 ? true : false;
            this.radioButton_20x.Checked = gain[selectedChannelVolt] == 20 ? true : false;
            this.radioButton_50x.Checked = gain[selectedChannelVolt] == 50 ? true : false;
            this.radioButton_100x.Checked = gain[selectedChannelVolt] == 100 ? true : false;
            this.radioButton_200x.Checked = gain[selectedChannelVolt] == 200 ? true : false;
            this.radioButton_500x.Checked = gain[selectedChannelVolt] == 500 ? true : false;
        }


        public void validate_radio_btns() {

            this.radioButton_trig_ch1.Enabled = actualCahnnels >= 1 ? true : false;
            this.radioButton_ver_cur_ch1.Enabled = actualCahnnels >= 1 ? true : false;
            this.radioButton_volt_ch1.Enabled = actualCahnnels >= 1 ? true : false;
            this.radioButton_hor_cur_ch1.Enabled = actualCahnnels >= 1 ? true : false;

            this.radioButton_trig_ch2.Enabled = actualCahnnels >= 2 ? true : false;
            this.radioButton_ver_cur_ch2.Enabled = actualCahnnels >= 2 ? true : false;
            this.radioButton_volt_ch2.Enabled = actualCahnnels >= 2 ? true : false;
            this.radioButton_hor_cur_ch2.Enabled = actualCahnnels >= 2 ? true : false;

            this.radioButton_trig_ch3.Enabled = actualCahnnels >= 3 ? true : false;
            this.radioButton_ver_cur_ch3.Enabled = actualCahnnels >= 3 ? true : false;
            this.radioButton_volt_ch3.Enabled = actualCahnnels >= 3 ? true : false;
            this.radioButton_hor_cur_ch3.Enabled = actualCahnnels >= 3 ? true : false;

            this.radioButton_trig_ch4.Enabled = actualCahnnels >= 4 ? true : false;
            this.radioButton_ver_cur_ch4.Enabled = actualCahnnels >= 4 ? true : false;
            this.radioButton_volt_ch4.Enabled = actualCahnnels >= 4 ? true : false;
            this.radioButton_hor_cur_ch4.Enabled = actualCahnnels >= 4 ? true : false;
        }

        public void validate_channel_colors() {
            this.label_color_ch1.Visible = actualCahnnels >= 1 ? true : false;
            this.label_color_ch2.Visible = actualCahnnels >= 2 ? true : false;
            this.label_color_ch3.Visible = actualCahnnels >= 3 ? true : false;
            this.label_color_ch4.Visible = actualCahnnels >= 4 ? true : false;
        }

        public void update_data_len_menu()
        {
            this.ToolStripMenuItem_100smp.Checked = numSamples == 100 ? true : false;
            this.ToolStripMenuItem_200smp.Checked = numSamples == 200 ? true : false;
            this.ToolStripMenuItem_500smp.Checked = numSamples == 500 ? true : false;
            this.ToolStripMenuItem_1ksmp.Checked = numSamples == 1000 ? true : false;
            this.ToolStripMenuItem_2ksmp.Checked = numSamples == 2000 ? true : false;
            this.ToolStripMenuItem_5ksmp.Checked = numSamples == 5000 ? true : false;
            this.ToolStripMenuItem_10ksmp.Checked = numSamples == 10000 ? true : false;
            this.ToolStripMenuItem_20ksmp.Checked = numSamples == 20000 ? true : false;
            this.ToolStripMenuItem_50ksmp.Checked = numSamples == 50000 ? true : false;
            this.ToolStripMenuItem_100ksmp.Checked = numSamples == 100000 ? true : false;
        }

        public void update_data_depth_menu() {
            this.bitsToolStripMenuItem_8bit.Checked = adcRes == 8 ? true : false;
            this.bitsToolStripMenuItem_12bit.Checked = adcRes == 12 ? true : false;
        }

        public void update_channels_menu(){
            this.channelToolStripMenuItem_1ch.Checked = this.actualCahnnels == 1 ? true : false;
            this.channelToolStripMenuItem_2ch.Checked = this.actualCahnnels == 2 ? true : false;
            this.channelToolStripMenuItem_3ch.Checked = this.actualCahnnels == 3 ? true : false;
            this.channelToolStripMenuItem_4ch.Checked = this.actualCahnnels == 4 ? true : false;
            this.toolStripMenuItem_XY_plot.Enabled = this.actualCahnnels >= 2 ? true : false;
            this.XYplot = measChann >= 2 ? XYplot : false;
        }

        public void update_ranges_menu() {
            this.range0ToolStripMenuItem.Checked = this.selectedRange == 0 ? true : false;
            this.range1ToolStripMenuItem.Checked = this.selectedRange == 1 ? true : false;
            this.range2ToolStripMenuItem.Checked = this.selectedRange == 2 ? true : false;
            this.range3ToolStripMenuItem.Checked = this.selectedRange == 3 ? true : false;
        }


        public void validate_menu()
        {
            if (ScopeDevice.maxNumChannels < 1) {
                this.channelToolStripMenuItem_1ch.Dispose();
            }
            if (ScopeDevice.maxNumChannels < 2)
            {
                this.channelToolStripMenuItem_2ch.Dispose();
            }
            if (ScopeDevice.maxNumChannels < 3)
            {
                this.channelToolStripMenuItem_3ch.Dispose();
            }
            if (ScopeDevice.maxNumChannels < 4)
            {
                this.channelToolStripMenuItem_4ch.Dispose();
            }
            
            if (ScopeDevice.maxBufferLength < 100) {
                this.ToolStripMenuItem_100smp.Dispose();
            }
            if (ScopeDevice.maxBufferLength < 200)
            {
                this.ToolStripMenuItem_200smp.Dispose();
            } if (ScopeDevice.maxBufferLength < 500)
            {
                this.ToolStripMenuItem_500smp.Dispose();
            } if (ScopeDevice.maxBufferLength < 1000)
            {
                this.ToolStripMenuItem_1ksmp.Dispose();
            } if (ScopeDevice.maxBufferLength < 2000)
            {
                this.ToolStripMenuItem_2ksmp.Dispose();
            } if (ScopeDevice.maxBufferLength < 5000)
            {
                this.ToolStripMenuItem_5ksmp.Dispose();
            } if (ScopeDevice.maxBufferLength < 10000)
            {
                this.ToolStripMenuItem_10ksmp.Dispose();
            } if (ScopeDevice.maxBufferLength < 20000)
            {
                this.ToolStripMenuItem_20ksmp.Dispose();
            } if (ScopeDevice.maxBufferLength < 50000)
            {
                this.ToolStripMenuItem_50ksmp.Dispose();
            } if (ScopeDevice.maxBufferLength < 100000)
            {
                this.ToolStripMenuItem_100ksmp.Dispose();
            }
           
            this.radioButton_1k.Enabled = ScopeDevice.maxSamplingFrequency >= 1000 ? true : false;
            this.radioButton_2k.Enabled = ScopeDevice.maxSamplingFrequency >= 2000 ? true : false;
            this.radioButton_5k.Enabled = ScopeDevice.maxSamplingFrequency >= 5000 ? true : false;
            this.radioButton_10k.Enabled = ScopeDevice.maxSamplingFrequency >= 10000 ? true : false;
            this.radioButton_20k.Enabled = ScopeDevice.maxSamplingFrequency >= 20000 ? true : false;
            this.radioButton_50k.Enabled = ScopeDevice.maxSamplingFrequency >= 50000 ? true : false;
            this.radioButton_100k.Enabled = ScopeDevice.maxSamplingFrequency >= 100000 ? true : false;
            this.radioButton_200k.Enabled = ScopeDevice.maxSamplingFrequency >= 200000 ? true : false;
            this.radioButton_500k.Enabled = ScopeDevice.maxSamplingFrequency >= 500000 ? true : false;
            this.radioButton_1m.Enabled = ScopeDevice.maxSamplingFrequency >= 1000000 ? true : false;
            this.radioButton_2m.Enabled = ScopeDevice.maxSamplingFrequency >= 2000000 ? true : false;
            this.radioButton_5m.Enabled = ScopeDevice.maxSamplingFrequency >= 5000000 ? true : false;


            if (ScopeDevice.ranges[0, 0] != 0 || ScopeDevice.ranges[1, 0] != 0)
            {
                this.range0ToolStripMenuItem.Text = "Default (" + ScopeDevice.ranges[0, 0] + "mV, " + ScopeDevice.ranges[1, 0] + "mV)";
            }

            if (ScopeDevice.ranges[0, 1] != 0 || ScopeDevice.ranges[1, 1] != 0)
            {
                this.range1ToolStripMenuItem.Text = "Range 1 (" + ScopeDevice.ranges[0, 1] + "mV, " + ScopeDevice.ranges[1, 1] + "mV)";
            }
            else {
                this.range1ToolStripMenuItem.Dispose();
            }

            if (ScopeDevice.ranges[0, 2] != 0 || ScopeDevice.ranges[1, 2] != 0)
            {
                this.range2ToolStripMenuItem.Text = "Range 2 (" + ScopeDevice.ranges[0, 2] + "mV, " + ScopeDevice.ranges[1, 2] + "mV)";
            }
            else {
                this.range2ToolStripMenuItem.Dispose();
            }

            if (ScopeDevice.ranges[0, 3] != 0 || ScopeDevice.ranges[1, 3] != 0)
            {
                this.range3ToolStripMenuItem.Text = "Range 3 (" + ScopeDevice.ranges[0, 3] + "mV, " + ScopeDevice.ranges[1, 3] + "mV)";
            }
            else {
                this.range3ToolStripMenuItem.Dispose();
            }

            if (ScopeDevice.maxNumChannels < 1) {
                this.toolStripMenuItem_ch1_meas.Dispose();
            }
            if (ScopeDevice.maxNumChannels < 2)
            {
                this.toolStripMenuItem_ch2_meas.Dispose();
            }
            if (ScopeDevice.maxNumChannels < 3)
            {
                this.toolStripMenuItem_ch3_meas.Dispose();
            }
            if (ScopeDevice.maxNumChannels < 4)
            {
                this.toolStripMenuItem_ch4_meas.Dispose();
            }

        }

        public void validate_vertical_curr()
        {
            this.trackBar_ver_cur_a.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.trackBar_ver_cur_b.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_freq.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_time_a.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_time_b.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_ua.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_cur_ub.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_time_diff.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label_ver_cur_du.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label5.Enabled = VerticalCursorSrc == 0 ? false : true;
            this.label6.Enabled = VerticalCursorSrc == 0 ? false : true;
            if (VerticalCursorSrc == 0) {
                tA = 0;
                tB = 0;
            }
        }

        public void validate_horizontal_curr() {
            this.trackBar_hor_cur_a.Enabled = HorizontalCursorSrc == 0 ? false : true;
            this.trackBar_hor_cur_b.Enabled = HorizontalCursorSrc == 0 ? false : true;
            this.label_hor_cur_volt_diff.Enabled = HorizontalCursorSrc == 0 ? false : true;
            this.label_cur_u_a.Enabled = HorizontalCursorSrc == 0 ? false : true;
            this.label_cur_u_b.Enabled = HorizontalCursorSrc == 0 ? false : true;
            this.label8.Enabled = HorizontalCursorSrc == 0 ? false : true;
            this.label10.Enabled = HorizontalCursorSrc == 0 ? false : true;

            if (HorizontalCursorSrc == 0)
            {
                uA = 0;
                uB = 0;
            }
        }

        public void validate_meas_chann() {
            this.toolStripMenuItem_ch1_meas.Checked = measChann == 1 ? true : false;
            this.toolStripMenuItem_ch2_meas.Checked = measChann == 2 ? true : false;
            this.toolStripMenuItem_ch3_meas.Checked = measChann == 3 ? true : false;
            this.toolStripMenuItem_ch4_meas.Checked = measChann == 4 ? true : false;
        }



        //scope painting and data processing

        public void process_signals()
        {
            for (int i = 0; i < device.scopeCfg.actualChannels; i++)
            {
                double scale = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 / Math.Pow(2, device.scopeCfg.actualRes) * gain[i];
                double off = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 * (double)offset[i] / 1000 * gain[i] * 2 + (double)device.scopeCfg.ranges[0, selectedRange] / 1000 * gain[i];
                
                switch (i)
                {
                    case 0:
                        this.signal_ch1 = new double[device.scopeCfg.timeBase.Length];
                        for (int j = 0; j < device.scopeCfg.timeBase.Length; j++)
                        {
                            signal_ch1[j] = device.scopeCfg.samples[0, j]*scale+off;
                        }
                        break;
                    case 1:
                        this.signal_ch2 = new double[device.scopeCfg.timeBase.Length];
                        for (int j = 0; j < device.scopeCfg.timeBase.Length; j++)
                        {
                            signal_ch2[j] = device.scopeCfg.samples[1, j] * scale + off;
                        }
                        break;
                    case 2:
                        this.signal_ch3 = new double[device.scopeCfg.timeBase.Length];
                        for (int j = 0; j < device.scopeCfg.timeBase.Length; j++)
                        {
                            signal_ch3[j] = device.scopeCfg.samples[2, j] * scale + off;
                        }
                        break;
                    case 3:
                        this.signal_ch4 = new double[device.scopeCfg.timeBase.Length];
                        for (int j = 0; j < device.scopeCfg.timeBase.Length; j++)
                        {
                            signal_ch4[j] = device.scopeCfg.samples[3, j] * scale + off;
                        }
                        break;
                }
            }

            if (math != math_def.NONE && actualCahnnels>=2)
            {
                this.signal_math = new double[device.scopeCfg.timeBase.Length];
                double scale_ch1 = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 / Math.Pow(2, device.scopeCfg.actualRes);
                double scale_ch2 = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 / Math.Pow(2, device.scopeCfg.actualRes);
                double off = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 * (double)offset[4] / 1000 * gain[4] * 2 + (double)device.scopeCfg.ranges[0, selectedRange] / 1000 * gain[4];

                switch (math)
                {
                    case math_def.SUM:
                        for (int j = 0; j < device.scopeCfg.timeBase.Length; j++)
                        {
                            signal_math[j] = (device.scopeCfg.samples[0, j] * scale_ch1 + device.scopeCfg.samples[1, j] * scale_ch2 + off)*gain[4];
                        }
                        break;
                    case math_def.DIFF_A:
                        for (int j = 0; j < device.scopeCfg.timeBase.Length; j++)
                        {
                            signal_math[j] = (device.scopeCfg.samples[0, j] * scale_ch1 - (device.scopeCfg.samples[1, j] * scale_ch2) + off)*gain[4];
                        }
                        break;
                    case math_def.DIFF_B:
                        for (int j = 0; j < device.scopeCfg.timeBase.Length; j++)
                        {
                            signal_math[j] = (-(device.scopeCfg.samples[0, j] * scale_ch1) + device.scopeCfg.samples[1, j] * scale_ch2 + off)*gain[4];
                        }
                        break;
                    case math_def.MULT:
                        for (int j = 0; j < device.scopeCfg.timeBase.Length; j++)
                        {
                            signal_math[j] = ((device.scopeCfg.samples[0, j] * scale_ch1) * (device.scopeCfg.samples[1, j] * scale_ch2) + off)*gain[4];
                        }
                        break;
                }
            }
        }


        public void paint_signals()
        {
            LineItem curve;

            if (XYplot && device.scopeCfg.actualChannels >= 2)
            {
                curve = scopePane.AddCurve("", signal_ch2, signal_ch1, Color.Red, SymbolType.Diamond);
                curve.Line.IsSmooth = interpolation;
                curve.Line.SmoothTension = smoothing;
                curve.Line.IsAntiAlias = antialiasing;
                curve.Line.IsOptimizedDraw = true;
                curve.Symbol.Size = showPoints ? 5 : 0;
            }
            else
            {
                if (device.scopeCfg.actualChannels >= 1)
                {
                    curve = scopePane.AddCurve("", device.scopeCfg.timeBase, signal_ch1, Color.Red, SymbolType.Diamond);
                    curve.Line.IsSmooth = interpolation;
                    curve.Line.SmoothTension = smoothing;
                    curve.Line.IsAntiAlias = antialiasing;
                    curve.Line.IsOptimizedDraw = true;
                    curve.Symbol.Size = showPoints ? 5 : 0;
                }
                if (device.scopeCfg.actualChannels >= 2)
                {
                    curve = scopePane.AddCurve("", device.scopeCfg.timeBase, signal_ch2, Color.Blue, SymbolType.Diamond);
                    curve.Line.IsSmooth = interpolation;
                    curve.Line.SmoothTension = smoothing;
                    curve.Line.IsAntiAlias = antialiasing;
                    curve.Line.IsOptimizedDraw = true;
                    curve.Symbol.Size = showPoints ? 5 : 0;
                }
                if (device.scopeCfg.actualChannels >= 3)
                {
                    curve = scopePane.AddCurve("", device.scopeCfg.timeBase, signal_ch3, Color.DarkGreen, SymbolType.Diamond);
                    curve.Line.IsSmooth = interpolation;
                    curve.Line.SmoothTension = smoothing;
                    curve.Line.IsAntiAlias = antialiasing;
                    curve.Line.IsOptimizedDraw = true;
                    curve.Symbol.Size = showPoints ? 5 : 0;
                }
                if (device.scopeCfg.actualChannels >= 4)
                {
                    curve = scopePane.AddCurve("", device.scopeCfg.timeBase, signal_ch4, Color.Magenta, SymbolType.Diamond);
                    curve.Line.IsSmooth = interpolation;
                    curve.Line.SmoothTension = smoothing;
                    curve.Line.IsAntiAlias = antialiasing;
                    curve.Line.IsOptimizedDraw = true;
                    curve.Symbol.Size = showPoints ? 5 : 0;
                }
                if (math != math_def.NONE) {
                    curve = scopePane.AddCurve("", device.scopeCfg.timeBase, signal_math, Color.Purple, SymbolType.Diamond);
                    curve.Line.IsSmooth = interpolation;
                    curve.Line.SmoothTension = smoothing;
                    curve.Line.IsAntiAlias = antialiasing;
                    curve.Line.IsOptimizedDraw = true;
                    curve.Symbol.Size = showPoints ? 5 : 0;
                }

            }

        }

        public void paint_markers() {
            LineItem curve;


            //zoom position
            PointPairList list1 = new PointPairList();
            list1.Add((device.scopeCfg.maxTime) * horPosition, maxY);
            curve = scopePane.AddCurve("", list1, Color.Red, SymbolType.TriangleDown);
            curve.Symbol.Size = 15;
            curve.Symbol.Fill.Color = Color.Red;
            curve.Symbol.Fill.IsVisible = true;

            //trigger time
            list1 = new PointPairList();
            list1.Add(((device.scopeCfg.maxTime) * pretrigger - (device.scopeCfg.maxTime / device.scopeCfg.timeBase.Length)), maxY);
            curve = scopePane.AddCurve("", list1, Color.Blue, SymbolType.TriangleDown);
            curve.Symbol.Size = 20;
            curve.Symbol.Fill.Color = Color.Blue;
            curve.Symbol.Fill.IsVisible = true;

            //triggerlevel
            list1 = new PointPairList();
            double off = ((double)device.scopeCfg.ranges[1, selectedRange] - (double)device.scopeCfg.ranges[0, selectedRange]) / 1000 * (double)offset[triggerChannel - 1] / 1000 * gain[triggerChannel - 1] * 2 + (double)device.scopeCfg.ranges[0, selectedRange] / 1000 * gain[triggerChannel - 1];
                
            list1.Add(minX, triggerLevel * (maxY - minY)*gain[triggerChannel-1]+off);
            curve = scopePane.AddCurve("", list1, Color.Green, SymbolType.Diamond);
            curve.Symbol.Size = 15;
            curve.Symbol.Fill.Color = Color.Green;
            curve.Symbol.Fill.IsVisible = true;
        }

        public void paint_cursors() {

            if (VerticalCursorSrc != 0)
            {
                Color col;
                switch (VerticalCursorSrc) { 
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
                        col=Color.Black;
                        break;
                }
                LineItem curve;
                PointPairList list1 = new PointPairList();

                list1 = new PointPairList();
                list1.Add(tA, minY);
                list1.Add(tA, maxY);
                curve = scopePane.AddCurve("", list1, col, SymbolType.HDash);
                curve.Symbol.Size = 0;
                curve.Line.IsSmooth = true;
                

                list1 = new PointPairList();
                list1.Add(tB, minY);
                list1.Add(tB, maxY);
                curve = scopePane.AddCurve("", list1, col, SymbolType.HDash);
                curve.Symbol.Size = 0;
                curve.Line.IsSmooth = true;
            }

            if (HorizontalCursorSrc != 0)
            {
                Color col;
                switch (HorizontalCursorSrc)
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
                list1.Add(minX, uA);
                list1.Add(maxX, uA);
                curve = scopePane.AddCurve("", list1, col, SymbolType.HDash);
                curve.Symbol.Size = 0;
                curve.Line.IsSmooth = true;


                list1 = new PointPairList();
                list1.Add(minX, uB);
                list1.Add(maxX, uB);
                curve = scopePane.AddCurve("", list1, col, SymbolType.HDash);
                curve.Symbol.Size = 0;
                curve.Line.IsSmooth = true;
            }
        }

        public void show_buffer_err_message() {
            MessageBox.Show("Buffer usage error \r\nTry to decrease number of samples, data resolution or number of channels", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void toolStripMenuItem_ch1_meas_Click(object sender, EventArgs e)
        {
            measChann = 1;
            validate_meas_chann();
        }

        private void toolStripMenuItem_ch2_meas_Click(object sender, EventArgs e)
        {
            measChann = 2;
            validate_meas_chann();
        }

        private void toolStripMenuItem_ch3_meas_Click(object sender, EventArgs e)
        {
            measChann = 3;
            validate_meas_chann();
        }

        private void toolStripMenuItem_ch4_meas_Click(object sender, EventArgs e)
        {
            measChann = 4;
            validate_meas_chann();
        }

        private void frequencyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.FREQUENCY);
            measValid = !measValid;
        }

        private void periodToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.PERIOD);
            measValid = !measValid;
        }

        private void dutyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.DUTY);
            measValid = !measValid;
        }

        private void highToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.HIGH);
            measValid = !measValid;
        }

        private void lowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.LOW);
            measValid = !measValid;
        }

        private void rMSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.RMS);
            measValid = !measValid;
        }

        private void meanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.MEAN);
            measValid = !measValid;
        }

        private void peakPeakToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.PKPK);
            measValid = !measValid;
        }

        private void minToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.MIN);
            measValid = !measValid;
        }

        private void minToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            meas.addMeasurement(measChann - 1, Measurements.MeasurementTypes.MAX);
            measValid = !measValid;
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            meas.clearMeasurements();
            this.label_meas1.Text = "";
            this.label_meas2.Text = "";
            this.label_meas3.Text = "";
            this.label_meas4.Text = "";
            this.label_meas5.Text = "";
        }

        private void toolStripMenuItem_XY_plot_Click(object sender, EventArgs e)
        {
            if (this.toolStripMenuItem_XY_plot.Enabled)
            {
                this.toolStripMenuItem_XY_plot.Checked = true;
                this.toolStripMenuItem_XT_plot.Checked = false;
                XYplot = true;
            }
        }

        private void toolStripMenuItem_XT_plot_Click(object sender, EventArgs e)
        {
            this.toolStripMenuItem_XY_plot.Checked = false;
            this.toolStripMenuItem_XT_plot.Checked = true;
            XYplot = false;
        }

        private void ch1Ch2ToolStripMenuItem_plus_Click(object sender, EventArgs e)
        {
            if (ch1Ch2ToolStripMenuItem_plus.Checked) {
                math = math_def.SUM;
            }
            else
            {
                math = math_def.NONE;
            }
            validate_math(); 
        }

        private void ch1Ch2ToolStripMenuItem_minus_Click(object sender, EventArgs e)
        {
            if (ch1Ch2ToolStripMenuItem_minus.Checked)
            {
                math = math_def.DIFF_A;
            }
            else
            {
                math = math_def.NONE;
            }
            validate_math(); 
        }

        private void ch2Ch1ToolStripMenuItem_minus_Click(object sender, EventArgs e)
        {
            if (ch2Ch1ToolStripMenuItem_minus.Checked)
            {
                math = math_def.DIFF_B;
            }
            else
            {
                math = math_def.NONE;
            }
            validate_math(); 
        }

        private void ch1XCh2ToolStripMenuItem_mult_Click(object sender, EventArgs e)
        {
            if (ch1XCh2ToolStripMenuItem_mult.Checked)
            {
                math = math_def.MULT;
            }
            else {
                math = math_def.NONE;
            }
            validate_math(); 
        }

        private void validate_math() {
            ch1Ch2ToolStripMenuItem_minus.Checked = math == math_def.DIFF_A ? true : false;
            ch1Ch2ToolStripMenuItem_plus.Checked = math == math_def.SUM ? true : false;
            ch2Ch1ToolStripMenuItem_minus.Checked = math == math_def.DIFF_B ? true : false;
            ch1XCh2ToolStripMenuItem_mult.Checked = math == math_def.MULT ? true : false;

            radioButton_hor_cur_math.Enabled = math == math_def.NONE ? false : true;
            radioButton_ver_cur_math.Enabled = math == math_def.NONE ? false : true;
            radioButton_volt_math.Enabled = math == math_def.NONE ? false : true;
            if (math == math_def.NONE) {
                radioButton_volt_ch1.Checked = true;
                radioButton_ver_cur_off.Checked = true;
                radioButton_hor_cur_off.Checked = true;
            }
        }

        private void saveSignalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zedGraphControl_scope.SaveAsBitmap();
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            StreamWriter signalWriter;
            SaveFileDialog saveSignal = new SaveFileDialog();

                        // Set filter options and filter index.
            saveSignal.Filter = "CSV Files (.csv)|*.csv|Text Files (.txt)|*.txt|All Files (*.*)|*.*";
            saveSignal.FilterIndex=1;
            saveSignal.FileName = "signal_1";
 
            // Call the ShowDialog method to show the dialog box.
            bool done = false;
            while (!done)
            {
                DialogResult userClickedOK = saveSignal.ShowDialog();
                if (userClickedOK.Equals(DialogResult.OK))
                {
                    if (File.Exists(saveSignal.FileName))
                    {
                        try
                        {
                            File.Delete(saveSignal.FileName);
                            done = true;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Cannot overwrite selected file \r\n", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            saveSignal.FileName = "signal_1";
                            continue;
                        }

                    }
                    else
                    {
                        done = true;
                    }
                }
                else {
                    done = true;
                }
            }


            
                signalWriter = File.AppendText(saveSignal.FileName);
                string tmp;
                char separator=';';

                switch(actualCahnnels){
                    case 1:
                        signalWriter.WriteLine("time" + separator + "signal1");
                        break;
                    case 2:
                        signalWriter.WriteLine("time" + separator + "signal1" + separator + "signal2");
                        break;
                    case 3:
                        signalWriter.WriteLine("time" + separator + "signal1" + separator + "signal2" + separator + "signal3");
                        break;
                    case 4:
                        signalWriter.WriteLine("time" + separator + "signal1" + separator + "signal2" + separator + "signal3" + separator + "signal4");
                        break;
                }

                for(int i=0;i<signal_ch1.Length;i++){

                    tmp = device.scopeCfg.timeBase[i].ToString();

                    
                    if(actualCahnnels>=1){
                        tmp += separator + signal_ch1[i].ToString();
                    }
                    if(actualCahnnels>=2){
                        tmp += separator + signal_ch2[i].ToString();
                    }
                    if(actualCahnnels>=3){
                        tmp += separator + signal_ch3[i].ToString();
                    }
                    if(actualCahnnels>=4){
                        tmp += separator + signal_ch4[i].ToString();
                    }
                    signalWriter.WriteLine(tmp);
                }

                signalWriter.Close();
        }

        private void exitOscilloscopeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label_reset_Click(object sender, EventArgs e)
        {
            reset_volt_set();
        }


























    }
}
