using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine.Core.Drivers
{
    public abstract class Driver
    {
        public abstract void Boot();
        public abstract void OnLoad();
        public abstract void OnUnload();
        public abstract void OnUpdate();

        public string Name { get; private set; }
        public Priority priority { get; private set; }

        public Driver(string name, Priority priority)
        {
            this.Name = name;
            this.priority = priority;
        }
    }
}