using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.INC
{
    public class INC_ESI : Instruction, IInc
    {
        public INC_ESI()
            : base(1, typeof(IInc))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.INC_ESI };
        }

        public void Execute(Registers registers)
        {
            registers.ESI++;
        }

        public override string ToString()
        {
            return "INC ESI";
        }

        public override void Dispose()
        {
            
        }
    }
}