using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class CloseHandle : Acall
    {
        public CloseHandle()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 1)
                AssemblerExecute.registers.EAX = Kernel32.CloseHandle(new IntPtr(Convert.ToInt32(args[0].value))) ? 1 : 0;
        }
    }
}