using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src;
using Asm.Net;
using System.Diagnostics;
using Asm.Net.src.Opcodes;
using Asm.Net.src.Interfaces;
using System.Threading;
using Asm.Net.src.Hardware;
using System.Windows.Forms;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Asm.Net.src.Sections;
using System.Runtime.InteropServices;

namespace executor
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.Run(new Form1());
        }
    }
}