using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine
{
    public enum VariableType
    {
        Register, Variable
    }

    public enum MessageBoxResult : uint
    {
        Ok = 1,
        Cancel = 2,
        Abort = 3,
        Retry = 4,
        Ignore = 5,
        Yes = 6,
        No = 7,
        Close,
        Help,
        TryAgain = 10,
        Continue = 11,
        Timeout = 32000
    }

    public enum Register : byte
    {
        EAX, //Extended Accumulator Register
        EBX, //Extended Base Register
        ECX, //Extended Counter Register
        EDX, //Extended Data Register
        ESI, //Extended Source Index
        EDI, //Extended Destination Index
        EBP, //Extended Base Pointer
        ESP, //Extended Stack Pointer
        EIP, //Extended Instruction Pointer
        AH, AX, AL, IP, CX, DX, BX,
        none //no register given
    }

    public enum VideoMode
    {
        Graphics, Text
    }

    public enum Priority
    {
        Realtime, High, Normal, Low
    }
}