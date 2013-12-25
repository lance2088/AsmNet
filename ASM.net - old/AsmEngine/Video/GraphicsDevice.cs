using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;

namespace AsmEngine.Video
{
    public class GraphicsDevice
    {
        [DllImport("User32.dll")]
        private static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        private static extern void ReleaseDC(IntPtr hWnd, IntPtr hDC);

        public VideoMemory videoMemory;
        internal Thread RenderThread;
        private bool draw;

        public GraphicsDevice(IntPtr hwnd, Size size, int RefreshRate)
        {
            //VideoMemory
            videoMemory = new VideoMemory(GetDC(hwnd), size, RefreshRate);
        }

        public void StartDrawLoop()
        {
            RenderThread = new Thread(new ThreadStart(DrawLoop));
            draw = true;
            RenderThread.Start();
        }

        public void StopDrawLoop()
        {
            draw = false;
        }

        private void DrawLoop()
        {
            using (Graphics g = Graphics.FromHdc(videoMemory.device))
            {
                while (draw)
                {
                    try
                    {
                        lock (videoMemory.screen)
                        {
                            g.DrawImage(videoMemory.screen, new Point(0, 0));

                            for (int i = 0; i < videoMemory.strings.Count; i++)
                                g.DrawString(videoMemory.strings[i].message, new Font("Arial", videoMemory.strings[i].size), new SolidBrush(Color.White), videoMemory.strings[i].loc);
                        }
                        Thread.Sleep(1000 / videoMemory.RefreshRate);
                    }
                    catch
                    {
                        return;
                    }
                }
            }
            if(videoMemory != null)
                ReleaseDC(IntPtr.Zero, videoMemory.device);
        }

        public void WriteString(string message, Point loc, int size, Color color)
        {
            lock (videoMemory.screen)
            {
                videoMemory.strings.Add(new VideoMessage(message, size, loc, color));
            }
        }

        public void SetPixel(Color color, Point loc)
        {
            lock (videoMemory.screen)
            {
                videoMemory.screen.SetPixel(loc.X, loc.Y, color);
            }
        }
    }
}