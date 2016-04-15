using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Xml;
using SolidStateZip;

namespace Unzipper
{
    public partial class ScanBuilds : Form
    {
        // http://manifest.swtor.com/patch/movies_en_us.patchmanifest
        public List<string> manifests = new List<string>() {
            "movies_en_us",
            "assets_swtor_main",
            "retailclient_swtor",
            "assets_swtor_en_us",
            "assets_swtor_de_de",
            "assets_swtor_fr_fr",
        };
        string outputDir { get; set; }
        internal SolidStateZip.ZipExtractor ZipExtractor;

        public ScanBuilds(string _outputDir)
        {
            InitializeComponent();
            outputDir = _outputDir;
            ZipExtractor = new ZipExtractor(null, false, null, null, null);
        }

        private void ClearItemList()
        {
            if (scan_listBox.InvokeRequired)
            {
                scan_listBox.Invoke((MethodInvoker)(() => scan_listBox.Items.Clear()));
            }
            else
            {
                scan_listBox.Items.Clear();
            }
        }

        private void AddItem(object item)
        {
            if (scan_listBox.InvokeRequired)
            {
                scan_listBox.Invoke((MethodInvoker)(() => scan_listBox.Items.Add(item)));
            }
            else
            {
                scan_listBox.Items.Add(item);
            }
        }

        private void scan_button_Click(object sender, EventArgs e)
        {
            ScanWorker.RunWorkerAsync(false);
        }

        private void Scan_Builds(object source, EventArgs e)
        {
            if (outputDir.Contains("\\"))
                Directory.CreateDirectory(outputDir.Substring(0, outputDir.LastIndexOf("\\")));
            foreach (var mankey in manifests)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(String.Format("{0}{1}.txt", outputDir, mankey), false))
                    file.Write("");
                using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(String.Format("{0}{1}.txt", outputDir, mankey), true))
                {
                    Stream rawXML = DownloadAndUnzipFileToStream(String.Format("http://manifest.swtor.com/patch/{0}.patchmanifest", mankey), "manifest.xml");
                    AddItem(mankey);
                    System.Xml.Linq.XDocument manifest = System.Xml.Linq.XDocument.Load(rawXML);

                    System.Xml.Linq.XElement update_paths = manifest.Root.Element("ReleaseUpdatePaths");
                    var updates = update_paths.Elements();
                    foreach (var update in updates)
                    {
                        long from = Int64.Parse(update.Element("From").Value);
                        long to = Int64.Parse(update.Element("To").Value);
                        if (from == to - 1)
                        {
                            var dataItems = update.Element("ExtraData").Elements("ExtraDataItem");
                            if (dataItems.Count() > 0)
                            {
                                var metaFileElement = dataItems.Where(x => x.Element("Key").Value == "MetafileUrl").First();
                                string metaFileUrl = metaFileElement.Element("Value").Value;
                                Stream rawMetaFile = DownloadAndUnzipFileToStream(metaFileUrl, "metafile.solid");
                                BencodeNET.Objects.BDictionary decoded = (BencodeNET.Objects.BDictionary)BencodeNET.Bencode.Decode(rawMetaFile);
                                rawMetaFile.Dispose();
                                long rawSeconds = Int64.Parse(decoded["creation date"].ToString());
                                DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                time = time.AddSeconds(rawSeconds).ToLocalTime();
                                file2.Write(String.Format("{0} : {1}\n", to, time.ToString()), true);
                                AddItem(String.Join(" : ", to, time.ToString()));
                            }
                            else
                            {
                                AddItem(String.Join(" : ", from, to, "Missing Data"));
                            }
                        }
                        continue;
                    }
                    rawXML.Dispose();
                }
            }
        }
        
        private Stream DownloadAndUnzipFileToStream(string url, string filename)
        {
            try
            {
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
                var response = (HttpWebResponse)req.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (MemoryStream memStream = new MemoryStream())
                        {
                            stream.CopyTo(memStream);
                            memStream.Position = 0;
                            BinaryReader brReader = new BinaryReader(memStream);
                            return ZipExtractor.UnzipSingleFile(ref brReader, filename);
                        }
                    }
                }
            }
            catch (WebException ex)
            {
                AddItem(String.Format("Error: {0}", ex.Message));
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    AddItem(String.Format("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode));
                    AddItem(String.Format("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription));
                }
            }
            return null;
        }

        private void ScanWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Scan_Builds(sender, e);
        }
    }
}
