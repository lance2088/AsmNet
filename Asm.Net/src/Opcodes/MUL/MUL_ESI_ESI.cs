using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_ESI_ESI : Instruction, IMul
    {
        public MUL_ESI_ESI()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_ESI_ESI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL ESI, ESI";
        }

        public void Multiply(Registers registers)
        {
            registers.ESI *= registers.ESI;
        }
    }
}