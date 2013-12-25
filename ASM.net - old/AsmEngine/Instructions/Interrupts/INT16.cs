using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using AsmEngine.Wrappers;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace AsmEngine.Instructions.Interrupts
{
    public unsafe class INT16 : IInterrupt, AsmOpcode
    {
        int AsmOpcode.MemAddress { get; set; }
        [DllImport("user32.dll")]
        static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);
        public int MemAddress { get; set; }

        public INT16(int MemAddress)
            : base()
        {
            this.MemAddress = MemAddress;
        }

        public void  Execute()
        {
            switch (AssemblerExecute.registers.AH)
            {
                case 0:
                {
                    //Return:
                    //AH = BIOS scan code
                    //AL = ASCII character

                    AssemblerExecute.flags.InterruptFlag = false;
                    bool KeyPressed = false;
                    while (!KeyPressed)
                    {
                        foreach (System.Int32 i in Enum.GetValues(typeof(Keys)))
                        {
                            int x = GetAsyncKeyState((Keys)i);
                            //if ((x > 64) || (x == -32767))
                            if ((x > 64) || (x == 0x0D))
                            {
                                AssemblerExecute.registers.AL = (byte)i;
                                KeyPressed = true;
                                break;
                            }
                        }
                        Thread.Sleep(100);
                    }
                    AssemblerExecute.flags.InterruptFlag = true;
                }
                break;
            }
        }
    }
}