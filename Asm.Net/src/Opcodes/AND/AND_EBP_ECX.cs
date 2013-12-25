using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_EBP_ECX : Instruction, IAnd
    {
        public AND_EBP_ECX()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_EBP_ECX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND EBP, ECX";
        }

        public void AndValue(Registers register)
        {
            register.EBP &= register.ECX;
        }
    }
}