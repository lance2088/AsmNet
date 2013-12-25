using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using System.Threading;

namespace AsmEngine.Events
{
    public delegate void RegisterUpdateEventHandler();

    public class RegisterUpdateEvent
    {
        public static event RegisterUpdateEventHandler RegisterUpdate;

        public static void onRegisterUpdate()
        {
            if (RegisterUpdate != null)
                ThreadPool.QueueUserWorkItem(UpdateThread);
        }

        private static void UpdateThread(object e)
        {
            try
            {
                RegisterUpdate();
            }
            catch { }
        }
    }
}