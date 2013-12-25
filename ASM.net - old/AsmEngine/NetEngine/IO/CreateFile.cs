using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.DataTypes;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine.IO
{
    internal class CreateFile : Acall
    {
        public CreateFile()
            : base()
        { }

        public override void Call()
        {
            if (args.Length == 7)
            {
                AssemblerExecute.registers.EAX = Kernel32.CreateFile(Convert.ToString(args[0].value),
                                                                     Convert.ToUInt32(args[1].value),
                                                                     Convert.ToUInt32(args[2].value),
                                                                     new IntPtr(Convert.ToInt32(args[3].value)),
                                                                     Convert.ToUInt32(args[4].value),
                                                                     Convert.ToUInt32(args[5].value),
                                                                     new IntPtr(Convert.ToInt32(args[6].value))).DangerousGetHandle().ToInt32();
            }
        }
    }
}