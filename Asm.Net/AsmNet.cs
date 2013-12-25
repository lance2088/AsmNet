using System;
using System.Collections.Generic;
using System.Text;
using Asm.Net.src;
using Asm.Net.src.Opcodes;
using Asm.Net.src.BeaEngine;
using System.IO;
using Asm.Net.src.Opcodes.INC;
using Asm.Net.src.Opcodes.MOV;
using Asm.Net.src.Hardware;
using Asm.Net.src.Opcodes.XOR;
using Asm.Net.src.PE;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Opcodes.Jumps;
using Asm.Net.src.Sections;

namespace Asm.Net
{
    public class AsmNet
    {
        public OpcodeReader DataSectionReader;
        public OpcodeReader CodeSectionReader;
        public Instruction[] Instructions { get; set; }
        public PEReader PeReader { get; private set; }
        public PELoader PeLoader { get; private set; }
        public PortableExecutableParser PeParser { get; private set; }
        public SortedList<string, Module> Modules;
        public ApiSection ApiSection { get; private set; }
        public DataSection DataSection { get; private set; }

        public bool DebugMode
        {
            get
            {
                if (processor != null)
                    return processor.IsDebugMode;
                return false;
            }
            set
            {
                if (processor != null)
                    processor.IsDebugMode = value;
            }
        }
        public Processor processor { get; set; }

        //Mainly used for the Processor class to call the private functions in this class
        internal delegate CurrentInstructionHandler GetCurrentInstructionEventHandler();
        internal delegate HALTHandler GetHALTEventHandler();
        internal delegate OpcodeReader GetDataSectionReaderHandler();

        //event stuff
        public delegate void CurrentInstructionHandler(Instruction instruction);
        public delegate void HALTHandler();
        public event CurrentInstructionHandler CurrentInstructionEvent;
        public event HALTHandler HALTEvent;

        public AsmNet(byte[] data)
        {
            byte[] payload = new byte[data.Length];
            Array.Copy(data, payload, payload.Length);
            payload = QuickLZ.Decompress(payload, 0);

            int ReadOffset = 0;
            byte[] DataSectionBytes = new byte[BitConverter.ToInt32(payload, ReadOffset)];
            ReadOffset += 4;
            Array.Copy(payload, 4, DataSectionBytes, 0, DataSectionBytes.Length);
            ReadOffset += DataSectionBytes.Length;

            byte[] CodeSectionBytes = new byte[BitConverter.ToInt32(payload, ReadOffset)];
            ReadOffset += 4;
            Array.Copy(payload, ReadOffset, CodeSectionBytes, 0, CodeSectionBytes.Length);
            ReadOffset += CodeSectionBytes.Length;

            byte[] ApiSectionBytes = new byte[BitConverter.ToInt32(payload, ReadOffset)];
            ReadOffset += 4;
            Array.Copy(payload, ReadOffset, ApiSectionBytes, 0, ApiSectionBytes.Length);
            ReadOffset += ApiSectionBytes.Length;

            byte[] DataSectionChecksum = new byte[16];
            byte[] CodeSectionChecksum = new byte[16];
            byte[] ApiSectionChecksum = new byte[16];
            
            Array.Copy(payload, ReadOffset, DataSectionChecksum, 0, 16);
            Array.Copy(payload, ReadOffset + 16, CodeSectionChecksum, 0, 16);
            Array.Copy(payload, ReadOffset + 32, ApiSectionChecksum, 0, 16);

            for(int i = 0;  i < DataSectionBytes.Length; i++)
                DataSectionBytes[i] ^= DataSectionChecksum[i % DataSectionChecksum.Length];
            for(int i = 0;  i < CodeSectionBytes.Length; i++)
                CodeSectionBytes[i] ^= CodeSectionChecksum[i % CodeSectionChecksum.Length];
            for(int i = 0;  i < ApiSectionBytes.Length; i++)
                ApiSectionBytes[i] ^= ApiSectionChecksum[i % ApiSectionChecksum.Length];

            ApiSection = new src.Sections.ApiSection(ApiSectionBytes);
            DataSection = new DataSection(DataSectionBytes);

            this.CodeSectionReader = new OpcodeReader(CodeSectionBytes, DataSection, ApiSection);
            this.DataSectionReader = new OpcodeReader(DataSectionBytes, DataSection, ApiSection);
            Instructions = CodeSectionReader.ReadAllInstructions();
        }

        /// <summary> Load a native file into memory and emulate the CPU *EXPERIMENTAL* </summary>
        public AsmNet(string FilePath)
        {
            PeParser = new PortableExecutableParser(FilePath);
            Instructions = x86Data.PeToInstructions(FilePath);

            //Load the modules we need to emulate also
            Modules = new SortedList<string, Module>();
            foreach (string dll in PeParser.Imports.Keys)
            {
                Modules.Add(dll, new Module(dll));
            }

            //Set comments to the instructions
            foreach (Instruction i in Instructions)
            {
                if (i.GetType() == typeof(CALL))
                {
                    foreach (SortedList<string, IntPtr> handles in PeParser.Imports.Values)
                    {
                        if (handles.Values.Contains(new IntPtr(((CALL)i).FuncAddress)))
                        {
                            string DLL = "???";
                            string Func = "???";

                            i.ExtraInformation = DLL + "." + Func;
                        }
                    }
                }
                else if (i.GetType() == typeof(JMP))
                {
                    foreach (SortedList<string, IntPtr> handles in PeParser.Imports.Values)
                    {
                        if (handles.Values.Contains(new IntPtr(((IJump)i).JumpAddress)))
                        {
                            string DLL = "???";
                            string Func = "???";

                            i.ExtraInformation = DLL + "." + Func;
                        }
                    }
                }
            }

            //PeLoader = new PELoader(File.Open(FilePath, FileMode.Open));
            //int EntryPoint = PeLoader.getEntryPoint();
            PeReader = new PEReader(FilePath);
        }

        /// <summary> Initialize the Processor </summary>
        public Processor InitializeCPU()
        {
            processor = new Processor();
            processor.GetCurrentInstructionEventHandler = new GetCurrentInstructionEventHandler(GetCurrentInstructionEvent);
            processor.GetDataSectionReader = new GetDataSectionReaderHandler(GetDataSectionReader);
            processor.GetHALTEvent = new GetHALTEventHandler(GetHALTHandler);

            for (int i = 0; i < Instructions.Length; i++)
            {
                processor.WriteInstruction(Instructions[i]);
            }
            processor.RamMemory.InstructionsToPE(Instructions);
            processor.RamMemory.MemoryStream.AddRange(this.CodeSectionReader.ToByteArray());
            processor.dataSection = this.DataSection;
            return processor;
        }

        private CurrentInstructionHandler GetCurrentInstructionEvent() { return CurrentInstructionEvent; }
        private OpcodeReader GetDataSectionReader() { return DataSectionReader; }
        private HALTHandler GetHALTHandler() { return HALTEvent; }
    }
}