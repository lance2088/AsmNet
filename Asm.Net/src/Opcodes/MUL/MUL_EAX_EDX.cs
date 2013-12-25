using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EAX_EDX : Instruction, IMul
    {
        public MUL_EAX_EDX()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EAX_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EAX, EDX";
        }

        public void Multiply(Registers registers)
        {
            registers.EAX *= registers.EDX;
        }
    }
}