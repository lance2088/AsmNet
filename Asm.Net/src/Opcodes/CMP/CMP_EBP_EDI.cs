using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.CMP
{
    public class CMP_EBP_EDI : Instruction, ICmp
    {
        public CMP_EBP_EDI()
            : base(2, typeof(ICmp))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.CMP_REGISTER, (byte)CmpRegisterOpcodes.CMP_EBP_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "CMP EBP, EDI";
        }

        public void Compare(ref Flags flags, Registers registers)
        {
            flags.ZeroFlag = registers.EBP == registers.EDI;
        }
    }
}