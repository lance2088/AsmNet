﻿using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_ESP_ESP : Instruction, IAnd
    {
        public AND_ESP_ESP()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_ESP_ESP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND ESP, ESP";
        }

        public void AndValue(Registers register)
        {
            register.ESP &= register.ESP;
        }
    }
}