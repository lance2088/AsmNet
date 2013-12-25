using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Wrappers;
using AsmEngine.Instructions;

namespace AsmEngine.NetEngine
{
    internal class Beep : Acall
    {
        public Beep()
            : base()
        { }

        public override void Call()
        {
            if(args.Length == 2)
                AssemblerExecute.registers.EAX = Kernel32.Beep(Convert.ToUInt32(args[0].value), Convert.ToUInt32(args[1].value)) ? 1 : 0;
        }
    }
}