using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_REGISTER_DWORD_PTR : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public Register register { get; private set; }
        public MOV_REGISTER_DWORD_PTR(Register register, VirtualAddress VariableAddr)
            : base(6, typeof(IMov))
        {
            this.register = register;
            this.ModifiyValue = VariableAddr;
        }

        public override byte[] ToByteArray()
        {
            PayloadWriter writer = new PayloadWriter();

            switch (register)
            {
                case Register.EAX: writer.WriteBytes(OtherOpcodes.MOV_EAX_DWORD_PTR); break;
                case Register.ECX: writer.WriteBytes(OtherOpcodes.MOV_ECX_DWORD_PTR); break;
                case Register.EBP: writer.WriteBytes(OtherOpcodes.MOV_EBP_DWORD_PTR); break;
                case Register.EBX: writer.WriteBytes(OtherOpcodes.MOV_EBX_DWORD_PTR); break;
                case Register.EDI: writer.WriteBytes(OtherOpcodes.MOV_EDI_DWORD_PTR); break;
                case Register.EDX: writer.WriteBytes(OtherOpcodes.MOV_EDX_DWORD_PTR); break;
                case Register.ESI: writer.WriteBytes(OtherOpcodes.MOV_ESI_DWORD_PTR); break;
                case Register.ESP: writer.WriteBytes(OtherOpcodes.MOV_ESP_DWORD_PTR); break;
            }
            writer.WriteInteger(ModifiyValue.Address);
            return writer.ToByteArray();
        }

        public override void Dispose()
        {
            
        }

        public override string ToString()
        {
            return "MOV " + register + ", DWORD PTR[" + ModifiyValue.Address.ToString("X6") + "]";
        }

        public void Execute(Registers registers, Sections.DataSection dataSection)
        {
            switch (register)
            {
                case Register.EAX: { registers.EAX = Convert.ToInt32(dataSection.Variables[ModifiyValue.Address].Value); break; }
                case Register.ECX: { registers.ECX = Convert.ToInt32(dataSection.Variables[ModifiyValue.Address].Value); break; }
                case Register.EBP: { registers.EBP = Convert.ToInt32(dataSection.Variables[ModifiyValue.Address].Value); break; }
                case Register.EBX: { registers.EBX = Convert.ToInt32(dataSection.Variables[ModifiyValue.Address].Value); break; }
                case Register.EDI: { registers.EDI = Convert.ToInt32(dataSection.Variables[ModifiyValue.Address].Value); break; }
                case Register.EDX: { registers.EDX = Convert.ToInt32(dataSection.Variables[ModifiyValue.Address].Value); break; }
                case Register.ESI: { registers.ESI = Convert.ToInt32(dataSection.Variables[ModifiyValue.Address].Value); break; }
                case Register.ESP: { registers.ESP = Convert.ToInt32(dataSection.Variables[ModifiyValue.Address].Value); break; }
            }
        }
    }
}