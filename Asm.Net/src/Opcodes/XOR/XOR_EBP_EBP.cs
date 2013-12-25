using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_EBP_EBP : Instruction, IXor
    {
        public XOR_EBP_EBP()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_EBP_EBP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR ESP, EBP";
        }

        public void XorValue(Registers register)
        {
            register.EBP ^= register.EBP;
        }
    }
}