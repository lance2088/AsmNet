using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using AsmEngine.Events;
using System.Drawing;

namespace AsmEngine.Instructions.Interrupts
{
    public unsafe class INT10 : IInterrupt, AsmOpcode
    {
        int AsmOpcode.MemAddress { get; set; }
        public int MemAddress { get; set; }

        public INT10(int MemAddress)
            : base()
        {
            this.MemAddress = MemAddress;
        }

        public void Execute()
        {
            switch (AssemblerExecute.registers.AH)
            {
                case 0:
                {
                    switch (AssemblerExecute.registers.AL)
                    {
                        case 0x13:
                            SetVideoModeEvent.onSetVideoMode(new SetVideoModeEventArgs(256, 320, 200));
                            AssemblerExecute.videoMode = VideoMode.Graphics;
                            AssemblerExecute.graphicsDevice = new Video.GraphicsDevice(IntPtr.Zero, new Size(320, 200), 5);
                            if (SetVideoModeEvent.VideoModeNULL()) //debug check
                                AssemblerExecute.graphicsDevice.StartDrawLoop();
                            break;
                    }
                    break;
                }
                case 0xC:
                {
                    Color color = Color.Black;

                    switch (AssemblerExecute.registers.AL)
                    {
                        case 0: color = Color.Black; break;
                        case 1: color = Color.DarkBlue; break;
                        case 2: color = Color.DarkGreen; break;
                        case 3: color = Color.DarkCyan; break;
                        case 4: color = Color.DarkRed; break;
                        case 5: color = Color.DarkMagenta; break;
                        case 6: color = Color.Yellow; break; //DarkYellow
                        case 7: color = Color.Gray; break;
                        case 8: color = Color.DarkGray; break;
                        case 9: color = Color.Blue; break;
                        case 10: color = Color.Green; break;
                        case 11: color = Color.Cyan; break;
                        case 12: color = Color.Magenta; break;
                        case 13: color = Color.Magenta; break; //?? purple
                        case 14: color = Color.Yellow; break;
                        case 15: color = Color.White; break;
                    }

                    DrawPixelEvent.onDrawPixel(new DrawPixelEventArgs(AssemblerExecute.registers.CX, //width
                                                                      AssemblerExecute.registers.DX, //height
                                                                      color));

                    AssemblerExecute.graphicsDevice.SetPixel(color, new Point(AssemblerExecute.registers.CX, AssemblerExecute.registers.DX));

                    /*if (AssemblerExecute.inConsole)
                    {
                        Console.SetCursorPosition(AssemblerExecute.registers.CX, AssemblerExecute.registers.DX);
                        Console.BackgroundColor = (ConsoleColor)Enum.Parse(typeof(ConsoleColor), color.Name);
                    }*/

                    break;
                }
            }
        }
    }
}