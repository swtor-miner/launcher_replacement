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
            this.WalkWorker = new System.ComponentModel.BackgroundWorker();
            this.download_button = new System.Windows.Forms.Button();
            this.Download_progressBar = new System.Windows.Forms.ProgressBar();
            this.Download_TextBox = new System.Windows.Forms.MaskedTextBox();
            this.extract_button = new System.Windows.Forms.Button();
            this.db_button = new System.Windows.Forms.Button();
            this.sym_button = new System.Windows.Forms.Button();
            this.output_button = new System.Windows.Forms.Button();
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
            // WalkWorker
            // 
            this.WalkWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WalkWorker_DoWork);
            // 
            // download_button
            // 
            this.download_button.Location = new System.Drawing.Point(741, 41);
            this.download_button.Name = "download_button";
            this.download_button.Size = new System.Drawing.Size(75, 23);
            this.download_button.TabIndex = 2;
            this.download_button.Text = "Download";
            this.download_button.UseVisualStyleBackColor = true;
            this.download_button.Click += new System.EventHandler(this.download_button_Click);
            // 
            // Download_progressBar
            // 
            this.Download_progressBar.Location = new System.Drawing.Point(13, 355);
            this.Download_progressBar.Name = "Download_progressBar";
            this.Download_progressBar.Size = new System.Drawing.Size(722, 23);
            this.Download_progressBar.TabIndex = 3;
            // 
            // Download_TextBox
            // 
            this.Download_TextBox.Location = new System.Drawing.Point(12, 385);
            this.Download_TextBox.Name = "Download_TextBox";
            this.Download_TextBox.Size = new System.Drawing.Size(723, 20);
            this.Download_TextBox.TabIndex = 4;
            this.Download_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // extract_button
            // 
            this.extract_button.Location = new System.Drawing.Point(741, 70);
            this.extract_button.Name = "extract_button";
            this.extract_button.Size = new System.Drawing.Size(75, 23);
            this.extract_button.TabIndex = 5;
            this.extract_button.Text = "Extract";
            this.extract_button.UseVisualStyleBackColor = true;
            this.extract_button.Click += new System.EventHandler(this.extract_button_Click);
            // 
            // db_button
            // 
            this.db_button.Location = new System.Drawing.Point(741, 99);
            this.db_button.Name = "db_button";
            this.db_button.Size = new System.Drawing.Size(75, 23);
            this.db_button.TabIndex = 6;
            this.db_button.Text = "DB";
            this.db_button.UseVisualStyleBackColor = true;
            this.db_button.Click += new System.EventHandler(this.db_button_Click);
            // 
            // sym_button
            // 
            this.sym_button.Location = new System.Drawing.Point(741, 128);
            this.sym_button.Name = "sym_button";
            this.sym_button.Size = new System.Drawing.Size(75, 23);
            this.sym_button.TabIndex = 7;
            this.sym_button.Text = "Sym";
            this.sym_button.UseVisualStyleBackColor = true;
            this.sym_button.Click += new System.EventHandler(this.sym_button_Click);
            // 
            // output_button
            // 
            this.output_button.Location = new System.Drawing.Point(741, 157);
            this.output_button.Name = "output_button";
            this.output_button.Size = new System.Drawing.Size(75, 23);
            this.output_button.TabIndex = 8;
            this.output_button.Text = "Output";
            this.output_button.UseVisualStyleBackColor = true;
            this.output_button.Click += new System.EventHandler(this.output_button_Click);
            // 
            // ScanBuilds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 414);
            this.Controls.Add(this.output_button);
            this.Controls.Add(this.sym_button);
            this.Controls.Add(this.db_button);
            this.Controls.Add(this.extract_button);
            this.Controls.Add(this.Download_TextBox);
            this.Controls.Add(this.Download_progressBar);
            this.Controls.Add(this.download_button);
            this.Controls.Add(this.scan_button);
            this.Controls.Add(this.scan_listBox);
            this.Name = "ScanBuilds";
            this.Text = "ScanBuilds";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox scan_listBox;
        private System.Windows.Forms.Button scan_button;
        private System.ComponentModel.BackgroundWorker WalkWorker;
        private System.Windows.Forms.Button download_button;
        private System.Windows.Forms.ProgressBar Download_progressBar;
        private System.Windows.Forms.MaskedTextBox Download_TextBox;
        private System.Windows.Forms.Button extract_button;
        private System.Windows.Forms.Button db_button;
        private System.Windows.Forms.Button sym_button;
        private System.Windows.Forms.Button output_button;
    }
}