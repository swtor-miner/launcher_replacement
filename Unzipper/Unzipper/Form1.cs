using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Ionic.Zip;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using SolidStateZip;

namespace Unzipper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            //Load paths from config.
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings["TempPath"] != null)
            {
                //If this exists they should all exist.
                tempPathBox.Text = config.AppSettings.Settings["TempPath"].Value;
                patchOutBox.Text = config.AppSettings.Settings["OutPath"].Value;
                patchBaseBox.Text = config.AppSettings.Settings["BasePath"].Value;
            }
            else
            {
                //Create setting entries.
                config.AppSettings.Settings.Add("TempPath", "C:\\swtorTemp\\");
                config.AppSettings.Settings.Add("OutPath", "C:\\swtorSelfPatch\\");
                config.AppSettings.Settings.Add("BasePath", "C:\\Program Files (x86)\\Electronic Arts\\BioWare\\Star Wars - The Old Republic\\");
                config.Save();

                tempPathBox.Text = config.AppSettings.Settings["TempPath"].Value;
                patchOutBox.Text = config.AppSettings.Settings["OutPath"].Value;
                patchBaseBox.Text = config.AppSettings.Settings["BasePath"].Value;
            }
        }
        internal SolidStateZip.ZipExtractor ZipExtractor;

        // moved to SolidStateZip Libary

        private void patchBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog fbd = new OpenFileDialog();
            fbd.FileName = textBox1.Text;
            if(fbd.ShowDialog() ==DialogResult.OK)
                textBox1.Text = fbd.FileName;
        }

        private void tempBrowse_Click(object sender, EventArgs e)
        {
            string tempPath = BrowseDirectory("Select folder to extract and download files to.");
            if(tempPath.Length > 0)
            {
                tempPathBox.Text = tempPath;
            }
        }

        private void outBrowse_Click(object sender, EventArgs e)
        {
            string outPath = BrowseDirectory("Select folder to write patch output to.");
            if (outPath.Length > 0)
            {
                patchOutBox.Text = outPath;
            }
        }

        private void patchBaseBrowseBtn_Click(object sender, EventArgs e)
        {
            string outPath = BrowseDirectory("Select folder to get files to use as the source for patching.");
            if (outPath.Length > 0)
            {
                patchBaseBox.Text = outPath;
            }
        }

        private string BrowseDirectory(string desc)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.Description = desc;
            if(fbd.ShowDialog() == DialogResult.OK)
            {
                return fbd.SelectedPath;
            } else
            {
                return string.Empty;
            }
        }

        private void tempPathBox_TextChanged(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["TempPath"].Value = tempPathBox.Text;
            config.Save();    
        }

        private void patchOutBox_TextChanged(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["OutPath"].Value = patchOutBox.Text;
            config.Save();
        }

        private void patchBaseBox_TextChanged(object sender, EventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["BasePath"].Value = patchBaseBox.Text;
            config.Save();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(!tempPathBox.Text.EndsWith("\\"))
            {
                tempPathBox.Text += "\\";
            }
            if(!patchBaseBox.Text.EndsWith("\\"))
            {
                patchBaseBox.Text += "\\";
            }
            if(!patchOutBox.Text.EndsWith("\\"))
            {
                patchOutBox.Text += "\\";
            }

            textBox1.Enabled = false;
            tempPathBox.Enabled = false;
            patchOutBox.Enabled = false;
            patchBaseBox.Enabled = false;

            patchBrowse.Enabled = false;
            tempBrowse.Enabled = false;
            outBrowse.Enabled = false;
            patchBaseBrowseBtn.Enabled = false;

            button1.Enabled = false;
            extractAndPatchButton.Enabled = false;
            useSymChk.Enabled = false;
            //searchPatchBtn.Enabled = false;
            //checkBox1.Enabled = false;

            string fullFileName = textBox1.Text;
            if (!File.Exists(fullFileName))
            {
                MessageBox.Show("ERROR: The requested file " + fullFileName + " does not exist.");
            }
            else
            {
                extractWorker1.RunWorkerAsync(false);
            }
        }

        private void extractAndPatchButton_Click(object sender, EventArgs e)
        {
            if (!tempPathBox.Text.EndsWith("\\"))
            {
                tempPathBox.Text += "\\";
            }
            if (!patchBaseBox.Text.EndsWith("\\"))
            {
                patchBaseBox.Text += "\\";
            }
            if (!patchOutBox.Text.EndsWith("\\"))
            {
                patchOutBox.Text += "\\";
            }

            textBox1.Enabled = false;
            tempPathBox.Enabled = false;
            patchOutBox.Enabled = false;
            patchBaseBox.Enabled = false;

            patchBrowse.Enabled = false;
            tempBrowse.Enabled = false;
            outBrowse.Enabled = false;
            patchBaseBrowseBtn.Enabled = false;

            button1.Enabled = false;
            extractAndPatchButton.Enabled = false;
            //searchPatchBtn.Enabled = false;
            //checkBox1.Enabled = false;
            string fullFileName = textBox1.Text;
            if (!File.Exists(fullFileName))
            {
                MessageBox.Show("ERROR: The requested file " + fullFileName + " does not exist.");
            }
            else
            {
                extractWorker1.RunWorkerAsync(true);
            }
        }

        private void ClearItemList()
        {
            if(listBox1.InvokeRequired)
            {
                listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Clear()));
            } else
            {
                listBox1.Items.Clear();
            }
        }

        private void AddItem(object item)
        {
            if (listBox1.InvokeRequired)
            {
                listBox1.Invoke((MethodInvoker)(() => listBox1.Items.Add(item)));
            }
            else
            {
                listBox1.Items.Add(item);
            }
        }

        private void ToggleButtons(object enable)
        {
            if (listBox1.InvokeRequired)
            {
                extractAndPatchButton.Invoke((MethodInvoker)(() => extractAndPatchButton.Enabled = (bool)enable));
            }
            else
            {
                extractAndPatchButton.Enabled = (bool)enable;
            }
        }

        private void extractWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            string fullFileName = textBox1.Text;
            bool doPatch = (bool)e.Argument;

            using (FileStream fs = File.Open(fullFileName, FileMode.Open))
            {
                BinaryReader br = new BinaryReader(fs);
                ClearItemList();

                ToggleButtons(false);
                ZipExtractor = new ZipExtractor(AddItem, useSymChk.Checked, patchBaseBox.Text, patchOutBox.Text, tempPathBox.Text);
                ZipExtractor.Unzip(ref br, fullFileName, doPatch);
                br.Close();
                br.Dispose();
                this.AddItem("Complete");
                if (ZipExtractor.errorOccurred)
                {
                    int count = 1;
                    if (ZipExtractor.errorTexts != null)
                        count = ZipExtractor.errorTexts.Count();
                    MessageBox.Show(String.Format("{0} error(s) have occurred.\n{1}", count, String.Join(Environment.NewLine, ZipExtractor.errorTexts)));
                }
                ToggleButtons(true);
            }
        }

        private void extractWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //Free up any memory when we are done.
            System.Runtime.GCSettings.LargeObjectHeapCompactionMode = System.Runtime.GCLargeObjectHeapCompactionMode.CompactOnce;
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);

            textBox1.Enabled = true;
            tempPathBox.Enabled = true;
            patchOutBox.Enabled = true;
            patchBaseBox.Enabled = true;

            patchBrowse.Enabled = true;
            tempBrowse.Enabled = true;
            outBrowse.Enabled = true;
            patchBaseBrowseBtn.Enabled = true;

            button1.Enabled = true;
            extractAndPatchButton.Enabled = true;
            useSymChk.Enabled = true;
            //searchPatchBtn.Enabled = true;
            //checkBox1.Enabled = true;

        }

        private void scan_button_Click(object sender, EventArgs e)
        {
            scan_button.Enabled = false;
            ScanBuilds testFile = new ScanBuilds(useSymChk.Checked, patchOutBox.Text, tempPathBox.Text, patchBaseBox.Text);
            DialogResult result = testFile.ShowDialog();
            scan_button.Enabled = true;
        }
    }
}


