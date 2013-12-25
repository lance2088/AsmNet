using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_EBP_EDI : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_EBP_EDI()
            : base(2, typeof(IMov))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_REGISTER, (byte)MovRegisterOpcodes.MOV_EBP_EDI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV ESP, EDI";
        }

        public void Execute(Registers registers, Sections.DataSection dataSection)
        {
            registers.EBP = registers.EDI;
        }
    }
}