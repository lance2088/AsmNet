using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine.Events
{
    public delegate void FlagUpdateEventHandler();
    public class FlagUpdateEvent
    {
        public static event FlagUpdateEventHandler FlagUpdate;

        public static void onFlagUpdate()
        {
            if (FlagUpdate != null)
                FlagUpdate();
        }
    }
}