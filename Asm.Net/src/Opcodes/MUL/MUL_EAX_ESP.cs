using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EAX_ESP : Instruction, IMul
    {
        public MUL_EAX_ESP()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EAX_ESP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EAX, ESP";
        }

        public void Multiply(Registers registers)
        {
            registers.EAX *= registers.ESP;
        }
    }
}