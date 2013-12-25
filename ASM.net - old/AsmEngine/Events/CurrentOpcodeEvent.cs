using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using System.Threading;

namespace AsmEngine.Events
{
    public delegate void CurrentOpcodeEventHandler(CurrentOpcodeEventArgs e);

    public class CurrentOpcodeEventArgs : EventArgs
    {
        public readonly AsmOpcode opcode;
        public readonly int CurrentLocation;

        public CurrentOpcodeEventArgs(AsmOpcode opcode, int CurrentLocation)
        {
            this.opcode = opcode;
            this.CurrentLocation = CurrentLocation;
        }
    }

    public class CurrentOpcodeEvent
    {
        public static event CurrentOpcodeEventHandler CurrentOpcode;

        public static void onParsedOpcodeEvent(CurrentOpcodeEventArgs e)
        {
            if (CurrentOpcode != null)
                ThreadPool.QueueUserWorkItem(UpdateThread, e);
        }

        private static void UpdateThread(object e)
        {
            if (CurrentOpcode != null)
                CurrentOpcode((CurrentOpcodeEventArgs)e);
        }

        public static bool IsNULL()
        {
            return (CurrentOpcode == null);
        }
    }
}