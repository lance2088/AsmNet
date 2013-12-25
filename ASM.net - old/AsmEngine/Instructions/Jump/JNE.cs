using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;

namespace AsmEngine.Instructions.Jump
{
    public class JNE : AsmOpcode
    {
        public int JumpMemAddress { get; set; }
        public int MemAddress { get; set; }

        public JNE(int JumpMemAddress, int MemAddress)
            : base()
        {
            this.JumpMemAddress = JumpMemAddress;
            this.MemAddress = MemAddress;
        }
    }
}