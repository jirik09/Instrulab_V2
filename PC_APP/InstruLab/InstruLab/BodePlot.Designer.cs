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
            this.channelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mathToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.zedGraphControl_freq_analysis = new ZedGraph.ZedGraphControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox_cursors_vertical = new System.Windows.Forms.GroupBox();
            this.radioButton_ver_cur_on = new System.Windows.Forms.RadioButton();
            this.radioButton_ver_cur_off = new System.Windows.Forms.RadioButton();
            this.label_cur_dphi = new System.Windows.Forms.Label();
            this.label_cur_df = new System.Windows.Forms.Label();
            this.label_cur_da = new System.Windows.Forms.Label();
            this.label_cur_b_phase = new System.Windows.Forms.Label();
            this.label_cur_b_freq = new System.Windows.Forms.Label();
            this.label_cur_a_phase = new System.Windows.Forms.Label();
            this.label_cur_b_ampl = new System.Windows.Forms.Label();
            this.label_cur_a_freq = new System.Windows.Forms.Label();
            this.label_cur_a_ampl = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.trackBar_ver_cur_a = new System.Windows.Forms.TrackBar();
            this.trackBar_ver_cur_b = new System.Windows.Forms.TrackBar();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel5 = new System.Windows.Forms.TableLayoutPanel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox_frequency = new System.Windows.Forms.GroupBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label_num_points = new System.Windows.Forms.Label();
            this.textBox_num_points = new System.Windows.Forms.TextBox();
            this.label_f_max = new System.Windows.Forms.Label();
            this.textBox_f_max = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_f_min = new System.Windows.Forms.TextBox();
            this.radioButton_log = new System.Windows.Forms.RadioButton();
            this.radioButton_lin = new System.Windows.Forms.RadioButton();
            this.groupBox_signal_params = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBox_amplitude = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.textBox_offset = new System.Windows.Forms.TextBox();
            this.groupBox_measurement = new System.Windows.Forms.GroupBox();
            this.label_info = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.radioButton_avg2 = new System.Windows.Forms.RadioButton();
            this.radioButton_avg8 = new System.Windows.Forms.RadioButton();
            this.radioButton_avg4 = new System.Windows.Forms.RadioButton();
            this.radioButton_avg1 = new System.Windows.Forms.RadioButton();
            this.button_run = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.groupBox_cursors_vertical.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ver_cur_a)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ver_cur_b)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            this.groupBox_frequency.SuspendLayout();
            this.groupBox_signal_params.SuspendLayout();
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
            this.tableLayoutPanel2.Controls.Add(this.groupBox_cursors_vertical, 0, 0);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(656, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 293F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(194, 293);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // groupBox_cursors_vertical
            // 
            this.groupBox_cursors_vertical.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox_cursors_vertical.Controls.Add(this.radioButton_ver_cur_on);
            this.groupBox_cursors_vertical.Controls.Add(this.radioButton_ver_cur_off);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_dphi);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_df);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_da);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_b_phase);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_b_freq);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_a_phase);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_b_ampl);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_a_freq);
            this.groupBox_cursors_vertical.Controls.Add(this.label_cur_a_ampl);
            this.groupBox_cursors_vertical.Controls.Add(this.label6);
            this.groupBox_cursors_vertical.Controls.Add(this.label5);
            this.groupBox_cursors_vertical.Controls.Add(this.trackBar_ver_cur_a);
            this.groupBox_cursors_vertical.Controls.Add(this.trackBar_ver_cur_b);
            this.groupBox_cursors_vertical.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_cursors_vertical.Location = new System.Drawing.Point(3, 3);
            this.groupBox_cursors_vertical.Name = "groupBox_cursors_vertical";
            this.groupBox_cursors_vertical.Size = new System.Drawing.Size(188, 287);
            this.groupBox_cursors_vertical.TabIndex = 4;
            this.groupBox_cursors_vertical.TabStop = false;
            this.groupBox_cursors_vertical.Text = "Vertical cursors";
            // 
            // radioButton_ver_cur_on
            // 
            this.radioButton_ver_cur_on.AutoSize = true;
            this.radioButton_ver_cur_on.Location = new System.Drawing.Point(80, 181);
            this.radioButton_ver_cur_on.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_ver_cur_on.Name = "radioButton_ver_cur_on";
            this.radioButton_ver_cur_on.Size = new System.Drawing.Size(39, 17);
            this.radioButton_ver_cur_on.TabIndex = 4;
            this.radioButton_ver_cur_on.Text = "On";
            this.radioButton_ver_cur_on.UseVisualStyleBackColor = true;
            this.radioButton_ver_cur_on.CheckedChanged += new System.EventHandler(this.radioButton_ver_cur_on_CheckedChanged);
            // 
            // radioButton_ver_cur_off
            // 
            this.radioButton_ver_cur_off.AutoSize = true;
            this.radioButton_ver_cur_off.Checked = true;
            this.radioButton_ver_cur_off.Location = new System.Drawing.Point(4, 181);
            this.radioButton_ver_cur_off.Margin = new System.Windows.Forms.Padding(2);
            this.radioButton_ver_cur_off.Name = "radioButton_ver_cur_off";
            this.radioButton_ver_cur_off.Size = new System.Drawing.Size(39, 17);
            this.radioButton_ver_cur_off.TabIndex = 4;
            this.radioButton_ver_cur_off.TabStop = true;
            this.radioButton_ver_cur_off.Text = "Off";
            this.radioButton_ver_cur_off.UseVisualStyleBackColor = true;
            this.radioButton_ver_cur_off.CheckedChanged += new System.EventHandler(this.radioButton_ver_cur_off_CheckedChanged);
            // 
            // label_cur_dphi
            // 
            this.label_cur_dphi.AutoSize = true;
            this.label_cur_dphi.Enabled = false;
            this.label_cur_dphi.Location = new System.Drawing.Point(2, 130);
            this.label_cur_dphi.Name = "label_cur_dphi";
            this.label_cur_dphi.Size = new System.Drawing.Size(44, 13);
            this.label_cur_dphi.TabIndex = 2;
            this.label_cur_dphi.Text = "dPhi -- °";
            this.label_cur_dphi.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_df
            // 
            this.label_cur_df.AutoSize = true;
            this.label_cur_df.Enabled = false;
            this.label_cur_df.Location = new System.Drawing.Point(2, 115);
            this.label_cur_df.Name = "label_cur_df";
            this.label_cur_df.Size = new System.Drawing.Size(41, 13);
            this.label_cur_df.TabIndex = 2;
            this.label_cur_df.Text = "df -- Hz";
            this.label_cur_df.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_da
            // 
            this.label_cur_da.AutoSize = true;
            this.label_cur_da.Enabled = false;
            this.label_cur_da.Location = new System.Drawing.Point(2, 100);
            this.label_cur_da.Name = "label_cur_da";
            this.label_cur_da.Size = new System.Drawing.Size(45, 13);
            this.label_cur_da.TabIndex = 2;
            this.label_cur_da.Text = "dA -- dB";
            this.label_cur_da.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_b_phase
            // 
            this.label_cur_b_phase.AutoSize = true;
            this.label_cur_b_phase.Enabled = false;
            this.label_cur_b_phase.Location = new System.Drawing.Point(70, 65);
            this.label_cur_b_phase.Name = "label_cur_b_phase";
            this.label_cur_b_phase.Size = new System.Drawing.Size(38, 13);
            this.label_cur_b_phase.TabIndex = 2;
            this.label_cur_b_phase.Text = "Phi -- °";
            this.label_cur_b_phase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_b_freq
            // 
            this.label_cur_b_freq.AutoSize = true;
            this.label_cur_b_freq.Enabled = false;
            this.label_cur_b_freq.Location = new System.Drawing.Point(70, 50);
            this.label_cur_b_freq.Name = "label_cur_b_freq";
            this.label_cur_b_freq.Size = new System.Drawing.Size(35, 13);
            this.label_cur_b_freq.TabIndex = 2;
            this.label_cur_b_freq.Text = "f -- Hz";
            this.label_cur_b_freq.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_a_phase
            // 
            this.label_cur_a_phase.AutoSize = true;
            this.label_cur_a_phase.Enabled = false;
            this.label_cur_a_phase.Location = new System.Drawing.Point(2, 65);
            this.label_cur_a_phase.Name = "label_cur_a_phase";
            this.label_cur_a_phase.Size = new System.Drawing.Size(38, 13);
            this.label_cur_a_phase.TabIndex = 2;
            this.label_cur_a_phase.Text = "Phi -- °";
            this.label_cur_a_phase.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_b_ampl
            // 
            this.label_cur_b_ampl.AutoSize = true;
            this.label_cur_b_ampl.Enabled = false;
            this.label_cur_b_ampl.Location = new System.Drawing.Point(70, 35);
            this.label_cur_b_ampl.Margin = new System.Windows.Forms.Padding(0);
            this.label_cur_b_ampl.Name = "label_cur_b_ampl";
            this.label_cur_b_ampl.Size = new System.Drawing.Size(39, 13);
            this.label_cur_b_ampl.TabIndex = 2;
            this.label_cur_b_ampl.Text = "A -- dB";
            this.label_cur_b_ampl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_a_freq
            // 
            this.label_cur_a_freq.AutoSize = true;
            this.label_cur_a_freq.Enabled = false;
            this.label_cur_a_freq.Location = new System.Drawing.Point(2, 50);
            this.label_cur_a_freq.Name = "label_cur_a_freq";
            this.label_cur_a_freq.Size = new System.Drawing.Size(35, 13);
            this.label_cur_a_freq.TabIndex = 2;
            this.label_cur_a_freq.Text = "f -- Hz";
            this.label_cur_a_freq.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label_cur_a_ampl
            // 
            this.label_cur_a_ampl.AutoSize = true;
            this.label_cur_a_ampl.Enabled = false;
            this.label_cur_a_ampl.Location = new System.Drawing.Point(2, 35);
            this.label_cur_a_ampl.Margin = new System.Windows.Forms.Padding(0);
            this.label_cur_a_ampl.Name = "label_cur_a_ampl";
            this.label_cur_a_ampl.Size = new System.Drawing.Size(39, 13);
            this.label_cur_a_ampl.TabIndex = 2;
            this.label_cur_a_ampl.Text = "A -- dB";
            this.label_cur_a_ampl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Enabled = false;
            this.label6.Location = new System.Drawing.Point(70, 17);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(16, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "2:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Enabled = false;
            this.label5.Location = new System.Drawing.Point(2, 17);
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
            this.trackBar_ver_cur_a.Maximum = 10000;
            this.trackBar_ver_cur_a.Minimum = 1;
            this.trackBar_ver_cur_a.Name = "trackBar_ver_cur_a";
            this.trackBar_ver_cur_a.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_ver_cur_a.Size = new System.Drawing.Size(23, 268);
            this.trackBar_ver_cur_a.TabIndex = 0;
            this.trackBar_ver_cur_a.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_ver_cur_a.Value = 1;
            this.trackBar_ver_cur_a.ValueChanged += new System.EventHandler(this.trackBar_ver_cur_a_ValueChanged);
            // 
            // trackBar_ver_cur_b
            // 
            this.trackBar_ver_cur_b.AutoSize = false;
            this.trackBar_ver_cur_b.Dock = System.Windows.Forms.DockStyle.Right;
            this.trackBar_ver_cur_b.Enabled = false;
            this.trackBar_ver_cur_b.Location = new System.Drawing.Point(162, 16);
            this.trackBar_ver_cur_b.Maximum = 10000;
            this.trackBar_ver_cur_b.Minimum = 1;
            this.trackBar_ver_cur_b.Name = "trackBar_ver_cur_b";
            this.trackBar_ver_cur_b.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar_ver_cur_b.Size = new System.Drawing.Size(23, 268);
            this.trackBar_ver_cur_b.TabIndex = 0;
            this.trackBar_ver_cur_b.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_ver_cur_b.Value = 1;
            this.trackBar_ver_cur_b.ValueChanged += new System.EventHandler(this.trackBar_ver_cur_b_ValueChanged);
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
            this.trackBar1.Enabled = false;
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
            this.trackBar2.Enabled = false;
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
            this.groupBox_frequency.Controls.Add(this.radioButton_log);
            this.groupBox_frequency.Controls.Add(this.radioButton_lin);
            this.groupBox_frequency.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_frequency.Location = new System.Drawing.Point(3, 3);
            this.groupBox_frequency.Name = "groupBox_frequency";
            this.groupBox_frequency.Size = new System.Drawing.Size(209, 108);
            this.groupBox_frequency.TabIndex = 0;
            this.groupBox_frequency.TabStop = false;
            this.groupBox_frequency.Text = "Frequency range";
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "f min";
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
            // radioButton_log
            // 
            this.radioButton_log.AutoSize = true;
            this.radioButton_log.Checked = true;
            this.radioButton_log.Location = new System.Drawing.Point(100, 85);
            this.radioButton_log.Name = "radioButton_log";
            this.radioButton_log.Size = new System.Drawing.Size(43, 17);
            this.radioButton_log.TabIndex = 0;
            this.radioButton_log.TabStop = true;
            this.radioButton_log.Text = "Log";
            this.radioButton_log.UseVisualStyleBackColor = true;
            // 
            // radioButton_lin
            // 
            this.radioButton_lin.AutoSize = true;
            this.radioButton_lin.Location = new System.Drawing.Point(149, 85);
            this.radioButton_lin.Name = "radioButton_lin";
            this.radioButton_lin.Size = new System.Drawing.Size(54, 17);
            this.radioButton_lin.TabIndex = 0;
            this.radioButton_lin.Text = "Linear";
            this.radioButton_lin.UseVisualStyleBackColor = true;
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
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(114, 55);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(22, 13);
            this.label12.TabIndex = 14;
            this.label12.Text = "mV";
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
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(114, 25);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(22, 13);
            this.label13.TabIndex = 13;
            this.label13.Text = "mV";
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
            // groupBox_measurement
            // 
            this.groupBox_measurement.Controls.Add(this.label_info);
            this.groupBox_measurement.Controls.Add(this.label17);
            this.groupBox_measurement.Controls.Add(this.label16);
            this.groupBox_measurement.Controls.Add(this.radioButton_avg2);
            this.groupBox_measurement.Controls.Add(this.radioButton_avg8);
            this.groupBox_measurement.Controls.Add(this.radioButton_avg4);
            this.groupBox_measurement.Controls.Add(this.radioButton_avg1);
            this.groupBox_measurement.Controls.Add(this.button_run);
            this.groupBox_measurement.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_measurement.Location = new System.Drawing.Point(656, 302);
            this.groupBox_measurement.Name = "groupBox_measurement";
            this.groupBox_measurement.Size = new System.Drawing.Size(194, 114);
            this.groupBox_measurement.TabIndex = 5;
            this.groupBox_measurement.TabStop = false;
            this.groupBox_measurement.Text = "Measurement";
            // 
            // label_info
            // 
            this.label_info.AutoSize = true;
            this.label_info.Location = new System.Drawing.Point(79, 72);
            this.label_info.Name = "label_info";
            this.label_info.Size = new System.Drawing.Size(98, 13);
            this.label_info.TabIndex = 3;
            this.label_info.Text = "Estimated time: 50s";
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
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(9, 17);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 13);
            this.label16.TabIndex = 2;
            this.label16.Text = "Averaging";
            // 
            // radioButton_avg2
            // 
            this.radioButton_avg2.AutoSize = true;
            this.radioButton_avg2.Enabled = false;
            this.radioButton_avg2.Location = new System.Drawing.Point(12, 53);
            this.radioButton_avg2.Name = "radioButton_avg2";
            this.radioButton_avg2.Size = new System.Drawing.Size(36, 17);
            this.radioButton_avg2.TabIndex = 1;
            this.radioButton_avg2.Text = "2x";
            this.radioButton_avg2.UseVisualStyleBackColor = true;
            // 
            // radioButton_avg8
            // 
            this.radioButton_avg8.AutoSize = true;
            this.radioButton_avg8.Enabled = false;
            this.radioButton_avg8.Location = new System.Drawing.Point(12, 93);
            this.radioButton_avg8.Name = "radioButton_avg8";
            this.radioButton_avg8.Size = new System.Drawing.Size(36, 17);
            this.radioButton_avg8.TabIndex = 1;
            this.radioButton_avg8.Text = "8x";
            this.radioButton_avg8.UseVisualStyleBackColor = true;
            // 
            // radioButton_avg4
            // 
            this.radioButton_avg4.AutoSize = true;
            this.radioButton_avg4.Enabled = false;
            this.radioButton_avg4.Location = new System.Drawing.Point(12, 73);
            this.radioButton_avg4.Name = "radioButton_avg4";
            this.radioButton_avg4.Size = new System.Drawing.Size(36, 17);
            this.radioButton_avg4.TabIndex = 1;
            this.radioButton_avg4.Text = "4x";
            this.radioButton_avg4.UseVisualStyleBackColor = true;
            // 
            // radioButton_avg1
            // 
            this.radioButton_avg1.AutoSize = true;
            this.radioButton_avg1.Checked = true;
            this.radioButton_avg1.Location = new System.Drawing.Point(12, 33);
            this.radioButton_avg1.Name = "radioButton_avg1";
            this.radioButton_avg1.Size = new System.Drawing.Size(36, 17);
            this.radioButton_avg1.TabIndex = 1;
            this.radioButton_avg1.TabStop = true;
            this.radioButton_avg1.Text = "1x";
            this.radioButton_avg1.UseVisualStyleBackColor = true;
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
            this.groupBox_cursors_vertical.ResumeLayout(false);
            this.groupBox_cursors_vertical.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ver_cur_a)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_ver_cur_b)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel5.ResumeLayout(false);
            this.tableLayoutPanel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            this.groupBox_frequency.ResumeLayout(false);
            this.groupBox_frequency.PerformLayout();
            this.groupBox_signal_params.ResumeLayout(false);
            this.groupBox_signal_params.PerformLayout();
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
        private System.Windows.Forms.RadioButton radioButton_ver_cur_on;
        private System.Windows.Forms.RadioButton radioButton_ver_cur_off;
        private System.Windows.Forms.Label label_cur_a_freq;
        private System.Windows.Forms.Label label_cur_a_ampl;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TrackBar trackBar_ver_cur_a;
        private System.Windows.Forms.TrackBar trackBar_ver_cur_b;
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
        private System.Windows.Forms.RadioButton radioButton_log;
        private System.Windows.Forms.RadioButton radioButton_lin;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBox_amplitude;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBox_offset;
        private System.Windows.Forms.Label label_info;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.RadioButton radioButton_avg2;
        private System.Windows.Forms.RadioButton radioButton_avg8;
        private System.Windows.Forms.RadioButton radioButton_avg4;
        private System.Windows.Forms.RadioButton radioButton_avg1;
        private System.Windows.Forms.Label label_cur_a_phase;
        private System.Windows.Forms.Label label_cur_dphi;
        private System.Windows.Forms.Label label_cur_df;
        private System.Windows.Forms.Label label_cur_da;
        private System.Windows.Forms.Label label_cur_b_phase;
        private System.Windows.Forms.Label label_cur_b_freq;
        private System.Windows.Forms.Label label_cur_b_ampl;
    }
}