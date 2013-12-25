using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Opcodes;
using Asm.Net.src.Opcodes.AsmNet;
using Asm.Net.src.Opcodes.INC;
using Asm.Net.src.Opcodes.MOV;
using Asm.Net.src.Opcodes.Jumps;
using Asm.Net.src.Opcodes.XOR;
using System.Reflection;
using Asm.Net.src.Opcodes.CMP;
using Asm.Net.src.Opcodes.AND;
using Asm.Net.src.Opcodes.PUSH;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Asm.Net.src.Sections;
using Asm.Net.src.Opcodes.ADD;
using Asm.Net.src.Opcodes.MUL;

namespace Asm.Net.src
{
    public enum OpcodeReaderError
    {
        IncorrectOpcode
    }

    public class OpcodeReader
    {
        public SortedList<int, Instruction> Instructions = new SortedList<int, Instruction>();
        public SortedList<int, Variable> Variables = new SortedList<int, Variable>();
        private byte[] InstructionData;
        private int offset = 0;
        private int MemAddress = Options.MemoryBaseAddress;
        private DataSection dataSection;
        private ApiSection apiSection;

        public OpcodeReader(byte[] data, DataSection dataSection, ApiSection apiSection)
        {
            this.InstructionData = data;
            this.dataSection = dataSection;
            this.apiSection = apiSection;
        }

        public byte[] ToByteArray()
        {
            return InstructionData;
        }

        public Instruction ReadNOP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.NOP)
                throw new Exception("Error reading the NOP instruction, reason: incorrect opcode");
            NOP nop = new NOP();
            nop.VirtualAddress.Address = MemAddress;
            offset += nop.VirtualAddress.Size;
            MemAddress += nop.VirtualAddress.Size;
            Instructions.Add(nop.VirtualAddress.Address, nop);
            return nop;
        }

        public Instruction ReadJMP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.JMP)
                throw new Exception("Error reading the JMP instruction, reason: incorrect opcode");

            JMP jmp = new JMP(BitConverter.ToInt32(InstructionData, offset + 1));
            jmp.VirtualAddress.Address = MemAddress;
            offset += jmp.VirtualAddress.Size;
            MemAddress += jmp.VirtualAddress.Size;
            Instructions.Add(jmp.VirtualAddress.Address, jmp);
            return jmp;
        }

        public Instruction ReadJE()
        {
            if (InstructionData[offset] != (byte)OpcodeList.JE)
                throw new Exception("Error reading the JE instruction, reason: incorrect opcode");

            JE je = new JE(BitConverter.ToInt32(InstructionData, offset + 1));
            je.VirtualAddress.Address = MemAddress;
            offset += je.VirtualAddress.Size;
            MemAddress += je.VirtualAddress.Size;
            Instructions.Add(je.VirtualAddress.Address, je);
            return je;
        }
        
        public Instruction ReadJNE()
        {
            if (InstructionData[offset] != (byte)OpcodeList.JNE)
                throw new Exception("Error reading the JNE instruction, reason: incorrect opcode");

            JNE jne = new JNE(BitConverter.ToInt32(InstructionData, offset + 1));
            jne.VirtualAddress.Address = MemAddress;
            offset += jne.VirtualAddress.Size;
            MemAddress += jne.VirtualAddress.Size;
            Instructions.Add(jne.VirtualAddress.Address, jne);
            return jne;
        }

        public Instruction ReadVariable()
        {
            if (InstructionData[offset] != 0xFF && InstructionData[offset+1] != 0)
                throw new Exception("Error reading the Variable, reason: incorrect opcode");

            int length = BitConverter.ToInt32(InstructionData, offset+2);
            byte[] SerializedData = new byte[length];
            Array.Copy(InstructionData, offset+6, SerializedData, 0, length);
            Variable var = Variable.ByteArrayToVariable(SerializedData);
            offset += var.VirtualAddress.Size;
            MemAddress += var.VirtualAddress.Size;
            Variables.Add(var.VirtualAddress.Address, var);
            return var;
        }

        public Instruction ReadRET()
        {
            if (InstructionData[offset] != (byte)OpcodeList.RET)
                throw new Exception("Error reading the RET instruction, reason: incorrect opcode");
            RET ret = new RET();
            ret.VirtualAddress.Address = MemAddress;
            offset += ret.VirtualAddress.Size;
            MemAddress += ret.VirtualAddress.Size;
            Instructions.Add(ret.VirtualAddress.Address, ret);
            return ret;
        }

        public Instruction ReadCALL()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CALL)
            {
                throw new Exception("Error reading the CALL instruction, reason: incorrect opcode");
            }

            int Address = BitConverter.ToInt32(InstructionData, offset + 1);
            Libraries lib = Libraries.Kernel32;
            Functions func = Functions.Kernel32_ExitProcess;
            Address = apiSection.ResolveAPI(Address, ref lib, ref func).Address;

            CALL call = new CALL(Address, lib, func);
            call.VirtualAddress.Address = MemAddress;
            Instructions.Add(call.VirtualAddress.Address, call);
            offset += call.VirtualAddress.Size;
            MemAddress += call.VirtualAddress.Size;
            return call;
        }

        
        public Instruction ReadPUSH_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_EAX)
                throw new Exception("Error reading the PUSH_EAX instruction, reason: incorrect opcode");
            PUSH_EAX push = new PUSH_EAX();
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadPUSH_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_EBP)
                throw new Exception("Error reading the PUSH_EBP instruction, reason: incorrect opcode");
            PUSH_EBP push = new PUSH_EBP();
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadPUSH_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_EBX)
                throw new Exception("Error reading the PUSH_EBX instruction, reason: incorrect opcode");
            PUSH_EBX push = new PUSH_EBX();
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadPUSH_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_ECX)
                throw new Exception("Error reading the PUSH_ECX instruction, reason: incorrect opcode");
            PUSH_ECX push = new PUSH_ECX();
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadPUSH_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_EDI)
                throw new Exception("Error reading the PUSH_EDI instruction, reason: incorrect opcode");
            PUSH_EDI push = new PUSH_EDI();
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadPUSH_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_EDX)
                throw new Exception("Error reading the PUSH_EDX instruction, reason: incorrect opcode");
            PUSH_EDX push = new PUSH_EDX();
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadPUSH_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_ESI)
                throw new Exception("Error reading the PUSH_ESI instruction, reason: incorrect opcode");
            PUSH_ESI push = new PUSH_ESI();
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadPUSH_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_ESP)
                throw new Exception("Error reading the PUSH_ESP instruction, reason: incorrect opcode");
            PUSH_ESP push = new PUSH_ESP();
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadPUSH_VARIABLE()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_VARIABLE)
                throw new Exception("Error reading the PUSH_VARIABLE instruction, reason: incorrect opcode");

            int addr = BitConverter.ToInt32(InstructionData, offset + 1);
            PUSH_VARIABLE push = new PUSH_VARIABLE(new VirtualAddress(4, addr), dataSection);
            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }
        public Instruction ReadADD_BYTE_PTR_EAX_AL()
        {
            if (InstructionData[offset] != (byte)OpcodeList.ADD)
                throw new Exception("Error reading the ADD_BYTE_PTR_EAX_AL instruction, reason: incorrect opcode");

            ADD_BYTE_PTR_EAX_AL add = new ADD_BYTE_PTR_EAX_AL();
            add.VirtualAddress.Address = MemAddress;
            offset += add.VirtualAddress.Size;
            MemAddress += add.VirtualAddress.Size;
            Instructions.Add(add.VirtualAddress.Address, add);
            return add;
        }
        

        public Instruction ReadPUSH_VALUE()
        {
            if (InstructionData[offset] != (byte)OpcodeList.PUSH_VALUE)
                throw new Exception("Error reading the PUSH_VALUE instruction, reason: incorrect opcode");

            byte type = InstructionData[offset + 1];
            PUSH_VALUE push = null;
            if (type == 0)
                push = new PUSH_VALUE(BitConverter.ToInt32(InstructionData, offset + 2));
            else if (type == 1)
                push = new PUSH_VALUE(BitConverter.ToUInt32(InstructionData, offset + 2));
            else if (type == 2)
                push = new PUSH_VALUE(InstructionData[offset + 2]);
            else if (type == 3)
                push = new PUSH_VALUE(BitConverter.ToInt16(InstructionData, offset + 2));
            else if (type == 4)
                push = new PUSH_VALUE(BitConverter.ToUInt16(InstructionData, offset + 2));
            else if (type == 5)
                push = new PUSH_VALUE(BitConverter.ToUInt64(InstructionData, offset + 2));
            else if (type == 6)
                push = new PUSH_VALUE(BitConverter.ToInt64(InstructionData, offset + 2));
            else
                throw new Exception("Incorrect PUSH value type");

            push.VirtualAddress.Address = MemAddress;
            offset += push.VirtualAddress.Size;
            MemAddress += push.VirtualAddress.Size;
            Instructions.Add(push.VirtualAddress.Address, push);
            return push;
        }

        #region XOR EAX
        public Instruction ReadXOR_EAX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EAX_EAX)
            {
                throw new Exception("Error reading the XOR EAX, EAX instruction, reason: incorrect opcode");
            }

            XOR_EAX_EAX xor = new XOR_EAX_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EAX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EAX_EBP)
            {
                throw new Exception("Error reading the XOR EAX, EBP instruction, reason: incorrect opcode");
            }

            XOR_EAX_EBP xor = new XOR_EAX_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EAX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EAX_EBX)
            {
                throw new Exception("Error reading the XOR EAX, EBX instruction, reason: incorrect opcode");
            }

            XOR_EAX_EBX xor = new XOR_EAX_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EAX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EAX_ECX)
            {
                throw new Exception("Error reading the XOR EAX, ECX instruction, reason: incorrect opcode");
            }

            XOR_EAX_ECX xor = new XOR_EAX_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EAX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EAX_EDI)
            {
                throw new Exception("Error reading the XOR EAX, EDI instruction, reason: incorrect opcode");
            }

            XOR_EAX_EDI xor = new XOR_EAX_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EAX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EAX_EDX)
            {
                throw new Exception("Error reading the XOR EAX, EDX instruction, reason: incorrect opcode");
            }

            XOR_EAX_EDX xor = new XOR_EAX_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EAX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EAX_ESI)
            {
                throw new Exception("Error reading the XOR EAX, ESI instruction, reason: incorrect opcode");
            }

            XOR_EAX_ESI xor = new XOR_EAX_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EAX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EAX_ESP)
            {
                throw new Exception("Error reading the XOR EAX, ESP instruction, reason: incorrect opcode");
            }

            XOR_EAX_ESP xor = new XOR_EAX_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region XOR EBP
        public Instruction ReadXOR_EBP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBP_EAX)
            {
                throw new Exception("Error reading the XOR EBP, EAX instruction, reason: incorrect opcode");
            }

            XOR_EBP_EAX xor = new XOR_EBP_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBP_EBP)
            {
                throw new Exception("Error reading the XOR EBP, EBP instruction, reason: incorrect opcode");
            }

            XOR_EBP_EBP xor = new XOR_EBP_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBP_EBX)
            {
                throw new Exception("Error reading the XOR EBP, EBX instruction, reason: incorrect opcode");
            }

            XOR_EBP_EBX xor = new XOR_EBP_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBP_ECX)
            {
                throw new Exception("Error reading the XOR EBP, ECX instruction, reason: incorrect opcode");
            }

            XOR_EBP_ECX xor = new XOR_EBP_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBP_EDI)
            {
                throw new Exception("Error reading the XOR EBP, EDI instruction, reason: incorrect opcode");
            }

            XOR_EBP_EDI xor = new XOR_EBP_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBP_EDX)
            {
                throw new Exception("Error reading the XOR EBP, EDX instruction, reason: incorrect opcode");
            }

            XOR_EBP_EDX xor = new XOR_EBP_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBP_ESI)
            {
                throw new Exception("Error reading the XOR EBP, ESI instruction, reason: incorrect opcode");
            }

            XOR_EBP_ESI xor = new XOR_EBP_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBP_ESP)
            {
                throw new Exception("Error reading the XOR EBP, ESP instruction, reason: incorrect opcode");
            }

            XOR_EBP_ESP xor = new XOR_EBP_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region XOR EBX
        public Instruction ReadXOR_EBX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBX_EAX)
            {
                throw new Exception("Error reading the XOR EBX, EAX instruction, reason: incorrect opcode");
            }

            XOR_EBX_EAX xor = new XOR_EBX_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBX_EBP)
            {
                throw new Exception("Error reading the XOR EBX, EBX instruction, reason: incorrect opcode");
            }

            XOR_EBX_EBP xor = new XOR_EBX_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBX_EBX)
            {
                throw new Exception("Error reading the XOR EAX, EBX instruction, reason: incorrect opcode");
            }

            XOR_EBX_EBX xor = new XOR_EBX_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBX_ECX)
            {
                throw new Exception("Error reading the XOR EBX, ECX instruction, reason: incorrect opcode");
            }

            XOR_EBX_ECX xor = new XOR_EBX_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBX_EDI)
            {
                throw new Exception("Error reading the XOR EBX, EDI instruction, reason: incorrect opcode");
            }

            XOR_EBX_EDI xor = new XOR_EBX_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBX_EDX)
            {
                throw new Exception("Error reading the XOR EBX, EDX instruction, reason: incorrect opcode");
            }

            XOR_EBX_EDX xor = new XOR_EBX_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBX_ESI)
            {
                throw new Exception("Error reading the XOR EBX, ESI instruction, reason: incorrect opcode");
            }

            XOR_EBX_ESI xor = new XOR_EBX_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EBX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EBX_ESP)
            {
                throw new Exception("Error reading the XOR EBX, ESP instruction, reason: incorrect opcode");
            }

            XOR_EBX_ESP xor = new XOR_EBX_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region XOR ECX
        public Instruction ReadXOR_ECX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ECX_EAX)
            {
                throw new Exception("Error reading the XOR ECX, EAX instruction, reason: incorrect opcode");
            }

            XOR_ECX_EAX xor = new XOR_ECX_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ECX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ECX_EBP)
            {
                throw new Exception("Error reading the XOR ECX, EBX instruction, reason: incorrect opcode");
            }

            XOR_ECX_EBP xor = new XOR_ECX_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ECX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ECX_EBX)
            {
                throw new Exception("Error reading the XOR ECX, EBX instruction, reason: incorrect opcode");
            }

            XOR_ECX_EBX xor = new XOR_ECX_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ECX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ECX_ECX)
            {
                throw new Exception("Error reading the XOR ECX, ECX instruction, reason: incorrect opcode");
            }

            XOR_ECX_ECX xor = new XOR_ECX_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ECX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ECX_EDI)
            {
                throw new Exception("Error reading the XOR ECX, EDI instruction, reason: incorrect opcode");
            }

            XOR_ECX_EDI xor = new XOR_ECX_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ECX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ECX_EDX)
            {
                throw new Exception("Error reading the XOR ECX, EDX instruction, reason: incorrect opcode");
            }

            XOR_ECX_EDX xor = new XOR_ECX_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ECX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ECX_ESI)
            {
                throw new Exception("Error reading the XOR ECX, ESI instruction, reason: incorrect opcode");
            }

            XOR_ECX_ESI xor = new XOR_ECX_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ECX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ECX_ESP)
            {
                throw new Exception("Error reading the XOR ECX, ESP instruction, reason: incorrect opcode");
            }

            XOR_ECX_ESP xor = new XOR_ECX_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region XOR EDI
        public Instruction ReadXOR_EDI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDI_EAX)
            {
                throw new Exception("Error reading the XOR EDI, EAX instruction, reason: incorrect opcode");
            }

            XOR_EDI_EAX xor = new XOR_EDI_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDI_EBP)
            {
                throw new Exception("Error reading the XOR EDI, EBP instruction, reason: incorrect opcode");
            }

            XOR_EDI_EBP xor = new XOR_EDI_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDI_EBX)
            {
                throw new Exception("Error reading the XOR EAX, EBX instruction, reason: incorrect opcode");
            }

            XOR_EDI_EBX xor = new XOR_EDI_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDI_ECX)
            {
                throw new Exception("Error reading the XOR EDI, ECX instruction, reason: incorrect opcode");
            }

            XOR_EDI_ECX xor = new XOR_EDI_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDI_EDI)
            {
                throw new Exception("Error reading the XOR EDI, EDI instruction, reason: incorrect opcode");
            }

            XOR_EDI_EDI xor = new XOR_EDI_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDI_EDX)
            {
                throw new Exception("Error reading the XOR EDI, EDX instruction, reason: incorrect opcode");
            }

            XOR_EDI_EDX xor = new XOR_EDI_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDI_ESI)
            {
                throw new Exception("Error reading the XOR EDI, ESI instruction, reason: incorrect opcode");
            }

            XOR_EDI_ESI xor = new XOR_EDI_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDI_ESP)
            {
                throw new Exception("Error reading the XOR EDI, ESP instruction, reason: incorrect opcode");
            }

            XOR_EDI_ESP xor = new XOR_EDI_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region XOR EDX
        public Instruction ReadXOR_EDX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDX_EAX)
            {
                throw new Exception("Error reading the XOR EDX, EAX instruction, reason: incorrect opcode");
            }

            XOR_EDX_EAX xor = new XOR_EDX_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDX_EBP)
            {
                throw new Exception("Error reading the XOR EDX, EBP instruction, reason: incorrect opcode");
            }

            XOR_EDX_EBP xor = new XOR_EDX_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDX_EBX)
            {
                throw new Exception("Error reading the XOR EAX, EBX instruction, reason: incorrect opcode");
            }

            XOR_EDX_EBX xor = new XOR_EDX_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDX_ECX)
            {
                throw new Exception("Error reading the XOR EDX, ECX instruction, reason: incorrect opcode");
            }

            XOR_EDX_ECX xor = new XOR_EDX_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDX_EDI)
            {
                throw new Exception("Error reading the XOR EDX, EDX instruction, reason: incorrect opcode");
            }

            XOR_EDX_EDI xor = new XOR_EDX_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDX_EDX)
            {
                throw new Exception("Error reading the XOR EDX, EDX instruction, reason: incorrect opcode");
            }

            XOR_EDX_EDX xor = new XOR_EDX_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDX_ESI)
            {
                throw new Exception("Error reading the XOR EDX, ESI instruction, reason: incorrect opcode");
            }

            XOR_EDX_ESI xor = new XOR_EDX_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_EDX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_EDX_ESP)
            {
                throw new Exception("Error reading the XOR EDX, ESP instruction, reason: incorrect opcode");
            }

            XOR_EDX_ESP xor = new XOR_EDX_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region XOR ESI
        public Instruction ReadXOR_ESI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESI_EAX)
            {
                throw new Exception("Error reading the XOR ESI, EAX instruction, reason: incorrect opcode");
            }

            XOR_ESI_EAX xor = new XOR_ESI_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESI_EBP)
            {
                throw new Exception("Error reading the XOR ESI, EBP instruction, reason: incorrect opcode");
            }

            XOR_ESI_EBP xor = new XOR_ESI_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESI_EBX)
            {
                throw new Exception("Error reading the XOR EAX, EBX instruction, reason: incorrect opcode");
            }

            XOR_ESI_EBX xor = new XOR_ESI_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESI_ECX)
            {
                throw new Exception("Error reading the XOR ESI, ECX instruction, reason: incorrect opcode");
            }

            XOR_ESI_ECX xor = new XOR_ESI_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESI_EDI)
            {
                throw new Exception("Error reading the XOR ESI, EDI instruction, reason: incorrect opcode");
            }

            XOR_ESI_EDI xor = new XOR_ESI_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESI_EDX)
            {
                throw new Exception("Error reading the XOR ESI, EDX instruction, reason: incorrect opcode");
            }

            XOR_ESI_EDX xor = new XOR_ESI_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESI_ESI)
            {
                throw new Exception("Error reading the XOR ESI, ESI instruction, reason: incorrect opcode");
            }

            XOR_ESI_ESI xor = new XOR_ESI_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESI_ESP)
            {
                throw new Exception("Error reading the XOR ESI, ESP instruction, reason: incorrect opcode");
            }

            XOR_ESI_ESP xor = new XOR_ESI_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region XOR ESP
        public Instruction ReadXOR_ESP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESP_EAX)
            {
                throw new Exception("Error reading the XOR ESP, EAX instruction, reason: incorrect opcode");
            }

            XOR_ESP_EAX xor = new XOR_ESP_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESP_EBP)
            {
                throw new Exception("Error reading the XOR ESP, EBP instruction, reason: incorrect opcode");
            }

            XOR_ESP_EBP xor = new XOR_ESP_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESP_EBX)
            {
                throw new Exception("Error reading the XOR EAX, EBX instruction, reason: incorrect opcode");
            }

            XOR_ESP_EBX xor = new XOR_ESP_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESP_ECX)
            {
                throw new Exception("Error reading the XOR ESP, ECX instruction, reason: incorrect opcode");
            }

            XOR_ESP_ECX xor = new XOR_ESP_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESP_EDI)
            {
                throw new Exception("Error reading the XOR ESP, ESP instruction, reason: incorrect opcode");
            }

            XOR_ESP_EDI xor = new XOR_ESP_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESP_EDX)
            {
                throw new Exception("Error reading the XOR ESP, EDX instruction, reason: incorrect opcode");
            }

            XOR_ESP_EDX xor = new XOR_ESP_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESP_ESI)
            {
                throw new Exception("Error reading the XOR ESP, ESI instruction, reason: incorrect opcode");
            }

            XOR_ESP_ESI xor = new XOR_ESP_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadXOR_ESP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.XOR_REGISTER &&
                InstructionData[offset + 1] != (byte)XorRegisterOpcodes.XOR_ESP_ESP)
            {
                throw new Exception("Error reading the XOR ESP, ESP instruction, reason: incorrect opcode");
            }

            XOR_ESP_ESP xor = new XOR_ESP_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion

        #region AND EAX
        public Instruction ReadAND_EAX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EAX_EAX)
            {
                throw new Exception("Error reading the AND EAX, EAX instruction, reason: incorrect opcode");
            }

            AND_EAX_EAX and = new AND_EAX_EAX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EAX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EAX_EBP)
            {
                throw new Exception("Error reading the AND EAX, EBP instruction, reason: incorrect opcode");
            }

            AND_EAX_EBP and = new AND_EAX_EBP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EAX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EAX_EBX)
            {
                throw new Exception("Error reading the AND EAX, EBX instruction, reason: incorrect opcode");
            }

            AND_EAX_EBX and = new AND_EAX_EBX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EAX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EAX_ECX)
            {
                throw new Exception("Error reading the AND EAX, ECX instruction, reason: incorrect opcode");
            }

            AND_EAX_ECX and = new AND_EAX_ECX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EAX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EAX_EDI)
            {
                throw new Exception("Error reading the AND EAX, EDI instruction, reason: incorrect opcode");
            }

            AND_EAX_EDI and = new AND_EAX_EDI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EAX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EAX_EDX)
            {
                throw new Exception("Error reading the AND EAX, EDX instruction, reason: incorrect opcode");
            }

            AND_EAX_EDX and = new AND_EAX_EDX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EAX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EAX_ESI)
            {
                throw new Exception("Error reading the AND EAX, ESI instruction, reason: incorrect opcode");
            }

            AND_EAX_ESI and = new AND_EAX_ESI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EAX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EAX_ESP)
            {
                throw new Exception("Error reading the AND EAX, ESP instruction, reason: incorrect opcode");
            }

            AND_EAX_ESP and = new AND_EAX_ESP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        #endregion
        #region AND EBP
        public Instruction ReadAND_EBP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBP_EAX)
            {
                throw new Exception("Error reading the AND EBP, EAX instruction, reason: incorrect opcode");
            }

            AND_EBP_EAX and = new AND_EBP_EAX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBP_EBP)
            {
                throw new Exception("Error reading the AND EBP, EBP instruction, reason: incorrect opcode");
            }

            AND_EBP_EBP and = new AND_EBP_EBP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBP_EBX)
            {
                throw new Exception("Error reading the AND EBP, EBX instruction, reason: incorrect opcode");
            }

            AND_EBP_EBX and = new AND_EBP_EBX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBP_ECX)
            {
                throw new Exception("Error reading the AND EBP, ECX instruction, reason: incorrect opcode");
            }

            AND_EBP_ECX and = new AND_EBP_ECX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBP_EDI)
            {
                throw new Exception("Error reading the AND EBP, EDI instruction, reason: incorrect opcode");
            }

            AND_EBP_EDI and = new AND_EBP_EDI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBP_EDX)
            {
                throw new Exception("Error reading the AND EBP, EDX instruction, reason: incorrect opcode");
            }

            AND_EBP_EDX and = new AND_EBP_EDX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBP_ESI)
            {
                throw new Exception("Error reading the AND EBP, ESI instruction, reason: incorrect opcode");
            }

            AND_EBP_ESI and = new AND_EBP_ESI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBP_ESP)
            {
                throw new Exception("Error reading the AND EBP, ESP instruction, reason: incorrect opcode");
            }

            AND_EBP_ESP and = new AND_EBP_ESP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        #endregion
        #region AND EBX
        public Instruction ReadAND_EBX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBX_EAX)
            {
                throw new Exception("Error reading the AND EBX, EAX instruction, reason: incorrect opcode");
            }

            AND_EBX_EAX and = new AND_EBX_EAX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBX_EBP)
            {
                throw new Exception("Error reading the AND EBX, EBX instruction, reason: incorrect opcode");
            }

            AND_EBX_EBP and = new AND_EBX_EBP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBX_EBX)
            {
                throw new Exception("Error reading the AND EAX, EBX instruction, reason: incorrect opcode");
            }

            AND_EBX_EBX and = new AND_EBX_EBX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBX_ECX)
            {
                throw new Exception("Error reading the AND EBX, ECX instruction, reason: incorrect opcode");
            }

            AND_EBX_ECX and = new AND_EBX_ECX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBX_EDI)
            {
                throw new Exception("Error reading the AND EBX, EDI instruction, reason: incorrect opcode");
            }

            AND_EBX_EDI and = new AND_EBX_EDI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBX_EDX)
            {
                throw new Exception("Error reading the AND EBX, EDX instruction, reason: incorrect opcode");
            }

            AND_EBX_EDX and = new AND_EBX_EDX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBX_ESI)
            {
                throw new Exception("Error reading the AND EBX, ESI instruction, reason: incorrect opcode");
            }

            AND_EBX_ESI and = new AND_EBX_ESI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EBX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EBX_ESP)
            {
                throw new Exception("Error reading the AND EBX, ESP instruction, reason: incorrect opcode");
            }

            AND_EBX_ESP and = new AND_EBX_ESP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        #endregion
        #region AND ECX
        public Instruction ReadAND_ECX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ECX_EAX)
            {
                throw new Exception("Error reading the AND ECX, EAX instruction, reason: incorrect opcode");
            }

            AND_ECX_EAX and = new AND_ECX_EAX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ECX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ECX_EBP)
            {
                throw new Exception("Error reading the AND ECX, EBX instruction, reason: incorrect opcode");
            }

            AND_ECX_EBP and = new AND_ECX_EBP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ECX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ECX_EBX)
            {
                throw new Exception("Error reading the AND ECX, EBX instruction, reason: incorrect opcode");
            }

            AND_ECX_EBX and = new AND_ECX_EBX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ECX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ECX_ECX)
            {
                throw new Exception("Error reading the AND ECX, ECX instruction, reason: incorrect opcode");
            }

            AND_ECX_ECX and = new AND_ECX_ECX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ECX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ECX_EDI)
            {
                throw new Exception("Error reading the AND ECX, EDI instruction, reason: incorrect opcode");
            }

            AND_ECX_EDI and = new AND_ECX_EDI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ECX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ECX_EDX)
            {
                throw new Exception("Error reading the AND ECX, EDX instruction, reason: incorrect opcode");
            }

            AND_ECX_EDX and = new AND_ECX_EDX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ECX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ECX_ESI)
            {
                throw new Exception("Error reading the AND ECX, ESI instruction, reason: incorrect opcode");
            }

            AND_ECX_ESI and = new AND_ECX_ESI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ECX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ECX_ESP)
            {
                throw new Exception("Error reading the AND ECX, ESP instruction, reason: incorrect opcode");
            }

            AND_ECX_ESP and = new AND_ECX_ESP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        #endregion
        #region AND EDI
        public Instruction ReadAND_EDI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDI_EAX)
            {
                throw new Exception("Error reading the AND EDI, EAX instruction, reason: incorrect opcode");
            }

            AND_EDI_EAX and = new AND_EDI_EAX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDI_EBP)
            {
                throw new Exception("Error reading the AND EDI, EBP instruction, reason: incorrect opcode");
            }

            AND_EDI_EBP and = new AND_EDI_EBP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDI_EBX)
            {
                throw new Exception("Error reading the AND EAX, EBX instruction, reason: incorrect opcode");
            }

            AND_EDI_EBX and = new AND_EDI_EBX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDI_ECX)
            {
                throw new Exception("Error reading the AND EDI, ECX instruction, reason: incorrect opcode");
            }

            AND_EDI_ECX and = new AND_EDI_ECX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDI_EDI)
            {
                throw new Exception("Error reading the AND EDI, EDI instruction, reason: incorrect opcode");
            }

            AND_EDI_EDI and = new AND_EDI_EDI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDI_EDX)
            {
                throw new Exception("Error reading the AND EDI, EDX instruction, reason: incorrect opcode");
            }

            AND_EDI_EDX and = new AND_EDI_EDX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDI_ESI)
            {
                throw new Exception("Error reading the AND EDI, ESI instruction, reason: incorrect opcode");
            }

            AND_EDI_ESI and = new AND_EDI_ESI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDI_ESP)
            {
                throw new Exception("Error reading the AND EDI, ESP instruction, reason: incorrect opcode");
            }

            AND_EDI_ESP and = new AND_EDI_ESP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        #endregion
        #region AND EDX
        public Instruction ReadAND_EDX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDX_EAX)
            {
                throw new Exception("Error reading the AND EDX, EAX instruction, reason: incorrect opcode");
            }

            AND_EDX_EAX and = new AND_EDX_EAX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDX_EBP)
            {
                throw new Exception("Error reading the AND EDX, EBP instruction, reason: incorrect opcode");
            }

            AND_EDX_EBP and = new AND_EDX_EBP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDX_EBX)
            {
                throw new Exception("Error reading the AND EAX, EBX instruction, reason: incorrect opcode");
            }

            AND_EDX_EBX and = new AND_EDX_EBX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDX_ECX)
            {
                throw new Exception("Error reading the AND EDX, ECX instruction, reason: incorrect opcode");
            }

            AND_EDX_ECX and = new AND_EDX_ECX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDX_EDI)
            {
                throw new Exception("Error reading the AND EDX, EDX instruction, reason: incorrect opcode");
            }

            AND_EDX_EDI and = new AND_EDX_EDI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDX_EDX)
            {
                throw new Exception("Error reading the AND EDX, EDX instruction, reason: incorrect opcode");
            }

            AND_EDX_EDX and = new AND_EDX_EDX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDX_ESI)
            {
                throw new Exception("Error reading the AND EDX, ESI instruction, reason: incorrect opcode");
            }

            AND_EDX_ESI and = new AND_EDX_ESI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_EDX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_EDX_ESP)
            {
                throw new Exception("Error reading the AND EDX, ESP instruction, reason: incorrect opcode");
            }

            AND_EDX_ESP and = new AND_EDX_ESP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        #endregion
        #region AND ESI
        public Instruction ReadAND_ESI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESI_EAX)
            {
                throw new Exception("Error reading the AND ESI, EAX instruction, reason: incorrect opcode");
            }

            AND_ESI_EAX and = new AND_ESI_EAX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESI_EBP)
            {
                throw new Exception("Error reading the AND ESI, EBP instruction, reason: incorrect opcode");
            }

            AND_ESI_EBP and = new AND_ESI_EBP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESI_EBX)
            {
                throw new Exception("Error reading the AND EAX, EBX instruction, reason: incorrect opcode");
            }

            AND_ESI_EBX and = new AND_ESI_EBX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESI_ECX)
            {
                throw new Exception("Error reading the AND ESI, ECX instruction, reason: incorrect opcode");
            }

            AND_ESI_ECX and = new AND_ESI_ECX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESI_EDI)
            {
                throw new Exception("Error reading the AND ESI, EDI instruction, reason: incorrect opcode");
            }

            AND_ESI_EDI and = new AND_ESI_EDI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESI_EDX)
            {
                throw new Exception("Error reading the AND ESI, EDX instruction, reason: incorrect opcode");
            }

            AND_ESI_EDX and = new AND_ESI_EDX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESI_ESI)
            {
                throw new Exception("Error reading the AND ESI, ESI instruction, reason: incorrect opcode");
            }

            AND_ESI_ESI and = new AND_ESI_ESI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESI_ESP)
            {
                throw new Exception("Error reading the AND ESI, ESP instruction, reason: incorrect opcode");
            }

            AND_ESI_ESP and = new AND_ESI_ESP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        #endregion
        #region AND ESP
        public Instruction ReadAND_ESP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESP_EAX)
            {
                throw new Exception("Error reading the AND ESP, EAX instruction, reason: incorrect opcode");
            }

            AND_ESP_EAX and = new AND_ESP_EAX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESP_EBP)
            {
                throw new Exception("Error reading the AND ESP, EBP instruction, reason: incorrect opcode");
            }

            AND_ESP_EBP and = new AND_ESP_EBP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESP_EBX)
            {
                throw new Exception("Error reading the AND EAX, EBX instruction, reason: incorrect opcode");
            }

            AND_ESP_EBX and = new AND_ESP_EBX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESP_ECX)
            {
                throw new Exception("Error reading the AND ESP, ECX instruction, reason: incorrect opcode");
            }

            AND_ESP_ECX and = new AND_ESP_ECX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESP_EDI)
            {
                throw new Exception("Error reading the AND ESP, EDI instruction, reason: incorrect opcode");
            }

            AND_ESP_EDI and = new AND_ESP_EDI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESP_EDX)
            {
                throw new Exception("Error reading the AND ESP, EDX instruction, reason: incorrect opcode");
            }

            AND_ESP_EDX and = new AND_ESP_EDX();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESP_ESI)
            {
                throw new Exception("Error reading the AND ESP, ESI instruction, reason: incorrect opcode");
            }

            AND_ESP_ESI and = new AND_ESP_ESI();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        public Instruction ReadAND_ESP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.AND_REGISTER &&
                InstructionData[offset + 1] != (byte)AndRegisterOpcodes.AND_ESP_ESP)
            {
                throw new Exception("Error reading the AND ESP, ESP instruction, reason: incorrect opcode");
            }

            AND_ESP_ESP and = new AND_ESP_ESP();
            and.VirtualAddress.Address = MemAddress;
            offset += and.VirtualAddress.Size;
            MemAddress += and.VirtualAddress.Size;
            Instructions.Add(and.VirtualAddress.Address, and);
            return and;
        }
        #endregion

        #region CMP EAX
        public Instruction ReadCMP_EAX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EAX_EAX)
            {
                throw new Exception("Error reading the CMP EAX, EAX instruction, reason: incorrect opcode");
            }

            CMP_EAX_EAX cmp = new CMP_EAX_EAX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EAX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EAX_EBP)
            {
                throw new Exception("Error reading the CMP EAX, EBP instruction, reason: incorrect opcode");
            }

            CMP_EAX_EBP cmp = new CMP_EAX_EBP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EAX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EAX_EBX)
            {
                throw new Exception("Error reading the CMP EAX, EBX instruction, reason: incorrect opcode");
            }

            CMP_EAX_EBX cmp = new CMP_EAX_EBX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EAX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EAX_ECX)
            {
                throw new Exception("Error reading the CMP EAX, ECX instruction, reason: incorrect opcode");
            }

            CMP_EAX_ECX cmp = new CMP_EAX_ECX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EAX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EAX_EDI)
            {
                throw new Exception("Error reading the CMP EAX, EDI instruction, reason: incorrect opcode");
            }

            CMP_EAX_EDI cmp = new CMP_EAX_EDI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EAX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EAX_EDX)
            {
                throw new Exception("Error reading the CMP EAX, EDX instruction, reason: incorrect opcode");
            }

            CMP_EAX_EDX cmp = new CMP_EAX_EDX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EAX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EAX_ESI)
            {
                throw new Exception("Error reading the CMP EAX, ESI instruction, reason: incorrect opcode");
            }

            CMP_EAX_ESI cmp = new CMP_EAX_ESI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EAX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EAX_ESP)
            {
                throw new Exception("Error reading the CMP EAX, ESP instruction, reason: incorrect opcode");
            }

            CMP_EAX_ESP cmp = new CMP_EAX_ESP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        #endregion
        #region CMP EBP
        public Instruction ReadCMP_EBP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBP_EAX)
            {
                throw new Exception("Error reading the CMP EBP, EAX instruction, reason: incorrect opcode");
            }

            CMP_EBP_EAX cmp = new CMP_EBP_EAX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBP_EBP)
            {
                throw new Exception("Error reading the CMP EBP, EBP instruction, reason: incorrect opcode");
            }

            CMP_EBP_EBP cmp = new CMP_EBP_EBP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBP_EBX)
            {
                throw new Exception("Error reading the CMP EBP, EBX instruction, reason: incorrect opcode");
            }

            CMP_EBP_EBX cmp = new CMP_EBP_EBX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBP_ECX)
            {
                throw new Exception("Error reading the CMP EBP, ECX instruction, reason: incorrect opcode");
            }

            CMP_EBP_ECX cmp = new CMP_EBP_ECX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBP_EDI)
            {
                throw new Exception("Error reading the CMP EBP, EDI instruction, reason: incorrect opcode");
            }

            CMP_EBP_EDI cmp = new CMP_EBP_EDI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBP_EDX)
            {
                throw new Exception("Error reading the CMP EBP, EDX instruction, reason: incorrect opcode");
            }

            CMP_EBP_EDX cmp = new CMP_EBP_EDX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBP_ESI)
            {
                throw new Exception("Error reading the CMP EBP, ESI instruction, reason: incorrect opcode");
            }

            CMP_EBP_ESI cmp = new CMP_EBP_ESI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBP_ESP)
            {
                throw new Exception("Error reading the CMP EBP, ESP instruction, reason: incorrect opcode");
            }

            CMP_EBP_ESP cmp = new CMP_EBP_ESP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        #endregion
        #region CMP EBX
        public Instruction ReadCMP_EBX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBX_EAX)
            {
                throw new Exception("Error reading the CMP EBX, EAX instruction, reason: incorrect opcode");
            }

            CMP_EBX_EAX cmp = new CMP_EBX_EAX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBX_EBP)
            {
                throw new Exception("Error reading the CMP EBX, EBX instruction, reason: incorrect opcode");
            }

            CMP_EBX_EBP cmp = new CMP_EBX_EBP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBX_EBX)
            {
                throw new Exception("Error reading the CMP EAX, EBX instruction, reason: incorrect opcode");
            }

            CMP_EBX_EBX cmp = new CMP_EBX_EBX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBX_ECX)
            {
                throw new Exception("Error reading the CMP EBX, ECX instruction, reason: incorrect opcode");
            }

            CMP_EBX_ECX cmp = new CMP_EBX_ECX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBX_EDI)
            {
                throw new Exception("Error reading the CMP EBX, EDI instruction, reason: incorrect opcode");
            }

            CMP_EBX_EDI cmp = new CMP_EBX_EDI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBX_EDX)
            {
                throw new Exception("Error reading the CMP EBX, EDX instruction, reason: incorrect opcode");
            }

            CMP_EBX_EDX cmp = new CMP_EBX_EDX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBX_ESI)
            {
                throw new Exception("Error reading the CMP EBX, ESI instruction, reason: incorrect opcode");
            }

            CMP_EBX_ESI cmp = new CMP_EBX_ESI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EBX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EBX_ESP)
            {
                throw new Exception("Error reading the CMP EBX, ESP instruction, reason: incorrect opcode");
            }

            CMP_EBX_ESP cmp = new CMP_EBX_ESP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        #endregion
        #region CMP ECX
        public Instruction ReadCMP_ECX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ECX_EAX)
            {
                throw new Exception("Error reading the CMP ECX, EAX instruction, reason: incorrect opcode");
            }

            CMP_ECX_EAX cmp = new CMP_ECX_EAX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ECX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ECX_EBP)
            {
                throw new Exception("Error reading the CMP ECX, EBX instruction, reason: incorrect opcode");
            }

            CMP_ECX_EBP cmp = new CMP_ECX_EBP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ECX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ECX_EBX)
            {
                throw new Exception("Error reading the CMP ECX, EBX instruction, reason: incorrect opcode");
            }

            CMP_ECX_EBX cmp = new CMP_ECX_EBX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ECX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ECX_ECX)
            {
                throw new Exception("Error reading the CMP ECX, ECX instruction, reason: incorrect opcode");
            }

            CMP_ECX_ECX cmp = new CMP_ECX_ECX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ECX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ECX_EDI)
            {
                throw new Exception("Error reading the CMP ECX, EDI instruction, reason: incorrect opcode");
            }

            CMP_ECX_EDI cmp = new CMP_ECX_EDI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ECX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ECX_EDX)
            {
                throw new Exception("Error reading the CMP ECX, EDX instruction, reason: incorrect opcode");
            }

            CMP_ECX_EDX cmp = new CMP_ECX_EDX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ECX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ECX_ESI)
            {
                throw new Exception("Error reading the CMP ECX, ESI instruction, reason: incorrect opcode");
            }

            CMP_ECX_ESI cmp = new CMP_ECX_ESI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ECX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ECX_ESP)
            {
                throw new Exception("Error reading the CMP ECX, ESP instruction, reason: incorrect opcode");
            }

            CMP_ECX_ESP cmp = new CMP_ECX_ESP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        #endregion
        #region CMP EDI
        public Instruction ReadCMP_EDI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDI_EAX)
            {
                throw new Exception("Error reading the CMP EDI, EAX instruction, reason: incorrect opcode");
            }

            CMP_EDI_EAX cmp = new CMP_EDI_EAX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDI_EBP)
            {
                throw new Exception("Error reading the CMP EDI, EBP instruction, reason: incorrect opcode");
            }

            CMP_EDI_EBP cmp = new CMP_EDI_EBP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDI_EBX)
            {
                throw new Exception("Error reading the CMP EAX, EBX instruction, reason: incorrect opcode");
            }

            CMP_EDI_EBX cmp = new CMP_EDI_EBX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDI_ECX)
            {
                throw new Exception("Error reading the CMP EDI, ECX instruction, reason: incorrect opcode");
            }

            CMP_EDI_ECX cmp = new CMP_EDI_ECX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDI_EDI)
            {
                throw new Exception("Error reading the CMP EDI, EDI instruction, reason: incorrect opcode");
            }

            CMP_EDI_EDI cmp = new CMP_EDI_EDI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDI_EDX)
            {
                throw new Exception("Error reading the CMP EDI, EDX instruction, reason: incorrect opcode");
            }

            CMP_EDI_EDX cmp = new CMP_EDI_EDX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDI_ESI)
            {
                throw new Exception("Error reading the CMP EDI, ESI instruction, reason: incorrect opcode");
            }

            CMP_EDI_ESI cmp = new CMP_EDI_ESI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDI_ESP)
            {
                throw new Exception("Error reading the CMP EDI, ESP instruction, reason: incorrect opcode");
            }

            CMP_EDI_ESP cmp = new CMP_EDI_ESP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        #endregion
        #region CMP EDX
        public Instruction ReadCMP_EDX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDX_EAX)
            {
                throw new Exception("Error reading the CMP EDX, EAX instruction, reason: incorrect opcode");
            }

            CMP_EDX_EAX cmp = new CMP_EDX_EAX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDX_EBP)
            {
                throw new Exception("Error reading the CMP EDX, EBP instruction, reason: incorrect opcode");
            }

            CMP_EDX_EBP cmp = new CMP_EDX_EBP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDX_EBX)
            {
                throw new Exception("Error reading the CMP EAX, EBX instruction, reason: incorrect opcode");
            }

            CMP_EDX_EBX cmp = new CMP_EDX_EBX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDX_ECX)
            {
                throw new Exception("Error reading the CMP EDX, ECX instruction, reason: incorrect opcode");
            }

            CMP_EDX_ECX cmp = new CMP_EDX_ECX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDX_EDI)
            {
                throw new Exception("Error reading the CMP EDX, EDX instruction, reason: incorrect opcode");
            }

            CMP_EDX_EDI cmp = new CMP_EDX_EDI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDX_EDX)
            {
                throw new Exception("Error reading the CMP EDX, EDX instruction, reason: incorrect opcode");
            }

            CMP_EDX_EDX cmp = new CMP_EDX_EDX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDX_ESI)
            {
                throw new Exception("Error reading the CMP EDX, ESI instruction, reason: incorrect opcode");
            }

            CMP_EDX_ESI cmp = new CMP_EDX_ESI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_EDX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_EDX_ESP)
            {
                throw new Exception("Error reading the CMP EDX, ESP instruction, reason: incorrect opcode");
            }

            CMP_EDX_ESP cmp = new CMP_EDX_ESP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        #endregion
        #region CMP ESI
        public Instruction ReadCMP_ESI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESI_EAX)
            {
                throw new Exception("Error reading the CMP ESI, EAX instruction, reason: incorrect opcode");
            }

            CMP_ESI_EAX cmp = new CMP_ESI_EAX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESI_EBP)
            {
                throw new Exception("Error reading the CMP ESI, EBP instruction, reason: incorrect opcode");
            }

            CMP_ESI_EBP cmp = new CMP_ESI_EBP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESI_EBX)
            {
                throw new Exception("Error reading the CMP EAX, EBX instruction, reason: incorrect opcode");
            }

            CMP_ESI_EBX cmp = new CMP_ESI_EBX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESI_ECX)
            {
                throw new Exception("Error reading the CMP ESI, ECX instruction, reason: incorrect opcode");
            }

            CMP_ESI_ECX cmp = new CMP_ESI_ECX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESI_EDI)
            {
                throw new Exception("Error reading the CMP ESI, EDI instruction, reason: incorrect opcode");
            }

            CMP_ESI_EDI cmp = new CMP_ESI_EDI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESI_EDX)
            {
                throw new Exception("Error reading the CMP ESI, EDX instruction, reason: incorrect opcode");
            }

            CMP_ESI_EDX cmp = new CMP_ESI_EDX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESI_ESI)
            {
                throw new Exception("Error reading the CMP ESI, ESI instruction, reason: incorrect opcode");
            }

            CMP_ESI_ESI cmp = new CMP_ESI_ESI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESI_ESP)
            {
                throw new Exception("Error reading the CMP ESI, ESP instruction, reason: incorrect opcode");
            }

            CMP_ESI_ESP cmp = new CMP_ESI_ESP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        #endregion
        #region CMP ESP
        public Instruction ReadCMP_ESP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESP_EAX)
            {
                throw new Exception("Error reading the CMP ESP, EAX instruction, reason: incorrect opcode");
            }

            CMP_ESP_EAX cmp = new CMP_ESP_EAX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESP_EBP)
            {
                throw new Exception("Error reading the CMP ESP, EBP instruction, reason: incorrect opcode");
            }

            CMP_ESP_EBP cmp = new CMP_ESP_EBP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESP_EBX)
            {
                throw new Exception("Error reading the CMP EAX, EBX instruction, reason: incorrect opcode");
            }

            CMP_ESP_EBX cmp = new CMP_ESP_EBX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESP_ECX)
            {
                throw new Exception("Error reading the CMP ESP, ECX instruction, reason: incorrect opcode");
            }

            CMP_ESP_ECX cmp = new CMP_ESP_ECX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESP_EDI)
            {
                throw new Exception("Error reading the CMP ESP, ESP instruction, reason: incorrect opcode");
            }

            CMP_ESP_EDI cmp = new CMP_ESP_EDI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESP_EDX)
            {
                throw new Exception("Error reading the CMP ESP, EDX instruction, reason: incorrect opcode");
            }

            CMP_ESP_EDX cmp = new CMP_ESP_EDX();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESP_ESI)
            {
                throw new Exception("Error reading the CMP ESP, ESI instruction, reason: incorrect opcode");
            }

            CMP_ESP_ESI cmp = new CMP_ESP_ESI();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        public Instruction ReadCMP_ESP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.CMP_REGISTER &&
                InstructionData[offset + 1] != (byte)CmpRegisterOpcodes.CMP_ESP_ESP)
            {
                throw new Exception("Error reading the CMP ESP, ESP instruction, reason: incorrect opcode");
            }

            CMP_ESP_ESP cmp = new CMP_ESP_ESP();
            cmp.VirtualAddress.Address = MemAddress;
            offset += cmp.VirtualAddress.Size;
            MemAddress += cmp.VirtualAddress.Size;
            Instructions.Add(cmp.VirtualAddress.Address, cmp);
            return cmp;
        }
        #endregion

        #region MOV EAX
        public Instruction ReadMOV_EAX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EAX_EAX)
            {
                throw new Exception("Error reading the MOV EAX, EAX instruction, reason: incorrect opcode");
            }

            MOV_EAX_EAX mov = new MOV_EAX_EAX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EAX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EAX_EBP)
            {
                throw new Exception("Error reading the MOV EAX, EBP instruction, reason: incorrect opcode");
            }

            MOV_EAX_EBP mov = new MOV_EAX_EBP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EAX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EAX_EBX)
            {
                throw new Exception("Error reading the MOV EAX, EBX instruction, reason: incorrect opcode");
            }

            MOV_EAX_EBX mov = new MOV_EAX_EBX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EAX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EAX_ECX)
            {
                throw new Exception("Error reading the MOV EAX, ECX instruction, reason: incorrect opcode");
            }

            MOV_EAX_ECX mov = new MOV_EAX_ECX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EAX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EAX_EDI)
            {
                throw new Exception("Error reading the MOV EAX, EDI instruction, reason: incorrect opcode");
            }

            MOV_EAX_EDI mov = new MOV_EAX_EDI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EAX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EAX_EDX)
            {
                throw new Exception("Error reading the MOV EAX, EDX instruction, reason: incorrect opcode");
            }

            MOV_EAX_EDX mov = new MOV_EAX_EDX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EAX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EAX_ESI)
            {
                throw new Exception("Error reading the MOV EAX, ESI instruction, reason: incorrect opcode");
            }

            MOV_EAX_ESI mov = new MOV_EAX_ESI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EAX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EAX_ESP)
            {
                throw new Exception("Error reading the MOV EAX, ESP instruction, reason: incorrect opcode");
            }

            MOV_EAX_ESP mov = new MOV_EAX_ESP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        #endregion
        #region MOV EBP
        public Instruction ReadMOV_EBP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBP_EAX)
            {
                throw new Exception("Error reading the MOV EBP, EAX instruction, reason: incorrect opcode");
            }

            MOV_EBP_EAX mov = new MOV_EBP_EAX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBP_EBP)
            {
                throw new Exception("Error reading the MOV EBP, EBP instruction, reason: incorrect opcode");
            }

            MOV_EBP_EBP mov = new MOV_EBP_EBP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBP_EBX)
            {
                throw new Exception("Error reading the MOV EBP, EBX instruction, reason: incorrect opcode");
            }

            MOV_EBP_EBX mov = new MOV_EBP_EBX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBP_ECX)
            {
                throw new Exception("Error reading the MOV EBP, ECX instruction, reason: incorrect opcode");
            }

            MOV_EBP_ECX mov = new MOV_EBP_ECX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBP_EDI)
            {
                throw new Exception("Error reading the MOV EBP, EDI instruction, reason: incorrect opcode");
            }

            MOV_EBP_EDI mov = new MOV_EBP_EDI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBP_EDX)
            {
                throw new Exception("Error reading the MOV EBP, EDX instruction, reason: incorrect opcode");
            }

            MOV_EBP_EDX mov = new MOV_EBP_EDX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBP_ESI)
            {
                throw new Exception("Error reading the MOV EBP, ESI instruction, reason: incorrect opcode");
            }

            MOV_EBP_ESI mov = new MOV_EBP_ESI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBP_ESP)
            {
                throw new Exception("Error reading the MOV EBP, ESP instruction, reason: incorrect opcode");
            }

            MOV_EBP_ESP mov = new MOV_EBP_ESP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        #endregion
        #region MOV EBX
        public Instruction ReadMOV_EBX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBX_EAX)
            {
                throw new Exception("Error reading the MOV EBX, EAX instruction, reason: incorrect opcode");
            }

            MOV_EBX_EAX mov = new MOV_EBX_EAX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBX_EBP)
            {
                throw new Exception("Error reading the MOV EBX, EBX instruction, reason: incorrect opcode");
            }

            MOV_EBX_EBP mov = new MOV_EBX_EBP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBX_EBX)
            {
                throw new Exception("Error reading the MOV EAX, EBX instruction, reason: incorrect opcode");
            }

            MOV_EBX_EBX mov = new MOV_EBX_EBX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBX_ECX)
            {
                throw new Exception("Error reading the MOV EBX, ECX instruction, reason: incorrect opcode");
            }

            MOV_EBX_ECX mov = new MOV_EBX_ECX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBX_EDI)
            {
                throw new Exception("Error reading the MOV EBX, EDI instruction, reason: incorrect opcode");
            }

            MOV_EBX_EDI mov = new MOV_EBX_EDI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBX_EDX)
            {
                throw new Exception("Error reading the MOV EBX, EDX instruction, reason: incorrect opcode");
            }

            MOV_EBX_EDX mov = new MOV_EBX_EDX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBX_ESI)
            {
                throw new Exception("Error reading the MOV EBX, ESI instruction, reason: incorrect opcode");
            }

            MOV_EBX_ESI mov = new MOV_EBX_ESI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EBX_ESP)
            {
                throw new Exception("Error reading the MOV EBX, ESP instruction, reason: incorrect opcode");
            }

            MOV_EBX_ESP mov = new MOV_EBX_ESP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        #endregion
        #region MOV ECX
        public Instruction ReadMOV_ECX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ECX_EAX)
            {
                throw new Exception("Error reading the MOV ECX, EAX instruction, reason: incorrect opcode");
            }

            MOV_ECX_EAX mov = new MOV_ECX_EAX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ECX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ECX_EBP)
            {
                throw new Exception("Error reading the MOV ECX, EBX instruction, reason: incorrect opcode");
            }

            MOV_ECX_EBP mov = new MOV_ECX_EBP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ECX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ECX_EBX)
            {
                throw new Exception("Error reading the MOV ECX, EBX instruction, reason: incorrect opcode");
            }

            MOV_ECX_EBX mov = new MOV_ECX_EBX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ECX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ECX_ECX)
            {
                throw new Exception("Error reading the MOV ECX, ECX instruction, reason: incorrect opcode");
            }

            MOV_ECX_ECX mov = new MOV_ECX_ECX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ECX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ECX_EDI)
            {
                throw new Exception("Error reading the MOV ECX, EDI instruction, reason: incorrect opcode");
            }

            MOV_ECX_EDI mov = new MOV_ECX_EDI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ECX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ECX_EDX)
            {
                throw new Exception("Error reading the MOV ECX, EDX instruction, reason: incorrect opcode");
            }

            MOV_ECX_EDX mov = new MOV_ECX_EDX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ECX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ECX_ESI)
            {
                throw new Exception("Error reading the MOV ECX, ESI instruction, reason: incorrect opcode");
            }

            MOV_ECX_ESI mov = new MOV_ECX_ESI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ECX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ECX_ESP)
            {
                throw new Exception("Error reading the MOV ECX, ESP instruction, reason: incorrect opcode");
            }

            MOV_ECX_ESP mov = new MOV_ECX_ESP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        #endregion
        #region MOV EDI
        public Instruction ReadMOV_EDI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDI_EAX)
            {
                throw new Exception("Error reading the MOV EDI, EAX instruction, reason: incorrect opcode");
            }

            MOV_EDI_EAX mov = new MOV_EDI_EAX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDI_EBP)
            {
                throw new Exception("Error reading the MOV EDI, EBP instruction, reason: incorrect opcode");
            }

            MOV_EDI_EBP mov = new MOV_EDI_EBP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDI_EBX)
            {
                throw new Exception("Error reading the MOV EAX, EBX instruction, reason: incorrect opcode");
            }

            MOV_EDI_EBX mov = new MOV_EDI_EBX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDI_ECX)
            {
                throw new Exception("Error reading the MOV EDI, ECX instruction, reason: incorrect opcode");
            }

            MOV_EDI_ECX mov = new MOV_EDI_ECX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDI_EDI)
            {
                throw new Exception("Error reading the MOV EDI, EDI instruction, reason: incorrect opcode");
            }

            MOV_EDI_EDI mov = new MOV_EDI_EDI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDI_EDX)
            {
                throw new Exception("Error reading the MOV EDI, EDX instruction, reason: incorrect opcode");
            }

            MOV_EDI_EDX mov = new MOV_EDI_EDX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDI_ESI)
            {
                throw new Exception("Error reading the MOV EDI, ESI instruction, reason: incorrect opcode");
            }

            MOV_EDI_ESI mov = new MOV_EDI_ESI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDI_ESP)
            {
                throw new Exception("Error reading the MOV EDI, ESP instruction, reason: incorrect opcode");
            }

            MOV_EDI_ESP mov = new MOV_EDI_ESP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        #endregion
        #region MOV EDX
        public Instruction ReadMOV_EDX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDX_EAX)
            {
                throw new Exception("Error reading the MOV EDX, EAX instruction, reason: incorrect opcode");
            }

            MOV_EDX_EAX mov = new MOV_EDX_EAX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDX_EBP)
            {
                throw new Exception("Error reading the MOV EDX, EBP instruction, reason: incorrect opcode");
            }

            MOV_EDX_EBP mov = new MOV_EDX_EBP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDX_EBX)
            {
                throw new Exception("Error reading the MOV EAX, EBX instruction, reason: incorrect opcode");
            }

            MOV_EDX_EBX mov = new MOV_EDX_EBX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDX_ECX)
            {
                throw new Exception("Error reading the MOV EDX, ECX instruction, reason: incorrect opcode");
            }

            MOV_EDX_ECX mov = new MOV_EDX_ECX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDX_EDI)
            {
                throw new Exception("Error reading the MOV EDX, EDX instruction, reason: incorrect opcode");
            }

            MOV_EDX_EDI mov = new MOV_EDX_EDI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDX_EDX)
            {
                throw new Exception("Error reading the MOV EDX, EDX instruction, reason: incorrect opcode");
            }

            MOV_EDX_EDX mov = new MOV_EDX_EDX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDX_ESI)
            {
                throw new Exception("Error reading the MOV EDX, ESI instruction, reason: incorrect opcode");
            }

            MOV_EDX_ESI mov = new MOV_EDX_ESI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_EDX_ESP)
            {
                throw new Exception("Error reading the MOV EDX, ESP instruction, reason: incorrect opcode");
            }

            MOV_EDX_ESP mov = new MOV_EDX_ESP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        #endregion
        #region MOV ESI
        public Instruction ReadMOV_ESI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESI_EAX)
            {
                throw new Exception("Error reading the MOV ESI, EAX instruction, reason: incorrect opcode");
            }

            MOV_ESI_EAX mov = new MOV_ESI_EAX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESI_EBP)
            {
                throw new Exception("Error reading the MOV ESI, EBP instruction, reason: incorrect opcode");
            }

            MOV_ESI_EBP mov = new MOV_ESI_EBP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESI_EBX)
            {
                throw new Exception("Error reading the MOV EAX, EBX instruction, reason: incorrect opcode");
            }

            MOV_ESI_EBX mov = new MOV_ESI_EBX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESI_ECX)
            {
                throw new Exception("Error reading the MOV ESI, ECX instruction, reason: incorrect opcode");
            }

            MOV_ESI_ECX mov = new MOV_ESI_ECX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESI_EDI)
            {
                throw new Exception("Error reading the MOV ESI, EDI instruction, reason: incorrect opcode");
            }

            MOV_ESI_EDI mov = new MOV_ESI_EDI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESI_EDX)
            {
                throw new Exception("Error reading the MOV ESI, EDX instruction, reason: incorrect opcode");
            }

            MOV_ESI_EDX mov = new MOV_ESI_EDX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESI_ESI)
            {
                throw new Exception("Error reading the MOV ESI, ESI instruction, reason: incorrect opcode");
            }

            MOV_ESI_ESI mov = new MOV_ESI_ESI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESI_ESP)
            {
                throw new Exception("Error reading the MOV ESI, ESP instruction, reason: incorrect opcode");
            }

            MOV_ESI_ESP mov = new MOV_ESI_ESP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        #endregion
        #region MOV ESP
        public Instruction ReadMOV_ESP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESP_EAX)
            {
                throw new Exception("Error reading the MOV ESP, EAX instruction, reason: incorrect opcode");
            }

            MOV_ESP_EAX mov = new MOV_ESP_EAX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESP_EBP)
            {
                throw new Exception("Error reading the MOV ESP, EBP instruction, reason: incorrect opcode");
            }

            MOV_ESP_EBP mov = new MOV_ESP_EBP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESP_EBX)
            {
                throw new Exception("Error reading the MOV EAX, EBX instruction, reason: incorrect opcode");
            }

            MOV_ESP_EBX mov = new MOV_ESP_EBX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESP_ECX)
            {
                throw new Exception("Error reading the MOV ESP, ECX instruction, reason: incorrect opcode");
            }

            MOV_ESP_ECX mov = new MOV_ESP_ECX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESP_EDI)
            {
                throw new Exception("Error reading the MOV ESP, EDI instruction, reason: incorrect opcode");
            }

            MOV_ESP_EDI mov = new MOV_ESP_EDI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESP_EDX)
            {
                throw new Exception("Error reading the MOV ESP, EDX instruction, reason: incorrect opcode");
            }

            MOV_ESP_EDX mov = new MOV_ESP_EDX();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESP_ESI)
            {
                throw new Exception("Error reading the MOV ESP, ESI instruction, reason: incorrect opcode");
            }

            MOV_ESP_ESI mov = new MOV_ESP_ESI();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_REGISTER &&
                InstructionData[offset + 1] != (byte)MovRegisterOpcodes.MOV_ESP_ESP)
            {
                throw new Exception("Error reading the MOV ESP, ESP instruction, reason: incorrect opcode");
            }

            MOV_ESP_ESP mov = new MOV_ESP_ESP();
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        #endregion

        #region MUL EAX
        public Instruction ReadMUL_EAX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EAX_EAX)
            {
                throw new Exception("Error reading the MUL EAX, EAX instruction, reason: incorrect opcode");
            }

            MUL_EAX_EAX xor = new MUL_EAX_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EAX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EAX_EBP)
            {
                throw new Exception("Error reading the MUL EAX, EBP instruction, reason: incorrect opcode");
            }

            MUL_EAX_EBP xor = new MUL_EAX_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EAX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EAX_EBX)
            {
                throw new Exception("Error reading the MUL EAX, EBX instruction, reason: incorrect opcode");
            }

            MUL_EAX_EBX xor = new MUL_EAX_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EAX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EAX_ECX)
            {
                throw new Exception("Error reading the MUL EAX, ECX instruction, reason: incorrect opcode");
            }

            MUL_EAX_ECX xor = new MUL_EAX_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EAX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EAX_EDI)
            {
                throw new Exception("Error reading the MUL EAX, EDI instruction, reason: incorrect opcode");
            }

            MUL_EAX_EDI xor = new MUL_EAX_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EAX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EAX_EDX)
            {
                throw new Exception("Error reading the MUL EAX, EDX instruction, reason: incorrect opcode");
            }

            MUL_EAX_EDX xor = new MUL_EAX_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EAX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EAX_ESI)
            {
                throw new Exception("Error reading the MUL EAX, ESI instruction, reason: incorrect opcode");
            }

            MUL_EAX_ESI xor = new MUL_EAX_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EAX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EAX_ESP)
            {
                throw new Exception("Error reading the MUL EAX, ESP instruction, reason: incorrect opcode");
            }

            MUL_EAX_ESP xor = new MUL_EAX_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region MUL EBP
        public Instruction ReadMUL_EBP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBP_EAX)
            {
                throw new Exception("Error reading the MUL EBP, EAX instruction, reason: incorrect opcode");
            }

            MUL_EBP_EAX xor = new MUL_EBP_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBP_EBP)
            {
                throw new Exception("Error reading the MUL EBP, EBP instruction, reason: incorrect opcode");
            }

            MUL_EBP_EBP xor = new MUL_EBP_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBP_EBX)
            {
                throw new Exception("Error reading the MUL EBP, EBX instruction, reason: incorrect opcode");
            }

            MUL_EBP_EBX xor = new MUL_EBP_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBP_ECX)
            {
                throw new Exception("Error reading the MUL EBP, ECX instruction, reason: incorrect opcode");
            }

            MUL_EBP_ECX xor = new MUL_EBP_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBP_EDI)
            {
                throw new Exception("Error reading the MUL EBP, EDI instruction, reason: incorrect opcode");
            }

            MUL_EBP_EDI xor = new MUL_EBP_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBP_EDX)
            {
                throw new Exception("Error reading the MUL EBP, EDX instruction, reason: incorrect opcode");
            }

            MUL_EBP_EDX xor = new MUL_EBP_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBP_ESI)
            {
                throw new Exception("Error reading the MUL EBP, ESI instruction, reason: incorrect opcode");
            }

            MUL_EBP_ESI xor = new MUL_EBP_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBP_ESP)
            {
                throw new Exception("Error reading the MUL EBP, ESP instruction, reason: incorrect opcode");
            }

            MUL_EBP_ESP xor = new MUL_EBP_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region MUL EBX
        public Instruction ReadMUL_EBX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBX_EAX)
            {
                throw new Exception("Error reading the MUL EBX, EAX instruction, reason: incorrect opcode");
            }

            MUL_EBX_EAX xor = new MUL_EBX_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBX_EBP)
            {
                throw new Exception("Error reading the MUL EBX, EBX instruction, reason: incorrect opcode");
            }

            MUL_EBX_EBP xor = new MUL_EBX_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBX_EBX)
            {
                throw new Exception("Error reading the MUL EAX, EBX instruction, reason: incorrect opcode");
            }

            MUL_EBX_EBX xor = new MUL_EBX_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBX_ECX)
            {
                throw new Exception("Error reading the MUL EBX, ECX instruction, reason: incorrect opcode");
            }

            MUL_EBX_ECX xor = new MUL_EBX_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBX_EDI)
            {
                throw new Exception("Error reading the MUL EBX, EDI instruction, reason: incorrect opcode");
            }

            MUL_EBX_EDI xor = new MUL_EBX_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBX_EDX)
            {
                throw new Exception("Error reading the MUL EBX, EDX instruction, reason: incorrect opcode");
            }

            MUL_EBX_EDX xor = new MUL_EBX_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBX_ESI)
            {
                throw new Exception("Error reading the MUL EBX, ESI instruction, reason: incorrect opcode");
            }

            MUL_EBX_ESI xor = new MUL_EBX_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EBX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EBX_ESP)
            {
                throw new Exception("Error reading the MUL EBX, ESP instruction, reason: incorrect opcode");
            }

            MUL_EBX_ESP xor = new MUL_EBX_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region MUL ECX
        public Instruction ReadMUL_ECX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ECX_EAX)
            {
                throw new Exception("Error reading the MUL ECX, EAX instruction, reason: incorrect opcode");
            }

            MUL_ECX_EAX xor = new MUL_ECX_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_ECX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ECX_EBP)
            {
                throw new Exception("Error reading the MUL ECX, EBX instruction, reason: incorrect opcode");
            }

            MUL_ECX_EBP xor = new MUL_ECX_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_ECX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ECX_EBX)
            {
                throw new Exception("Error reading the MUL ECX, EBX instruction, reason: incorrect opcode");
            }

            MUL_ECX_EBX xor = new MUL_ECX_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_ECX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ECX_ECX)
            {
                throw new Exception("Error reading the MUL ECX, ECX instruction, reason: incorrect opcode");
            }

            MUL_ECX_ECX xor = new MUL_ECX_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_ECX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ECX_EDI)
            {
                throw new Exception("Error reading the MUL ECX, EDI instruction, reason: incorrect opcode");
            }

            MUL_ECX_EDI xor = new MUL_ECX_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_ECX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ECX_EDX)
            {
                throw new Exception("Error reading the MUL ECX, EDX instruction, reason: incorrect opcode");
            }

            MUL_ECX_EDX xor = new MUL_ECX_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_ECX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ECX_ESI)
            {
                throw new Exception("Error reading the MUL ECX, ESI instruction, reason: incorrect opcode");
            }

            MUL_ECX_ESI xor = new MUL_ECX_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_ECX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ECX_ESP)
            {
                throw new Exception("Error reading the MUL ECX, ESP instruction, reason: incorrect opcode");
            }

            MUL_ECX_ESP xor = new MUL_ECX_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region MUL EDI
        public Instruction ReadMUL_EDI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDI_EAX)
            {
                throw new Exception("Error reading the MUL EDI, EAX instruction, reason: incorrect opcode");
            }

            MUL_EDI_EAX xor = new MUL_EDI_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDI_EBP)
            {
                throw new Exception("Error reading the MUL EDI, EBP instruction, reason: incorrect opcode");
            }

            MUL_EDI_EBP xor = new MUL_EDI_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDI_EBX)
            {
                throw new Exception("Error reading the MUL EAX, EBX instruction, reason: incorrect opcode");
            }

            MUL_EDI_EBX xor = new MUL_EDI_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDI_ECX)
            {
                throw new Exception("Error reading the MUL EDI, ECX instruction, reason: incorrect opcode");
            }

            MUL_EDI_ECX xor = new MUL_EDI_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDI_EDI)
            {
                throw new Exception("Error reading the MUL EDI, EDI instruction, reason: incorrect opcode");
            }

            MUL_EDI_EDI xor = new MUL_EDI_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDI_EDX)
            {
                throw new Exception("Error reading the MUL EDI, EDX instruction, reason: incorrect opcode");
            }

            MUL_EDI_EDX xor = new MUL_EDI_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDI_ESI)
            {
                throw new Exception("Error reading the MUL EDI, ESI instruction, reason: incorrect opcode");
            }

            MUL_EDI_ESI xor = new MUL_EDI_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDI_ESP)
            {
                throw new Exception("Error reading the MUL EDI, ESP instruction, reason: incorrect opcode");
            }

            MUL_EDI_ESP xor = new MUL_EDI_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region MUL EDX
        public Instruction ReadMUL_EDX_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDX_EAX)
            {
                throw new Exception("Error reading the MUL EDX, EAX instruction, reason: incorrect opcode");
            }

            MUL_EDX_EAX xor = new MUL_EDX_EAX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDX_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDX_EBP)
            {
                throw new Exception("Error reading the MUL EDX, EBP instruction, reason: incorrect opcode");
            }

            MUL_EDX_EBP xor = new MUL_EDX_EBP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDX_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDX_EBX)
            {
                throw new Exception("Error reading the MUL EAX, EBX instruction, reason: incorrect opcode");
            }

            MUL_EDX_EBX xor = new MUL_EDX_EBX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDX_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDX_ECX)
            {
                throw new Exception("Error reading the MUL EDX, ECX instruction, reason: incorrect opcode");
            }

            MUL_EDX_ECX xor = new MUL_EDX_ECX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDX_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDX_EDI)
            {
                throw new Exception("Error reading the MUL EDX, EDX instruction, reason: incorrect opcode");
            }

            MUL_EDX_EDI xor = new MUL_EDX_EDI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDX_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDX_EDX)
            {
                throw new Exception("Error reading the MUL EDX, EDX instruction, reason: incorrect opcode");
            }

            MUL_EDX_EDX xor = new MUL_EDX_EDX();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDX_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDX_ESI)
            {
                throw new Exception("Error reading the MUL EDX, ESI instruction, reason: incorrect opcode");
            }

            MUL_EDX_ESI xor = new MUL_EDX_ESI();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        public Instruction ReadMUL_EDX_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_EDX_ESP)
            {
                throw new Exception("Error reading the MUL EDX, ESP instruction, reason: incorrect opcode");
            }

            MUL_EDX_ESP xor = new MUL_EDX_ESP();
            xor.VirtualAddress.Address = MemAddress;
            offset += xor.VirtualAddress.Size;
            MemAddress += xor.VirtualAddress.Size;
            Instructions.Add(xor.VirtualAddress.Address, xor);
            return xor;
        }
        #endregion
        #region MUL ESI
        public Instruction ReadMUL_ESI_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESI_EAX)
            {
                throw new Exception("Error reading the MUL ESI, EAX instruction, reason: incorrect opcode");
            }

            MUL_ESI_EAX mul = new MUL_ESI_EAX();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESI_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESI_EBP)
            {
                throw new Exception("Error reading the MUL ESI, EBP instruction, reason: incorrect opcode");
            }

            MUL_ESI_EBP mul = new MUL_ESI_EBP();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESI_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESI_EBX)
            {
                throw new Exception("Error reading the MUL EAX, EBX instruction, reason: incorrect opcode");
            }

            MUL_ESI_EBX mul = new MUL_ESI_EBX();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESI_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESI_ECX)
            {
                throw new Exception("Error reading the MUL ESI, ECX instruction, reason: incorrect opcode");
            }

            MUL_ESI_ECX mul = new MUL_ESI_ECX();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESI_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESI_EDI)
            {
                throw new Exception("Error reading the MUL ESI, EDI instruction, reason: incorrect opcode");
            }

            MUL_ESI_EDI mul = new MUL_ESI_EDI();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESI_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESI_EDX)
            {
                throw new Exception("Error reading the MUL ESI, EDX instruction, reason: incorrect opcode");
            }

            MUL_ESI_EDX mul = new MUL_ESI_EDX();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESI_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESI_ESI)
            {
                throw new Exception("Error reading the MUL ESI, ESI instruction, reason: incorrect opcode");
            }

            MUL_ESI_ESI mul = new MUL_ESI_ESI();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESI_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESI_ESP)
            {
                throw new Exception("Error reading the MUL ESI, ESP instruction, reason: incorrect opcode");
            }

            MUL_ESI_ESP mul = new MUL_ESI_ESP();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        #endregion
        #region MUL ESP
        public Instruction ReadMUL_ESP_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESP_EAX)
            {
                throw new Exception("Error reading the MUL ESP, EAX instruction, reason: incorrect opcode");
            }

            MUL_ESP_EAX mul = new MUL_ESP_EAX();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESP_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESP_EBP)
            {
                throw new Exception("Error reading the MUL ESP, EBP instruction, reason: incorrect opcode");
            }

            MUL_ESP_EBP mul = new MUL_ESP_EBP();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESP_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESP_EBX)
            {
                throw new Exception("Error reading the MUL EAX, EBX instruction, reason: incorrect opcode");
            }

            MUL_ESP_EBX mul = new MUL_ESP_EBX();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESP_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESP_ECX)
            {
                throw new Exception("Error reading the MUL ESP, ECX instruction, reason: incorrect opcode");
            }

            MUL_ESP_ECX mul = new MUL_ESP_ECX();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESP_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESP_EDI)
            {
                throw new Exception("Error reading the MUL ESP, ESP instruction, reason: incorrect opcode");
            }

            MUL_ESP_EDI mul = new MUL_ESP_EDI();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESP_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESP_EDI)
            {
                throw new Exception("Error reading the MUL ESP, EDX instruction, reason: incorrect opcode");
            }

            MUL_ESP_EDX mul = new MUL_ESP_EDX();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESP_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESP_EDI)
            {
                throw new Exception("Error reading the MUL ESP, ESI instruction, reason: incorrect opcode");
            }

            MUL_ESP_ESI mul = new MUL_ESP_ESI();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        public Instruction ReadMUL_ESP_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MUL &&
                InstructionData[offset + 1] != (byte)MulRegisterOpcodes.MUL_ESP_ESP)
            {
                throw new Exception("Error reading the MUL ESP, ESP instruction, reason: incorrect opcode");
            }

            MUL_ESP_ESP mul = new MUL_ESP_ESP();
            mul.VirtualAddress.Address = MemAddress;
            offset += mul.VirtualAddress.Size;
            MemAddress += mul.VirtualAddress.Size;
            Instructions.Add(mul.VirtualAddress.Address, mul);
            return mul;
        }
        #endregion

        #region INC
        public Instruction ReadINC_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.INC_EAX)
                throw new Exception("Error reading the INC EAX instruction, reason: incorrect opcode");
            INC_EAX inc = new INC_EAX();
            inc.VirtualAddress.Address = MemAddress;
            offset += inc.VirtualAddress.Size;
            MemAddress += inc.VirtualAddress.Size;
            Instructions.Add(inc.VirtualAddress.Address, inc);
            return inc;
        }
        public Instruction ReadINC_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.INC_EBP)
                throw new Exception("Error reading the INC EBP instruction, reason: incorrect opcode");
            INC_EBP inc = new INC_EBP();
            inc.VirtualAddress.Address = MemAddress;
            offset += inc.VirtualAddress.Size;
            MemAddress += inc.VirtualAddress.Size;
            Instructions.Add(inc.VirtualAddress.Address, inc);
            return inc;
        }
        public Instruction ReadINC_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.INC_EBX)
                throw new Exception("Error reading the INC EBX instruction, reason: incorrect opcode");
            INC_EBX inc = new INC_EBX();
            inc.VirtualAddress.Address = MemAddress;
            offset += inc.VirtualAddress.Size;
            MemAddress += inc.VirtualAddress.Size;
            Instructions.Add(inc.VirtualAddress.Address, inc);
            return inc;
        }
        public Instruction ReadINC_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.INC_ECX)
                throw new Exception("Error reading the INC ECX instruction, reason: incorrect opcode");
            INC_ECX inc = new INC_ECX();
            inc.VirtualAddress.Address = MemAddress;
            offset += inc.VirtualAddress.Size;
            MemAddress += inc.VirtualAddress.Size;
            Instructions.Add(inc.VirtualAddress.Address, inc);
            return inc;
        }
        public Instruction ReadINC_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.INC_EDI)
                throw new Exception("Error reading the INC EDI instruction, reason: incorrect opcode");
            INC_EDI inc = new INC_EDI();
            inc.VirtualAddress.Address = MemAddress;
            offset += inc.VirtualAddress.Size;
            MemAddress += inc.VirtualAddress.Size;
            Instructions.Add(inc.VirtualAddress.Address, inc);
            return inc;
        }
        public Instruction ReadINC_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.INC_EDX)
                throw new Exception("Error reading the INC EDX instruction, reason: incorrect opcode");
            INC_EDX inc = new INC_EDX();
            inc.VirtualAddress.Address = MemAddress;
            offset += inc.VirtualAddress.Size;
            MemAddress += inc.VirtualAddress.Size;
            Instructions.Add(inc.VirtualAddress.Address, inc);
            return inc;
        }
        public Instruction ReadINC_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.INC_ESI)
                throw new Exception("Error reading the INC ESI instruction, reason: incorrect opcode");
            INC_ESI inc = new INC_ESI();
            inc.VirtualAddress.Address = MemAddress;
            offset += inc.VirtualAddress.Size;
            MemAddress += inc.VirtualAddress.Size;
            Instructions.Add(inc.VirtualAddress.Address, inc);
            return inc;
        }
        public Instruction ReadINC_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.INC_ESP)
                throw new Exception("Error reading the INC ESP instruction, reason: incorrect opcode");
            INC_ESP inc = new INC_ESP();
            inc.VirtualAddress.Address = MemAddress;
            offset += inc.VirtualAddress.Size;
            MemAddress += inc.VirtualAddress.Size;
            Instructions.Add(inc.VirtualAddress.Address, inc);
            return inc;
        }
        #endregion
        #region MOV
        public Instruction ReadMOV_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_EAX)
                throw new Exception("Error reading the MOV EAX instruction, reason: incorrect opcode");
            MOV_EAX mov = new MOV_EAX(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_EBP)
                throw new Exception("Error reading the MOV EBP instruction, reason: incorrect opcode");
            MOV_EBP mov = new MOV_EBP(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_EBX)
                throw new Exception("Error reading the MOV EBX instruction, reason: incorrect opcode");
            MOV_EBX mov = new MOV_EBX(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_ECX)
                throw new Exception("Error reading the MOV ECX instruction, reason: incorrect opcode");
            MOV_ECX mov = new MOV_ECX(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_EDI)
                throw new Exception("Error reading the MOV EDI instruction, reason: incorrect opcode");
            MOV_EDI mov = new MOV_EDI(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_EDX)
                throw new Exception("Error reading the MOV EDX instruction, reason: incorrect opcode");
            MOV_EDX mov = new MOV_EDX(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_ESI)
                throw new Exception("Error reading the MOV ESI instruction, reason: incorrect opcode");
            MOV_ESI mov = new MOV_ESI(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_ESP)
                throw new Exception("Error reading the MOV ESP instruction, reason: incorrect opcode");
            MOV_ESP mov = new MOV_ESP(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_DWORD_PTR_EAX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_DWORD_PTR_EAX)
                throw new Exception("Error reading the MOV_DWORD_PTR_EAX instruction, reason: incorrect opcode");
            MOV_DWORD_PTR_EAX mov = new MOV_DWORD_PTR_EAX(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 1)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_DWORD_PTR_EBP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_DWORD_PTR_REGISTER ||
                InstructionData[offset+1] != (byte)OpcodeList.MOV_DWORD_PTR_EBP)
                throw new Exception("Error reading the MOV DWORD PTR[...], EBP instruction, reason: incorrect opcode");
            MOV_DWORD_PTR_EBP mov = new MOV_DWORD_PTR_EBP(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 2)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_DWORD_PTR_EBX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_DWORD_PTR_REGISTER ||
                InstructionData[offset+1] != (byte)OpcodeList.MOV_DWORD_PTR_EBX)
                throw new Exception("Error reading the MOV DWORD PTR[...], EBX instruction, reason: incorrect opcode");
            MOV_DWORD_PTR_EBX mov = new MOV_DWORD_PTR_EBX(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 2)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_DWORD_PTR_ECX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_DWORD_PTR_REGISTER ||
                InstructionData[offset+1] != (byte)OpcodeList.MOV_DWORD_PTR_ECX)
                throw new Exception("Error reading the MOV DWORD PTR[...], ECX instruction, reason: incorrect opcode");
            MOV_DWORD_PTR_ECX mov = new MOV_DWORD_PTR_ECX(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 2)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_DWORD_PTR_EDI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_DWORD_PTR_REGISTER ||
                InstructionData[offset+1] != (byte)OpcodeList.MOV_DWORD_PTR_EDI)
                throw new Exception("Error reading the MOV DWORD PTR[...], EDI instruction, reason: incorrect opcode");
            MOV_DWORD_PTR_EDI mov = new MOV_DWORD_PTR_EDI(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 2)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_DWORD_PTR_EDX()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_DWORD_PTR_REGISTER ||
                InstructionData[offset+1] != (byte)OpcodeList.MOV_DWORD_PTR_EDX)
                throw new Exception("Error reading the MOV DWORD PTR[...], EDX instruction, reason: incorrect opcode");
            MOV_DWORD_PTR_EDX mov = new MOV_DWORD_PTR_EDX(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 2)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_DWORD_PTR_ESI()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_DWORD_PTR_REGISTER ||
                InstructionData[offset+1] != (byte)OpcodeList.MOV_DWORD_PTR_ESI)
                throw new Exception("Error reading the MOV DWORD PTR[...], ESI instruction, reason: incorrect opcode");
            MOV_DWORD_PTR_ESI mov = new MOV_DWORD_PTR_ESI(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 2)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_DWORD_PTR_ESP()
        {
            if (InstructionData[offset] != (byte)OpcodeList.MOV_DWORD_PTR_REGISTER ||
                InstructionData[offset+1] != (byte)OpcodeList.MOV_DWORD_PTR_ESP)
                throw new Exception("Error reading the MOV DWORD PTR[...], ESP instruction, reason: incorrect opcode");
            MOV_DWORD_PTR_ESP mov = new MOV_DWORD_PTR_ESP(new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 2)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }




        public Instruction ReadMOV_REGISTER_DWORD_PTR()
        {
            if (InstructionData[offset] != OtherOpcodes.MOV_EAX_DWORD_PTR[0])
                throw new Exception("Error reading the MOV_REGISTER_DWORD_PTR instruction, reason: incorrect opcode");

            Register register = Register.EAX;
            if (OtherOpcodes.MOV_EAX_DWORD_PTR[1] == InstructionData[offset+1])
                register = Register.EAX;
            else if (OtherOpcodes.MOV_ECX_DWORD_PTR[1] == InstructionData[offset+1])
                register = Register.ECX;
            else if (OtherOpcodes.MOV_EBP_DWORD_PTR[1] == InstructionData[offset+1])
                register = Register.EBP;
            else if (OtherOpcodes.MOV_EBX_DWORD_PTR[1] == InstructionData[offset+1])
                register = Register.EBX;
            else if (OtherOpcodes.MOV_EDI_DWORD_PTR[1] == InstructionData[offset+1])
                register = Register.EDI;
            else if (OtherOpcodes.MOV_EDX_DWORD_PTR[1] == InstructionData[offset+1])
                register = Register.EDX;
            else if (OtherOpcodes.MOV_ESI_DWORD_PTR[1] == InstructionData[offset+1])
                register = Register.ESI;
            else if (OtherOpcodes.MOV_ESP_DWORD_PTR[1] == InstructionData[offset+1])
                register = Register.ESP;

            MOV_REGISTER_DWORD_PTR mov = new MOV_REGISTER_DWORD_PTR(register, new VirtualAddress(4, BitConverter.ToInt32(InstructionData, offset + 2)));
            mov.VirtualAddress.Address = MemAddress;
            offset += mov.VirtualAddress.Size;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_VARIABLE_VALUE()
        {
            if (InstructionData[offset] != OtherOpcodes.MOV_VARIABLE_VALUE[0] ||
                InstructionData[offset+1] != OtherOpcodes.MOV_VARIABLE_VALUE[1] ||
                InstructionData[offset+2] != OtherOpcodes.MOV_VARIABLE_VALUE[2])
            {
                throw new Exception("Error reading the MOV_VARIABLE_VALUE instruction, reason: incorrect opcode");
            }

            PayloadReader pr = new PayloadReader(InstructionData);
            pr.ReadBytes(offset + 3);
            VirtualAddress VarAddr = new VirtualAddress(4, pr.ReadInteger());
            string varName = pr.ReadString();

            bool isRegister = pr.ReadByte() == 1;
            MOV_VARIABLE_VALUE mov = null;
            if (isRegister)
            {
                mov = new MOV_VARIABLE_VALUE(VarAddr, varName, (Register)pr.ReadByte());
                mov.isRegister = true;
                offset += mov.VirtualAddress.Size + ((varName.Length * 2) + 8);
            }
            else
            {
                int length = pr.ReadInteger();
                object newValue = new BinaryFormatter().Deserialize(new MemoryStream(pr.ReadBytes(length)));
                mov = new MOV_VARIABLE_VALUE(VarAddr, varName, newValue);
                offset += mov.VirtualAddress.Size + ((varName.Length * 2) + 11 + length);
            }
            
            mov.VirtualAddress.Address = MemAddress;
            MemAddress += mov.VirtualAddress.Size;
            Instructions.Add(mov.VirtualAddress.Address, mov);
            return mov;
        }
        public Instruction ReadMOV_VARIABLE_INDEX_VALUE()
        {
            if (InstructionData[offset] != OtherOpcodes.MOV_VARIABLE_INDEX_VALUE[0] ||
                InstructionData[offset+1] != OtherOpcodes.MOV_VARIABLE_INDEX_VALUE[1] ||
                InstructionData[offset+2] != OtherOpcodes.MOV_VARIABLE_INDEX_VALUE[2])
            {
                throw new Exception("Error reading the MOV_VARIABLE_VALUE instruction, reason: incorrect opcode");
            }

            PayloadReader pr = new PayloadReader(InstructionData);
            pr.ReadBytes(offset + 3);
            VirtualAddress VarAddr = new VirtualAddress(4, pr.ReadInteger());
            bool hasVarName = pr.ReadByte() == 1;
            bool isRegister = pr.ReadByte() == 1;
            int index = pr.ReadInteger();
            MOV_VARIABLE_INDEX_VALUE mov = null;

            if (hasVarName)
            {
                string varName = pr.ReadString();
                
                if (isRegister)
                {
                    mov = new MOV_VARIABLE_INDEX_VALUE(VarAddr, varName, index, (Register)pr.ReadByte());
                    mov.isRegister = true;
                    offset += mov.VirtualAddress.Size + ((varName.Length * 2) + 13);
                }
                else
                {
                    int length = pr.ReadInteger();
                    object newValue = new BinaryFormatter().Deserialize(new MemoryStream(pr.ReadBytes(length)));
                    mov = new MOV_VARIABLE_INDEX_VALUE(VarAddr, varName, index, newValue);
                    offset += mov.VirtualAddress.Size + ((varName.Length * 2) + 16 + length);
                }

                mov.VirtualAddress.Address = MemAddress;
                MemAddress += mov.VirtualAddress.Size;
                Instructions.Add(mov.VirtualAddress.Address, mov);
                return mov;
            }
            else
            {
                if (isRegister)
                {
                    mov = new MOV_VARIABLE_INDEX_VALUE(VarAddr, index, (Register)pr.ReadByte());
                    mov.isRegister = true;
                    offset += mov.VirtualAddress.Size + 11;
                }
                else
                {
                    int length = pr.ReadInteger();
                    object newValue = new BinaryFormatter().Deserialize(new MemoryStream(pr.ReadBytes(length)));
                    mov = new MOV_VARIABLE_INDEX_VALUE(VarAddr, index, newValue);
                    offset += mov.VirtualAddress.Size +  14 + length;
                }
                mov.VirtualAddress.Address = MemAddress;
                MemAddress += mov.VirtualAddress.Size;
                Instructions.Add(mov.VirtualAddress.Address, mov);
                return mov;
            }
        }
        #endregion

        public Instruction[] ReadAllInstructions()
        {
            List<Instruction> instructions = new List<Instruction>();
            offset = 0; //reset offset
            while (offset != InstructionData.Length)
            {
                instructions.Add(ReadNextInstruction());
            }
            return instructions.ToArray();
        }

        public Instruction ReadNextInstruction()
        {
            switch (InstructionData[offset])
            {
                case 0: { return ReadADD_BYTE_PTR_EAX_AL(); }
                case (byte)OpcodeList.NOP: { return ReadNOP(); }
                case (byte)OpcodeList.JMP: { return ReadJMP(); }
                case (byte)OpcodeList.JE: { return ReadJE(); }
                case (byte)OpcodeList.JNE: { return ReadJNE(); }
                    
                case (byte)OpcodeList.INC_EAX: { return ReadINC_EAX(); }
                case (byte)OpcodeList.INC_EBP: { return ReadINC_EBP(); }
                case (byte)OpcodeList.INC_EBX: { return ReadINC_EBX(); }
                case (byte)OpcodeList.INC_ECX: { return ReadINC_ECX(); }
                case (byte)OpcodeList.INC_EDI: { return ReadINC_EDI(); }
                case (byte)OpcodeList.INC_EDX: { return ReadINC_EDX(); }
                case (byte)OpcodeList.INC_ESI: { return ReadINC_ESI(); }
                case (byte)OpcodeList.INC_ESP: { return ReadINC_ESP(); }
                    
                case (byte)OpcodeList.MOV_DWORD_PTR_EAX: { return ReadMOV_DWORD_PTR_EAX(); }
                case (byte)OpcodeList.MOV_EAX: { return ReadMOV_EAX(); }
                case (byte)OpcodeList.MOV_EBP: { return ReadMOV_EBP(); }
                case (byte)OpcodeList.MOV_EBX: { return ReadMOV_EBX(); }
                case (byte)OpcodeList.MOV_ECX: { return ReadMOV_ECX(); }
                case (byte)OpcodeList.MOV_EDI: { return ReadMOV_EDI(); }
                case (byte)OpcodeList.MOV_EDX: { return ReadMOV_EDX(); }
                case (byte)OpcodeList.MOV_ESI: { return ReadMOV_ESI(); }
                case (byte)OpcodeList.MOV_ESP: { return ReadMOV_ESP(); }

                case (byte)OpcodeList.RET: { return ReadRET(); }
                case (byte)OpcodeList.CALL: { return ReadCALL(); }
                case (byte)OpcodeList.PUSH_EAX: { return ReadPUSH_EAX(); }
                case (byte)OpcodeList.PUSH_EBP: { return ReadPUSH_EBP(); }
                case (byte)OpcodeList.PUSH_EBX: { return ReadPUSH_EBX(); }
                case (byte)OpcodeList.PUSH_ECX: { return ReadPUSH_ECX(); }
                case (byte)OpcodeList.PUSH_EDI: { return ReadPUSH_EDI(); }
                case (byte)OpcodeList.PUSH_EDX: { return ReadPUSH_EDX(); }
                case (byte)OpcodeList.PUSH_ESI: { return ReadPUSH_ESI(); }
                case (byte)OpcodeList.PUSH_ESP: { return ReadPUSH_ESP(); }
                case (byte)OpcodeList.PUSH_VALUE: { return ReadPUSH_VALUE(); }
                case (byte)OpcodeList.PUSH_VARIABLE: { return ReadPUSH_VARIABLE(); }

                case (byte)OpcodeList.MOV_DWORD_PTR_REGISTER:
                {
                    switch (InstructionData[offset + 1])
                    {
                        case (byte)OpcodeList.MOV_DWORD_PTR_EBP: { return ReadMOV_DWORD_PTR_EBP(); }
                        case (byte)OpcodeList.MOV_DWORD_PTR_EBX: { return ReadMOV_DWORD_PTR_EBX(); }
                        case (byte)OpcodeList.MOV_DWORD_PTR_ECX: { return ReadMOV_DWORD_PTR_ECX(); }
                        case (byte)OpcodeList.MOV_DWORD_PTR_EDI: { return ReadMOV_DWORD_PTR_EDI(); }
                        case (byte)OpcodeList.MOV_DWORD_PTR_EDX: { return ReadMOV_DWORD_PTR_EDX(); }
                        case (byte)OpcodeList.MOV_DWORD_PTR_ESI: { return ReadMOV_DWORD_PTR_ESI(); }
                        case (byte)OpcodeList.MOV_DWORD_PTR_ESP: { return ReadMOV_DWORD_PTR_ESP(); }
                    }
                    break;
                }

                case (byte)OpcodeList.XOR_REGISTER:
                {
                    switch (InstructionData[offset+1])
                    {
                        #region XOR
                        case (byte)XorRegisterOpcodes.XOR_EAX_EAX: { return ReadXOR_EAX_EAX(); }
                        case (byte)XorRegisterOpcodes.XOR_EAX_EBP: { return ReadXOR_EAX_EBP(); }
                        case (byte)XorRegisterOpcodes.XOR_EAX_EBX: { return ReadXOR_EAX_EBX(); }
                        case (byte)XorRegisterOpcodes.XOR_EAX_ECX: { return ReadXOR_EAX_ECX(); }
                        case (byte)XorRegisterOpcodes.XOR_EAX_EDI: { return ReadXOR_EAX_EDI(); }
                        case (byte)XorRegisterOpcodes.XOR_EAX_EDX: { return ReadXOR_EAX_EDX(); }
                        case (byte)XorRegisterOpcodes.XOR_EAX_ESI: { return ReadXOR_EAX_ESI(); }
                        case (byte)XorRegisterOpcodes.XOR_EAX_ESP: { return ReadXOR_EAX_ESP(); }
                        case (byte)XorRegisterOpcodes.XOR_EBP_EAX: { return ReadXOR_EBP_EAX(); }
                        case (byte)XorRegisterOpcodes.XOR_EBP_EBP: { return ReadXOR_EBP_EBP(); }
                        case (byte)XorRegisterOpcodes.XOR_EBP_EBX: { return ReadXOR_EBP_EBX(); }
                        case (byte)XorRegisterOpcodes.XOR_EBP_ECX: { return ReadXOR_EBP_ECX(); }
                        case (byte)XorRegisterOpcodes.XOR_EBP_EDI: { return ReadXOR_EBP_EDI(); }
                        case (byte)XorRegisterOpcodes.XOR_EBP_EDX: { return ReadXOR_EBP_EDX(); }
                        case (byte)XorRegisterOpcodes.XOR_EBP_ESI: { return ReadXOR_EBP_ESI(); }
                        case (byte)XorRegisterOpcodes.XOR_EBP_ESP: { return ReadXOR_EBP_ESP(); }
                        case (byte)XorRegisterOpcodes.XOR_EBX_EAX: { return ReadXOR_EBX_EAX(); }
                        case (byte)XorRegisterOpcodes.XOR_EBX_EBP: { return ReadXOR_EBX_EBP(); }
                        case (byte)XorRegisterOpcodes.XOR_EBX_EBX: { return ReadXOR_EBX_EBX(); }
                        case (byte)XorRegisterOpcodes.XOR_EBX_ECX: { return ReadXOR_EBX_ECX(); }
                        case (byte)XorRegisterOpcodes.XOR_EBX_EDI: { return ReadXOR_EBX_EDI(); }
                        case (byte)XorRegisterOpcodes.XOR_EBX_EDX: { return ReadXOR_EBX_EDX(); }
                        case (byte)XorRegisterOpcodes.XOR_EBX_ESI: { return ReadXOR_EBX_ESI(); }
                        case (byte)XorRegisterOpcodes.XOR_EBX_ESP: { return ReadXOR_EBX_ESP(); }
                        case (byte)XorRegisterOpcodes.XOR_ECX_EAX: { return ReadXOR_ECX_EAX(); }
                        case (byte)XorRegisterOpcodes.XOR_ECX_EBP: { return ReadXOR_ECX_EBP(); }
                        case (byte)XorRegisterOpcodes.XOR_ECX_EBX: { return ReadXOR_ECX_EBX(); }
                        case (byte)XorRegisterOpcodes.XOR_ECX_ECX: { return ReadXOR_ECX_ECX(); }
                        case (byte)XorRegisterOpcodes.XOR_ECX_EDI: { return ReadXOR_ECX_EDI(); }
                        case (byte)XorRegisterOpcodes.XOR_ECX_EDX: { return ReadXOR_ECX_EDX(); }
                        case (byte)XorRegisterOpcodes.XOR_ECX_ESI: { return ReadXOR_ECX_ESI(); }
                        case (byte)XorRegisterOpcodes.XOR_ECX_ESP: { return ReadXOR_ECX_ESP(); }
                        case (byte)XorRegisterOpcodes.XOR_EDI_EAX: { return ReadXOR_EDI_EAX(); }
                        case (byte)XorRegisterOpcodes.XOR_EDI_EBP: { return ReadXOR_EDI_EBP(); }
                        case (byte)XorRegisterOpcodes.XOR_EDI_EBX: { return ReadXOR_EDI_EBX(); }
                        case (byte)XorRegisterOpcodes.XOR_EDI_ECX: { return ReadXOR_EDI_ECX(); }
                        case (byte)XorRegisterOpcodes.XOR_EDI_EDI: { return ReadXOR_EDI_EDI(); }
                        case (byte)XorRegisterOpcodes.XOR_EDI_EDX: { return ReadXOR_EDI_EDX(); }
                        case (byte)XorRegisterOpcodes.XOR_EDI_ESI: { return ReadXOR_EDI_ESI(); }
                        case (byte)XorRegisterOpcodes.XOR_EDI_ESP: { return ReadXOR_EDI_ESP(); }
                        case (byte)XorRegisterOpcodes.XOR_EDX_EAX: { return ReadXOR_EDX_EAX(); }
                        case (byte)XorRegisterOpcodes.XOR_EDX_EBP: { return ReadXOR_EDX_EBP(); }
                        case (byte)XorRegisterOpcodes.XOR_EDX_EBX: { return ReadXOR_EDX_EBX(); }
                        case (byte)XorRegisterOpcodes.XOR_EDX_ECX: { return ReadXOR_EDX_ECX(); }
                        case (byte)XorRegisterOpcodes.XOR_EDX_EDI: { return ReadXOR_EDX_EDI(); }
                        case (byte)XorRegisterOpcodes.XOR_EDX_EDX: { return ReadXOR_EDX_EDX(); }
                        case (byte)XorRegisterOpcodes.XOR_EDX_ESI: { return ReadXOR_EDX_ESI(); }
                        case (byte)XorRegisterOpcodes.XOR_EDX_ESP: { return ReadXOR_EDX_ESP(); }
                        case (byte)XorRegisterOpcodes.XOR_ESI_EAX: { return ReadXOR_ESI_EAX(); }
                        case (byte)XorRegisterOpcodes.XOR_ESI_EBP: { return ReadXOR_ESI_EBP(); }
                        case (byte)XorRegisterOpcodes.XOR_ESI_EBX: { return ReadXOR_ESI_EBX(); }
                        case (byte)XorRegisterOpcodes.XOR_ESI_ECX: { return ReadXOR_ESI_ECX(); }
                        case (byte)XorRegisterOpcodes.XOR_ESI_EDI: { return ReadXOR_ESI_EDI(); }
                        case (byte)XorRegisterOpcodes.XOR_ESI_EDX: { return ReadXOR_ESI_EDX(); }
                        case (byte)XorRegisterOpcodes.XOR_ESI_ESI: { return ReadXOR_ESI_ESI(); }
                        case (byte)XorRegisterOpcodes.XOR_ESI_ESP: { return ReadXOR_ESI_ESP(); }
                        case (byte)XorRegisterOpcodes.XOR_ESP_EAX: { return ReadXOR_ESP_EAX(); }
                        case (byte)XorRegisterOpcodes.XOR_ESP_EBP: { return ReadXOR_ESP_EBP(); }
                        case (byte)XorRegisterOpcodes.XOR_ESP_EBX: { return ReadXOR_ESP_EBX(); }
                        case (byte)XorRegisterOpcodes.XOR_ESP_ECX: { return ReadXOR_ESP_ECX(); }
                        case (byte)XorRegisterOpcodes.XOR_ESP_EDI: { return ReadXOR_ESP_EDI(); }
                        case (byte)XorRegisterOpcodes.XOR_ESP_EDX: { return ReadXOR_ESP_EDX(); }
                        case (byte)XorRegisterOpcodes.XOR_ESP_ESI: { return ReadXOR_ESP_ESI(); }
                        case (byte)XorRegisterOpcodes.XOR_ESP_ESP: { return ReadXOR_ESP_ESP(); }
                        #endregion
                    }
                    break;
                }
                case (byte)OpcodeList.CMP_REGISTER:
                {
                    switch (InstructionData[offset + 1])
                    {
                        #region CMP
                        case (byte)CmpRegisterOpcodes.CMP_EAX_EAX: { return ReadCMP_EAX_EAX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EAX_EBP: { return ReadCMP_EAX_EBP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EAX_EBX: { return ReadCMP_EAX_EBX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EAX_ECX: { return ReadCMP_EAX_ECX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EAX_EDI: { return ReadCMP_EAX_EDI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EAX_EDX: { return ReadCMP_EAX_EDX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EAX_ESI: { return ReadCMP_EAX_ESI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EAX_ESP: { return ReadCMP_EAX_ESP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBP_EAX: { return ReadCMP_EBP_EAX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBP_EBP: { return ReadCMP_EBP_EBP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBP_EBX: { return ReadCMP_EBP_EBX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBP_ECX: { return ReadCMP_EBP_ECX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBP_EDI: { return ReadCMP_EBP_EDI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBP_EDX: { return ReadCMP_EBP_EDX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBP_ESI: { return ReadCMP_EBP_ESI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBP_ESP: { return ReadCMP_EBP_ESP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBX_EAX: { return ReadCMP_EBX_EAX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBX_EBP: { return ReadCMP_EBX_EBP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBX_EBX: { return ReadCMP_EBX_EBX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBX_ECX: { return ReadCMP_EBX_ECX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBX_EDI: { return ReadCMP_EBX_EDI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBX_EDX: { return ReadCMP_EBX_EDX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBX_ESI: { return ReadCMP_EBX_ESI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EBX_ESP: { return ReadCMP_EBX_ESP(); }
                        case (byte)CmpRegisterOpcodes.CMP_ECX_EAX: { return ReadCMP_ECX_EAX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ECX_EBP: { return ReadCMP_ECX_EBP(); }
                        case (byte)CmpRegisterOpcodes.CMP_ECX_EBX: { return ReadCMP_ECX_EBX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ECX_ECX: { return ReadCMP_ECX_ECX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ECX_EDI: { return ReadCMP_ECX_EDI(); }
                        case (byte)CmpRegisterOpcodes.CMP_ECX_EDX: { return ReadCMP_ECX_EDX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ECX_ESI: { return ReadCMP_ECX_ESI(); }
                        case (byte)CmpRegisterOpcodes.CMP_ECX_ESP: { return ReadCMP_ECX_ESP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDI_EAX: { return ReadCMP_EDI_EAX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDI_EBP: { return ReadCMP_EDI_EBP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDI_EBX: { return ReadCMP_EDI_EBX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDI_ECX: { return ReadCMP_EDI_ECX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDI_EDI: { return ReadCMP_EDI_EDI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDI_EDX: { return ReadCMP_EDI_EDX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDI_ESI: { return ReadCMP_EDI_ESI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDI_ESP: { return ReadCMP_EDI_ESP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDX_EAX: { return ReadCMP_EDX_EAX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDX_EBP: { return ReadCMP_EDX_EBP(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDX_EBX: { return ReadCMP_EDX_EBX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDX_ECX: { return ReadCMP_EDX_ECX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDX_EDI: { return ReadCMP_EDX_EDI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDX_EDX: { return ReadCMP_EDX_EDX(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDX_ESI: { return ReadCMP_EDX_ESI(); }
                        case (byte)CmpRegisterOpcodes.CMP_EDX_ESP: { return ReadCMP_EDX_ESP(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESI_EAX: { return ReadCMP_ESI_EAX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESI_EBP: { return ReadCMP_ESI_EBP(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESI_EBX: { return ReadCMP_ESI_EBX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESI_ECX: { return ReadCMP_ESI_ECX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESI_EDI: { return ReadCMP_ESI_EDI(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESI_EDX: { return ReadCMP_ESI_EDX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESI_ESI: { return ReadCMP_ESI_ESI(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESI_ESP: { return ReadCMP_ESI_ESP(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESP_EAX: { return ReadCMP_ESP_EAX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESP_EBP: { return ReadCMP_ESP_EBP(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESP_EBX: { return ReadCMP_ESP_EBX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESP_ECX: { return ReadCMP_ESP_ECX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESP_EDI: { return ReadCMP_ESP_EDI(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESP_EDX: { return ReadCMP_ESP_EDX(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESP_ESI: { return ReadCMP_ESP_ESI(); }
                        case (byte)CmpRegisterOpcodes.CMP_ESP_ESP: { return ReadCMP_ESP_ESP(); }
                        #endregion
                    }
                    break;
                }
                case (byte)OpcodeList.AND_REGISTER:
                {
                    switch (InstructionData[offset + 1])
                    {
                        #region AND
                        case (byte)AndRegisterOpcodes.AND_EAX_EAX: { return ReadAND_EAX_EAX(); }
                        case (byte)AndRegisterOpcodes.AND_EAX_EBP: { return ReadAND_EAX_EBP(); }
                        case (byte)AndRegisterOpcodes.AND_EAX_EBX: { return ReadAND_EAX_EBX(); }
                        case (byte)AndRegisterOpcodes.AND_EAX_ECX: { return ReadAND_EAX_ECX(); }
                        case (byte)AndRegisterOpcodes.AND_EAX_EDI: { return ReadAND_EAX_EDI(); }
                        case (byte)AndRegisterOpcodes.AND_EAX_EDX: { return ReadAND_EAX_EDX(); }
                        case (byte)AndRegisterOpcodes.AND_EAX_ESI: { return ReadAND_EAX_ESI(); }
                        case (byte)AndRegisterOpcodes.AND_EAX_ESP: { return ReadAND_EAX_ESP(); }
                        case (byte)AndRegisterOpcodes.AND_EBP_EAX: { return ReadAND_EBP_EAX(); }
                        case (byte)AndRegisterOpcodes.AND_EBP_EBP: { return ReadAND_EBP_EBP(); }
                        case (byte)AndRegisterOpcodes.AND_EBP_EBX: { return ReadAND_EBP_EBX(); }
                        case (byte)AndRegisterOpcodes.AND_EBP_ECX: { return ReadAND_EBP_ECX(); }
                        case (byte)AndRegisterOpcodes.AND_EBP_EDI: { return ReadAND_EBP_EDI(); }
                        case (byte)AndRegisterOpcodes.AND_EBP_EDX: { return ReadAND_EBP_EDX(); }
                        case (byte)AndRegisterOpcodes.AND_EBP_ESI: { return ReadAND_EBP_ESI(); }
                        case (byte)AndRegisterOpcodes.AND_EBP_ESP: { return ReadAND_EBP_ESP(); }
                        case (byte)AndRegisterOpcodes.AND_EBX_EAX: { return ReadAND_EBX_EAX(); }
                        case (byte)AndRegisterOpcodes.AND_EBX_EBP: { return ReadAND_EBX_EBP(); }
                        case (byte)AndRegisterOpcodes.AND_EBX_EBX: { return ReadAND_EBX_EBX(); }
                        case (byte)AndRegisterOpcodes.AND_EBX_ECX: { return ReadAND_EBX_ECX(); }
                        case (byte)AndRegisterOpcodes.AND_EBX_EDI: { return ReadAND_EBX_EDI(); }
                        case (byte)AndRegisterOpcodes.AND_EBX_EDX: { return ReadAND_EBX_EDX(); }
                        case (byte)AndRegisterOpcodes.AND_EBX_ESI: { return ReadAND_EBX_ESI(); }
                        case (byte)AndRegisterOpcodes.AND_EBX_ESP: { return ReadAND_EBX_ESP(); }
                        case (byte)AndRegisterOpcodes.AND_ECX_EAX: { return ReadAND_ECX_EAX(); }
                        case (byte)AndRegisterOpcodes.AND_ECX_EBP: { return ReadAND_ECX_EBP(); }
                        case (byte)AndRegisterOpcodes.AND_ECX_EBX: { return ReadAND_ECX_EBX(); }
                        case (byte)AndRegisterOpcodes.AND_ECX_ECX: { return ReadAND_ECX_ECX(); }
                        case (byte)AndRegisterOpcodes.AND_ECX_EDI: { return ReadAND_ECX_EDI(); }
                        case (byte)AndRegisterOpcodes.AND_ECX_EDX: { return ReadAND_ECX_EDX(); }
                        case (byte)AndRegisterOpcodes.AND_ECX_ESI: { return ReadAND_ECX_ESI(); }
                        case (byte)AndRegisterOpcodes.AND_ECX_ESP: { return ReadAND_ECX_ESP(); }
                        case (byte)AndRegisterOpcodes.AND_EDI_EAX: { return ReadAND_EDI_EAX(); }
                        case (byte)AndRegisterOpcodes.AND_EDI_EBP: { return ReadAND_EDI_EBP(); }
                        case (byte)AndRegisterOpcodes.AND_EDI_EBX: { return ReadAND_EDI_EBX(); }
                        case (byte)AndRegisterOpcodes.AND_EDI_ECX: { return ReadAND_EDI_ECX(); }
                        case (byte)AndRegisterOpcodes.AND_EDI_EDI: { return ReadAND_EDI_EDI(); }
                        case (byte)AndRegisterOpcodes.AND_EDI_EDX: { return ReadAND_EDI_EDX(); }
                        case (byte)AndRegisterOpcodes.AND_EDI_ESI: { return ReadAND_EDI_ESI(); }
                        case (byte)AndRegisterOpcodes.AND_EDI_ESP: { return ReadAND_EDI_ESP(); }
                        case (byte)AndRegisterOpcodes.AND_EDX_EAX: { return ReadAND_EDX_EAX(); }
                        case (byte)AndRegisterOpcodes.AND_EDX_EBP: { return ReadAND_EDX_EBP(); }
                        case (byte)AndRegisterOpcodes.AND_EDX_EBX: { return ReadAND_EDX_EBX(); }
                        case (byte)AndRegisterOpcodes.AND_EDX_ECX: { return ReadAND_EDX_ECX(); }
                        case (byte)AndRegisterOpcodes.AND_EDX_EDI: { return ReadAND_EDX_EDI(); }
                        case (byte)AndRegisterOpcodes.AND_EDX_EDX: { return ReadAND_EDX_EDX(); }
                        case (byte)AndRegisterOpcodes.AND_EDX_ESI: { return ReadAND_EDX_ESI(); }
                        case (byte)AndRegisterOpcodes.AND_EDX_ESP: { return ReadAND_EDX_ESP(); }
                        case (byte)AndRegisterOpcodes.AND_ESI_EAX: { return ReadAND_ESI_EAX(); }
                        case (byte)AndRegisterOpcodes.AND_ESI_EBP: { return ReadAND_ESI_EBP(); }
                        case (byte)AndRegisterOpcodes.AND_ESI_EBX: { return ReadAND_ESI_EBX(); }
                        case (byte)AndRegisterOpcodes.AND_ESI_ECX: { return ReadAND_ESI_ECX(); }
                        case (byte)AndRegisterOpcodes.AND_ESI_EDI: { return ReadAND_ESI_EDI(); }
                        case (byte)AndRegisterOpcodes.AND_ESI_EDX: { return ReadAND_ESI_EDX(); }
                        case (byte)AndRegisterOpcodes.AND_ESI_ESI: { return ReadAND_ESI_ESI(); }
                        case (byte)AndRegisterOpcodes.AND_ESI_ESP: { return ReadAND_ESI_ESP(); }
                        case (byte)AndRegisterOpcodes.AND_ESP_EAX: { return ReadAND_ESP_EAX(); }
                        case (byte)AndRegisterOpcodes.AND_ESP_EBP: { return ReadAND_ESP_EBP(); }
                        case (byte)AndRegisterOpcodes.AND_ESP_EBX: { return ReadAND_ESP_EBX(); }
                        case (byte)AndRegisterOpcodes.AND_ESP_ECX: { return ReadAND_ESP_ECX(); }
                        case (byte)AndRegisterOpcodes.AND_ESP_EDI: { return ReadAND_ESP_EDI(); }
                        case (byte)AndRegisterOpcodes.AND_ESP_EDX: { return ReadAND_ESP_EDX(); }
                        case (byte)AndRegisterOpcodes.AND_ESP_ESI: { return ReadAND_ESP_ESI(); }
                        case (byte)AndRegisterOpcodes.AND_ESP_ESP: { return ReadAND_ESP_ESP(); }
                        #endregion
                    }
                    break;
                }
                case (byte)OpcodeList.MOV_REGISTER:
                {
                    switch (InstructionData[offset + 1])
                    {
                        #region MOV
                        case (byte)MovRegisterOpcodes.MOV_EAX_EAX: { return ReadMOV_EAX_EAX(); }
                        case (byte)MovRegisterOpcodes.MOV_EAX_EBP: { return ReadMOV_EAX_EBP(); }
                        case (byte)MovRegisterOpcodes.MOV_EAX_EBX: { return ReadMOV_EAX_EBX(); }
                        case (byte)MovRegisterOpcodes.MOV_EAX_ECX: { return ReadMOV_EAX_ECX(); }
                        case (byte)MovRegisterOpcodes.MOV_EAX_EDI: { return ReadMOV_EAX_EDI(); }
                        case (byte)MovRegisterOpcodes.MOV_EAX_EDX: { return ReadMOV_EAX_EDX(); }
                        case (byte)MovRegisterOpcodes.MOV_EAX_ESI: { return ReadMOV_EAX_ESI(); }
                        case (byte)MovRegisterOpcodes.MOV_EAX_ESP: { return ReadMOV_EAX_ESP(); }
                        case (byte)MovRegisterOpcodes.MOV_EBP_EAX: { return ReadMOV_EBP_EAX(); }
                        case (byte)MovRegisterOpcodes.MOV_EBP_EBP: { return ReadMOV_EBP_EBP(); }
                        case (byte)MovRegisterOpcodes.MOV_EBP_EBX: { return ReadMOV_EBP_EBX(); }
                        case (byte)MovRegisterOpcodes.MOV_EBP_ECX: { return ReadMOV_EBP_ECX(); }
                        case (byte)MovRegisterOpcodes.MOV_EBP_EDI: { return ReadMOV_EBP_EDI(); }
                        case (byte)MovRegisterOpcodes.MOV_EBP_EDX: { return ReadMOV_EBP_EDX(); }
                        case (byte)MovRegisterOpcodes.MOV_EBP_ESI: { return ReadMOV_EBP_ESI(); }
                        case (byte)MovRegisterOpcodes.MOV_EBP_ESP: { return ReadMOV_EBP_ESP(); }
                        case (byte)MovRegisterOpcodes.MOV_EBX_EAX: { return ReadMOV_EBX_EAX(); }
                        case (byte)MovRegisterOpcodes.MOV_EBX_EBP: { return ReadMOV_EBX_EBP(); }
                        case (byte)MovRegisterOpcodes.MOV_EBX_EBX: { return ReadMOV_EBX_EBX(); }
                        case (byte)MovRegisterOpcodes.MOV_EBX_ECX: { return ReadMOV_EBX_ECX(); }
                        case (byte)MovRegisterOpcodes.MOV_EBX_EDI: { return ReadMOV_EBX_EDI(); }
                        case (byte)MovRegisterOpcodes.MOV_EBX_EDX: { return ReadMOV_EBX_EDX(); }
                        case (byte)MovRegisterOpcodes.MOV_EBX_ESI: { return ReadMOV_EBX_ESI(); }
                        case (byte)MovRegisterOpcodes.MOV_EBX_ESP: { return ReadMOV_EBX_ESP(); }
                        case (byte)MovRegisterOpcodes.MOV_ECX_EAX: { return ReadMOV_ECX_EAX(); }
                        case (byte)MovRegisterOpcodes.MOV_ECX_EBP: { return ReadMOV_ECX_EBP(); }
                        case (byte)MovRegisterOpcodes.MOV_ECX_EBX: { return ReadMOV_ECX_EBX(); }
                        case (byte)MovRegisterOpcodes.MOV_ECX_ECX: { return ReadMOV_ECX_ECX(); }
                        case (byte)MovRegisterOpcodes.MOV_ECX_EDI: { return ReadMOV_ECX_EDI(); }
                        case (byte)MovRegisterOpcodes.MOV_ECX_EDX: { return ReadMOV_ECX_EDX(); }
                        case (byte)MovRegisterOpcodes.MOV_ECX_ESI: { return ReadMOV_ECX_ESI(); }
                        case (byte)MovRegisterOpcodes.MOV_ECX_ESP: { return ReadMOV_ECX_ESP(); }
                        case (byte)MovRegisterOpcodes.MOV_EDI_EAX: { return ReadMOV_EDI_EAX(); }
                        case (byte)MovRegisterOpcodes.MOV_EDI_EBP: { return ReadMOV_EDI_EBP(); }
                        case (byte)MovRegisterOpcodes.MOV_EDI_EBX: { return ReadMOV_EDI_EBX(); }
                        case (byte)MovRegisterOpcodes.MOV_EDI_ECX: { return ReadMOV_EDI_ECX(); }
                        case (byte)MovRegisterOpcodes.MOV_EDI_EDI: { return ReadMOV_EDI_EDI(); }
                        case (byte)MovRegisterOpcodes.MOV_EDI_EDX: { return ReadMOV_EDI_EDX(); }
                        case (byte)MovRegisterOpcodes.MOV_EDI_ESI: { return ReadMOV_EDI_ESI(); }
                        case (byte)MovRegisterOpcodes.MOV_EDI_ESP: { return ReadMOV_EDI_ESP(); }
                        case (byte)MovRegisterOpcodes.MOV_EDX_EAX: { return ReadMOV_EDX_EAX(); }
                        case (byte)MovRegisterOpcodes.MOV_EDX_EBP: { return ReadMOV_EDX_EBP(); }
                        case (byte)MovRegisterOpcodes.MOV_EDX_EBX: { return ReadMOV_EDX_EBX(); }
                        case (byte)MovRegisterOpcodes.MOV_EDX_ECX: { return ReadMOV_EDX_ECX(); }
                        case (byte)MovRegisterOpcodes.MOV_EDX_EDI: { return ReadMOV_EDX_EDI(); }
                        case (byte)MovRegisterOpcodes.MOV_EDX_EDX: { return ReadMOV_EDX_EDX(); }
                        case (byte)MovRegisterOpcodes.MOV_EDX_ESI: { return ReadMOV_EDX_ESI(); }
                        case (byte)MovRegisterOpcodes.MOV_EDX_ESP: { return ReadMOV_EDX_ESP(); }
                        case (byte)MovRegisterOpcodes.MOV_ESI_EAX: { return ReadMOV_ESI_EAX(); }
                        case (byte)MovRegisterOpcodes.MOV_ESI_EBP: { return ReadMOV_ESI_EBP(); }
                        case (byte)MovRegisterOpcodes.MOV_ESI_EBX: { return ReadMOV_ESI_EBX(); }
                        case (byte)MovRegisterOpcodes.MOV_ESI_ECX: { return ReadMOV_ESI_ECX(); }
                        case (byte)MovRegisterOpcodes.MOV_ESI_EDI: { return ReadMOV_ESI_EDI(); }
                        case (byte)MovRegisterOpcodes.MOV_ESI_EDX: { return ReadMOV_ESI_EDX(); }
                        case (byte)MovRegisterOpcodes.MOV_ESI_ESI: { return ReadMOV_ESI_ESI(); }
                        case (byte)MovRegisterOpcodes.MOV_ESI_ESP: { return ReadMOV_ESI_ESP(); }
                        case (byte)MovRegisterOpcodes.MOV_ESP_EAX: { return ReadMOV_ESP_EAX(); }
                        case (byte)MovRegisterOpcodes.MOV_ESP_EBP: { return ReadMOV_ESP_EBP(); }
                        case (byte)MovRegisterOpcodes.MOV_ESP_EBX: { return ReadMOV_ESP_EBX(); }
                        case (byte)MovRegisterOpcodes.MOV_ESP_ECX: { return ReadMOV_ESP_ECX(); }
                        case (byte)MovRegisterOpcodes.MOV_ESP_EDI: { return ReadMOV_ESP_EDI(); }
                        case (byte)MovRegisterOpcodes.MOV_ESP_EDX: { return ReadMOV_ESP_EDX(); }
                        case (byte)MovRegisterOpcodes.MOV_ESP_ESI: { return ReadMOV_ESP_ESI(); }
                        case (byte)MovRegisterOpcodes.MOV_ESP_ESP: { return ReadMOV_ESP_ESP(); }
                        #endregion
                        case 0x4D: //MOV REGISTER, DWORD PTR[ADDRESS]
                        case 0x45:
                        {
                            return ReadMOV_REGISTER_DWORD_PTR();
                        }
                    }
                    break;
                }
                case (byte)OpcodeList.MUL:
                {
                    switch (InstructionData[offset + 1])
                    {
                        #region MUL
                        case (byte)MulRegisterOpcodes.MUL_EAX_EAX: { return ReadMUL_EAX_EAX(); }
                        case (byte)MulRegisterOpcodes.MUL_EAX_EBP: { return ReadMUL_EAX_EBP(); }
                        case (byte)MulRegisterOpcodes.MUL_EAX_EBX: { return ReadMUL_EAX_EBX(); }
                        case (byte)MulRegisterOpcodes.MUL_EAX_ECX: { return ReadMUL_EAX_ECX(); }
                        case (byte)MulRegisterOpcodes.MUL_EAX_EDI: { return ReadMUL_EAX_EDI(); }
                        case (byte)MulRegisterOpcodes.MUL_EAX_EDX: { return ReadMUL_EAX_EDX(); }
                        case (byte)MulRegisterOpcodes.MUL_EAX_ESI: { return ReadMUL_EAX_ESI(); }
                        case (byte)MulRegisterOpcodes.MUL_EAX_ESP: { return ReadMUL_EAX_ESP(); }
                        case (byte)MulRegisterOpcodes.MUL_EBP_EAX: { return ReadMUL_EBP_EAX(); }
                        case (byte)MulRegisterOpcodes.MUL_EBP_EBP: { return ReadMUL_EBP_EBP(); }
                        case (byte)MulRegisterOpcodes.MUL_EBP_EBX: { return ReadMUL_EBP_EBX(); }
                        case (byte)MulRegisterOpcodes.MUL_EBP_ECX: { return ReadMUL_EBP_ECX(); }
                        case (byte)MulRegisterOpcodes.MUL_EBP_EDI: { return ReadMUL_EBP_EDI(); }
                        case (byte)MulRegisterOpcodes.MUL_EBP_EDX: { return ReadMUL_EBP_EDX(); }
                        case (byte)MulRegisterOpcodes.MUL_EBP_ESI: { return ReadMUL_EBP_ESI(); }
                        case (byte)MulRegisterOpcodes.MUL_EBP_ESP: { return ReadMUL_EBP_ESP(); }
                        case (byte)MulRegisterOpcodes.MUL_EBX_EAX: { return ReadMUL_EBX_EAX(); }
                        case (byte)MulRegisterOpcodes.MUL_EBX_EBP: { return ReadMUL_EBX_EBP(); }
                        case (byte)MulRegisterOpcodes.MUL_EBX_EBX: { return ReadMUL_EBX_EBX(); }
                        case (byte)MulRegisterOpcodes.MUL_EBX_ECX: { return ReadMUL_EBX_ECX(); }
                        case (byte)MulRegisterOpcodes.MUL_EBX_EDI: { return ReadMUL_EBX_EDI(); }
                        case (byte)MulRegisterOpcodes.MUL_EBX_EDX: { return ReadMUL_EBX_EDX(); }
                        case (byte)MulRegisterOpcodes.MUL_EBX_ESI: { return ReadMUL_EBX_ESI(); }
                        case (byte)MulRegisterOpcodes.MUL_EBX_ESP: { return ReadMUL_EBX_ESP(); }
                        case (byte)MulRegisterOpcodes.MUL_ECX_EAX: { return ReadMUL_ECX_EAX(); }
                        case (byte)MulRegisterOpcodes.MUL_ECX_EBP: { return ReadMUL_ECX_EBP(); }
                        case (byte)MulRegisterOpcodes.MUL_ECX_EBX: { return ReadMUL_ECX_EBX(); }
                        case (byte)MulRegisterOpcodes.MUL_ECX_ECX: { return ReadMUL_ECX_ECX(); }
                        case (byte)MulRegisterOpcodes.MUL_ECX_EDI: { return ReadMUL_ECX_EDI(); }
                        case (byte)MulRegisterOpcodes.MUL_ECX_EDX: { return ReadMUL_ECX_EDX(); }
                        case (byte)MulRegisterOpcodes.MUL_ECX_ESI: { return ReadMUL_ECX_ESI(); }
                        case (byte)MulRegisterOpcodes.MUL_ECX_ESP: { return ReadMUL_ECX_ESP(); }
                        case (byte)MulRegisterOpcodes.MUL_EDI_EAX: { return ReadMUL_EDI_EAX(); }
                        case (byte)MulRegisterOpcodes.MUL_EDI_EBP: { return ReadMUL_EDI_EBP(); }
                        case (byte)MulRegisterOpcodes.MUL_EDI_EBX: { return ReadMUL_EDI_EBX(); }
                        case (byte)MulRegisterOpcodes.MUL_EDI_ECX: { return ReadMUL_EDI_ECX(); }
                        case (byte)MulRegisterOpcodes.MUL_EDI_EDI: { return ReadMUL_EDI_EDI(); }
                        case (byte)MulRegisterOpcodes.MUL_EDI_EDX: { return ReadMUL_EDI_EDX(); }
                        case (byte)MulRegisterOpcodes.MUL_EDI_ESI: { return ReadMUL_EDI_ESI(); }
                        case (byte)MulRegisterOpcodes.MUL_EDI_ESP: { return ReadMUL_EDI_ESP(); }
                        case (byte)MulRegisterOpcodes.MUL_EDX_EAX: { return ReadMUL_EDX_EAX(); }
                        case (byte)MulRegisterOpcodes.MUL_EDX_EBP: { return ReadMUL_EDX_EBP(); }
                        case (byte)MulRegisterOpcodes.MUL_EDX_EBX: { return ReadMUL_EDX_EBX(); }
                        case (byte)MulRegisterOpcodes.MUL_EDX_ECX: { return ReadMUL_EDX_ECX(); }
                        case (byte)MulRegisterOpcodes.MUL_EDX_EDI: { return ReadMUL_EDX_EDI(); }
                        case (byte)MulRegisterOpcodes.MUL_EDX_EDX: { return ReadMUL_EDX_EDX(); }
                        case (byte)MulRegisterOpcodes.MUL_EDX_ESI: { return ReadMUL_EDX_ESI(); }
                        case (byte)MulRegisterOpcodes.MUL_EDX_ESP: { return ReadMUL_EDX_ESP(); }
                        case (byte)MulRegisterOpcodes.MUL_ESI_EAX: { return ReadMUL_ESI_EAX(); }
                        case (byte)MulRegisterOpcodes.MUL_ESI_EBP: { return ReadMUL_ESI_EBP(); }
                        case (byte)MulRegisterOpcodes.MUL_ESI_EBX: { return ReadMUL_ESI_EBX(); }
                        case (byte)MulRegisterOpcodes.MUL_ESI_ECX: { return ReadMUL_ESI_ECX(); }
                        case (byte)MulRegisterOpcodes.MUL_ESI_EDI: { return ReadMUL_ESI_EDI(); }
                        case (byte)MulRegisterOpcodes.MUL_ESI_EDX: { return ReadMUL_ESI_EDX(); }
                        case (byte)MulRegisterOpcodes.MUL_ESI_ESI: { return ReadMUL_ESI_ESI(); }
                        case (byte)MulRegisterOpcodes.MUL_ESI_ESP: { return ReadMUL_ESI_ESP(); }
                        case (byte)MulRegisterOpcodes.MUL_ESP_EAX: { return ReadMUL_ESP_EAX(); }
                        case (byte)MulRegisterOpcodes.MUL_ESP_EBP: { return ReadMUL_ESP_EBP(); }
                        case (byte)MulRegisterOpcodes.MUL_ESP_EBX: { return ReadMUL_ESP_EBX(); }
                        case (byte)MulRegisterOpcodes.MUL_ESP_ECX: { return ReadMUL_ESP_ECX(); }
                        case (byte)MulRegisterOpcodes.MUL_ESP_EDI: { return ReadMUL_ESP_EDI(); }
                        case (byte)MulRegisterOpcodes.MUL_ESP_EDX: { return ReadMUL_ESP_EDX(); }
                        case (byte)MulRegisterOpcodes.MUL_ESP_ESI: { return ReadMUL_ESP_ESI(); }
                        case (byte)MulRegisterOpcodes.MUL_ESP_ESP: { return ReadMUL_ESP_ESP(); }
                        #endregion
                    }
                    break;
                }
                case (byte)OpcodeList.Variable: //0xFF, just a custom opcode at the moment, need to use Data Section
                {
                    switch (InstructionData[offset + 1])
                    {
                        case 0: //Variable
                        {
                            return ReadVariable();
                        }
                    }
                    break;
                }
                case 0x66:
                {
                    switch (InstructionData[offset+1])
                    {
                        case 0xC7:
                        {
                            switch (InstructionData[offset+2])
                            {
                                case 0x05:
                                {
                                    return ReadMOV_VARIABLE_VALUE();
                                }
                                case 0x45:
                                {
                                    return ReadMOV_VARIABLE_INDEX_VALUE();
                                }
                            }
                            break;
                        }
                    }
                    break;
                }
            }

            throw new Exception("Unknown opcode, " + InstructionData[offset].ToString("X2"));
        }

        private void ThrowException(string opcode, OpcodeReaderError reason)
        {
            throw new Exception("Error reading the " + opcode + " instruction, reason: " + reason);
        }
    }
}