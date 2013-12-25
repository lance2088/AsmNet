using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EBX_EBX : Instruction, IMul
    {
        public MUL_EBX_EBX()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EBX_EBX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EBX, EBX";
        }

        public void Multiply(Registers registers)
        {
            registers.EBX *= registers.EBX;
        }
    }
}