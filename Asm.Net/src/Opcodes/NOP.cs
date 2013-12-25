using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes
{
    public class NOP : Instruction, INop
    {
        public NOP()
            : base(1, typeof(INop))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.NOP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "NOP";
        }
    }
}