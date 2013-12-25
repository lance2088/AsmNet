using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_EDI_ECX : Instruction, IXor
    {
        public XOR_EDI_ECX()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_EDI_ECX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR EDI, ECX";
        }

        public void XorValue(Registers register)
        {
            register.EDI ^= register.ECX;
        }
    }
}