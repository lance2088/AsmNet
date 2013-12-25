using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AsmEngine.Events
{
    public delegate void SetListviewBackColorEventHandler(SetListviewBackColorEventArgs e);

    public class SetListviewBackColorEventArgs : EventArgs
    {
        public readonly Color BackColor;
        public readonly int index;

        public SetListviewBackColorEventArgs(Color BackColor, int index)
        {
            this.BackColor = BackColor;
            this.index = index;
        }
    }

    public class SetListviewBackColorEvent
    {
        public static event SetListviewBackColorEventHandler SetListviewBackColor;

        public static void onSetListviewBackColor(SetListviewBackColorEventArgs e)
        {
            if (SetListviewBackColor != null)
                SetListviewBackColor(e);
        }
    }
}