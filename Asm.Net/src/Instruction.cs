using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src
{
    [Serializable]
    public abstract class Instruction
    {
        public VirtualAddress VirtualAddress;
        public string ExtraInformation = "";

        /// <summary> Only used for x86 programs which were loaded from the hdd </summary>
        public VirtualAddress NativeVirtualAddress;
        public Type InterfaceType { get; private set; }

        public Instruction(int Size, Type InterfaceType)
        {
            this.VirtualAddress = new VirtualAddress(Size, 0);
            this.NativeVirtualAddress = new VirtualAddress(0, 0);
            this.InterfaceType = InterfaceType;
        }

        public abstract byte[] ToByteArray();
        public abstract void Dispose();
        public abstract string ToString();
    }
}