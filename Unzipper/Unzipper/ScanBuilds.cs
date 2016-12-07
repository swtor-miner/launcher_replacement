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
using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using System.Xml.Linq;

namespace Unzipper
{
    public partial class ScanBuilds : Form
    {
        // http://manifest.swtor.com/patch/movies_en_us.patchmanifest
        public List<string> manifests = new List<string>() {
            //"movies_en_us",
            "assets_swtor_main",
            "retailclient_swtor",
            "assets_swtor_en_us",
            "assets_swtor_de_de",
            "assets_swtor_fr_fr",
        };
        bool useSymChk { get; set; }
        string outputDir { get; set; }
        string baseDir { get; set; }
        string tempDir { get; set; }
        internal SolidStateZip.ZipExtractor ZipExtractor;

        public ScanBuilds(bool _useSymChk, string _outputDir, string _tempDir, string _baseDir)
        {
            InitializeComponent();
            useSymChk = _useSymChk;
            outputDir = _outputDir;
            tempDir = _tempDir;
            baseDir = _baseDir;
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
            WalkWorker.RunWorkerAsync(WalkType.Scan);
        }
        private void download_button_Click(object sender, EventArgs e)
        {
            WalkWorker.RunWorkerAsync(WalkType.Download);
        }
        private void extract_button_Click(object sender, EventArgs e)
        {
            WalkWorker.RunWorkerAsync(WalkType.Extract);
        }

        private void WalkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Walk_Builds((WalkType)e.Argument);
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

        enum WalkType
        {
            Scan = 0,
            Download = 1,
            Extract = 2
        }
        private async void Walk_Builds(WalkType wtype)
        {
            Load_Patches();
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
                    HashSet<string> exceptionList = new HashSet<string>()
                    {
                        "assets_swtor_en_us_146to149",
                        "assets_swtor_fr_fr_146to148"
                    };
                    foreach (var update in updates)
                    {
                        long from = Int64.Parse(update.Element("From").Value);
                        long to = Int64.Parse(update.Element("To").Value);
                        bool isException = false;

                        if (exceptionList.Contains(String.Format("{0}_{1}to{2}", mankey, from, to)))
                            isException = true;
                        if (from == to - 1 || isException)
                        {
                            if (wtype == WalkType.Extract && Directory.Exists(String.Format("{0}\\base\\{1}\\{2}\\", outputDir, mankey, to)))
                                continue;
                            var dataItems = update.Element("ExtraData").Elements("ExtraDataItem");
                            if (dataItems.Count() > 0)
                            {
                                var metaFileElement = dataItems.Where(x => x.Element("Key").Value == "MetafileUrl").First();
                                string metaFileUrl = metaFileElement.Element("Value").Value;
                                Stream rawMetaFile = null;
                                int i = 0;
                                while (rawMetaFile == null && i < 5)
                                {
                                    var cache_file = String.Format(@"cache/{0}_{1}to{2}.metafile.solid", mankey, from, to);
                                    if (System.IO.File.Exists(cache_file))
                                    {
                                        using (FileStream file = new FileStream(cache_file, FileMode.Open, FileAccess.Read))
                                        {
                                            rawMetaFile = new MemoryStream((int)file.Length);
                                            file.CopyTo(rawMetaFile);
                                        }
                                    }
                                    else
                                    { 
                                        rawMetaFile = DownloadAndUnzipFileToStream(metaFileUrl, "metafile.solid");
                                        using (FileStream file = new FileStream(cache_file, FileMode.OpenOrCreate, FileAccess.Write))
                                        {
                                            rawMetaFile.CopyTo(file);
                                            rawMetaFile.Position = 0;
                                        }
                                    }
                                    i++;
                                }
                                if (rawMetaFile == null)
                                    continue;
                                rawMetaFile.Position = 0;
                                BencodeNET.Objects.BDictionary decoded = (BencodeNET.Objects.BDictionary)BencodeNET.Bencode.Decode(rawMetaFile);
                                rawMetaFile.Dispose();
                                long rawSeconds = Int64.Parse(decoded["creation date"].ToString());
                                DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                                time = time.AddSeconds(rawSeconds).ToLocalTime();
                                switch (wtype)
                                {
                                    case WalkType.Scan:
                                        var versions_to_update = patch_parsed
                                            .Where(x => x.Value >= time)
                                            .Select(x => x.Key).ToList();
                                        if (versions_to_update.Count > 0)
                                        {
                                            var eles = patches.Element("patches").Elements()
                                                .Where(x => versions_to_update.Contains(x.Attribute("version").Value));
                                            foreach (var ele in eles)
                                            {
                                                if (ele.Attribute(mankey) != null)
                                                    ele.Attribute(mankey).Value = to.ToString();
                                                else
                                                    ele.Add(new XAttribute(mankey, to));
                                                AddItem(String.Format("Updated {0}: {1} to {2}", ele.Attribute("version"), mankey, to));
                                            }
                                        }
                                        //var filter = Builders<BsonDocument>.Filter.Gte<BsonDateTime>("date", time);
                                        //var sort = Builders<BsonDocument>.Sort.Ascending("date");

                                        //using (var cursor = await collection.Find(filter).Sort(sort).ToCursorAsync())
                                        //{
                                        //    while (await cursor.MoveNextAsync())
                                        //    {
                                        //        var batch = cursor.Current;
                                        //        foreach (var document in batch)
                                        //        {
                                        //            await collection.UpdateOneAsync(
                                        //                Builders<BsonDocument>.Filter.Eq("patch", document["patch"]),
                                        //                Builders<BsonDocument>.Update.Set(mankey, to));
                                        //            AddItem(String.Format("Updated {0}: {1} to {2}", document["patch"], mankey, to));
                                        //            break;
                                        //        }
                                        //        break;
                                        //    }
                                        //}
                                        break;
                                    case WalkType.Download:
                                        BencodeNET.Objects.BList files = ((BencodeNET.Objects.BList)((BencodeNET.Objects.BDictionary)decoded["info"])["files"]);
                                        string title = decoded["title"].ToString();
                                        string reliable = decoded["reliable"].ToString();
                                        foreach (var bFile in files)
                                        {
                                            BencodeNET.Objects.BDictionary dict = bFile as BencodeNET.Objects.BDictionary;
                                            string filename = ((BencodeNET.Objects.BList)dict["path"])[0].ToString();

                                            string outputDirectory = String.Format("{0}patches\\{1}\\", outputDir, title).Replace(": ", "_");
                                            string outputFilename = String.Format("{0}{1}", outputDirectory, filename);
                                            string downloadUrl = String.Format("{0}{1}", reliable, filename);
                                            Directory.CreateDirectory(outputDirectory);
                                            if (File.Exists(outputFilename))
                                            {
                                                file2.Write(String.Format("Skipped: {0}\n", outputFilename), true);
                                                continue;
                                            }
                                            try
                                            {
                                                AddItem(String.Format("Downloading: {0}", filename));
                                                DownloadFile(new Uri(downloadUrl), outputFilename);
                                                file2.Write(String.Format("Created: {0}\n", outputFilename), true);
                                            }
                                            catch (WebException ex)
                                            {
                                                AddItem(String.Format("Error: {0}", ex.Message));
                                                file2.Write(String.Format("Error: {0} : {1}\n", ex.Message, downloadUrl), true);
                                                if (ex.Status == WebExceptionStatus.ProtocolError)
                                                {
                                                    AddItem(String.Format("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode));
                                                    file2.Write(String.Format("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode), true);
                                                    AddItem(String.Format("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription));
                                                    file2.Write(String.Format("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription), true);
                                                }
                                            }
                                        }
                                        break;
                                    case WalkType.Extract:
                                        string fullFileName = String.Format("{0}patches\\{1}_{2}to{3}\\{1}_{2}to{3}.zip", tempDir, mankey, from, to);
                                        using (FileStream fs = File.Open(fullFileName, FileMode.Open))
                                        {
                                            BinaryReader br = new BinaryReader(fs);
                                            ClearItemList();
                                            AddItem(String.Format("Patching: {0}_{1}to{2}", mankey, from, to));
                                            string tempPath = String.Format("{0}diffs\\{1}_{2}to{3}\\", tempDir, mankey, from, to);
                                            Directory.CreateDirectory(tempPath);

                                            string basePath = String.Format("{0}\\base\\{1}\\{2}\\", outputDir, mankey, from);
                                            Directory.CreateDirectory(basePath);

                                            string outPath = String.Format("{0}\\base\\{1}\\{2}\\", outputDir, mankey, to);
                                            if (Directory.Exists(outPath))
                                                continue;
                                            Directory.CreateDirectory(outPath);

                                            ZipExtractor = new ZipExtractor(AddItem, true, basePath, outPath, tempPath);
                                            ZipExtractor.Unzip(ref br, fullFileName, true);
                                            br.Close();
                                            br.Dispose();
                                        }
                                        break;
                                }
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
            ClearItemList();
            AddItem(String.Format("{0} Finished", wtype.ToString()));
        }

        private void DownloadFile(Uri uri, string destination)
        {
            using (WebClient downloader = new WebClient())
            {
                downloader.DownloadProgressChanged += client_DownloadProgressChanged;
                downloader.DownloadFileCompleted += client_DownloadComplete;

                var syncObject = new Object();
                lock (syncObject)
                {
                    downloader.DownloadFileAsync(uri, destination, syncObject);
                    //This would block the thread until download completes
                    System.Threading.Monitor.Wait(syncObject);
                }
            }
        }
        public void client_DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            lock (e.UserState)
            {
                //releases blocked thread
                System.Threading.Monitor.Pulse(e.UserState);
            }
        }
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                Download_TextBox.Text = String.Format("{0}MB of {1}MB", Math.Round(e.BytesReceived / 1024f / 1024f, 2), Math.Round(e.TotalBytesToReceive / 1024f / 1024f, 2));
                Download_progressBar.Value = int.Parse(Math.Truncate(percentage).ToString());
                Download_progressBar.Update();
            });
        }

        XDocument patches = null;
        Dictionary<string, DateTime> patch_parsed = new Dictionary<string, DateTime>();
        private void Load_Patches()
        {
            if(patches == null)
            {
                patches = XDocument.Load(@"../../patches.xml");
                var patchEles = patches.Element("patches").Elements("patch");
                foreach (var patch in patchEles)
                {
                    var ver = patch.Attribute("version").Value;
                    var datePieces = patch.Attribute("date").Value.Split('/');
                    var month = Int32.Parse(datePieces[0]);
                    var day = Int32.Parse(datePieces[1]);
                    var year = Int32.Parse(datePieces[2]);
                    var date = new DateTime(year, month, day);
                    patch_parsed.Add(ver, date);
                }
            }
        }

        private void Save_Patches()
        {
            using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(@"../../patches.xml"))
            {
                patches.Save(file2, SaveOptions.None);
            }
        }
        private async void SymLink_Patches()
        {
            Load_Patches();
            var patchEles = patches.Element("patches").Elements();
            foreach (var element in patchEles)
            {
                string target = String.Format("{0}\\patches\\{1}\\", outputDir, element.Attribute("version").Value);
                AddItem(target);
                foreach (var mankey in manifests)
                {
                    if (element.Attribute(mankey) == null)
                        continue;
                    string source = String.Format("{0}\\base\\{1}\\{2}\\", baseDir, mankey, element.Attribute(mankey).Value);

                    string[] basePatchFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
                    Directory.CreateDirectory(target);

                    foreach (string basePatchFilePath in basePatchFiles)
                    {
                        string basePatchFileName = basePatchFilePath.Replace(source, "");

                        if (!File.Exists(target + basePatchFileName))
                        {
                            //Check to make sure the target file doesn't exist and then create the symlink.
                            string patchFileName = target + basePatchFileName;
                            string patchDir = patchFileName.Substring(0, patchFileName.LastIndexOf("\\"));
                            Directory.CreateDirectory(patchDir);
                            ZipExtractor.CreateSafeSymbolicLink(patchFileName, basePatchFilePath);

                        }
                    }
                }
            }
            AddItem("Finish");
        }

        private async void Generate_Output()
        {
            var patchEles = patches.Element("patches").Elements();
            foreach (var element in patchEles)
            {
                string target = String.Format("{0}\\patches\\{1}\\", outputDir, element.Attribute("version"));
                AddItem(target);
                foreach (var mankey in manifests)
                {
                    string patch = element.Attribute("version").Value;
                    if (element.Attribute(manifests[0]) == null)
                    {
                        AddItem(String.Format("No Asset Changes {0}", patch));
                        continue;
                    }
                    AddItem(String.Format("Outputting {0}", patch));
                    using (Process deltaProcess = new Process())
                    {
                        //Maybe this shouldn't be hard coded? Could do this in memory instead to.
                        deltaProcess.StartInfo.FileName = "ConsoleTools.exe";

                        deltaProcess.StartInfo.CreateNoWindow = true;
                        deltaProcess.StartInfo.UseShellExecute = false;
                        deltaProcess.StartInfo.RedirectStandardOutput = true;
                        deltaProcess.StartInfo.RedirectStandardError = true;
                        deltaProcess.StartInfo.Arguments = String.Format("{0} {1} {2}", patch, baseDir, outputDir);
                        deltaProcess.Start();
                        string output = deltaProcess.StandardOutput.ReadToEnd();
                        Console.WriteLine(output);
                        deltaProcess.WaitForExit();
                    }
                }
            }
        }

        //protected static IMongoClient _client;
        //protected static IMongoDatabase _database;

        //private async void SymLink_Patches()
        //{
        //    _client = new MongoClient();
        //    _database = _client.GetDatabase("patches");
        //    var collection = _database.GetCollection<BsonDocument>("live");

        //    using (var cursor = await collection.FindAsync(new BsonDocument()))
        //    {
        //        while (await cursor.MoveNextAsync())
        //        {
        //            var batch = cursor.Current;
        //            foreach (var document in batch)
        //            {
        //                string target = String.Format("{0}\\patches\\{1}\\", outputDir, document["patch"]);
        //                AddItem(target);
        //                foreach (var mankey in manifests)
        //                {
        //                    if (document[mankey] == 0)
        //                        continue;
        //                    string source = String.Format("{0}\\base\\{1}\\{2}\\", baseDir, mankey, document[mankey]);

        //                    string[] basePatchFiles = Directory.GetFiles(source, "*", SearchOption.AllDirectories);
        //                    Directory.CreateDirectory(target);

        //                    foreach (string basePatchFilePath in basePatchFiles)
        //                    {
        //                        string basePatchFileName = basePatchFilePath.Replace(source, "");

        //                        if (!File.Exists(target + basePatchFileName))
        //                        {
        //                            //Check to make sure the target file doesn't exist and then create the symlink.
        //                            string patchFileName = target + basePatchFileName;
        //                            string patchDir = patchFileName.Substring(0, patchFileName.LastIndexOf("\\"));
        //                            Directory.CreateDirectory(patchDir);
        //                            ZipExtractor.CreateSafeSymbolicLink(patchFileName, basePatchFilePath);

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    AddItem("Finish");
        //}

        //private void Insert_Patches()
        //{
        //    _client = new MongoClient();
        //    _database = _client.GetDatabase("patches");
        //    var collection = _database.GetCollection<BsonDocument>("live");
        //    Dictionary<string, string> old_patches = new Dictionary<string, string>()
        //    {
        //        {"1.0.0", "12/15/2011"},
        //        {"1.0.0a", "12/16/2011"},
        //        {"1.0.0b", "12/17/2011"},
        //        {"1.0.0c", "12/19/2011"},
        //        {"1.0.0d", "12/20/2011"},
        //        {"1.0.0e", "12/22/2011"},
        //        {"1.0.0f", "12/22/2011"},
        //        {"1.0.1", "12/27/2011"},
        //        {"1.0.1a", "12/29/2011"},
        //        {"1.0.2", "1/4/2012"},
        //        {"1.0.2a", "1/4/2012"},
        //        {"1.0.2b", "1/5/2012"},
        //        {"1.0.2c", "1/6/2012"},
        //        {"1.0.2d", "1/12/2012"},
        //        {"1.1.0", "1/18/2012"},
        //        {"1.1.0a", "1/19/2012"},
        //        {"1.1.0b", "1/24/2012"},
        //        {"1.1.0c", "1/28/2012"},
        //        {"1.1.1", "1/31/2012"},
        //        {"1.1.1a", "2/2/2012"},
        //        {"1.1.2", "2/7/2012"},
        //        {"1.1.2a", "2/9/2012"},
        //        {"1.1.3", "2/14/2012"},
        //        {"1.1.3a", "2/14/2012"},
        //        {"1.1.4", "2/22/2012"},
        //        {"1.1.5", "3/6/2012"},
        //        {"1.1.5a", "3/13/2012"},
        //        {"1.1.5b", "3/15/2012"},
        //        {"1.1.5c", "3/20/2012"},
        //        {"1.1.5d", "3/23/2012"},
        //        {"1.1.5e", "4/3/2012"},
        //        {"1.2.0", "4/12/2012"},
        //        {"1.2.0a", "4/13/2012"},
        //        {"1.2.0b", "4/17/2012"},
        //        {"1.2.0c", "4/19/2012"},
        //        {"1.2.1", "4/24/2012"},
        //        {"1.2.1a", "4/27/2012"},
        //        {"1.2.2", "5/1/2012"},
        //        {"1.2.3", "5/8/2012"},
        //        {"1.2.3a", "5/10/2012"},
        //        {"1.2.4", "5/15/2012"},
        //        {"1.2.5", "5/30/2012"},
        //        {"1.2.6", "6/5/2012"},
        //        {"1.2.6a", "6/7/2012"},
        //        {"1.2.7", "6/12/2012"},
        //        {"1.2.7a", "6/13/2012"},
        //        {"1.2.7b", "6/19/2012"},
        //        {"1.3.0", "6/26/2012"},
        //        {"1.3.0a", "6/28/2012"},
        //        {"1.3.0b", "6/29/2012"},
        //        {"1.3.1", "7/3/2012"},
        //        {"1.3.2", "7/10/2012"},
        //        {"1.3.3", "7/17/2012"},
        //        {"1.3.4", "7/24/2012"},
        //        {"1.3.4a", "7/26/2012"},
        //        {"1.3.5", "8/7/2012"},
        //        {"1.3.5a", "8/9/2012"},
        //        {"1.3.5b", "8/16/2012"},
        //        {"1.3.6", "8/21/2012"},
        //        {"1.3.7", "9/5/2012"},
        //        {"1.3.7a", "9/11/2012"},
        //        {"1.3.8", "9/13/2012"},
        //        {"1.4.0", "9/26/2012"},
        //        {"1.4.0a", "9/27/2012"},
        //        {"1.4.0b", "10/2/2012"},
        //        {"1.4.1", "10/9/2012"},
        //        {"1.4.1a", "10/12/2012"},
        //        {"1.4.2", "10/16/2012"},
        //        {"1.4.3", "10/30/2012"},
        //        {"1.5.0", "11/15/2012"},
        //        {"1.5.0a", "11/16/2012"},
        //        {"1.5.1", "11/27/2012"},
        //        {"1.5.1a", "11/28/2012"},
        //        {"1.5.2", "12/4/2012"},
        //        {"1.5.2a", "12/6/2012"},
        //        {"1.6.0", "12/11/2012"},
        //        {"1.6.1", "12/18/2012"},
        //        {"1.6.2", "1/8/2013"},
        //        {"1.6.2a", "1/15/2013"},
        //        {"1.6.3", "1/23/2013"},
        //        {"1.6.3a", "1/25/2013"},
        //        {"1.7.0", "2/12/2013"},
        //        {"1.7.0a", "2/14/2013"},
        //        {"1.7.0b", "2/16/2013"},
        //        {"1.7.1", "2/27/2013"},
        //        {"1.7.2", "3/12/2013"},
        //        {"1.7.2a", "3/14/2013"},
        //        {"1.7.2b", "3/22/2013"},
        //        {"1.7.3", "3/26/2013"},
        //        {"1.7.3a", "4/2/2013"},
        //        {"2.0", "4/9/2013"},
        //        {"2.0.0a", "4/10/2013"},
        //        {"2.0.0b", "4/16/2013"},
        //        {"2.0.1", "4/24/2013"},
        //        {"2.1.0", "5/14/2013"},
        //        {"2.1.0a", "5/15/2013"},
        //        {"2.1.0b", "5/17/2013"},
        //        {"2.1.1", "5/30/2013"},
        //        {"2.2", "6/12/2013"},
        //        {"2.2.0a", "6/18/2013"},
        //        {"2.2.1", "6/25/2013"},
        //        {"2.2.2", "7/10/2013"},
        //        {"2.2.2a", "7/16/2013"},
        //        {"2.2.3", "7/23/2013"},
        //        {"2.3", "8/6/2013"},
        //        {"2.3.0a", "8/8/2013"},
        //        {"2.3.0b", "8/9/2013"},
        //        {"2.3.0c", "8/16/2013"},
        //        {"2.3.1", "8/21/2013"},
        //        {"2.3.1a", "8/22/2013"},
        //        {"2.3.2", "9/4/2013"},
        //        {"2.3.2a", "9/6/2013"},
        //        {"2.3.2b", "9/11/2013"},
        //        {"2.3.3", "9/17/2013"},
        //        {"2.3.3a", "9/20/2013"},
        //        {"2.4.0", "10/1/2013"},
        //        {"2.4.0a", "10/3/2013"},
        //        {"2.4.0b", "10/8/2013"},
        //        {"2.4.1", "10/15/2013"},
        //        {"2.4.1a", "10/17/2013"},
        //        {"2.4.1b", "10/22/2013"},
        //        {"2.4.2", "10/29/2013"},
        //        {"2.4.3", "11/12/2013"},
        //        {"2.4.3a", "11/19/2013"},
        //        {"2.5", "12/3/2013"},
        //        {"2.5.1", "12/17/2013"},
        //        {"2.5.1a", "12/19/2013"},
        //        {"2.5.2", "1/14/2014"},
        //        {"2.5.2a", "1/21/2014"},
        //        {"2.5a", "12/10/2013"},
        //        {"2.5b", "12/11/2013"},
        //        {"2.6", "2/4/2014"},
        //        {"2.6a", "2/10/2014"},
        //        {"2.6.1", "2/25/2014"},
        //        {"2.6.1a", "3/10/2014"},
        //        {"2.6.2", "3/18/2014"},
        //        {"2.7", "4/8/2014"},
        //        {"2.7.1", "4/29/2014"},
        //        {"2.7.2", "5/20/2014"},
        //        {"2.8", "6/10/2014"},
        //        {"2.8.1", "7/1/2014"},
        //        {"2.8.2", "7/22/2014"},
        //        {"2.8a", "6/13/2014"},
        //        {"2.8b", "6/18/2014"},
        //        {"2.9", "8/19/2014"},
        //        {"2.9a", "8/21/2014"},
        //        {"2.9b", "8/26/2014"},
        //        {"2.9c", "8/29/2014"},
        //        {"2.9d", "9/4/2014"},
        //        {"2.10", "9/9/2014"},
        //        {"2.10.1", "9/30/2014"},
        //        {"2.10.1a", "10/2/2014"},
        //        {"2.10.1c", "10/15/2014"},
        //        {"2.10.2", "10/22/2014"},
        //        {"2.10.2a", "11/4/2014"},
        //        {"2.10.3", "11/11/2014"},
        //        {"2.10.3a", "11/18/2014"},
        //        {"3.0", "12/2/2014"},
        //        {"3.0.0a", "12/9/2014"},
        //        {"3.0.0b", "12/10/2014"},
        //        {"3.0.1", "12/16/2014"},
        //        {"3.0.2", "1/13/2015"},
        //        {"3.0.2a", "1/21/2015"},
        //        {"3.0.2b", "1/27/2015"},
        //        {"3.1", "2/12/2015"},
        //        {"3.1a", "3/3/2015"},
        //        {"3.1.1", "3/12/2015"},
        //        {"3.1.2", "4/7/2015"},
        //        {"3.1.2a", "4/9/2015"},
        //        {"3.2", "4/28/2015"},
        //        {"3.2a", "4/30/2015"},
        //        {"3.2b", "5/12/2015"},
        //        {"3.2c", "5/14/2015"},
        //        {"3.2.1", "5/26/2015"},
        //        {"3.2.2", "6/23/2015"},
        //        {"3.2.2a", "6/25/2015"},
        //        {"3.3", "7/22/2015"},
        //        {"3.3a", "7/28/2015"},
        //        {"3.3.1", "8/18/2015"},
        //        {"3.3.2", "9/22/2015"},
        //        {"4.0", "10/20/2015"},
        //        {"4.0a", "10/22/2015"},
        //        {"4.0.1", "10/27/2015"},
        //        {"4.0.1a", "11/4/2015"},
        //        {"4.0.2", "11/17/2015"},
        //        {"4.0.2a", "11/24/2015"},
        //        {"4.0.3", "12/7/2015"},
        //        {"4.0.3a", "12/10/2015"},
        //        {"4.0.4", "1/12/2016"},
        //        {"4.1", "2/8/2016"},
        //        {"4.1a", "2/11/2016"},
        //        {"4.1b", "2/18/2016"},
        //        {"4.2", "3/8/2016"},
        //        {"4.2a", "3/10/2016"},
        //        {"4.2b", "3/24/2016"},
        //        {"4.3", "4/5/2016"},
        //        {"4.3a", "4/7/2016" },
        //        {"4.4", "5/3/2016"},
        //        {"4.5", "6/1/2016" },
        //        {"4.6", "6/28/2016" },
        //        {"4.6a", "7/5/2016" },
        //        {"4.7", "8/9/2016" },
        //        {"4.7a", "8/16/2016" },
        //        {"4.7.1", "9/7/2016" },
        //        {"4.7.2", "10/4/2016" },
        //        {"4.7.3", "10/25/2016" },
        //        {"4.7.3a", "10/27/2016" },
        //    };
        //    var indexopts = new CreateIndexOptions();
        //    indexopts.Unique = true;
        //    collection.Indexes.CreateOne(Builders<BsonDocument>.IndexKeys.Ascending("patch"), indexopts);
        //    foreach(var kvp in old_patches)
        //    {
        //        DateTime pTime;
        //        if (DateTime.TryParse(kvp.Value, out pTime))
        //        {
        //            var document = new BsonDocument() {
        //                { "patch", kvp.Key },
        //                { "date", pTime},
        //                { "assets_swtor_main", 0 },
        //                {"retailclient_swtor", 0 },
        //                { "assets_swtor_en_us", 0 },
        //                { "assets_swtor_de_de", 0 },
        //                { "assets_swtor_fr_fr", 0 },
        //            };
        //            collection.ReplaceOne(
        //                filter: new BsonDocument("patch", kvp.Key),
        //                options: new UpdateOptions { IsUpsert = true },
        //                replacement: document);
        //        }
        //        else
        //        {
        //            throw new IndexOutOfRangeException();
        //        }
        //    }
        //}

        //private async void Generate_Output()
        //{
        //    _client = new MongoClient();
        //    _database = _client.GetDatabase("patches");
        //    var collection = _database.GetCollection<BsonDocument>("live");
        //    var sort = Builders<BsonDocument>.Sort.Ascending("patch");
        //    var options = new FindOptions<BsonDocument>
        //        {
        //            Sort = sort
        //        };

        //    using (var cursor = await collection.FindAsync(new BsonDocument(), options))
        //    {
        //        while (await cursor.MoveNextAsync())
        //        {
        //            var batch = cursor.Current;
        //            foreach (var document in batch)
        //            {
        //                string patch = document["patch"].ToString();
        //                if (document[manifests[0]].ToInt32() == 0)
        //                {
        //                    AddItem(String.Format("No Asset Changes {0}", patch));
        //                    continue;
        //                }
        //                AddItem(String.Format("Outputting {0}", patch));
        //                using (Process deltaProcess = new Process())
        //                {
        //                    //Maybe this shouldn't be hard coded? Could do this in memory instead to.
        //                    deltaProcess.StartInfo.FileName = "ConsoleTools.exe";

        //                    deltaProcess.StartInfo.CreateNoWindow = true;
        //                    deltaProcess.StartInfo.UseShellExecute = false;
        //                    deltaProcess.StartInfo.RedirectStandardOutput = true;
        //                    deltaProcess.StartInfo.RedirectStandardError = true;
        //                    deltaProcess.StartInfo.Arguments = String.Format("{0} {1} {2}", patch, baseDir, outputDir);
        //                    deltaProcess.Start();
        //                    string output = deltaProcess.StandardOutput.ReadToEnd();
        //                    Console.WriteLine(output);
        //                    deltaProcess.WaitForExit();
        //                }
        //            }
        //        }
        //    }
        //}

        public enum WorkerFuncs
        {
            Insert = 0,
            Symlink = 1,
            Output = 2
        }
        private void DBWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            switch ((WorkerFuncs)e.Argument)
            {
                case WorkerFuncs.Insert:
                    Save_Patches(); break;
                case WorkerFuncs.Symlink:
                    SymLink_Patches(); break;
                case WorkerFuncs.Output:
                    Generate_Output(); break;
            }
        }

        private void db_button_Click(object sender, EventArgs e)
        {
            var db_worker = new BackgroundWorker();
            db_worker.DoWork += new DoWorkEventHandler(DBWorker_DoWork);
            db_worker.RunWorkerAsync(WorkerFuncs.Insert);
        }

        private void sym_button_Click(object sender, EventArgs e)
        {
            var db_worker = new BackgroundWorker();
            db_worker.DoWork += new DoWorkEventHandler(DBWorker_DoWork);
            db_worker.RunWorkerAsync(WorkerFuncs.Symlink);
        }

        private void output_button_Click(object sender, EventArgs e)
        {
            var db_worker = new BackgroundWorker();
            db_worker.DoWork += new DoWorkEventHandler(DBWorker_DoWork);
            db_worker.RunWorkerAsync(WorkerFuncs.Output);
        }
    }
}
