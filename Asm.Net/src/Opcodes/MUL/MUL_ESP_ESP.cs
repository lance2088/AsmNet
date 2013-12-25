using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_ESP_ESP : Instruction, IMul
    {
        public MUL_ESP_ESP()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_ESP_ESP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL ESP, ESP";
        }

        public void Multiply(Registers registers)
        {
            registers.ESP *= registers.ESP;
        }
    }
}