using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_ESI_ESI : Instruction, IXor
    {
        public XOR_ESI_ESI()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_ESI_ESI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR ESI, ESI";
        }

        public void XorValue(Registers register)
        {
            register.ESI ^= register.ESI;
        }
    }
}