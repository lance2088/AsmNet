﻿using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_ESP_EDI : Instruction, IMul
    {
        public MUL_ESP_EDI()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_ESP_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL ESP, EDI";
        }

        public void Multiply(Registers registers)
        {
            registers.ESP *= registers.EDI;
        }
    }
}