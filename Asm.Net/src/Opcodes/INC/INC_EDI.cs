using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.INC
{
    public class INC_EDI : Instruction, IInc
    {
        public INC_EDI()
            : base(1, typeof(IInc))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.INC_EDI };
        }

        public void Execute(Registers registers)
        {
            registers.EDI++;
        }

        public override string ToString()
        {
            return "INC EDI";
        }

        public override void Dispose()
        {
            
        }
    }
}