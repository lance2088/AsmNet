using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using System.Threading;
using System.Diagnostics;
using AsmEngine.Events;

namespace AsmEngine.Instructions.Interrupts
{
    public unsafe class INT21 : IInterrupt, AsmOpcode
    {
        int AsmOpcode.MemAddress { get; set; }
        public int MemAddress { get; set; }

        public INT21(int MemAddress)
            : base()
        {
            this.MemAddress = MemAddress;
        }

        public void Execute()
        {
            switch (AssemblerExecute.registers.AH)
            {
                case 0:
                {
                    AssemblerExecute.flags.InterruptFlag = false;
                    AssemblerExecute.Halt = true;
                    HaltEvent.onHalt();
                    ProcessStatusUpateEvent.onProcessUpdate(new ProcessStatusUpdateEventArgs("Process Terminated"));
                    AssemblerExecute.flags.InterruptFlag = true;

                    if (CurrentOpcodeEvent.IsNULL())
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }
                break;
            }
        }
    }
}