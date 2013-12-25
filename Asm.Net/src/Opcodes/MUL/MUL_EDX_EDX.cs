using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EDX_EDX : Instruction, IMul
    {
        public MUL_EDX_EDX()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EDX_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EDX, EDX";
        }

        public void Multiply(Registers registers)
        {
            registers.EDX *= registers.EDX;
        }
    }
}