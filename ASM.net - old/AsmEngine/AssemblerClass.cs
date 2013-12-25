using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using AsmEngine.NetEngine;
using AsmEngine.Instructions;

namespace AsmEngine
{
    public class AssemblerClass
    {
        public byte name { get; private set; }
        public Accessor accessor { get; private set; }
        public List<Byte> ExecuteBytes;
        public List<AsmOpcode> opcodes;
        public SortedList<byte, Variable> Variables;

        public AssemblerClass(byte name, Accessor access)
        {
            this.name = name;
            this.accessor = access;
            ExecuteBytes = new List<byte>();
            opcodes = new List<AsmOpcode>();
            Variables = new SortedList<byte, Variable>();
        }
    }
}