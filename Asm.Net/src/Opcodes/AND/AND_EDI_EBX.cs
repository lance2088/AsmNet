using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_EDI_EBX : Instruction, IAnd
    {
        public AND_EDI_EBX()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_EDI_EBX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND EDI, EBX";
        }

        public void AndValue(Registers register)
        {
            register.EDI &= register.EBX;
        }
    }
}