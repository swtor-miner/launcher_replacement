namespace Unzipper
{
    partial class Form1
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.patchBrowse = new System.Windows.Forms.Button();
            this.tempPathBox = new System.Windows.Forms.TextBox();
            this.patchOutBox = new System.Windows.Forms.TextBox();
            this.tempBrowse = new System.Windows.Forms.Button();
            this.outBrowse = new System.Windows.Forms.Button();
            this.extractAndPatchButton = new System.Windows.Forms.Button();
            this.searchPatchBtn = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.patchBaseBox = new System.Windows.Forms.TextBox();
            this.patchBaseBrowseBtn = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.extractWorker1 = new System.ComponentModel.BackgroundWorker();
            this.useSymChk = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(80, 12);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(351, 20);
            this.textBox1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(80, 138);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Extract";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(7, 167);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(507, 251);
            this.listBox1.TabIndex = 2;
            // 
            // patchBrowse
            // 
            this.patchBrowse.Location = new System.Drawing.Point(439, 12);
            this.patchBrowse.Name = "patchBrowse";
            this.patchBrowse.Size = new System.Drawing.Size(75, 23);
            this.patchBrowse.TabIndex = 3;
            this.patchBrowse.Text = "Browse";
            this.patchBrowse.UseVisualStyleBackColor = true;
            this.patchBrowse.Click += new System.EventHandler(this.patchBrowse_Click);
            // 
            // tempPathBox
            // 
            this.tempPathBox.Location = new System.Drawing.Point(80, 39);
            this.tempPathBox.Name = "tempPathBox";
            this.tempPathBox.Size = new System.Drawing.Size(351, 20);
            this.tempPathBox.TabIndex = 4;
            this.tempPathBox.TextChanged += new System.EventHandler(this.tempPathBox_TextChanged);
            // 
            // patchOutBox
            // 
            this.patchOutBox.Location = new System.Drawing.Point(80, 66);
            this.patchOutBox.Name = "patchOutBox";
            this.patchOutBox.Size = new System.Drawing.Size(351, 20);
            this.patchOutBox.TabIndex = 5;
            this.patchOutBox.TextChanged += new System.EventHandler(this.patchOutBox_TextChanged);
            // 
            // tempBrowse
            // 
            this.tempBrowse.Location = new System.Drawing.Point(439, 42);
            this.tempBrowse.Name = "tempBrowse";
            this.tempBrowse.Size = new System.Drawing.Size(75, 23);
            this.tempBrowse.TabIndex = 6;
            this.tempBrowse.Text = "Browse";
            this.tempBrowse.UseVisualStyleBackColor = true;
            this.tempBrowse.Click += new System.EventHandler(this.tempBrowse_Click);
            // 
            // outBrowse
            // 
            this.outBrowse.Location = new System.Drawing.Point(439, 72);
            this.outBrowse.Name = "outBrowse";
            this.outBrowse.Size = new System.Drawing.Size(75, 23);
            this.outBrowse.TabIndex = 7;
            this.outBrowse.Text = "Browse";
            this.outBrowse.UseVisualStyleBackColor = true;
            this.outBrowse.Click += new System.EventHandler(this.outBrowse_Click);
            // 
            // extractAndPatchButton
            // 
            this.extractAndPatchButton.Location = new System.Drawing.Point(162, 137);
            this.extractAndPatchButton.Name = "extractAndPatchButton";
            this.extractAndPatchButton.Size = new System.Drawing.Size(92, 23);
            this.extractAndPatchButton.TabIndex = 8;
            this.extractAndPatchButton.Text = "Extract + Patch";
            this.extractAndPatchButton.UseVisualStyleBackColor = true;
            this.extractAndPatchButton.Click += new System.EventHandler(this.extractAndPatchButton_Click);
            // 
            // searchPatchBtn
            // 
            this.searchPatchBtn.Enabled = false;
            this.searchPatchBtn.Location = new System.Drawing.Point(260, 137);
            this.searchPatchBtn.Name = "searchPatchBtn";
            this.searchPatchBtn.Size = new System.Drawing.Size(80, 23);
            this.searchPatchBtn.TabIndex = 9;
            this.searchPatchBtn.Text = "Find Patches";
            this.searchPatchBtn.UseVisualStyleBackColor = true;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Enabled = false;
            this.checkBox1.Location = new System.Drawing.Point(346, 147);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(130, 17);
            this.checkBox1.TabIndex = 10;
            this.checkBox1.Text = "Download from TORC";
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Patch file:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(25, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 26);
            this.label2.TabIndex = 12;
            this.label2.Text = "Extract\r\nDirectory\r\n";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 69);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(73, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Patch Output:";
            // 
            // patchBaseBox
            // 
            this.patchBaseBox.Location = new System.Drawing.Point(80, 93);
            this.patchBaseBox.Name = "patchBaseBox";
            this.patchBaseBox.Size = new System.Drawing.Size(351, 20);
            this.patchBaseBox.TabIndex = 14;
            this.patchBaseBox.TextChanged += new System.EventHandler(this.patchBaseBox_TextChanged);
            // 
            // patchBaseBrowseBtn
            // 
            this.patchBaseBrowseBtn.Location = new System.Drawing.Point(439, 102);
            this.patchBaseBrowseBtn.Name = "patchBaseBrowseBtn";
            this.patchBaseBrowseBtn.Size = new System.Drawing.Size(75, 23);
            this.patchBaseBrowseBtn.TabIndex = 15;
            this.patchBaseBrowseBtn.Text = "Browse";
            this.patchBaseBrowseBtn.UseVisualStyleBackColor = true;
            this.patchBaseBrowseBtn.Click += new System.EventHandler(this.patchBaseBrowseBtn_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 13);
            this.label4.TabIndex = 16;
            this.label4.Text = "Patch Base:";
            // 
            // extractWorker1
            // 
            this.extractWorker1.DoWork += new System.ComponentModel.DoWorkEventHandler(this.extractWorker1_DoWork);
            this.extractWorker1.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.extractWorker1_RunWorkerCompleted);
            // 
            // useSymChk
            // 
            this.useSymChk.AutoSize = true;
            this.useSymChk.Location = new System.Drawing.Point(346, 124);
            this.useSymChk.Name = "useSymChk";
            this.useSymChk.Size = new System.Drawing.Size(89, 17);
            this.useSymChk.TabIndex = 17;
            this.useSymChk.Text = "Use Symlinks";
            this.toolTip1.SetToolTip(this.useSymChk, "When patching create a symlink back to the patch base for unchanged files.\r\n");
            this.useSymChk.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 430);
            this.Controls.Add(this.useSymChk);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.patchBaseBrowseBtn);
            this.Controls.Add(this.patchBaseBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.searchPatchBtn);
            this.Controls.Add(this.extractAndPatchButton);
            this.Controls.Add(this.outBrowse);
            this.Controls.Add(this.tempBrowse);
            this.Controls.Add(this.patchOutBox);
            this.Controls.Add(this.tempPathBox);
            this.Controls.Add(this.patchBrowse);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.textBox1);
            this.Name = "Form1";
            this.Text = "Patcher";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button patchBrowse;
        private System.Windows.Forms.TextBox tempPathBox;
        private System.Windows.Forms.TextBox patchOutBox;
        private System.Windows.Forms.Button tempBrowse;
        private System.Windows.Forms.Button outBrowse;
        private System.Windows.Forms.Button extractAndPatchButton;
        private System.Windows.Forms.Button searchPatchBtn;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox patchBaseBox;
        private System.Windows.Forms.Button patchBaseBrowseBtn;
        private System.Windows.Forms.Label label4;
        private System.ComponentModel.BackgroundWorker extractWorker1;
        private System.Windows.Forms.CheckBox useSymChk;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}

