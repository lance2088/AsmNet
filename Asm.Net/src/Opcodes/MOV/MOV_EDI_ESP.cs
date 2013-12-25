using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_EDI_ESP : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_EDI_ESP()
            : base(2, typeof(IMov))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_REGISTER, (byte)MovRegisterOpcodes.MOV_EDI_ESP };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV EDI, ESP";
        }

        public void Execute(Registers registers, Sections.DataSection dataSection)
        {
            registers.EDI = registers.ESP;
        }
    }
}