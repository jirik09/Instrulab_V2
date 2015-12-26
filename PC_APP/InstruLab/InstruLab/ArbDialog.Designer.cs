namespace InstruLab
{
    partial class ArbDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button_OK = new System.Windows.Forms.Button();
            this.button_cancel = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_colon = new System.Windows.Forms.RadioButton();
            this.radioButton_comma = new System.Windows.Forms.RadioButton();
            this.radioButton_semi = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.radioButton_adcRes = new System.Windows.Forms.RadioButton();
            this.radioButton_vref = new System.Windows.Forms.RadioButton();
            this.radioButton_double = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_OK
            // 
            this.button_OK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button_OK.Location = new System.Drawing.Point(12, 135);
            this.button_OK.Name = "button_OK";
            this.button_OK.Size = new System.Drawing.Size(75, 25);
            this.button_OK.TabIndex = 0;
            this.button_OK.Text = "OK";
            this.button_OK.UseVisualStyleBackColor = true;
            // 
            // button_cancel
            // 
            this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button_cancel.Location = new System.Drawing.Point(93, 135);
            this.button_cancel.Name = "button_cancel";
            this.button_cancel.Size = new System.Drawing.Size(75, 25);
            this.button_cancel.TabIndex = 0;
            this.button_cancel.Text = "Cancel";
            this.button_cancel.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton_colon);
            this.groupBox1.Controls.Add(this.radioButton_comma);
            this.groupBox1.Controls.Add(this.radioButton_semi);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(127, 117);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Separator";
            // 
            // radioButton_colon
            // 
            this.radioButton_colon.AutoSize = true;
            this.radioButton_colon.Location = new System.Drawing.Point(6, 75);
            this.radioButton_colon.Name = "radioButton_colon";
            this.radioButton_colon.Size = new System.Drawing.Size(83, 21);
            this.radioButton_colon.TabIndex = 2;
            this.radioButton_colon.Text = "Colon (:)";
            this.radioButton_colon.UseVisualStyleBackColor = true;
            this.radioButton_colon.CheckedChanged += new System.EventHandler(this.radioButton_colon_CheckedChanged);
            // 
            // radioButton_comma
            // 
            this.radioButton_comma.AutoSize = true;
            this.radioButton_comma.Location = new System.Drawing.Point(6, 48);
            this.radioButton_comma.Name = "radioButton_comma";
            this.radioButton_comma.Size = new System.Drawing.Size(94, 21);
            this.radioButton_comma.TabIndex = 2;
            this.radioButton_comma.Text = "Comma (,)";
            this.radioButton_comma.UseVisualStyleBackColor = true;
            this.radioButton_comma.CheckedChanged += new System.EventHandler(this.radioButton_comma_CheckedChanged);
            // 
            // radioButton_semi
            // 
            this.radioButton_semi.AutoSize = true;
            this.radioButton_semi.Checked = true;
            this.radioButton_semi.Location = new System.Drawing.Point(6, 21);
            this.radioButton_semi.Name = "radioButton_semi";
            this.radioButton_semi.Size = new System.Drawing.Size(112, 21);
            this.radioButton_semi.TabIndex = 2;
            this.radioButton_semi.TabStop = true;
            this.radioButton_semi.Text = "Semicolon (;)";
            this.radioButton_semi.UseVisualStyleBackColor = true;
            this.radioButton_semi.CheckedChanged += new System.EventHandler(this.radioButton_semi_CheckedChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButton_adcRes);
            this.groupBox2.Controls.Add(this.radioButton_vref);
            this.groupBox2.Controls.Add(this.radioButton_double);
            this.groupBox2.Location = new System.Drawing.Point(145, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(157, 117);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Range";
            // 
            // radioButton_adcRes
            // 
            this.radioButton_adcRes.AutoSize = true;
            this.radioButton_adcRes.Location = new System.Drawing.Point(6, 75);
            this.radioButton_adcRes.Name = "radioButton_adcRes";
            this.radioButton_adcRes.Size = new System.Drawing.Size(57, 21);
            this.radioButton_adcRes.TabIndex = 2;
            this.radioButton_adcRes.Text = "ADC";
            this.radioButton_adcRes.UseVisualStyleBackColor = true;
            this.radioButton_adcRes.CheckedChanged += new System.EventHandler(this.radioButton_adcRes_CheckedChanged);
            // 
            // radioButton_vref
            // 
            this.radioButton_vref.AutoSize = true;
            this.radioButton_vref.Location = new System.Drawing.Point(6, 48);
            this.radioButton_vref.Name = "radioButton_vref";
            this.radioButton_vref.Size = new System.Drawing.Size(150, 21);
            this.radioButton_vref.TabIndex = 2;
            this.radioButton_vref.Text = "Voltage (0.0 - Vref)";
            this.radioButton_vref.UseVisualStyleBackColor = true;
            this.radioButton_vref.CheckedChanged += new System.EventHandler(this.radioButton_vref_CheckedChanged);
            // 
            // radioButton_double
            // 
            this.radioButton_double.AutoSize = true;
            this.radioButton_double.Checked = true;
            this.radioButton_double.Location = new System.Drawing.Point(6, 21);
            this.radioButton_double.Name = "radioButton_double";
            this.radioButton_double.Size = new System.Drawing.Size(141, 21);
            this.radioButton_double.TabIndex = 2;
            this.radioButton_double.TabStop = true;
            this.radioButton_double.Text = "Double (0.0 - 1.0)";
            this.radioButton_double.UseVisualStyleBackColor = true;
            this.radioButton_double.CheckedChanged += new System.EventHandler(this.radioButton_double_CheckedChanged);
            // 
            // ArbDialog
            // 
            this.AcceptButton = this.button_OK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button_cancel;
            this.ClientSize = new System.Drawing.Size(309, 168);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_cancel);
            this.Controls.Add(this.button_OK);
            this.MaximumSize = new System.Drawing.Size(327, 213);
            this.MinimumSize = new System.Drawing.Size(327, 213);
            this.Name = "ArbDialog";
            this.Text = "Arbitary signal Dialog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_OK;
        private System.Windows.Forms.Button button_cancel;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_colon;
        private System.Windows.Forms.RadioButton radioButton_comma;
        private System.Windows.Forms.RadioButton radioButton_semi;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioButton_adcRes;
        private System.Windows.Forms.RadioButton radioButton_vref;
        private System.Windows.Forms.RadioButton radioButton_double;
    }
}