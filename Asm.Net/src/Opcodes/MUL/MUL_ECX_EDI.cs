﻿using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MUL
{
    public class MUL_ECX_EDI : Instruction, IMul
    {
        public MUL_ECX_EDI()
            : base(2, typeof(IMul))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MUL, (byte)MulRegisterOpcodes.MUL_ECX_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MUL ECX, EDI";
        }

        public void Multiply(Registers registers)
        {
            registers.ECX *= registers.EDI;
        }
    }
}