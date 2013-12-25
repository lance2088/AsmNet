using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_DWORD_PTR_ESP : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_DWORD_PTR_ESP(VirtualAddress virtualAddress)
            : base(6, typeof(IMov))
        {
            this.ModifiyValue = virtualAddress;
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_DWORD_PTR_REGISTER,
                                (byte)OpcodeList.MOV_DWORD_PTR_ESP,
                                BitConverter.GetBytes(ModifiyValue.Address)[0],
                                BitConverter.GetBytes(ModifiyValue.Address)[1],
                                BitConverter.GetBytes(ModifiyValue.Address)[2],
                                BitConverter.GetBytes(ModifiyValue.Address)[3]
                              };
        }

        public void Execute(Registers registers, Sections.DataSection dataSection)
        {
            if (dataSection.Variables.ContainsKey(ModifiyValue.Address))
            {
                Type type = dataSection.Variables[ModifiyValue.Address].GetType();
                if (type == typeof(byte))
                    dataSection.Variables[ModifiyValue.Address].Value = Convert.ToByte(registers.ESP);
                else if (type == typeof(short))
                    dataSection.Variables[ModifiyValue.Address].Value = Convert.ToInt16(registers.ESP);
                else if (type == typeof(ushort))
                    dataSection.Variables[ModifiyValue.Address].Value = Convert.ToUInt16(registers.ESP);
                else if (type == typeof(int))
                    dataSection.Variables[ModifiyValue.Address].Value = Convert.ToInt32(registers.ESP);
                else if (type == typeof(uint))
                    dataSection.Variables[ModifiyValue.Address].Value = Convert.ToUInt32(registers.ESP);
                else if (type == typeof(long))
                    dataSection.Variables[ModifiyValue.Address].Value = Convert.ToInt64(registers.ESP);
                else if (type == typeof(ulong))
                    dataSection.Variables[ModifiyValue.Address].Value = Convert.ToUInt64(registers.ESP);
                else if (type == typeof(IntPtr))
                    dataSection.Variables[ModifiyValue.Address].Value = new IntPtr(registers.ESP);
                else if (type == typeof(UIntPtr))
                    dataSection.Variables[ModifiyValue.Address].Value = new UIntPtr(Convert.ToUInt32(registers.ESP));
                else
                    throw new Exception("Variable type \"" + type + "\" not supported at " + ToString());
                return;
            }

            try
            {
                unsafe
                {
                    switch (ModifiyValue.Size)
                    {
                        case 4:
                        {
                            *(int*)(ModifiyValue.Address) = registers.ESP;
                            break;
                        }
                        //x64
                        /*case 8:
                        {
                            registers.RSP = *(long*)(ModifiyAddress.Address);
                            break;
                        }*/
                        default:
                        {
                            throw new NotSupportedException(ToString());
                        }
                    }
                }
            } catch { throw new Exception("\"" + ToString() + "\"Invalid memory address"); }
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV DWORD PTR [0x" + ModifiyValue.Address.ToString("X6") + "], ESP";
        }
    }
}