using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Asm.Net.src.Sections;
using System.Reflection;

namespace Asm.Net.src.Opcodes.MOV
{
    class MOV_REGISTER_VARIABLE_INDEX : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public string varName {get; private set;}
        public Register register { get; private set; }
        public int Index { get; private set; }

        ///<summary> the newValue must be serializable </summary>
        public MOV_REGISTER_VARIABLE_INDEX(VirtualAddress VariableAddress, string varName, int index, Register register)
            : base(6, typeof(IMov))
        {
            this.ModifiyValue = VariableAddress;
            this.varName = varName;
            this.register = register;
            this.Index = index;
        }
        public MOV_REGISTER_VARIABLE_INDEX(VirtualAddress VariableAddress, int index, Register register)
            : base(6, typeof(IMov))
        {
            this.varName = null;
            this.ModifiyValue = VariableAddress;
            this.register = register;
            this.Index = index;
        }

        public override byte[] ToByteArray()
        {
            PayloadWriter writer = new PayloadWriter();
            writer.WriteByte((byte)OpcodeList.MOV_EAX_DWORD_PTR);
            writer.WriteInteger(ModifiyValue.Address);
            writer.WriteByte(varName == null ? (byte)0 : (byte)1);
            writer.WriteInteger(Index);

            if (varName != null)
            {
                writer.WriteString(varName);
            }
            writer.WriteByte((byte)register);
            return writer.ToByteArray();
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV " + register.ToString() + ", DWORD PTR[" + ModifiyValue.Address.ToString("X6") + " + " + Index + "]";
        }

        public void Execute(Registers registers, DataSection dataSection)
        {
            if (dataSection.Variables[ModifiyValue.Address].Value == null)
                throw new Exception("Variable is not initialized, " + ToString());
            if (varName != null)
            {
                if (dataSection.Variables[ModifiyValue.Address].Value.GetType().GetField(varName) == null)
                {
                    throw new Exception("Unable to find the variable in the structure/class, " + ToString());
                }
            }

            FieldInfo field = null;
            if(varName != null)
                dataSection.Variables[ModifiyValue.Address].Value.GetType().GetField(varName);

            //lets convert to the correct Type
            object newVal = null;

            if (varName != null)
            {
                if (field.FieldType == typeof(byte[]))
                {
                    newVal = ((byte[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(int[]))
                {
                    newVal = ((int[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(uint[]))
                {
                    newVal = ((uint[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(ushort[]))
                {
                    newVal = ((ushort[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(short[]))
                {
                    newVal = ((short[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(ulong[]))
                {
                    newVal = ((ulong[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(long[]))
                {
                    newVal = ((long[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(IntPtr))
                {
                    newVal = ((IntPtr[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(UIntPtr))
                {
                    newVal = ((UIntPtr[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(string[]))
                {
                    newVal = ((string[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index];
                }
                else if (field.FieldType == typeof(string))
                {
                    newVal = ((string)field.GetValue(dataSection.Variables[ModifiyValue.Address].Value)).ToCharArray()[Index];
                }
            }
            else
            {
                Type type = dataSection.Variables[ModifiyValue.Address].Value.GetType();

                if (type == typeof(byte[]))
                {
                    newVal = ((byte[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(int[]))
                {
                    newVal = ((int[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(uint[]))
                {
                    newVal = ((uint[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(ushort[]))
                {
                    newVal = ((ushort[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(short[]))
                {
                    newVal = ((short[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(ulong[]))
                {
                    newVal = ((ulong[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(long[]))
                {
                    newVal = ((long[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(IntPtr))
                {
                    newVal = ((IntPtr[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(UIntPtr))
                {
                    newVal = ((UIntPtr[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(string[]))
                {
                    newVal = ((string[])dataSection.Variables[ModifiyValue.Address].Value)[Index];
                }
                else if (type == typeof(string))
                {
                    newVal = ((string)dataSection.Variables[ModifiyValue.Address].Value).ToCharArray()[Index];
                }
            }

            switch (this.register)
            {
                case Register.EAX: { registers.EAX = Convert.ToInt32(newVal); break; }
                case Register.EBP: { registers.EBP = Convert.ToInt32(newVal); break; }
                case Register.EBX: { registers.EBX = Convert.ToInt32(newVal); break; }
                case Register.ECX: { registers.ECX = Convert.ToInt32(newVal); break; }
                case Register.EDI: { registers.EDI = Convert.ToInt32(newVal); break; }
                case Register.EDX: { registers.EDX = Convert.ToInt32(newVal); break; }
                case Register.ESI: { registers.ESI = Convert.ToInt32(newVal); break; }
                case Register.ESP: { registers.ESP = Convert.ToInt32(newVal); break; }
            }
        }
    }
}