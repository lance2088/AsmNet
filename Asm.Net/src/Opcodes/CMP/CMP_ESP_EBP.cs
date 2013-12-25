using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.CMP
{
    public class CMP_ESP_EBP : Instruction, ICmp
    {
        public CMP_ESP_EBP()
            : base(2, typeof(ICmp))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.CMP_REGISTER, (byte)CmpRegisterOpcodes.CMP_ESP_EBP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "CMP ESP, EBP";
        }

        public void Compare(ref Flags flags, Registers registers)
        {
            flags.ZeroFlag = registers.ESP == registers.EBP;
        }
    }
}