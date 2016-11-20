namespace LEO
{
    partial class VoltageSource
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VoltageSource));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox_ch_1 = new System.Windows.Forms.GroupBox();
            this.label_ch1_volt = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_volt_1 = new System.Windows.Forms.TextBox();
            this.trackBar_chann_1 = new System.Windows.Forms.TrackBar();
            this.groupBox_ch_2 = new System.Windows.Forms.GroupBox();
            this.label_ch2_volt = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox_volt_2 = new System.Windows.Forms.TextBox();
            this.trackBar_chann_2 = new System.Windows.Forms.TrackBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label_used_vdda = new System.Windows.Forms.Label();
            this.panel_status = new System.Windows.Forms.Panel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox_ch_1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_chann_1)).BeginInit();
            this.groupBox_ch_2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_chann_2)).BeginInit();
            this.panel1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox_ch_1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox_ch_2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 24);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(348, 196);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox_ch_1
            // 
            this.groupBox_ch_1.Controls.Add(this.label_ch1_volt);
            this.groupBox_ch_1.Controls.Add(this.label1);
            this.groupBox_ch_1.Controls.Add(this.textBox_volt_1);
            this.groupBox_ch_1.Controls.Add(this.trackBar_chann_1);
            this.groupBox_ch_1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_ch_1.Location = new System.Drawing.Point(3, 3);
            this.groupBox_ch_1.Name = "groupBox_ch_1";
            this.groupBox_ch_1.Size = new System.Drawing.Size(342, 79);
            this.groupBox_ch_1.TabIndex = 0;
            this.groupBox_ch_1.TabStop = false;
            this.groupBox_ch_1.Text = "Channel 1";
            // 
            // label_ch1_volt
            // 
            this.label_ch1_volt.AutoSize = true;
            this.label_ch1_volt.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_ch1_volt.Location = new System.Drawing.Point(150, 8);
            this.label_ch1_volt.Name = "label_ch1_volt";
            this.label_ch1_volt.Size = new System.Drawing.Size(97, 37);
            this.label_ch1_volt.TabIndex = 5;
            this.label_ch1_volt.Text = "0 mV";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(112, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(22, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "mV";
            // 
            // textBox_volt_1
            // 
            this.textBox_volt_1.Location = new System.Drawing.Point(9, 23);
            this.textBox_volt_1.Name = "textBox_volt_1";
            this.textBox_volt_1.Size = new System.Drawing.Size(100, 20);
            this.textBox_volt_1.TabIndex = 3;
            this.textBox_volt_1.Text = "0";
            this.textBox_volt_1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_volt_1_KeyPress);
            this.textBox_volt_1.Leave += new System.EventHandler(this.textBox_volt_1_Leave);
            // 
            // trackBar_chann_1
            // 
            this.trackBar_chann_1.AutoSize = false;
            this.trackBar_chann_1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackBar_chann_1.Location = new System.Drawing.Point(3, 44);
            this.trackBar_chann_1.Name = "trackBar_chann_1";
            this.trackBar_chann_1.Size = new System.Drawing.Size(336, 32);
            this.trackBar_chann_1.TabIndex = 1;
            this.trackBar_chann_1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_chann_1.ValueChanged += new System.EventHandler(this.trackBar_chann_1_ValueChanged);
            // 
            // groupBox_ch_2
            // 
            this.groupBox_ch_2.Controls.Add(this.label_ch2_volt);
            this.groupBox_ch_2.Controls.Add(this.label2);
            this.groupBox_ch_2.Controls.Add(this.textBox_volt_2);
            this.groupBox_ch_2.Controls.Add(this.trackBar_chann_2);
            this.groupBox_ch_2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox_ch_2.Location = new System.Drawing.Point(3, 88);
            this.groupBox_ch_2.Name = "groupBox_ch_2";
            this.groupBox_ch_2.Size = new System.Drawing.Size(342, 79);
            this.groupBox_ch_2.TabIndex = 1;
            this.groupBox_ch_2.TabStop = false;
            this.groupBox_ch_2.Text = "Channel 2";
            // 
            // label_ch2_volt
            // 
            this.label_ch2_volt.AutoSize = true;
            this.label_ch2_volt.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.label_ch2_volt.Location = new System.Drawing.Point(150, 9);
            this.label_ch2_volt.Name = "label_ch2_volt";
            this.label_ch2_volt.Size = new System.Drawing.Size(97, 37);
            this.label_ch2_volt.TabIndex = 5;
            this.label_ch2_volt.Text = "0 mV";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(115, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(22, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "mV";
            // 
            // textBox_volt_2
            // 
            this.textBox_volt_2.Location = new System.Drawing.Point(9, 24);
            this.textBox_volt_2.Name = "textBox_volt_2";
            this.textBox_volt_2.Size = new System.Drawing.Size(100, 20);
            this.textBox_volt_2.TabIndex = 4;
            this.textBox_volt_2.Text = "0";
            this.textBox_volt_2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_volt_2_KeyPress);
            this.textBox_volt_2.Leave += new System.EventHandler(this.textBox_volt_2_Leave);
            // 
            // trackBar_chann_2
            // 
            this.trackBar_chann_2.AutoSize = false;
            this.trackBar_chann_2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.trackBar_chann_2.Location = new System.Drawing.Point(3, 44);
            this.trackBar_chann_2.Name = "trackBar_chann_2";
            this.trackBar_chann_2.Size = new System.Drawing.Size(336, 32);
            this.trackBar_chann_2.TabIndex = 0;
            this.trackBar_chann_2.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar_chann_2.ValueChanged += new System.EventHandler(this.trackBar_chann_2_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label_used_vdda);
            this.panel1.Controls.Add(this.panel_status);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 173);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(342, 20);
            this.panel1.TabIndex = 2;
            // 
            // label_used_vdda
            // 
            this.label_used_vdda.Dock = System.Windows.Forms.DockStyle.Left;
            this.label_used_vdda.Location = new System.Drawing.Point(19, 0);
            this.label_used_vdda.Name = "label_used_vdda";
            this.label_used_vdda.Size = new System.Drawing.Size(115, 20);
            this.label_used_vdda.TabIndex = 1;
            this.label_used_vdda.Text = "Used Vdda 3300 mV";
            this.label_used_vdda.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // panel_status
            // 
            this.panel_status.BackColor = System.Drawing.Color.Red;
            this.panel_status.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel_status.Location = new System.Drawing.Point(0, 0);
            this.panel_status.Name = "panel_status";
            this.panel_status.Size = new System.Drawing.Size(19, 20);
            this.panel_status.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(348, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // VoltageSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(348, 220);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "VoltageSource";
            this.Text = "VoltageSource";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VoltageSource_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox_ch_1.ResumeLayout(false);
            this.groupBox_ch_1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_chann_1)).EndInit();
            this.groupBox_ch_2.ResumeLayout(false);
            this.groupBox_ch_2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar_chann_2)).EndInit();
            this.panel1.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox_ch_1;
        private System.Windows.Forms.GroupBox groupBox_ch_2;
        private System.Windows.Forms.TrackBar trackBar_chann_2;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.TrackBar trackBar_chann_1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_volt_1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox_volt_2;
        private System.Windows.Forms.Label label_ch1_volt;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label_used_vdda;
        private System.Windows.Forms.Panel panel_status;
        private System.Windows.Forms.Label label_ch2_volt;
    }
}