using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_EBX_EDI : Instruction, IAnd
    {
        public AND_EBX_EDI()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_EBX_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND EBX, EDI";
        }

        public void AndValue(Registers register)
        {
            register.EBX &= register.EDI;
        }
    }
}