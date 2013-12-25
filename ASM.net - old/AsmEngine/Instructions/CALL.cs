using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.DataTypes;
using AsmEngine.Objects;
using AsmEngine.Instructions;

namespace AsmEngine.Instructions
{
    public abstract class Acall : AsmOpcode
    {
        public int MemAddress { get; set; }
        public PUSH[] args;
        public abstract void Call();
        
        public Acall()
        {
        }
    }
}