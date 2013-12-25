using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_EBX_EAX : Instruction, IMul
    {
        public MUL_EBX_EAX()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_EBX_EAX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL EBX, EAX";
        }

        public void Multiply(Registers registers)
        {
            registers.EBX *= registers.EAX;
        }
    }
}