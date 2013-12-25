using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.Jumps
{
    public class JE : Instruction, IJump
    {
        string IJump.Label { get; set; }
        int IJump.JumpAddress { get; set; }
        public JE(int JmpAddress)
            : base(5, typeof(IJump))
        {
            ((IJump)this).JumpAddress = JmpAddress;
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.JE,
                                BitConverter.GetBytes(((IJump)this).JumpAddress)[0],
                                BitConverter.GetBytes(((IJump)this).JumpAddress)[1],
                                BitConverter.GetBytes(((IJump)this).JumpAddress)[2],
                                BitConverter.GetBytes(((IJump)this).JumpAddress)[3]
                              };
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "JE 0x" + ((IJump)this).JumpAddress.ToString("X6");
        }

        public int NextIpAddress(Flags flags, Registers registers)
        {
            if (flags.ZeroFlag)
            {
                return ((IJump)this).JumpAddress;
            }
            return VirtualAddress.Address + VirtualAddress.Size;
        }
    }
}