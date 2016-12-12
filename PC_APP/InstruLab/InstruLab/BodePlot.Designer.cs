namespace LEO
{
    partial class BodePlot
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BodePlot));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.zedGraphControl_freq_analysis = new ZedGraph.ZedGraphControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox_cursors_vertical = new System.Windows.Forms.GroupBox();
            this.radioButton_ver_cur_math = new System.Windows.Forms.RadioButton();
            this.radioButton_ver_cur_ch2 = new System.Windows.Forms.RadioButton();
            this.radioButton_ver_cur_off = new System.Windows.Forms.RadioButton();
            this.radioButton_ver_cur_ch1 = new System.Windows.Forms.RadioButton();
            this.label_cur_freq = new System.Windows.Forms.Label();
            this.label_time_diff = new System.Windows.Forms.Label();
            this.label_ver_cur_du = new System.Windows.Forms.Label();
            this.label_cur_time_b = new System.Windows.Forms.Label();
            this.label_cur_time_a = new System.Windows.Forms.Label();
            this.label_cur_ub = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label_cur_ua = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.trackBar_ver_cur_a = new System.Windows.Forms.TrackBar();
            this.trackBar_ver_cur_b = new System.Windows.Forms.TrackBar();
            this.groupBox_cursors_horizontal = new System.Windows.Forms.GroupBox();
            this.trackBar_hor_cur_a = new System.Windows.Forms.TrackBar();
            this.radioButton_hor_cur_math = new System.Windows.Forms.RadioButton();
            this.radioButton_hor_cur_off = new System.Windows.Forms.RadioButton();
            this.radioButton_hor_cur_ch2 = new System.Windows.Forms.RadioButton();
            this.label_hor_cur_volt_diff = new System.Windows.Forms.Label();
            this.radioButton_hor_cur_ch1 = new System.Windows.Forms.RadioButton();
            this.trackBar_hor_cur_b = new System.Windows.Forms.TrackBar();
            this.label_cur_u_b = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label_cur_u_a = new System.Windows.Forms.Label();
            this.groupBox_frequency = new System.Windows.Forms.GroupBox();
            this.groupBox_signal_params = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox_measurement = new System.Windows.Forms.GroupBox();
            this.button_run = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.textBox_f_min = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.channelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label_f_max = new System.Windows.Forms.Label();
            this.textBox_f_max = new System.Windows.Forms.TextBox();
            this.label_num_points = new System.Windows.Forms.Label();
            this.textBox_num_points = new System.Windows.Forms.TextBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_offset = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBox_amplitude = new System.Windows.Forms.TextBox();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.label16 = new System.Windows.Forms.Label();
            this.radioButton4 = new System.Windows.Forms.RadioButton();
            this.radioButton5 = new System.Windows.Forms.RadioButton();
            this.radioButton6 = new System.Windows.Forms.RadioButton();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            this.groupBox_cursors_vertical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ver_cur_a)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ver_cur_b)).BeginInit();
            this.groupBox_cursors_horizontal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_hor_cur_a)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_hor_cur_b)).BeginInit();
            this.groupBox_frequency.SuspendLayout();
            this.groupBox_signal_params.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.groupBox_measurement.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.channelToolStripMenuItem,
            this.mathToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(853, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
            this.tableLayoutPanel1.Controls.Add(this.zedGraphControl_freq_analysis, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox_measurement, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(853, 419);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // zedGraphControl_freq_analysis
            // 
            this.zedGraphControl_freq_analysis.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.zedGraphControl_freq_analysis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControl_freq_analysis.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.zedGraphControl_freq_analysis.IsEnableVZoom = false;
            this.zedGraphControl_freq_analysis.Location = new System.Drawing.Point(2, 2);
            this.zedGraphControl_freq_analysis.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.zedGraphControl_freq_analysis.Name = "zedGraphControl_freq_analysis";
            this.zedGraphControl_freq_analysis.Padding = new System.Windows.Forms.Padding(3);
            this.zedGraphControl_freq_analysis.ScrollGrace = 0D;
            this.zedGraphControl_freq_analysis.ScrollMaxX = 0D;
            this.zedGraphControl_freq_analysis.ScrollMaxY = 0D;
            this.zedGraphControl_freq_analysis.ScrollMaxY2 = 0D;
            this.zedGraphControl_freq_analysis.ScrollMinX = 0D;
            this.zedGraphControl_freq_analysis.ScrollMinY = 0D;
            this.zedGraphControl_freq_analysis.ScrollMinY2 = 0D;
            this.zedGraphControl_freq_analysis.Size = new System.Drawing.Size(649, 295);
            this.zedGraphControl_freq_analysis.TabIndex = 2;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 1;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel2.Controls.Add(this.groupBox_cursors_horizontal, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.groupBox_cursors_vertical, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(656, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(194, 293);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 3;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel5, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.groupBox_frequency, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.groupBox_signal_params, 1, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 302);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(647, 114);
            this.tableLayoutPanel3.TabIndex = 4;
            // 
            // groupBox_cursors_vertical
            // 
            this.groupBox_cursors_vertical.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox_cursors_vertical.Controls.Add(this.radioButton_ver_cur_math);
            this.groupBox_cursors_vertical.Controls.Add(this.radioButton_ver_cur_ch2);
            this.groupBox_cursors_vertical.Controls.Add(this.radioButton_ver_cur_off);
            this.groupBox_cursors_vertical.Controls.Add(this.radioButton_ver_cur_ch1);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_freq);
            this.groupBox_cursors_vertical.Controls.Add(this.label_time_diff);
            this.groupBox_cursors_vertical.Controls.Add(this.label_ver_cur_du);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_time_b);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_time_a);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_ub);
            this.groupBox_cursors_vertical.Controls.Add(this.label6);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_ua);
            this.groupBox_cursors_vertical.Controls.Add(this.label5);
            this.groupBox_cursors_vertical.Controls.Add(this.trackBar_ver_cur_a);
            this.groupBox_cursors_vertical.Controls.Add(this.trackBar_ver_cur_b);
            this.groupBox_cursors_vertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_cursors_vertical.Location = new System.Drawing.Point(3, 3);
            this.groupBox_cursors_vertical.Name = "groupBox_cursors_vertical";
            this.groupBox_cursors_vertical.Size = new System.Drawing.Size(188, 140);
            this.groupBox_cursors_vertical.TabIndex = 4;
            this.groupBox_cursors_vertical.TabStop = false;
            this.groupBox_cursors_vertical.Text = "Vertical cursors";
            // 
            // radioButton_ver_cur_math
            // 
            this.radioButton_ver_cur_math.AutoSize = true;
            this.radioButton_ver_cur_math.Enabled = false;
            this.radioButton_ver_cur_math.Location = new System.Drawing.Point(79, 89);
            this.radioButton_ver_cur_math.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_ver_cur_math.Name = "radioButton_ver_cur_math";
            this.radioButton_ver_cur_math.Size = new System.Drawing.Size(49, 17);
            this.radioButton_ver_cur_math.TabIndex = 4;
            this.radioButton_ver_cur_math.Text = "Math";
            this.radioButton_ver_cur_math.UseVisualStyleBackColor = true;
            // 
            // radioButton_ver_cur_ch2
            // 
            this.radioButton_ver_cur_ch2.AutoSize = true;
            this.radioButton_ver_cur_ch2.Location = new System.Drawing.Point(79, 111);
            this.radioButton_ver_cur_ch2.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_ver_cur_ch2.Name = "radioButton_ver_cur_ch2";
            this.radioButton_ver_cur_ch2.Size = new System.Drawing.Size(47, 17);
            this.radioButton_ver_cur_ch2.TabIndex = 4;
            this.radioButton_ver_cur_ch2.Text = "Ch 2";
            this.radioButton_ver_cur_ch2.UseVisualStyleBackColor = true;
            // 
            // radioButton_ver_cur_off
            // 
            this.radioButton_ver_cur_off.AutoSize = true;
            this.radioButton_ver_cur_off.Checked = true;
            this.radioButton_ver_cur_off.Location = new System.Drawing.Point(4, 89);
            this.radioButton_ver_cur_off.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_ver_cur_off.Name = "radioButton_ver_cur_off";
            this.radioButton_ver_cur_off.Size = new System.Drawing.Size(39, 17);
            this.radioButton_ver_cur_off.TabIndex = 4;
            this.radioButton_ver_cur_off.TabStop = true;
            this.radioButton_ver_cur_off.Text = "Off";
            this.radioButton_ver_cur_off.UseVisualStyleBackColor = true;
            // 
            // radioButton_ver_cur_ch1
            // 
            this.radioButton_ver_cur_ch1.AutoSize = true;
            this.radioButton_ver_cur_ch1.Location = new System.Drawing.Point(4, 111);
            this.radioButton_ver_cur_ch1.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_ver_cur_ch1.Name = "radioButton_ver_cur_ch1";
            this.radioButton_ver_cur_ch1.Size = new System.Drawing.Size(47, 17);
            this.radioButton_ver_cur_ch1.TabIndex = 4;
            this.radioButton_ver_cur_ch1.Text = "Ch 1";
            this.radioButton_ver_cur_ch1.UseVisualStyleBackColor = true;
            // 
            // label_cur_freq
            // 
            this.label_cur_freq.AutoSize = true;
            this.label_cur_freq.Enabled = false;
            this.label_cur_freq.Location = new System.Drawing.Point(79, 50);
            this.label_cur_freq.Name = "label_cur_freq";
            this.label_cur_freq.Size = new System.Drawing.Size(35, 13);
            this.label_cur_freq.TabIndex = 3;
            this.label_cur_freq.Text = "f -- Hz";
            // 
            // label_time_diff
            // 
            this.label_time_diff.AutoSize = true;
            this.label_time_diff.Enabled = false;
            this.label_time_diff.Location = new System.Drawing.Point(79, 67);
            this.label_time_diff.Name = "label_time_diff";
            this.label_time_diff.Size = new System.Drawing.Size(43, 13);
            this.label_time_diff.TabIndex = 2;
            this.label_time_diff.Text = "dt -- mS";
            this.label_time_diff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_ver_cur_du
            // 
            this.label_ver_cur_du.AutoSize = true;
            this.label_ver_cur_du.Enabled = false;
            this.label_ver_cur_du.Location = new System.Drawing.Point(3, 67);
            this.label_ver_cur_du.Name = "label_ver_cur_du";
            this.label_ver_cur_du.Size = new System.Drawing.Size(48, 13);
            this.label_ver_cur_du.TabIndex = 2;
            this.label_ver_cur_du.Text = "dU -- mV";
            this.label_ver_cur_du.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_time_b
            // 
            this.label_cur_time_b.AutoSize = true;
            this.label_cur_time_b.Enabled = false;
            this.label_cur_time_b.Location = new System.Drawing.Point(79, 28);
            this.label_cur_time_b.Name = "label_cur_time_b";
            this.label_cur_time_b.Size = new System.Drawing.Size(37, 13);
            this.label_cur_time_b.TabIndex = 2;
            this.label_cur_time_b.Text = "t -- mS";
            this.label_cur_time_b.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_time_a
            // 
            this.label_cur_time_a.AutoSize = true;
            this.label_cur_time_a.Enabled = false;
            this.label_cur_time_a.Location = new System.Drawing.Point(79, 15);
            this.label_cur_time_a.Name = "label_cur_time_a";
            this.label_cur_time_a.Size = new System.Drawing.Size(37, 13);
            this.label_cur_time_a.TabIndex = 2;
            this.label_cur_time_a.Text = "t -- mS";
            this.label_cur_time_a.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_ub
            // 
            this.label_cur_ub.AutoSize = true;
            this.label_cur_ub.Enabled = false;
            this.label_cur_ub.Location = new System.Drawing.Point(12, 28);
            this.label_cur_ub.Margin = new System.Windows.Forms.Padding(0);
            this.label_cur_ub.Name = "label_cur_ub";
            this.label_cur_ub.Size = new System.Drawing.Size(42, 13);
            this.label_cur_ub.TabIndex = 2;
            this.label_cur_ub.Text = "U -- mV";
            this.label_cur_ub.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(2, 28);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "2:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_ua
            // 
            this.label_cur_ua.AutoSize = true;
            this.label_cur_ua.Enabled = false;
            this.label_cur_ua.Location = new System.Drawing.Point(12, 15);
            this.label_cur_ua.Margin = new System.Windows.Forms.Padding(0);
            this.label_cur_ua.Name = "label_cur_ua";
            this.label_cur_ua.Size = new System.Drawing.Size(42, 13);
            this.label_cur_ua.TabIndex = 2;
            this.label_cur_ua.Text = "U -- mV";
            this.label_cur_ua.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(2, 15);
            this.label5.Margin = new System.Windows.Forms.Padding(0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(16, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "1:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // trackBar_ver_cur_a
            // 
            this.trackBar_ver_cur_a.AutoSize = false;
            this.trackBar_ver_cur_a.Dock = System.Windows.Forms.DockStyle.Right;
            this.trackBar_ver_cur_a.Enabled = false;
            this.trackBar_ver_cur_a.Location = new System.Drawing.Point(139, 16);
            this.trackBar_ver_cur_a.Maximum = 4096;
            this.trackBar_ver_cur_a.Name = "trackBar_ver_cur_a";
            this.trackBar_ver_cur_a.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_ver_cur_a.Size = new System.Drawing.Size(23, 121);
            this.trackBar_ver_cur_a.TabIndex = 0;
            this.trackBar_ver_cur_a.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // trackBar_ver_cur_b
            // 
            this.trackBar_ver_cur_b.AutoSize = false;
            this.trackBar_ver_cur_b.Dock = System.Windows.Forms.DockStyle.Right;
            this.trackBar_ver_cur_b.Enabled = false;
            this.trackBar_ver_cur_b.Location = new System.Drawing.Point(162, 16);
            this.trackBar_ver_cur_b.Maximum = 4096;
            this.trackBar_ver_cur_b.Name = "trackBar_ver_cur_b";
            this.trackBar_ver_cur_b.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_ver_cur_b.Size = new System.Drawing.Size(23, 121);
            this.trackBar_ver_cur_b.TabIndex = 0;
            this.trackBar_ver_cur_b.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // groupBox_cursors_horizontal
            // 
            this.groupBox_cursors_horizontal.Controls.Add(this.trackBar_hor_cur_a);
            this.groupBox_cursors_horizontal.Controls.Add(this.radioButton_hor_cur_math);
            this.groupBox_cursors_horizontal.Controls.Add(this.radioButton_hor_cur_off);
            this.groupBox_cursors_horizontal.Controls.Add(this.radioButton_hor_cur_ch2);
            this.groupBox_cursors_horizontal.Controls.Add(this.label_hor_cur_volt_diff);
            this.groupBox_cursors_horizontal.Controls.Add(this.radioButton_hor_cur_ch1);
            this.groupBox_cursors_horizontal.Controls.Add(this.trackBar_hor_cur_b);
            this.groupBox_cursors_horizontal.Controls.Add(this.label_cur_u_b);
            this.groupBox_cursors_horizontal.Controls.Add(this.label8);
            this.groupBox_cursors_horizontal.Controls.Add(this.label10);
            this.groupBox_cursors_horizontal.Controls.Add(this.label_cur_u_a);
            this.groupBox_cursors_horizontal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_cursors_horizontal.Location = new System.Drawing.Point(3, 149);
            this.groupBox_cursors_horizontal.Name = "groupBox_cursors_horizontal";
            this.groupBox_cursors_horizontal.Size = new System.Drawing.Size(188, 141);
            this.groupBox_cursors_horizontal.TabIndex = 5;
            this.groupBox_cursors_horizontal.TabStop = false;
            this.groupBox_cursors_horizontal.Text = "Horizontal cursors";
            // 
            // trackBar_hor_cur_a
            // 
            this.trackBar_hor_cur_a.AutoSize = false;
            this.trackBar_hor_cur_a.Dock = System.Windows.Forms.DockStyle.Right;
            this.trackBar_hor_cur_a.Enabled = false;
            this.trackBar_hor_cur_a.Location = new System.Drawing.Point(135, 16);
            this.trackBar_hor_cur_a.Maximum = 2048;
            this.trackBar_hor_cur_a.Name = "trackBar_hor_cur_a";
            this.trackBar_hor_cur_a.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_hor_cur_a.Size = new System.Drawing.Size(25, 122);
            this.trackBar_hor_cur_a.TabIndex = 0;
            this.trackBar_hor_cur_a.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // radioButton_hor_cur_math
            // 
            this.radioButton_hor_cur_math.AutoSize = true;
            this.radioButton_hor_cur_math.Enabled = false;
            this.radioButton_hor_cur_math.Location = new System.Drawing.Point(76, 73);
            this.radioButton_hor_cur_math.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_hor_cur_math.Name = "radioButton_hor_cur_math";
            this.radioButton_hor_cur_math.Size = new System.Drawing.Size(49, 17);
            this.radioButton_hor_cur_math.TabIndex = 4;
            this.radioButton_hor_cur_math.Text = "Math";
            this.radioButton_hor_cur_math.UseVisualStyleBackColor = true;
            // 
            // radioButton_hor_cur_off
            // 
            this.radioButton_hor_cur_off.AutoSize = true;
            this.radioButton_hor_cur_off.Checked = true;
            this.radioButton_hor_cur_off.Location = new System.Drawing.Point(4, 73);
            this.radioButton_hor_cur_off.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_hor_cur_off.Name = "radioButton_hor_cur_off";
            this.radioButton_hor_cur_off.Size = new System.Drawing.Size(39, 17);
            this.radioButton_hor_cur_off.TabIndex = 4;
            this.radioButton_hor_cur_off.TabStop = true;
            this.radioButton_hor_cur_off.Text = "Off";
            this.radioButton_hor_cur_off.UseVisualStyleBackColor = true;
            // 
            // radioButton_hor_cur_ch2
            // 
            this.radioButton_hor_cur_ch2.AutoSize = true;
            this.radioButton_hor_cur_ch2.Location = new System.Drawing.Point(76, 95);
            this.radioButton_hor_cur_ch2.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_hor_cur_ch2.Name = "radioButton_hor_cur_ch2";
            this.radioButton_hor_cur_ch2.Size = new System.Drawing.Size(47, 17);
            this.radioButton_hor_cur_ch2.TabIndex = 4;
            this.radioButton_hor_cur_ch2.Text = "Ch 2";
            this.radioButton_hor_cur_ch2.UseVisualStyleBackColor = true;
            // 
            // label_hor_cur_volt_diff
            // 
            this.label_hor_cur_volt_diff.AutoSize = true;
            this.label_hor_cur_volt_diff.Enabled = false;
            this.label_hor_cur_volt_diff.Location = new System.Drawing.Point(28, 57);
            this.label_hor_cur_volt_diff.Name = "label_hor_cur_volt_diff";
            this.label_hor_cur_volt_diff.Size = new System.Drawing.Size(48, 13);
            this.label_hor_cur_volt_diff.TabIndex = 2;
            this.label_hor_cur_volt_diff.Text = "dU -- mV";
            this.label_hor_cur_volt_diff.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // radioButton_hor_cur_ch1
            // 
            this.radioButton_hor_cur_ch1.AutoSize = true;
            this.radioButton_hor_cur_ch1.Location = new System.Drawing.Point(4, 95);
            this.radioButton_hor_cur_ch1.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_hor_cur_ch1.Name = "radioButton_hor_cur_ch1";
            this.radioButton_hor_cur_ch1.Size = new System.Drawing.Size(47, 17);
            this.radioButton_hor_cur_ch1.TabIndex = 4;
            this.radioButton_hor_cur_ch1.Text = "Ch 1";
            this.radioButton_hor_cur_ch1.UseVisualStyleBackColor = true;
            // 
            // trackBar_hor_cur_b
            // 
            this.trackBar_hor_cur_b.AutoSize = false;
            this.trackBar_hor_cur_b.Dock = System.Windows.Forms.DockStyle.Right;
            this.trackBar_hor_cur_b.Enabled = false;
            this.trackBar_hor_cur_b.Location = new System.Drawing.Point(160, 16);
            this.trackBar_hor_cur_b.Maximum = 2048;
            this.trackBar_hor_cur_b.Name = "trackBar_hor_cur_b";
            this.trackBar_hor_cur_b.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_hor_cur_b.Size = new System.Drawing.Size(25, 122);
            this.trackBar_hor_cur_b.TabIndex = 0;
            this.trackBar_hor_cur_b.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // label_cur_u_b
            // 
            this.label_cur_u_b.AutoSize = true;
            this.label_cur_u_b.Enabled = false;
            this.label_cur_u_b.Location = new System.Drawing.Point(32, 36);
            this.label_cur_u_b.Name = "label_cur_u_b";
            this.label_cur_u_b.Size = new System.Drawing.Size(42, 13);
            this.label_cur_u_b.TabIndex = 2;
            this.label_cur_u_b.Text = "U -- mV";
            this.label_cur_u_b.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Enabled = false;
            this.label8.Location = new System.Drawing.Point(7, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(16, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "1:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Enabled = false;
            this.label10.Location = new System.Drawing.Point(7, 36);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(16, 13);
            this.label10.TabIndex = 2;
            this.label10.Text = "2:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_u_a
            // 
            this.label_cur_u_a.AutoSize = true;
            this.label_cur_u_a.Enabled = false;
            this.label_cur_u_a.Location = new System.Drawing.Point(32, 23);
            this.label_cur_u_a.Name = "label_cur_u_a";
            this.label_cur_u_a.Size = new System.Drawing.Size(42, 13);
            this.label_cur_u_a.TabIndex = 2;
            this.label_cur_u_a.Text = "U -- mV";
            this.label_cur_u_a.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox_frequency
            // 
            this.groupBox_frequency.Controls.Add(this.label9);
            this.groupBox_frequency.Controls.Add(this.label11);
            this.groupBox_frequency.Controls.Add(this.label_num_points);
            this.groupBox_frequency.Controls.Add(this.textBox_num_points);
            this.groupBox_frequency.Controls.Add(this.label_f_max);
            this.groupBox_frequency.Controls.Add(this.textBox_f_max);
            this.groupBox_frequency.Controls.Add(this.label1);
            this.groupBox_frequency.Controls.Add(this.textBox_f_min);
            this.groupBox_frequency.Controls.Add(this.radioButton2);
            this.groupBox_frequency.Controls.Add(this.radioButton1);
            this.groupBox_frequency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_frequency.Location = new System.Drawing.Point(3, 3);
            this.groupBox_frequency.Name = "groupBox_frequency";
            this.groupBox_frequency.Size = new System.Drawing.Size(209, 108);
            this.groupBox_frequency.TabIndex = 0;
            this.groupBox_frequency.TabStop = false;
            this.groupBox_frequency.Text = "Frequency range";
            // 
            // groupBox_signal_params
            // 
            this.groupBox_signal_params.Controls.Add(this.label12);
            this.groupBox_signal_params.Controls.Add(this.textBox_amplitude);
            this.groupBox_signal_params.Controls.Add(this.label13);
            this.groupBox_signal_params.Controls.Add(this.label15);
            this.groupBox_signal_params.Controls.Add(this.label14);
            this.groupBox_signal_params.Controls.Add(this.textBox_offset);
            this.groupBox_signal_params.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_signal_params.Location = new System.Drawing.Point(218, 3);
            this.groupBox_signal_params.Name = "groupBox_signal_params";
            this.groupBox_signal_params.Size = new System.Drawing.Size(209, 108);
            this.groupBox_signal_params.TabIndex = 1;
            this.groupBox_signal_params.TabStop = false;
            this.groupBox_signal_params.Text = "Signal parameters";
            // 
            // tableLayoutPanel5
            // 
            this.tableLayoutPanel5.AutoSize = true;
            this.tableLayoutPanel5.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel5.ColumnCount = 1;
            this.tableLayoutPanel5.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel5.Controls.Add(this.trackBar1, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.trackBar2, 0, 3);
            this.tableLayoutPanel5.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel5.Controls.Add(this.label4, 0, 2);
            this.tableLayoutPanel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel5.Location = new System.Drawing.Point(433, 3);
            this.tableLayoutPanel5.Name = "tableLayoutPanel5";
            this.tableLayoutPanel5.RowCount = 4;
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 17F));
            this.tableLayoutPanel5.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel5.Size = new System.Drawing.Size(211, 108);
            this.tableLayoutPanel5.TabIndex = 6;
            // 
            // trackBar1
            // 
            this.trackBar1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar1.Location = new System.Drawing.Point(3, 20);
            this.trackBar1.Maximum = 1024;
            this.trackBar1.MaximumSize = new System.Drawing.Size(1024, 30);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(205, 30);
            this.trackBar1.TabIndex = 1;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // trackBar2
            // 
            this.trackBar2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBar2.Location = new System.Drawing.Point(3, 75);
            this.trackBar2.Maximum = 1024;
            this.trackBar2.MaximumSize = new System.Drawing.Size(1024, 30);
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Size = new System.Drawing.Size(205, 30);
            this.trackBar2.TabIndex = 1;
            this.trackBar2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar2.Value = 512;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "Zoom";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Position";
            // 
            // groupBox_measurement
            // 
            this.groupBox_measurement.Controls.Add(this.label18);
            this.groupBox_measurement.Controls.Add(this.label17);
            this.groupBox_measurement.Controls.Add(this.label16);
            this.groupBox_measurement.Controls.Add(this.radioButton6);
            this.groupBox_measurement.Controls.Add(this.radioButton5);
            this.groupBox_measurement.Controls.Add(this.radioButton4);
            this.groupBox_measurement.Controls.Add(this.radioButton3);
            this.groupBox_measurement.Controls.Add(this.button_run);
            this.groupBox_measurement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_measurement.Location = new System.Drawing.Point(656, 302);
            this.groupBox_measurement.Name = "groupBox_measurement";
            this.groupBox_measurement.Size = new System.Drawing.Size(194, 114);
            this.groupBox_measurement.TabIndex = 5;
            this.groupBox_measurement.TabStop = false;
            this.groupBox_measurement.Text = "Measurement";
            // 
            // button_run
            // 
            this.button_run.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_run.Location = new System.Drawing.Point(79, 88);
            this.button_run.Name = "button_run";
            this.button_run.Size = new System.Drawing.Size(112, 23);
            this.button_run.TabIndex = 0;
            this.button_run.Text = "Run";
            this.button_run.UseVisualStyleBackColor = true;
            this.button_run.Click += new System.EventHandler(this.button_run_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(149, 85);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(54, 17);
            this.radioButton1.TabIndex = 0;
            this.radioButton1.Text = "Linear";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // textBox_f_min
            // 
            this.textBox_f_min.Location = new System.Drawing.Point(44, 22);
            this.textBox_f_min.Name = "textBox_f_min";
            this.textBox_f_min.Size = new System.Drawing.Size(51, 20);
            this.textBox_f_min.TabIndex = 1;
            this.textBox_f_min.Text = "1";
            this.textBox_f_min.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_f_min_KeyPress);
            this.textBox_f_min.Leave += new System.EventHandler(this.textBox_f_min_Leave);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "f min";
            // 
            // channelToolStripMenuItem
            // 
            this.channelToolStripMenuItem.Name = "channelToolStripMenuItem";
            this.channelToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.channelToolStripMenuItem.Text = "Channel";
            // 
            // mathToolStripMenuItem
            // 
            this.mathToolStripMenuItem.Name = "mathToolStripMenuItem";
            this.mathToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.mathToolStripMenuItem.Text = "Math";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // label_f_max
            // 
            this.label_f_max.AutoSize = true;
            this.label_f_max.Location = new System.Drawing.Point(7, 55);
            this.label_f_max.Name = "label_f_max";
            this.label_f_max.Size = new System.Drawing.Size(32, 13);
            this.label_f_max.TabIndex = 4;
            this.label_f_max.Text = "f max";
            // 
            // textBox_f_max
            // 
            this.textBox_f_max.Location = new System.Drawing.Point(44, 52);
            this.textBox_f_max.Name = "textBox_f_max";
            this.textBox_f_max.Size = new System.Drawing.Size(51, 20);
            this.textBox_f_max.TabIndex = 3;
            this.textBox_f_max.Text = "100000";
            this.textBox_f_max.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_f_max_KeyPress);
            this.textBox_f_max.Leave += new System.EventHandler(this.textBox_f_max_Leave);
            // 
            // label_num_points
            // 
            this.label_num_points.AutoSize = true;
            this.label_num_points.Location = new System.Drawing.Point(7, 85);
            this.label_num_points.Name = "label_num_points";
            this.label_num_points.Size = new System.Drawing.Size(36, 13);
            this.label_num_points.TabIndex = 6;
            this.label_num_points.Text = "Points";
            // 
            // textBox_num_points
            // 
            this.textBox_num_points.Location = new System.Drawing.Point(44, 82);
            this.textBox_num_points.Name = "textBox_num_points";
            this.textBox_num_points.Size = new System.Drawing.Size(51, 20);
            this.textBox_num_points.TabIndex = 5;
            this.textBox_num_points.Text = "20";
            this.textBox_num_points.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_num_points_KeyPress);
            this.textBox_num_points.Leave += new System.EventHandler(this.textBox_num_points_Leave);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(100, 85);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(43, 17);
            this.radioButton2.TabIndex = 0;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Log";
            this.radioButton2.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(97, 55);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(20, 13);
            this.label9.TabIndex = 8;
            this.label9.Text = "Hz";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(97, 25);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(20, 13);
            this.label11.TabIndex = 7;
            this.label11.Text = "Hz";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(114, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "mV";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(114, 25);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(22, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "mV";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(8, 55);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(35, 13);
            this.label14.TabIndex = 12;
            this.label14.Text = "Offset";
            // 
            // textBox_offset
            // 
            this.textBox_offset.Location = new System.Drawing.Point(61, 52);
            this.textBox_offset.Name = "textBox_offset";
            this.textBox_offset.Size = new System.Drawing.Size(51, 20);
            this.textBox_offset.TabIndex = 11;
            this.textBox_offset.Text = "1600";
            this.textBox_offset.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_offset_KeyPress);
            this.textBox_offset.Leave += new System.EventHandler(this.textBox_offset_Leave);
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(8, 25);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(53, 13);
            this.label15.TabIndex = 10;
            this.label15.Text = "Amplitude";
            // 
            // textBox_amplitude
            // 
            this.textBox_amplitude.Location = new System.Drawing.Point(61, 22);
            this.textBox_amplitude.Name = "textBox_amplitude";
            this.textBox_amplitude.Size = new System.Drawing.Size(51, 20);
            this.textBox_amplitude.TabIndex = 9;
            this.textBox_amplitude.Text = "500";
            this.textBox_amplitude.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_amplitude_KeyPress);
            this.textBox_amplitude.Leave += new System.EventHandler(this.textBox_amplitude_Leave);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Checked = true;
            this.radioButton3.Location = new System.Drawing.Point(12, 33);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(36, 17);
            this.radioButton3.TabIndex = 1;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "1x";
            this.radioButton3.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 17);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Averaging";
            // 
            // radioButton4
            // 
            this.radioButton4.AutoSize = true;
            this.radioButton4.Location = new System.Drawing.Point(12, 73);
            this.radioButton4.Name = "radioButton4";
            this.radioButton4.Size = new System.Drawing.Size(36, 17);
            this.radioButton4.TabIndex = 1;
            this.radioButton4.Text = "4x";
            this.radioButton4.UseVisualStyleBackColor = true;
            // 
            // radioButton5
            // 
            this.radioButton5.AutoSize = true;
            this.radioButton5.Location = new System.Drawing.Point(12, 93);
            this.radioButton5.Name = "radioButton5";
            this.radioButton5.Size = new System.Drawing.Size(36, 17);
            this.radioButton5.TabIndex = 1;
            this.radioButton5.Text = "8x";
            this.radioButton5.UseVisualStyleBackColor = true;
            // 
            // radioButton6
            // 
            this.radioButton6.AutoSize = true;
            this.radioButton6.Location = new System.Drawing.Point(12, 53);
            this.radioButton6.Name = "radioButton6";
            this.radioButton6.Size = new System.Drawing.Size(36, 17);
            this.radioButton6.TabIndex = 1;
            this.radioButton6.Text = "2x";
            this.radioButton6.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(79, 55);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(25, 13);
            this.label17.TabIndex = 3;
            this.label17.Text = "Info";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(79, 72);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(98, 13);
            this.label18.TabIndex = 3;
            this.label18.Text = "Estimated time: 50s";
            // 
            // BodePlot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(853, 443);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "BodePlot";
            this.Text = "BodePlot";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.groupBox_cursors_vertical.ResumeLayout(false);
            this.groupBox_cursors_vertical.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ver_cur_a)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ver_cur_b)).EndInit();
            this.groupBox_cursors_horizontal.ResumeLayout(false);
            this.groupBox_cursors_horizontal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_hor_cur_a)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_hor_cur_b)).EndInit();
            this.groupBox_frequency.ResumeLayout(false);
            this.groupBox_frequency.PerformLayout();
            this.groupBox_signal_params.ResumeLayout(false);
            this.groupBox_signal_params.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.groupBox_measurement.ResumeLayout(false);
            this.groupBox_measurement.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private ZedGraph.ZedGraphControl zedGraphControl_freq_analysis;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.GroupBox groupBox_cursors_vertical;
        private System.Windows.Forms.RadioButton radioButton_ver_cur_math;
        private System.Windows.Forms.RadioButton radioButton_ver_cur_ch2;
        private System.Windows.Forms.RadioButton radioButton_ver_cur_off;
        private System.Windows.Forms.RadioButton radioButton_ver_cur_ch1;
        private System.Windows.Forms.Label label_cur_freq;
        private System.Windows.Forms.Label label_time_diff;
        private System.Windows.Forms.Label label_ver_cur_du;
        private System.Windows.Forms.Label label_cur_time_b;
        private System.Windows.Forms.Label label_cur_time_a;
        private System.Windows.Forms.Label label_cur_ub;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label_cur_ua;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackBar_ver_cur_a;
        private System.Windows.Forms.TrackBar trackBar_ver_cur_b;
        private System.Windows.Forms.GroupBox groupBox_cursors_horizontal;
        private System.Windows.Forms.TrackBar trackBar_hor_cur_a;
        private System.Windows.Forms.RadioButton radioButton_hor_cur_math;
        private System.Windows.Forms.RadioButton radioButton_hor_cur_off;
        private System.Windows.Forms.RadioButton radioButton_hor_cur_ch2;
        private System.Windows.Forms.Label label_hor_cur_volt_diff;
        private System.Windows.Forms.RadioButton radioButton_hor_cur_ch1;
        private System.Windows.Forms.TrackBar trackBar_hor_cur_b;
        private System.Windows.Forms.Label label_cur_u_b;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label_cur_u_a;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel5;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox_frequency;
        private System.Windows.Forms.GroupBox groupBox_signal_params;
        private System.Windows.Forms.GroupBox groupBox_measurement;
        private System.Windows.Forms.Button button_run;
        private System.Windows.Forms.ToolStripMenuItem channelToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mathToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label_num_points;
        private System.Windows.Forms.TextBox textBox_num_points;
        private System.Windows.Forms.Label label_f_max;
        private System.Windows.Forms.TextBox textBox_f_max;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_f_min;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_amplitude;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_offset;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.RadioButton radioButton6;
        private System.Windows.Forms.RadioButton radioButton5;
        private System.Windows.Forms.RadioButton radioButton4;
        private System.Windows.Forms.RadioButton radioButton3;
    }
}