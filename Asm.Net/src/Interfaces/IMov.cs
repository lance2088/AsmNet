using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Sections;

namespace Asm.Net.src.Interfaces
{
    public interface IMov
    {
        VirtualAddress ModifiyValue { get; set; }
        void Execute(Registers registers, DataSection dataSection);
    }
}