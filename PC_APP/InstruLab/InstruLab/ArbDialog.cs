using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace InstruLab
{
    public partial class ArbDialog : Form
    {
        private char separator = ';';
        private double maxValue = 1;
        int vref;
        int adc_res;
        public ArbDialog(int vref, int adc_res)
        {
            InitializeComponent();
            this.radioButton_vref.Text = "Voltage (0.0 - " + Math.Round((double)(vref) / 1000, 1).ToString() + ")";
            this.radioButton_adcRes.Text = "ADC (0 - " + (Math.Pow(2,adc_res)-1).ToString() + ")";
            this.vref = vref;
            this.adc_res = adc_res;
        }

        private void radioButton_semi_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_semi.Checked) {
                separator = ';';
            }
        }

        private void radioButton_comma_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_comma.Checked)
            {
                separator = ',';
            }
        }

        private void radioButton_colon_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_colon.Checked)
            {
                separator = ':';
            }
        }

        private void radioButton_double_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_double.Checked)
            {
                maxValue = 1;
            }
        }

        private void radioButton_vref_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_vref.Checked)
            {
                maxValue = (double)(vref)/1000;
            }
        }

        private void radioButton_adcRes_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_adcRes.Checked)
            {
                maxValue = Math.Pow(2,adc_res)-1;
            }
        }

        public char GetSeparator() {
            return separator;
        }

        public double GetMaxValue() {
            return maxValue;
        }
    }
}
