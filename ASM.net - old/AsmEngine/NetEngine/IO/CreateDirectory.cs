using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine.IO
{
    internal class CreateDirectory : Acall
    {
        public CreateDirectory()
            : base()
        { }

        public override void Call()
        {
            AssemblerExecute.registers.EAX = Kernel32.CreateDirectory(Convert.ToString(args[0].value),
                                                                      new IntPtr(Convert.ToInt32(args[1].value))) ? 1 : 0;
        }
    }
}