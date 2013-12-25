using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_ECX_EDI : Instruction, IAnd
    {
        public AND_ECX_EDI()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_ECX_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND ECX, EDI";
        }

        public void AndValue(Registers register)
        {
            register.ECX &= register.EDI;
        }
    }
}