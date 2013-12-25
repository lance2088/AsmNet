using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.DataTypes;
using AsmEngine.Collection;

namespace AsmEngine.Events
{
    public delegate void StackUpdateEventHandler(StackUpdateEventArgs e);

    public class StackUpdateEventArgs : EventArgs
    {
        public readonly StackCollection stack;

        public StackUpdateEventArgs(StackCollection stack)
        {
            this.stack = stack;
        }
    }

    public class StackUpdateEvent
    {
        public static event StackUpdateEventHandler StackUpdate;

        public static void onStackUpdate(StackUpdateEventArgs e)
        {
            if (StackUpdate != null)
                StackUpdate(e);
        }
    }
}