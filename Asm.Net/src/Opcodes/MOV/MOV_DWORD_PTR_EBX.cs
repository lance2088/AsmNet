using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Interfaces;

namespace Asm.Net.src.Opcodes.MOV
{
    public class MOV_DWORD_PTR_EBX : Instruction, IMov
    {
        public VirtualAddress ModifiyValue { get; set; }
        public MOV_DWORD_PTR_EBX(VirtualAddress virtualAddress)
            : base(6, typeof(IMov))
        {
            this.ModifiyValue = virtualAddress;
        }

        public override byte[] ToByteArray()
        {
            return new byte[] { (byte)OpcodeList.MOV_DWORD_PTR_REGISTER,
                                (byte)OpcodeList.MOV_DWORD_PTR_EBX,
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
                if (dataSection.Variables.ContainsKey(ModifiyValue.Address))
                {
                    Type type = dataSection.Variables[ModifiyValue.Address].GetType();
                    if (type == typeof(byte))
                        dataSection.Variables[ModifiyValue.Address].Value = Convert.ToByte(registers.EBX);
                    else if (type == typeof(short))
                        dataSection.Variables[ModifiyValue.Address].Value = Convert.ToInt16(registers.EBX);
                    else if (type == typeof(ushort))
                        dataSection.Variables[ModifiyValue.Address].Value = Convert.ToUInt16(registers.EBX);
                    else if (type == typeof(int))
                        dataSection.Variables[ModifiyValue.Address].Value = Convert.ToInt32(registers.EBX);
                    else if (type == typeof(uint))
                        dataSection.Variables[ModifiyValue.Address].Value = Convert.ToUInt32(registers.EBX);
                    else if (type == typeof(long))
                        dataSection.Variables[ModifiyValue.Address].Value = Convert.ToInt64(registers.EBX);
                    else if (type == typeof(ulong))
                        dataSection.Variables[ModifiyValue.Address].Value = Convert.ToUInt64(registers.EBX);
                    else if (type == typeof(IntPtr))
                        dataSection.Variables[ModifiyValue.Address].Value = new IntPtr(registers.EBX);
                    else if (type == typeof(UIntPtr))
                        dataSection.Variables[ModifiyValue.Address].Value = new UIntPtr(Convert.ToUInt32(registers.EBX));
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
                                *(int*)(ModifiyValue.Address) = registers.EBX;
                                break;
                            }
                            //x64
                            /*case 8:
                            {
                                registers.RBX = *(long*)(ModifiyAddress.Address);
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
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "MOV DWORD PTR [0x" + ModifiyValue.Address.ToString("X6") + "], EBX";
        }
    }
}