using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine.DataTypes
{
    public class AsmString : IData
    {
        public AsmString(IData dataType, int VariableId, int MemAddress)
            : base(dataType.value, typeof(String), VariableId, MemAddress)
        {
        }
    }
}