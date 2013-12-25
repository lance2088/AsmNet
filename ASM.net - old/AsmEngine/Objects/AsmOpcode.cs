using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.DataTypes;

namespace AsmEngine.Objects
{
    public interface AsmOpcode
    {
        int MemAddress { get; set; }
    }
}