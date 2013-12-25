using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Opcodes.AsmNet;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Asm.Net.src.Sections
{
    public class DataSection : Section
    {
        public SortedList<int, Variable> Variables { get; private set; }
        public List<byte> stream;

        public DataSection()
            : base()
        {
            stream = new List<byte>();
            Variables = new SortedList<int, Variable>();
        }
        public DataSection(byte[] data)
            : base()
        {
            stream = new List<byte>();
            Variables = new SortedList<int, Variable>();

            for (int i = 0; i < data.Length; )
            {
                switch (data[i])
                {
                    case 0xFF:
                    {
                        switch (data[i + 1])
                        {
                            case 0:
                            {
                                int length = BitConverter.ToInt32(data, i + 2);
                                byte[] payload = new byte[length];
                                Array.Copy(data, i + 6, payload, 0, payload.Length);
                                Variable var = (Variable)new BinaryFormatter().Deserialize(new MemoryStream(payload));
                                Variables.Add((int)stream.Count + Options.DataSectionBaseAddress, var);
                                stream.AddRange(var.ToByteArray());
                                i += 6 + length;
                                break;
                            }
                            default:
                            {
                                goto end;
                            }
                        }
                        break;
                    }
                    default:
                    {
                        goto end;
                    }
                }
            }

        end:
            stream.Clear();
            stream.AddRange(data);
        }

        /// <summary> Create a new variable with a type and value, the value needs to be Serializable </summary>
        public VirtualAddress CreateVariable(Object value)
        {
            Variable var = new Variable(value); //Type is not importtant right now
            var.VirtualAddress = new VirtualAddress(var.ToByteArray().Length, (int)stream.Count + Options.DataSectionBaseAddress);
            Variables.Add((int)stream.Count + Options.DataSectionBaseAddress, var);
            stream.AddRange(var.ToByteArray());
            return var.VirtualAddress;
        }

        public VirtualAddress AddString(string value)
        {
            VirtualAddress addr = new VirtualAddress(value.Length, (int)stream.Count);
            stream.AddRange(ASCIIEncoding.ASCII.GetBytes(value));
            stream.Add(0);
            return addr;
        }
        public bool LoadString(VirtualAddress addr, ref string ret)
        {
            if (addr.Address >= 0 && addr.Address <= stream.Count)
            {
                for (int i = addr.Address; i < stream.Count; i++)
                {
                    if (stream[i] == 0)
                        break;
                    ret += (char)stream[i];
                }
                return true;
            }
            return false;
        }
    }
}