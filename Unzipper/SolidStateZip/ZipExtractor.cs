using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace SolidStateZip
{
    public class ZipExtractor
    {
        public ZipExtractor(Action<string> _MessageDelegate, bool _useSymChk, string _baseFileDir, string _outputDir, string _tempDir)
        {
            MessageDelegate = _MessageDelegate;
            useSymChk = _useSymChk;
            baseFileDir = _baseFileDir;
            outputDir = _outputDir;
            tempDir = _tempDir;
        }
        public Action<string> MessageDelegate { get; set; }

        public void InvokeMessage(String mess)
        {
            if (MessageDelegate != null)
            {
                MessageDelegate(mess);
            }
        }
        bool useSymChk { get; set; }
        string baseFileDir { get; set; }
        string outputDir { get; set; }
        string tempDir { get; set; }

        private HashSet<string> Passwords { get; set; }
        private HashSet<string> DeletedArchives { get; set; }
        private string ArchiveType;
        public bool Unzip(ref BinaryReader brReader, string fullFileName, bool execXdelta)
        {
            ArchiveType = "";
            DeletedArchives = new HashSet<string>();
            var fileName = fullFileName.Substring(fullFileName.LastIndexOf("\\") + 1);

            if (brReader.BaseStream.Length < 0x1800)
            {
                MessageBox.Show("ERROR: The file " + fullFileName + " is too small to be a zip file.");
                return false;
            }
            brReader.BaseStream.Position = brReader.BaseStream.Length - 4L; // = size of certificates + end of central dir
            // find end of central dir
            while (brReader.BaseStream.Position > 0)
            {
                if (brReader.ReadUInt32() == 0x6054B50)
                {
                    long centralDirPos = brReader.BaseStream.Position - 4L;
                    ushort numDisks = brReader.ReadUInt16();
                    brReader.ReadBytes(4); // skip number of disk with start of central directory AND total number of entries in central dir on this disk
                    ushort numFiles = brReader.ReadUInt16();
                    uint centralDirSize = brReader.ReadUInt32();
                    uint centralDirOffset = brReader.ReadUInt32();
                    // go to central file headers
                    brReader.BaseStream.Position = centralDirPos - centralDirSize;
                    while (brReader.ReadUInt32() == 0x2014B50)
                    {
                        brReader.ReadBytes(24);
                        uint fileNameLength = brReader.ReadUInt16();
                        uint extraFieldLength = brReader.ReadUInt16();
                        brReader.ReadBytes(2);
                        uint diskNumberStart = brReader.ReadUInt16(); // disk number start
                        brReader.ReadBytes(6);
                        uint relOffset = brReader.ReadUInt32(); // relative offset to local fileheader
                        string curFileName = Encoding.Default.GetString(brReader.ReadBytes((int)fileNameLength));
                        string curFileNiceName = curFileName;

                        // read extra field
                        long extraFieldStart = brReader.BaseStream.Position;
                        byte[] password = { };
                        int diffType = -1;
                        while (brReader.BaseStream.Position + 4 <= extraFieldStart + extraFieldLength)
                        {
                            ushort tmpId = brReader.ReadUInt16();
                            ushort tmpSize = brReader.ReadUInt16();
                            //if (tmpId == 0x8810)// password in unmodified form
                            //    password = brReader.ReadBytes(tmpSize);
                            //else
                            //    brReader.ReadBytes(tmpSize);
                            switch (tmpId)
                            {
                                case 0x8810:
                                    password = brReader.ReadBytes(tmpSize);
                                    break;
                                case 0x80AE:
                                    diffType = (int)brReader.ReadUInt32();
                                    brReader.ReadBytes(56);
                                    switch (diffType)
                                    {
                                        case 0: curFileNiceName += " - Added"; break;
                                        case 1: curFileNiceName += " - Deleted"; break;
                                        case 2: curFileNiceName += " - VCDIFF"; break;
                                        case 3: curFileNiceName += " - Unchanged"; break;
                                    }
                                    break;
                                default:
                                    brReader.ReadBytes(tmpSize);
                                    break;
                            }
                        }
                        if(MessageDelegate != null)
                            InvokeMessage(curFileNiceName);
                        // go to local file header
                        if (centralDirOffset > 0)
                        { // local headers are stored in this file
                            long oldPos = brReader.BaseStream.Position;
                            brReader.BaseStream.Position = centralDirPos - centralDirSize - centralDirOffset + relOffset;
                            ZipReader zipReader = new ZipReader(ref brReader);
                            readLocalHeader(ref zipReader, password, fileName, curFileName, diffType, execXdelta);
                            brReader.BaseStream.Position = oldPos;
                        }
                        else
                        {  // local headers are stored in a different file, like .z01
                            //if(diskNumberStart == 0 && !curFileName.Contains("bmaf"))
                            //{
                            //    continue;
                            //}
                            ZipReader zipReader = new ZipReader(fullFileName, (int)diskNumberStart, relOffset);
                            readLocalHeader(ref zipReader, password, fileName, curFileName, diffType, execXdelta);
                            zipReader.Dispose();
                        }
                    }
                    brReader.BaseStream.Position = centralDirPos - centralDirSize - centralDirOffset;
                    while (brReader.ReadUInt32() == 0x4034B50) { }

                    if (ArchiveType.Length > 0 && execXdelta && useSymChk)
                    {
                        //When we have succesfully finished reading the archive if we are using symlinks and patching create symlinks for assets folder.
                        string patchBaseDir = baseFileDir;
                        if (!patchBaseDir.EndsWith("\\"))
                        {
                            patchBaseDir += "\\Assets\\";
                        }
                        else
                        {
                            patchBaseDir += "Assets\\";
                        }

                        if (!Directory.Exists(patchBaseDir))
                        {
                            //Make sure the base has assets.
                            return true;
                        }

                        string[] basePatchFiles = Directory.GetFiles(patchBaseDir);
                        string patchTargetDir = outputDir;
                        if (!patchTargetDir.EndsWith("\\"))
                        {
                            patchTargetDir += "\\Assets\\";
                        }
                        else
                        {
                            patchTargetDir += "Assets\\";
                        }
                        Directory.CreateDirectory(patchTargetDir);

                        foreach (string basePatchFilePath in basePatchFiles)
                        {
                            string basePatchFileName = basePatchFilePath.Substring(basePatchFilePath.LastIndexOf("\\") + 1);
                            if (DeletedArchives.Contains(basePatchFileName) || !basePatchFileName.Contains(ArchiveType))
                            {
                                //Don't symlink deleted archives and only sym link archives of this patch type
                                continue;
                            }

                            if (!File.Exists(patchTargetDir + basePatchFileName))
                            {
                                //Check to make sure the target file doesn't exist and then create the symlink.
                                CreateSymbolicLink(patchTargetDir + basePatchFileName, basePatchFilePath);
                            }
                        }
                    }

                    return true;
                }
                brReader.BaseStream.Position -= 5; // go back one byte (plus the four bytes we//ve just read)
            }
            MessageBox.Show("ERROR: The end of central dir could not be found in " + fullFileName + ".");
            return false;
        }

        public Stream UnzipSingleFile(ref BinaryReader brReader, string targetFile)
        {
            ArchiveType = "";
            DeletedArchives = new HashSet<string>();

            if (brReader.BaseStream.Length < 0x1800)
            {
                MessageBox.Show("ERROR: The file is too small to be a zip file.");
                return null;
            }
            brReader.BaseStream.Position = brReader.BaseStream.Length - 4L; // = size of certificates + end of central dir
            // find end of central dir
            while (brReader.BaseStream.Position > 0)
            {
                if (brReader.ReadUInt32() == 0x6054B50)
                {
                    long centralDirPos = brReader.BaseStream.Position - 4L;
                    ushort numDisks = brReader.ReadUInt16();
                    brReader.ReadBytes(4); // skip number of disk with start of central directory AND total number of entries in central dir on this disk
                    ushort numFiles = brReader.ReadUInt16();
                    uint centralDirSize = brReader.ReadUInt32();
                    uint centralDirOffset = brReader.ReadUInt32();
                    // go to central file headers
                    brReader.BaseStream.Position = centralDirPos - centralDirSize;
                    while (brReader.ReadUInt32() == 0x2014B50)
                    {
                        brReader.ReadBytes(24);
                        uint fileNameLength = brReader.ReadUInt16();
                        uint extraFieldLength = brReader.ReadUInt16();
                        brReader.ReadBytes(2);
                        uint diskNumberStart = brReader.ReadUInt16(); // disk number start
                        brReader.ReadBytes(6);
                        uint relOffset = brReader.ReadUInt32(); // relative offset to local fileheader
                        string curFileName = Encoding.Default.GetString(brReader.ReadBytes((int)fileNameLength));
                        string curFileNiceName = curFileName;
                        if (curFileName != targetFile)
                            return null;

                        // read extra field
                        long extraFieldStart = brReader.BaseStream.Position;
                        byte[] password = { };
                        int diffType = -1;
                        while (brReader.BaseStream.Position + 4 <= extraFieldStart + extraFieldLength)
                        {
                            ushort tmpId = brReader.ReadUInt16();
                            ushort tmpSize = brReader.ReadUInt16();
                            //if (tmpId == 0x8810)// password in unmodified form
                            //    password = brReader.ReadBytes(tmpSize);
                            //else
                            //    brReader.ReadBytes(tmpSize);
                            switch (tmpId)
                            {
                                case 0x8810:
                                    password = brReader.ReadBytes(tmpSize);
                                    break;
                                case 0x80AE:
                                    diffType = (int)brReader.ReadUInt32();
                                    brReader.ReadBytes(56);
                                    switch (diffType)
                                    {
                                        case 0: curFileNiceName += " - Added"; break;
                                        case 1: curFileNiceName += " - Deleted"; break;
                                        case 2: curFileNiceName += " - VCDIFF"; break;
                                        case 3: curFileNiceName += " - Unchanged"; break;
                                    }
                                    break;
                                default:
                                    brReader.ReadBytes(tmpSize);
                                    break;
                            }
                        }
                        if (MessageDelegate != null)
                            InvokeMessage(curFileNiceName);
                        // go to local file header
                        if (centralDirOffset > 0)
                        { // local headers are stored in this file
                            long oldPos = brReader.BaseStream.Position;
                            brReader.BaseStream.Position = centralDirPos - centralDirSize - centralDirOffset + relOffset;
                            ZipReader zipReader = new ZipReader(ref brReader);
                            //readLocalHeader(ref zipReader, password, "", curFileName, diffType, false);
                            if (brReader.ReadUInt32() != 0x4034B50)
                            {
                                MessageBox.Show("ERROR: Magic number is wrong for local file header for manifext.xml");
                                return null;
                            }
                            brReader.ReadBytes(2); // skip version
                            ushort bitflag = brReader.ReadUInt16();
                            ushort compression = brReader.ReadUInt16();
                            ushort lastModTime = brReader.ReadUInt16();
                            ushort lastModDate = brReader.ReadUInt16();
                            brReader.ReadBytes(4); // skip CRC
                            uint comprSize = brReader.ReadUInt32();
                            uint uncomprSize = brReader.ReadUInt32();
                            ushort manFileNameLength = brReader.ReadUInt16();
                            ushort manExtraFieldLength = brReader.ReadUInt16();
                            brReader.ReadBytes(manFileNameLength);
                            brReader.ReadBytes(manExtraFieldLength); // skip extra field
                            byte[] file = brReader.ReadBytes((int)comprSize); // read file
                            if (uncomprSize == 0)
                            {
                                MessageBox.Show("ERROR: The file manifest.xml was empty; it will not be extracted.");
                            }
                            if (uncomprSize > 0 && (bitflag & 1) == 1)
                            {// if it has encryption
                             // modify password
                                modifyPassword(ref password);
                                //initialize keys
                                uint key_0 = 0x12345678;
                                uint key_1 = 0x23456789;
                                uint key_2 = 0x34567890;
                                //update keys against password
                                for (int i = 0; i < password.Length; i++)
                                {
                                    if (password[i] == 0) break;
                                    updateKeys(password[i], ref key_0, ref key_1, ref key_2);
                                }
                                //decrypt file
                                for (int i = 0; i < file.Length; i++)
                                {
                                    file[i] = (byte)(file[i] ^ decrypt_byte(key_2));
                                    updateKeys(file[i], ref key_0, ref key_1, ref key_2);
                                }
                                //Remove the ZipCrypto header. Should probably use this to verify password just in case?
                                Array.Copy(file, 12, file, 0, file.Length - 12);
                            }
                            if (uncomprSize > 0 && compression == 8)
                            { //uncompress file
                                try
                                {
                                    //using (System.IO.FileStream file2 = new System.IO.FileStream("i:\\"+ curFileName, FileMode.Create, FileAccess.Write))
                                    //{
                                    //    file2.Write(file, 0, file.Length);
                                    //}
                                    file = Ionic.Zlib.DeflateStream.UncompressBuffer(file);

                                }
                                catch (Exception e)
                                {
                                    MessageBox.Show("ERROR: Something went wrong, could not uncompress:" + Environment.NewLine + e.ToString());
                                    return null;
                                }
                            }
                            else if (uncomprSize > 0 && compression > 0)
                            {
                                MessageBox.Show("ERROR: The compression " + compression + " was not recognized in " + curFileName);
                                return null;
                            }
                            //extract file
                            if (uncomprSize > 0)
                            {
                                MemoryStream memStream = new MemoryStream(file);
                                return memStream;
                            }
                            brReader.BaseStream.Position = oldPos;
                        }
                    }
                    brReader.BaseStream.Position = centralDirPos - centralDirSize - centralDirOffset;
                    while (brReader.ReadUInt32() == 0x4034B50) { }

                    
                }
                brReader.BaseStream.Position -= 5; // go back one byte (plus the four bytes we//ve just read)
            }
            //MessageBox.Show("ERROR: The end of central dir could not be found in " + fullFileName + ".");
            //return false;
            return null;
        }

        private void readLocalHeader(ref ZipReader brReader, byte[] password, string fileName, string curFileName, int diffType, bool doPatch)
        {
            if (brReader.ReadUInt32() != 0x4034B50)
            {
                MessageBox.Show("ERROR: Magic number is wrong for local file header for " + curFileName + "; will skip that file");
                return;
            }
            brReader.ReadBytes(2); // skip version
            ushort bitflag = brReader.ReadUInt16();
            ushort compression = brReader.ReadUInt16();
            ushort lastModTime = brReader.ReadUInt16();
            ushort lastModDate = brReader.ReadUInt16();
            brReader.ReadBytes(4); // skip CRC
            uint comprSize = brReader.ReadUInt32();
            uint uncomprSize = brReader.ReadUInt32();
            ushort fileNameLength = brReader.ReadUInt16();
            ushort extraFieldLength = brReader.ReadUInt16();
            brReader.ReadBytes(fileNameLength);
            brReader.ReadBytes(extraFieldLength); // skip extra field
            byte[] file = brReader.ReadBytes((int)comprSize); // read file
            if (uncomprSize == 0)
            {
                if (diffType == 2)
                    MessageBox.Show("ERROR: The file " + curFileName + " was empty; it will not be extracted.");
                //else
                //added/changed/deleted
                //return;
            }
            if (uncomprSize > 0 && (bitflag & 1) == 1)
            {// if it has encryption
             // modify password
                modifyPassword(ref password);
                //initialize keys
                uint key_0 = 0x12345678;
                uint key_1 = 0x23456789;
                uint key_2 = 0x34567890;
                //update keys against password
                for (int i = 0; i < password.Length; i++)
                {
                    if (password[i] == 0) break;
                    updateKeys(password[i], ref key_0, ref key_1, ref key_2);
                }
                //decrypt file
                for (int i = 0; i < file.Length; i++)
                {
                    file[i] = (byte)(file[i] ^ decrypt_byte(key_2));
                    updateKeys(file[i], ref key_0, ref key_1, ref key_2);
                }
                //Remove the ZipCrypto header. Should probably use this to verify password just in case?
                Array.Copy(file, 12, file, 0, file.Length - 12);
            }
            if (uncomprSize > 0 && compression == 8)
            { //uncompress file
                try
                {
                    //using (System.IO.FileStream file2 = new System.IO.FileStream("i:\\"+ curFileName, FileMode.Create, FileAccess.Write))
                    //{
                    //    file2.Write(file, 0, file.Length);
                    //}
                    file = Ionic.Zlib.DeflateStream.UncompressBuffer(file);

                }
                catch (Exception e)
                {
                    MessageBox.Show("ERROR: Something went wrong, could not uncompress:" + Environment.NewLine + e.ToString());
                    return;
                }
            }
            else if (uncomprSize > 0 && compression > 0)
            {
                MessageBox.Show("ERROR: The compression " + compression + " was not recognized in " + curFileName);
                return;
            }
            //extract file
            if (!doPatch && uncomprSize > 0)
            {
                //Just write the data without patching.
                string fullName = tempDir + fileName.Replace(".", "-") + "\\" + curFileName.Replace("/", "\\");
                if (fullName.Contains("\\"))
                    Directory.CreateDirectory(fullName.Substring(0, fullName.LastIndexOf("\\")));

                File.WriteAllBytes(fullName, file);
                File.SetLastWriteTime(fullName, new DateTime(1980 + (lastModDate >> 9), (lastModDate & 0x1E0) >> 5, lastModDate & 0x1F, lastModTime >> 11, (lastModTime & 0x7E0) >> 5, (lastModTime & 0x1F) * 2));
            }
            else
            {
                if (ArchiveType.Length == 0)
                {
                    //Determine asset names to symlink later. We can count on the version file being here.
                    ArchiveType = (curFileName ?? "").Replace("assets_", "").Replace(".version", "_");
                    //switch (curFileName)
                    //{
                    //    case "assets_swtor_main.version":
                    //        ArchiveType = "swtor_main_";
                    //        break;
                    //    case "assets_swtor_en_us.version":
                    //        ArchiveType = "swtor_en_us_";
                    //        break;
                    //    case "assets_swtor_de_de.version":
                    //        ArchiveType = "swtor_en_us_";
                    //        break;
                    //    case "assets_swtor_fr_fr.version":
                    //        ArchiveType = "swtor_en_us_";
                    //        break;
                    //    case "assets_swtor_test_main.version":
                    //        ArchiveType = "swtor_test_main_";
                    //        break;
                    //    case "assets_swtor_test_en_us.version":
                    //        ArchiveType = "swtor_test_en_us_";
                    //        break;
                    //    case "assets_swtor_test_de_de.version":
                    //        ArchiveType = "swtor_test_en_us_";
                    //        break;
                    //    case "assets_swtor_test_fr_fr.version":
                    //        ArchiveType = "swtor_test_en_us_";
                    //        break;
                    //    default:
                    //        break;
                    //}
                }

                //Perform patching.
                switch (diffType)
                {
                    case 0:
                        {
                            //Added
                            string fullName = outputDir + curFileName.Replace("/", "\\");
                            if (fullName.Contains("\\"))
                            {
                                //Just in case the patch output path wasn't created ahead of time.
                                Directory.CreateDirectory(fullName.Substring(0, fullName.LastIndexOf("\\")));
                            }

                            //Write data and set timestamp. Existing file should be overwritten.
                            File.WriteAllBytes(fullName, file);
                            File.SetLastWriteTime(fullName, new DateTime(1980 + (lastModDate >> 9), (lastModDate & 0x1E0) >> 5, lastModDate & 0x1F, lastModTime >> 11, (lastModTime & 0x7E0) >> 5, (lastModTime & 0x1F) * 2));
                            break;
                        }

                    case 1:
                        {
                            //Deleted.
                            string fullName = outputDir + curFileName.Replace("/", "\\");
                            DeletedArchives.Add(curFileName);
                            if (File.Exists(fullName))
                            {
                                File.Delete(fullName);
                            }
                            break;
                        }
                    case 2:
                        {
                            //VCDIFF. Use Xdelta to patch.
                            //Extract data to temp folder.
                            string tempName = tempDir + fileName.Replace(".", "-") + "\\" + curFileName.Replace("/", "\\");
                            if (tempName.Contains("\\"))
                                Directory.CreateDirectory(tempName.Substring(0, tempName.LastIndexOf("\\")));

                            File.WriteAllBytes(tempName, file);
                            DateTime fileTimestamp = new DateTime(1980 + (lastModDate >> 9), (lastModDate & 0x1E0) >> 5, lastModDate & 0x1F, lastModTime >> 11, (lastModTime & 0x7E0) >> 5, (lastModTime & 0x1F) * 2);
                            File.SetLastWriteTime(tempName, fileTimestamp);

                            //Get output path and source path.
                            string patchName = outputDir + curFileName.Replace("/", "\\");
                            string baseName = patchName.Replace(outputDir, baseFileDir);
                            if (patchName.Contains("\\"))
                            {
                                Directory.CreateDirectory(patchName.Substring(0, patchName.LastIndexOf("\\")));
                            }

                            if (File.Exists(patchName))
                            {
                                //If file exists then delete it. Used due to symlinks potentially being here.
                                File.Delete(patchName);
                            }

                            using (Process deltaProcess = new Process())
                            {
                                //Maybe this shouldn't be hard coded? Could do this in memory instead to.
                                deltaProcess.StartInfo.FileName = "xdelta3-x86_64-3.0.10.exe";

                                deltaProcess.StartInfo.CreateNoWindow = true;
                                deltaProcess.StartInfo.UseShellExecute = false;
                                deltaProcess.StartInfo.RedirectStandardOutput = true;
                                deltaProcess.StartInfo.Arguments = "-d -s \"" + baseName + "\" \"" + tempName + "\" \"" + patchName + "\"";
                                deltaProcess.Start();
                                if (MessageDelegate != null)
                                    InvokeMessage("Started patching " + curFileName);
                                string output = deltaProcess.StandardOutput.ReadToEnd();
                                Console.WriteLine(output);
                                deltaProcess.WaitForExit();
                            }

                            //Set the timestamp on the patched file.
                            File.SetLastWriteTime(patchName, fileTimestamp);
                            //Delete the temp patch file.
                            File.Delete(tempName);

                            break;
                        }
                    case 3:
                        {
                            string fullName = outputDir + curFileName.Replace("/", "\\");
                            if (!useSymChk)
                            {
                                //No changes. Not using sym links. Update timestamp.
                                if (File.Exists(fullName))
                                {
                                    File.SetLastWriteTime(fullName, new DateTime(1980 + (lastModDate >> 9), (lastModDate & 0x1E0) >> 5, lastModDate & 0x1F, lastModTime >> 11, (lastModTime & 0x7E0) >> 5, (lastModTime & 0x1F) * 2));
                                }
                            }
                            else
                            {
                                //No changes. Using sym link, link back to base.
                                if (fullName.Contains("\\"))
                                {
                                    //Just in case the patch output path wasn't created ahead of time.
                                    Directory.CreateDirectory(fullName.Substring(0, fullName.LastIndexOf("\\")));
                                }

                                string baseName = baseFileDir + curFileName.Replace("/", "\\");
                                if (File.Exists(baseName) && !File.Exists(fullName))
                                {
                                    if (!CreateSymbolicLink(fullName, baseName))
                                    {
                                        //Sym link failed?!?
                                        throw new Exception();
                                    }
                                }
                            }

                            break;
                        }
                }
            }
        }

        private void modifyPassword(ref byte[] pwd)
        {
            for (int i = 0; i < pwd.Length; i++)
            {
                if (pwd[i] == 0) break;
                //step 1: add (1 << pos) to first 8 bytes of a 32 byte block
                if ((i % 32) < 8)
                {
                    pwd[i] = Convert.ToByte((Convert.ToUInt16(pwd[i]) + (1 << (i % 32))) & 0xFF);
                }
                //step 2: substract 0x80 from all bytes > 0x7E
                if (pwd[i] == 0xFF)
                    pwd[i] = 0x3F; // unset the 0x80 and 0x40 bit
                else if (pwd[i] > 0x7F)
                    pwd[i] = (byte)(pwd[i] - 0x80); // unset the 0x80 bit
                else if (pwd[i] == 0x7F)
                    pwd[i] = 0x3F; // unset the 0x40 bit
                                   //alternate code for step 2:
                                   //if( pwd[i] > 0x7E  ) {
                                   //    pwd[i] = pwd[i] And Convert.ToByte(0x7F)
                                   //    if( pwd[i] > 0x7E  ) {
                                   //        pwd[i] = pwd[i] And Convert.ToByte(0x3F)
                                   //    }
                                   //}
                                   //step 3: change all bytes < 0x21
                if (pwd[i] < 0x21)
                    pwd[i] = (byte)((pwd[i] | (1 << ((pwd[i] % 3) + 5))) + 1);
            }
        }

        private void updateKeys(byte curChar, ref uint key_0, ref uint key_1, ref uint key_2)
        {
            key_0 = crc32(key_0, curChar); ;
            key_1 += (byte)key_0;
            key_1 = key_1 * 0x08088405 + 1;
            key_2 = crc32(key_2, (byte)(key_1 >> 24));
        }

        private byte decrypt_byte(uint key_2)
        {
            ushort t = (ushort)((ushort)(key_2 & 0xFFFF) | 2);
            return (byte)((t * (t ^ 1)) >> 8);
        }

        private void outputHashes(uint key_0, uint key_1, uint key_2, byte curChar)
        {
            ListViewItem item = new ListViewItem(curChar.ToString("X2"));
            item.SubItems.Add(this.uintToHex(key_0));
            item.SubItems.Add(this.uintToHex(key_1));
            item.SubItems.Add(this.uintToHex(key_2));
            //AddItem(item);
        }

        private string uintToHex(uint inty)
        {
            return (inty & 0xFF).ToString("X2") + " " + ((inty & 0xFF00) >> 8).ToString("X2") + " " + ((inty & 0xFF0000) >> 16).ToString("X2") + " " + ((inty & 0xFF000000) >> 24).ToString("X2");
        }

        private string Bytes_To_String(byte[] bytes_Input)
        {
            StringBuilder strTemp = new StringBuilder(bytes_Input.Length * 2);
            foreach (byte b in bytes_Input)
                strTemp.Append(b.ToString("X02") + " ");

            return strTemp.ToString();
        }

        private static readonly uint[] TLookup = new uint[] {
                0, 0x77073096, 0xee0e612c, 0x990951ba, 0x76dc419, 0x706af48f, 0xe963a535, 0x9e6495a3, 0xedb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988, 0x9b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91,
                0x1db71064, 0x6ab020f2, 0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7, 0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9, 0xfa0f3d63, 0x8d080df5,
                0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172, 0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c, 0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
                0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423, 0xcfba9599, 0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924, 0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d,
                0x76dc4190, 0x1db7106, 0x98d220bc, 0xefd5102a, 0x71b18589, 0x6b6b51f, 0x9fbfe4a5, 0xe8b8d433, 0x7807c9a2, 0xf00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x86d3d2d, 0x91646c97, 0xe6635c01,
                0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950, 0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
                0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7, 0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9,
                0x5005713c, 0x270241aa, 0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f, 0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 0xb7bd5c3b, 0xc0ba6cad,
                0xedb88320, 0x9abfb3b6, 0x3b6e20c, 0x74b1d29a, 0xead54739, 0x9dd277af, 0x4db2615, 0x73dc1683, 0xe3630b12, 0x94643b84, 0xd6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0xa00ae27, 0x7d079eb1,
                0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb, 0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5,
                0xd6d6a3e8, 0xa1d1937e, 0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b, 0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 0x316e8eef, 0x4669be79,
                0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236, 0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28, 0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
                0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x26d930a, 0x9c0906a9, 0xeb0e363f, 0x72076785, 0x5005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0xcb61b38, 0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0xbdbdf21,
                0x86d3d2d4, 0xf1d4e242, 0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777, 0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 0x616bffd3, 0x166ccf45,
                0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2, 0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc, 0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
                0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693, 0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94, 0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d
             };
        private uint crc32(uint crc, byte c)
        {
            return TLookup[(crc ^ c) & 0xFF] ^ (crc >> 8);
        }

        //Invoke Windows API for sym link.
        enum SymbolicLink
        {
            File = 0,
            Directory = 1
        }

        /// <summary>
        /// Create a symbolic link.</summary>
        /// <param name="lpSymlinkFileName">Where to create the symbolic link.</param>
        /// <param name="lpTargetFileName">Where the symbolic link points to (source file / directory).</param>
        /// <param name="dwFlags">Link for a file or a directory? Default = file.</param>
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.I1)]
        static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, SymbolicLink dwFlags = SymbolicLink.File);
    }
}
