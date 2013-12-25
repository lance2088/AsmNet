using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_EDX_ESI : Instruction, IAnd
    {
        public AND_EDX_ESI()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_EDX_ESI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND EDX, ESI";
        }

        public void AndValue(Registers register)
        {
            register.EDX &= register.ESI;
        }
    }
}