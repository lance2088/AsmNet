using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src
{
    //just added some of the opcodes.. this are not all of them, still need to reverse them >:3
    public enum OpcodeList : byte
    {
        ADD = 0,
        MOV_EAX_DWORD_PTR = 0xA1,

        
        MOV_DWORD_PTR_REGISTER = 0x89,
        MOV_DWORD_PTR_EAX = 0xA3,
        MOV_DWORD_PTR_EDX = 0x15,//8915 D8 mov dword ptr [address], edx
        MOV_DWORD_PTR_EBP = 0x2D,//892D D8 mov dword ptr [address], ebp
        MOV_DWORD_PTR_EBX = 0x1D,//891D D8 mov dword ptr [address], ebx
        MOV_DWORD_PTR_ECX = 0x0D,//890D D8 mov dword ptr [address], ecx
        MOV_DWORD_PTR_EDI = 0x3D,//893D D8 mov dword ptr [address], edi
        MOV_DWORD_PTR_ESI = 0x35,//8935 D8 mov dword ptr [address], esi
        MOV_DWORD_PTR_ESP = 0x25,//8925 D8 mov dword ptr [address], esp

        AND_REGISTER = 0x21,
        XOR_REGISTER = 0x33,
        CMP_REGISTER = 0x3B,
        MOV_REGISTER = 0x8B,

        INC_EAX = 0X40,
        INC_ECX = 0X41,
        INC_EDX = 0X42,
        INC_EBX = 0X43,
        INC_ESP = 0X44,
        INC_EBP = 0X45,
        INC_ESI = 0X46,
        INC_EDI = 0X47,

        PUSH_EAX = 0x50,
        PUSH_ECX = 0x51,
        PUSH_EDX = 0x52,
        PUSH_EBX = 0X53,
        PUSH_ESP = 0x54,
        PUSH_EBP = 0x55,
        PUSH_ESI = 0X56,
        PUSH_EDI = 0X57,

        //I need a confirm here...
        //0x68 Goes from 0-255 and after that it goes to 0x6A
        //in masm32 0x68 are values till 255 but variables+numbers can be both 0x6A... umm
        PUSH_VARIABLE = 0x68,
        PUSH_VALUE = 0x6A,
        NOP = 0x90,

        MOV_EAX = 0xB8,
        MOV_ECX = 0xB9,
        MOV_EDX = 0xBA,
        MOV_EBX = 0xBB,
        MOV_ESP = 0xBC,
        MOV_EBP = 0xBD,
        MOV_ESI = 0xBE,
        MOV_EDI = 0xBF,

        RET = 0xC3,

        //Jumps
        JE = 0x74,
        JNZ = 0x75,
        JNE = 0x75,
        JB_one = 0x0F,
        JB_two = 0x82,

        CALL = 0xE8,
        JMP = 0xE9,
        MUL = 0xF7,

        Variable = 0x00FF, //only used in Asm.Net, invalid assembler opcode
    };

    public enum Register
    {
        EAX,
        EBP,
        EBX,
        ECX,
        EDI,
        EDX,
        ESI,
        ESP
    }

    public class OtherOpcodes
    {
        //I'm not so sure about
        //masm32: MOV VARIABLE.VARNAME, VALUE (0x66, 0xC7, 0x05)
        //OllyDbg: mov word ptr [40303A], 2
        //I'll just leave it here...
        public static readonly byte[] MOV_VARIABLE_VALUE = new byte[] { 0x66, 0xC7, 0x05 };
        public static readonly byte[] MOV_DWORD_PTR_VALUE = new byte[] { 0xC7, 0x05 }; //mov dword ptr [address], value
        public static readonly byte[] MOV_VARIABLE_INDEX_VALUE = new byte[] { 0x66, 0xC7, 0x45 }; //66:C745 C0 0000    mov word ptr [address], 0 (set value at index)
        


        public static readonly byte[] MOV_EAX_DWORD_PTR = new byte[] { 0x8B, 0x45 }; //8B45 D8 mov eax, dword ptr [ebp-28]
        public static readonly byte[] MOV_ECX_DWORD_PTR = new byte[] { 0x8B, 0x4D }; //8B4D D8 mov ecx, dword ptr [ebp-28]
        public static readonly byte[] MOV_EBP_DWORD_PTR = new byte[] { 0x8B, 0x6D }; //8B6D D8 mov ebp, dword ptr [ebp-28]
        public static readonly byte[] MOV_EBX_DWORD_PTR = new byte[] { 0x8B, 0x5D }; //8B5D D8 mov ebx, dword ptr [ebp-28]
        public static readonly byte[] MOV_EDI_DWORD_PTR = new byte[] { 0x8B, 0x7D }; //8B7D D8 mov edi, dword ptr [ebp-28]
        public static readonly byte[] MOV_EDX_DWORD_PTR = new byte[] { 0x8B, 0x55 }; //8B55 D8 mov edx, dword ptr [ebp-28]
        public static readonly byte[] MOV_ESI_DWORD_PTR = new byte[] { 0x8B, 0x75 }; //8B75 D8 mov esi, dword ptr [ebp-28]
        public static readonly byte[] MOV_ESP_DWORD_PTR = new byte[] { 0x8B, 0x65 }; //8B65 D8 mov esp, dword ptr [ebp-28]
    }

    public enum XorRegisterOpcodes
    {
        XOR_EAX_EAX = 0xC0,
        XOR_EAX_ECX = 0xC1,
        XOR_EAX_EDX = 0xC2,
        XOR_EAX_EBX = 0xC3,
        XOR_EAX_ESP = 0xC4,
        XOR_EAX_EBP = 0xC5,
        XOR_EAX_ESI = 0xC6,
        XOR_EAX_EDI = 0xC7,

        XOR_ECX_EAX = 0xC8,
        XOR_ECX_ECX = 0xC9,
        XOR_ECX_EDX = 0xCA,
        XOR_ECX_EBX = 0xCB,
        XOR_ECX_ESP = 0xCC,
        XOR_ECX_EBP = 0xCD,
        XOR_ECX_ESI = 0xCE,
        XOR_ECX_EDI = 0xCF,

        XOR_EDX_EAX = 0xD0,
        XOR_EDX_ECX = 0xD1,
        XOR_EDX_EDX = 0xD2,
        XOR_EDX_EBX = 0xD3,
        XOR_EDX_ESP = 0xD4,
        XOR_EDX_EBP = 0xD5,
        XOR_EDX_ESI = 0xD6,
        XOR_EDX_EDI = 0xD7,

        XOR_EBX_EAX = 0xD8,
        XOR_EBX_ECX = 0xD9,
        XOR_EBX_EDX = 0xDA,
        XOR_EBX_EBX = 0xDB,
        XOR_EBX_ESP = 0xDC,
        XOR_EBX_EBP = 0xDD,
        XOR_EBX_ESI = 0xDE,
        XOR_EBX_EDI = 0xDF,

        XOR_ESP_EAX = 0xE0,
        XOR_ESP_ECX = 0xE1,
        XOR_ESP_EDX = 0xE2,
        XOR_ESP_EBX = 0xE3,
        XOR_ESP_ESP = 0xE4,
        XOR_ESP_EBP = 0xE5,
        XOR_ESP_ESI = 0xE6,
        XOR_ESP_EDI = 0xE7,

        XOR_EBP_EAX = 0xE8,
        XOR_EBP_ECX = 0xE9,
        XOR_EBP_EDX = 0xEA,
        XOR_EBP_EBX = 0xEB,
        XOR_EBP_ESP = 0xEC,
        XOR_EBP_EBP = 0xED,
        XOR_EBP_ESI = 0xEE,
        XOR_EBP_EDI = 0xEF,

        XOR_ESI_EAX = 0xF0,
        XOR_ESI_ECX = 0xF1,
        XOR_ESI_EDX = 0xF2,
        XOR_ESI_EBX = 0xF3,
        XOR_ESI_ESP = 0xF4,
        XOR_ESI_EBP = 0xF5,
        XOR_ESI_ESI = 0xF6,
        XOR_ESI_EDI = 0xF7,

        XOR_EDI_EAX = 0xF8,
        XOR_EDI_ECX = 0xF9,
        XOR_EDI_EDX = 0xFA,
        XOR_EDI_EBX = 0xFB,
        XOR_EDI_ESP = 0xFC,
        XOR_EDI_EBP = 0xFD,
        XOR_EDI_ESI = 0xFE,
        XOR_EDI_EDI = 0xFF,
    };
    public enum AndRegisterOpcodes
    {
        AND_EAX_EAX = 0xC0,
        AND_EAX_ECX = 0xC8,
        AND_EAX_EDX = 0xD0,
        AND_EAX_EBX = 0xD8,
        AND_EAX_ESP = 0xE0,
        AND_EAX_EBP = 0xE8,
        AND_EAX_ESI = 0xF0,
        AND_EAX_EDI = 0xF8,

        AND_ECX_EAX = 0xC1,
        AND_ECX_ECX = 0xC9,
        AND_ECX_EDX = 0xD1,
        AND_ECX_EBX = 0xD9,
        AND_ECX_ESP = 0xE1,
        AND_ECX_EBP = 0xE9,
        AND_ECX_ESI = 0xF1,
        AND_ECX_EDI = 0xF9,

        AND_EDX_EAX = 0xC2,
        AND_EDX_ECX = 0xCA,
        AND_EDX_EDX = 0xD2,
        AND_EDX_EBX = 0xDA,
        AND_EDX_ESP = 0xE2,
        AND_EDX_EBP = 0xEA,
        AND_EDX_ESI = 0xF2,
        AND_EDX_EDI = 0xFA,

        AND_EBX_EAX = 0xC3,
        AND_EBX_ECX = 0xCB,
        AND_EBX_EDX = 0xD3,
        AND_EBX_EBX = 0xDB,
        AND_EBX_ESP = 0xE3,
        AND_EBX_EBP = 0xEB,
        AND_EBX_ESI = 0xF3,
        AND_EBX_EDI = 0xFB,

        AND_ESP_EAX = 0xC4,
        AND_ESP_ECX = 0xCC,
        AND_ESP_EDX = 0xD4,
        AND_ESP_EBX = 0xDC,
        AND_ESP_ESP = 0xE4,
        AND_ESP_EBP = 0xEC,
        AND_ESP_ESI = 0xF4,
        AND_ESP_EDI = 0xFC,

        AND_EBP_EAX = 0xC5,
        AND_EBP_ECX = 0xCD,
        AND_EBP_EDX = 0xD5,
        AND_EBP_EBX = 0xDD,
        AND_EBP_ESP = 0xE5,
        AND_EBP_EBP = 0xED,
        AND_EBP_ESI = 0xF5,
        AND_EBP_EDI = 0xFD,

        AND_ESI_EAX = 0xC6,
        AND_ESI_ECX = 0xCE,
        AND_ESI_EDX = 0xD6,
        AND_ESI_EBX = 0xDE,
        AND_ESI_ESP = 0xE6,
        AND_ESI_EBP = 0xEE,
        AND_ESI_ESI = 0xF6,
        AND_ESI_EDI = 0xFE,

        AND_EDI_EAX = 0xC7,
        AND_EDI_ECX = 0xCF,
        AND_EDI_EDX = 0xD7,
        AND_EDI_EBX = 0xDF,
        AND_EDI_ESP = 0xE7,
        AND_EDI_EBP = 0xEF,
        AND_EDI_ESI = 0xF7,
        AND_EDI_EDI = 0xFF,
    };

    public enum CmpRegisterOpcodes
    {
        CMP_EAX_EAX = 0xC0,
        CMP_EAX_ECX = 0xC1,
        CMP_EAX_EDX = 0xC2,
        CMP_EAX_EBX = 0xC3,
        CMP_EAX_ESP = 0xC4,
        CMP_EAX_EBP = 0xC5,
        CMP_EAX_ESI = 0xC6,
        CMP_EAX_EDI = 0xC7,

        CMP_ECX_EAX = 0xC8,
        CMP_ECX_ECX = 0xC9,
        CMP_ECX_EDX = 0xCA,
        CMP_ECX_EBX = 0xCB,
        CMP_ECX_ESP = 0xCC,
        CMP_ECX_EBP = 0xCD,
        CMP_ECX_ESI = 0xCE,
        CMP_ECX_EDI = 0xCF,

        CMP_EDX_EAX = 0xD0,
        CMP_EDX_ECX = 0xD1,
        CMP_EDX_EDX = 0xD2,
        CMP_EDX_EBX = 0xD3,
        CMP_EDX_ESP = 0xD4,
        CMP_EDX_EBP = 0xD5,
        CMP_EDX_ESI = 0xD6,
        CMP_EDX_EDI = 0xD7,

        CMP_EBX_EAX = 0xD8,
        CMP_EBX_ECX = 0xD9,
        CMP_EBX_EDX = 0xDA,
        CMP_EBX_EBX = 0xDB,
        CMP_EBX_ESP = 0xDC,
        CMP_EBX_EBP = 0xDD,
        CMP_EBX_ESI = 0xDE,
        CMP_EBX_EDI = 0xDF,

        CMP_ESP_EAX = 0xE0,
        CMP_ESP_ECX = 0xE1,
        CMP_ESP_EDX = 0xE2,
        CMP_ESP_EBX = 0xE3,
        CMP_ESP_ESP = 0xE4,
        CMP_ESP_EBP = 0xE5,
        CMP_ESP_ESI = 0xE6,
        CMP_ESP_EDI = 0xE7,

        CMP_EBP_EAX = 0xE8,
        CMP_EBP_ECX = 0xE9,
        CMP_EBP_EDX = 0xEA,
        CMP_EBP_EBX = 0xEB,
        CMP_EBP_ESP = 0xEC,
        CMP_EBP_EBP = 0xED,
        CMP_EBP_ESI = 0xEE,
        CMP_EBP_EDI = 0xEF,

        CMP_ESI_EAX = 0xF0,
        CMP_ESI_ECX = 0xF1,
        CMP_ESI_EDX = 0xF2,
        CMP_ESI_EBX = 0xF3,
        CMP_ESI_ESP = 0xF4,
        CMP_ESI_EBP = 0xF5,
        CMP_ESI_ESI = 0xF6,
        CMP_ESI_EDI = 0xF7,

        CMP_EDI_EAX = 0xF8,
        CMP_EDI_ECX = 0xF9,
        CMP_EDI_EDX = 0xFA,
        CMP_EDI_EBX = 0xFB,
        CMP_EDI_ESP = 0xFC,
        CMP_EDI_EBP = 0xFD,
        CMP_EDI_ESI = 0xFE,
        CMP_EDI_EDI = 0xFF,
    };

    public enum MovRegisterOpcodes
    {
        MOV_EAX_EAX = 0xC0,
        MOV_EAX_ECX = 0xC1,
        MOV_EAX_EDX = 0xC2,
        MOV_EAX_EBX = 0xC3,
        MOV_EAX_ESP = 0xC4,
        MOV_EAX_EBP = 0xC5,
        MOV_EAX_ESI = 0xC6,
        MOV_EAX_EDI = 0xC7,

        MOV_ECX_EAX = 0xC8,
        MOV_ECX_ECX = 0xC9,
        MOV_ECX_EDX = 0xCA,
        MOV_ECX_EBX = 0xCB,
        MOV_ECX_ESP = 0xCC,
        MOV_ECX_EBP = 0xCD,
        MOV_ECX_ESI = 0xCE,
        MOV_ECX_EDI = 0xCF,

        MOV_EDX_EAX = 0xD0,
        MOV_EDX_ECX = 0xD1,
        MOV_EDX_EDX = 0xD2,
        MOV_EDX_EBX = 0xD3,
        MOV_EDX_ESP = 0xD4,
        MOV_EDX_EBP = 0xD5,
        MOV_EDX_ESI = 0xD6,
        MOV_EDX_EDI = 0xD7,

        MOV_EBX_EAX = 0xD8,
        MOV_EBX_ECX = 0xD9,
        MOV_EBX_EDX = 0xDA,
        MOV_EBX_EBX = 0xDB,
        MOV_EBX_ESP = 0xDC,
        MOV_EBX_EBP = 0xDD,
        MOV_EBX_ESI = 0xDE,
        MOV_EBX_EDI = 0xDF,

        MOV_ESP_EAX = 0xE0,
        MOV_ESP_ECX = 0xE1,
        MOV_ESP_EDX = 0xE2,
        MOV_ESP_EBX = 0xE3,
        MOV_ESP_ESP = 0xE4,
        MOV_ESP_EBP = 0xE5,
        MOV_ESP_ESI = 0xE6,
        MOV_ESP_EDI = 0xE7,

        MOV_EBP_EAX = 0xE8,
        MOV_EBP_ECX = 0xE9,
        MOV_EBP_EDX = 0xEA,
        MOV_EBP_EBX = 0xEB,
        MOV_EBP_ESP = 0xEC,
        MOV_EBP_EBP = 0xED,
        MOV_EBP_ESI = 0xEE,
        MOV_EBP_EDI = 0xEF,

        MOV_ESI_EAX = 0xF0,
        MOV_ESI_ECX = 0xF1,
        MOV_ESI_EDX = 0xF2,
        MOV_ESI_EBX = 0xF3,
        MOV_ESI_ESP = 0xF4,
        MOV_ESI_EBP = 0xF5,
        MOV_ESI_ESI = 0xF6,
        MOV_ESI_EDI = 0xF7,

        MOV_EDI_EAX = 0xF8,
        MOV_EDI_ECX = 0xF9,
        MOV_EDI_EDX = 0xFA,
        MOV_EDI_EBX = 0xFB,
        MOV_EDI_ESP = 0xFC,
        MOV_EDI_EBP = 0xFD,
        MOV_EDI_ESI = 0xFE,
        MOV_EDI_EDI = 0xFF,
    };

    public enum MulRegisterOpcodes //custom... why is it so damn hard to do a simple 5*5 ? lol
    {
        MUL_EAX_EAX = 0xC0,
        MUL_EAX_ECX = 0xC1,
        MUL_EAX_EDX = 0xC2,
        MUL_EAX_EBX = 0xC3,
        MUL_EAX_ESP = 0xC4,
        MUL_EAX_EBP = 0xC5,
        MUL_EAX_ESI = 0xC6,
        MUL_EAX_EDI = 0xC7,

        MUL_ECX_EAX = 0xC8,
        MUL_ECX_ECX = 0xC9,
        MUL_ECX_EDX = 0xCA,
        MUL_ECX_EBX = 0xCB,
        MUL_ECX_ESP = 0xCC,
        MUL_ECX_EBP = 0xCD,
        MUL_ECX_ESI = 0xCE,
        MUL_ECX_EDI = 0xCF,

        MUL_EDX_EAX = 0xD0,
        MUL_EDX_ECX = 0xD1,
        MUL_EDX_EDX = 0xD2,
        MUL_EDX_EBX = 0xD3,
        MUL_EDX_ESP = 0xD4,
        MUL_EDX_EBP = 0xD5,
        MUL_EDX_ESI = 0xD6,
        MUL_EDX_EDI = 0xD7,

        MUL_EBX_EAX = 0xD8,
        MUL_EBX_ECX = 0xD9,
        MUL_EBX_EDX = 0xDA,
        MUL_EBX_EBX = 0xDB,
        MUL_EBX_ESP = 0xDC,
        MUL_EBX_EBP = 0xDD,
        MUL_EBX_ESI = 0xDE,
        MUL_EBX_EDI = 0xDF,

        MUL_ESP_EAX = 0xE0,
        MUL_ESP_ECX = 0xE1,
        MUL_ESP_EDX = 0xE2,
        MUL_ESP_EBX = 0xE3,
        MUL_ESP_ESP = 0xE4,
        MUL_ESP_EBP = 0xE5,
        MUL_ESP_ESI = 0xE6,
        MUL_ESP_EDI = 0xE7,

        MUL_EBP_EAX = 0xE8,
        MUL_EBP_ECX = 0xE9,
        MUL_EBP_EDX = 0xEA,
        MUL_EBP_EBX = 0xEB,
        MUL_EBP_ESP = 0xEC,
        MUL_EBP_EBP = 0xED,
        MUL_EBP_ESI = 0xEE,
        MUL_EBP_EDI = 0xEF,

        MUL_ESI_EAX = 0xF0,
        MUL_ESI_ECX = 0xF1,
        MUL_ESI_EDX = 0xF2,
        MUL_ESI_EBX = 0xF3,
        MUL_ESI_ESP = 0xF4,
        MUL_ESI_EBP = 0xF5,
        MUL_ESI_ESI = 0xF6,
        MUL_ESI_EDI = 0xF7,

        MUL_EDI_EAX = 0xF8,
        MUL_EDI_ECX = 0xF9,
        MUL_EDI_EDX = 0xFA,
        MUL_EDI_EBX = 0xFB,
        MUL_EDI_ESP = 0xFC,
        MUL_EDI_EBP = 0xFD,
        MUL_EDI_ESI = 0xFE,
        MUL_EDI_EDI = 0xFF,
    };
}