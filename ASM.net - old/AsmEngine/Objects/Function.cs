using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.DataTypes;
using AsmEngine.NetEngine;
using AsmEngine.Instructions;

namespace AsmEngine.Objects
{
    internal class Function : Acall
    {
        public List<PUSH> asm;

        public Function(byte[] FunctionBytes)
            : base() //need to read the arguments
        {
            asm = new List<PUSH>();
        }

        public override void Call()
        {
            
        }
    }
}