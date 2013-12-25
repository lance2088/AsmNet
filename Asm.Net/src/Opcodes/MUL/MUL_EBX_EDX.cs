using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EBX_EDX : Instruction, IMul
    {
        public MUL_EBX_EDX()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EBX_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EBX, EDX";
        }

        public void Multiply(Registers registers)
        {
            registers.EBX *= registers.EDX;
        }
    }
}