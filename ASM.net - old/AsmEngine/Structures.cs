using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Events;
using System.Runtime.InteropServices;

namespace AsmEngine
{
    public struct Flags
    {
        private bool _CarryFlag; //Carry or borrow to destination operand
        private bool _ParityFlag; //Even parity in low-order byte of result
        private bool _CarryFourBitsFlag; //Carry or borrow to low four-order bits
        private bool _ZeroFlag; //Result equal to 0
        private bool _SignFlag; //Result has negative sign
        private bool _TrapFlag; //Switches to single-step mode
        private bool _InterruptFlag; //Enables interrupts (disables if cleared)
        private bool _DirectionFlag; // String operations process down rather than up
        private bool _OverflowFlag; //Result too large or small for destination
        private bool _IOFlag; //I/O Privilege Level for IN, INS, OUT, OUTS, Priv CLI, STI (286+)
        private bool _NestedTaskFlag; //Instruction caused nested task switch (286+)
        private bool _ResumeFlag; //Debug faults disabled during instructionexecution (386+)
        private bool _Virtual8086Flag; //Currently executing 8086 code on virtual processor (386+)
        private bool _AlignmentFlag; //Data aligned to four-byte boundary (486+) Check

        public bool CarryFlag
        {
            get { return _CarryFlag; }
            set
            {
                _CarryFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }

        public bool ParityFlag
        {
            get { return _ParityFlag; }
            set
            {
                _ParityFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool CarryFourBitsFlag
        {
            get { return _CarryFourBitsFlag; }
            set
            {
                _CarryFourBitsFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool ZeroFlag
        {
            get { return _ZeroFlag; }
            set
            {
                _ZeroFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool SignFlag
        {
            get { return _SignFlag; }
            set
            {
                _SignFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool TrapFlag
        {
            get { return _TrapFlag; }
            set
            {
                _TrapFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool InterruptFlag
        {
            get { return _InterruptFlag; }
            set
            {
                _InterruptFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool DirectionFlag
        {
            get { return _DirectionFlag; }
            set
            {
                _DirectionFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool OverflowFlag
        {
            get { return _OverflowFlag; }
            set
            {
                _OverflowFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool IOFlag
        {
            get { return _IOFlag; }
            set
            {
                _IOFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool NestedTaskFlag
        {
            get { return _NestedTaskFlag; }
            set
            {
                _NestedTaskFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool ResumeFlag
        {
            get { return _ResumeFlag; }
            set
            {
                _ResumeFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool Virtual8086Flag
        {
            get { return _Virtual8086Flag; }
            set
            {
                _Virtual8086Flag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
        public bool AlignmentFlag
        {
            get { return _AlignmentFlag; }
            set
            {
                _AlignmentFlag = value;
                FlagUpdateEvent.onFlagUpdate();
            }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct Registers
    {
        [FieldOffset(0)]
        private int _EAX;
        [FieldOffset(0)]
        private short _AX;
        [FieldOffset(0)]
        private byte _AL;
        [FieldOffset(1)]
        private byte _AH;

        [FieldOffset(4)]
        private int _EBX;
        [FieldOffset(4)]
        private short _BX;
        [FieldOffset(4)]
        private byte _BL;
        [FieldOffset(5)]
        private byte _BH;

        [FieldOffset(8)]
        private int _ECX;
        [FieldOffset(8)]
        private short _CX;
        [FieldOffset(8)]
        private byte _CL;
        [FieldOffset(9)]
        private byte _CH;

        [FieldOffset(12)]
        private int _EDX;
        [FieldOffset(12)]
        private short _DX;
        [FieldOffset(12)]
        private byte _DL;
        [FieldOffset(13)]
        private byte _DH;

        [FieldOffset(16)]
        private int _EIP;
        [FieldOffset(16)]
        private short _IP;

        public int EAX
        {
            get { return _EAX; }
            set
            {
                _EAX = value;
                if (_EAX < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public int ECX
        {
            get { return _ECX; }
            set
            {
                _ECX = value;
                if (_ECX < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public int EBX
        {
            get { return _EBX; }
            set
            {
                _EBX = value;
                if (_EBX < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public int EDX
        {
            get { return _EDX; }
            set
            {
                _EDX = value;
                if (_EDX < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public int EIP
        {
            get { return _EIP; }
            set
            {
                _EIP = value;
                if (_EIP < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }

        public short AX
        {
            get { return _AX; }
            set
            {
                _AX = value;
                if (_AX < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public byte AH
        {
            get { return _AH; }
            set
            {
                _AH = value;
                if (_AH < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public byte AL
        {
            get { return _AL; }
            set
            {
                _AL = value;
                if (_AL < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public short IP
        {
            get { return _IP; }
            set
            {
                _IP = value;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public short CX
        {
            get { return _CX; }
            set
            {
                _CX = value;
                if (_CX < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public short DX
        {
            get { return _DX; }
            set
            {
                _DX = value;
                if (_DX < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
        public short BX
        {
            get { return _BX; }
            set
            {
                _BX = value;
                if (_BX < 0)
                    AssemblerExecute.flags.OverflowFlag = true;
                RegisterUpdateEvent.onRegisterUpdate();
            }
        }
    }
}