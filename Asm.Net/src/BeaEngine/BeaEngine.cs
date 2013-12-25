using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Asm.Net.src.BeaEngine
{
    public class BeaEngine
    {
        [DllImport("BeaEngine.dll")]
        public static extern int Disasm([In, Out, MarshalAs(UnmanagedType.LPStruct)] Disasm disasm);

        [DllImport("BeaEngine.dll")]
        private static extern string BeaEngineVersion();

        [DllImport("BeaEngine.dll")]
        private static extern string BeaEngineRevision();

        public static string Version
        {
            get
            {
                return BeaEngineVersion();
            }
        }

        public static string Revision
        {
            get
            {
                return BeaEngineRevision();
            }
        }
    }
}