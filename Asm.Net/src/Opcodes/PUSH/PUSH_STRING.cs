using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Sections;

namespace Asm.Net.src.Opcodes.PUSH
{
    public class PUSH_STRING : Instruction, IPush
    {
        public int ValueAddress { get; set; }
        public object Value { get; set; }

        internal PUSH_STRING()
            : base(5) //1=opcode, 4=address
        {
        }

        public PUSH_STRING(string value, DataSection dataSection)
            : base(5) //1=opcode, 4=address
        {
            this.Value = value;
            this.ValueAddress = dataSection.AddString(value).Address;
        }

        public PUSH_STRING(int StringAddress, DataSection dataSection)
            : base(5) //1=opcode, 4=address
        {
            this.Value = dataSection.LoadString(new VirtualAddress(4, StringAddress));
            this.ValueAddress = StringAddress;
        }

        public override byte[] ToByteArray()
        {
            return new byte[] {
                                (byte)OpcodeList.PUSH_STRING,
                                BitConverter.GetBytes(ValueAddress)[0],
                                BitConverter.GetBytes(ValueAddress)[1],
                                BitConverter.GetBytes(ValueAddress)[2],
                                BitConverter.GetBytes(ValueAddress)[3]
                              };
        }

        public override void Dispose()
        {

        }

        public void AddToStack(Registers registers, List<object> Stack, DataSection dataSection)
        {
            Stack.Add(Value);
        }

        public override string ToString()
        {
            return "PUSH 0x" + ValueAddress.ToString("X6") + " ASCII \"" + ((Value != null) ? Value.ToString() : "") + "\"";
        }
    }
}