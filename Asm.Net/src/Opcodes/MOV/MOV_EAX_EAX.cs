﻿using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_EAX_EAX : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_EAX_EAX()
            : base(2, typeof(IMov))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_REGISTER, (byte)MovRegisterOpcodes.MOV_EAX_EAX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV EAX, EAX";
        }

        public void Execute(Registers registers, Sections.DataSection dataSection)
        {
            registers.EAX = registers.EAX;
        }
    }
}