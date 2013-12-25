using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.PUSH
{
    public class PUSH_VALUE : Instruction, IPush
    {
        public object Value { get; set; }
        public int ValueAddress { get; set; }
        
        public PUSH_VALUE(ulong value) : base(10, typeof(IPush))
        {
            this.Value = value;
        }
        public PUSH_VALUE(long value) : base(10, typeof(IPush))
        {
            this.Value = value;
        }
        public PUSH_VALUE(uint value) : base(6, typeof(IPush))
        {
            this.Value = value;
        }
        public PUSH_VALUE(int value) : base(6, typeof(IPush))
        {
            this.Value = value;
        }
        public PUSH_VALUE(short value) : base(4, typeof(IPush))
        {
            this.Value = value;
        }
        public PUSH_VALUE(ushort value) : base(4, typeof(IPush))
        {
            this.Value = value;
        }
        public PUSH_VALUE(byte value) : base(3, typeof(IPush))
        {
            this.Value = value;
        }

        public override byte[] ToByteArray()
        {
            byte[] tmp = new byte[0];
            PayloadWriter writer = new PayloadWriter();
            writer.WriteByte((byte)OpcodeList.PUSH_VALUE);
            
            if(Value.GetType() == typeof(int))
            {
                tmp = BitConverter.GetBytes((int)Value);
                writer.WriteByte(0);
            }
            else if(Value.GetType() == typeof(uint))
            {
                tmp = BitConverter.GetBytes((uint)Value);
                writer.WriteByte(1);
            }
            else if(Value.GetType() == typeof(byte))
            {
                tmp = BitConverter.GetBytes((byte)Value);
                writer.WriteByte(2);
            }
            else if(Value.GetType() == typeof(short))
            {
               tmp = BitConverter.GetBytes((short)Value);
                writer.WriteByte(3);
            }
            else if(Value.GetType() == typeof(ushort))
            {
                tmp = BitConverter.GetBytes((ushort)Value);
                writer.WriteByte(4);
            }
            else if(Value.GetType() == typeof(ulong))
            {
                tmp = BitConverter.GetBytes((ulong)Value);
                writer.WriteByte(5);
            }
            else if(Value.GetType() == typeof(long))
            {
                tmp = BitConverter.GetBytes((long)Value);
                writer.WriteByte(6);
            }
            writer.WriteBytes(tmp);
            return writer.ToByteArray();
        }

        public override void Dispose()
        {
            
        }

        public void AddToStack(Registers registers, List<object> Stack, Sections.DataSection dataSection)
        {
            Stack.Add(this.Value);
        }

        public override string ToString()
        {
            return "PUSH " + Value;
        }
    }
}