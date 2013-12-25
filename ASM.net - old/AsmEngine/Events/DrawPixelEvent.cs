using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AsmEngine.Events
{
    public delegate void DrawPixelEventHandler(DrawPixelEventArgs e);

    public class DrawPixelEventArgs : EventArgs
    {
        public readonly int column;
        public readonly int row;
        public readonly Color color;

        public DrawPixelEventArgs(int column, int row, Color color)
        {
            this.column = column;
            this.row = row;
            this.color = color;
        }
    }

    public class DrawPixelEvent
    {
        public static event DrawPixelEventHandler DrawPixel;

        public static void onDrawPixel(DrawPixelEventArgs e)
        {
            if (DrawPixel != null)
                DrawPixel(e);
        }
    }
}