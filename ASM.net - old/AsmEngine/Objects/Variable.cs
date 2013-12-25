using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.DataTypes;

namespace AsmEngine.Objects
{
    public class Variable : IData
    {
        public Variable(byte Id, object value, Type type, int MemAddress)
            : base(value, type, Id, MemAddress)
        {
        }
        public Variable(object value, Type type, int MemAddress)
            : base(value, type, 0, MemAddress)
        {
        }
    }
}