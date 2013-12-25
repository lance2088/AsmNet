using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_EAX_EDX : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_EAX_EDX()
            : base(2, typeof(IMov))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_REGISTER, (byte)MovRegisterOpcodes.MOV_EAX_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV EAX, EDX";
        }

        public void Execute(Registers registers, Sections.DataSection dataSection)
        {
            registers.EAX = registers.EDX;
        }
    }
}