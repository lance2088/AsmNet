using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine.Objects
{
    public enum Accessor : byte
    {
        Private = 1,
        Public = 2,
        Protected = 4,
        Internal = 8,
        Static = 16
    }
}