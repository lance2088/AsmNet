using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using AsmEngine.DataTypes;

namespace AsmEngine.Instructions
{
    public class PUSH : IData, AsmOpcode
    {
        public Register register = Register.none;
        public int MemAddress { get; set; }

        public PUSH(IData dataType, int VariableId, int MemAddress)
            : base(dataType.value, dataType.type, VariableId, MemAddress)
        {
            this.MemAddress = MemAddress;
        }
        public PUSH(IData dataType, Register register, int MemAddress)
            : base(dataType.value, dataType.type, 0, MemAddress)
        {
            this.register = register;
            this.MemAddress = MemAddress;
        }
    }
}