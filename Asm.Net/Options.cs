using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net
{
    public class Options
    {
        public static int MemoryBaseAddress { get; private set; }
        public static int DataSectionBaseAddress { get; private set; }

        static Options()
        {
            MemoryBaseAddress = 0x401000; //should be correct, re checked it with OllyIce, OllyDbg
            DataSectionBaseAddress = 0x000100; //just a quick guess...
        }
    }
}
