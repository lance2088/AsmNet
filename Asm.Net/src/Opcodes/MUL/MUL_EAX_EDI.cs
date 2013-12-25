using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EAX_EDI : Instruction, IMul
    {
        public MUL_EAX_EDI()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EAX_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EAX, EDI";
        }

        public void Multiply(Registers registers)
        {
            registers.EAX *= registers.EDI;
        }
    }
}