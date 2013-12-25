using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;

namespace AsmEngine.Instructions.Interrupts
{
    public interface IInterrupt
    {
        void Execute();
    }
}