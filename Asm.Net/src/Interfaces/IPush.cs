using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Sections;

namespace Asm.Net.src.Interfaces
{
    public interface IPush
    {
        void AddToStack(Registers registers, List<object> Stack, DataSection dataSection);
        object Value { get; set; }
        int ValueAddress { get; set; }
    }
}