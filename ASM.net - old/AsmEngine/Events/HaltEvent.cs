using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine.Events
{
    public delegate void HaltEventHandler();
    public class HaltEvent
    {
        public static event HaltEventHandler Halt;

        public static void onHalt()
        {
            if (Halt != null)
                Halt();
        }
    }
}