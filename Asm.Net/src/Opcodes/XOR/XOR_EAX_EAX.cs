using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class XOR_EAX_EAX : Instruction, IXor
    {
        public XOR_EAX_EAX()
            : base(2, typeof(IXor))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.XOR_REGISTER, (byte)XorRegisterOpcodes.XOR_EAX_EAX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "XOR EAX, EAX";
        }

        public void XorValue(Registers register)
        {
            register.EAX ^= register.EAX;
        }
    }
}