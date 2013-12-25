using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EBX_EDI : Instruction, IMul
    {
        public MUL_EBX_EDI()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EBX_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EBX, EDI";
        }

        public void Multiply(Registers registers)
        {
            registers.EBX *= registers.EDI;
        }
    }
}