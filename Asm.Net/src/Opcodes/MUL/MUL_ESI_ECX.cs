using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_ESI_ECX : Instruction, IMul
    {
        public MUL_ESI_ECX()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_ESI_ECX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL ESI, ECX";
        }

        public void Multiply(Registers registers)
        {
            registers.ESI *= registers.ECX;
        }
    }
}