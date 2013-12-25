using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine.IO
{
    internal class CreateDirectoryEx : Acall
    {
        public CreateDirectoryEx()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 3)
                AssemblerExecute.registers.EAX = Kernel32.CreateDirectoryEx(Convert.ToString(args[0].value),
                                                                            Convert.ToString(args[1].value),
                                                                            new IntPtr(Convert.ToInt32(args[2].value))) ? 1 : 0;
        }
    }
}