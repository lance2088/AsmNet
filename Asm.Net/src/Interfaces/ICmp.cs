using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src.Interfaces
{
    public interface ICmp
    {
        void Compare(ref Flags flags, Registers registers);
    }
}