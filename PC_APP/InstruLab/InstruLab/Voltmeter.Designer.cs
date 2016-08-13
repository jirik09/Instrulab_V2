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
            this.components = new System.ComponentModel.Container();
            this.zedGraphControl_volt = new ZedGraph.ZedGraphControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.minimumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox_min = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.maximumToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripTextBox_max = new System.Windows.Forms.ToolStripTextBox();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // zedGraphControl_volt
            // 
            this.zedGraphControl_volt.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.zedGraphControl_volt.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zedGraphControl_volt.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World);
            this.zedGraphControl_volt.IsEnableVZoom = false;
            this.zedGraphControl_volt.Location = new System.Drawing.Point(0, 24);
            this.zedGraphControl_volt.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.zedGraphControl_volt.Name = "zedGraphControl_volt";
            this.zedGraphControl_volt.Padding = new System.Windows.Forms.Padding(3);
            this.zedGraphControl_volt.ScrollGrace = 0D;
            this.zedGraphControl_volt.ScrollMaxX = 0D;
            this.zedGraphControl_volt.ScrollMaxY = 0D;
            this.zedGraphControl_volt.ScrollMaxY2 = 0D;
            this.zedGraphControl_volt.ScrollMinX = 0D;
            this.zedGraphControl_volt.ScrollMinY = 0D;
            this.zedGraphControl_volt.ScrollMinY2 = 0D;
            this.zedGraphControl_volt.Size = new System.Drawing.Size(658, 305);
            this.zedGraphControl_volt.TabIndex = 5;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.rangeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(658, 24);
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
            // Voltmeter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(658, 329);
            this.Controls.Add(this.zedGraphControl_volt);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Voltmeter";
            this.Text = "Voltmeter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Voltmeter_FormClosing);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ZedGraph.ZedGraphControl zedGraphControl_volt;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem minimumToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_min;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem maximumToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolStripTextBox_max;
    }
}