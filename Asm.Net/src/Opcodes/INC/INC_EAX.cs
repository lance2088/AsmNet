using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Sections;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.INC
{
    public class INC_EAX : Instruction, IInc
    {
        public INC_EAX()
            : base(1, typeof(IInc))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.INC_EAX };
        }

        public void Execute(Registers registers)
        {
            registers.EAX++;
        }

        public override string ToString()
        {
            return "INC EAX";
        }

        public override void Dispose()
        {
            
        }
    }
}