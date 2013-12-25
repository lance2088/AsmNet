using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class IsDebuggerPresent : Acall
    {
        public IsDebuggerPresent()
            : base()
        { }

        public override void Call()
        {
            AssemblerExecute.registers.EAX = Kernel32.IsDebuggerPresent() ? 1 : 0;
        }
    }
}
