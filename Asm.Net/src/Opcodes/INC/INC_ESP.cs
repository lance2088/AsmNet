using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.INC
{
    public class INC_ESP : Instruction, IInc
    {
        public INC_ESP()
            : base(1, typeof(IInc))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.INC_ESP };
        }

        public void Execute(Registers registers)
        {
            registers.ESP++;
        }

        public override string ToString()
        {
            return "INC ESP";
        }

        public override void Dispose()
        {
            
        }
    }
}