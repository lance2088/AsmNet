using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using AsmEngine.DataTypes;

namespace AsmEngine.Instructions
{
    public class NOP : IData, AsmOpcode
    {
        public int MemAddress { get; set; }
        public NOP(IData dataType, int MemAddress)
            : base(dataType.value, typeof(String), 0, MemAddress)
        {
            this.MemAddress = MemAddress;
        }
    }
}