using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_EAX_EBX : Instruction, IXor
    {
        public XOR_EAX_EBX()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_EAX_EBX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR EAX, EBX";
        }

        public void XorValue(Registers register)
        {
            register.EAX ^= register.EBX;
        }
    }
}