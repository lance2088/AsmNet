using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;

namespace AsmEngine.Instructions
{
    public class INC : AsmOpcode
    {
        public VariableType type { get; private set; }
        public Register register { get; private set; }
        public byte VariableId { get; private set; }
        public int MemAddress { get; set; }

        public INC(byte VariableId, VariableType type, int MemAddress)
            : base()
        {
            this.VariableId = VariableId;
            this.type = type;
            this.MemAddress = MemAddress;
        }

        public INC(Register register, VariableType type, int MemAddress)
            : base()
        {
            this.register = register;
            this.type = type;
            this.MemAddress = MemAddress;
        }

    }
}