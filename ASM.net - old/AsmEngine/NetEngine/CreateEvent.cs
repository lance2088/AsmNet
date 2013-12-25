using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class CreateEvent : Acall
    {
        public CreateEvent()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 4)
                AssemblerExecute.registers.EAX = Kernel32.CreateEvent(new IntPtr(Convert.ToInt32(args[0].value)),
                                                                      Convert.ToBoolean(args[1].value),
                                                                      Convert.ToBoolean(args[2].value),
                                                                      Convert.ToString(args[3].value)).ToInt32();
        }
    }
}