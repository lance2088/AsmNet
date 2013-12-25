using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Asm.Net.src.Sections;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Utils;

namespace Asm.Net.src.Opcodes
{
    public class CALL : Instruction, ICall
    {
        public int FuncAddress { get; set; }
        public Libraries lib = Libraries.User32;
        public Functions func = Functions.User32_MessageBoxA;

        public CALL(int FunctionAddress)
            : base(5, typeof(ICall))
        {
            FuncAddress = FunctionAddress;
        }
        public CALL(int FunctionAddress, Libraries lib, Functions func)
            : base(5, typeof(ICall))
        {
            FuncAddress = FunctionAddress;
            this.lib = lib;
            this.func = func;
        }

        public override byte[] ToByteArray()
        {
            List<byte> data = new List<byte>();
            data.Add((byte)OpcodeList.CALL);
            data.AddRange(BitConverter.GetBytes(FuncAddress));
            return data.ToArray();
        }

        public override void Dispose()
        {
        }

        public override string ToString()
        {
            return "CALL 0x" + FuncAddress.ToString("X6") + "::" + lib.ToString() + "->" + func.ToString().Substring(lib.ToString().Length + 1);
        }

        public void CallFunction(List<object> Stack, Registers registers)
        {
            Delegate FuncDelegate = Prototypes.GetDelegate(lib, func);

            if (FuncDelegate != null)
            {
                object[] tmp = Stack.ToArray();
                Array.Reverse(tmp);

                List<Type> Types = new List<Type>();
                List<object> ReversedStack = new List<object>(tmp);
                tmp = null;

                ParameterInfo[] param = FuncDelegate.Method.GetParameters();

                if (param.Length != ReversedStack.Count)
                    throw new Exception("Parameter count does not match");

                List<IntPtr> Allocations = new List<IntPtr>();

                for(int i = 0; i < param.Length; i++)
                {
                    Types.Add(ReversedStack[i].GetType());

                    if (param[i].ParameterType != ReversedStack[i].GetType())
                    {
                        //ReversedStack[i] = EasyConvert.Convert<IntPtr>(ReversedStack[i]);

                        unsafe
                        {
                            if (ReversedStack[i].GetType() == typeof(byte[]))
                                fixed (byte* p = (byte[])ReversedStack[i]) ReversedStack[i] = new IntPtr(p);
                            else if (ReversedStack[i].GetType() == typeof(short[]))
                                fixed (short* p = (short[])ReversedStack[i]) ReversedStack[i] = new IntPtr(p);
                            else if (ReversedStack[i].GetType() == typeof(ushort[]))
                                fixed (ushort* p = (ushort[])ReversedStack[i]) ReversedStack[i] = new IntPtr(p);
                            else if (ReversedStack[i].GetType() == typeof(int[]))
                                fixed (int* p = (int[])ReversedStack[i]) ReversedStack[i] = new IntPtr(p);
                            else if (ReversedStack[i].GetType() == typeof(uint[]))
                                fixed (uint* p = (uint[])ReversedStack[i]) ReversedStack[i] = new IntPtr(p);
                            else if (ReversedStack[i].GetType() == typeof(long[]))
                                fixed (long* p = (long[])ReversedStack[i]) ReversedStack[i] = new IntPtr(p);
                            else if (ReversedStack[i].GetType() == typeof(ulong[]))
                                fixed (ulong* p = (ulong[])ReversedStack[i]) ReversedStack[i] = new IntPtr(p);
                            else if (ReversedStack[i].GetType() == typeof(byte))
                            {
                                if(param[i].ParameterType.ToString().Contains("*"))
                                    ReversedStack[i] = new IntPtr((byte*)((byte)ReversedStack[i]));
                            }
                            else if (ReversedStack[i].GetType() == typeof(short))
                            {
                                if(param[i].ParameterType.ToString().Contains("*"))
                                    ReversedStack[i] = new IntPtr((short*)((short)ReversedStack[i]));
                            }
                            else if (ReversedStack[i].GetType() == typeof(int))
                            {
                                if(param[i].ParameterType.ToString().Contains("*"))
                                    ReversedStack[i] = new IntPtr((int*)((int)ReversedStack[i]));
                            }
                            else if (ReversedStack[i].GetType() == typeof(uint))
                            {
                                if(param[i].ParameterType.ToString().Contains("*"))
                                    ReversedStack[i] = new IntPtr((uint*)((uint)ReversedStack[i]));
                            }
                            else if (ReversedStack[i].GetType() == typeof(long))
                            {
                                if(param[i].ParameterType.ToString().Contains("*"))
                                    ReversedStack[i] = new IntPtr((long*)((long)ReversedStack[i]));
                            }
                            else if (ReversedStack[i].GetType() == typeof(ulong))
                            {
                                if(param[i].ParameterType.ToString().Contains("*"))
                                    ReversedStack[i] = new IntPtr((ulong*)((ulong)ReversedStack[i]));
                            }
                            else
                            {
                                //throw new Exception("CALL could not be executed because this type is not supported yet, " + param[i].ParameterType.ToString());
                            }
                        }
                    }
                }

                Delegate del = Marshal.GetDelegateForFunctionPointer(new IntPtr(FuncAddress), FuncDelegate.GetType());

                if (del.Method.ReturnType == typeof(bool))
                {
                    registers.EAX = (bool)del.DynamicInvoke(ReversedStack.ToArray()) ? 1 : 0;
                }
                else if (del.Method.ReturnType == typeof(byte) || del.Method.ReturnType == typeof(Int16) ||
                         del.Method.ReturnType == typeof(Int32) || del.Method.ReturnType == typeof(Int64) ||
                         del.Method.ReturnType == typeof(UInt16) || del.Method.ReturnType == typeof(UInt32) ||
                         del.Method.ReturnType == typeof(UInt64))
                {
                    registers.EAX = Convert.ToInt32(del.DynamicInvoke(ReversedStack.ToArray()));
                }
                else if (del.Method.ReturnType == typeof(IntPtr))
                {
                    registers.EAX = ((IntPtr)del.DynamicInvoke(ReversedStack.ToArray())).ToInt32();
                }
                else if (del.Method.ReturnType == typeof(UIntPtr))
                {
                    registers.EAX = (int)((UIntPtr)del.DynamicInvoke(ReversedStack.ToArray())).ToUInt32();
                }
                else //more types need to be added here
                {
                    del.DynamicInvoke(ReversedStack.ToArray());
                }
            }
            else
            {
                //Execute function at Address
                throw new Exception("Unable to execute function, Delegate not found for " + lib + "." + func);
            }
            Stack.Clear();
        }
    }
}