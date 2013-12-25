using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace ASM.net.src
{
    public enum BuildOutput { Console, WindowsApp }

    public class Settings
    {
        public static int VirtualScreenFPS;
        public static BuildOutput buildOutput = BuildOutput.Console;
        public static bool MergeEngine = true;
        public static bool CompressEngine = false;

        //obfuscate settings
        public static bool Obfuscate = false;

        public Settings()
        {
            VirtualScreenFPS = 10;
        }
    }
}