using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Core.Drivers;
using AsmEngine.Video;
using System.Threading;
using System.Drawing;

namespace AsmEngine.Core
{
    public class BIOS
    {
        public List<Driver> drivers;

        public BIOS()
        {
            Console.WriteLine("Initializing BIOS");

            GraphicsDevice device = new GraphicsDevice(IntPtr.Zero, new System.Drawing.Size(320, 200), 1);
            device.WriteString("ASM.net", new Point(10, 10), 50, Color.White);
            device.WriteString("Virtual 8086", new Point(10, 75), 20, Color.White);
            device.WriteString("BIOS", new Point(150, 75), 25, Color.White);
            device.StartDrawLoop();
            //Thread.Sleep(10000); //wait 10 seconds


            drivers = new List<Driver>();
            drivers.Add(new VgaDriver());
            drivers.Add(new KeyboardDriver());

            //Load the drivers
            for (int i = 0; i < drivers.Count; i++)
            {
                drivers[i].Boot();
                drivers[i].OnLoad();
                ((VgaDriver)drivers[0]).DrawString(new VideoMessage("Loaded " + drivers[i].Name, 8, new Point(5, 100 + (i * 10)), Color.White));

                if (drivers[i].GetType() == typeof(VgaDriver))
                {
                    //Stop the BIOS GraphicsDevice and use the driver instead
                    device.StopDrawLoop();
                    ((VgaDriver)drivers[i]).DrawString(new VideoMessage("ASM.net", 50, new Point(10, 10), Color.Red));
                    ((VgaDriver)drivers[i]).DrawString(new VideoMessage("Virtual 8086", 20, new Point(10, 75), Color.Green));
                    ((VgaDriver)drivers[i]).DrawString(new VideoMessage("BIOS", 25, new Point(150, 75), Color.White));

                    device.videoMemory.screen = null;
                    device.videoMemory = null;
                    device = null;
                }

                Thread.Sleep(2500);
            }
        }

        public void UpdateDrivers()
        {
            for (int i = 0; i < drivers.Count; i++)
            {
                drivers[i].OnUpdate();
            }
        }
    }
}