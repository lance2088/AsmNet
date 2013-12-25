using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using AsmEngine.DataTypes;
using AsmEngine.Instructions;

namespace AsmEngine.NetEngine.Console
{
    internal class Write : Acall
    {
        [DllImport("kernel32.dll")]
        protected static extern bool WriteConsole(IntPtr hConsoleOutput, string lpBuffer,
                                        uint nNumberOfCharsToWrite, out uint lpNumberOfCharsWritten,
                                        IntPtr lpReserved);
        [DllImport("kernel32.dll", SetLastError = true)]
        protected static extern IntPtr GetStdHandle(int nStdHandle);

        public static IntPtr ConsoleHandle { get; protected set; }

        public Write()
            : base()
        { }

        public override void Call()
        {
            if (ConsoleHandle == IntPtr.Zero)
                ConsoleHandle = GetStdHandle(-12);

            if (AssemblerExecute.videoMode == VideoMode.Text || AssemblerExecute.inConsole)
            {
                uint length = (uint)((string)args[0].value).Length;
                WriteConsole(ConsoleHandle, (string)args[0].value, length, out length, IntPtr.Zero);
            }
        }
    }
}