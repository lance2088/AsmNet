using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine.Events
{
    public delegate void ProcessStatusUpdateEventHandler(ProcessStatusUpdateEventArgs e);

    public class ProcessStatusUpdateEventArgs : EventArgs
    {
        public readonly string Status;

        public ProcessStatusUpdateEventArgs(string Status)
        {
            this.Status = Status;
        }
    }

    public class ProcessStatusUpateEvent
    {
        public static event ProcessStatusUpdateEventHandler ProcessUpdate;

        public static void onProcessUpdate(ProcessStatusUpdateEventArgs e)
        {
            if (ProcessUpdate != null)
                ProcessUpdate(e);
        }
    }
}