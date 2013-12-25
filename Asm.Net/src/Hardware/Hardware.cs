using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src.Hardware
{
    public abstract class Hardware
    {
        /// <summary> Used for hardware emulation, I'm curious what it will show soon in Cpu-z </summary>
        public Hardware()
        {
        }

        public abstract string HardwareName { get; }
    }
}