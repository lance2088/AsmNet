using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_EBX_EDX : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_EBX_EDX()
            : base(2, typeof(IMov))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_REGISTER, (byte)MovRegisterOpcodes.MOV_EBX_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV EBX, EDX";
        }

        public void Execute(Registers registers, Sections.DataSection dataSection)
        {
            registers.EBX = registers.EDX;
        }
    }
}