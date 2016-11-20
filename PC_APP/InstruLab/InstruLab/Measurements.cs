using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace LEO
{
    class Measurements
    {

        public enum MeasurementTypes { FREQUENCY, PERIOD, DUTY, LOW, HIGH, RMS, MEAN, PKPK, MAX, MIN }
        private int maxMeasCount;

        public class Measurement
        {
            public Measurement(MeasurementTypes meas, int chan)
            {
                this.measType = meas;
                this.measChann = chan;
            }
            public MeasurementTypes measType;
            public int measChann;

        }
        public string[] measStrings;
        public Color[] measCol;


        bool[] calcVolt = new bool[4] { false,  false, false, false };
        double[] RMS = new double[4] { 0, 0, 0, 0 };
        
        double[] Mean = new double[4] { 0, 0, 0, 0 };
        ushort[] Max = new ushort[4] { ushort.MinValue, ushort.MinValue, ushort.MinValue, ushort.MinValue };
        ushort[] Min = new ushort[4] { ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue };

        bool[] calcTime = new bool[4] { false, false, false, false };
        double[] Period = new double[4] { 0, 0, 0, 0};
        double[] Freq = new double[4] { 0, 0, 0, 0 };
        double[] High = new double[4] { 0, 0, 0, 0 };

        List<Measurement> measurements = new List<Measurement>();


        public Measurements(int count) {
            measStrings = new string[count];
            measCol = new Color[count];
            maxMeasCount = count;
        }




        public void addMeasurement(int chann, MeasurementTypes type)
        {
            measurements.Add(new Measurement(type, chann));
            if (measurements.Count > maxMeasCount)
            {
                measurements.RemoveRange(0, 1);
            }
        }

        public void clearMeasurements()
        {
            measurements.Clear();
        }

        public string getMeas(int index) {
            return this.measStrings[index];
        }
        public Color getColor(int index) {
            return measCol[index];
        }

        public int getMeasCount() {
            return measurements.Count;
        }

        public double getMean(int chann)
        {
            return this.Mean[chann];
        }

        public double getFreq(int chann)
        {
            return this.Freq[chann];
        }
        public double getPkPk(int chann, int rangeMax, int rangeMin, int res)
        {
            return (Max[chann] - Min[chann]) * ((double)rangeMax - (double)rangeMin) / 1000 / Math.Pow(2, res);
        }

        public void setColor(int chann, int index) {
            switch (chann) { 
                case 0:
                    measCol[index] = Color.Red;
                    break;
                case 1:
                    measCol[index] = Color.Blue;
                    break;
                case 2:
                    measCol[index] = Color.DarkGreen;
                    break;
                case 3:
                    measCol[index] = Color.Magenta;
                    break;
            }        
        }

        public void calculate_time(ushort[,] samples, int samplingFreq, int buffleng, int ch)
        {
            if (!calcTime[ch])
            {
                double center = (Max[ch] + Min[ch]) / 2;
                //double center = Mean[ch] / scale;

                int state = 0;
                int up = 0;
                int down = 0;
                bool below = false;
                int periods = 0;
                double frq = 0;
                double LP = samples[ch, 0];
                int totalUp = 0;
                int totalDown = 0;
                if (samples[ch, 0] < center) {
                    below = true;
                }

                for (int i = 3; i < buffleng-4; i++)
                {
                    //LP = (samples[ch, i] + samples[ch, i + 1] + samples[ch, i - 1] + samples[ch, i + 2] + samples[ch, i - 2] + samples[ch, i + 3] + samples[ch, i - 3] + samples[ch, i +4]) / 8;
                    LP = 0.9 * LP + 0.1 * samples[ch, i];
                    if (state == 0)
                    {
                        if ((below && LP >= center))
                        {
                            state = 1; ;
                        }
                        else if ((!below && LP < center))
                        {
                            state = 2;
                        }
                    }
                    else if (state == 1)
                    { //signal was below and now is above
                        if (LP < center)
                        {
                            state = 3;
                        }
                        up++;
                    }
                    else if (state == 2)
                    { //signal was above and now is below
                        if (LP > center)
                        {
                            state = 4;
                        }
                        down++;
                    }
                    else if (state == 3)
                    { //signal was above and now is below
                        if (LP > center)
                        {
                            state = 5;
                        }
                        down++;
                    }
                    else if (state == 4)
                    { //signal was below and now is above
                        if (LP < center)
                        {
                            state = 6;
                        }
                        up++;
                    }
                    else if (state >= 5)
                    {
                        if (up > 3 && down > 3)
                        {
                            periods++;
                            frq += (double)samplingFreq / (up + down);
                        }
                        state -= 4;

                        totalUp += up;
                        totalDown += down;
                        up = 0;
                        down = 0;
                    }
                }

                if (periods >= 1)
                {
                    Freq[ch] = frq / periods;
                    Period[ch] = 1 / Freq[ch];
                    High[ch] = Math.Round((double)totalUp / (totalUp + totalDown), 3);
                }
                else
                {
                    Freq[ch] = Double.PositiveInfinity;
                    Period[ch] = Double.PositiveInfinity;
                    High[ch] = Math.Round((double)totalUp / (totalUp + totalDown), 3);
                }
                calcTime[ch] = true;
            }
        }


        public void calculate_time2(ushort[,] samples, int samplingFreq, int buffleng, int ch)
        {
            if (!calcTime[ch])
            {
                double center = (Max[ch] + Min[ch]) / 2;
                int up = 0;
                int down = 0;
                int periods = 0;
                bool countEN = false;
                bool rise = false;
                double frq = 0;
                int totalhigh = 0;
                int totallow = 0;

                if (samples[ch, 0] < center)
                {
                    rise = true;
                }
                for (int i = 0; i < buffleng; i++)
                {
                    if ((rise && !countEN && samples[ch, i] >= center) || (!rise && !countEN && samples[ch, i] < center)) //nabezna || sestupna hrana
                    {
                        countEN = true;
                    }
                    if ((rise && countEN && samples[ch, i] >= center && down > 0) || (!rise && countEN && samples[ch, i] < center && up > 0))
                    {
                        countEN = false;
                        totallow += down;
                        totalhigh += up;
                        if (up > 7 && down > 7)
                        {
                            periods++;
                            if (frq == 0)
                            {
                                frq = (double)samplingFreq / (up + down);
                            }
                            else
                            {
                                frq += (double)samplingFreq / (up + down);
                            }
                        }
                        up = 0;
                        down = 0;
                    }

                    if (countEN)
                    {
                        if (samples[ch, i] >= center)
                        {
                            up++;
                        }
                        else
                        {
                            down++;
                        }
                    }
                }

                if (periods >= 2)
                {
                    Freq[ch] = frq / periods;
                    Period[ch] = 1 / Freq[ch];
                    High[ch]=Math.Round((double)totalhigh / (totalhigh + totallow),3);
                }
                else
                {
                    Freq[ch] = Double.PositiveInfinity;
                    Period[ch] = Double.PositiveInfinity; 
                    High[ch]=Math.Round((double)totalhigh / (totalhigh + totallow),3);
                }
                calcTime[ch] = true;
            }
        }

        public void calculateMeasurements(ushort[,] samples, int rangeMax, int rangeMin, int numChann, int samplingFreq, int buffleng,int res)
        {
            calcTime = new bool[4] { false, false, false, false };
            calcVolt = new bool[4] { false, false, false, false};
            RMS = new double[4] { 0, 0, 0, 0 };

            Mean = new double[4] { 0, 0, 0, 0 };
            Max = new ushort[4] { ushort.MinValue, ushort.MinValue, ushort.MinValue, ushort.MinValue };
            Min = new ushort[4] { ushort.MaxValue, ushort.MaxValue, ushort.MaxValue, ushort.MaxValue };

            Period = new double[4] { 0, 0, 0, 0 };
            Freq = new double[4] { 0, 0, 0, 0 };
            High = new double[4] { 0, 0, 0, 0 };

            int meascount = 0;
            foreach (var item in measurements)
            {
                for (int ch = 0; ch < numChann; ch++)
                {
                    if (item.measChann == ch)
                    {
                        if (!calcVolt[ch])
                        {
                            for (int i = 0; i < buffleng; i++)
                            {
                                RMS[ch] += Math.Pow(samples[ch, i] * ((double)rangeMax - (double)rangeMin) / 1000 / (Math.Pow(2, res) - 1) + (double)rangeMin / 1000, 2) ;
                                Mean[ch] += samples[ch, i] * ((double)rangeMax - (double)rangeMin) / 1000 / (Math.Pow(2, res) - 1) + (double)rangeMin / 1000;
                                if (samples[ch, i] > Max[ch])
                                {
                                    Max[ch] = samples[ch, i];
                                }
                                if (samples[ch, i] < Min[ch])
                                {
                                    Min[ch] = samples[ch, i];
                                }
                            }
                            RMS[ch] = Math.Sqrt(RMS[ch] / buffleng);
                            Mean[ch] = (Mean[ch] / buffleng);
                            calcVolt[ch] = true;
                        }



                        switch (item.measType)
                        {
                            case MeasurementTypes.FREQUENCY:
                                calculate_time(samples, samplingFreq, buffleng, ch);
                                double freq = Freq[ch];
                                if (Double.IsPositiveInfinity(freq)) 
                                {
                                    measStrings[meascount] = "f: N/A";
                                }
                                else if (freq > 1000000)
                                {
                                    measStrings[meascount] = "f: " + Math.Round(freq / 1000000, 3) + "MHz";
                                }
                                else if (freq > 1000)
                                {
                                    measStrings[meascount] = "f: " + Math.Round(freq / 1000, 3) + "kHz";
                                }
                                else
                                {
                                    measStrings[meascount] = "f: " + Math.Round(freq, 3) + "Hz";
                                }
                                setColor(ch, meascount);
                                meascount++;
                                break;
                            case MeasurementTypes.PERIOD:
                                calculate_time(samples, samplingFreq, buffleng, ch);
                                double per = Period[ch];
                                if (Double.IsPositiveInfinity(per))
                                {
                                    measStrings[meascount] = "Per: N/A";
                                }
                                else if (per > 1)
                                {
                                    measStrings[meascount] = "Per: " + Math.Round(per, 3) + "s";
                                }
                                else if (per > 0.001)
                                {
                                    measStrings[meascount] = "Per: " + Math.Round(per *1000, 3) + "ms";
                                }
                                else
                                {
                                    measStrings[meascount] = "Per: " + Math.Round(per*1000000, 3) + "us";
                                }
                                setColor(ch, meascount);
                                meascount++;
                                break;
                            case MeasurementTypes.DUTY:
                                calculate_time(samples, samplingFreq, buffleng, ch);
                                if(double.IsNaN(High[ch])){
                                    measStrings[meascount] = "Duty: N/A";
                                }else{
                                    measStrings[meascount] = "Duty: " + Math.Round(High[ch]*100, 3) + "%";
                                }
                                setColor(ch, meascount);
                                meascount++;
                                break;
                            case MeasurementTypes.LOW:
                                calculate_time(samples, samplingFreq, buffleng, ch);
                                if (double.IsNaN(High[ch]))
                                {
                                    measStrings[meascount] = "Low: N/A";
                                }
                                else
                                {
                                    measStrings[meascount] = "Low: " + Math.Round((1-High[ch]) * 100, 3) + "%";
                                }
                                setColor(ch, meascount);
                                meascount++;
                                break;
                            case MeasurementTypes.HIGH:
                                calculate_time(samples, samplingFreq, buffleng, ch);
                                if(double.IsNaN(High[ch])){
                                    measStrings[meascount] = "High: N/A";
                                }else{
                                    measStrings[meascount] = "High: " + Math.Round(High[ch]*100, 3) + "%";
                                }setColor(ch, meascount);
                                meascount++;
                                break;

                            case MeasurementTypes.RMS:
                                measStrings[meascount] = "RMS: " + Math.Round(RMS[ch], 3) + "V";
                                setColor(ch, meascount);
                                meascount++;
                                break;
                            case MeasurementTypes.MEAN:
                                measStrings[meascount] = "Mean: " + Math.Round(Mean[ch], 3) + "V";
                                setColor(ch, meascount);
                                meascount++;
                                break;
                            case MeasurementTypes.MAX:
                                double Vmax = Max[ch] * ((double)rangeMax - (double)rangeMin) / 1000 / Math.Pow(2, res) + (double)rangeMin / 1000;
                                measStrings[meascount] = "Max: " + Math.Round(Vmax, 3) + "V";
                                setColor(ch, meascount);
                                meascount++;
                                break;
                            case MeasurementTypes.MIN:
                                double Vmin = Min[ch] * ((double)rangeMax - (double)rangeMin) / 1000 / Math.Pow(2, res) + (double)rangeMin / 1000;
                                measStrings[meascount] = "Min: " + Math.Round(Vmin, 3) + "V";
                                setColor(ch, meascount);
                                meascount++;
                                break;
                            case MeasurementTypes.PKPK:
                                double Vpkpk = (Max[ch] - Min[ch]) * ((double)rangeMax - (double)rangeMin) / 1000 / Math.Pow(2, res);
                                measStrings[meascount] = "PkPk: " + Math.Round(Vpkpk, 3) + "V";
                                setColor(ch, meascount);
                                meascount++;
                                break;
                        }
                    }
                }
            }
        }
    }
}
