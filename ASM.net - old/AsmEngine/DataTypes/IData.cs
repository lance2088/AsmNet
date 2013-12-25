using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;

namespace AsmEngine.DataTypes
{
    public class IData : AsmOpcode
    {
        public Type type;
        public Object value { get; set; }
        public int Id { get; private set; }
        public int MemAddress { get; set; }

        public IData(Object value, Type type, int Id, int MemAddress)
        {
            this.value = value;
            this.type = type;
            this.Id = Id;
            this.MemAddress = MemAddress;
        }

        public IData() { }
    }
}