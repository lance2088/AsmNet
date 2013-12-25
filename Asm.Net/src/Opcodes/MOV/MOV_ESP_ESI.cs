using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Sections;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_ESP_ESI : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_ESP_ESI()
            : base(2, typeof(IMov))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_REGISTER, (byte)MovRegisterOpcodes.MOV_ESP_ESI };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV ESP, ESI";
        }

        public void Execute(Registers registers, DataSection dataSection)
        {
            registers.ESP = registers.ESI;
        }
    }
}