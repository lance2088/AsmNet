using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src.Opcodes.INC;
using Asm.Net.src.Interfaces;
using System.Diagnostics;
using Asm.Net.src.Opcodes.AsmNet;
using Asm.Net.src.Opcodes.MOV;
using Asm.Net.src.Opcodes;
using Asm.Net.src.Sections;
using Asm.Net.src.Opcodes.Jumps;
using System.Runtime.InteropServices;

namespace Asm.Net.src.Hardware
{
    /// <summary>
    /// This class should be renamed to cpu but it can't be a cpu without ALU >.<
    /// And there is no memory what so ever sooo... lets use system memory ? :3
    /// Soon I will try to make the ALU etc... but first the memory (NOT THIS CLASS)
    /// only the CPU can execute instructions and the ram is just the memory
    /// at the moment this class is 50% cpu and 50% memory
    /// </summary>
    public class Processor : Hardware
    {
        public int MaximumSize { get; set; }
        
        private bool _HALT { get; set; }
        public bool IncreaseEIP { get; private set; }
        public bool IsDebugMode { get; set; }
        public ulong TotalExecutedInstructions { get; private set; }

        private Flags flags;
        public Flags Flags
        {
            get { return flags; }
        }


        public Registers registers { get; private set; }
        public Memory RamMemory { get; set; }
        public DataSection dataSection { get; set; }

        internal AsmNet.GetCurrentInstructionEventHandler GetCurrentInstructionEventHandler;
        internal AsmNet.GetDataSectionReaderHandler GetDataSectionReader;
        internal AsmNet.GetHALTEventHandler GetHALTEvent;
        private bool isExecutingInstruction = false; //prevents from going futher while it's executing another instruction
        private VirtualAddress PreviousEIP = new VirtualAddress(4, Options.MemoryBaseAddress);

        public List<object> Stack { get; set; }

        public bool HALT
        {
            get
            {
                return _HALT;
            }
            set
            {
                _HALT = value;
                if (value == true)
                {
                    if(GetHALTEvent() != null)
                        GetHALTEvent()();
                }
            }
        }

        private Stopwatch IPSsw = Stopwatch.StartNew();
        private int IpsTmp = 0;
        public int InstructionsPerSecond { get; private set; }
        public string CurrentSpeed
        {
            get
            {
                return Math.Round((float)((InstructionsPerSecond / 1000F) / 1000F), 4) + " MHz";
            }
        }

        public Processor()
            : base()
        {
            MaximumSize = int.MaxValue;
            this.flags = new Flags();
            this.registers = new Registers(flags);
            this.registers.EIP = Options.MemoryBaseAddress;
            RamMemory = new Memory(10000); //10kb max for testing
            Stack = new List<object>();
        }

        public void WriteInstruction(Instruction instruction)
        {
            if (instruction.GetType() != typeof(Variable)) //variable is not a instruction
            {
                RamMemory.Instructions.Add(instruction.VirtualAddress.Address, instruction);
            }
        }

        public void RunLoop()
        {
            while (!HALT)
            {
                NextStep();
            }
        }

        public void NextStep()
        {
            if (isExecutingInstruction)
                return;

            isExecutingInstruction = true;
            ExecuteNextInstruction();
            IpsTmp++;
            if (!HALT)
            {
                if (IPSsw.ElapsedMilliseconds >= 1000)
                {
                    InstructionsPerSecond = IpsTmp;
                    IpsTmp = 0;
                    IPSsw = Stopwatch.StartNew();
                }
            }
            isExecutingInstruction = false;
        }

        /// <summary> Restarting the processor with "clean" enabled will clean the stack, reset registers, This can't reset the variables, Be sure the CPU is done executing everything </summary>
        public void Restart(bool clean)
        {
            if (HALT)
            {
                HALT = false;
                if (RamMemory.Instructions.Count > 0)
                {
                    registers.EIP = RamMemory.Instructions[0].VirtualAddress.Address;

                    if (clean)
                    {
                        Stack.Clear();
                        registers.EAX = 0;
                        registers.EBP = 0;
                        registers.EBX = 0;
                        registers.ECX = 0;
                        registers.EDI = 0;
                        registers.EDX = 0;
                        registers.ESI = 0;
                        registers.ESP = 0;
                    }
                }
            }
        }

        private void ExecuteNextInstruction()
        {
            //check if we reached the end
            if (HALT || RamMemory.Instructions.Count == 0)
                return;

            if (!RamMemory.Instructions.ContainsKey(registers.EIP))
            {
                if (RamMemory.Instructions.IndexOfKey(PreviousEIP.Address) == RamMemory.Instructions.Count - 1)
                {
                    registers.EIP = PreviousEIP.Address;
                    HALT = true;
                    return;
                }
            }

            Instruction CurInstruction = RamMemory.Instructions[registers.EIP];

            //Call event if we are in debug mode
            if(IsDebugMode)
            {
                if (GetCurrentInstructionEventHandler() != null)
                {
                    GetCurrentInstructionEventHandler()(CurInstruction);
                }
            }

            IncreaseEIP = true;
            
            if (CurInstruction.InterfaceType == typeof(IPush))
            {
                ((IPush)CurInstruction).AddToStack(registers, Stack, this.dataSection);
            }
            else if (CurInstruction.InterfaceType == typeof(INop)) { }
            else if (CurInstruction.InterfaceType == typeof(ICall))
            {
                ((CALL)CurInstruction).CallFunction(Stack, this.registers);
            }
            else if (CurInstruction.InterfaceType == typeof(IJump))
            {
                IncreaseEIP = false;
                registers.EIP = ((IJump)CurInstruction).NextIpAddress(flags, registers);
            }
            else if (CurInstruction.InterfaceType == typeof(IInc))
            {
                ((IInc)CurInstruction).Execute(registers);
            }
            else if (CurInstruction.InterfaceType == typeof(IMov))
            {
                ((IMov)CurInstruction).Execute(registers, dataSection);
            }
            else if (CurInstruction.InterfaceType == typeof(IXor))
            {
                ((IXor)CurInstruction).XorValue(registers);
            }
            else if (CurInstruction.InterfaceType == typeof(IXor))
            {
                ((IXor)CurInstruction).XorValue(registers);
            }
            else if (CurInstruction.InterfaceType == typeof(IAnd))
            {
                ((IAnd)CurInstruction).AndValue(registers);
            }
            else if (CurInstruction.InterfaceType == typeof(ICmp))
            {
                ((ICmp)CurInstruction).Compare(ref flags, registers);
            }
            else if (CurInstruction.InterfaceType == typeof(IMul))
            {
                ((IMul)CurInstruction).Multiply(registers);
            }

            PreviousEIP = new VirtualAddress(4, registers.EIP);
            if (IncreaseEIP) //set new EIP for going to our next instruction
            {
                registers.EIP += CurInstruction.VirtualAddress.Size;
            }
        }

        public override string HardwareName
        {
            get { return ""; }
        }
    }
}