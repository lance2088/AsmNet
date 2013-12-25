using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine.IO
{
    internal class CopyFile : Acall
    {
        public CopyFile()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 3)
                AssemblerExecute.registers.EAX = Kernel32.CopyFile(Convert.ToString(args[0].value), Convert.ToString(args[1].value), Convert.ToBoolean(args[2].value)) ? 1 : 0;
        }
    }
}