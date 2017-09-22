﻿namespace Unzipper
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
            this.SuspendLayout();
            // 
            // scan_listBox
            // 
            this.scan_listBox.FormattingEnabled = true;
            this.scan_listBox.Location = new System.Drawing.Point(12, 13);
            this.scan_listBox.Name = "scan_listBox";
            this.scan_listBox.Size = new System.Drawing.Size(723, 316);
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
            this.Download_progressBar.Location = new System.Drawing.Point(13, 368);
            this.Download_progressBar.Name = "Download_progressBar";
            this.Download_progressBar.Size = new System.Drawing.Size(722, 10);
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
            this.db_button.Text = "Save Patches";
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
            // OverallProgressBar
            // 
            this.OverallProgressBar.Location = new System.Drawing.Point(13, 339);
            this.OverallProgressBar.Name = "OverallProgressBar";
            this.OverallProgressBar.Size = new System.Drawing.Size(722, 23);
            this.OverallProgressBar.TabIndex = 9;
            // 
            // Manual_Button
            // 
            this.Manual_Button.Location = new System.Drawing.Point(741, 306);
            this.Manual_Button.Name = "Manual_Button";
            this.Manual_Button.Size = new System.Drawing.Size(75, 23);
            this.Manual_Button.TabIndex = 10;
            this.Manual_Button.Text = "Manual";
            this.Manual_Button.UseVisualStyleBackColor = true;
            this.Manual_Button.Click += new System.EventHandler(this.Manual_Button_Click);
            // 
            // ptsCheckBox
            // 
            this.ptsCheckBox.AutoSize = true;
            this.ptsCheckBox.Location = new System.Drawing.Point(741, 339);
            this.ptsCheckBox.Name = "ptsCheckBox";
            this.ptsCheckBox.Size = new System.Drawing.Size(47, 17);
            this.ptsCheckBox.TabIndex = 11;
            this.ptsCheckBox.Text = "PTS";
            this.ptsCheckBox.UseVisualStyleBackColor = true;
            this.ptsCheckBox.CheckedChanged += new System.EventHandler(this.ptsCheckBox_CheckedChanged);
            // 
            // upcoming_button
            // 
            this.upcoming_button.Location = new System.Drawing.Point(741, 186);
            this.upcoming_button.Name = "upcoming_button";
            this.upcoming_button.Size = new System.Drawing.Size(75, 23);
            this.upcoming_button.TabIndex = 12;
            this.upcoming_button.Text = "Upcoming";
            this.upcoming_button.UseVisualStyleBackColor = true;
            this.upcoming_button.Click += new System.EventHandler(this.upcoming_button_Click);
            // 
            // ScanBuilds
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(828, 414);
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
    }
}