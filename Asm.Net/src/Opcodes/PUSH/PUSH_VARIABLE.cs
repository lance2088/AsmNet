using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Sections;

namespace Asm.Net.src.Opcodes.PUSH
{
    public class PUSH_VARIABLE : Instruction, IPush
    {
        public object Value { get; set; }
        public int ValueAddress { get; set; }
        private DataSection dataSection;
        
        public PUSH_VARIABLE(VirtualAddress VariableAddress, DataSection dataSection)
            : base(5, typeof(IPush))
        {
            /*string val = "";
            if (dataSection.LoadString(VariableAddress, ref val))
            {
                this.Value = val;
            }*/
            this.ValueAddress = VariableAddress.Address;
            this.dataSection = dataSection;
        }

        public override byte[] ToByteArray()
        {
            return new byte[] {
                                (byte)OpcodeList.PUSH_VARIABLE,
                                BitConverter.GetBytes(ValueAddress)[0],
                                BitConverter.GetBytes(ValueAddress)[1],
                                BitConverter.GetBytes(ValueAddress)[2],
                                BitConverter.GetBytes(ValueAddress)[3]
                              };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            if (dataSection.Variables[ValueAddress].Value != null)
            {
                if(dataSection.Variables[ValueAddress].Value.GetType() == typeof(string))
                    return "PUSH 0x" + ValueAddress.ToString("X6") + " ASCII \"" + dataSection.Variables[ValueAddress].Value + "\"";
                return "PUSH 0x" + ValueAddress.ToString("X6") + " - " + dataSection.Variables[ValueAddress].Value.GetType().Name + "::" + dataSection.Variables[ValueAddress].Value.ToString();
            }
            else
            {
                return "PUSH 0x" + ValueAddress.ToString("X6");
            }
        }

        public void AddToStack(Registers registers, List<object> Stack, DataSection dataSection)
        {
            Stack.Add(dataSection.Variables[ValueAddress].Value);
        }
    }
}