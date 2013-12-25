using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src
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
        private bool _AdjustFlag;
        private bool _VirtualInterruptFlag;
        private bool _VirtualInterruptPendingFlag;
        private bool _CpuIdFlag;

        public bool CarryFlag
        {
            get { return _CarryFlag; }
            set
            {
                _CarryFlag = value;
            }
        }

        public bool ParityFlag
        {
            get { return _ParityFlag; }
            set
            {
                _ParityFlag = value;
            }
        }
        public bool CarryFourBitsFlag
        {
            get { return _CarryFourBitsFlag; }
            set
            {
                _CarryFourBitsFlag = value;
            }
        }
        public bool ZeroFlag
        {
            get { return _ZeroFlag; }
            set
            {
                _ZeroFlag = value;
            }
        }
        public bool SignFlag
        {
            get { return _SignFlag; }
            set
            {
                _SignFlag = value;
            }
        }
        public bool TrapFlag
        {
            get { return _TrapFlag; }
            set
            {
                _TrapFlag = value;
            }
        }
        public bool InterruptFlag
        {
            get { return _InterruptFlag; }
            set
            {
                _InterruptFlag = value;
            }
        }
        public bool DirectionFlag
        {
            get { return _DirectionFlag; }
            set
            {
                _DirectionFlag = value;
            }
        }
        public bool OverflowFlag
        {
            get { return _OverflowFlag; }
            set
            {
                _OverflowFlag = value;
            }
        }
        public bool IOFlag
        {
            get { return _IOFlag; }
            set
            {
                _IOFlag = value;
            }
        }
        public bool NestedTaskFlag
        {
            get { return _NestedTaskFlag; }
            set
            {
                _NestedTaskFlag = value;
            }
        }
        public bool ResumeFlag
        {
            get { return _ResumeFlag; }
            set
            {
                _ResumeFlag = value;
            }
        }
        public bool Virtual8086Flag
        {
            get { return _Virtual8086Flag; }
            set
            {
                _Virtual8086Flag = value;
            }
        }
        public bool AlignmentFlag
        {
            get { return _AlignmentFlag; }
            set
            {
                _AlignmentFlag = value;
            }
        }
    }
}