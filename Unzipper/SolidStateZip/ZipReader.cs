using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SolidStateZip
{
    public class ZipReader
    {
        private bool isInternal;
        private BinaryReader brReader;
        private string fileName;
        private int disknumber;
        public ZipReader(ref BinaryReader BrReader)
        {
            isInternal = true;
            brReader = BrReader;
        }

        public ZipReader(string fileName, int diskNumber, uint startOffset)
        {
            isInternal = false;
            this.fileName = fileName;
            this.disknumber = diskNumber;
            FileStream fileStream = new FileStream(fileName.Replace(".zip", ".z" + (diskNumber + 1).ToString("D2")), FileMode.Open, FileAccess.Read);
            brReader = new BinaryReader(fileStream);
            brReader.BaseStream.Position = startOffset;
        }

        public byte[] ReadBytes(int count)
        {
            if (isInternal)
                return brReader.ReadBytes(count);
            else {
                if (count > brReader.BaseStream.Length - brReader.BaseStream.Position)
                {// if it is spread over multiple disks
                    byte[] output = new byte[count];
                    // read part 1
                    count -= (int)(brReader.BaseStream.Length - brReader.BaseStream.Position);
                    brReader.Read(output, 0, (int)(brReader.BaseStream.Length - brReader.BaseStream.Position));
                    brReader.Close();
                    brReader.Dispose();
                    // read part 2
                    disknumber += 1;
                    using (FileStream fileStream = new FileStream(fileName.Replace(".zip", ".z" + (disknumber + 1).ToString("D2")), FileMode.Open, FileAccess.Read))
                    {
                        brReader = new BinaryReader(fileStream);
                        brReader.Read(output, output.Length - count, count);
                    }
                    // return output
                    return output;
                }
                else
                { // if it can be completely read in the current disk
                    return brReader.ReadBytes(count);
                }
            }
        }

        public ushort ReadUInt16()
        {
            byte[] byteArray = ReadBytes(2);
            return BitConverter.ToUInt16(byteArray, 0);
        }

        public uint ReadUInt32()
        {
            byte[] byteArray = ReadBytes(4);
            return BitConverter.ToUInt32(byteArray, 0);
        }

        public void Dispose()
        {
            if (!isInternal)
            {
                brReader.Close();
                brReader.Dispose();
            }
        }
    }
}
