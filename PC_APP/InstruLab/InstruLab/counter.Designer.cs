namespace LEO
{
    partial class counter
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
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tabPage_REF = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label_cnt_ref_value = new System.Windows.Forms.Label();
            this.tabPage_IC = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cnt_ic2_buffRecalc_textBox = new System.Windows.Forms.TextBox();
            this.cnt_ic1_buffRecalc_textBox = new System.Windows.Forms.TextBox();
            this.cnt_ic2_buffer_textBox = new System.Windows.Forms.TextBox();
            this.cnt_ic1_buffer_textBox = new System.Windows.Forms.TextBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.label_cnt_ic2_value = new System.Windows.Forms.Label();
            this.cnt_ic2_label = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label_cnt_ic1_value = new System.Windows.Forms.Label();
            this.cnt_ic1_label = new System.Windows.Forms.Label();
            this.tabPage_ETR = new System.Windows.Forms.TabPage();
            this.avgr_groupBox = new System.Windows.Forms.GroupBox();
            this.cnt_etr_avrg_textBox = new System.Windows.Forms.TextBox();
            this.cnt_etr_trackBar = new System.Windows.Forms.TrackBar();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton_10s = new System.Windows.Forms.RadioButton();
            this.radioButton_1s = new System.Windows.Forms.RadioButton();
            this.radioButton_100m = new System.Windows.Forms.RadioButton();
            this.radioButton_10m = new System.Windows.Forms.RadioButton();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label_cnt_etr_avrg = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label_cnt_etr_value = new System.Windows.Forms.Label();
            this.cnt_mode_tabControl = new System.Windows.Forms.TabControl();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.tabPage_REF.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPage_IC.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPage_ETR.SuspendLayout();
            this.avgr_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cnt_etr_trackBar)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.cnt_mode_tabControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(690, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // tabPage_REF
            // 
            this.tabPage_REF.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage_REF.Controls.Add(this.groupBox3);
            this.tabPage_REF.Controls.Add(this.label5);
            this.tabPage_REF.Controls.Add(this.label_cnt_ref_value);
            this.tabPage_REF.Location = new System.Drawing.Point(4, 22);
            this.tabPage_REF.Name = "tabPage_REF";
            this.tabPage_REF.Size = new System.Drawing.Size(682, 127);
            this.tabPage_REF.TabIndex = 2;
            this.tabPage_REF.Text = "REF";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.textBox1);
            this.groupBox3.Location = new System.Drawing.Point(569, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(105, 84);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Sample count";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(7, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(92, 20);
            this.textBox1.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label5.Location = new System.Drawing.Point(6, 6);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(127, 21);
            this.label5.TabIndex = 1;
            this.label5.Text = "Frequency ratio";
            // 
            // label_cnt_ref_value
            // 
            this.label_cnt_ref_value.AutoSize = true;
            this.label_cnt_ref_value.Font = new System.Drawing.Font("Times New Roman", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_cnt_ref_value.Location = new System.Drawing.Point(159, 48);
            this.label_cnt_ref_value.Name = "label_cnt_ref_value";
            this.label_cnt_ref_value.Size = new System.Drawing.Size(122, 42);
            this.label_cnt_ref_value.TabIndex = 0;
            this.label_cnt_ref_value.Text = "0.0000";
            // 
            // tabPage_IC
            // 
            this.tabPage_IC.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage_IC.Controls.Add(this.groupBox2);
            this.tabPage_IC.Controls.Add(this.groupBox5);
            this.tabPage_IC.Controls.Add(this.groupBox4);
            this.tabPage_IC.Location = new System.Drawing.Point(4, 22);
            this.tabPage_IC.Name = "tabPage_IC";
            this.tabPage_IC.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_IC.Size = new System.Drawing.Size(682, 127);
            this.tabPage_IC.TabIndex = 1;
            this.tabPage_IC.Text = "IC";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cnt_ic2_buffRecalc_textBox);
            this.groupBox2.Controls.Add(this.cnt_ic1_buffRecalc_textBox);
            this.groupBox2.Controls.Add(this.cnt_ic2_buffer_textBox);
            this.groupBox2.Controls.Add(this.cnt_ic1_buffer_textBox);
            this.groupBox2.Location = new System.Drawing.Point(471, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(208, 121);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Sample count";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(112, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(70, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Recalculated";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(112, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Recalculated";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "IC2 buffer";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "IC1 buffer";
            // 
            // cnt_ic2_buffRecalc_textBox
            // 
            this.cnt_ic2_buffRecalc_textBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cnt_ic2_buffRecalc_textBox.Enabled = false;
            this.cnt_ic2_buffRecalc_textBox.Location = new System.Drawing.Point(115, 88);
            this.cnt_ic2_buffRecalc_textBox.Name = "cnt_ic2_buffRecalc_textBox";
            this.cnt_ic2_buffRecalc_textBox.Size = new System.Drawing.Size(84, 20);
            this.cnt_ic2_buffRecalc_textBox.TabIndex = 3;
            this.cnt_ic2_buffRecalc_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cnt_ic1_buffRecalc_textBox
            // 
            this.cnt_ic1_buffRecalc_textBox.BackColor = System.Drawing.SystemColors.ControlLight;
            this.cnt_ic1_buffRecalc_textBox.Enabled = false;
            this.cnt_ic1_buffRecalc_textBox.Location = new System.Drawing.Point(115, 38);
            this.cnt_ic1_buffRecalc_textBox.Name = "cnt_ic1_buffRecalc_textBox";
            this.cnt_ic1_buffRecalc_textBox.Size = new System.Drawing.Size(84, 20);
            this.cnt_ic1_buffRecalc_textBox.TabIndex = 2;
            this.cnt_ic1_buffRecalc_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // cnt_ic2_buffer_textBox
            // 
            this.cnt_ic2_buffer_textBox.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.cnt_ic2_buffer_textBox.Location = new System.Drawing.Point(13, 88);
            this.cnt_ic2_buffer_textBox.Name = "cnt_ic2_buffer_textBox";
            this.cnt_ic2_buffer_textBox.Size = new System.Drawing.Size(79, 20);
            this.cnt_ic2_buffer_textBox.TabIndex = 1;
            this.cnt_ic2_buffer_textBox.Text = "4";
            this.cnt_ic2_buffer_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cnt_ic2_buffer_textBox.Click += new System.EventHandler(this.cnt_ic2_buffer_textBox_Click);
            this.cnt_ic2_buffer_textBox.Enter += new System.EventHandler(this.cnt_ic2_buffer_textBox_Enter);
            this.cnt_ic2_buffer_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cnt_ic2_buffer_textBox_KeyPress);
            this.cnt_ic2_buffer_textBox.Leave += new System.EventHandler(this.cnt_ic2_buffer_textBox_Leave);
            // 
            // cnt_ic1_buffer_textBox
            // 
            this.cnt_ic1_buffer_textBox.ForeColor = System.Drawing.SystemColors.WindowFrame;
            this.cnt_ic1_buffer_textBox.Location = new System.Drawing.Point(13, 38);
            this.cnt_ic1_buffer_textBox.Name = "cnt_ic1_buffer_textBox";
            this.cnt_ic1_buffer_textBox.Size = new System.Drawing.Size(79, 20);
            this.cnt_ic1_buffer_textBox.TabIndex = 0;
            this.cnt_ic1_buffer_textBox.Text = "4";
            this.cnt_ic1_buffer_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cnt_ic1_buffer_textBox.Click += new System.EventHandler(this.cnt_ic1_buffer_textBox_Click);
            this.cnt_ic1_buffer_textBox.Enter += new System.EventHandler(this.cnt_ic1_buffer_textBox_Enter);
            this.cnt_ic1_buffer_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cnt_ic1_buffer_textBox_KeyPress);
            this.cnt_ic1_buffer_textBox.Leave += new System.EventHandler(this.cnt_ic1_buffer_textBox_Leave);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label_cnt_ic2_value);
            this.groupBox5.Controls.Add(this.cnt_ic2_label);
            this.groupBox5.Location = new System.Drawing.Point(3, 60);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(462, 64);
            this.groupBox5.TabIndex = 6;
            this.groupBox5.TabStop = false;
            // 
            // label_cnt_ic2_value
            // 
            this.label_cnt_ic2_value.AutoSize = true;
            this.label_cnt_ic2_value.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_cnt_ic2_value.Location = new System.Drawing.Point(193, 19);
            this.label_cnt_ic2_value.Name = "label_cnt_ic2_value";
            this.label_cnt_ic2_value.Size = new System.Drawing.Size(103, 36);
            this.label_cnt_ic2_value.TabIndex = 2;
            this.label_cnt_ic2_value.Text = "0.0000";
            // 
            // cnt_ic2_label
            // 
            this.cnt_ic2_label.AutoSize = true;
            this.cnt_ic2_label.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cnt_ic2_label.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cnt_ic2_label.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.cnt_ic2_label.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cnt_ic2_label.Location = new System.Drawing.Point(0, 0);
            this.cnt_ic2_label.Name = "cnt_ic2_label";
            this.cnt_ic2_label.Size = new System.Drawing.Size(185, 19);
            this.cnt_ic2_label.TabIndex = 4;
            this.cnt_ic2_label.Text = "Input Capture channel 2 [Hz]";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label_cnt_ic1_value);
            this.groupBox4.Controls.Add(this.cnt_ic1_label);
            this.groupBox4.Location = new System.Drawing.Point(3, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(462, 67);
            this.groupBox4.TabIndex = 5;
            this.groupBox4.TabStop = false;
            // 
            // label_cnt_ic1_value
            // 
            this.label_cnt_ic1_value.AutoSize = true;
            this.label_cnt_ic1_value.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_cnt_ic1_value.Location = new System.Drawing.Point(193, 18);
            this.label_cnt_ic1_value.Name = "label_cnt_ic1_value";
            this.label_cnt_ic1_value.Size = new System.Drawing.Size(103, 36);
            this.label_cnt_ic1_value.TabIndex = 1;
            this.label_cnt_ic1_value.Text = "0.0000";
            // 
            // cnt_ic1_label
            // 
            this.cnt_ic1_label.AutoSize = true;
            this.cnt_ic1_label.BackColor = System.Drawing.Color.WhiteSmoke;
            this.cnt_ic1_label.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cnt_ic1_label.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.cnt_ic1_label.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cnt_ic1_label.Location = new System.Drawing.Point(0, 0);
            this.cnt_ic1_label.Name = "cnt_ic1_label";
            this.cnt_ic1_label.Size = new System.Drawing.Size(185, 19);
            this.cnt_ic1_label.TabIndex = 3;
            this.cnt_ic1_label.Text = "Input Capture channel 1 [Hz]";
            // 
            // tabPage_ETR
            // 
            this.tabPage_ETR.BackColor = System.Drawing.Color.WhiteSmoke;
            this.tabPage_ETR.Controls.Add(this.avgr_groupBox);
            this.tabPage_ETR.Controls.Add(this.groupBox1);
            this.tabPage_ETR.Controls.Add(this.groupBox7);
            this.tabPage_ETR.Controls.Add(this.groupBox6);
            this.tabPage_ETR.Location = new System.Drawing.Point(4, 22);
            this.tabPage_ETR.Name = "tabPage_ETR";
            this.tabPage_ETR.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_ETR.Size = new System.Drawing.Size(682, 127);
            this.tabPage_ETR.TabIndex = 0;
            this.tabPage_ETR.Text = "ETR";
            // 
            // avgr_groupBox
            // 
            this.avgr_groupBox.Controls.Add(this.cnt_etr_avrg_textBox);
            this.avgr_groupBox.Controls.Add(this.cnt_etr_trackBar);
            this.avgr_groupBox.Location = new System.Drawing.Point(438, 3);
            this.avgr_groupBox.Name = "avgr_groupBox";
            this.avgr_groupBox.Size = new System.Drawing.Size(149, 121);
            this.avgr_groupBox.TabIndex = 4;
            this.avgr_groupBox.TabStop = false;
            this.avgr_groupBox.Text = "Averaging";
            // 
            // cnt_etr_avrg_textBox
            // 
            this.cnt_etr_avrg_textBox.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.cnt_etr_avrg_textBox.Location = new System.Drawing.Point(35, 79);
            this.cnt_etr_avrg_textBox.Name = "cnt_etr_avrg_textBox";
            this.cnt_etr_avrg_textBox.Size = new System.Drawing.Size(83, 20);
            this.cnt_etr_avrg_textBox.TabIndex = 1;
            this.cnt_etr_avrg_textBox.Text = "Enter a number";
            this.cnt_etr_avrg_textBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cnt_etr_avrg_textBox.Click += new System.EventHandler(this.cnt_etr_avrg_textBox_Click);
            this.cnt_etr_avrg_textBox.Enter += new System.EventHandler(this.cnt_etr_avrg_textBox_Enter);
            this.cnt_etr_avrg_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cnt_etr_avrg_textBox_KeyPress);
            this.cnt_etr_avrg_textBox.Leave += new System.EventHandler(this.cnt_etr_avrg_textBox_Leave);
            // 
            // cnt_etr_trackBar
            // 
            this.cnt_etr_trackBar.LargeChange = 100;
            this.cnt_etr_trackBar.Location = new System.Drawing.Point(6, 28);
            this.cnt_etr_trackBar.Maximum = 10000;
            this.cnt_etr_trackBar.Minimum = 2;
            this.cnt_etr_trackBar.Name = "cnt_etr_trackBar";
            this.cnt_etr_trackBar.Size = new System.Drawing.Size(137, 45);
            this.cnt_etr_trackBar.SmallChange = 10;
            this.cnt_etr_trackBar.TabIndex = 0;
            this.cnt_etr_trackBar.Value = 2;
            this.cnt_etr_trackBar.Scroll += new System.EventHandler(this.cnt_etr_trackBar_Scroll);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.radioButton_10s);
            this.groupBox1.Controls.Add(this.radioButton_1s);
            this.groupBox1.Controls.Add(this.radioButton_100m);
            this.groupBox1.Controls.Add(this.radioButton_10m);
            this.groupBox1.Location = new System.Drawing.Point(593, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(86, 120);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Gate time";
            // 
            // radioButton_10s
            // 
            this.radioButton_10s.AutoSize = true;
            this.radioButton_10s.Location = new System.Drawing.Point(15, 94);
            this.radioButton_10s.Name = "radioButton_10s";
            this.radioButton_10s.Size = new System.Drawing.Size(57, 17);
            this.radioButton_10s.TabIndex = 0;
            this.radioButton_10s.Text = "10 sec";
            this.radioButton_10s.UseVisualStyleBackColor = true;
            this.radioButton_10s.CheckedChanged += new System.EventHandler(this.radioButton_10s_CheckedChanged);
            // 
            // radioButton_1s
            // 
            this.radioButton_1s.AutoSize = true;
            this.radioButton_1s.Checked = true;
            this.radioButton_1s.Location = new System.Drawing.Point(15, 71);
            this.radioButton_1s.Name = "radioButton_1s";
            this.radioButton_1s.Size = new System.Drawing.Size(51, 17);
            this.radioButton_1s.TabIndex = 0;
            this.radioButton_1s.TabStop = true;
            this.radioButton_1s.Text = "1 sec";
            this.radioButton_1s.UseVisualStyleBackColor = true;
            this.radioButton_1s.CheckedChanged += new System.EventHandler(this.radioButton_1s_CheckedChanged);
            // 
            // radioButton_100m
            // 
            this.radioButton_100m.AutoSize = true;
            this.radioButton_100m.Location = new System.Drawing.Point(15, 48);
            this.radioButton_100m.Name = "radioButton_100m";
            this.radioButton_100m.Size = new System.Drawing.Size(54, 17);
            this.radioButton_100m.TabIndex = 0;
            this.radioButton_100m.Text = "100 m";
            this.radioButton_100m.UseVisualStyleBackColor = true;
            this.radioButton_100m.CheckedChanged += new System.EventHandler(this.radioButton_100m_CheckedChanged);
            // 
            // radioButton_10m
            // 
            this.radioButton_10m.AutoSize = true;
            this.radioButton_10m.Location = new System.Drawing.Point(15, 24);
            this.radioButton_10m.Name = "radioButton_10m";
            this.radioButton_10m.Size = new System.Drawing.Size(48, 17);
            this.radioButton_10m.TabIndex = 0;
            this.radioButton_10m.Text = "10 m";
            this.radioButton_10m.UseVisualStyleBackColor = true;
            this.radioButton_10m.CheckedChanged += new System.EventHandler(this.radioButton_10m_CheckedChanged);
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label_cnt_etr_avrg);
            this.groupBox7.Controls.Add(this.label7);
            this.groupBox7.Location = new System.Drawing.Point(4, 60);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(428, 64);
            this.groupBox7.TabIndex = 8;
            this.groupBox7.TabStop = false;
            // 
            // label_cnt_etr_avrg
            // 
            this.label_cnt_etr_avrg.AutoSize = true;
            this.label_cnt_etr_avrg.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_cnt_etr_avrg.Location = new System.Drawing.Point(192, 19);
            this.label_cnt_etr_avrg.Name = "label_cnt_etr_avrg";
            this.label_cnt_etr_avrg.Size = new System.Drawing.Size(103, 36);
            this.label_cnt_etr_avrg.TabIndex = 6;
            this.label_cnt_etr_avrg.Text = "0.0000";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label7.Location = new System.Drawing.Point(-1, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(153, 19);
            this.label7.TabIndex = 5;
            this.label7.Text = "Average frequency [Hz]";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.label_cnt_etr_value);
            this.groupBox6.Location = new System.Drawing.Point(4, 3);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(428, 62);
            this.groupBox6.TabIndex = 7;
            this.groupBox6.TabStop = false;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Times New Roman", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.label6.Location = new System.Drawing.Point(-1, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 19);
            this.label6.TabIndex = 3;
            this.label6.Text = "Frequency [Hz]";
            // 
            // label_cnt_etr_value
            // 
            this.label_cnt_etr_value.AutoSize = true;
            this.label_cnt_etr_value.Font = new System.Drawing.Font("Times New Roman", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label_cnt_etr_value.Location = new System.Drawing.Point(192, 18);
            this.label_cnt_etr_value.Name = "label_cnt_etr_value";
            this.label_cnt_etr_value.Size = new System.Drawing.Size(103, 36);
            this.label_cnt_etr_value.TabIndex = 1;
            this.label_cnt_etr_value.Text = "0.0000";
            this.label_cnt_etr_value.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cnt_mode_tabControl
            // 
            this.cnt_mode_tabControl.Controls.Add(this.tabPage_ETR);
            this.cnt_mode_tabControl.Controls.Add(this.tabPage_IC);
            this.cnt_mode_tabControl.Controls.Add(this.tabPage_REF);
            this.cnt_mode_tabControl.Cursor = System.Windows.Forms.Cursors.Default;
            this.cnt_mode_tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cnt_mode_tabControl.Location = new System.Drawing.Point(0, 24);
            this.cnt_mode_tabControl.Name = "cnt_mode_tabControl";
            this.cnt_mode_tabControl.SelectedIndex = 0;
            this.cnt_mode_tabControl.Size = new System.Drawing.Size(690, 153);
            this.cnt_mode_tabControl.TabIndex = 1;
            this.cnt_mode_tabControl.Selecting += new System.Windows.Forms.TabControlCancelEventHandler(this.cnt_mode_tabControl_SelectedIndexchanged);
            // 
            // counter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(690, 177);
            this.Controls.Add(this.cnt_mode_tabControl);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "counter";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Text = "counter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.counter_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabPage_REF.ResumeLayout(false);
            this.tabPage_REF.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPage_IC.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.tabPage_ETR.ResumeLayout(false);
            this.avgr_groupBox.ResumeLayout(false);
            this.avgr_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.cnt_etr_trackBar)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.cnt_mode_tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage_REF;
        private System.Windows.Forms.TabPage tabPage_IC;
        private System.Windows.Forms.Label cnt_ic2_label;
        private System.Windows.Forms.Label cnt_ic1_label;
        private System.Windows.Forms.Label label_cnt_ic2_value;
        private System.Windows.Forms.Label label_cnt_ic1_value;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox cnt_ic2_buffer_textBox;
        private System.Windows.Forms.TextBox cnt_ic1_buffer_textBox;
        private System.Windows.Forms.TabPage tabPage_ETR;
        private System.Windows.Forms.Label label_cnt_etr_value;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButton_10s;
        private System.Windows.Forms.RadioButton radioButton_1s;
        private System.Windows.Forms.RadioButton radioButton_100m;
        private System.Windows.Forms.RadioButton radioButton_10m;
        private System.Windows.Forms.TabControl cnt_mode_tabControl;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox cnt_ic2_buffRecalc_textBox;
        private System.Windows.Forms.TextBox cnt_ic1_buffRecalc_textBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label_cnt_ref_value;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.GroupBox avgr_groupBox;
        private System.Windows.Forms.TextBox cnt_etr_avrg_textBox;
        private System.Windows.Forms.TrackBar cnt_etr_trackBar;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label_cnt_etr_avrg;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.GroupBox groupBox7;
    }
}