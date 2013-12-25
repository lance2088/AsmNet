using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.DataTypes;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using AsmEngine.Instructions;

namespace AsmEngine.NetEngine
{
    internal unsafe class MessageBox : Acall
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "MessageBoxA")]
        protected static extern int MessageBoxA(IntPtr hWnd, string text, string caption, long options);
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "MessageBoxW")]
        public static extern MessageBoxResult MessageBoxW(IntPtr hWnd, String text, String caption, int options);

        public MessageBox()
            : base()
        { }

        public override void Call()
        {
            string message = "";
            string title = "";

            if (args[1].value.GetType() == typeof(string))
                message = Convert.ToString(args[1].value);
            if (args[2].value.GetType() == typeof(string))
                title = Convert.ToString(args[2].value);

            AssemblerExecute.registers.EAX = ((int)MessageBoxW(new IntPtr((int)args[0].value), message, title, Convert.ToInt32(args[3].value)));
        }
    }
}