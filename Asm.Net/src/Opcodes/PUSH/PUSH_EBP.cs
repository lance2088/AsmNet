using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Sections;

namespace Asm.Net.src.Opcodes.PUSH
{
    public class PUSH_EBP : Instruction, IPush
    {
        public int ValueAddress { get; set; }
        public object Value { get; set; }
        public PUSH_EBP()
            : base(1, typeof(IPush))
        {
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.PUSH_EBP };
        }

        public override void Dispose()
        {

        }

        public void AddToStack(Registers registers, List<object> Stack, DataSection dataSection)
        {
            Stack.Add(registers.EBP);
        }

        public override string ToString()
        {
            return "PUSH EBP";
        }
    }
}
