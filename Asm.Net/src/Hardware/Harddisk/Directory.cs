using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src.Hardware.Harddisk
{
    public class Directory
    {
        public string DirectoryName { get; set; }
        public string DirectoryPath { get; set; }

        public SortedList<string, File> Files;
        public SortedList<string, Directory> Directories;

        public Directory(string name, string path)
        {
            this.DirectoryName = name;
            this.DirectoryPath = path;
        }
    }
}