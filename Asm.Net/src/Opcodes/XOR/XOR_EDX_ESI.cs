using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_EDX_ESI : Instruction, IXor
    {
        public XOR_EDX_ESI()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_EDX_ESI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR EDX, ESI";
        }

        public void XorValue(Registers register)
        {
            register.EDX ^= register.ESI;
        }
    }
}