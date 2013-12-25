using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Asm.Net.src.Opcodes;
using Asm.Net.src.Interfaces;
using Asm.Net.src.Opcodes.AsmNet;
using Asm.Net.src.Sections;
using System.Security.Cryptography;

namespace Asm.Net.src
{
    public class OpcodeWriter
    {
        public DataSection dataSection;
        public CodeSection codeSection;
        public ApiSection ApiSection;

        public OpcodeWriter()
        {
            ApiSection = new ApiSection();
            dataSection = new DataSection();
            codeSection = new CodeSection(dataSection, ApiSection);
        }

        ///<summary>
        /// <para Name="AddressChecking"> This will re-check the JUMP addresses and such to prevent errors at runtime </para>
        ///</summary>
        public byte[] Generate(bool AddressChecking)
        {
            codeSection.FixJumps();
            List<Byte> stream = new List<Byte>();

            //checksums
            byte[] DataSectionChecksum = new MD5CryptoServiceProvider().ComputeHash(dataSection.stream.ToArray());
            byte[] CodeSectionChecksum = new MD5CryptoServiceProvider().ComputeHash(codeSection.stream.ToArray());
            byte[] ApiSectionChecksum = new MD5CryptoServiceProvider().ComputeHash(ApiSection.ToByteArray);

            byte[] dataTmp = dataSection.stream.ToArray();
            byte[] codeTmp = codeSection.stream.ToArray();
            byte[] apiTmp = ApiSection.ToByteArray;

            for(int i = 0;  i < dataTmp.Length; i++)
                dataTmp[i] ^= DataSectionChecksum[i % DataSectionChecksum.Length];
            for(int i = 0;  i < codeTmp.Length; i++)
                codeTmp[i] ^= CodeSectionChecksum[i % CodeSectionChecksum.Length];
            for(int i = 0;  i < apiTmp.Length; i++)
                apiTmp[i] ^= ApiSectionChecksum[i % ApiSectionChecksum.Length];

            //create data section
            stream.AddRange(BitConverter.GetBytes(dataTmp.Length));
            stream.AddRange(dataTmp);

            //create code section
            stream.AddRange(BitConverter.GetBytes(codeTmp.Length));
            stream.AddRange(codeTmp);

            //create api section
            stream.AddRange(BitConverter.GetBytes(apiTmp.Length));
            stream.AddRange(apiTmp);

            stream.AddRange(DataSectionChecksum);
            stream.AddRange(CodeSectionChecksum);
            stream.AddRange(ApiSectionChecksum);

            byte[] ret = stream.ToArray();
            ret = QuickLZ.Compress(ret, 0, (uint)ret.Length);

            //lets see if 1 of our JUMPs is having a unreachable address
            //This is just a extra check for runtime if a error will occur
            if (AddressChecking)
            {
                AsmNet asm = new AsmNet(ret);
                asm.InitializeCPU();
                foreach (Instruction i in asm.processor.RamMemory.Instructions.Values)
                {
                    if (i.ToString().StartsWith("JE") || i.ToString().StartsWith("JMP") || i.ToString().StartsWith("JNE") ||
                        i.ToString().StartsWith("JNZ"))
                    {
                        //ok lets see if our jump address is reachable
                        bool found = false;
                        foreach (Instruction x in asm.processor.RamMemory.Instructions.Values)
                        {
                            if (((IJump)i).JumpAddress == x.VirtualAddress.Address)
                            {
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                        {
                            if (codeSection.labels.ContainsValue(((IJump)i).JumpAddress - Options.MemoryBaseAddress))
                            {
                                throw new Exception(i.ToString() + ", Unreachable memory address, Label: " + codeSection.labels.Keys[codeSection.labels.IndexOfValue(((IJump)i).JumpAddress - Options.MemoryBaseAddress)]);
                            }
                            throw new Exception(i.ToString() + ", Unreachable memory address");
                        }
                    }
                }
            }
            return ret;
        }

        
    }
}