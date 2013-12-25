using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Runtime.InteropServices;

namespace Asm.Net.src.Sections
{
    public class ApiSection : Section
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr LoadLibrary(string lpFileName);
        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Ansi)]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

        private SortedList<string, SortedList<string, VirtualAddress>> ResolveList;


        public ApiSection()
            : base()
        {
            ResolveList = new SortedList<string, SortedList<string, VirtualAddress>>();
        }

        public ApiSection(byte[] Data)
            : base()
        {
            ResolveList = new SortedList<string, SortedList<string, VirtualAddress>>();

            PayloadReader reader = new PayloadReader(Data);
            int length = reader.ReadInteger();
            for (int i = 0; i < length; i++)
            {
                string lib_func = ((Libraries)reader.ReadByte()).ToString();
                string lib = lib_func + ".dll";

                ResolveList.Add(lib, new SortedList<string, VirtualAddress>());
                short len = reader.ReadShort();
                for (int x = 0; x < len; x++)
                {
                    string func = ((Functions)reader.ReadByte()).ToString().Substring(lib_func.Length + 1);
                    ResolveList[lib].Add(func, new VirtualAddress(4, reader.ReadInteger()));
                }
            }
        }

        public VirtualAddress GetApiAddress(string lib, string func)
        {
            IntPtr libPtr = LoadLibrary(lib);
            if (libPtr == IntPtr.Zero)
            {
                return new VirtualAddress(4, 0);
            }

            VirtualAddress addr = new VirtualAddress(4, GetProcAddress(libPtr, func).ToInt32());

            if (!ResolveList.ContainsKey(lib))
                ResolveList.Add(lib, new SortedList<string, VirtualAddress>());
            if (!ResolveList[lib].ContainsKey(func))
            {
                ResolveList[lib].Add(func, addr);
            }
            return addr;
        }

        public VirtualAddress ResolveAPI(int FuncAddr, ref Libraries library, ref Functions function)
        {
            for (int i = 0; i < ResolveList.Count; i++)
            {
                for (int j = 0; j < ResolveList.Values[i].Count; j++)
                {
                    if (ResolveList.Values[i].Values[j].Address == FuncAddr)
                    {
                        library = (Libraries)Enum.Parse(typeof(Libraries), ResolveList.Keys[i].Substring(0, ResolveList.Keys[i].Length - 4));
                        function = (Functions)Enum.Parse(typeof(Functions), ResolveList.Keys[i].Substring(0, ResolveList.Keys[i].Length - 4) + "_" + ResolveList.Values[i].Keys[j]);

                        return new VirtualAddress(4, GetProcAddress(LoadLibrary(ResolveList.Keys[i]), ResolveList.Values[i].Keys[j]).ToInt32());
                    }
                }
            }
            return new VirtualAddress(4, 0);
        }

        public byte[] ToByteArray
        {
            get
            {
                PayloadWriter writer = new PayloadWriter();
                writer.WriteInteger(ResolveList.Count);

                foreach (string lib in ResolveList.Keys)
                {
                    string name = lib.Substring(0, lib.Length-4);
                    writer.WriteByte((byte)(Libraries)Enum.Parse(typeof(Libraries), name));
                    writer.WriteShort((short)ResolveList[lib].Count);
                    for (int i = 0; i < ResolveList[lib].Count; i++)
                    {
                        writer.WriteByte((byte)(Libraries)Enum.Parse(typeof(Functions), name + "_" + ResolveList[lib].Keys[i]));
                        writer.WriteInteger(ResolveList[lib].Values[i].Address);
                    }
                }
                return writer.ToByteArray();
            }
        }


    }
}