using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.DataTypes;
using AsmEngine.Objects;

namespace AsmEngine.Instructions
{
    public class MOV : IData, AsmOpcode
    {
        public int MemAddress { get; set; }
        public Register register;
        public Register RegisterValue;

        public MOV(IData dataType, int VariableId, int MemAddress)
            : base(dataType.value, typeof(String), VariableId, MemAddress)
        {
            this.MemAddress = MemAddress;
            this.RegisterValue = Register.none;
        }
        public MOV(IData dataType, Register register, int MemAddress)
            : base(dataType.value, typeof(String), 0, MemAddress)
        {
            this.register = register;
            this.MemAddress = MemAddress;
            this.RegisterValue = Register.none;
        }
        public MOV(IData dataType, Register register, int MemAddress, Register RegisterValue)
            : base(dataType.value, typeof(String), 0, MemAddress)
        {
            this.register = register;
            this.MemAddress = MemAddress;
            this.RegisterValue = RegisterValue;
        }
    }
}