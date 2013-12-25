using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_ECX_ESI : Instruction, IMul
    {
        public MUL_ECX_ESI()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_ECX_ESI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL ECX, ESI";
        }

        public void Multiply(Registers registers)
        {
            registers.ECX *= registers.ESI;
        }
    }
}