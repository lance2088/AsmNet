using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;
using System.Runtime.InteropServices;

namespace AsmEngine.NetEngine
{
    internal class GetLastError : Acall
    {
        public GetLastError()
            : base()
        { }

        public override void Call()
        {
            AssemblerExecute.registers.EAX = Marshal.GetLastWin32Error();
        }
    }
}