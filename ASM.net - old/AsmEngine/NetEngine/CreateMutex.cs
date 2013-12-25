using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class CreateMutex : Acall
    {
        public CreateMutex()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 3)
            {
                AssemblerExecute.registers.EAX = Kernel32.CreateMutex(new IntPtr(Convert.ToInt32(args[0].value)),
                                                                      Convert.ToBoolean(args[1].value),
                                                                      Convert.ToString(args[2].value)).ToInt32();
            }
        }
    }
}