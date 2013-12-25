using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class ActivateActCtx : Acall
    {
        public ActivateActCtx()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 2)
            {
                IntPtr outty = new IntPtr(Convert.ToInt32(args[1].value));
                AssemblerExecute.registers.EAX = Kernel32.ActivateActCtx(new IntPtr(Convert.ToInt32(args[0].value)), out outty) ? 1 : 0;
            }
        }
    }
}
