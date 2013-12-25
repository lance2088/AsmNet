using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.ADD
{
    public class ADD_BYTE_PTR_EAX_AL : Instruction, IADD
    {
        public ADD_BYTE_PTR_EAX_AL()
            : base(1, typeof(IADD))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.ADD };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "ADD BYTE PTR [EAX], AL";
        }
    }
}