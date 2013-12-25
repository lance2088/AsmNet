using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine
{
    internal class WaitForSingleObject : Acall
    {
        public WaitForSingleObject()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 2)
            {
                AssemblerExecute.registers.EAX = Convert.ToInt32(Kernel32.WaitForSingleObject(new IntPtr(Convert.ToInt32(args[0].value)),
                                                                                              Convert.ToUInt32(args[1].value)));
            }
        }
    }
}