using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Video;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AsmEngine.Core.Drivers
{
    public class VgaDriver : Driver
    {
        [DllImport("User32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        private static extern void ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public const string DRIVER_NAME = "VGA Driver";
        public VideoMemory videoMemory;

        public VgaDriver()
            : base(DRIVER_NAME, Priority.High)
        {
            videoMemory = new VideoMemory(GetDC(IntPtr.Zero), new System.Drawing.Size(320, 200), 10);
        }

        public override void Boot()
        {
            Console.WriteLine("booting vga driver");
        }

        public override void OnLoad()
        {
            Console.WriteLine("Loaded VGA Driver");
        }

        public override void OnUnload()
        {
            ReleaseDC(IntPtr.Zero, videoMemory.device);
        }

        public override void OnUpdate()
        {
            using (Graphics g = Graphics.FromHdc(videoMemory.device))
                g.DrawImage(videoMemory.screen, new Point(0, 0));
        }

        internal void DrawString(VideoMessage message)
        {
            Graphics g = Graphics.FromImage(videoMemory.screen);
            g.DrawString(message.message, new Font("Arial", message.size), new SolidBrush(message.color), message.loc);
        }
    }
}