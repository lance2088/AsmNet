using System;
using System.Collections.Generic;
using System.Text;

namespace ASM.net.src
{
    public class RegexPatterns
    {
        public static string Accessor = "(public|private|internal|protected)";
        public static string Keyword = "(namespace|class)";
        public static string String = "\"(.*?)\"";
        public static string Assembler = "(push|nop|mov|add|xor|call|inc|dec|cmp|jz|jmp|jnz)";
        public static string Caster = "(as)";
        public static string DataTypes = "(byte|int16|int32|int64|string|wchar_t|float|double" +
                                          "byte_ptr|int16_ptr|int32_ptr|int64_ptr|float_ptr|double_ptr|" +
                                          "u_byte|u_int16|u_int32|u_int64|u_float|u_double|" +
                                          "u_byte_ptr|u_int16_ptr|u_int32_ptr|u_int64_ptr|u_float_ptr|u_double_ptr|" +
                                          "dword|handle|hwnd|" +
                                          "bool|char|lpcstr|lpcwstr)";
        public static string Registers = "(eax|ebx|ecx|edx|esi|edi|ebp|esp|eip|ah|ex|al|ip|dx|cx|bx|si|bp|sp|ax)";
    }
}