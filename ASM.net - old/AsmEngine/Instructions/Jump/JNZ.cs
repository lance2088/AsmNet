﻿using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;

namespace AsmEngine.Instructions.Jump
{
    public class JNZ : AsmOpcode
    {
        public int JumpMemAddress { get; set; }
        public int MemAddress { get; set; }

        public JNZ(int JumpMemAddress, int MemAddress)
            : base()
        {
            this.JumpMemAddress = JumpMemAddress;
            this.MemAddress = MemAddress;
        }
    }
}