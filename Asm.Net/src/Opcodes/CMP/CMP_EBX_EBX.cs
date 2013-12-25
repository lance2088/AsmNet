using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.CMP
{
    public class CMP_EBX_EBX : Instruction, ICmp
    {
        public CMP_EBX_EBX()
            : base(2, typeof(ICmp))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.CMP_REGISTER, (byte)CmpRegisterOpcodes.CMP_EBX_EBX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "CMP EBX, EBX";
        }

        public void Compare(ref Flags flags, Registers registers)
        {
            flags.ZeroFlag = true;
        }
    }
}