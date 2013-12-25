using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Asm.Net.src
{
    public class Module
    {
        public string DLL { get; private set; }
        public string FilePath { get; private set; }
        public Instruction[] Instructions { get; private set; }

        public Module(string dll)
        {
            this.DLL = dll;

            //Get module path
            //for temp we are using this way to get the file path
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.FileName.ToLower().EndsWith(dll.ToLower()))
                {
                    this.FilePath = module.FileName;
                    break;
                }
            }
            Instructions = x86Data.PeToInstructions(FilePath);
        }
    }
}