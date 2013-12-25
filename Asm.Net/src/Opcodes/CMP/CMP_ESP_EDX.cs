using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.CMP
{
    public class CMP_ESP_EDX : Instruction, ICmp
    {
        public CMP_ESP_EDX()
            : base(2, typeof(ICmp))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.CMP_REGISTER, (byte)CmpRegisterOpcodes.CMP_ESP_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "CMP ESP, EDX";
        }

        public void Compare(ref Flags flags, Registers registers)
        {
            flags.ZeroFlag = registers.ESP == registers.EDX;
        }
    }
}