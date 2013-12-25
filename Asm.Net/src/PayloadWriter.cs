using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src
{
    internal class PayloadWriter : IDisposable
    {
        private List<byte> stream;
        public PayloadWriter()
        {
            stream = new List<byte>();
        }

        public void WriteBytes(byte[] value)
        {
            stream.AddRange(value);
        }

        public void WriteInteger(int value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        public void WriteShort(short value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        public void WriteByte(byte value)
        {
            stream.Add(value);
        }

        public void WriteDouble(double value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteLong(long value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }
        public void WriteFloat(float value)
        {
            WriteBytes(BitConverter.GetBytes(value));
        }

        public void WriteString(string value)
        {
            if (!(value == null))
                WriteBytes(System.Text.Encoding.Unicode.GetBytes(value));
            else
                WriteBytes(System.Text.Encoding.Unicode.GetBytes(""));
            WriteByte(0);
            WriteByte(0);
        }

        public byte[] ToByteArray()
        {
            return stream.ToArray();
        }

        public long Length
        {
            get { return stream.Count; }
        }

        public void Dispose()
        {
            if (stream != null)
            {
                stream.Clear();
                stream = null;
            }
        }
    }
}