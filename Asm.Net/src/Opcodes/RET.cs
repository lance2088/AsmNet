using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes
{
    public class RET : Instruction, IReturn
    {
        public RET()
            : base(1, typeof(IReturn))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.RET };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "RET";
        }
    }
}