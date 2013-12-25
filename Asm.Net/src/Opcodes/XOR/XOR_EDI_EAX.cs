using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_EDI_EAX : Instruction, IXor
    {
        public XOR_EDI_EAX()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_EDI_EAX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR EDI, EAX";
        }

        public void XorValue(Registers register)
        {
            register.EDI ^= register.EAX;
        }
    }
}