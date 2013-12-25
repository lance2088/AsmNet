using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Asm.Net.src
{
    [StructLayout(LayoutKind.Explicit)]
    public unsafe class Registers
    {
        public Registers(Flags flags)
        {
            this.flags = flags;
        }

        [FieldOffset(0)]
        public int EAX;
        [FieldOffset(0)]
        public short AX;
        [FieldOffset(0)]
        public byte AL;
        [FieldOffset(1)]
        public byte AH;

        [FieldOffset(4)]
        public int EBX;
        [FieldOffset(4)]
        public short BX;
        [FieldOffset(4)]
        public byte BL;

        [FieldOffset(8)]
        public int ECX;
        [FieldOffset(8)]
        public short CX;
        [FieldOffset(8)]
        public byte CL;

        [FieldOffset(12)]
        public int EDX;
        [FieldOffset(12)]
        public short DX;
        [FieldOffset(12)]
        public byte DL;

        [FieldOffset(16)]
        private int _EIP;
        [FieldOffset(16)]
        public short IP;

        [FieldOffset(20)]
        public Flags flags; //SizeOf = 56

        [FieldOffset(80)]
        public int ESP;
        [FieldOffset(80)]
        public short SP;
        
        [FieldOffset(84)]
        public int EBP;
        [FieldOffset(85)]
        public byte CH;
        [FieldOffset(84)]
        public short BP;
        
        [FieldOffset(88)]
        public int ESI;
        [FieldOffset(89)]
        public byte DH;
        [FieldOffset(88)]
        public short SI;
        
        [FieldOffset(92)]
        public int EDI;
        [FieldOffset(93)]
        public byte BH;
        [FieldOffset(92)]
        public short DI;

        public int EIP
        {
            get { return _EIP; }
            set
            {
                _EIP = value;
                if (_EIP <= 0)
                {
                    flags.OverflowFlag = true;
                    throw new Exception("access violation at address " + value.ToString("X6"));
                }
            }
        }
    }
}