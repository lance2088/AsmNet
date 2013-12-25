using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.CMP
{
    public class CMP_ECX_ESI : Instruction, ICmp
    {
        public CMP_ECX_ESI()
            : base(2, typeof(ICmp))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.CMP_REGISTER, (byte)CmpRegisterOpcodes.CMP_ECX_ESI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "CMP ECX, ESI";
        }

        public void Compare(ref Flags flags, Registers registers)
        {
            flags.ZeroFlag = registers.ECX == registers.ESI;
        }
    }
}