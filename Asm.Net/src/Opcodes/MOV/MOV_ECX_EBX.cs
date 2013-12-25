using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_ECX_EBX : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_ECX_EBX()
            : base(2, typeof(IMov))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_REGISTER, (byte)MovRegisterOpcodes.MOV_ECX_EBX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV ECX, EBX";
        }

        public void Execute(Registers registers, Sections.DataSection dataSection)
        {
            registers.ECX = registers.EBX;
        }
    }
}