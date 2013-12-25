using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_ESI_EDI : Instruction, IXor
    {
        public XOR_ESI_EDI()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_ESI_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR ESI, EDI";
        }

        public void XorValue(Registers register)
        {
            register.ESI ^= register.EDI;
        }
    }
}