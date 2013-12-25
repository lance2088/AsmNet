using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.BeaEngine;
using System.IO;
using Asm.Net.src.Opcodes.INC;
using Asm.Net.src.Opcodes.MOV;
using Asm.Net.src.Opcodes;
using Asm.Net.src.Opcodes.Jumps;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Opcodes.ADD;
using Asm.Net.src.Opcodes.XOR;

namespace Asm.Net.src
{
    public class x86Data
    {
        public static Instruction[] PeToInstructions(string FilePath)
        {
            UnmanagedBuffer buffer = new UnmanagedBuffer(File.ReadAllBytes(FilePath));
            List<Instruction> instructs = new List<Instruction>();
            Disasm disasm = new Disasm();
            disasm.EIP = new IntPtr(buffer.Ptr.ToInt64() + 0x400);

            List<int> Addresses = new List<int>();

            while (true)
            {
                int result = Asm.Net.src.BeaEngine.BeaEngine.Disasm(disasm);
                Addresses.Add(disasm.EIP.ToInt32());
                
                if (result == (int)BeaConstants.SpecialInfo.UNKNOWN_OPCODE)
                {
                    break;
                }

                //Console.WriteLine("0x" + disasm.EIP.ToString("X") + " " + disasm.CompleteInstr);

                //convert the data to instructions so we are able to execute it in Asm.Net
                //We also need to change the pointers for push, call, inc, dec etc... so Asm.Net is able to understand it

                switch (disasm.Instruction.Opcode)
                {
                    case (int)OpcodeList.CALL:
                    {
                        string tmp = disasm.Argument1.ArgMnemonic.Substring(0, disasm.Argument1.ArgMnemonic.Length - 1).Replace("FFFFFFFF", "");
                        int Jmpvalue = Convert.ToInt32(tmp, 16);

                        CALL call = new CALL(Jmpvalue);
                        call.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                        instructs.Add(call);
                        break;
                    }
                    case (int)OpcodeList.INC_EAX:
                    {
                        INC_EAX IncEax = new INC_EAX();
                        IncEax.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                        instructs.Add(IncEax);
                        break;
                    }
                    case (int)OpcodeList.INC_EDX:
                    {

                        break;
                    }
                    case (int)OpcodeList.JE:
                    {

                        break;
                    }
                    case (int)OpcodeList.JMP:
                    {
                        string tmp = disasm.Argument1.ArgMnemonic.Substring(0, disasm.Argument1.ArgMnemonic.Length - 1);
                        int Jmpvalue = Convert.ToInt32(tmp.Replace("FFFFFFFF", ""), 16);

                        JMP jmp = new JMP(Jmpvalue);
                        jmp.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                        //instructs.Add(jmp);
                        break;
                    }
                    case (int)OpcodeList.JNZ:
                    {
                        string tmp = disasm.Argument1.ArgMnemonic.Substring(0, disasm.Argument1.ArgMnemonic.Length - 1);
                        int Jmpvalue = Convert.ToInt32(tmp.Replace("FFFFFFFF", ""), 16);

                        JNZ jnz = new JNZ(Jmpvalue);
                        jnz.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                        //instructs.Add(jnz);
                        break;
                    }
                    case (int)OpcodeList.MOV_EAX:
                    {
                        string tmp = disasm.Argument2.ArgMnemonic.Substring(0, disasm.Argument2.ArgMnemonic.Length - 1);
                        int MovValue = Convert.ToInt32(tmp, 16);
                        MOV_EAX MovEAX = new MOV_EAX(new VirtualAddress(0, MovValue));
                        MovEAX.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                        instructs.Add(MovEAX);
                        break;
                    }
                    case (int)OpcodeList.NOP:
                    {
                        NOP nop = new NOP();
                        nop.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                        instructs.Add(nop);
                        break;
                    }
                    case (int)OpcodeList.PUSH_EAX:
                    {
                        break;
                    }
                    case (int)OpcodeList.RET:
                    {
                        RET ret = new RET();
                        ret.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                        instructs.Add(ret);
                        break;
                    }
                    case (int)OpcodeList.ADD:
                    {
                        //need to reverse check the opcodes...
                        Instruction ADD = null;
                        switch (disasm.Argument1.ArgMnemonic)
                        {
                            case "eax":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "al":
                                    {
                                        ADD = new ADD_BYTE_PTR_EAX_AL();
                                        break;
                                    }
                                }
                                break;
                            }
                        }

                        if (ADD != null)
                        {
                            ADD.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                            instructs.Add(ADD);
                        }
                        break;
                    }
                    case (int)OpcodeList.XOR_REGISTER:
                    {
                        Instruction xor = null;
                        switch (disasm.Argument1.ArgMnemonic)
                        {
                            case "eax":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "eax": { xor = new XOR_EAX_EAX(); break; }
                                    case "ecx": { xor = new XOR_EAX_ECX(); break; }
                                    case "edx": { xor = new XOR_EAX_EDX(); break; }
                                    case "ebx": { xor = new XOR_EAX_EBX(); break; }
                                    case "esp": { xor = new XOR_EAX_ESP(); break; }
                                    case "ebp": { xor = new XOR_EAX_EBP(); break; }
                                    case "esi": { xor = new XOR_EAX_ESI(); break; }
                                    case "edi": { xor = new XOR_EAX_EDI(); break; }
                                }
                                break;
                            }
                            case "ecx":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "eax": { xor = new XOR_ECX_EAX(); break; }
                                    case "ecx": { xor = new XOR_ECX_ECX(); break; }
                                    case "edx": { xor = new XOR_ECX_EDX(); break; }
                                    case "ebx": { xor = new XOR_ECX_EBX(); break; }
                                    case "esp": { xor = new XOR_ECX_ESP(); break; }
                                    case "ebp": { xor = new XOR_ECX_EBP(); break; }
                                    case "esi": { xor = new XOR_ECX_ESI(); break; }
                                    case "edi": { xor = new XOR_ECX_EDI(); break; }
                                }
                                break;
                            }
                            case "edx":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "eax": { xor = new XOR_EDX_EAX(); break; }
                                    case "ecx": { xor = new XOR_EDX_ECX(); break; }
                                    case "edx": { xor = new XOR_EDX_EDX(); break; }
                                    case "ebx": { xor = new XOR_EDX_EBX(); break; }
                                    case "esp": { xor = new XOR_EDX_ESP(); break; }
                                    case "ebp": { xor = new XOR_EDX_EBP(); break; }
                                    case "esi": { xor = new XOR_EDX_ESI(); break; }
                                    case "edi": { xor = new XOR_EDX_EDI(); break; }
                                }
                                break;
                            }
                            case "ebx":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "eax": { xor = new XOR_EBX_EAX(); break; }
                                    case "ecx": { xor = new XOR_EBX_ECX(); break; }
                                    case "edx": { xor = new XOR_EBX_EDX(); break; }
                                    case "ebx": { xor = new XOR_EBX_EBX(); break; }
                                    case "esp": { xor = new XOR_EBX_ESP(); break; }
                                    case "ebp": { xor = new XOR_EBX_EBP(); break; }
                                    case "esi": { xor = new XOR_EBX_ESI(); break; }
                                    case "edi": { xor = new XOR_EBX_EDI(); break; }
                                }
                                break;
                            }
                            case "esp":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "eax": { xor = new XOR_ESP_EAX(); break; }
                                    case "ecx": { xor = new XOR_ESP_ECX(); break; }
                                    case "edx": { xor = new XOR_ESP_EDX(); break; }
                                    case "ebx": { xor = new XOR_ESP_EBX(); break; }
                                    case "esp": { xor = new XOR_ESP_ESP(); break; }
                                    case "ebp": { xor = new XOR_ESP_EBP(); break; }
                                    case "esi": { xor = new XOR_ESP_ESI(); break; }
                                    case "edi": { xor = new XOR_ESP_EDI(); break; }
                                }
                                break;
                            }
                            case "ebp":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "eax": { xor = new XOR_EBP_EAX(); break; }
                                    case "ecx": { xor = new XOR_EBP_ECX(); break; }
                                    case "edx": { xor = new XOR_EBP_EDX(); break; }
                                    case "ebx": { xor = new XOR_EBP_EBX(); break; }
                                    case "esp": { xor = new XOR_EBP_ESP(); break; }
                                    case "ebp": { xor = new XOR_EBP_EBP(); break; }
                                    case "esi": { xor = new XOR_EBP_ESI(); break; }
                                    case "edi": { xor = new XOR_EBP_EDI(); break; }
                                }
                                break;
                            }
                            case "esi":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "eax": { xor = new XOR_ESI_EAX(); break; }
                                    case "ecx": { xor = new XOR_ESI_ECX(); break; }
                                    case "edx": { xor = new XOR_ESI_EDX(); break; }
                                    case "ebx": { xor = new XOR_ESI_EBX(); break; }
                                    case "esp": { xor = new XOR_ESI_ESP(); break; }
                                    case "ebp": { xor = new XOR_ESI_EBP(); break; }
                                    case "esi": { xor = new XOR_ESI_ESI(); break; }
                                    case "edi": { xor = new XOR_ESI_EDI(); break; }
                                }
                                break;
                            }
                            case "edi":
                            {
                                switch (disasm.Argument2.ArgMnemonic)
                                {
                                    case "eax": { xor = new XOR_EDI_EAX(); break; }
                                    case "ecx": { xor = new XOR_EDI_ECX(); break; }
                                    case "edx": { xor = new XOR_EDI_EDX(); break; }
                                    case "ebx": { xor = new XOR_EDI_EBX(); break; }
                                    case "esp": { xor = new XOR_EDI_ESP(); break; }
                                    case "ebp": { xor = new XOR_EDI_EBP(); break; }
                                    case "esi": { xor = new XOR_EDI_ESI(); break; }
                                    case "edi": { xor = new XOR_EDI_EDI(); break; }
                                }
                                break;
                            }
                        }

                        if (xor != null) //this check is just for temp, not all the XOR instructions are added
                        {
                            xor.NativeVirtualAddress.Address = disasm.EIP.ToInt32();
                            instructs.Add(xor);
                        }
                        break;
                    }
                }

                disasm.EIP = new IntPtr(disasm.EIP.ToInt64() + result);
            }

            //set all the pointers correct
            int offset = Options.MemoryBaseAddress;
            foreach (Instruction instruction in instructs)
            {
                switch (instruction.ToByteArray()[0])
                {
                    case (int)OpcodeList.CALL:
                    {
                        break;
                    }
                    case (int)OpcodeList.INC_EAX:
                    {
                        break;
                    }
                    case (int)OpcodeList.INC_EDX:
                    {
                        break;
                    }
                    case (int)OpcodeList.JE:
                    {
                        break;
                    }
                    case (int)OpcodeList.JMP:
                    {
                        //lets set our new jmp pointer, We also should load the modules which are required to run this program
                        //We need to load the Import Table and get all the .dll's from it and getting all the instructions from it
                        bool NewSet = false;
                        foreach (Instruction instruct in instructs)
                        {
                            if (Addresses.Contains(((IJump)instruction).JumpAddress))
                            {
                                //Set the ASM.net pointer
                                int index = Addresses.IndexOf(((IJump)instruction).JumpAddress) - 1;
                            }

                            //if (((IJump)instruction).JumpAddress == instruct.NativeVirtualAddress.Address)
                            //{
                            //}
                        }

                        //if (!NewSet)
                        //    throw new Exception("Unable to find the JMP Pointer, Invalid memory address ?");
                        break;
                    }
                    case (int)OpcodeList.MOV_EAX:
                    {
                        break;
                    }
                    case (int)OpcodeList.NOP:
                    {
                        break;
                    }
                    case (int)OpcodeList.PUSH_EAX:
                    {
                        break;
                    }
                    case (int)OpcodeList.RET:
                    {
                        break;
                    }
                }

                instruction.VirtualAddress.Address = offset;
                offset += instruction.VirtualAddress.Size;
            }

            return instructs.ToArray();
        }
    }
}