using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Instructions;

namespace AsmEngine.NetEngine.IO
{
    internal class WriteFile : Acall
    {
        public WriteFile()
            : base()
        {
        }

        public override void Call()
        {
            System.IO.File.WriteAllText((string)args[0].value, (string)args[1].value);
        }
    }
}