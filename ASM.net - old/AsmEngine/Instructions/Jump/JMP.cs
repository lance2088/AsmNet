using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;

namespace AsmEngine.Instructions.Jump
{
    public class JMP : AsmOpcode
    {
        public int JumpMemAddress { get; set; }
        public int MemAddress { get; set; }

        public JMP(int JumpMemAddress, int MemAddress)
            : base()
        {
            this.JumpMemAddress = JumpMemAddress;
            this.MemAddress = MemAddress;
        }
    }
}