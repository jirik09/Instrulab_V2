using InstruLab;
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
    public partial class Voltmeter : Form
    {
        private Device device;
        int semaphoreTimeout = 3000;

        System.Timers.Timer GUITimer;

        private Queue<InstruLab.Message> volt_q = new Queue<InstruLab.Message>();
        InstruLab.Message messg;

        Measurements meas = new Measurements(12);
        Thread calcSignal_th;

        public GraphPane VoltPane;

        int rangeMin;
        int rangeMax;



        public Voltmeter(Device dev)
        {
            device = dev;
            
            InitializeComponent();
            zedGraphControl_volt.MasterPane[0].IsFontsScaled = false;
            zedGraphControl_volt.MasterPane[0].Title.IsVisible = false;
            zedGraphControl_volt.MasterPane[0].XAxis.MajorGrid.IsVisible = true;
            zedGraphControl_volt.MasterPane[0].XAxis.Title.IsVisible = false;

            zedGraphControl_volt.IsEnableZoom = false;

            zedGraphControl_volt.MasterPane[0].YAxis.MajorGrid.IsVisible = true;
            zedGraphControl_volt.MasterPane[0].YAxis.Title.IsVisible = false;
            VoltPane = zedGraphControl_volt.GraphPane;



            /*
            // Set the Titles
            myPane.Title.Text = "My Test Bar Graph";
            myPane.XAxis.Title.Text = "Label";
            myPane.YAxis.Title.Text = "My Y Axis";

            // Make up some random data points
            string[] labels = { "Panther", "Lion", "Cheetah", 
                      "Cougar", "Tiger", "Leopard" };
            double[] y = { 100, 115, 75, 22, 98, 40 };
            double[] y2 = { 90, 100, 95, 35, 80, 35 };
            double[] y3 = { 80, 110, 65, 15, 54, 67 };
            double[] y4 = { 120, 125, 100, 40, 105, 75 };
             * 
             * 
             * GraphPane.BarSettings.Type

            // Generate a red bar with "Curve 1" in the legend
            BarItem myBar = myPane.AddBar("Curve 1", null, y,
                                                        Color.Red);
            myBar.Bar.Fill = new Fill(Color.Red, Color.White,
                                                        Color.Red);

            // Generate a blue bar with "Curve 2" in the legend
            myBar = myPane.AddBar("Curve 2", null, y2, Color.Blue);
            myBar.Bar.Fill = new Fill(Color.Blue, Color.White,
                                                        Color.Blue);

            // Generate a green bar with "Curve 3" in the legend
            myBar = myPane.AddBar("Curve 3", null, y3, Color.Green);
            myBar.Bar.Fill = new Fill(Color.Green, Color.White,
                                                        Color.Green);

            // Generate a black line with "Curve 4" in the legend
            LineItem myCurve = myPane.AddCurve("Curve 4",
                  null, y4, Color.Black, SymbolType.Circle);
            myCurve.Line.Fill = new Fill(Color.White,
                                  Color.LightSkyBlue, -45F);

            // Fix up the curve attributes a little
            myCurve.Symbol.Size = 8.0F;
            myCurve.Symbol.Fill = new Fill(Color.White);
            myCurve.Line.Width = 2.0F;

            // Draw the X tics between the labels instead of 
            // at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;

            // Set the XAxis labels
            myPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Fill the Axis and Pane backgrounds
            myPane.Chart.Fill = new Fill(Color.White,
                  Color.FromArgb(255, 255, 166), 90F);
            myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));

            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zedGraphControl_volt.AxisChange();
            */


            rangeMin = 0;
            rangeMax = device.scopeCfg.VRef;
            this.toolStripTextBox_max.Text = rangeMax.ToString();

            GUITimer = new System.Timers.Timer(5);
            GUITimer.Elapsed += new ElapsedEventHandler(Update_GUI);
            GUITimer.Start();

            meas.clearMeasurements();
            meas.addMeasurement(0, Measurements.MeasurementTypes.FREQUENCY);
            meas.addMeasurement(0, Measurements.MeasurementTypes.MEAN);
            meas.addMeasurement(0, Measurements.MeasurementTypes.PKPK);
            meas.addMeasurement(1, Measurements.MeasurementTypes.FREQUENCY);
            meas.addMeasurement(1, Measurements.MeasurementTypes.MEAN);
            meas.addMeasurement(1, Measurements.MeasurementTypes.PKPK);
            meas.addMeasurement(2, Measurements.MeasurementTypes.FREQUENCY);
            meas.addMeasurement(2, Measurements.MeasurementTypes.MEAN);
            meas.addMeasurement(2, Measurements.MeasurementTypes.PKPK);
            meas.addMeasurement(3, Measurements.MeasurementTypes.FREQUENCY);
            meas.addMeasurement(3, Measurements.MeasurementTypes.MEAN);
            meas.addMeasurement(3, Measurements.MeasurementTypes.PKPK);
            



            device.takeCommsSemaphore(semaphoreTimeout + 109);

            device.send(Commands.SCOPE + ":" + Commands.START + ";");

            device.send(Commands.SCOPE + ":" + Commands.SAMPLING_FREQ + " " + Commands.FREQ_5K + ";");
            device.send(Commands.SCOPE + ":" + Commands.DATA_LENGTH + " " + Commands.SAMPLES_200 + ";");
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_TRIG_MODE + " " + Commands.MODE_AUTO + ";");
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_DATA_DEPTH + " " + Commands.DATA_DEPTH_12B + ";");

            switch (device.scopeCfg.maxNumChannels)
            {
                case 1:
                    device.send(Commands.SCOPE + ":" + Commands.CHANNELS + " " + Commands.CHANNELS_1 + ";");
                    break;
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
            device.send(Commands.SCOPE + ":" + Commands.START + ";");

            device.giveCommsSemaphore();
        }


        private void Update_GUI(object sender, ElapsedEventArgs e)
        {
            if (volt_q.Count > 0)
            {
                messg = volt_q.Dequeue();
                if (messg == null)
                {
                    return;
                }
                switch (messg.GetRequest())
                {
                    case InstruLab.Message.MsgRequest.VOLT_NEW_DATA:
                        if (calcSignal_th != null && calcSignal_th.IsAlive)
                        {
                            calcSignal_th.Join();
                        }
                        calcSignal_th = new Thread(() => meas.calculateMeasurements(device.scopeCfg.samples, rangeMax, rangeMin, device.scopeCfg.actualChannels, device.scopeCfg.realSmplFreq, device.scopeCfg.timeBase.Length, device.scopeCfg.actualRes));
                        calcSignal_th.Start();
                        Thread.Sleep(1);
                        if (calcSignal_th.IsAlive)
                        {
                            calcSignal_th.Join();
                        }


                        VoltPane.CurveList.Clear();
                        //process_signals();
                        paint_voltages();

                        this.Invalidate();


                        Thread.Sleep(100);
                        device.takeCommsSemaphore(semaphoreTimeout * 2 + 108);
                        device.send(Commands.SCOPE + ":" + Commands.SCOPE_NEXT + ";");
                        device.giveCommsSemaphore();

                        break;
                }
            }
        }

        public void paint_voltages() {
            string[] labels = { "Ch 1", "Ch 2", "Ch 3", "Ch 4"};

            double[] ch1 = { meas.getMean(0), 0, 0, 0 };
            double[] ch2 = { 0, meas.getMean(1), 0, 0 };
            double[] ch3 = { 0, 0, meas.getMean(2), 0 };
            double[] ch4 = { 0, 0, 0, meas.getMean(3) };
            
            // Generate a red bar with "Curve 1" in the legend
            BarItem myBar = VoltPane.AddBar("Curve 1", null, ch1, Color.Red);
            myBar.Bar.Fill = new Fill(Color.DarkRed, Color.Red, Color.DarkRed);

            // Generate a blue bar with "Curve 2" in the legend
            myBar = VoltPane.AddBar("Curve 2", null, ch2, Color.Blue);
            myBar.Bar.Fill = new Fill(Color.DarkBlue, Color.Blue,Color.DarkBlue);

            // Generate a green bar with "Curve 3" in the legend
            myBar = VoltPane.AddBar("Curve 3", null, ch3, Color.DarkGreen);
            myBar.Bar.Fill = new Fill(Color.DarkGreen, Color.LightGreen, Color.DarkGreen);
            // Generate a green bar with "Curve 3" in the legend
            myBar = VoltPane.AddBar("Curve 4", null, ch4, Color.Magenta);
            myBar.Bar.Fill = new Fill(Color.Purple, Color.Magenta, Color.Purple);

            VoltPane.GraphObjList.Clear();
            for (int i = 0; i < 4; i++)
            {
                double pkpk = meas.getPkPk(i, rangeMax, rangeMin, device.scopeCfg.actualRes);
                // format the label string to have 1 decimal place
                // create the text item (assumes the x axis is ordinal or text)
                // for negative bars, the label appears just above the zero value
                //TextObj text = new TextObj(lab, (float)(i + 1), (float)(ch4[i] < 0 ? 0.0 : ch4[i]) + shift);
                TextObj text;
                if (pkpk > 0.05 * meas.getMean(i))
                {
                    text = new TextObj("(" + Math.Round(pkpk * 1000, 2) + "mV pkpk)\r\n(" + meas.getMeas(i * 3) + ")\r\n"+Math.Round(meas.getMean(i) * 1000, 2) + "mV", i + 1, meas.getMean(i) + (double)rangeMax / 8000);
                }
                else {
                    text = new TextObj(Math.Round(meas.getMean(i)*1000,2)+"mV", i + 1, meas.getMean(i) + (double)rangeMax / 30000);
                }
                // tell Zedgraph to use user scale units for locating the TextItem
                text.Location.CoordinateFrame = CoordType.AxisXYScale;
                // AlignH the left-center of the text to the specified point
                text.Location.AlignH = AlignH.Center;
                text.Location.AlignV = AlignV.Center;
                text.FontSpec.Border.IsVisible = false;
                text.FontSpec.Fill.IsVisible = false;
                text.FontSpec.Size = 17;
                text.FontSpec.FontColor = meas.getColor(i * 3);
                // rotate the text 90 degrees
                text.FontSpec.Angle = 0;
                // add the TextItem to the list
                VoltPane.GraphObjList.Add(text);
                
            }
            VoltPane.Chart.Fill = new Fill(Color.White, Color.White, 45.0F);

            VoltPane.BarSettings.Type=BarType.Overlay;
            VoltPane.Legend.IsVisible = false;


            VoltPane.YAxis.Scale.MaxAuto = false;
            VoltPane.YAxis.Scale.MinAuto = false;

            VoltPane.YAxis.Scale.Max = (double)rangeMax/1000*1.1;
            VoltPane.YAxis.Scale.Min = rangeMin;

            VoltPane.XAxis.Scale.MaxAuto = false;
            VoltPane.XAxis.Scale.MinAuto = false;

            VoltPane.XAxis.Scale.Min=0.5;
            VoltPane.XAxis.Scale.Max = device.scopeCfg.maxNumChannels+0.5;

            // Set the XAxis labels
            VoltPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            VoltPane.XAxis.Type = AxisType.Text;


            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zedGraphControl_volt.AxisChange();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            zedGraphControl_volt.Refresh();
            base.OnPaint(e);
        }

        public void add_message(InstruLab.Message msg)
        {
            this.volt_q.Enqueue(msg);
        }

        private void Voltmeter_FormClosing(object sender, FormClosingEventArgs e)
        {
            device.takeCommsSemaphore(semaphoreTimeout + 110);
            device.send(Commands.SCOPE + ":" + Commands.STOP + ";");
            device.giveCommsSemaphore();
            device.voltClosed();
        }

        private void toolStripTextBox_min_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.toolStripTextBox_min.Text))
                {
                    int val = int.Parse(this.toolStripTextBox_min.Text);
                    rangeMin = val;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void toolStripTextBox_max_TextChanged(object sender, EventArgs e)
        {
            try
            {
                if (!String.IsNullOrEmpty(this.toolStripTextBox_max.Text))
                {
                    int val = int.Parse(this.toolStripTextBox_max.Text);
                    rangeMax = val;
                }
            }
            catch (Exception ex)
            {

            }

        }
    }
}
