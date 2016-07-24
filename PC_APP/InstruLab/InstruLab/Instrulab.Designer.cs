namespace InstruLab
{
    partial class Instrulab
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Instrulab));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox_device_info = new System.Windows.Forms.GroupBox();
            this.label_device = new System.Windows.Forms.Label();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox_general = new System.Windows.Forms.GroupBox();
            this.label42 = new System.Windows.Forms.Label();
            this.label_MCU = new System.Windows.Forms.Label();
            this.label_con4 = new System.Windows.Forms.Label();
            this.label_con3 = new System.Windows.Forms.Label();
            this.label_con2 = new System.Windows.Forms.Label();
            this.label_fw = new System.Windows.Forms.Label();
            this.label_RTOS = new System.Windows.Forms.Label();
            this.label_HAL = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label_con1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label_Freq = new System.Windows.Forms.Label();
            this.groupBox_scope = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.btn_scope_open = new System.Windows.Forms.Button();
            this.label_scope_smpl = new System.Windows.Forms.Label();
            this.label_scope_pins = new System.Windows.Forms.Label();
            this.label_scope_vref = new System.Windows.Forms.Label();
            this.label_scope_channs = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label_scope_buff_len = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.groupBox_generator = new System.Windows.Forms.GroupBox();
            this.label52 = new System.Windows.Forms.Label();
            this.btn_gen_open = new System.Windows.Forms.Button();
            this.label_gen_smpl = new System.Windows.Forms.Label();
            this.label_gen_data_depth = new System.Windows.Forms.Label();
            this.label_gen_pins = new System.Windows.Forms.Label();
            this.label_gen_channs = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.label_gen_vref = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label_gen_buff_len = new System.Windows.Forms.Label();
            this.btn_connect = new System.Windows.Forms.Button();
            this.btn_scan = new System.Windows.Forms.Button();
            this.listBox_devices = new System.Windows.Forms.ListBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel_color = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitInstrulabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.uARTSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox1 = new System.Windows.Forms.ToolStripTextBox();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.sendUsFeedbackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            this.groupBox_device_info.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.groupBox_general.SuspendLayout();
            this.groupBox_scope.SuspendLayout();
            this.groupBox_generator.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.panel2);
            this.panel1.Controls.Add(this.groupBox_device_info);
            this.panel1.Controls.Add(this.btn_connect);
            this.panel1.Controls.Add(this.btn_scan);
            this.panel1.Controls.Add(this.listBox_devices);
            this.panel1.Location = new System.Drawing.Point(2, 22);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(595, 340);
            this.panel1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel2.BackgroundImage")));
            this.panel2.Location = new System.Drawing.Point(430, 200);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(161, 109);
            this.panel2.TabIndex = 4;
            // 
            // groupBox_device_info
            // 
            this.groupBox_device_info.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox_device_info.Controls.Add(this.label_device);
            this.groupBox_device_info.Controls.Add(this.tableLayoutPanel8);
            this.groupBox_device_info.Location = new System.Drawing.Point(6, 8);
            this.groupBox_device_info.Margin = new System.Windows.Forms.Padding(2);
            this.groupBox_device_info.Name = "groupBox_device_info";
            this.groupBox_device_info.Padding = new System.Windows.Forms.Padding(2);
            this.groupBox_device_info.Size = new System.Drawing.Size(420, 326);
            this.groupBox_device_info.TabIndex = 3;
            this.groupBox_device_info.TabStop = false;
            this.groupBox_device_info.Text = "Device info";
            // 
            // label_device
            // 
            this.label_device.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label_device.Font = new System.Drawing.Font("Microsoft Sans Serif", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_device.Location = new System.Drawing.Point(4, 15);
            this.label_device.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label_device.Name = "label_device";
            this.label_device.Size = new System.Drawing.Size(410, 28);
            this.label_device.TabIndex = 3;
            this.label_device.Text = "No device connected";
            this.label_device.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel8.ColumnCount = 3;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanel8.Controls.Add(this.groupBox_general, 0, 0);
            this.tableLayoutPanel8.Controls.Add(this.groupBox_scope, 2, 0);
            this.tableLayoutPanel8.Controls.Add(this.groupBox_generator, 1, 0);
            this.tableLayoutPanel8.Location = new System.Drawing.Point(5, 46);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 274F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(410, 274);
            this.tableLayoutPanel8.TabIndex = 2;
            // 
            // groupBox_general
            // 
            this.groupBox_general.Controls.Add(this.label42);
            this.groupBox_general.Controls.Add(this.label_MCU);
            this.groupBox_general.Controls.Add(this.label_con4);
            this.groupBox_general.Controls.Add(this.label_con3);
            this.groupBox_general.Controls.Add(this.label_con2);
            this.groupBox_general.Controls.Add(this.label_fw);
            this.groupBox_general.Controls.Add(this.label_RTOS);
            this.groupBox_general.Controls.Add(this.label_HAL);
            this.groupBox_general.Controls.Add(this.label8);
            this.groupBox_general.Controls.Add(this.label_con1);
            this.groupBox_general.Controls.Add(this.label5);
            this.groupBox_general.Controls.Add(this.label29);
            this.groupBox_general.Controls.Add(this.label3);
            this.groupBox_general.Controls.Add(this.label35);
            this.groupBox_general.Controls.Add(this.label_Freq);
            this.groupBox_general.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_general.Location = new System.Drawing.Point(3, 3);
            this.groupBox_general.Name = "groupBox_general";
            this.groupBox_general.Size = new System.Drawing.Size(130, 268);
            this.groupBox_general.TabIndex = 0;
            this.groupBox_general.TabStop = false;
            this.groupBox_general.Text = "General";
            // 
            // label42
            // 
            this.label42.AutoSize = true;
            this.label42.Location = new System.Drawing.Point(6, 16);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(41, 13);
            this.label42.TabIndex = 0;
            this.label42.Text = "Device";
            // 
            // label_MCU
            // 
            this.label_MCU.AutoSize = true;
            this.label_MCU.Location = new System.Drawing.Point(14, 32);
            this.label_MCU.Name = "label_MCU";
            this.label_MCU.Size = new System.Drawing.Size(13, 13);
            this.label_MCU.TabIndex = 1;
            this.label_MCU.Text = "--";
            // 
            // label_con4
            // 
            this.label_con4.AutoSize = true;
            this.label_con4.Location = new System.Drawing.Point(14, 146);
            this.label_con4.Name = "label_con4";
            this.label_con4.Size = new System.Drawing.Size(0, 13);
            this.label_con4.TabIndex = 1;
            // 
            // label_con3
            // 
            this.label_con3.AutoSize = true;
            this.label_con3.Location = new System.Drawing.Point(14, 130);
            this.label_con3.Name = "label_con3";
            this.label_con3.Size = new System.Drawing.Size(0, 13);
            this.label_con3.TabIndex = 1;
            // 
            // label_con2
            // 
            this.label_con2.AutoSize = true;
            this.label_con2.Location = new System.Drawing.Point(14, 114);
            this.label_con2.Name = "label_con2";
            this.label_con2.Size = new System.Drawing.Size(0, 13);
            this.label_con2.TabIndex = 1;
            // 
            // label_fw
            // 
            this.label_fw.AutoSize = true;
            this.label_fw.Location = new System.Drawing.Point(14, 191);
            this.label_fw.Name = "label_fw";
            this.label_fw.Size = new System.Drawing.Size(13, 13);
            this.label_fw.TabIndex = 1;
            this.label_fw.Text = "--";
            // 
            // label_RTOS
            // 
            this.label_RTOS.AutoSize = true;
            this.label_RTOS.Location = new System.Drawing.Point(14, 220);
            this.label_RTOS.Name = "label_RTOS";
            this.label_RTOS.Size = new System.Drawing.Size(13, 13);
            this.label_RTOS.TabIndex = 1;
            this.label_RTOS.Text = "--";
            // 
            // label_HAL
            // 
            this.label_HAL.AutoSize = true;
            this.label_HAL.Location = new System.Drawing.Point(14, 248);
            this.label_HAL.Name = "label_HAL";
            this.label_HAL.Size = new System.Drawing.Size(13, 13);
            this.label_HAL.TabIndex = 1;
            this.label_HAL.Text = "--";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 177);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(87, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "Firmware Version";
            // 
            // label_con1
            // 
            this.label_con1.AutoSize = true;
            this.label_con1.Location = new System.Drawing.Point(14, 98);
            this.label_con1.Name = "label_con1";
            this.label_con1.Size = new System.Drawing.Size(13, 13);
            this.label_con1.TabIndex = 1;
            this.label_con1.Text = "--";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 206);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 13);
            this.label5.TabIndex = 0;
            this.label5.Text = "FreeRTOS";
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Location = new System.Drawing.Point(6, 49);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(79, 13);
            this.label29.TabIndex = 0;
            this.label29.Text = "Core frequency";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 234);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(45, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "ST HAL";
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(6, 81);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(61, 13);
            this.label35.TabIndex = 0;
            this.label35.Text = "Connection";
            // 
            // label_Freq
            // 
            this.label_Freq.AutoSize = true;
            this.label_Freq.Location = new System.Drawing.Point(14, 65);
            this.label_Freq.Name = "label_Freq";
            this.label_Freq.Size = new System.Drawing.Size(13, 13);
            this.label_Freq.TabIndex = 1;
            this.label_Freq.Text = "--";
            // 
            // groupBox_scope
            // 
            this.groupBox_scope.Controls.Add(this.label17);
            this.groupBox_scope.Controls.Add(this.btn_scope_open);
            this.groupBox_scope.Controls.Add(this.label_scope_smpl);
            this.groupBox_scope.Controls.Add(this.label_scope_pins);
            this.groupBox_scope.Controls.Add(this.label_scope_vref);
            this.groupBox_scope.Controls.Add(this.label_scope_channs);
            this.groupBox_scope.Controls.Add(this.label7);
            this.groupBox_scope.Controls.Add(this.label_scope_buff_len);
            this.groupBox_scope.Controls.Add(this.label9);
            this.groupBox_scope.Controls.Add(this.label4);
            this.groupBox_scope.Controls.Add(this.label22);
            this.groupBox_scope.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_scope.Location = new System.Drawing.Point(275, 3);
            this.groupBox_scope.Name = "groupBox_scope";
            this.groupBox_scope.Size = new System.Drawing.Size(132, 268);
            this.groupBox_scope.TabIndex = 3;
            this.groupBox_scope.TabStop = false;
            this.groupBox_scope.Text = "Oscilloscope";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 16);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(100, 13);
            this.label17.TabIndex = 0;
            this.label17.Text = "Sampling frequency";
            // 
            // btn_scope_open
            // 
            this.btn_scope_open.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_scope_open.Enabled = false;
            this.btn_scope_open.Location = new System.Drawing.Point(5, 242);
            this.btn_scope_open.Margin = new System.Windows.Forms.Padding(2);
            this.btn_scope_open.Name = "btn_scope_open";
            this.btn_scope_open.Size = new System.Drawing.Size(122, 20);
            this.btn_scope_open.TabIndex = 1;
            this.btn_scope_open.Text = "Open";
            this.btn_scope_open.UseVisualStyleBackColor = true;
            this.btn_scope_open.Click += new System.EventHandler(this.btn_scope_open_Click);
            // 
            // label_scope_smpl
            // 
            this.label_scope_smpl.AutoSize = true;
            this.label_scope_smpl.Location = new System.Drawing.Point(14, 32);
            this.label_scope_smpl.Name = "label_scope_smpl";
            this.label_scope_smpl.Size = new System.Drawing.Size(38, 13);
            this.label_scope_smpl.TabIndex = 1;
            this.label_scope_smpl.Text = "-- ksps";
            // 
            // label_scope_pins
            // 
            this.label_scope_pins.AutoSize = true;
            this.label_scope_pins.Location = new System.Drawing.Point(14, 162);
            this.label_scope_pins.Name = "label_scope_pins";
            this.label_scope_pins.Size = new System.Drawing.Size(13, 13);
            this.label_scope_pins.TabIndex = 1;
            this.label_scope_pins.Text = "--";
            // 
            // label_scope_vref
            // 
            this.label_scope_vref.AutoSize = true;
            this.label_scope_vref.Location = new System.Drawing.Point(14, 98);
            this.label_scope_vref.Name = "label_scope_vref";
            this.label_scope_vref.Size = new System.Drawing.Size(31, 13);
            this.label_scope_vref.TabIndex = 1;
            this.label_scope_vref.Text = "-- mV";
            // 
            // label_scope_channs
            // 
            this.label_scope_channs.AutoSize = true;
            this.label_scope_channs.Location = new System.Drawing.Point(14, 130);
            this.label_scope_channs.Name = "label_scope_channs";
            this.label_scope_channs.Size = new System.Drawing.Size(13, 13);
            this.label_scope_channs.TabIndex = 1;
            this.label_scope_channs.Text = "--";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 146);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 13);
            this.label7.TabIndex = 0;
            this.label7.Text = "Scope pin";
            // 
            // label_scope_buff_len
            // 
            this.label_scope_buff_len.AutoSize = true;
            this.label_scope_buff_len.Location = new System.Drawing.Point(14, 65);
            this.label_scope_buff_len.Name = "label_scope_buff_len";
            this.label_scope_buff_len.Size = new System.Drawing.Size(41, 13);
            this.label_scope_buff_len.TabIndex = 1;
            this.label_scope_buff_len.Text = "-- bytes";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 81);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(61, 13);
            this.label9.TabIndex = 0;
            this.label9.Text = "Voltage ref.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Channels";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 49);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(67, 13);
            this.label22.TabIndex = 0;
            this.label22.Text = "Buffer lenght";
            // 
            // groupBox_generator
            // 
            this.groupBox_generator.Controls.Add(this.label52);
            this.groupBox_generator.Controls.Add(this.btn_gen_open);
            this.groupBox_generator.Controls.Add(this.label_gen_smpl);
            this.groupBox_generator.Controls.Add(this.label_gen_data_depth);
            this.groupBox_generator.Controls.Add(this.label_gen_pins);
            this.groupBox_generator.Controls.Add(this.label_gen_channs);
            this.groupBox_generator.Controls.Add(this.label44);
            this.groupBox_generator.Controls.Add(this.label_gen_vref);
            this.groupBox_generator.Controls.Add(this.label45);
            this.groupBox_generator.Controls.Add(this.label2);
            this.groupBox_generator.Controls.Add(this.label48);
            this.groupBox_generator.Controls.Add(this.label46);
            this.groupBox_generator.Controls.Add(this.label_gen_buff_len);
            this.groupBox_generator.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_generator.Location = new System.Drawing.Point(139, 3);
            this.groupBox_generator.Name = "groupBox_generator";
            this.groupBox_generator.Size = new System.Drawing.Size(130, 268);
            this.groupBox_generator.TabIndex = 2;
            this.groupBox_generator.TabStop = false;
            this.groupBox_generator.Text = "Generator";
            // 
            // label52
            // 
            this.label52.AutoSize = true;
            this.label52.Location = new System.Drawing.Point(6, 16);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(100, 13);
            this.label52.TabIndex = 0;
            this.label52.Text = "Sampling frequency";
            // 
            // btn_gen_open
            // 
            this.btn_gen_open.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_gen_open.Enabled = false;
            this.btn_gen_open.Location = new System.Drawing.Point(8, 242);
            this.btn_gen_open.Margin = new System.Windows.Forms.Padding(2);
            this.btn_gen_open.Name = "btn_gen_open";
            this.btn_gen_open.Size = new System.Drawing.Size(117, 20);
            this.btn_gen_open.TabIndex = 1;
            this.btn_gen_open.Text = "Open";
            this.btn_gen_open.UseVisualStyleBackColor = true;
            this.btn_gen_open.Click += new System.EventHandler(this.btn_gen_open_Click);
            // 
            // label_gen_smpl
            // 
            this.label_gen_smpl.AutoSize = true;
            this.label_gen_smpl.Location = new System.Drawing.Point(14, 32);
            this.label_gen_smpl.Name = "label_gen_smpl";
            this.label_gen_smpl.Size = new System.Drawing.Size(38, 13);
            this.label_gen_smpl.TabIndex = 1;
            this.label_gen_smpl.Text = "-- ksps";
            // 
            // label_gen_data_depth
            // 
            this.label_gen_data_depth.AutoSize = true;
            this.label_gen_data_depth.Location = new System.Drawing.Point(14, 65);
            this.label_gen_data_depth.Name = "label_gen_data_depth";
            this.label_gen_data_depth.Size = new System.Drawing.Size(27, 13);
            this.label_gen_data_depth.TabIndex = 1;
            this.label_gen_data_depth.Text = "-- bit";
            // 
            // label_gen_pins
            // 
            this.label_gen_pins.AutoSize = true;
            this.label_gen_pins.Location = new System.Drawing.Point(14, 195);
            this.label_gen_pins.Name = "label_gen_pins";
            this.label_gen_pins.Size = new System.Drawing.Size(13, 13);
            this.label_gen_pins.TabIndex = 1;
            this.label_gen_pins.Text = "--";
            // 
            // label_gen_channs
            // 
            this.label_gen_channs.AutoSize = true;
            this.label_gen_channs.Location = new System.Drawing.Point(14, 162);
            this.label_gen_channs.Name = "label_gen_channs";
            this.label_gen_channs.Size = new System.Drawing.Size(13, 13);
            this.label_gen_channs.TabIndex = 1;
            this.label_gen_channs.Text = "--";
            // 
            // label44
            // 
            this.label44.AutoSize = true;
            this.label44.Location = new System.Drawing.Point(6, 81);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(67, 13);
            this.label44.TabIndex = 0;
            this.label44.Text = "Buffer lenght";
            // 
            // label_gen_vref
            // 
            this.label_gen_vref.AutoSize = true;
            this.label_gen_vref.Location = new System.Drawing.Point(14, 130);
            this.label_gen_vref.Name = "label_gen_vref";
            this.label_gen_vref.Size = new System.Drawing.Size(31, 13);
            this.label_gen_vref.TabIndex = 1;
            this.label_gen_vref.Text = "-- mV";
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.Location = new System.Drawing.Point(6, 49);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(60, 13);
            this.label45.TabIndex = 0;
            this.label45.Text = "Data depth";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 146);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(51, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Channels";
            // 
            // label48
            // 
            this.label48.AutoSize = true;
            this.label48.Location = new System.Drawing.Point(6, 179);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(71, 13);
            this.label48.TabIndex = 0;
            this.label48.Text = "Generator pin";
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.Location = new System.Drawing.Point(6, 114);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(61, 13);
            this.label46.TabIndex = 0;
            this.label46.Text = "Voltage ref.";
            // 
            // label_gen_buff_len
            // 
            this.label_gen_buff_len.AutoSize = true;
            this.label_gen_buff_len.Location = new System.Drawing.Point(14, 98);
            this.label_gen_buff_len.Name = "label_gen_buff_len";
            this.label_gen_buff_len.Size = new System.Drawing.Size(41, 13);
            this.label_gen_buff_len.TabIndex = 1;
            this.label_gen_buff_len.Text = "-- bytes";
            // 
            // btn_connect
            // 
            this.btn_connect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_connect.Enabled = false;
            this.btn_connect.Location = new System.Drawing.Point(430, 314);
            this.btn_connect.Margin = new System.Windows.Forms.Padding(2);
            this.btn_connect.Name = "btn_connect";
            this.btn_connect.Size = new System.Drawing.Size(82, 20);
            this.btn_connect.TabIndex = 1;
            this.btn_connect.Text = "Connect";
            this.btn_connect.UseVisualStyleBackColor = true;
            this.btn_connect.Click += new System.EventHandler(this.btn_connect_Click);
            // 
            // btn_scan
            // 
            this.btn_scan.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_scan.Location = new System.Drawing.Point(517, 314);
            this.btn_scan.Margin = new System.Windows.Forms.Padding(2);
            this.btn_scan.Name = "btn_scan";
            this.btn_scan.Size = new System.Drawing.Size(76, 20);
            this.btn_scan.TabIndex = 1;
            this.btn_scan.Text = "Scan";
            this.btn_scan.UseVisualStyleBackColor = true;
            this.btn_scan.Click += new System.EventHandler(this.btn_scan_Click);
            // 
            // listBox_devices
            // 
            this.listBox_devices.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBox_devices.FormattingEnabled = true;
            this.listBox_devices.Location = new System.Drawing.Point(430, 8);
            this.listBox_devices.Margin = new System.Windows.Forms.Padding(2);
            this.listBox_devices.Name = "listBox_devices";
            this.listBox_devices.Size = new System.Drawing.Size(161, 186);
            this.listBox_devices.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel_color,
            this.toolStripStatusLabel,
            this.toolStripProgressBar});
            this.statusStrip1.Location = new System.Drawing.Point(0, 362);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.statusStrip1.Size = new System.Drawing.Size(603, 25);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel_color
            // 
            this.toolStripStatusLabel_color.AutoSize = false;
            this.toolStripStatusLabel_color.BackColor = System.Drawing.Color.Red;
            this.toolStripStatusLabel_color.Name = "toolStripStatusLabel_color";
            this.toolStripStatusLabel_color.Size = new System.Drawing.Size(20, 20);
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.AutoSize = false;
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(240, 20);
            this.toolStripStatusLabel.Text = "Click scan to find available devices";
            this.toolStripStatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // toolStripProgressBar
            // 
            this.toolStripProgressBar.Name = "toolStripProgressBar";
            this.toolStripProgressBar.Size = new System.Drawing.Size(75, 19);
            this.toolStripProgressBar.Step = 1;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.settingsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(4, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(603, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitInstrulabToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // exitInstrulabToolStripMenuItem
            // 
            this.exitInstrulabToolStripMenuItem.Name = "exitInstrulabToolStripMenuItem";
            this.exitInstrulabToolStripMenuItem.Size = new System.Drawing.Size(141, 22);
            this.exitInstrulabToolStripMenuItem.Text = "Exit Instrulab";
            this.exitInstrulabToolStripMenuItem.Click += new System.EventHandler(this.exitInstrulabToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.uARTSpeedToolStripMenuItem,
            this.toolStripTextBox1});
            this.settingsToolStripMenuItem.Enabled = false;
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // uARTSpeedToolStripMenuItem
            // 
            this.uARTSpeedToolStripMenuItem.Name = "uARTSpeedToolStripMenuItem";
            this.uARTSpeedToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.uARTSpeedToolStripMenuItem.Text = "UART speed";
            // 
            // toolStripTextBox1
            // 
            this.toolStripTextBox1.Name = "toolStripTextBox1";
            this.toolStripTextBox1.Size = new System.Drawing.Size(100, 23);
            this.toolStripTextBox1.Text = "115200";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.sendUsFeedbackToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(453, 368);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(138, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "CTU FEE 2016 - Jiří Hladík";
            // 
            // sendUsFeedbackToolStripMenuItem
            // 
            this.sendUsFeedbackToolStripMenuItem.Name = "sendUsFeedbackToolStripMenuItem";
            this.sendUsFeedbackToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
            this.sendUsFeedbackToolStripMenuItem.Text = "Send us feedback";
            this.sendUsFeedbackToolStripMenuItem.Click += new System.EventHandler(this.sendUsFeedbackToolStripMenuItem_Click);
            // 
            // Instrulab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(603, 387);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MinimumSize = new System.Drawing.Size(619, 426);
            this.Name = "Instrulab";
            this.Text = "Little Embedded Oscilloscope";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Instrulab_FormClosing);
            this.panel1.ResumeLayout(false);
            this.groupBox_device_info.ResumeLayout(false);
            this.tableLayoutPanel8.ResumeLayout(false);
            this.groupBox_general.ResumeLayout(false);
            this.groupBox_general.PerformLayout();
            this.groupBox_scope.ResumeLayout(false);
            this.groupBox_scope.PerformLayout();
            this.groupBox_generator.ResumeLayout(false);
            this.groupBox_generator.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ListBox listBox_devices;
        private System.Windows.Forms.Button btn_connect;
        private System.Windows.Forms.Button btn_scan;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel_color;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel8;
        private System.Windows.Forms.GroupBox groupBox_general;
        private System.Windows.Forms.Label label42;
        private System.Windows.Forms.Label label_MCU;
        private System.Windows.Forms.Label label_con2;
        private System.Windows.Forms.Label label_con1;
        private System.Windows.Forms.Label label29;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.Label label_Freq;
        private System.Windows.Forms.GroupBox groupBox_scope;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label_scope_smpl;
        private System.Windows.Forms.Label label_scope_pins;
        private System.Windows.Forms.Label label_scope_vref;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label_scope_buff_len;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.GroupBox groupBox_generator;
        private System.Windows.Forms.Label label52;
        private System.Windows.Forms.Label label_gen_smpl;
        private System.Windows.Forms.Label label_gen_data_depth;
        private System.Windows.Forms.Label label_gen_channs;
        private System.Windows.Forms.Label label44;
        private System.Windows.Forms.Label label_gen_vref;
        private System.Windows.Forms.Label label45;
        private System.Windows.Forms.Label label48;
        private System.Windows.Forms.Label label46;
        private System.Windows.Forms.Label label_gen_buff_len;
        private System.Windows.Forms.GroupBox groupBox_device_info;
        private System.Windows.Forms.Label label_device;
        private System.Windows.Forms.Label label_scope_channs;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label_gen_pins;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_gen_open;
        private System.Windows.Forms.Button btn_scope_open;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitInstrulabToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem uARTSpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.Label label_con4;
        private System.Windows.Forms.Label label_con3;
        private System.Windows.Forms.Label label_fw;
        private System.Windows.Forms.Label label_RTOS;
        private System.Windows.Forms.Label label_HAL;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ToolStripProgressBar toolStripProgressBar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem sendUsFeedbackToolStripMenuItem;


    }
}

