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

        int rangeMin=0;
        int rangeMax=1;
        int channels=1;

        public Voltmeter(Device dev)
        {
            device = dev;
            
            InitializeComponent();



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
          /*  int tmp=0;
            tmp = 0x00000008;
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_ADC_CHANNEL + " ");
            device.send_int((int)(tmp));
            device.send(";");

            tmp = 0x00000109;
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_ADC_CHANNEL + " ");
            device.send_int((int)(tmp));
            device.send(";");

            tmp = 0x00000203;
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_ADC_CHANNEL + " ");
            device.send_int((int)(tmp));
            device.send(";");

            tmp = 0x00000303;
            device.send(Commands.SCOPE + ":" + Commands.SCOPE_ADC_CHANNEL + " ");
            device.send_int((int)(tmp));
            device.send(";");*/

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

                        channels = device.scopeCfg.actualChannels;
                        
                        this.Invalidate();

                        Thread.Sleep(100);
                        device.takeCommsSemaphore(semaphoreTimeout * 2 + 108);
                        device.send(Commands.SCOPE + ":" + Commands.SCOPE_NEXT + ";");
                        device.giveCommsSemaphore();

                        break;
                }
            }
        }


        protected override void OnPaint(PaintEventArgs e)
        {
            this.groupBox_1.Enabled = channels >= 1 ? true : false;
            this.groupBox_2.Enabled = channels >= 2 ? true : false;
            this.groupBox_3.Enabled = channels >= 3 ? true : false;
            this.groupBox_4.Enabled = channels >= 4 ? true : false;

            this.label_volt_1.Text = Math.Round(meas.getMean(0) * 1000, 2) + " mV";
            this.label_volt_2.Text = Math.Round(meas.getMean(1) * 1000, 2) + " mV";
            this.label_volt_3.Text = Math.Round(meas.getMean(2) * 1000, 2) + " mV";
            this.label_volt_4.Text = Math.Round(meas.getMean(3) * 1000, 2) + " mV";

            this.progressBar_volt_1.Value = channels >= 1 ? (int)((meas.getMean(0) * 1000 - rangeMin) / ((double)rangeMax - rangeMin) * 100) : 0;
            this.progressBar_volt_2.Value = channels >= 2 ? (int)((meas.getMean(1) * 1000 - rangeMin) / ((double)rangeMax - rangeMin) * 100) : 0;
            this.progressBar_volt_3.Value = channels >= 3 ? (int)((meas.getMean(2) * 1000 - rangeMin) / ((double)rangeMax - rangeMin) * 100) : 0;
            this.progressBar_volt_4.Value = channels >= 4 ? (int)((meas.getMean(3) * 1000 - rangeMin) / ((double)rangeMax - rangeMin) * 100) : 0;

            this.label_ripp_1.Text = channels >= 1 ? "ripple: " + Math.Round(meas.getPkPk(0, rangeMax, rangeMin, device.scopeCfg.actualRes) * 1000, 2) + " mV pkpk" : "";
            this.label_ripp_2.Text = channels >= 2 ? "ripple: " + Math.Round(meas.getPkPk(1, rangeMax, rangeMin, device.scopeCfg.actualRes) * 1000, 2) + " mV pkpk" : "";
            this.label_ripp_3.Text = channels >= 3 ? "ripple: " + Math.Round(meas.getPkPk(2, rangeMax, rangeMin, device.scopeCfg.actualRes) * 1000, 2) + " mV pkpk" : "";
            this.label_ripp_4.Text = channels >= 4 ? "ripple: " + Math.Round(meas.getPkPk(3, rangeMax, rangeMin, device.scopeCfg.actualRes) * 1000, 2) + " mV pkpk" : "";

            this.label_freq_1.Text = channels >= 1 ? meas.getMeas(0 * 3) : "";
            this.label_freq_2.Text = channels >= 2 ? meas.getMeas(1 * 3) : "";
            this.label_freq_3.Text = channels >= 3 ? meas.getMeas(2 * 3) : "";
            this.label_freq_4.Text = channels >= 4 ? meas.getMeas(3 * 3) : "";

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
