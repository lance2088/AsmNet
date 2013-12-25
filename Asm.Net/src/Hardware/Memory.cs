using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Asm.Net.src.Hardware
{
    public class Memory : Hardware
    {
        private List<Byte> PeBytes;
        public long MaxMemory { get; private set; }
        public List<byte> MemoryStream;
        public SortedList<int, Instruction> Instructions { get; set; }

        public long UsedMemory
        {
            get
            {
                return PeBytes.Count + MemoryStream.Count;
            }
        }

        public Memory(long MaxMemory)
            : base()
        {
            this.Instructions = new SortedList<int, Instruction>();
            this.MaxMemory = MaxMemory;
            PeBytes = new List<Byte>(new byte[] { 0x4D, 0x5A });
            PeBytes.AddRange(new byte[] //DosStub: This program must be run under Win32
            {
                0x0E, 0x1F, 0xBA, 0x0E, 0x00, 0xB4, 0x09, 0xCD, 0x21, 0xB8, 0x01, 0x4C, 0xCD, 0x21, 0x54, 0x68,
                0x69, 0x73, 0x20, 0x70, 0x72, 0x6F, 0x67, 0x72, 0x61, 0x6D, 0x20, 0x63, 0x61, 0x6E, 0x6E, 0x6F,
                0x74, 0x20, 0x62, 0x65, 0x20, 0x72, 0x75, 0x6E, 0x20, 0x69, 0x6E, 0x20, 0x44, 0x4F, 0x53, 0x20,
                0x6D, 0x6F, 0x64, 0x65, 0x2E, 0x0D, 0x0D, 0x0A, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
            });
            MemoryStream = new List<byte>();
        }

        internal void InstructionsToPE(Instruction[] Instructions)
        {
            foreach (Instruction instruction in Instructions)
                PeBytes.AddRange(instruction.ToByteArray());

            PeBytes.AddRange(new byte[30]); //End Of File
        }

        public void DumpToFile(string FileLocation)
        {
            File.WriteAllBytes(FileLocation, PeBytes.ToArray());
        }

        public byte[] ToByteArray()
        {
            return PeBytes.ToArray();
        }

        public override string HardwareName
        {
            get { return ""; }
        }
    }
}