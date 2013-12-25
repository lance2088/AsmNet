using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine.Core.Drivers
{
    public class KeyboardDriver : Driver
    {
        public const string DRIVER_NAME = "keyboard";
        public KeyboardDriver()
            : base(DRIVER_NAME, Priority.High)
        { }

        public override void Boot()
        {
            Console.WriteLine("booting keyboard driver");
        }

        public override void OnLoad()
        {
            Console.WriteLine("Loaded Keyboard Driver");
        }

        public override void OnUnload()
        {
            
        }

        public override void OnUpdate()
        {
            
        }
    }
}