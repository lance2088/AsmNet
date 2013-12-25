using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EAX_ESI : Instruction, IMul
    {
        public MUL_EAX_ESI()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EAX_ESI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EAX, ESI";
        }

        public void Multiply(Registers registers)
        {
            registers.EAX *= registers.ESI;
        }
    }
}