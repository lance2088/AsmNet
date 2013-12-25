using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Asm.Net.src.Sections;
using Asm.Net.src.Interfaces;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Asm.Net.src.Opcodes.MOV
{
    class MOV_VARIABLE_INDEX_VALUE : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public string varName {get; private set;}
        public object newValue {get; private set;}
        public Register register { get; private set; }
        internal bool isRegister = false;
        public int Index { get; private set; }

        ///<summary> the newValue must be serializable </summary>
        public MOV_VARIABLE_INDEX_VALUE(VirtualAddress VariableAddress, string varName, int index, object newValue)
            : base(3, typeof(IMov)) //0x66, 0xC7, 0x45
        {
            this.ModifiyValue = VariableAddress;
            this.varName = varName;
            this.newValue = newValue;
            this.Index = index;
        }
        public MOV_VARIABLE_INDEX_VALUE(VirtualAddress VariableAddress, string varName, int index, Register register)
            : base(3, typeof(IMov)) //0x66, 0xC7, 0x45
        {
            this.ModifiyValue = VariableAddress;
            this.varName = varName;
            this.register = register;
            this.isRegister = true;
            this.Index = index;
        }

        
        ///<summary> the newValue must be serializable </summary>
        public MOV_VARIABLE_INDEX_VALUE(VirtualAddress VariableAddress, int index, object newValue)
            : base(3, typeof(IMov)) //0x66, 0xC7, 0x45
        {
            this.ModifiyValue = VariableAddress;
            this.newValue = newValue;
            this.Index = index;
            this.varName = null;
        }
        public MOV_VARIABLE_INDEX_VALUE(VirtualAddress VariableAddress, int index, Register register)
            : base(3, typeof(IMov)) //0x66, 0xC7, 0x45
        {
            this.varName = null;
            this.ModifiyValue = VariableAddress;
            this.register = register;
            this.isRegister = true;
            this.Index = index;
        }

        public override byte[] ToByteArray()
        {
            PayloadWriter writer = new PayloadWriter();
            writer.WriteBytes(OtherOpcodes.MOV_VARIABLE_INDEX_VALUE);
            writer.WriteInteger(ModifiyValue.Address);
            writer.WriteByte(varName == null ? (byte)0 : (byte)1);
            writer.WriteByte(isRegister ? (byte)1 : (byte)0);
            writer.WriteInteger(Index);

            if (varName != null)
            {
                writer.WriteString(varName);
            }
            
            if (isRegister)
            {
                writer.WriteByte((byte)register);
            }
            else
            {
                //lets serialize the new value
                MemoryStream mem = new MemoryStream();
                new BinaryFormatter().Serialize(mem, newValue);
                writer.WriteInteger((int)mem.Length);
                writer.WriteBytes(mem.ToArray());
            }

            return writer.ToByteArray();
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            if(isRegister)
                return "MOV WORD PTR[" + ModifiyValue.Address.ToString("X6") + " + " + Index + "], " + register.ToString();
            return "MOV WORD PTR[" + ModifiyValue.Address.ToString("X6") + " + " + Index + "], " + newValue.ToString();
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

            if (isRegister)
            {
                switch (this.register)
                {
                    case Register.EAX: { newVal = registers.EAX; break; }
                    case Register.EBP: { newVal = registers.EBP; break; }
                    case Register.EBX: { newVal = registers.EBX; break; }
                    case Register.ECX: { newVal = registers.ECX; break; }
                    case Register.EDI: { newVal = registers.EDI; break; }
                    case Register.EDX: { newVal = registers.EDX; break; }
                    case Register.ESI: { newVal = registers.ESI; break; }
                    case Register.ESP: { newVal = registers.ESP; break; }
                }
            }
            else
            {
                newVal = this.newValue;
            }

            if (varName != null)
            {
                if (field.FieldType == typeof(byte[]))
                {
                    ((byte[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = (byte)newVal;
                }
                else if (field.FieldType == typeof(int[]))
                {
                    ((int[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = Convert.ToInt32(newVal);
                }
                else if (field.FieldType == typeof(uint[]))
                {
                    ((uint[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = Convert.ToUInt32(newVal);
                }
                else if (field.FieldType == typeof(ushort[]))
                {
                    ((ushort[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = Convert.ToUInt16(newVal);
                }
                else if (field.FieldType == typeof(short[]))
                {
                    ((short[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = Convert.ToInt16(newVal);
                }
                else if (field.FieldType == typeof(ulong[]))
                {
                    ((ulong[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = Convert.ToUInt64(newVal);
                }
                else if (field.FieldType == typeof(long[]))
                {
                    ((long[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = Convert.ToInt64(newVal);
                }
                else if (field.FieldType == typeof(IntPtr))
                {
                    ((IntPtr[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = new IntPtr(Convert.ToUInt32(newVal));
                }
                else if (field.FieldType == typeof(UIntPtr))
                {
                    ((UIntPtr[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = new UIntPtr(Convert.ToUInt32(newVal));
                }
                else if (field.FieldType == typeof(string[]))
                {
                    ((string[])field.GetValue(dataSection.Variables[ModifiyValue.Address].Value))[Index] = newVal.ToString();
                }
                else if (field.FieldType == typeof(string))
                {
                    char[] chrStr = ((string)field.GetValue(dataSection.Variables[ModifiyValue.Address].Value)).ToCharArray();
                    chrStr[Index] = Char.ConvertFromUtf32(Convert.ToInt32(newVal))[0];
                    dataSection.Variables[ModifiyValue.Address].Value = new String(chrStr);
                }
            }
            else
            {
                Type type = dataSection.Variables[ModifiyValue.Address].Value.GetType();

                if (type == typeof(byte[]))
                {
                    ((byte[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = Convert.ToByte(newVal);
                }
                else if (type == typeof(int[]))
                {
                    ((int[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = Convert.ToInt32(newVal);
                }
                else if (type == typeof(uint[]))
                {
                    ((uint[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = Convert.ToUInt32(newVal);
                }
                else if (type == typeof(ushort[]))
                {
                    ((ushort[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = Convert.ToUInt16(newVal);
                }
                else if (type == typeof(short[]))
                {
                    ((short[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = Convert.ToInt16(newVal);
                }
                else if (type == typeof(ulong[]))
                {
                    ((ulong[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = Convert.ToUInt64(newVal);
                }
                else if (type == typeof(long[]))
                {
                    ((long[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = Convert.ToInt64(newVal);
                }
                else if (type == typeof(IntPtr))
                {
                    ((IntPtr[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = new IntPtr(Convert.ToUInt32(newVal));
                }
                else if (type == typeof(UIntPtr))
                {
                    ((UIntPtr[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = new UIntPtr(Convert.ToUInt32(newVal));
                }
                else if (type == typeof(string[]))
                {
                    ((string[])dataSection.Variables[ModifiyValue.Address].Value)[Index] = newVal.ToString();
                }
                else if (type == typeof(string))
                {
                    char[] chrStr = ((string)dataSection.Variables[ModifiyValue.Address].Value).ToCharArray();
                    chrStr[Index] = Char.ConvertFromUtf32(Convert.ToInt32(newVal))[0];
                    dataSection.Variables[ModifiyValue.Address].Value = new String(chrStr);
                }
            }
        }
    }
}