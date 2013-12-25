using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_ESP_EDX : Instruction, IAnd
    {
        public AND_ESP_EDX()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_ESP_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND ESP, EDX";
        }

        public void AndValue(Registers register)
        {
            register.ESP &= register.EDX;
        }
    }
}