using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_ESI_ESP : Instruction, IXor
    {
        public XOR_ESI_ESP()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_ESI_ESP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR ESI, ESP";
        }

        public void XorValue(Registers register)
        {
            register.ESI ^= register.ESP;
        }
    }
}