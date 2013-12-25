using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine.Events
{
    public delegate void VirtualWriteEventHandler(VirtualWriteEventArgs e);

    public class VirtualWriteEventArgs : EventArgs
    {
        public readonly string message;

        public VirtualWriteEventArgs(string message)
        {
            this.message = message;
        }
    }

    public class VirtualWriteEvent
    {
        public static event VirtualWriteEventHandler VirtualWrite;

        public static void onVirtualWrite(VirtualWriteEventArgs e)
        {
            if (VirtualWrite != null)
                VirtualWrite(e);
        }
    }
}