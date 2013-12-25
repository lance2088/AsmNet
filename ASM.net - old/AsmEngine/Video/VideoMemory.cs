using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace AsmEngine.Video
{
    public class VideoMessage
    {
        public Point loc;
        public int size;
        public String message;
        public Color color;

        public VideoMessage(string message, int size, Point loc, Color color)
        {
            this.message = message;
            this.size = size;
            this.loc = loc;
            this.color = color;
        }
    }

    public class VideoMemory
    {
        internal Bitmap screen;
        public List<VideoMessage> strings;

        public int Width { get; private set; }
        public int Height { get; private set; }
        public IntPtr device { get; private set; }
        public int RefreshRate { get; private set; }

        public VideoMemory(IntPtr device, Size size, int RefreshRate)
        {
            this.screen = new Bitmap(size.Width, size.Height);
            this.strings = new List<VideoMessage>();
            this.device = device;
            this.Width = size.Width;
            this.Height = size.Height;
            this.RefreshRate = RefreshRate;

            for (int x = 0; x < size.Width; x++)
            {
                for (int y = 0; y < size.Height; y++)
                {
                    screen.SetPixel(x, y, Color.Black);
                }
            }
        }
    }
}