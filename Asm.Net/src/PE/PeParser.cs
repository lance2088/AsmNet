using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Security;

namespace Asm.Net.src.PE
{
    public unsafe class PortableExecutableParser
    {
        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        static extern UIntPtr GetProcAddress(IntPtr hModule, string procName);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);

        public List<string> Exports = new List<string>();
        public SortedList<string, SortedList<string, IntPtr>> Imports = new SortedList<string, SortedList<string, IntPtr>>();

        public PortableExecutableParser(string path)
        {
            LOADED_IMAGE loadedImage;

            if (MapAndLoad(path, null, out loadedImage, true, true))
            {
                LoadExports(loadedImage);
                LoadImports(loadedImage);
            }
        }

        private void LoadExports(LOADED_IMAGE loadedImage)
        {
            var hMod = (void*)loadedImage.MappedAddress;

            if (hMod != null)
            {
                uint size;
                IMAGE_EXPORT_DIRECTORY* pExportDir = (IMAGE_EXPORT_DIRECTORY*)ImageDirectoryEntryToData((void*)loadedImage.MappedAddress, false, IMAGE_DIRECTORY_ENTRY_EXPORT, out size);

                if (pExportDir != null)
                {
                    uint* pFuncNames = (uint*)RvaToVa(loadedImage, pExportDir->AddressOfNames);

                    for (uint i = 0; i < pExportDir->NumberOfNames; i++)
                    {
                        uint funcNameRva = pFuncNames[i];
                        if (funcNameRva != 0)
                        {
                            char* funcName = (char*)RvaToVa(loadedImage, funcNameRva);
                            string name = Marshal.PtrToStringAnsi((IntPtr)funcName);
                            Exports.Add(name);
                        }
                    }
                }
            }
        }

        private static IntPtr RvaToVa(LOADED_IMAGE loadedImage, uint rva)
        {
            return ImageRvaToVa(loadedImage.FileHeader, loadedImage.MappedAddress, rva, IntPtr.Zero);
        }
        private static IntPtr RvaToVa(LOADED_IMAGE loadedImage, IntPtr rva)
        {
            return RvaToVa(loadedImage, (uint)(rva.ToInt32()));
        }

        private void LoadImports(LOADED_IMAGE loadedImage)
        {
            var hMod = (void*)loadedImage.MappedAddress;

            if (hMod != null)
            {
                Console.WriteLine("Got handle");

                uint size;
                var pImportDir =
                    (IMAGE_IMPORT_DESCRIPTOR*)
                    ImageDirectoryEntryToData(hMod, false, IMAGE_DIRECTORY_ENTRY_IMPORT, out size);
                if (pImportDir != null)
                {
                    while (pImportDir->OriginalFirstThunk != 0)
                    {
                        try
                        {
                            var szName = (char*)RvaToVa(loadedImage, pImportDir->Name);
                            string name = Marshal.PtrToStringAnsi((IntPtr)szName);

                            Imports.Add(name, new SortedList<string, IntPtr>());

                            var pThunkOrg = (THUNK_DATA*)RvaToVa(loadedImage, pImportDir->OriginalFirstThunk);

                            while (pThunkOrg->AddressOfData != IntPtr.Zero)
                            {
                                uint ord;

                                if ((pThunkOrg->Ordinal & 0x80000000) > 0)
                                {
                                    ord = pThunkOrg->Ordinal & 0xffff;
                                }
                                else
                                {
                                    IMAGE_IMPORT_BY_NAME* pImageByName = (IMAGE_IMPORT_BY_NAME*)RvaToVa(loadedImage, pThunkOrg->AddressOfData);

                                    if (!IsBadReadPtr(pImageByName, (uint)sizeof(IMAGE_IMPORT_BY_NAME)))
                                    {
                                        ord = pImageByName->Hint;
                                        var szImportName = pImageByName->Name;
                                        string sImportName = Marshal.PtrToStringAnsi((IntPtr)szImportName);
                                        string logzor = String.Format("imports ({0}).{1}@{2} - Address: {3}", name, sImportName, ord, pThunkOrg->Function);

                                        IntPtr ModuleHandle = GetModuleHandle(name);
                                        UIntPtr addr = GetProcAddress(ModuleHandle, sImportName);

                                        Imports[name].Add(sImportName, new IntPtr(pThunkOrg->Function + 0x400000));
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }

                                pThunkOrg++;
                            }
                        }
                        catch (AccessViolationException e)
                        {
                        }

                        pImportDir++;
                    }
                }
            }
        }


        // ReSharper disable InconsistentNaming
        private const ushort IMAGE_DIRECTORY_ENTRY_IMPORT = 1;
        private const ushort IMAGE_DIRECTORY_ENTRY_EXPORT = 0;

        private const CallingConvention WINAPI = CallingConvention.Winapi;

        private const string KERNEL_DLL = "kernel32";
        private const string DBGHELP_DLL = "Dbghelp";
        private const string IMAGEHLP_DLL = "ImageHlp";
        // ReSharper restore InconsistentNaming

        [DllImport(KERNEL_DLL, CallingConvention = WINAPI, EntryPoint = "GetModuleHandleA"), SuppressUnmanagedCodeSecurity]
        public static extern void* GetModuleHandleA(/*IN*/ char* lpModuleName);

        [DllImport(KERNEL_DLL, CallingConvention = WINAPI, EntryPoint = "GetModuleHandleW"), SuppressUnmanagedCodeSecurity]
        public static extern void* GetModuleHandleW(/*IN*/ char* lpModuleName);

        [DllImport(KERNEL_DLL, CallingConvention = WINAPI, EntryPoint = "IsBadReadPtr"), SuppressUnmanagedCodeSecurity]
        public static extern bool IsBadReadPtr(void* lpBase, uint ucb);

        [DllImport(DBGHELP_DLL, CallingConvention = WINAPI, EntryPoint = "ImageDirectoryEntryToData"), SuppressUnmanagedCodeSecurity]
        public static extern void* ImageDirectoryEntryToData(void* pBase, bool mappedAsImage, ushort directoryEntry, out uint size);

        [DllImport(DBGHELP_DLL, CallingConvention = WINAPI), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr ImageRvaToVa(
            IntPtr pNtHeaders,
            IntPtr pBase,
            uint rva,
            IntPtr pLastRvaSection);

        [DllImport(DBGHELP_DLL, CallingConvention = CallingConvention.StdCall), SuppressUnmanagedCodeSecurity]
        public static extern IntPtr ImageNtHeader(IntPtr pImageBase);

        [DllImport(IMAGEHLP_DLL, CallingConvention = CallingConvention.Winapi), SuppressUnmanagedCodeSecurity]
        public static extern bool MapAndLoad(string imageName, string dllPath, out LOADED_IMAGE loadedImage, bool dotDll, bool readOnly);

    }


    // ReSharper disable InconsistentNaming
    [StructLayout(LayoutKind.Sequential)]
    public struct LOADED_IMAGE
    {
        public IntPtr moduleName;
        public IntPtr hFile;
        public IntPtr MappedAddress;
        public IntPtr FileHeader;
        public IntPtr lastRvaSection;
        public UInt32 numbOfSections;
        public IntPtr firstRvaSection;
        public UInt32 charachteristics;
        public ushort systemImage;
        public ushort dosImage;
        public ushort readOnly;
        public ushort version;
        public IntPtr links_1;  // these two comprise the LIST_ENTRY
        public IntPtr links_2;
        public UInt32 sizeOfImage;
    }



    [StructLayout(LayoutKind.Explicit)]
    public unsafe struct IMAGE_IMPORT_BY_NAME
    {
        [FieldOffset(0)]
        public ushort Hint;
        [FieldOffset(2)]
        public fixed char Name[1];
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct IMAGE_IMPORT_DESCRIPTOR
    {
        #region union
        /// <summary>
        /// CSharp doesnt really support unions, but they can be emulated by a field offset 0
        /// </summary>

        [FieldOffset(0)]
        public uint Characteristics;            // 0 for terminating null import descriptor
        [FieldOffset(0)]
        public uint OriginalFirstThunk;         // RVA to original unbound IAT (PIMAGE_THUNK_DATA)
        #endregion

        [FieldOffset(4)]
        public uint TimeDateStamp;
        [FieldOffset(8)]
        public uint ForwarderChain;
        [FieldOffset(12)]
        public uint Name;
        [FieldOffset(16)]
        public uint FirstThunk;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct IMAGE_EXPORT_DIRECTORY
    {
        public UInt32 Characteristics;
        public UInt32 TimeDateStamp;
        public UInt16 MajorVersion;
        public UInt16 MinorVersion;
        public UInt32 Name;
        public UInt32 Base;
        public UInt32 NumberOfFunctions;
        public UInt32 NumberOfNames;
        public IntPtr AddressOfFunctions;     // RVA from base of image
        public IntPtr AddressOfNames;     // RVA from base of image
        public IntPtr AddressOfNameOrdinals;  // RVA from base of image
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct THUNK_DATA
    {
        [FieldOffset(0)]
        public uint ForwarderString;      // PBYTE 
        [FieldOffset(0)]
        public uint Function;             // PDWORD
        [FieldOffset(0)]
        public uint Ordinal;
        [FieldOffset(0)]
        public IntPtr AddressOfData;        // PIMAGE_IMPORT_BY_NAME
    }
}