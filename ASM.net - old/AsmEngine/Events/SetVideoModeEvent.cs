using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AsmEngine.Events
{
    public delegate void SetVideoModeEventHandler(SetVideoModeEventArgs e);

    public class SetVideoModeEventArgs : EventArgs
    {
        public readonly int colors;
        public readonly int height;
        public readonly int width;

        public SetVideoModeEventArgs(int colors, int width, int height)
        {
            this.colors = colors;
            this.height = height;
            this.width = width;
        }
    }

    public class SetVideoModeEvent
    {
        public static event SetVideoModeEventHandler SetVideoMode;

        public static void onSetVideoMode(SetVideoModeEventArgs e)
        {
            if (SetVideoMode != null)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(UpdateThread));
                thread.Start(e);
            }
        }

        public static bool VideoModeNULL()
        {
            return (SetVideoMode == null);
        }

        private static void UpdateThread(object e)
        {
            SetVideoMode((SetVideoModeEventArgs)e);
        }
    }
}