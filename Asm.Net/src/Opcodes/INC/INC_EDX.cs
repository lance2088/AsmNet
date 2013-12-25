using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.INC
{
    public class INC_EDX : Instruction, IInc
    {
        public INC_EDX()
            : base(1, typeof(IInc))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.INC_EDX };
        }

        public void Execute(Registers registers)
        {
            registers.EDX++;
        }

        public override string ToString()
        {
            return "INC EDX";
        }

        public override void Dispose()
        {
            
        }
    }
}