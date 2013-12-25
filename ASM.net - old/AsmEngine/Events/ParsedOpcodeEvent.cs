using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;
using System.Windows.Forms;

namespace AsmEngine.Events
{
    public delegate void ParsedOpcodeEventHandler(ParsedOpcodeEventArgs e);

    public class ParsedOpcodeEventArgs : EventArgs
    {
        public readonly AsmOpcode opcode;
        public readonly string AsmCode;
        public readonly int MemAddress;

        public ParsedOpcodeEventArgs(AsmOpcode opcode, string AsmCode)
        {
            this.opcode = opcode;
            this.AsmCode = AsmCode;
        }
        public ParsedOpcodeEventArgs(AsmOpcode opcode, string AsmCode, int MemAddress)
        {
            this.opcode = opcode;
            this.AsmCode = AsmCode;
            this.MemAddress = MemAddress;
        }
    }

    public class ParsedOpcodeEvent
    {
        public static event ParsedOpcodeEventHandler ParsedOpcode;

        public static void onParsedOpcodeEvent(ParsedOpcodeEventArgs e)
        {
            if (ParsedOpcode != null)
                ParsedOpcode(e);
        }
    }
}