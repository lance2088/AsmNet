using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace AsmEngine.Core.CPU
{
    public class Virtual8086
    {
        public BIOS bios;
        internal Thread ProcessorThread;
        internal bool halt;

        public Virtual8086()
        {
            bios = new BIOS();
            ProcessorThread = new Thread(new ThreadStart(CpuLoop));
            ProcessorThread.Start();
        }

        private void CpuLoop()
        {
            while (!halt)
            {
                bios.UpdateDrivers();
            }
        }

        public void HALT()
        {
            this.halt = true;
        }
    }
}