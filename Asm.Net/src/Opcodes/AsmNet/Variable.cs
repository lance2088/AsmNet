using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Asm.Net.src.Opcodes.AsmNet
{
    [Serializable]
    public class Variable : Instruction
    {
        public Object Value { get; set; }

        public Variable(object value)
            : base(6, null)
        {
            this.Value = value;
            if (value != null)
            {
                if (value.GetType() == typeof(string))
                    this.VirtualAddress.Size = 6 + value.ToString().Length;
                else if (value.GetType() == typeof(byte[]))
                    this.VirtualAddress.Size = 6 + ((byte[])value).Length;
                else
                    this.VirtualAddress.Size = 6 + Marshal.SizeOf(value);
            }
        }

        public override byte[] ToByteArray()
        {
            //lets serialize the variable, don't want to make alot of switch/cases
            //and checking here the type hardcoded will not allow you to use structs, classes to use
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            bf.Serialize(stream, this);
            List<Byte> bytes = new List<Byte>();
            bytes.AddRange(new byte[] { 0xFF, 0 });
            bytes.AddRange(BitConverter.GetBytes((int)stream.Length));
            bytes.AddRange(stream.ToArray());
            stream.Close();
            stream.Dispose();
            bf = null;
            return bytes.ToArray();
        }

        public static Variable ByteArrayToVariable(byte[] data)
        {
            return (Variable)new BinaryFormatter().Deserialize(new MemoryStream(data));
        }

        public override void Dispose()
        {
            Value = null;
        }

        public override string ToString()
        {
            return "variable";
        }
    }
}