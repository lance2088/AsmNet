using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EDI_EAX : Instruction, IMul
    {
        public MUL_EDI_EAX()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EDI_EAX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EDI, EAX";
        }

        public void Multiply(Registers registers)
        {
            registers.EDI *= registers.EAX;
        }
    }
}