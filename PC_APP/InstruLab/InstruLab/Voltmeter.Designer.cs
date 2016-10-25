namespace LEO
{
    partial class Voltmeter
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox_min = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.maximumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox_max = new System.Windows.Forms.ToolStripTextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox_4 = new System.Windows.Forms.GroupBox();
            this.progressBar_volt_4 = new System.Windows.Forms.ProgressBar();
            this.label_ripp_4 = new System.Windows.Forms.Label();
            this.label_freq_4 = new System.Windows.Forms.Label();
            this.label_volt_4 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox_1 = new System.Windows.Forms.GroupBox();
            this.progressBar_volt_1 = new System.Windows.Forms.ProgressBar();
            this.label_ripp_1 = new System.Windows.Forms.Label();
            this.label_freq_1 = new System.Windows.Forms.Label();
            this.label_volt_1 = new System.Windows.Forms.Label();
            this.groupBox_2 = new System.Windows.Forms.GroupBox();
            this.progressBar_volt_2 = new System.Windows.Forms.ProgressBar();
            this.label_ripp_2 = new System.Windows.Forms.Label();
            this.label_freq_2 = new System.Windows.Forms.Label();
            this.label_volt_2 = new System.Windows.Forms.Label();
            this.groupBox_3 = new System.Windows.Forms.GroupBox();
            this.progressBar_volt_3 = new System.Windows.Forms.ProgressBar();
            this.label_ripp_3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label_freq_3 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label_volt_3 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox_4.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox_1.SuspendLayout();
            this.groupBox_2.SuspendLayout();
            this.groupBox_3.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.rangeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(324, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // rangeToolStripMenuItem
            // 
            this.rangeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.minimumToolStripMenuItem,
            this.toolStripTextBox_min,
            this.toolStripSeparator1,
            this.maximumToolStripMenuItem,
            this.toolStripTextBox_max});
            this.rangeToolStripMenuItem.Name = "rangeToolStripMenuItem";
            this.rangeToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.rangeToolStripMenuItem.Text = "Range";
            // 
            // minimumToolStripMenuItem
            // 
            this.minimumToolStripMenuItem.Name = "minimumToolStripMenuItem";
            this.minimumToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.minimumToolStripMenuItem.Text = "Minimum";
            // 
            // toolStripTextBox_min
            // 
            this.toolStripTextBox_min.Name = "toolStripTextBox_min";
            this.toolStripTextBox_min.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox_min.Text = "0";
            this.toolStripTextBox_min.TextChanged += new System.EventHandler(this.toolStripTextBox_min_TextChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(157, 6);
            // 
            // maximumToolStripMenuItem
            // 
            this.maximumToolStripMenuItem.Name = "maximumToolStripMenuItem";
            this.maximumToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.maximumToolStripMenuItem.Text = "Maximum";
            // 
            // toolStripTextBox_max
            // 
            this.toolStripTextBox_max.Name = "toolStripTextBox_max";
            this.toolStripTextBox_max.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox_max.TextChanged += new System.EventHandler(this.toolStripTextBox_max_TextChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox_4, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox_2, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox_3, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(324, 347);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // groupBox_4
            // 
            this.groupBox_4.Controls.Add(this.progressBar_volt_4);
            this.groupBox_4.Controls.Add(this.label_ripp_4);
            this.groupBox_4.Controls.Add(this.label_freq_4);
            this.groupBox_4.Controls.Add(this.label_volt_4);
            this.groupBox_4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_4.Location = new System.Drawing.Point(3, 261);
            this.groupBox_4.Name = "groupBox_4";
            this.groupBox_4.Size = new System.Drawing.Size(318, 83);
            this.groupBox_4.TabIndex = 11;
            this.groupBox_4.TabStop = false;
            this.groupBox_4.Text = "Channel 4";
            // 
            // progressBar_volt_4
            // 
            this.progressBar_volt_4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_volt_4.Enabled = false;
            this.progressBar_volt_4.Location = new System.Drawing.Point(3, 61);
            this.progressBar_volt_4.Name = "progressBar_volt_4";
            this.progressBar_volt_4.Size = new System.Drawing.Size(312, 19);
            this.progressBar_volt_4.Step = 100;
            this.progressBar_volt_4.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar_volt_4.TabIndex = 6;
            this.progressBar_volt_4.Value = 50;
            // 
            // label_ripp_4
            // 
            this.label_ripp_4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_ripp_4.AutoSize = true;
            this.label_ripp_4.Location = new System.Drawing.Point(200, 16);
            this.label_ripp_4.Name = "label_ripp_4";
            this.label_ripp_4.Size = new System.Drawing.Size(106, 13);
            this.label_ripp_4.TabIndex = 8;
            this.label_ripp_4.Text = "Ripping xxx mVpkpk ";
            // 
            // label_freq_4
            // 
            this.label_freq_4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_freq_4.AutoSize = true;
            this.label_freq_4.Location = new System.Drawing.Point(200, 34);
            this.label_freq_4.Name = "label_freq_4";
            this.label_freq_4.Size = new System.Drawing.Size(91, 13);
            this.label_freq_4.TabIndex = 8;
            this.label_freq_4.Text = "Frequency xxx Hz";
            // 
            // label_volt_4
            // 
            this.label_volt_4.AutoSize = true;
            this.label_volt_4.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_volt_4.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_volt_4.Location = new System.Drawing.Point(3, 16);
            this.label_volt_4.Margin = new System.Windows.Forms.Padding(5);
            this.label_volt_4.Name = "label_volt_4";
            this.label_volt_4.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label_volt_4.Size = new System.Drawing.Size(192, 42);
            this.label_volt_4.TabIndex = 7;
            this.label_volt_4.Text = "2578,25mV";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.groupBox_1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(318, 80);
            this.panel1.TabIndex = 8;
            // 
            // groupBox_1
            // 
            this.groupBox_1.Controls.Add(this.progressBar_volt_1);
            this.groupBox_1.Controls.Add(this.label_ripp_1);
            this.groupBox_1.Controls.Add(this.label_freq_1);
            this.groupBox_1.Controls.Add(this.label_volt_1);
            this.groupBox_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_1.Location = new System.Drawing.Point(0, 0);
            this.groupBox_1.Name = "groupBox_1";
            this.groupBox_1.Size = new System.Drawing.Size(318, 80);
            this.groupBox_1.TabIndex = 10;
            this.groupBox_1.TabStop = false;
            this.groupBox_1.Text = "Channel 1";
            // 
            // progressBar_volt_1
            // 
            this.progressBar_volt_1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_volt_1.Enabled = false;
            this.progressBar_volt_1.Location = new System.Drawing.Point(3, 58);
            this.progressBar_volt_1.MarqueeAnimationSpeed = 1000000;
            this.progressBar_volt_1.Name = "progressBar_volt_1";
            this.progressBar_volt_1.Size = new System.Drawing.Size(312, 19);
            this.progressBar_volt_1.Step = 1;
            this.progressBar_volt_1.TabIndex = 6;
            this.progressBar_volt_1.Value = 50;
            // 
            // label_ripp_1
            // 
            this.label_ripp_1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_ripp_1.AutoSize = true;
            this.label_ripp_1.Location = new System.Drawing.Point(200, 17);
            this.label_ripp_1.Name = "label_ripp_1";
            this.label_ripp_1.Size = new System.Drawing.Size(106, 13);
            this.label_ripp_1.TabIndex = 8;
            this.label_ripp_1.Text = "Ripping xxx mVpkpk ";
            // 
            // label_freq_1
            // 
            this.label_freq_1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_freq_1.AutoSize = true;
            this.label_freq_1.Location = new System.Drawing.Point(200, 35);
            this.label_freq_1.Name = "label_freq_1";
            this.label_freq_1.Size = new System.Drawing.Size(91, 13);
            this.label_freq_1.TabIndex = 8;
            this.label_freq_1.Text = "Frequency xxx Hz";
            // 
            // label_volt_1
            // 
            this.label_volt_1.AutoSize = true;
            this.label_volt_1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_volt_1.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_volt_1.Location = new System.Drawing.Point(3, 16);
            this.label_volt_1.Margin = new System.Windows.Forms.Padding(5);
            this.label_volt_1.Name = "label_volt_1";
            this.label_volt_1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label_volt_1.Size = new System.Drawing.Size(192, 42);
            this.label_volt_1.TabIndex = 7;
            this.label_volt_1.Text = "2578,25mV";
            // 
            // groupBox_2
            // 
            this.groupBox_2.Controls.Add(this.progressBar_volt_2);
            this.groupBox_2.Controls.Add(this.label_ripp_2);
            this.groupBox_2.Controls.Add(this.label_freq_2);
            this.groupBox_2.Controls.Add(this.label_volt_2);
            this.groupBox_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_2.Location = new System.Drawing.Point(3, 89);
            this.groupBox_2.Name = "groupBox_2";
            this.groupBox_2.Size = new System.Drawing.Size(318, 80);
            this.groupBox_2.TabIndex = 9;
            this.groupBox_2.TabStop = false;
            this.groupBox_2.Text = "Channel 2";
            // 
            // progressBar_volt_2
            // 
            this.progressBar_volt_2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_volt_2.Enabled = false;
            this.progressBar_volt_2.Location = new System.Drawing.Point(3, 58);
            this.progressBar_volt_2.Name = "progressBar_volt_2";
            this.progressBar_volt_2.Size = new System.Drawing.Size(312, 19);
            this.progressBar_volt_2.Step = 100;
            this.progressBar_volt_2.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar_volt_2.TabIndex = 6;
            this.progressBar_volt_2.Value = 50;
            // 
            // label_ripp_2
            // 
            this.label_ripp_2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_ripp_2.AutoSize = true;
            this.label_ripp_2.Location = new System.Drawing.Point(200, 17);
            this.label_ripp_2.Name = "label_ripp_2";
            this.label_ripp_2.Size = new System.Drawing.Size(106, 13);
            this.label_ripp_2.TabIndex = 8;
            this.label_ripp_2.Text = "Ripping xxx mVpkpk ";
            // 
            // label_freq_2
            // 
            this.label_freq_2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_freq_2.AutoSize = true;
            this.label_freq_2.Location = new System.Drawing.Point(200, 35);
            this.label_freq_2.Name = "label_freq_2";
            this.label_freq_2.Size = new System.Drawing.Size(91, 13);
            this.label_freq_2.TabIndex = 8;
            this.label_freq_2.Text = "Frequency xxx Hz";
            // 
            // label_volt_2
            // 
            this.label_volt_2.AutoSize = true;
            this.label_volt_2.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_volt_2.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_volt_2.Location = new System.Drawing.Point(3, 16);
            this.label_volt_2.Margin = new System.Windows.Forms.Padding(5);
            this.label_volt_2.Name = "label_volt_2";
            this.label_volt_2.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label_volt_2.Size = new System.Drawing.Size(192, 42);
            this.label_volt_2.TabIndex = 7;
            this.label_volt_2.Text = "2578,25mV";
            // 
            // groupBox_3
            // 
            this.groupBox_3.Controls.Add(this.progressBar_volt_3);
            this.groupBox_3.Controls.Add(this.label_ripp_3);
            this.groupBox_3.Controls.Add(this.label4);
            this.groupBox_3.Controls.Add(this.label_freq_3);
            this.groupBox_3.Controls.Add(this.label8);
            this.groupBox_3.Controls.Add(this.label_volt_3);
            this.groupBox_3.Location = new System.Drawing.Point(3, 175);
            this.groupBox_3.Name = "groupBox_3";
            this.groupBox_3.Size = new System.Drawing.Size(318, 80);
            this.groupBox_3.TabIndex = 10;
            this.groupBox_3.TabStop = false;
            this.groupBox_3.Text = "Channel 3";
            // 
            // progressBar_volt_3
            // 
            this.progressBar_volt_3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar_volt_3.Enabled = false;
            this.progressBar_volt_3.Location = new System.Drawing.Point(3, 58);
            this.progressBar_volt_3.Name = "progressBar_volt_3";
            this.progressBar_volt_3.Size = new System.Drawing.Size(312, 19);
            this.progressBar_volt_3.Step = 100;
            this.progressBar_volt_3.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar_volt_3.TabIndex = 6;
            this.progressBar_volt_3.Value = 50;
            // 
            // label_ripp_3
            // 
            this.label_ripp_3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_ripp_3.AutoSize = true;
            this.label_ripp_3.Location = new System.Drawing.Point(200, 11);
            this.label_ripp_3.Name = "label_ripp_3";
            this.label_ripp_3.Size = new System.Drawing.Size(106, 13);
            this.label_ripp_3.TabIndex = 8;
            this.label_ripp_3.Text = "Ripping xxx mVpkpk ";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(200, 81);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(106, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Ripping xxx mVpkpk ";
            // 
            // label_freq_3
            // 
            this.label_freq_3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label_freq_3.AutoSize = true;
            this.label_freq_3.Location = new System.Drawing.Point(200, 29);
            this.label_freq_3.Name = "label_freq_3";
            this.label_freq_3.Size = new System.Drawing.Size(91, 13);
            this.label_freq_3.TabIndex = 8;
            this.label_freq_3.Text = "Frequency xxx Hz";
            // 
            // label8
            // 
            this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(200, 99);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(91, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "Frequency xxx Hz";
            // 
            // label_volt_3
            // 
            this.label_volt_3.AutoSize = true;
            this.label_volt_3.Dock = System.Windows.Forms.DockStyle.Top;
            this.label_volt_3.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_volt_3.Location = new System.Drawing.Point(3, 16);
            this.label_volt_3.Margin = new System.Windows.Forms.Padding(5);
            this.label_volt_3.Name = "label_volt_3";
            this.label_volt_3.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.label_volt_3.Size = new System.Drawing.Size(192, 42);
            this.label_volt_3.TabIndex = 7;
            this.label_volt_3.Text = "2578,25mV";
            // 
            // Voltmeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(324, 371);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.MaximumSize = new System.Drawing.Size(340, 410);
            this.MinimumSize = new System.Drawing.Size(340, 410);
            this.Name = "Voltmeter";
            this.Text = "Voltmeter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Voltmeter_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox_4.ResumeLayout(false);
            this.groupBox_4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox_1.ResumeLayout(false);
            this.groupBox_1.PerformLayout();
            this.groupBox_2.ResumeLayout(false);
            this.groupBox_2.PerformLayout();
            this.groupBox_3.ResumeLayout(false);
            this.groupBox_3.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimumToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_min;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem maximumToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_max;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox_2;
        private System.Windows.Forms.ProgressBar progressBar_volt_2;
        private System.Windows.Forms.Label label_ripp_2;
        private System.Windows.Forms.Label label_freq_2;
        private System.Windows.Forms.Label label_volt_2;
        private System.Windows.Forms.GroupBox groupBox_3;
        private System.Windows.Forms.ProgressBar progressBar_volt_3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label_volt_3;
        private System.Windows.Forms.GroupBox groupBox_1;
        private System.Windows.Forms.ProgressBar progressBar_volt_1;
        private System.Windows.Forms.Label label_ripp_1;
        private System.Windows.Forms.Label label_freq_1;
        private System.Windows.Forms.Label label_volt_1;
        private System.Windows.Forms.GroupBox groupBox_4;
        private System.Windows.Forms.ProgressBar progressBar_volt_4;
        private System.Windows.Forms.Label label_ripp_4;
        private System.Windows.Forms.Label label_freq_4;
        private System.Windows.Forms.Label label_volt_4;
        private System.Windows.Forms.Label label_ripp_3;
        private System.Windows.Forms.Label label_freq_3;
    }
}