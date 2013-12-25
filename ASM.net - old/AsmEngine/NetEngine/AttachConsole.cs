using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class AttachConsole : Acall
    {
        public AttachConsole()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 1)
                AssemblerExecute.registers.EAX = Kernel32.AttachConsole(Convert.ToUInt32(args[0].value)) ? 1 : 0;
        }
    }
}