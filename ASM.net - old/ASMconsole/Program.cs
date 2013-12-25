using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AsmEngine;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;
using AsmEngine.Video;
using AsmEngine.Core.CPU;
using AsmEngine.Core.Drivers;
using AsmEngine.Wrappers;

namespace ASMconsole
{
    class Program
    {
        [DllImport("User32.dll")]
        static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("User32.dll")]
        static extern void ReleaseDC(IntPtr hWnd, IntPtr hDC);

        static void Main(string[] args)
        {
            /*Console.Title = "C# benchmark";
            Stopwatch sw = Stopwatch.StartNew();
            byte lol = 0;
            int loops = 0;
            for (int x = 0; x < 255; x++)
            {
                for (int i = 0; i < 255; i++)
                {
                    lol++;
                    loops++;
                }
                Console.WriteLine("ecx: dsadsadsadsadsadsadsadsadsadsadsadsadsa");
            }
            Console.WriteLine(sw.ElapsedMilliseconds);*/


/*
            Type type = typeof(ConsoleColor);
            Console.ForegroundColor = ConsoleColor.White;
            foreach (var name in Enum.GetNames(type))
            {
                //Console.BackgroundColor = (ConsoleColor)Enum.Parse(type, name);
                Console.Write(" ");
                //Console.WriteLine(name);
            }

            Console.BackgroundColor = ConsoleColor.Black;
            foreach (var name in Enum.GetNames(type))
            {
                Console.ForegroundColor = (ConsoleColor)Enum.Parse(type, name);
                Console.WriteLine(name);
            }*/

            //GraphicsDevice graphics = new GraphicsDevice(IntPtr.Zero, new Size(320, 200), 10);

            Virtual8086 lol = new Virtual8086();

            Process.GetCurrentProcess().WaitForExit();
        }
    }
}
