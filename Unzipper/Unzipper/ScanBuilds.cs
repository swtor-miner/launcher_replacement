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
        public Dictionary<string, List<string>> full_manifests = new Dictionary<string, List<string>>() {
            {"Live", new List<string>() {
                //"movies_en_us",
                "assets_swtor_main",
                "retailclient_swtor",
                "assets_swtor_en_us",
                "assets_swtor_de_de",
                "assets_swtor_fr_fr",
                //"retailclient_liveqatest",
            }
            },
            {"PTS", new List<string>() {
                "assets_swtor_test_main",
                "retailclient_publictest",
                "assets_swtor_test_en_us",
                //"assets_swtor_test_de_de",
                //"assets_swtor_test_fr_fr",
                }
            },
            {"Beta", new List<string>() {
                "retailclient_squadron157",
                "red_assets_en_us",
                "red_assets_main",
                }
            }
        };
        public List<string> manifests = new List<string>() {
            //"movies_en_us",
            "assets_swtor_main",
            "retailclient_swtor",
            "assets_swtor_en_us",
            "assets_swtor_de_de",
            "assets_swtor_fr_fr",
            //"retailclient_liveqatest",
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
            // Fix Ending slashes for consistency
            if (outputDir.EndsWith("\\"))
            {
                outputDir = outputDir.TrimEnd('\\');
            }
            if (tempDir.EndsWith("\\"))
            {
                tempDir = tempDir.TrimEnd('\\');
            }
            if (baseDir.EndsWith("\\"))
            {
                baseDir = baseDir.TrimEnd('\\');
            }
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

        private void Scan_button_Click(object sender, EventArgs e)
        {
            WalkWorker.RunWorkerAsync(WalkType.Scan);
        }
        private void Download_button_Click(object sender, EventArgs e)
        {
            WalkWorker.RunWorkerAsync(WalkType.Download);
        }
        private void Extract_button_Click(object sender, EventArgs e)
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
                AddItem(String.Format("Url: {0}", url));
                AddItem(String.Format("File: {0}", filename));
                AddItem(String.Format("Error: {0}", ex.Message));
                if (ex.Status == WebExceptionStatus.ProtocolError)
                {
                    AddItem(String.Format("Status Code : {0}", ((HttpWebResponse)ex.Response).StatusCode));
                    AddItem(String.Format("Status Description : {0}", ((HttpWebResponse)ex.Response).StatusDescription));
                }
            }
            return null;
        }

        private Stream LoadLocalAndUnzipFileToStream(string url, string filename)
        {
            try
            {
                if (File.Exists(url))
                {
                    using (Stream stream = File.OpenRead(url))
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
                AddItem(String.Format("Url: {0}", url));
                AddItem(String.Format("File: {0}", filename));
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
            Extract = 2,
            Verify = 3
        }
        private void Walk_Builds(WalkType wtype)
        {
            Load_Patches();
            if (outputDir.Contains("\\"))
                Directory.CreateDirectory(outputDir.Substring(0, outputDir.LastIndexOf("\\")));
            int m = -1;

            int mcount = manifests.Count;
            foreach (var mankey in manifests)
            {
                m++;
                int k = ((100 / mcount) * m);
                if (k < 0) k = 0;
                OverallProgressChanged(this, k);
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(String.Format("{0}{1}.txt", outputDir, mankey), false))
                    file.Write("");
                System.IO.StreamWriter file2 = new System.IO.StreamWriter(String.Format("{0}{1}.txt", outputDir, mankey), true);
                Stream rawXML = DownloadAndUnzipFileToStream(String.Format("http://manifest.swtor.com/patch/{0}.patchmanifest", mankey), "manifest.xml");
                if (rawXML == null) continue;
                //Stream rawXML = LoadLocalAndUnzipFileToStream("E:\\swtor\\beta\\red\\SWTOR\\patch\\Host.8f0ca3b5a8cfebd48d06cfc6021936a55ea4f0b5\\64207dd70022986c01c63a027196b9913ef5a3a3.patchmanifest", "manifest.xml");
                AddItem(mankey);
                System.Xml.Linq.XDocument manifest = System.Xml.Linq.XDocument.Load(rawXML);

                System.Xml.Linq.XElement update_paths = manifest.Root.Element("ReleaseUpdatePaths");
                var updates = update_paths.Elements();
                var ucount = updates.Count();
                HashSet<string> exceptionList = new HashSet<string>()
                    {
                        "assets_swtor_en_us_146to149",
                        "assets_swtor_fr_fr_146to148",
                        "assets_swtor_test_de_de_0to108",
                        "assets_swtor_test_fr_fr_0to111",
                        "retailclient_publictest_41to43",
                        "retailclient_publictest_41to44",
                        "retailclient_publictest_41to45",
                        "assets_swtor_test_main_225to245",
                        "assets_swtor_test_main_0to263",
                        "assets_swtor_test_en_us_0to244",
                        "assets_swtor_test_fr_fr_0to244",
                        "assets_swtor_test_de_de_0to244",
                        "assets_swtor_test_de_de_0to262",
                    };
                HashSet<string> skipList = new HashSet<string>()
                    {
                        "assets_swtor_test_de_de_106to107",
                        "assets_swtor_test_de_de_107to108",
                        "assets_swtor_test_fr_fr_109to110",
                        "assets_swtor_test_fr_fr_110to111",
                        "assets_swtor_test_main_243to244",
                        "assets_swtor_test_main_244to245",
                        "assets_swtor_test_main_261to262",
                        "assets_swtor_test_main_262to263",
                        "assets_swtor_test_en_us_242to243",
                        "assets_swtor_test_en_us_243to244",
                        "assets_swtor_test_fr_fr_242to243",
                        "assets_swtor_test_fr_fr_243to244",
                        "assets_swtor_test_de_de_242to243",
                        "assets_swtor_test_de_de_243to244",
                        "assets_swtor_test_de_de_260to261",
                        "assets_swtor_test_de_de_261to262",
                    };
                int u = 0;
                foreach (var update in updates)
                {
                    u++;
                    OverallProgressChanged(this, k + (100 / mcount) * (u / ucount));
                    long from = Int64.Parse(update.Element("From").Value);
                    long to = Int64.Parse(update.Element("To").Value);
                    bool isException = false;

                    if (exceptionList.Contains(String.Format("{0}_{1}to{2}", mankey, from, to)))
                        isException = true;
                    if (skipList.Contains(String.Format("{0}_{1}to{2}", mankey, from, to)))
                        continue;
                    if (from == to - 1 || isException)
                    {
                        if (wtype == WalkType.Extract && Directory.Exists(String.Format("{0}\\base\\{1}\\{2}\\", outputDir, mankey, to)))
                            continue;
                        var dataItems = update.Element("ExtraData").Elements("ExtraDataItem");
                        if (dataItems.Count() > 0)
                        {
                            var metaFileElement = dataItems.Where(x => x.Element("Key").Value == "MetafileUrl").First();
                            string metaFileUrl = metaFileElement.Element("Value").Value;

                            WalkBuild(wtype, metaFileUrl, mankey, from, to, file2);

                        }
                        else
                        {
                            AddItem(String.Join(" : ", from, to, "Missing Data"));
                        }
                    }
                    continue;
                }
                rawXML.Dispose();
                file2.Close();
            }
            //ClearItemList();
            AddItem(String.Format("{0} Finished", wtype.ToString()));
            OverallProgressChanged(this, 0);
        }

        private bool WalkBuild(WalkType wtype, string metaFileUrl, string mankey, long from, long to, System.IO.StreamWriter file2)
        {
            Stream rawMetaFile = null;
            int i = 0;
            var cache_file = String.Format(@"cache/{0}_{1}to{2}.metafile.solid", mankey, from, to);
            while (rawMetaFile == null && i < 5)
            {
                if (System.IO.File.Exists(cache_file))
                {
                    using (FileStream file = new FileStream(cache_file, FileMode.Open, FileAccess.Read))
                    {
                        rawMetaFile = new MemoryStream((int)file.Length);
                        file.CopyTo(rawMetaFile);
                        rawMetaFile.Position = 0;
                    }
                }
                else
                {
                    rawMetaFile = DownloadAndUnzipFileToStream(metaFileUrl, "metafile.solid");
                    if (rawMetaFile == null)
                        return false;
                    using (FileStream file = new FileStream(cache_file, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        rawMetaFile.CopyTo(file);
                        rawMetaFile.Position = 0;
                    }
                }
                i++;
            }
            
            BencodeNET.Objects.BDictionary decoded = (BencodeNET.Objects.BDictionary)BencodeNET.Bencode.Decode(rawMetaFile);
            rawMetaFile.Dispose();
            long rawSeconds = Int64.Parse(decoded["creation date"].ToString());
            DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            time = time.AddSeconds(rawSeconds).ToLocalTime();

            switch (wtype)
            {
                case WalkType.Scan:
                    //if (mankey == "assets_swtor_test_main")
                    //{
                    //    patches.Element("patches").Add(new XElement("patch", new XAttribute("version", to), new XAttribute("date", time.ToString("G"))));
                    //}
                    //else
                    //{
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
                                {
                                    if (Int32.Parse(ele.Attribute(mankey).Value) >= to)
                                        continue;
                                    else
                                        ele.Attribute(mankey).Value = to.ToString();
                                }
                                else
                                    ele.Add(new XAttribute(mankey, to));
                                AddItem(String.Format("Updated {0}: {1} to {2}", ele.Attribute("version"), mankey, to));
                            }
                        }
                    //}
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
                case WalkType.Verify:
                    string tit = decoded["title"].ToString();
                    string verifyDirectory = String.Format("{0}\\files\\{1}\\", outputDir, tit).Replace(": ", "_");
                    var torrent = BitTorrent.Torrent.LoadFromFile(cache_file, verifyDirectory);
                    //for(int p =0; p < torrent.PieceCount; p++)
                    //{
                    //    torrent.Verify(p);
                    //}
                    if (torrent.VerifiedRatio != 1)
                    {
                        //MessageBox.Show(String.Format("A file failed verification: {0}", tit));
                        file2.Write(String.Format("A file failed verification: {0}", tit), true);
                    }
                    break;
                case WalkType.Download:
                    BencodeNET.Objects.BList files = ((BencodeNET.Objects.BList)((BencodeNET.Objects.BDictionary)decoded["info"])["files"]);
                    string title = decoded["title"].ToString();
                    string reliable = decoded["reliable"].ToString();
                    string outputDirectory = String.Format("{0}\\files\\{1}\\", outputDir, title).Replace(": ", "_");

                    foreach (var bFile in files)
                    {
                        BencodeNET.Objects.BDictionary dict = bFile as BencodeNET.Objects.BDictionary;
                        string filename = ((BencodeNET.Objects.BList)dict["path"])[0].ToString();
                        
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
                    string fullFileName = String.Format("{0}\\files\\{1}_{2}to{3}\\{1}_{2}to{3}.zip", tempDir, mankey, from, to);
                    using (FileStream fs = File.Open(fullFileName, FileMode.Open))
                    {
                        BinaryReader br = new BinaryReader(fs);
                        ClearItemList();
                        AddItem(String.Format("Patching: {0}_{1}to{2}", mankey, from, to));
                        string tempPath = String.Format("{0}\\diffs\\{1}_{2}to{3}\\", tempDir, mankey, from, to);
                        Directory.CreateDirectory(tempPath);

                        string basePath = String.Format("{0}\\base\\{1}\\{2}\\", outputDir, mankey, from);
                        Directory.CreateDirectory(basePath);

                        string outPath = String.Format("{0}\\base\\{1}\\{2}\\", outputDir, mankey, to);
                        if (Directory.Exists(outPath))
                            return true;
                        Directory.CreateDirectory(outPath);

                        ZipExtractor = new ZipExtractor(AddItem, true, basePath, outPath, tempPath);
                        ZipExtractor.Unzip(ref br, fullFileName, true);
                        br.Close();
                        br.Dispose();
                    }
                    break;
            }
            file2.Write(String.Format("{0} : {1}\n", to, time.ToString()), true);
            //AddItem(String.Join(" : ", to, time.ToString()));
            return true;
        }

        private void Upcoming_Build_Search()
        {
            Load_Patches();
            if (outputDir.Contains("\\"))
                Directory.CreateDirectory(outputDir.Substring(0, outputDir.LastIndexOf("\\")));
            int i = 0;
            bool notfailed = true;
            while (notfailed)
            {
                i++;
                int m = -1;
                string main_sym_dir = "";
                int mcount = manifests.Count;
                foreach (var mankey in manifests)
                {
                    m++;
                    int k = ((100 / mcount) * m);
                    if (k < 0) k = 0;
                    OverallProgressChanged(this, k);
                    using (System.IO.StreamWriter file = new System.IO.StreamWriter(String.Format("{0}{1}.txt", outputDir, mankey), false))
                        file.Write("");
                    System.IO.StreamWriter file2 = new System.IO.StreamWriter(String.Format("{0}{1}.txt", outputDir, mankey), true);
                    Stream rawXML = DownloadAndUnzipFileToStream(String.Format("http://manifest.swtor.com/patch/{0}.patchmanifest", mankey), "manifest.xml");
                    AddItem(mankey);
                    System.Xml.Linq.XDocument manifest = System.Xml.Linq.XDocument.Load(rawXML);
                    long current_release;
                    Int64.TryParse(manifest.Root.Element("RequiredRelease").Value, out current_release);
                    current_release = current_release + i - 1;
                    long upcoming_release = current_release + 1;
                    string metaFileUrl = String.Format("http://cdn-patch.swtor.com/patch/{0}/{0}_{1}to{2}.solidpkg", mankey, current_release, upcoming_release);
                    if (UrlIsValid(new Uri(metaFileUrl)))
                    {
                        if (mankey.Contains("main"))
                            main_sym_dir = upcoming_release.ToString();
                        WalkBuild(WalkType.Download, metaFileUrl, mankey, current_release, upcoming_release, file2);
                        WalkBuild(WalkType.Verify, metaFileUrl, mankey, current_release, upcoming_release, file2);
                        if (!Directory.Exists(String.Format("{0}\\base\\{1}\\{2}\\", outputDir, mankey, upcoming_release)))
                            WalkBuild(WalkType.Extract, metaFileUrl, mankey, current_release, upcoming_release, file2);
                        string source = String.Format("{0}\\base\\{1}\\{2}\\", baseDir, mankey, upcoming_release);
                        string target = String.Format("{0}\\patches\\{1}\\", outputDir, main_sym_dir);
                        SymLink_Dir(target, source);
                    }
                    else if (mankey.Contains("main"))
                        notfailed = false;
                }
                OverallProgressChanged(this, 100);
            }
            AddItem("Finished");
        }

        private void Manual_Walk()
        {
            Load_Patches();
            if (outputDir.Contains("\\"))
                Directory.CreateDirectory(outputDir.Substring(0, outputDir.LastIndexOf("\\")));
            var inputDialog = new InputForm();
            if(inputDialog.ShowDialog() == DialogResult.OK)
            {
                if (!String.IsNullOrWhiteSpace(inputDialog.textBox1.Text)) {
                    string patch = inputDialog.textBox1.Text;
                    if (patch_parsed.ContainsKey(patch))
                    {
                        var element = patches.Element("patches").Elements()
                            .Where(x => x.Attribute("version").Value == patch).FirstOrDefault();
                        if (element != null)
                        {
                            string target = String.Format("{0}\\patches\\{1}\\", outputDir, patch);
                            foreach (var mankey in manifests)
                            {
                                if (element.Attribute(mankey) == null)
                                    continue;
                                
                                long to = Int64.Parse(element.Attribute(mankey).Value);
                                long from = to - 1;
                                using (System.IO.StreamWriter file = new System.IO.StreamWriter(String.Format("{0}{1}.txt", outputDir, mankey), false))
                                    file.Write("");
                                System.IO.StreamWriter file2 = new System.IO.StreamWriter(String.Format("{0}{1}.txt", outputDir, mankey), true);
                                if (!WalkBuild(WalkType.Download, String.Format("http://cdn-patch.swtor.com/patch/{0}/{0}_{1}to{2}.solidpkg", mankey, from, to), mankey, from, to, file2))
                                    continue;
                                if (!WalkBuild(WalkType.Extract, String.Format("http://cdn-patch.swtor.com/patch/{0}/{0}_{1}to{2}.solidpkg", mankey, from, to), mankey, from, to, file2))
                                    continue;
                                file2.Close();

                                string source = String.Format("{0}\\base\\{1}\\{2}\\", baseDir, mankey, to);

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
                    }
                }
            }
            inputDialog.Dispose();
        }

        private void DownloadFile(Uri uri, string destination)
        {
            using (WebClient downloader = new WebClient())
            {
                downloader.DownloadProgressChanged += Client_DownloadProgressChanged;
                downloader.DownloadFileCompleted += Client_DownloadComplete;

                var syncObject = new Object();
                lock (syncObject)
                {
                    downloader.DownloadFileAsync(uri, destination, syncObject);
                    //This would block the thread until download completes
                    System.Threading.Monitor.Wait(syncObject);
                }
            }
        }
        private bool UrlIsValid(Uri uri){
            using (MyClient check = new MyClient())
            {
                try
                {
                    check.HeadOnly = true;
                    string s1 = check.DownloadString(uri);
                    return true;
                }
                catch(Exception ex)
                {
                    return false;
                }
            }
        }

        class MyClient : WebClient
        {
            public bool HeadOnly { get; set; }
            protected override WebRequest GetWebRequest(Uri address)
            {
                WebRequest req = base.GetWebRequest(address);
                if (HeadOnly && req.Method == "GET")
                {
                    req.Method = "HEAD";
                }
                return req;
            }
        }

        public void Client_DownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            lock (e.UserState)
            {
                //releases blocked thread
                System.Threading.Monitor.Pulse(e.UserState);
            }
        }
        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
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

        private void OverallProgressChanged(object sender, int e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                OverallProgressBar.Value = e;
                OverallProgressBar.Update();
            });
        }

        XDocument patches = null;
        string patch_file = @"patches.xml";
        Dictionary<string, DateTime> patch_parsed = new Dictionary<string, DateTime>();
        private void Load_Patches()
        {
            if(patches == null)
            {
                patches = XDocument.Load(patch_file);
                var patchEles = patches.Element("patches").Elements("patch");
                foreach (var patch in patchEles)
                {
                    var ver = patch.Attribute("version").Value;
                    //var datePieces = patch.Attribute("date").Value.Split('/');
                    //var month = Int32.Parse(datePieces[0]);
                    //var day = Int32.Parse(datePieces[1]);
                    //var year = Int32.Parse(datePieces[2]);
                    //var date = new DateTime(year, month, day);
                    var date = Convert.ToDateTime(patch.Attribute("date").Value);
                    patch_parsed.Add(ver, date);
                }
            }
        }

        private void Save_Patches()
        {
            using (System.IO.StreamWriter file2 = new System.IO.StreamWriter(patch_file))
            {
                patches.Save(file2, SaveOptions.None);
            }
        }
        private void SymLink_Patches()
        {
            Load_Patches();
            var patchEles = patches.Element("patches").Elements();
            OverallProgressChanged(this, 0);
            int count = patchEles.Count();
            int i = 0;

            XElement prevElement = null;
            foreach (var element in patchEles)
            {
                string target = String.Format("{0}\\patches\\{1}\\", outputDir, element.Attribute("version").Value);
                // AddItem(target);
                foreach (var mankey in manifests)
                {
                    if (element.Attribute(mankey) == null)
                        continue;
                    // Fix for missing files

                    string source = "";
                    // Workaround for missing files
                    if (i == 0)
                    {
                        int val = 0;
                        int.TryParse(element.Attribute(mankey).Value, out val);
                        // Start at expected and go backwards down to 0 if not found (can't symlink existing links)
                        for (int j = val; j > -1; j--)
                        // for (int j = -1; j < val; j++)
                        {
                            source = String.Format("{0}\\base\\{1}\\{2}\\", baseDir, mankey, j);
                            SymLink_Dir(target, source);
                        }
                    }
                    else
                    {
                        // Try to symlink the correct patch, if exists
                        source = String.Format("{0}\\base\\{1}\\{2}\\", baseDir, mankey, element.Attribute(mankey).Value);
                        SymLink_Dir(target, source);
                        // Try to symlink the previous patch (will only work if correct one didn't symlink)
                        if (prevElement != null)
                        {
                            source = String.Format("{0}\\base\\{1}\\{2}\\", baseDir, mankey, prevElement.Attribute(mankey).Value);
                            SymLink_Dir(target, source);
                        }
                    }
                    // source = String.Format("{0}\\base\\{1}\\{2}\\", baseDir, mankey, element.Attribute(mankey).Value);
                    // SymLink_Dir(target, source);
                }
                prevElement = element;
                i++;
                OverallProgressChanged(this, 100 * i / count);
            }
            AddItem("Finish");
        }

        private void SymLink_Dir(string target, string source)
        {
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

        private void Generate_Output()
        {
            var patchEles = patches.Element("patches").Elements();
            var strings = new List<string> { "6.2.0b" };
            
            foreach (var element in patchEles)
            {
                string target = String.Format("{0}\\patches\\{1}\\", outputDir, element.Attribute("version"));
                AddItem(target);
                string patch = element.Attribute("version").Value;
                foreach (var mankey in manifests)
                {
                    if (element.Attribute(manifests[0]) == null)
                    {
                        AddItem(String.Format("No Asset Changes {0}", patch));
                        continue;
                    }
                }
                AddItem(String.Format("Outputting {0}", patch));

                bool contains = strings.Contains(patch, StringComparer.OrdinalIgnoreCase);
                if (contains) {
                    using (Process deltaProcess = new Process())
                    {
                        //Maybe this shouldn't be hard coded? Could do this in memory instead to.
                        // deltaProcess.StartInfo.FileName = "ConsoleTools.exe";
                        deltaProcess.StartInfo.FileName = "E:\\Game Rush\\Sources\\TORCommunity\\GitHub\\PugTools\\ConsoleTools\\Bin\\ConsoleTools.exe";

                        deltaProcess.StartInfo.CreateNoWindow = true;
                        deltaProcess.StartInfo.UseShellExecute = false;
                        deltaProcess.StartInfo.RedirectStandardOutput = true;
                        deltaProcess.StartInfo.RedirectStandardError = false;
                        // deltaProcess.StartInfo.Arguments = String.Format("{0} {1} {2}", patch, baseDir, outputDir);
                        var args = String.Format("{0} {1} {2} {3}", patch, baseDir + "\\patches\\", outputDir + "\\processed\\", "false");
                        // var args = new string[] { patch, baseDir + "\\patches\\", outputDir + "\\processed\\", "false" };
                        deltaProcess.StartInfo.Arguments = args;
                        deltaProcess.Start();
                        string output = deltaProcess.StandardOutput.ReadToEnd();
                        Console.WriteLine(output);
                        deltaProcess.WaitForExit();
                    }
                }
            }
        }

        public enum WorkerFuncs
        {
            Insert = 0,
            Symlink = 1,
            Output = 2,
            Manual = 3,
            Upcoming = 4
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
                case WorkerFuncs.Manual:
                    Manual_Walk(); break;
                case WorkerFuncs.Upcoming:
                    Upcoming_Build_Search(); break;
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

        private void Manual_Button_Click(object sender, EventArgs e)
        {
            var db_worker = new BackgroundWorker();
            db_worker.DoWork += new DoWorkEventHandler(DBWorker_DoWork);
            db_worker.RunWorkerAsync(WorkerFuncs.Manual);
        }

        private void ptsCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ptsCheckBox.Checked)
            {
                betaCheckBox.Checked = false;
                manifests = full_manifests["PTS"];
                patch_file = @"pts.xml";
            }
            else
            {
                manifests = full_manifests["Live"];
                patch_file = @"patches.xml";
            }
        }

        private void betaCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (betaCheckBox.Checked)
            {
                ptsCheckBox.Checked = false;
                manifests = full_manifests["Beta"];
                patch_file = @"beta.xml";
            }
            else
            {
                manifests = full_manifests["Live"];
                patch_file = @"patches.xml";
            }
        }

        private void upcoming_button_Click(object sender, EventArgs e)
        {
            var db_worker = new BackgroundWorker();
            db_worker.DoWork += new DoWorkEventHandler(DBWorker_DoWork);
            db_worker.RunWorkerAsync(WorkerFuncs.Upcoming);
        }

        private void verify_button_Click(object sender, EventArgs e)
        {
            WalkWorker.RunWorkerAsync(WalkType.Verify);
        }
    }
}
