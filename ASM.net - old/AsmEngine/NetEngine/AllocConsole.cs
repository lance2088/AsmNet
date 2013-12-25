using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class AllocConsole : Acall
    {
        public AllocConsole()
            : base()
        { }

        public override void Call()
        {
            AssemblerExecute.registers.EAX = Kernel32.AllocConsole() ? 1 : 0;
        }
    }
}