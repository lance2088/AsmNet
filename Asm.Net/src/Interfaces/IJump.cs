using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src.Interfaces
{
    public interface IJump
    {
        int NextIpAddress(Flags flags, Registers registers);
        string Label { get; set; }
        int JumpAddress { get; set; }
    }
}