using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine
{
    public class AssemblerNamespace
    {
        public byte name { get; private set; }
        public SortedList<byte, AssemblerClass> classes;


        public AssemblerNamespace(byte name)
        {
            this.name = name;
            classes = new SortedList<byte,AssemblerClass>();
        }

        public void AddClass(AssemblerClass asmClass)
        {
            if (!classes.ContainsKey(asmClass.name))
                classes.Add(asmClass.name, asmClass);
        }
    }
}