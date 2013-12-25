using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.INC
{
    public class INC_EBX : Instruction, IInc
    {
        public INC_EBX()
            : base(1, typeof(IInc))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.INC_EBX };
        }

        public void Execute(Registers registers)
        {
            registers.EBX++;
        }

        public override string ToString()
        {
            return "INC EBX";
        }

        public override void Dispose()
        {
            
        }
    }
}
