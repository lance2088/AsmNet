using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_EDI_EDX : Instruction, IXor
    {
        public XOR_EDI_EDX()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_EDI_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR EDI, EDX";
        }

        public void XorValue(Registers register)
        {
            register.EDI ^= register.EDX;
        }
    }
}