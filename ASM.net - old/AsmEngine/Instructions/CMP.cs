using System;
using System.Collections.Generic;
using System.Text;
using AsmEngine.Objects;

namespace AsmEngine.Instructions
{
    public class CMP : AsmOpcode
    {
        public int MemAddress { get; set; }
        public object Expression1 { get; private set; }
        public object Expression2 { get; private set; }

        public CMP(int MemAddress, object exp1, object exp2)
            : base()
        {
            this.MemAddress = MemAddress;
            this.Expression1 = exp1;
            this.Expression2 = exp2;
        }

        public bool Compare()
        {
            int exp1 = 0;
            int exp2 = 0;

            if (Expression1.GetType() == typeof(Register))
            {
                if ((Register)Expression1 == Register.EAX)
                    exp1 = AssemblerExecute.registers.EAX;
                else if ((Register)Expression1 == Register.AH)
                    exp1 = AssemblerExecute.registers.AH;
                else if ((Register)Expression1 == Register.AL)
                    exp1 = AssemblerExecute.registers.AL;
                else if ((Register)Expression1 == Register.ECX)
                    exp1 = AssemblerExecute.registers.ECX;
                else if ((Register)Expression1 == Register.AX)
                    exp1 = AssemblerExecute.registers.AX;
                else if ((Register)Expression1 == Register.CX)
                    exp1 = AssemblerExecute.registers.CX;
                else if ((Register)Expression1 == Register.DX)
                    exp1 = AssemblerExecute.registers.DX;
                else if ((Register)Expression1 == Register.BX)
                    exp1 = AssemblerExecute.registers.BX;
            }

            if (Expression2.GetType() == typeof(Register))
            {
                if ((Register)Expression2 == Register.EAX)
                    exp2 = AssemblerExecute.registers.EAX;
                else if ((Register)Expression2 == Register.AH)
                    exp2 = AssemblerExecute.registers.AH;
                else if ((Register)Expression2 == Register.AL)
                    exp2 = AssemblerExecute.registers.AL;
                else if ((Register)Expression2 == Register.ECX)
                    exp2 = AssemblerExecute.registers.ECX;
                else if ((Register)Expression2 == Register.AX)
                    exp2 = AssemblerExecute.registers.AX;
                else if ((Register)Expression2 == Register.CX)
                    exp2 = AssemblerExecute.registers.CX;
                else if ((Register)Expression2 == Register.DX)
                    exp2 = AssemblerExecute.registers.DX;
                else if ((Register)Expression2 == Register.BX)
                    exp2 = AssemblerExecute.registers.BX;
            }
            else if (Expression2.GetType() == typeof(byte))
                exp2 = Convert.ToInt32(Expression2);
            else if (Expression2.GetType() == typeof(short))
                exp2 = Convert.ToInt32(Expression2);
            else if (Expression2.GetType() == typeof(int))
                exp2 = Convert.ToInt32(Expression2);
            else if (Expression2.GetType() == typeof(uint))
                exp2 = Convert.ToInt32(Expression2);
            else if (Expression2.GetType() == typeof(long))
                exp2 = Convert.ToInt32(Expression2);
            else if (Expression2.GetType() == typeof(ulong))
                exp2 = Convert.ToInt32(Expression2);

            if (exp1 == exp2)
            {
                AssemblerExecute.flags.ZeroFlag = true;
                return true;
            }
            else
            {
                AssemblerExecute.flags.ZeroFlag = false;
                return true;
            }
        }
    }
}