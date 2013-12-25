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
    class MOV_VARIABLE_VALUE : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public string varName {get; private set;}
        public object newValue {get; private set;}
        public Register register { get; private set; }
        internal bool isRegister = false;

        ///<summary> the newValue must be serializable </summary>
        public MOV_VARIABLE_VALUE(VirtualAddress VariableAddress, string varName, object newValue)
            : base(3, typeof(IMov)) //0x66, 0xC7, 0x05
        {
            this.ModifiyValue = VariableAddress;
            this.varName = varName;
            this.newValue = newValue;
        }
        public MOV_VARIABLE_VALUE(VirtualAddress VariableAddress, string varName, Register register)
            : base(3, typeof(IMov)) //0x66, 0xC7, 0x05
        {
            this.ModifiyValue = VariableAddress;
            this.varName = varName;
            this.register = register;
            this.isRegister = true;
        }

        public override byte[] ToByteArray()
        {
            PayloadWriter writer = new PayloadWriter();
            writer.WriteBytes(OtherOpcodes.MOV_VARIABLE_VALUE);
            writer.WriteInteger(ModifiyValue.Address);
            writer.WriteString(varName);
            
            //lets serialize the new value
            writer.WriteByte(isRegister ? (byte)1 : (byte)0);

            if (isRegister)
            {
                writer.WriteByte((byte)register);
            }
            else
            {
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
                return "MOV WORD PTR[" + ModifiyValue.Address.ToString("X6") + "], " + register.ToString();
            return "MOV WORD PTR[" + ModifiyValue.Address.ToString("X6") + "], " + newValue.ToString();
        }

        public void Execute(Registers registers, DataSection dataSection)
        {
            if (dataSection.Variables[ModifiyValue.Address].Value == null)
                throw new Exception("Variable is not initialized, " + ToString());
            if (dataSection.Variables[ModifiyValue.Address].Value.GetType().GetField(varName) == null)
                throw new Exception("Unable to find the variable in the structure/class, " + ToString());

            FieldInfo field = dataSection.Variables[ModifiyValue.Address].Value.GetType().GetField(varName);
            if (isRegister)
            {
                //lets convert to the correct Type
                object value = 0;
                switch (this.register)
                {
                    case Register.EAX: { value = registers.EAX; break; }
                    case Register.EBP: { value = registers.EBP; break; }
                    case Register.EBX: { value = registers.EBX; break; }
                    case Register.ECX: { value = registers.ECX; break; }
                    case Register.EDI: { value = registers.EDI; break; }
                    case Register.EDX: { value = registers.EDX; break; }
                    case Register.ESI: { value = registers.ESI; break; }
                    case Register.ESP: { value = registers.ESP; break; }
                }
                
                if (field.FieldType == typeof(byte))
                    value = (byte)value;
                else if (field.FieldType == typeof(int))
                    value = Convert.ToInt32(value);
                else if (field.FieldType == typeof(uint))
                    value = Convert.ToUInt32(value);
                else if (field.FieldType == typeof(ushort))
                    value = Convert.ToUInt16(value);
                else if (field.FieldType == typeof(short))
                    value = Convert.ToInt16(value);
                else if (field.FieldType == typeof(ulong))
                    value = Convert.ToUInt64(value);
                else if (field.FieldType == typeof(long))
                    value = Convert.ToInt64(value);
                else if (field.FieldType == typeof(IntPtr))
                    value = new IntPtr(Convert.ToInt32(value));
                else if (field.FieldType == typeof(UIntPtr))
                    value = new IntPtr(Convert.ToUInt32(value));

                field.SetValue(dataSection.Variables[ModifiyValue.Address].Value, value);
            }
            else
            {
                field.SetValue(dataSection.Variables[ModifiyValue.Address].Value, newValue);
            }
        }
    }
}