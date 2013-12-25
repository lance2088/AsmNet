using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Asm.Net.src.Opcodes;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Opcodes.INC;
using Asm.Net.src.Opcodes.MOV;
using Asm.Net.src.Opcodes.Jumps;
using Asm.Net.src.Opcodes.XOR;
using Asm.Net.src.Opcodes.CMP;
using Asm.Net.src.Opcodes.PUSH;
using System.Reflection;
using Asm.Net.src.Opcodes.ADD;
using Asm.Net.src.Opcodes.MUL;

namespace Asm.Net.src.Sections
{
    public class CodeSection : Section
    {
        internal SortedList<int, Instruction> Jumps = new SortedList<int, Instruction>();
        internal SortedList<string, int> labels = new SortedList<string, int>();
        internal MemoryStream stream { get; private set; }
        internal DataSection dataSection;
        internal ApiSection apiSection;
        private int MemAddress = 0;

        public CodeSection(DataSection dataSection, ApiSection apiSection)
            : base()
        {
            stream = new MemoryStream();
            Jumps = new SortedList<int, Instruction>();
            labels = new SortedList<string, int>();
            this.dataSection = dataSection;
            this.apiSection = apiSection;
        }

        private CodeSection WriteByte(byte value)
        {
            stream.WriteByte(value);
            return this;
        }

        private CodeSection WriteBytes(byte[] bytes)
        {
            stream.Write(bytes, 0, bytes.Length);
            return this;
        }

        ///<summary> No Operation </summary>
        public CodeSection NOP()
        {
            WriteBytes(new NOP().ToByteArray());
            MemAddress += new NOP().VirtualAddress.Size;
            return this;
        }
        public CodeSection ADD_BYTE_PTR_EAX_AL()
        {
            WriteBytes(new ADD_BYTE_PTR_EAX_AL().ToByteArray());
            MemAddress += new ADD_BYTE_PTR_EAX_AL().VirtualAddress.Size;
            return this;
        }

        #region Jumps
        public CodeSection JMP(int JmpAddress)
        {
            JMP jmp = new JMP(JmpAddress);
            WriteBytes(jmp.ToByteArray());
            MemAddress += jmp.VirtualAddress.Size;
            return this;
        }
        public CodeSection JMP(string label)
        {
            JMP jmp = new JMP(0);
            ((IJump)jmp).Label = label;
            Jumps.Add((int)stream.Length, jmp);
            WriteBytes(jmp.ToByteArray());
            MemAddress += jmp.VirtualAddress.Size;
            return this;
        }

        public CodeSection JE(int JmpAddress)
        {
            WriteBytes(new JE(JmpAddress).ToByteArray());
            MemAddress += new JE(0).VirtualAddress.Size;
            return this;
        }
        public CodeSection JE(string label)
        {
            JE je = new JE(0);
            ((IJump)je).Label = label;
            Jumps.Add((int)stream.Length, je);
            WriteBytes(je.ToByteArray());
            MemAddress += je.VirtualAddress.Size;
            return this;
        }

        public CodeSection JNE(int JmpAddress)
        {
            WriteBytes(new JNE(JmpAddress).ToByteArray());
            MemAddress += new JNE(0).VirtualAddress.Size;
            return this;
        }
        public CodeSection JNE(string label)
        {
            JNE jne = new JNE(0);
            ((IJump)jne).Label = label;
            Jumps.Add((int)stream.Length, jne);
            WriteBytes(jne.ToByteArray());
            MemAddress += new JNE(0).VirtualAddress.Size;
            return this;
        }
        #endregion
        #region INC
        public CodeSection INC_EAX()
        {
            WriteBytes(new INC_EAX().ToByteArray());
            MemAddress += new INC_EAX().VirtualAddress.Size;
            return this;
        }
        public CodeSection INC_EBP()
        {
            WriteBytes(new INC_EBP().ToByteArray());
            MemAddress += new INC_EBP().VirtualAddress.Size;
            return this;
        }
        public CodeSection INC_EBX()
        {
            WriteBytes(new INC_EBX().ToByteArray());
            MemAddress += new INC_EBX().VirtualAddress.Size;
            return this;
        }
        public CodeSection INC_ECX()
        {
            WriteBytes(new INC_ECX().ToByteArray());
            MemAddress += new INC_ECX().VirtualAddress.Size;
            return this;
        }
        public CodeSection INC_EDI()
        {
            WriteBytes(new INC_EDI().ToByteArray());
            MemAddress += new INC_EDI().VirtualAddress.Size;
            return this;
        }
        public CodeSection INC_EDX()
        {
            WriteBytes(new INC_EDX().ToByteArray());
            MemAddress += new INC_EDX().VirtualAddress.Size;
            return this;
        }
        public CodeSection INC_ESI()
        {
            WriteBytes(new INC_ESI().ToByteArray());
            MemAddress += new INC_ESI().VirtualAddress.Size;
            return this;
        }
        public CodeSection INC_ESP()
        {
            WriteBytes(new INC_ESP().ToByteArray());
            MemAddress += new INC_ESP().VirtualAddress.Size;
            return this;
        }
        #endregion
        #region MOV
        public CodeSection MOV_EAX(int value)
        {
            WriteBytes(new MOV_EAX(new VirtualAddress(4, value)).ToByteArray());
            MemAddress += new MOV_EAX(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EAX(VirtualAddress address)
        {
            WriteBytes(new MOV_EAX(address).ToByteArray());
            MemAddress += new MOV_EAX(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EBP(int value)
        {
            WriteBytes(new MOV_EBP(new VirtualAddress(4, value)).ToByteArray());
            MemAddress += new MOV_EBP(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EBP(VirtualAddress address)
        {
            WriteBytes(new MOV_EBP(address).ToByteArray());
            MemAddress += new MOV_EBP(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EBX(int value)
        {
            WriteBytes(new MOV_EBX(new VirtualAddress(4, value)).ToByteArray());
            MemAddress += new MOV_EBX(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EBX(VirtualAddress address)
        {
            WriteBytes(new MOV_EBX(address).ToByteArray());
            MemAddress += new MOV_EBX(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_ECX(int value)
        {
            WriteBytes(new MOV_ECX(new VirtualAddress(4, value)).ToByteArray());
            MemAddress += new MOV_ECX(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_ECX(VirtualAddress address)
        {
            WriteBytes(new MOV_ECX(address).ToByteArray());
            MemAddress += new MOV_ECX(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EDI(int value)
        {
            WriteBytes(new MOV_EDI(new VirtualAddress(4, value)).ToByteArray());
            MemAddress += new MOV_EDI(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EDI(VirtualAddress address)
        {
            WriteBytes(new MOV_EDI(address).ToByteArray());
            MemAddress += new MOV_EDI(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EDX(int value)
        {
            WriteBytes(new MOV_EDX(new VirtualAddress(4, value)).ToByteArray());
            MemAddress += new MOV_EDX(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_EDX(VirtualAddress address)
        {
            WriteBytes(new MOV_EDX(address).ToByteArray());
            MemAddress += new MOV_EDX(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_ESI(int value)
        {
            WriteBytes(new MOV_ESI(new VirtualAddress(4, value)).ToByteArray());
            MemAddress += new MOV_ESI(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_ESI(VirtualAddress address)
        {
            WriteBytes(new MOV_ESI(address).ToByteArray());
            MemAddress += new MOV_ESI(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_ESP(int value)
        {
            WriteBytes(new MOV_ESP(new VirtualAddress(4, value)).ToByteArray());
            MemAddress += new MOV_ESP(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_ESP(VirtualAddress address)
        {
            WriteBytes(new MOV_ESP(address).ToByteArray());
            MemAddress += new MOV_ESP(new VirtualAddress(0, 0)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_DWORD_PTR_EAX(VirtualAddress addr)
        {
            WriteBytes(new MOV_DWORD_PTR_EAX(new VirtualAddress(4, addr.Address)).ToByteArray());
            MemAddress += new MOV_DWORD_PTR_EAX(new VirtualAddress(0, addr.Address)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_DWORD_PTR_EBP(VirtualAddress addr)
        {
            WriteBytes(new MOV_DWORD_PTR_EBP(new VirtualAddress(4, addr.Address)).ToByteArray());
            MemAddress += new MOV_DWORD_PTR_EBP(new VirtualAddress(0, addr.Address)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_DWORD_PTR_EBX(VirtualAddress addr)
        {
            WriteBytes(new MOV_DWORD_PTR_EBX(new VirtualAddress(4, addr.Address)).ToByteArray());
            MemAddress += new MOV_DWORD_PTR_EBX(new VirtualAddress(0, addr.Address)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_DWORD_PTR_ECX(VirtualAddress addr)
        {
            WriteBytes(new MOV_DWORD_PTR_ECX(new VirtualAddress(4, addr.Address)).ToByteArray());
            MemAddress += new MOV_DWORD_PTR_ECX(new VirtualAddress(0, addr.Address)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_DWORD_PTR_EDI(VirtualAddress addr)
        {
            WriteBytes(new MOV_DWORD_PTR_EDI(new VirtualAddress(4, addr.Address)).ToByteArray());
            MemAddress += new MOV_DWORD_PTR_EDI(new VirtualAddress(0, addr.Address)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_DWORD_PTR_EDX(VirtualAddress addr)
        {
            WriteBytes(new MOV_DWORD_PTR_EDX(new VirtualAddress(4, addr.Address)).ToByteArray());
            MemAddress += new MOV_DWORD_PTR_EDX(new VirtualAddress(0, addr.Address)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_DWORD_PTR_ESI(VirtualAddress addr)
        {
            WriteBytes(new MOV_DWORD_PTR_ESI(new VirtualAddress(4, addr.Address)).ToByteArray());
            MemAddress += new MOV_DWORD_PTR_ESI(new VirtualAddress(0, addr.Address)).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_DWORD_PTR_ESP(VirtualAddress addr)
        {
            WriteBytes(new MOV_DWORD_PTR_ESP(new VirtualAddress(4, addr.Address)).ToByteArray());
            MemAddress += new MOV_DWORD_PTR_ESP(new VirtualAddress(0, addr.Address)).VirtualAddress.Size;
            return this;
        }

        ///<summary> Make sure the newValue is Serializable </summary>
        public CodeSection MOV_VARIABLE_VALUE(VirtualAddress VariableAddress, string VariableName, object newValue)
        {
            WriteBytes(new MOV_VARIABLE_VALUE(VariableAddress, VariableName, newValue).ToByteArray());
            MemAddress += new MOV_VARIABLE_VALUE(VariableAddress, VariableName, newValue).VirtualAddress.Size;
            return this;
        }

        ///<summary> Make sure the newValue is Serializable </summary>
        public CodeSection MOV_VARIABLE_INDEX_VALUE(VirtualAddress VariableAddress, string VariableName, int index, object newValue)
        {
            WriteBytes(new MOV_VARIABLE_INDEX_VALUE(VariableAddress, VariableName, index, newValue).ToByteArray());
            MemAddress += new MOV_VARIABLE_INDEX_VALUE(VariableAddress, VariableName, index, newValue).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_VARIABLE_INDEX_REGISTER(VirtualAddress VariableAddress, string VariableName, int index, Register register)
        {
            WriteBytes(new MOV_VARIABLE_INDEX_VALUE(VariableAddress, VariableName, index, register).ToByteArray());
            MemAddress += new MOV_VARIABLE_INDEX_VALUE(VariableAddress, VariableName, index, register).VirtualAddress.Size;
            return this;
        }
        ///<summary> Make sure the newValue is Serializable </summary>
        public CodeSection MOV_VARIABLE_INDEX_VALUE(VirtualAddress VariableAddress, int index, object newValue)
        {
            WriteBytes(new MOV_VARIABLE_INDEX_VALUE(VariableAddress, index, newValue).ToByteArray());
            MemAddress += new MOV_VARIABLE_INDEX_VALUE(VariableAddress, index, newValue).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_VARIABLE_INDEX_REGISTER(VirtualAddress VariableAddress, int index, Register register)
        {
            WriteBytes(new MOV_VARIABLE_INDEX_VALUE(VariableAddress, index, register).ToByteArray());
            MemAddress += new MOV_VARIABLE_INDEX_VALUE(VariableAddress, index, register).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_REGISTER_DWORD_PTR(Register register, VirtualAddress VariableAddress)
        {
            WriteBytes(new MOV_REGISTER_DWORD_PTR(register, VariableAddress).ToByteArray());
            MemAddress += new MOV_REGISTER_DWORD_PTR(register, VariableAddress).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_REGISTER_VARIABLE_INDEX(Register register, VirtualAddress VariableAddress, string VariableName, int index)
        {
            WriteBytes(new MOV_REGISTER_VARIABLE_INDEX(VariableAddress, VariableName, index, register).ToByteArray());
            MemAddress += new MOV_REGISTER_VARIABLE_INDEX(VariableAddress, VariableName, index, register).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_REGISTER_VARIABLE_INDEX(Register register, VirtualAddress VariableAddress, int index)
        {
            WriteBytes(new MOV_REGISTER_VARIABLE_INDEX(VariableAddress, index, register).ToByteArray());
            MemAddress += new MOV_REGISTER_VARIABLE_INDEX(VariableAddress, index, register).VirtualAddress.Size;
            return this;
        }
        public CodeSection MOV_VARIABLE_REGISTER(VirtualAddress VariableAddress, string VariableName, Register register)
        {
            WriteBytes(new MOV_VARIABLE_VALUE(VariableAddress, VariableName, register).ToByteArray());
            MemAddress += new MOV_VARIABLE_VALUE(VariableAddress, VariableName, register).VirtualAddress.Size;
            return this;
        }

        #endregion
        #region PUSH
        public CodeSection PUSH_EAX()
        {
            WriteBytes(new PUSH_EAX().ToByteArray());
            MemAddress += new PUSH_EAX().VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_EBP()
        {
            WriteBytes(new PUSH_EBP().ToByteArray());
            MemAddress += new PUSH_EBP().VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_EBX()
        {
            WriteBytes(new PUSH_EBX().ToByteArray());
            MemAddress += new PUSH_EBX().VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_ECX()
        {
            WriteBytes(new PUSH_ECX().ToByteArray());
            MemAddress += new PUSH_ECX().VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_EDI()
        {
            WriteBytes(new PUSH_EDI().ToByteArray());
            MemAddress += new PUSH_EDI().VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_EDX()
        {
            WriteBytes(new PUSH_EDX().ToByteArray());
            MemAddress += new PUSH_EDX().VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_ESI()
        {
            WriteBytes(new PUSH_ESI().ToByteArray());
            MemAddress += new PUSH_ESI().VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_ESP()
        {
            WriteBytes(new PUSH_ESP().ToByteArray());
            MemAddress += new PUSH_ESP().VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_STRING(string value) //this is variable
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            VirtualAddress addr = dataSection.CreateVariable(value);
            WriteBytes(new PUSH_VARIABLE(addr, dataSection).ToByteArray());
            MemAddress += new PUSH_VARIABLE(addr, dataSection).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VARIABLE(VirtualAddress VariableAddress)
        {
            WriteBytes(new PUSH_VARIABLE(VariableAddress, dataSection).ToByteArray());
            MemAddress += new PUSH_VARIABLE(VariableAddress, dataSection).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(uint value)
        {
            WriteBytes(new PUSH_VALUE(value).ToByteArray());
            MemAddress += new PUSH_VALUE(value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(int value)
        {
            WriteBytes(new PUSH_VALUE(value).ToByteArray());
            MemAddress += new PUSH_VALUE(value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(short value)
        {
            WriteBytes(new PUSH_VALUE(value).ToByteArray());
            MemAddress += new PUSH_VALUE(value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(ushort value)
        {
            WriteBytes(new PUSH_VALUE(value).ToByteArray());
            MemAddress += new PUSH_VALUE(value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(long value)
        {
            WriteBytes(new PUSH_VALUE(value).ToByteArray());
            MemAddress += new PUSH_VALUE(value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(ulong value)
        {
            WriteBytes(new PUSH_VALUE(value).ToByteArray());
            MemAddress += new PUSH_VALUE(value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(byte value)
        {
            WriteBytes(new PUSH_VALUE(value).ToByteArray());
            MemAddress += new PUSH_VALUE(value).VirtualAddress.Size;
            return this;
        }        
        public CodeSection PUSH_VALUE(ValueCodes value, uint AlwaysZero)
        {
            WriteBytes(new PUSH_VALUE((uint)value).ToByteArray());
            MemAddress += new PUSH_VALUE((uint)value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(ValueCodes value, int AlwaysZero)
        {
            WriteBytes(new PUSH_VALUE((int)value).ToByteArray());
            MemAddress += new PUSH_VALUE((int)value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(ValueCodes value, short AlwaysZero)
        {
            WriteBytes(new PUSH_VALUE((short)value).ToByteArray());
            MemAddress += new PUSH_VALUE((short)value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(ValueCodes value, ushort AlwaysZero)
        {
            WriteBytes(new PUSH_VALUE((ushort)value).ToByteArray());
            MemAddress += new PUSH_VALUE((ushort)value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(ValueCodes value, long AlwaysZero)
        {
            WriteBytes(new PUSH_VALUE((long)value).ToByteArray());
            MemAddress += new PUSH_VALUE((long)value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(ValueCodes value, ulong AlwaysZero)
        {
            WriteBytes(new PUSH_VALUE((ulong)value).ToByteArray());
            MemAddress += new PUSH_VALUE((ulong)value).VirtualAddress.Size;
            return this;
        }
        public CodeSection PUSH_VALUE(ValueCodes value, byte AlwaysZero)
        {
            WriteBytes(new PUSH_VALUE((byte)value).ToByteArray());
            MemAddress += new PUSH_VALUE((byte)value).VirtualAddress.Size;
            return this;
        }

        #endregion

        public CodeSection RET()
        {
            WriteBytes(new RET().ToByteArray());
            MemAddress += new RET().VirtualAddress.Size;
            return this;
        }
        public CodeSection CALL(int FunctionAddress)
        {
            WriteBytes(new CALL(FunctionAddress).ToByteArray());
            MemAddress += new CALL(0).VirtualAddress.Size;
            return this;
        }

        public CodeSection CALL(Functions Function)
        {
            Libraries Library = Libraries.Kernel32;
            if (Function.ToString().ToLower().StartsWith("user32"))
                Library = Libraries.User32;
            else if (Function.ToString().ToLower().StartsWith("ws2_32"))
                Library = Libraries.ws2_32;

            VirtualAddress addr = apiSection.GetApiAddress(Library.ToString() + ".dll", Function.ToString().Substring(Library.ToString().Length + 1));

            if (Prototypes.GetDelegate(Library, Function) == null)
            {
                throw new Exception("Couldn't find the function " + Library + "." + Function);
            }

            WriteBytes(new CALL(addr.Address, Library, Function).ToByteArray());
            MemAddress += new CALL(0).VirtualAddress.Size;
            return this;
        }

        public CodeSection XOR(XorRegisterOpcodes xor)
        {
            switch (xor)
            {
                case XorRegisterOpcodes.XOR_EAX_EAX: { WriteBytes(new XOR_EAX_EAX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EAX_EBP: { WriteBytes(new XOR_EAX_EBP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EAX_EBX: { WriteBytes(new XOR_EAX_EBX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EAX_ECX: { WriteBytes(new XOR_EAX_ECX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EAX_EDI: { WriteBytes(new XOR_EAX_EDI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EAX_EDX: { WriteBytes(new XOR_EAX_EDX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EAX_ESI: { WriteBytes(new XOR_EAX_ESI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EAX_ESP: { WriteBytes(new XOR_EAX_ESP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBP_EAX: { WriteBytes(new XOR_EBP_EAX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBP_EBP: { WriteBytes(new XOR_EBP_EBP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBP_EBX: { WriteBytes(new XOR_EBP_EBX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBP_ECX: { WriteBytes(new XOR_EBP_ECX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBP_EDI: { WriteBytes(new XOR_EBP_EDI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBP_EDX: { WriteBytes(new XOR_EBP_EDX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBP_ESI: { WriteBytes(new XOR_EBP_ESI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBP_ESP: { WriteBytes(new XOR_EBP_ESP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBX_EAX: { WriteBytes(new XOR_EBX_EAX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBX_EBP: { WriteBytes(new XOR_EBX_EBP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBX_EBX: { WriteBytes(new XOR_EBX_EBX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBX_ECX: { WriteBytes(new XOR_EBX_ECX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBX_EDI: { WriteBytes(new XOR_EBX_EDI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBX_EDX: { WriteBytes(new XOR_EBX_EDX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBX_ESI: { WriteBytes(new XOR_EBX_ESI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EBX_ESP: { WriteBytes(new XOR_EBX_ESP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ECX_EAX: { WriteBytes(new XOR_ECX_EAX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ECX_EBP: { WriteBytes(new XOR_ECX_EBP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ECX_EBX: { WriteBytes(new XOR_ECX_EBX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ECX_ECX: { WriteBytes(new XOR_ECX_ECX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ECX_EDI: { WriteBytes(new XOR_ECX_EDI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ECX_EDX: { WriteBytes(new XOR_ECX_EDX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ECX_ESI: { WriteBytes(new XOR_ECX_ESI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ECX_ESP: { WriteBytes(new XOR_ECX_ESP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDI_EAX: { WriteBytes(new XOR_EDI_EAX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDI_EBP: { WriteBytes(new XOR_EDI_EBP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDI_EBX: { WriteBytes(new XOR_EDI_EBX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDI_ECX: { WriteBytes(new XOR_EDI_ECX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDI_EDI: { WriteBytes(new XOR_EDI_EDI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDI_EDX: { WriteBytes(new XOR_EDI_EDX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDI_ESI: { WriteBytes(new XOR_EDI_ESI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDI_ESP: { WriteBytes(new XOR_EDI_ESP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDX_EAX: { WriteBytes(new XOR_EDX_EAX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDX_EBP: { WriteBytes(new XOR_EDX_EBP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDX_EBX: { WriteBytes(new XOR_EDX_EBX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDX_ECX: { WriteBytes(new XOR_EDX_ECX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDX_EDI: { WriteBytes(new XOR_EDX_EDI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDX_EDX: { WriteBytes(new XOR_EDX_EDX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDX_ESI: { WriteBytes(new XOR_EDX_ESI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_EDX_ESP: { WriteBytes(new XOR_EDX_ESP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESI_EAX: { WriteBytes(new XOR_ESI_EAX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESI_EBP: { WriteBytes(new XOR_ESI_EBP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESI_EBX: { WriteBytes(new XOR_ESI_EBX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESI_ECX: { WriteBytes(new XOR_ESI_ECX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESI_EDI: { WriteBytes(new XOR_ESI_EDI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESI_EDX: { WriteBytes(new XOR_ESI_EDX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESI_ESI: { WriteBytes(new XOR_ESI_ESI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESI_ESP: { WriteBytes(new XOR_ESI_ESP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESP_EAX: { WriteBytes(new XOR_ESP_EAX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESP_EBP: { WriteBytes(new XOR_ESP_EBP().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESP_EBX: { WriteBytes(new XOR_ESP_EBX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESP_ECX: { WriteBytes(new XOR_ESP_ECX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESP_EDI: { WriteBytes(new XOR_ESP_EDI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESP_EDX: { WriteBytes(new XOR_ESP_EDX().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESP_ESI: { WriteBytes(new XOR_ESP_ESI().ToByteArray()); MemAddress += 2; break; }
                case XorRegisterOpcodes.XOR_ESP_ESP: { WriteBytes(new XOR_ESP_ESP().ToByteArray()); MemAddress += 2; break; }
            }
            return this;
        }

        public CodeSection CMP(CmpRegisterOpcodes cmp)
        {
            switch (cmp)
            {
                case CmpRegisterOpcodes.CMP_EAX_EAX: { WriteBytes(new CMP_EAX_EAX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EAX_EBP: { WriteBytes(new CMP_EAX_EBP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EAX_EBX: { WriteBytes(new CMP_EAX_EBX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EAX_ECX: { WriteBytes(new CMP_EAX_ECX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EAX_EDI: { WriteBytes(new CMP_EAX_EDI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EAX_EDX: { WriteBytes(new CMP_EAX_EDX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EAX_ESI: { WriteBytes(new CMP_EAX_ESI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EAX_ESP: { WriteBytes(new CMP_EAX_ESP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBP_EAX: { WriteBytes(new CMP_EBP_EAX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBP_EBP: { WriteBytes(new CMP_EBP_EBP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBP_EBX: { WriteBytes(new CMP_EBP_EBX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBP_ECX: { WriteBytes(new CMP_EBP_ECX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBP_EDI: { WriteBytes(new CMP_EBP_EDI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBP_EDX: { WriteBytes(new CMP_EBP_EDX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBP_ESI: { WriteBytes(new CMP_EBP_ESI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBP_ESP: { WriteBytes(new CMP_EBP_ESP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBX_EAX: { WriteBytes(new CMP_EBX_EAX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBX_EBP: { WriteBytes(new CMP_EBX_EBP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBX_EBX: { WriteBytes(new CMP_EBX_EBX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBX_ECX: { WriteBytes(new CMP_EBX_ECX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBX_EDI: { WriteBytes(new CMP_EBX_EDI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBX_EDX: { WriteBytes(new CMP_EBX_EDX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBX_ESI: { WriteBytes(new CMP_EBX_ESI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EBX_ESP: { WriteBytes(new CMP_EBX_ESP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ECX_EAX: { WriteBytes(new CMP_ECX_EAX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ECX_EBP: { WriteBytes(new CMP_ECX_EBP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ECX_EBX: { WriteBytes(new CMP_ECX_EBX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ECX_ECX: { WriteBytes(new CMP_ECX_ECX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ECX_EDI: { WriteBytes(new CMP_ECX_EDI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ECX_EDX: { WriteBytes(new CMP_ECX_EDX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ECX_ESI: { WriteBytes(new CMP_ECX_ESI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ECX_ESP: { WriteBytes(new CMP_ECX_ESP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDI_EAX: { WriteBytes(new CMP_EDI_EAX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDI_EBP: { WriteBytes(new CMP_EDI_EBP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDI_EBX: { WriteBytes(new CMP_EDI_EBX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDI_ECX: { WriteBytes(new CMP_EDI_ECX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDI_EDI: { WriteBytes(new CMP_EDI_EDI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDI_EDX: { WriteBytes(new CMP_EDI_EDX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDI_ESI: { WriteBytes(new CMP_EDI_ESI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDI_ESP: { WriteBytes(new CMP_EDI_ESP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDX_EAX: { WriteBytes(new CMP_EDX_EAX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDX_EBP: { WriteBytes(new CMP_EDX_EBP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDX_EBX: { WriteBytes(new CMP_EDX_EBX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDX_ECX: { WriteBytes(new CMP_EDX_ECX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDX_EDI: { WriteBytes(new CMP_EDX_EDI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDX_EDX: { WriteBytes(new CMP_EDX_EDX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDX_ESI: { WriteBytes(new CMP_EDX_ESI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_EDX_ESP: { WriteBytes(new CMP_EDX_ESP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESI_EAX: { WriteBytes(new CMP_ESI_EAX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESI_EBP: { WriteBytes(new CMP_ESI_EBP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESI_EBX: { WriteBytes(new CMP_ESI_EBX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESI_ECX: { WriteBytes(new CMP_ESI_ECX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESI_EDI: { WriteBytes(new CMP_ESI_EDI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESI_EDX: { WriteBytes(new CMP_ESI_EDX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESI_ESI: { WriteBytes(new CMP_ESI_ESI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESI_ESP: { WriteBytes(new CMP_ESI_ESP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESP_EAX: { WriteBytes(new CMP_ESP_EAX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESP_EBP: { WriteBytes(new CMP_ESP_EBP().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESP_EBX: { WriteBytes(new CMP_ESP_EBX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESP_ECX: { WriteBytes(new CMP_ESP_ECX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESP_EDI: { WriteBytes(new CMP_ESP_EDI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESP_EDX: { WriteBytes(new CMP_ESP_EDX().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESP_ESI: { WriteBytes(new CMP_ESP_ESI().ToByteArray()); MemAddress += 2; break; }
                case CmpRegisterOpcodes.CMP_ESP_ESP: { WriteBytes(new CMP_ESP_ESP().ToByteArray()); MemAddress += 2; break; }
            }
            return this;
        }

        public CodeSection MOV(MovRegisterOpcodes mov)
        {
            switch (mov)
            {
                case MovRegisterOpcodes.MOV_EAX_EAX: { WriteBytes(new MOV_EAX_EAX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EAX_EBP: { WriteBytes(new MOV_EAX_EBP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EAX_EBX: { WriteBytes(new MOV_EAX_EBX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EAX_ECX: { WriteBytes(new MOV_EAX_ECX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EAX_EDI: { WriteBytes(new MOV_EAX_EDI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EAX_EDX: { WriteBytes(new MOV_EAX_EDX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EAX_ESI: { WriteBytes(new MOV_EAX_ESI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EAX_ESP: { WriteBytes(new MOV_EAX_ESP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBP_EAX: { WriteBytes(new MOV_EBP_EAX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBP_EBP: { WriteBytes(new MOV_EBP_EBP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBP_EBX: { WriteBytes(new MOV_EBP_EBX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBP_ECX: { WriteBytes(new MOV_EBP_ECX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBP_EDI: { WriteBytes(new MOV_EBP_EDI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBP_EDX: { WriteBytes(new MOV_EBP_EDX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBP_ESI: { WriteBytes(new MOV_EBP_ESI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBP_ESP: { WriteBytes(new MOV_EBP_ESP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBX_EAX: { WriteBytes(new MOV_EBX_EAX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBX_EBP: { WriteBytes(new MOV_EBX_EBP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBX_EBX: { WriteBytes(new MOV_EBX_EBX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBX_ECX: { WriteBytes(new MOV_EBX_ECX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBX_EDI: { WriteBytes(new MOV_EBX_EDI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBX_EDX: { WriteBytes(new MOV_EBX_EDX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBX_ESI: { WriteBytes(new MOV_EBX_ESI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EBX_ESP: { WriteBytes(new MOV_EBX_ESP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ECX_EAX: { WriteBytes(new MOV_ECX_EAX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ECX_EBP: { WriteBytes(new MOV_ECX_EBP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ECX_EBX: { WriteBytes(new MOV_ECX_EBX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ECX_ECX: { WriteBytes(new MOV_ECX_ECX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ECX_EDI: { WriteBytes(new MOV_ECX_EDI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ECX_EDX: { WriteBytes(new MOV_ECX_EDX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ECX_ESI: { WriteBytes(new MOV_ECX_ESI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ECX_ESP: { WriteBytes(new MOV_ECX_ESP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDI_EAX: { WriteBytes(new MOV_EDI_EAX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDI_EBP: { WriteBytes(new MOV_EDI_EBP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDI_EBX: { WriteBytes(new MOV_EDI_EBX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDI_ECX: { WriteBytes(new MOV_EDI_ECX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDI_EDI: { WriteBytes(new MOV_EDI_EDI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDI_EDX: { WriteBytes(new MOV_EDI_EDX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDI_ESI: { WriteBytes(new MOV_EDI_ESI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDI_ESP: { WriteBytes(new MOV_EDI_ESP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDX_EAX: { WriteBytes(new MOV_EDX_EAX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDX_EBP: { WriteBytes(new MOV_EDX_EBP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDX_EBX: { WriteBytes(new MOV_EDX_EBX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDX_ECX: { WriteBytes(new MOV_EDX_ECX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDX_EDI: { WriteBytes(new MOV_EDX_EDI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDX_EDX: { WriteBytes(new MOV_EDX_EDX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDX_ESI: { WriteBytes(new MOV_EDX_ESI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_EDX_ESP: { WriteBytes(new MOV_EDX_ESP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESI_EAX: { WriteBytes(new MOV_ESI_EAX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESI_EBP: { WriteBytes(new MOV_ESI_EBP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESI_EBX: { WriteBytes(new MOV_ESI_EBX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESI_ECX: { WriteBytes(new MOV_ESI_ECX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESI_EDI: { WriteBytes(new MOV_ESI_EDI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESI_EDX: { WriteBytes(new MOV_ESI_EDX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESI_ESI: { WriteBytes(new MOV_ESI_ESI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESI_ESP: { WriteBytes(new MOV_ESI_ESP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESP_EAX: { WriteBytes(new MOV_ESP_EAX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESP_EBP: { WriteBytes(new MOV_ESP_EBP().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESP_EBX: { WriteBytes(new MOV_ESP_EBX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESP_ECX: { WriteBytes(new MOV_ESP_ECX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESP_EDI: { WriteBytes(new MOV_ESP_EDI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESP_EDX: { WriteBytes(new MOV_ESP_EDX().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESP_ESI: { WriteBytes(new MOV_ESP_ESI().ToByteArray()); MemAddress += 2; break; }
                case MovRegisterOpcodes.MOV_ESP_ESP: { WriteBytes(new MOV_ESP_ESP().ToByteArray()); MemAddress += 2; break; }
            }
            return this;
        }
        
        ///<summary> Multiply </summary>
        public CodeSection MUL(MulRegisterOpcodes mul)
        {
            switch (mul)
            {
                case MulRegisterOpcodes.MUL_EAX_EAX: { WriteBytes(new MUL_EAX_EAX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EAX_EBP: { WriteBytes(new MUL_EAX_EBP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EAX_EBX: { WriteBytes(new MUL_EAX_EBX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EAX_ECX: { WriteBytes(new MUL_EAX_ECX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EAX_EDI: { WriteBytes(new MUL_EAX_EDI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EAX_EDX: { WriteBytes(new MUL_EAX_EDX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EAX_ESI: { WriteBytes(new MUL_EAX_ESI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EAX_ESP: { WriteBytes(new MUL_EAX_ESP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBP_EAX: { WriteBytes(new MUL_EBP_EAX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBP_EBP: { WriteBytes(new MUL_EBP_EBP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBP_EBX: { WriteBytes(new MUL_EBP_EBX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBP_ECX: { WriteBytes(new MUL_EBP_ECX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBP_EDI: { WriteBytes(new MUL_EBP_EDI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBP_EDX: { WriteBytes(new MUL_EBP_EDX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBP_ESI: { WriteBytes(new MUL_EBP_ESI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBP_ESP: { WriteBytes(new MUL_EBP_ESP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBX_EAX: { WriteBytes(new MUL_EBX_EAX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBX_EBP: { WriteBytes(new MUL_EBX_EBP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBX_EBX: { WriteBytes(new MUL_EBX_EBX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBX_ECX: { WriteBytes(new MUL_EBX_ECX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBX_EDI: { WriteBytes(new MUL_EBX_EDI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBX_EDX: { WriteBytes(new MUL_EBX_EDX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBX_ESI: { WriteBytes(new MUL_EBX_ESI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EBX_ESP: { WriteBytes(new MUL_EBX_ESP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ECX_EAX: { WriteBytes(new MUL_ECX_EAX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ECX_EBP: { WriteBytes(new MUL_ECX_EBP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ECX_EBX: { WriteBytes(new MUL_ECX_EBX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ECX_ECX: { WriteBytes(new MUL_ECX_ECX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ECX_EDI: { WriteBytes(new MUL_ECX_EDI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ECX_EDX: { WriteBytes(new MUL_ECX_EDX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ECX_ESI: { WriteBytes(new MUL_ECX_ESI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ECX_ESP: { WriteBytes(new MUL_ECX_ESP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDI_EAX: { WriteBytes(new MUL_EDI_EAX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDI_EBP: { WriteBytes(new MUL_EDI_EBP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDI_EBX: { WriteBytes(new MUL_EDI_EBX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDI_ECX: { WriteBytes(new MUL_EDI_ECX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDI_EDI: { WriteBytes(new MUL_EDI_EDI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDI_EDX: { WriteBytes(new MUL_EDI_EDX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDI_ESI: { WriteBytes(new MUL_EDI_ESI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDI_ESP: { WriteBytes(new MUL_EDI_ESP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDX_EAX: { WriteBytes(new MUL_EDX_EAX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDX_EBP: { WriteBytes(new MUL_EDX_EBP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDX_EBX: { WriteBytes(new MUL_EDX_EBX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDX_ECX: { WriteBytes(new MUL_EDX_ECX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDX_EDI: { WriteBytes(new MUL_EDX_EDI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDX_EDX: { WriteBytes(new MUL_EDX_EDX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDX_ESI: { WriteBytes(new MUL_EDX_ESI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_EDX_ESP: { WriteBytes(new MUL_EDX_ESP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESI_EAX: { WriteBytes(new MUL_ESI_EAX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESI_EBP: { WriteBytes(new MUL_ESI_EBP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESI_EBX: { WriteBytes(new MUL_ESI_EBX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESI_ECX: { WriteBytes(new MUL_ESI_ECX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESI_EDI: { WriteBytes(new MUL_ESI_EDI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESI_EDX: { WriteBytes(new MUL_ESI_EDX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESI_ESI: { WriteBytes(new MUL_ESI_ESI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESI_ESP: { WriteBytes(new MUL_ESI_ESP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESP_EAX: { WriteBytes(new MUL_ESP_EAX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESP_EBP: { WriteBytes(new MUL_ESP_EBP().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESP_EBX: { WriteBytes(new MUL_ESP_EBX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESP_ECX: { WriteBytes(new MUL_ESP_ECX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESP_EDI: { WriteBytes(new MUL_ESP_EDI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESP_EDX: { WriteBytes(new MUL_ESP_EDX().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESP_ESI: { WriteBytes(new MUL_ESP_ESI().ToByteArray()); MemAddress += 2; break; }
                case MulRegisterOpcodes.MUL_ESP_ESP: { WriteBytes(new MUL_ESP_ESP().ToByteArray()); MemAddress += 2; break; }
            }
            return this;
        }

        public CodeSection CreateLabel(string label)
        {
            if (label == null)
                throw new Exception("Label cannot be null");
            if (label == "")
                throw new Exception("Label is having a invalid name.");
            if (labels.ContainsKey(label))
                throw new Exception("Label already created.");
            labels.Add(label, MemAddress);
            return this;
        }

        public void FixJumps()
        {
            for (int i = 0; i < Jumps.Count; i++)
            {
                if (labels.ContainsKey(((IJump)Jumps.Values[i]).Label))
                {
                    ((IJump)Jumps.Values[i]).JumpAddress = (labels[((IJump)Jumps.Values[i]).Label] + Options.MemoryBaseAddress);

                    //set new data in the stream
                    stream.Position = Jumps.Keys[i];
                    stream.Write(Jumps.Values[i].ToByteArray(), 0, Jumps.Values[i].VirtualAddress.Size);
                }
                else
                {
                    throw new Exception("Label \"" + ((IJump)Jumps.Values[i]).Label + "\" is not found.");
                }
            }
        }
    }
}