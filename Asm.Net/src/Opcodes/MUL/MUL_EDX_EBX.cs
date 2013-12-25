using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EDX_EBX : Instruction, IMul
    {
        public MUL_EDX_EBX()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EDX_EBX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EDX, EBX";
        }

        public void Multiply(Registers registers)
        {
            registers.EDX *= registers.EBX;
        }
    }
}