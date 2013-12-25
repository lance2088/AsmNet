using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.INC
{
    public class INC_ECX : Instruction, IInc
    {
        public INC_ECX()
            : base(1, typeof(IInc))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.INC_ECX };
        }

        public void Execute(Registers registers)
        {
            registers.ECX++;
        }

        public override string ToString()
        {
            return "INC ECX";
        }

        public override void Dispose()
        {
            
        }
    }
}
