using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_ECX_EBX : Instruction, IAnd
    {
        public AND_ECX_EBX()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_ECX_EBX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND ECX, EBX";
        }

        public void AndValue(Registers register)
        {
            register.ECX &= register.EBX;
        }
    }
}