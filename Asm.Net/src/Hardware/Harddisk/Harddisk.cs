using System;
using System.Collections.Generic;
using System.Text;

namespace Asm.Net.src.Hardware.Harddisk
{
    public class Harddisk : Hardware
    {
        public string Drive { get; private set; }
        public string Label { get; set; }
        public SortedList<string, Directory> Directories;

        /// <summary> Emulation of the Harddisk, Just the basic stuff at the moment </summary>
        public Harddisk(string drive, string LabelName)
            : base()
        {
            this.Drive = drive + ":\\";
            this.Label = LabelName;
            this.Directories = new SortedList<string, Directory>();
            this.Directories.Add(Drive, new Directory("", Drive));
        }

        public override string HardwareName
        {
            get { return ""; }
        }
    }
}