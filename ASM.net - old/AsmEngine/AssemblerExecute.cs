using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Threading;
using AsmEngine.DataTypes;
using AsmEngine.Objects;
using AsmEngine.NetEngine;
using AsmEngine.NetEngine.Console;
using AsmEngine.Events;
using System.Runtime.InteropServices;
using AsmEngine.Wrappers;
using AsmEngine.Collection;
using AsmEngine.Instructions;
using AsmEngine.Instructions.Interrupts;
using AsmEngine.Instructions.Jump;
using System.Drawing;
using AsmEngine.Video;

namespace AsmEngine
{
    public unsafe class AssemblerExecute
    {
        public byte[] ByteExecution { get; private set; }
        public SortedList<byte, AssemblerNamespace> namespaces;
        public StackCollection pushes;
        public static GraphicsDevice graphicsDevice;
        private bool AllowNextStep;

        public GraphicsDevice GRAPHICSDEVICE
        {
            get { return graphicsDevice; }
            set { graphicsDevice = value; }
        }

        //For the emulator
        public int CurrentNamespaceLocId;
        public int CurrentClassLocId;
        public int CurrentOpcodeLocId;
        public static bool Halt;

        public bool HALT
        {
            get { return Halt; }
            set { Halt = value; }
        }

        //Registers
        public static Registers registers;

        public Registers register
        {
            get { return registers; }
            set { registers = value; }
        }
        
        public static Flags flags;
        public Flags flag
        {
            get { return flags; }
            set { flags = value; }
        }

        public int TotalOpcodes
        {
            get
            {
                int total = 0;
                for (int i = 0; i < namespaces.Count; i++)
                    for (int c = 0; c < namespaces.Values[i].classes.Count; c++)
                        total += namespaces.Values[i].classes.Values[c].opcodes.Count;
                return total;
            }
        }

        public static bool inConsole { get; private set; }
        public static VideoMode videoMode = VideoMode.Graphics;

        public AssemblerExecute()
        {
            AllowNextStep = true;
        }
        ~AssemblerExecute()
        {
            HALT = false;
            ByteExecution = null;
            namespaces.Clear();
            namespaces = null;
            pushes.Clear();
            pushes = null;
            inConsole = false;
        }

        public unsafe void Execute(byte[] bytes)
        {
            //Initialize the registers
            registers = new Registers();
            flags = new Flags();
            flags.InterruptFlag = true; //default

            this.ByteExecution = bytes;
            pushes = new StackCollection();
            namespaces = new SortedList<byte, AssemblerNamespace>();

            Halt = false;

            //Check if we are in a console
            inConsole = Convert.ToBoolean(Kernel32.GetStdHandle(-10).ToInt32()) ? true : false;

            string CurrentNamespace = "";
            bool BodyOpen = false;

            //Increment id's
            byte NamespaceId = 0;
            byte ClassId = 0;

            string LastKeyword = "";
            byte LastNamespaceId = 0;
            byte LastClassId = 0;
            string LastNamespaceName = "";
            string LastClassName = "";

            bool InsideBody = false;

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < ByteExecution.Length; i += 2)
            {
                switch (ByteExecution[i])
                {
                    case 0: //namespace, class
                        switch (ByteExecution[i + 1])
                        {
                            case 0: //namespace
                                if (!namespaces.ContainsKey(ByteExecution[i + 2]))
                                    namespaces.Add(ByteExecution[i + 2], new AssemblerNamespace(ByteExecution[i + 2]));
                                LastNamespaceId = ByteExecution[i + 2];
                                i++;
                                break;
                            case 1: //class
                                Accessor access = Accessor.Private;
                                if(ByteExecution[i + 2] == 1)
                                    access = Accessor.Private;
                                else if(ByteExecution[i + 2] == 2)
                                    access = Accessor.Public;
                                else if(ByteExecution[i + 2] == 4)
                                    access = Accessor.Protected;

                                AssemblerClass AsmClass = new AssemblerClass(ByteExecution[i + 4], access);
                                namespaces[ByteExecution[i + 3]].AddClass(AsmClass);
                                LastClassId = ByteExecution[i + 4];
                                i += 3;
                                break;
                        }
                        break;
                    case 1: //push, add etc
                        switch (ByteExecution[i + 1])
                        {
                            case 0://push
                            {
                                if (ByteExecution[i + 2] == 0)
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(BitConverter.ToInt32(ByteExecution, i + 3), typeof(Int32), 0, i), 0, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(BitConverter.ToInt32(ByteExecution, i + 3), typeof(Int32), 0, i), 0, i), "PUSH " + BitConverter.ToInt32(ByteExecution, i + 3), i));
                                }
                                else if (ByteExecution[i + 2] == 1)
                                {
                                    int StringLength = BitConverter.ToInt32(ByteExecution, i + 3);
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(ASCIIEncoding.ASCII.GetString(ByteExecution, i + 7, StringLength), typeof(String), 0, i), 0, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(ASCIIEncoding.ASCII.GetString(ByteExecution, i + 7, StringLength), typeof(String), 0, i), 0, i), "PUSH " + ASCIIEncoding.ASCII.GetString(ByteExecution, i + 7, StringLength), i));
                                    i += StringLength;
                                }
                                else if (ByteExecution[i + 2] == 2)
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(BitConverter.ToInt64(ByteExecution, i + 3), typeof(Int64), 0, i), 0, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(BitConverter.ToInt64(ByteExecution, i + 3), typeof(Int64), 0, i), 0, i), "PUSH " + BitConverter.ToInt64(ByteExecution, i + 3), i));
                                    i += 4;
                                }
                                else if (ByteExecution[i + 2] == 3) //eax
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.EAX, registers.EAX.GetType(), 0, i), Register.EAX, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.EAX, registers.EAX.GetType(), 0, i), Register.EAX, i), "PUSH EAX", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 4) //ebx
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.EBX, registers.EBX.GetType(), 0, i), Register.EBX, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.EBX, registers.EBX.GetType(), 0, i), Register.EBX, i), "PUSH EBX", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 5) //ecx
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.ECX, registers.ECX.GetType(), 0, i), Register.ECX, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.ECX, registers.ECX.GetType(), 0, i), Register.ECX, i), "PUSH ECX", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 6) //edx
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.EDX, registers.EDX.GetType(), 0, i), Register.EDX, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.EDX, registers.EDX.GetType(), 0, i), Register.EDX, i), "PUSH EDX", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 7) //esi
                                {
                                    //namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.ESI, registers.ESI.GetType(), 0, i), Register.ESI, i));
                                    //ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.ESI, registers.ESI.GetType(), 0, i), Register.ESI, i), "PUSH ESI", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 8) //edi
                                {
                                    //namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.EDI, registers.EDI.GetType(), 0, i), Register.EDI, i));
                                    //ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.EDI, registers.EDI.GetType(), 0, i), Register.EDI, i), "PUSH EDI", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 9) //ebp
                                {
                                    //namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.EBP, registers.EBP.GetType(), 0, i), Register.EBP, i));
                                    //ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.EBP, registers.EBP.GetType(), 0, i), Register.EBP, i), "PUSH EBP", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 10) //esp
                                {
                                    //namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.ESP, registers.ESP.GetType(), 0, i), Register.ESP, i));
                                    //ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.ESP, registers.ESP.GetType(), 0, i), Register.ESP, i), "PUSH ESP", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 11) //eip
                                {
                                    //namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.EIP, registers.EIP.GetType(), 0, i), Register.EIP, i));
                                    //ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.EIP, registers.EIP.GetType(), 0, i), Register.EIP, i), "PUSH EIP", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 12) //cx
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.CX, registers.CX.GetType(), 0, i), Register.CX, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.CX, registers.CX.GetType(), 0, i), Register.CX, i), "PUSH CX", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 13) //dx
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.DX, registers.DX.GetType(), 0, i), Register.DX, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.DX, registers.DX.GetType(), 0, i), Register.DX, i), "PUSH DX", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 14) //bx
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(registers.BX, registers.BX.GetType(), 0, i), Register.BX, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(registers.BX, registers.BX.GetType(), 0, i), Register.BX, i), "PUSH BX", i));
                                    i++;
                                    break;
                                }
                                else if (ByteExecution[i + 2] == 15)
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(BitConverter.ToUInt32(ByteExecution, i + 3), typeof(Int32), 0, i), 0, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(BitConverter.ToUInt32(ByteExecution, i + 3), typeof(Int32), 0, i), 0, i), "PUSH " + BitConverter.ToUInt32(ByteExecution, i + 3), i));
                                }
                                else if (ByteExecution[i + 2] == 16)
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(BitConverter.ToUInt64(ByteExecution, i + 3), typeof(Int32), 0, i), 0, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(BitConverter.ToUInt64(ByteExecution, i + 3), typeof(Int32), 0, i), 0, i), "PUSH " + BitConverter.ToUInt64(ByteExecution, i + 3), i));
                                    i += 4;
                                }
                                i += 5;
                                break;
                            }
                            case 1: //call
                                Acall call = null;
                                UIntPtr memAddress = UIntPtr.Zero;
                                string ApiName = "";

                                switch (ByteExecution[i + 2])
                                {
                                    case 1:
                                        call = new NetEngine.MessageBox();
                                        ApiName = "MessageBoxW";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("user32"), "MessageBoxW");
                                        break;
                                    case 2:
                                        call = new NetEngine.IO.CreateFile();
                                        ApiName = "CreateFileA";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "CreateFileA");
                                        break;
                                    case 3:
                                        call = new NetEngine.Sockets.Listen();
                                        ApiName = "listen";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("ws2_32"), "listen");
                                        break;
                                    case 4:
                                        call = new NetEngine.IO.WriteFile();
                                        ApiName = "WriteFileA";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "WriteFileA");
                                        break;
                                    case 5:
                                        call = new NetEngine.Beep();
                                        ApiName = "Beep";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "Beep");
                                        break;
                                    case 6:
                                        call = new NetEngine.AllocConsole();
                                        ApiName = "AllocConsole";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "AllocConsole");
                                        break;
                                    case 7:
                                        call = new NetEngine.AttachConsole();
                                        ApiName = "AttachConsole";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "AttachConsole");
                                        break;
                                    case 8:
                                        call = new NetEngine.ActivateActCtx();
                                        ApiName = "ActivateActCtx";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "ActivateActCtx");
                                        break;
                                    case 9:
                                        call = new NetEngine.IsDebuggerPresent();
                                        ApiName = "IsDebuggerPresent";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "IsDebuggerPresent");
                                        break;
                                    case 10:
                                        call = new NetEngine.DebugActiveProcess();
                                        ApiName = "DebugActiveProcess";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "DebugActiveProcess");
                                        break;
                                    case 11:
                                        call = new NetEngine.CloseHandle();
                                        ApiName = "CloseHandle";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "CloseHandle");
                                        break;
                                    case 12:
                                        call = new NetEngine.IO.CopyFile();
                                        ApiName = "CopyFile";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "CopyFile");
                                        break;
                                    case 13:
                                        call = new NetEngine.Console.CreateConsoleScreenBuffer();
                                        ApiName = "CreateConsoleScreenBuffer";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "CreateConsoleScreenBuffer");
                                        break;
                                    case 14:
                                        call = new NetEngine.IO.CreateDirectory();
                                        ApiName = "CreateDirectory";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "CreateDirectory");
                                        break;
                                    case 15:
                                        call = new NetEngine.IO.CreateDirectoryEx();
                                        ApiName = "CreateDirectoryEx";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "CreateDirectoryEx");
                                        break;
                                    case 16:
                                        call = new NetEngine.CreateEvent();
                                        ApiName = "CreateEvent";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "CreateEvent");
                                        break;
                                    case 17:
                                        call = new NetEngine.CreateMutex();
                                        ApiName = "CreateMutexA";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "CreateMutexA");
                                        break;
                                    case 18:
                                        call = new NetEngine.OpenMutex();
                                        ApiName = "OpenMutexA";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "OpenMutexA");
                                        break;
                                    case 19:
                                        call = new NetEngine.GetLastError();
                                        ApiName = "GetLastError";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "GetLastError");
                                        break;
                                    case 20:
                                        call = new NetEngine.WaitForSingleObject();
                                        ApiName = "WaitForSingleObject";
                                        memAddress = Kernel32.GetProcAddress(Kernel32.GetModuleHandle("Kernel32"), "WaitForSingleObject");
                                        break;
                                }
                                if (call != null)
                                {
                                    call.MemAddress = i;
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(call);
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(call, "CALL " + ApiName + " (" + memAddress.ToUInt32().ToString("X") + ")", i));
                                }
                                i++;
                                break;
                            case 2: //nop
                                namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new NOP(new IData(0, typeof(NOP), 0, i), i));
                                ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new NOP(new IData(0, typeof(NOP), 0, i), i), "NOP", i));
                                break;
                            case 3: //inc
                                if (ByteExecution[i + 2] == 0)
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new INC(ByteExecution[i + 3], VariableType.Variable, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new INC(ByteExecution[i + 3], VariableType.Variable, i), "INC " + ByteExecution[i + 3], i));
                                }
                                else
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new INC((Register)ByteExecution[i + 3], VariableType.Register, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new INC(ByteExecution[i + 3], VariableType.Variable, i), "INC " + (Register)ByteExecution[i + 3], i));
                                }
                                i += 2;
                                break;
                            case 4: //dec
                                if (ByteExecution[i + 2] == 0)
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new DEC(ByteExecution[i + 3], VariableType.Variable, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new DEC(ByteExecution[i + 3], VariableType.Variable, i), "DEC $" + ByteExecution[i + 3], i));
                                }
                                else
                                {
                                    namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new DEC((Register)ByteExecution[i + 3], VariableType.Register, i));
                                    ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new DEC((Register)ByteExecution[i + 3], VariableType.Register, i), "DEC " + (Register)ByteExecution[i + 3], i));
                                }
                                i += 2;
                                break;
                            case 5: //mov
                                switch (ByteExecution[i + 2])
                                {
                                    case 0://register - AH
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.AH, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.AH, i), "MOV AH, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.AH, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.AH, i, (Register)ByteExecution[i + 4]), "MOV AH, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 1: //register - EAX
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.EAX, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.EAX, i), "MOV EAX, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.EAX, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.EAX, i, (Register)ByteExecution[i + 4]), "MOV EAX, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 2: //register - AL
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.AL, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.AL, i), "MOV AL, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.AL, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.AL, i, (Register)ByteExecution[i + 4]), "MOV AL, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 3: //register - EBX
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.EBX, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.EBX, i), "MOV EBX, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.EBX, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.EBX, i, (Register)ByteExecution[i + 4]), "MOV EBX, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }

                                    case 4: //register - ECX
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.ECX, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.ECX, i), "MOV ECX, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.ECX, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.ECX, i, (Register)ByteExecution[i + 4]), "MOV ECX, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 5: //register - EDX
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.EDX, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.EDX, i), "MOV EDX, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.EDX, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.EDX, i, (Register)ByteExecution[i + 4]), "MOV EDX, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 6: //register - ESI
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.ESI, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.ESI, i), "MOV ESI, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.ESI, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.ESI, i, (Register)ByteExecution[i + 4]), "MOV ESI, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 7: //register - EDI
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.EDI, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.EDI, i), "MOV EDI, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.EDI, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.EDI, i, (Register)ByteExecution[i + 4]), "MOV EDI, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 8: //register - AX
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.AX, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.AX, i), "MOV AX, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.AX, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.AX, i, (Register)ByteExecution[i + 4]), "MOV AX, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 9: //register - CX
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.CX, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.CX, i), "MOV CX, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.CX, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.CX, i, (Register)ByteExecution[i + 4]), "MOV CX, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 10: //register - DX
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.DX, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.DX, i), "MOV DX, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.DX, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.DX, i, (Register)ByteExecution[i + 4]), "MOV DX, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                    case 11: //register - BX
                                    {
                                        if (ByteExecution[i + 3] == 0)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.BX, i));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(BitConverter.ToInt16(ByteExecution, i + 4), typeof(short), 0, i), Register.BX, i), "MOV BX, " + BitConverter.ToInt16(ByteExecution, i + 4), i));
                                            i += 4;
                                        }
                                        else if (ByteExecution[i + 3] == 1)
                                        {
                                            namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new MOV(new IData(0, typeof(short), 0, i), Register.BX, i, (Register)ByteExecution[i + 4]));
                                            ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new MOV(new IData(0, typeof(short), 0, i), Register.BX, i, (Register)ByteExecution[i + 4]), "MOV BX, " + (Register)ByteExecution[i + 4], i));
                                            i += 3;
                                        }
                                        break;
                                    }
                                }
                                break;
                            case 6: //interrupt
                                switch (ByteExecution[i + 2])
                                {
                                    case 10:
                                        namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new INT10(i));
                                        ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new INT10(i), "INT " + ByteExecution[i + 2], i));
                                        break;
                                    case 16:
                                        namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new INT16(i));
                                        ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new INT16(i), "INT " + ByteExecution[i + 2], i));
                                        break;
                                    case 21:
                                        namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new INT21(i));
                                        ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new INT21(i), "INT " + ByteExecution[i + 2], i));
                                        break;
                                }
                                i++;
                                break;
                            case 8: //Jumps
                                switch (ByteExecution[i + 2])
                                {
                                    case 0: //jmp
                                        namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new JMP(BitConverter.ToInt16(ByteExecution, i + 3), i));
                                        ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new JMP(BitConverter.ToInt16(ByteExecution, i + 3), i), "JMP 0x" + BitConverter.ToInt16(ByteExecution, i + 3).ToString("X"), i));
                                        break;
                                    case 1: //jz
                                        namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new JZ(BitConverter.ToInt16(ByteExecution, i + 3), i));
                                        ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new JZ(BitConverter.ToInt16(ByteExecution, i + 3), i), "JZ 0x" + BitConverter.ToInt16(ByteExecution, i + 3).ToString("X"), i));
                                        break;
                                    case 2: //jnz
                                        namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new JNZ(BitConverter.ToInt16(ByteExecution, i + 3), i));
                                        ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new JNZ(BitConverter.ToInt16(ByteExecution, i + 3), i), "JNZ 0x" + BitConverter.ToInt16(ByteExecution, i + 3).ToString("X"), i));
                                        break;
                                    case 3: //jne
                                        namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new JNE(BitConverter.ToInt16(ByteExecution, i + 3), i));
                                        ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new JNE(BitConverter.ToInt16(ByteExecution, i + 3), i), "JNE 0x" + BitConverter.ToInt16(ByteExecution, i + 3).ToString("X"), i));
                                        break;
                                    case 4: //je
                                        namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new JE(BitConverter.ToInt16(ByteExecution, i + 3), i));
                                        ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new JE(BitConverter.ToInt16(ByteExecution, i + 3), i), "JE 0x" + BitConverter.ToInt16(ByteExecution, i + 3).ToString("X"), i));
                                        break;
                                }
                                i += 3;
                                break;
                            case 9: //cmp
                                object Exp1 = null;
                                if (ByteExecution[i + 2] == 0) //register
                                    Exp1 = (Register)(ByteExecution[i + 3]);

                                object Exp2 = null;
                                if (ByteExecution[i + 4] == 0) //register
                                    Exp2 = (Register)(ByteExecution[i + 5]);
                                else if (ByteExecution[i + 4] == 1) //const number
                                    Exp2 = BitConverter.ToInt64(ByteExecution, i + 5);

                                namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new CMP(i, Exp1, Exp2));
                                ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new CMP(i, Exp1, Exp2), "CMP " + Exp1 + ", " + Exp2, i));
                                i += 11;
                                break;
                        }
                        break;
                    case 2:
                        switch (ByteExecution[i + 1])
                        {
                            case 0: //write
                                int StringLength = BitConverter.ToInt32(ByteExecution, i + 2);
                                namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new PUSH(new IData(ASCIIEncoding.ASCII.GetString(ByteExecution, i + 6, StringLength), typeof(String), 0, i), 0, i));
                                namespaces.Values[NamespaceId].classes.Values[ClassId].opcodes.Add(new Write());
                                ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new PUSH(new IData(ASCIIEncoding.ASCII.GetString(ByteExecution, i + 6, StringLength), typeof(String), 0, i), 0, i), "PUSH " + ASCIIEncoding.ASCII.GetString(ByteExecution, i + 6, StringLength), i));
                                ParsedOpcodeEvent.onParsedOpcodeEvent(new ParsedOpcodeEventArgs(new Write(), "CALL Write", i));
                                i += (StringLength + 4);
                                break;
                        }
                        break;
                    case 3:
                        switch (ByteExecution[i + 1])
                        {
                            case 0:
                                switch (ByteExecution[i + 2])
                                {
                                    case 0: //string
                                        int StringLength = BitConverter.ToInt32(ByteExecution, i + 4);
                                        string StringValue = ASCIIEncoding.ASCII.GetString(ByteExecution, i + 8, StringLength);
                                        namespaces[NamespaceId].classes[ClassId].Variables.Add(ByteExecution[i + 3], new Variable(ByteExecution[i + 3], StringValue, typeof(String), i));
                                        i += (StringLength+6);
                                        break;
                                    case 1: //int32
                                        namespaces[NamespaceId].classes[ClassId].Variables.Add(ByteExecution[i + 3], new Variable(ByteExecution[i + 3], BitConverter.ToInt32(ByteExecution, i + 4), typeof(Int32), i));
                                        i += 6;
                                        break;
                                }
                                break;
                        }
                        break;
                    case 5:
                        switch (ByteExecution[i + 1])
                        {
                            case 0://open
                                break;
                            case 1://close
                                break;
                        }
                        break;
                }
            }
            sw.Stop();
        }

        public void NextStep()
        {
            if (AllowNextStep)
            {
                AllowNextStep = false;
                try
                {
                    for (int i = CurrentNamespaceLocId; i < namespaces.Count; i++)
                    {
                        for (int c = CurrentClassLocId; c < namespaces.Values[i].classes.Count; c++)
                        {
                            AsmOpcode CurrentOpcode = null;
                            if (CurrentOpcodeLocId < namespaces.Values[i].classes.Values[c].opcodes.Count && !HALT)
                            {
                                CurrentOpcode = namespaces.Values[i].classes.Values[c].opcodes[CurrentOpcodeLocId];
                                CurrentOpcodeEvent.onParsedOpcodeEvent(new CurrentOpcodeEventArgs(CurrentOpcode, CurrentOpcodeLocId++));
                                registers.IP = (short)((AsmOpcode)(CurrentOpcode)).MemAddress;

                                if (CurrentOpcode.GetType() == typeof(PUSH))
                                {
                                    //set the current EAX value
                                    switch (((PUSH)CurrentOpcode).register)
                                    {
                                        case Register.AH: ((PUSH)CurrentOpcode).value = registers.AH; break;
                                        case Register.AL: ((PUSH)CurrentOpcode).value = registers.AL; break;
                                        case Register.AX: ((PUSH)CurrentOpcode).value = registers.AX; break;
                                        case Register.BX: ((PUSH)CurrentOpcode).value = registers.BX; break;
                                        case Register.CX: ((PUSH)CurrentOpcode).value = registers.CX; break;
                                        case Register.DX: ((PUSH)CurrentOpcode).value = registers.DX; break;
                                        case Register.EAX: ((PUSH)CurrentOpcode).value = registers.EAX; break;
                                        case Register.ECX: ((PUSH)CurrentOpcode).value = registers.ECX; break;
                                        case Register.EBX: ((PUSH)CurrentOpcode).value = registers.EBX; break;
                                        case Register.EDX: ((PUSH)CurrentOpcode).value = registers.EDX; break;
                                    }
                                    pushes.Add((PUSH)CurrentOpcode);
                                }
                                else if (CurrentOpcode.GetType().IsSubclassOf(typeof(Acall)))
                                {
                                    try
                                    {
                                        Acall call = (Acall)(CurrentOpcode);
                                        //Check if a push is from 1 of the registers

                                        PUSH[] pushs = new PUSH[pushes.Count];
                                        for (int p = 0; p < pushes.Count; p++) //reversed stack
                                        {
                                            if (pushes[p].register == Register.EAX)
                                                pushs[p] = new PUSH(new IData(registers.EAX, typeof(int), 0, i), 0, i);
                                            else if (pushes[p].register == Register.AH)
                                                pushs[p] = new PUSH(new IData(registers.AH, typeof(byte), 0, i), 0, i);
                                            else if (pushes[p].register == Register.AL)
                                                pushs[p] = new PUSH(new IData(registers.AL, typeof(byte), 0, i), 0, i);
                                            else if (pushes[p].register == Register.AX)
                                                pushs[p] = new PUSH(new IData(registers.AX, typeof(short), 0, i), 0, i);
                                            else if (pushes[p].register == Register.CX)
                                                pushs[p] = new PUSH(new IData(registers.CX, typeof(short), 0, i), 0, i);
                                            else if (pushes[p].register == Register.DX)
                                                pushs[p] = new PUSH(new IData(registers.DX, typeof(short), 0, i), 0, i);
                                            else if (pushes[p].register == Register.ECX)
                                                pushs[p] = new PUSH(new IData(registers.ECX, typeof(int), 0, i), 0, i);
                                            else if (pushes[p].register == Register.BX)
                                                pushs[p] = new PUSH(new IData(registers.BX, typeof(short), 0, i), 0, i);
                                            else if (pushes[p].register == Register.EBX)
                                                pushs[p] = new PUSH(new IData(registers.EBX, typeof(int), 0, i), 0, i);
                                            else if (pushes[p].register == Register.EDX)
                                                pushs[p] = new PUSH(new IData(registers.EDX, typeof(int), 0, i), 0, i);
                                            else
                                                pushs[p] = pushes[p];
                                        }

                                        call.args = pushs;
                                        call.Call();
                                        pushes.Clear();
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine("error: " + ex.Message);
                                    }
                                }
                                //AsmOpcodes
                                else if (CurrentOpcode.GetType() == typeof(INC))
                                {
                                    switch (((INC)CurrentOpcode).type)
                                    {
                                        case VariableType.Variable:
                                        {
                                            int CurValue = (int)namespaces.Values[i].classes.Values[c].Variables[((INC)CurrentOpcode).VariableId].value;
                                            CurValue++;
                                            //set value
                                            namespaces.Values[i].classes.Values[c].Variables[((INC)CurrentOpcode).VariableId].value = CurValue;
                                            break;
                                        }
                                        case VariableType.Register:
                                        {
                                            switch (((INC)CurrentOpcode).register)
                                            {
                                                case Register.AH: registers.AH++; break;
                                                case Register.AL: registers.AL++; break;
                                                case Register.AX: registers.AX++; break;
                                                case Register.EAX: registers.EAX++; break;
                                                //case Register.EBP: registers.EBP++; break;
                                                case Register.EBX: registers.EBX++; break;
                                                case Register.ECX: registers.ECX++; break;
                                                //case Register.EDI: registers.EDI++; break;
                                                case Register.EDX: registers.EDX++; break;
                                                //case Register.ESI: registers.ESI++; break;
                                                //case Register.ESP: registers.ESP++; break;
                                                case Register.CX: registers.CX++; break;
                                                case Register.DX: registers.DX++; break;
                                                case Register.BX: registers.BX++; break;
                                            }
                                            break;
                                        }
                                    }
                                }
                                else if (CurrentOpcode.GetType() == typeof(DEC))
                                {
                                    switch (((DEC)CurrentOpcode).type)
                                    {
                                        case VariableType.Variable:
                                        {
                                            int CurValue = (int)namespaces.Values[i].classes.Values[c].Variables[((DEC)CurrentOpcode).VariableId].value;
                                            CurValue++;
                                            //set value
                                            namespaces.Values[i].classes.Values[c].Variables[((DEC)CurrentOpcode).VariableId].value = CurValue;
                                            break;
                                        }
                                        case VariableType.Register:
                                        {
                                            switch (((DEC)CurrentOpcode).register)
                                            {
                                                case Register.AH: registers.AH--; break;
                                                case Register.AL: registers.AL--; break;
                                                case Register.AX: registers.AX--; break;
                                                case Register.EAX: registers.EAX--; break;
                                                //case Register.EBP: registers.EBP--; break;
                                                case Register.EBX: registers.EBX--; break;
                                                case Register.ECX: registers.ECX--; break;
                                                //case Register.EDI: registers.EDI--; break;
                                                case Register.EDX: registers.EDX--; break;
                                                //case Register.ESI: registers.ESI--; break;
                                                //case Register.ESP: registers.ESP--; break;
                                                case Register.CX: registers.CX--; break;
                                                case Register.DX: registers.DX--; break;
                                                case Register.BX: registers.BX--; break;
                                            }
                                            break;
                                        }
                                    }
                                }
                                else if (CurrentOpcode.GetType() == typeof(MOV))
                                {
                                    if (((MOV)CurrentOpcode).register == Register.AH)
                                    {
                                        switch (((MOV)CurrentOpcode).RegisterValue)
                                        {
                                            case Register.AH: registers.AH = registers.AH; break;
                                            case Register.AL: registers.AH = registers.AL; break;
                                            case Register.AX: registers.AH = (byte)registers.AX; break;
                                            case Register.CX: registers.AH = (byte)registers.CX; break;
                                            case Register.DX: registers.AH = (byte)registers.DX; break;
                                            case Register.EAX: registers.AH = (byte)registers.EAX; break;
                                            case Register.ECX: registers.AH = (byte)registers.ECX; break;
                                            case Register.BX: registers.AH = (byte)registers.BX; break;
                                            case Register.EBX: registers.AH = (byte)registers.EBX; break;
                                            case Register.EDX: registers.AH = (byte)registers.EDX; break;
                                            default: registers.AH = Convert.ToByte(((MOV)CurrentOpcode).value); break;
                                        }
                                    }
                                    else if (((MOV)CurrentOpcode).register == Register.EAX)
                                    {
                                        switch (((MOV)CurrentOpcode).RegisterValue)
                                        {
                                            case Register.AH: registers.EAX = registers.AH; break;
                                            case Register.AL: registers.EAX = registers.AL; break;
                                            case Register.AX: registers.EAX = registers.AX; break;
                                            case Register.CX: registers.EAX = registers.CX; break;
                                            case Register.DX: registers.EAX = registers.DX; break;
                                            case Register.EAX: registers.EAX = registers.EAX; break;
                                            case Register.ECX: registers.EAX = registers.ECX; break;
                                            case Register.BX: registers.EAX = registers.BX; break;
                                            case Register.EBX: registers.EAX = registers.EBX; break;
                                            case Register.EDX: registers.EAX = registers.EDX; break;
                                            default: registers.EAX = Convert.ToInt32(((MOV)CurrentOpcode).value); break;
                                        }
                                    }
                                    else if (((MOV)CurrentOpcode).register == Register.AL)
                                    {
                                        switch (((MOV)CurrentOpcode).RegisterValue)
                                        {
                                            case Register.AH: registers.AL = registers.AH; break;
                                            case Register.AL: registers.AL = registers.AL; break;
                                            case Register.AX: registers.AL = (byte)registers.AX; break;
                                            case Register.CX: registers.AL = (byte)registers.CX; break;
                                            case Register.DX: registers.AL = (byte)registers.DX; break;
                                            case Register.EAX: registers.AL = (byte)registers.EAX; break;
                                            case Register.ECX: registers.AL = (byte)registers.ECX; break;
                                            case Register.BX: registers.AL = (byte)registers.BX; break;
                                            case Register.EBX: registers.AL = (byte)registers.EBX; break;
                                            case Register.EDX: registers.AL = (byte)registers.EDX; break;
                                            default: registers.AL = Convert.ToByte(((MOV)CurrentOpcode).value); break;
                                        }
                                    }
                                    else if (((MOV)CurrentOpcode).register == Register.ECX)
                                    {
                                        switch (((MOV)CurrentOpcode).RegisterValue)
                                        {
                                            case Register.AH: registers.ECX = registers.AH; break;
                                            case Register.AL: registers.ECX = registers.AL; break;
                                            case Register.AX: registers.ECX = registers.AX; break;
                                            case Register.CX: registers.ECX = registers.CX; break;
                                            case Register.DX: registers.ECX = registers.DX; break;
                                            case Register.EAX: registers.ECX = registers.EAX; break;
                                            case Register.ECX: registers.ECX = registers.ECX; break;
                                            case Register.BX: registers.ECX = registers.BX; break;
                                            case Register.EBX: registers.ECX = registers.EBX; break;
                                            case Register.EDX: registers.ECX = registers.EDX; break;
                                            default: registers.ECX = Convert.ToInt32(((MOV)CurrentOpcode).value); break;
                                        }
                                    }
                                    else if (((MOV)CurrentOpcode).register == Register.AX)
                                    {
                                        switch (((MOV)CurrentOpcode).RegisterValue)
                                        {
                                            case Register.AH: registers.AX = registers.AH; break;
                                            case Register.AL: registers.AX = registers.AL; break;
                                            case Register.AX: registers.AX = registers.AX; break;
                                            case Register.CX: registers.AX = registers.CX; break;
                                            case Register.DX: registers.AX = registers.DX; break;
                                            case Register.EAX: registers.AX = (short)registers.EAX; break;
                                            case Register.ECX: registers.AX = (short)registers.ECX; break;
                                            case Register.BX: registers.AX = (byte)registers.BX; break;
                                            case Register.EBX: registers.AX = (short)registers.EBX; break;
                                            case Register.EDX: registers.AX = (short)registers.EDX; break;
                                            default: registers.AX = Convert.ToInt16(((MOV)CurrentOpcode).value); break;
                                        }
                                    }
                                    else if (((MOV)CurrentOpcode).register == Register.CX)
                                    {
                                        switch (((MOV)CurrentOpcode).RegisterValue)
                                        {
                                            case Register.AH: registers.CX = registers.AH; break;
                                            case Register.AL: registers.CX = registers.AL; break;
                                            case Register.AX: registers.CX = registers.AX; break;
                                            case Register.CX: registers.CX = registers.CX; break;
                                            case Register.DX: registers.CX = registers.DX; break;
                                            case Register.EAX: registers.CX = (short)registers.EAX; break;
                                            case Register.ECX: registers.CX = (short)registers.ECX; break;
                                            case Register.BX: registers.CX = (byte)registers.BX; break;
                                            case Register.EBX: registers.CX = (short)registers.EBX; break;
                                            case Register.EDX: registers.CX = (short)registers.EDX; break;
                                            default: registers.CX = Convert.ToInt16(((MOV)CurrentOpcode).value); break;
                                        }
                                    }
                                    else if (((MOV)CurrentOpcode).register == Register.DX)
                                    {
                                        switch (((MOV)CurrentOpcode).RegisterValue)
                                        {
                                            case Register.AH: registers.DX = registers.AH; break;
                                            case Register.AL: registers.DX = registers.AL; break;
                                            case Register.AX: registers.DX = registers.AX; break;
                                            case Register.CX: registers.DX = registers.CX; break;
                                            case Register.DX: registers.DX = registers.DX; break;
                                            case Register.EAX: registers.DX = (short)registers.EAX; break;
                                            case Register.ECX: registers.DX = (short)registers.ECX; break;
                                            case Register.BX: registers.DX = (byte)registers.BX; break;
                                            case Register.EBX: registers.DX = (short)registers.EBX; break;
                                            case Register.EDX: registers.DX = (short)registers.EDX; break;
                                            default: registers.DX = Convert.ToInt16(((MOV)CurrentOpcode).value); break;
                                        }
                                    }
                                    else if (((MOV)CurrentOpcode).register == Register.BX)
                                    {
                                        switch (((MOV)CurrentOpcode).RegisterValue)
                                        {
                                            case Register.AH: registers.BX = registers.AH; break;
                                            case Register.AL: registers.BX = registers.AL; break;
                                            case Register.AX: registers.BX = registers.AX; break;
                                            case Register.CX: registers.BX = registers.CX; break;
                                            case Register.DX: registers.BX = registers.DX; break;
                                            case Register.EAX: registers.BX = (short)registers.EAX; break;
                                            case Register.ECX: registers.BX = (short)registers.ECX; break;
                                            case Register.BX: registers.BX = registers.BX; break;
                                            case Register.EBX: registers.BX = (short)registers.EBX; break;
                                            case Register.EDX: registers.BX = (short)registers.EDX; break;
                                            default: registers.BX = Convert.ToInt16(((MOV)CurrentOpcode).value); break;
                                        }
                                    }

                                }
                                else if (CurrentOpcode.GetType() == typeof(INT10) ||
                                         CurrentOpcode.GetType() == typeof(INT16) ||
                                         CurrentOpcode.GetType() == typeof(INT21))
                                {
                                    ((IInterrupt)(CurrentOpcode)).Execute();
                                }
                                else if (CurrentOpcode.GetType() == typeof(JMP) || CurrentOpcode.GetType() == typeof(JZ) ||
                                         CurrentOpcode.GetType() == typeof(JNZ) || CurrentOpcode.GetType() == typeof(JNE) ||
                                         CurrentOpcode.GetType() == typeof(JE))
                                {
                                    if ((CurrentOpcode.GetType() == typeof(JZ)) && flags.ZeroFlag == true)
                                        JumpToAddress(((JZ)CurrentOpcode).JumpMemAddress, i, c);
                                    else if (CurrentOpcode.GetType() == typeof(JMP))
                                        JumpToAddress(((JMP)CurrentOpcode).JumpMemAddress, i, c);
                                    else if ((CurrentOpcode.GetType() == typeof(JNZ)) && flags.ZeroFlag == false)
                                        JumpToAddress(((JNZ)CurrentOpcode).JumpMemAddress, i, c);
                                    else if ((CurrentOpcode.GetType() == typeof(JNE)) && flags.ZeroFlag == false)
                                        JumpToAddress(((JNE)CurrentOpcode).JumpMemAddress, i, c);
                                    else if ((CurrentOpcode.GetType() == typeof(JE)) && flags.ZeroFlag == true)
                                        JumpToAddress(((JE)CurrentOpcode).JumpMemAddress, i, c);
                                }
                                else if (CurrentOpcode.GetType() == typeof(CMP))
                                {
                                    ((CMP)CurrentOpcode).Compare();
                                }

                                AllowNextStep = true;
                                return; //just execute 1 instruction
                            }
                            else
                            {
                                CurrentOpcodeLocId = 0;
                            }
                            CurrentClassLocId++;
                        }
                        CurrentNamespaceLocId++;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message + "\r\n" + ex.StackTrace);
                }
                HALT = true;
                HaltEvent.onHalt();
                AllowNextStep = true;
            }
        }

        public void JumpToAddress(int MemAddress, int CurrentNamespace, int CurrentClass)
        {
            for (int x = 0; x < namespaces.Values[CurrentNamespace].classes.Values[CurrentClass].opcodes.Count; x++)
            {
                if (((AsmOpcode)(namespaces.Values[CurrentNamespace].classes.Values[CurrentClass].opcodes[x])).MemAddress == MemAddress)
                {
                    CurrentOpcodeLocId = x;
                    return;
                }
            }
            System.Windows.Forms.MessageBox.Show("Memory access vialoation, Unable to access 0x" + MemAddress.ToString("X8"), "AsmEngine", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
    }
}