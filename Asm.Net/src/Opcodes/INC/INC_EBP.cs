using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.INC
{
    public class INC_EBP : Instruction, IInc
    {
        public INC_EBP()
            : base(1, typeof(IInc))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.INC_EBP };
        }

        public void Execute(Registers registers)
        {
            registers.EBP++;
        }

        public override string ToString()
        {
            return "INC EBP";
        }

        public override void Dispose()
        {
            
        }
    }
}
