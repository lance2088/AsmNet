﻿using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.XOR
{
    public class AND_EAX_EBP : Instruction, IAnd
    {
        public AND_EAX_EBP()
            : base(2, typeof(IAnd))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.AND_REGISTER, (byte)AndRegisterOpcodes.AND_EAX_EBP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "AND EAX, EBP";
        }

        public void AndValue(Registers register)
        {
            register.EAX &= register.EBP;
        }
    }
}