using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src
{
    public class BreakPoint
    {
        //The fun about a interpreter is that we don't need to modify anything in the memory
        //or even have to debug the process to set/trigger a breakpoint

        public VirtualAddress virtualAddress { get; private set; }
        public long Calls { get; private set; }

        public BreakPoint(VirtualAddress address)
        {
            this.virtualAddress = virtualAddress;
        }
    }
}