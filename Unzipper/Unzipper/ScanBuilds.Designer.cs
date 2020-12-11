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
            this.OverallProgressBar = new System.Windows.Forms.ProgressBar();
            this.Manual_Button = new System.Windows.Forms.Button();
            this.ptsCheckBox = new System.Windows.Forms.CheckBox();
            this.upcoming_button = new System.Windows.Forms.Button();
            this.verify_button = new System.Windows.Forms.Button();
            this.betaCheckBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // scan_listBox
            // 
            this.scan_listBox.FormattingEnabled = true;
            this.scan_listBox.ItemHeight = 16;
            this.scan_listBox.Location = new System.Drawing.Point(16, 16);
            this.scan_listBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.scan_listBox.Name = "scan_listBox";
            this.scan_listBox.Size = new System.Drawing.Size(963, 388);
            this.scan_listBox.TabIndex = 0;
            // 
            // scan_button
            // 
            this.scan_button.Location = new System.Drawing.Point(988, 15);
            this.scan_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.scan_button.Name = "scan_button";
            this.scan_button.Size = new System.Drawing.Size(100, 28);
            this.scan_button.TabIndex = 1;
            this.scan_button.Text = "Scan";
            this.scan_button.UseVisualStyleBackColor = true;
            this.scan_button.Click += new System.EventHandler(this.Scan_button_Click);
            // 
            // WalkWorker
            // 
            this.WalkWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.WalkWorker_DoWork);
            // 
            // download_button
            // 
            this.download_button.Location = new System.Drawing.Point(988, 50);
            this.download_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.download_button.Name = "download_button";
            this.download_button.Size = new System.Drawing.Size(100, 28);
            this.download_button.TabIndex = 2;
            this.download_button.Text = "Download";
            this.download_button.UseVisualStyleBackColor = true;
            this.download_button.Click += new System.EventHandler(this.Download_button_Click);
            // 
            // Download_progressBar
            // 
            this.Download_progressBar.Location = new System.Drawing.Point(17, 453);
            this.Download_progressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Download_progressBar.Name = "Download_progressBar";
            this.Download_progressBar.Size = new System.Drawing.Size(963, 12);
            this.Download_progressBar.TabIndex = 3;
            // 
            // Download_TextBox
            // 
            this.Download_TextBox.Location = new System.Drawing.Point(16, 474);
            this.Download_TextBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Download_TextBox.Name = "Download_TextBox";
            this.Download_TextBox.Size = new System.Drawing.Size(963, 22);
            this.Download_TextBox.TabIndex = 4;
            this.Download_TextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // extract_button
            // 
            this.extract_button.Location = new System.Drawing.Point(988, 86);
            this.extract_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.extract_button.Name = "extract_button";
            this.extract_button.Size = new System.Drawing.Size(100, 28);
            this.extract_button.TabIndex = 5;
            this.extract_button.Text = "Extract";
            this.extract_button.UseVisualStyleBackColor = true;
            this.extract_button.Click += new System.EventHandler(this.Extract_button_Click);
            // 
            // db_button
            // 
            this.db_button.Location = new System.Drawing.Point(988, 122);
            this.db_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.db_button.Name = "db_button";
            this.db_button.Size = new System.Drawing.Size(100, 28);
            this.db_button.TabIndex = 6;
            this.db_button.Text = "Save Patches";
            this.db_button.UseVisualStyleBackColor = true;
            this.db_button.Click += new System.EventHandler(this.db_button_Click);
            // 
            // sym_button
            // 
            this.sym_button.Location = new System.Drawing.Point(988, 158);
            this.sym_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.sym_button.Name = "sym_button";
            this.sym_button.Size = new System.Drawing.Size(100, 28);
            this.sym_button.TabIndex = 7;
            this.sym_button.Text = "Sym";
            this.sym_button.UseVisualStyleBackColor = true;
            this.sym_button.Click += new System.EventHandler(this.sym_button_Click);
            // 
            // output_button
            // 
            this.output_button.Location = new System.Drawing.Point(988, 193);
            this.output_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.output_button.Name = "output_button";
            this.output_button.Size = new System.Drawing.Size(100, 28);
            this.output_button.TabIndex = 8;
            this.output_button.Text = "Output";
            this.output_button.UseVisualStyleBackColor = true;
            this.output_button.Click += new System.EventHandler(this.output_button_Click);
            // 
            // OverallProgressBar
            // 
            this.OverallProgressBar.Location = new System.Drawing.Point(17, 417);
            this.OverallProgressBar.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.OverallProgressBar.Name = "OverallProgressBar";
            this.OverallProgressBar.Size = new System.Drawing.Size(963, 28);
            this.OverallProgressBar.TabIndex = 9;
            // 
            // Manual_Button
            // 
            this.Manual_Button.Location = new System.Drawing.Point(988, 377);
            this.Manual_Button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Manual_Button.Name = "Manual_Button";
            this.Manual_Button.Size = new System.Drawing.Size(100, 28);
            this.Manual_Button.TabIndex = 10;
            this.Manual_Button.Text = "Manual";
            this.Manual_Button.UseVisualStyleBackColor = true;
            this.Manual_Button.Click += new System.EventHandler(this.Manual_Button_Click);
            // 
            // ptsCheckBox
            // 
            this.ptsCheckBox.AutoSize = true;
            this.ptsCheckBox.Location = new System.Drawing.Point(988, 417);
            this.ptsCheckBox.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ptsCheckBox.Name = "ptsCheckBox";
            this.ptsCheckBox.Size = new System.Drawing.Size(57, 21);
            this.ptsCheckBox.TabIndex = 11;
            this.ptsCheckBox.Text = "PTS";
            this.ptsCheckBox.UseVisualStyleBackColor = true;
            this.ptsCheckBox.CheckedChanged += new System.EventHandler(this.ptsCheckBox_CheckedChanged);
            // 
            // upcoming_button
            // 
            this.upcoming_button.Location = new System.Drawing.Point(988, 229);
            this.upcoming_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.upcoming_button.Name = "upcoming_button";
            this.upcoming_button.Size = new System.Drawing.Size(100, 28);
            this.upcoming_button.TabIndex = 12;
            this.upcoming_button.Text = "Upcoming";
            this.upcoming_button.UseVisualStyleBackColor = true;
            this.upcoming_button.Click += new System.EventHandler(this.upcoming_button_Click);
            // 
            // verify_button
            // 
            this.verify_button.Location = new System.Drawing.Point(988, 265);
            this.verify_button.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.verify_button.Name = "verify_button";
            this.verify_button.Size = new System.Drawing.Size(100, 28);
            this.verify_button.TabIndex = 13;
            this.verify_button.Text = "Verify";
            this.verify_button.UseVisualStyleBackColor = true;
            this.verify_button.Click += new System.EventHandler(this.verify_button_Click);
            // 
            // betaCheckBox
            // 
            this.betaCheckBox.AutoSize = true;
            this.betaCheckBox.Location = new System.Drawing.Point(988, 444);
            this.betaCheckBox.Margin = new System.Windows.Forms.Padding(4);
            this.betaCheckBox.Name = "betaCheckBox";
            this.betaCheckBox.Size = new System.Drawing.Size(59, 21);
            this.betaCheckBox.TabIndex = 14;
            this.betaCheckBox.Text = "Beta";
            this.betaCheckBox.UseVisualStyleBackColor = true;
            this.betaCheckBox.CheckedChanged += new System.EventHandler(this.betaCheckBox_CheckedChanged);
            // 
            // ScanBuilds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1104, 510);
            this.Controls.Add(this.betaCheckBox);
            this.Controls.Add(this.verify_button);
            this.Controls.Add(this.upcoming_button);
            this.Controls.Add(this.ptsCheckBox);
            this.Controls.Add(this.Manual_Button);
            this.Controls.Add(this.OverallProgressBar);
            this.Controls.Add(this.output_button);
            this.Controls.Add(this.sym_button);
            this.Controls.Add(this.db_button);
            this.Controls.Add(this.extract_button);
            this.Controls.Add(this.Download_TextBox);
            this.Controls.Add(this.Download_progressBar);
            this.Controls.Add(this.download_button);
            this.Controls.Add(this.scan_button);
            this.Controls.Add(this.scan_listBox);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
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
        private System.Windows.Forms.ProgressBar OverallProgressBar;
        private System.Windows.Forms.Button Manual_Button;
        private System.Windows.Forms.CheckBox ptsCheckBox;
        private System.Windows.Forms.Button upcoming_button;
        private System.Windows.Forms.Button verify_button;
        private System.Windows.Forms.CheckBox betaCheckBox;
    }
}