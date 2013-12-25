using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Sections;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_ESP_EDX : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_ESP_EDX()
            : base(2, typeof(IMov))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_REGISTER, (byte)MovRegisterOpcodes.MOV_ESP_EDX };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV ESP, EDX";
        }

        public void Execute(Registers registers, DataSection dataSection)
        {
            registers.ESP = registers.EDX;
        }
    }
}