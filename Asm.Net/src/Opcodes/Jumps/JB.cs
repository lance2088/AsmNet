using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.Jumps
{
    public class JB : Instruction, IJump
    {
        public string Label { get; set; }
        public int JumpAddress { get; set; }

        public JB(int JmpAddress)
            : base(6, typeof(IJump))
        {
            ((IJump)this).JumpAddress = JmpAddress;
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.JB_one,
                                (byte)OpcodeList.JB_two,
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
            return "JB 0x" + ((IJump)this).JumpAddress.ToString("X6");
        }

        public int NextIpAddress(Flags flags, Registers registers)
        {
            if (flags.CarryFlag)
            {
                return ((IJump)this).JumpAddress;
            }
            return VirtualAddress.Address + VirtualAddress.Size;
        }
    }
}