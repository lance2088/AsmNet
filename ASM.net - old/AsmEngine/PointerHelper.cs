using System;
using System.Collections.Generic;
using System.Text;

namespace AsmEngine
{
    internal unsafe class PointerHelper
    {
        public static byte PointerToByte(void* pointer)
        {
            return (byte)pointer;
        }
        public static Int16 PointerToInt16(void* pointer)
        {
            return (short)pointer;
        }
        public static Int32 PointerToInt32(void* pointer)
        {
            return (int)pointer;
        }
        public static Int64 PointerToInt64(void* pointer)
        {
            return (long)pointer;
        }
        public static void* VarToPointer(void* value)
        {
            return value;
        }

        public static string PointerToString(void* pointer)
        {
            return ((int)pointer).ToString();
        }
    }
}