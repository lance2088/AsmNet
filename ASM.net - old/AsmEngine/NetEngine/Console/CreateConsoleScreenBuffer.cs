using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using AsmEngine.Wrappers;

namespace AsmEngine.NetEngine.Console
{
    internal class CreateConsoleScreenBuffer : Acall
    {
        public CreateConsoleScreenBuffer()
            : base()
        { }

        public override void Call()
        {
            AssemblerExecute.registers.EAX = Kernel32.CreateConsoleScreenBuffer(Convert.ToUInt32(args[0].value),
                                                                                Convert.ToUInt32(args[1].value),
                                                                                new IntPtr(Convert.ToInt32(args[2].value)),
                                                                                Convert.ToUInt32(args[3].value),
                                                                                new IntPtr(Convert.ToInt32(args[4].value))).ToInt32();


        }
    }
}