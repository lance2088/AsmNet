using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Asm.Net.src.Hardware.Harddisk
{
    public class File
    {
        public MemoryStream stream { get; set; }
        public string Name { get; set; }
        public File(string name)
        {
            this.Name = name;
            this.stream = new MemoryStream();
        }

        public void WriteBytes(byte[] values)
        {
            stream.Write(values, 0, values.Length);
        }
        public void WriteTextA(string value)
        {
            WriteBytes(ASCIIEncoding.ASCII.GetBytes(value));
        }
        public void WriteTextW(string value)
        {
            WriteBytes(ASCIIEncoding.Unicode.GetBytes(value));
        }
    }
}