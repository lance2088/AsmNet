using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class OpenMutex : Acall
    {
        public OpenMutex()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 3)
            {
                AssemblerExecute.registers.EAX = Kernel32.OpenMutex(Convert.ToUInt32(args[0].value),
                                                                    Convert.ToBoolean(args[1].value),
                                                                    Convert.ToString(args[2].value)).ToInt32();
            }
        }
    }
}