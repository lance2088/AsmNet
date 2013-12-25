using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src
{
    [Serializable]
    public class VirtualAddress
    {
        public int Size { get; internal set; }
        public int Address { get; internal set; }
        
        public VirtualAddress(int size, int address)
        {
            this.Size = size;
            this.Address = address;
        }
    }
}