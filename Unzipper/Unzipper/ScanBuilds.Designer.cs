namespace Unzipper
{
    partial class ScanBuilds
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
            this.scan_listBox = new System.Windows.Forms.ListBox();
            this.scan_button = new System.Windows.Forms.Button();
            this.ScanWorker = new System.ComponentModel.BackgroundWorker();
            this.SuspendLayout();
            // 
            // scan_listBox
            // 
            this.scan_listBox.FormattingEnabled = true;
            this.scan_listBox.Location = new System.Drawing.Point(12, 13);
            this.scan_listBox.Name = "scan_listBox";
            this.scan_listBox.Size = new System.Drawing.Size(723, 329);
            this.scan_listBox.TabIndex = 0;
            // 
            // scan_button
            // 
            this.scan_button.Location = new System.Drawing.Point(741, 12);
            this.scan_button.Name = "scan_button";
            this.scan_button.Size = new System.Drawing.Size(75, 23);
            this.scan_button.TabIndex = 1;
            this.scan_button.Text = "Scan";
            this.scan_button.UseVisualStyleBackColor = true;
            this.scan_button.Click += new System.EventHandler(this.scan_button_Click);
            // 
            // ScanWorker
            // 
            this.ScanWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.ScanWorker_DoWork);
            // 
            // ScanBuilds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 349);
            this.Controls.Add(this.scan_button);
            this.Controls.Add(this.scan_listBox);
            this.Name = "ScanBuilds";
            this.Text = "ScanBuilds";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox scan_listBox;
        private System.Windows.Forms.Button scan_button;
        private System.ComponentModel.BackgroundWorker ScanWorker;
    }
}