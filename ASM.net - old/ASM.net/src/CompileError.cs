using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace ASM.net.src
{
    public class CompileError
    {
        public ListViewItem item;
        public string Details;
        public int line;
        public int id;

        public CompileError(int line, string details)
        {
            string[] str = new string[3];
            str[0] = (GlobalVariables.MainForm.listView1.Items.Count).ToString();
            str[1] = line.ToString();
            str[2] = details;
            item = new ListViewItem(str, 0);
            this.line = line;
            this.Details = details;
            this.id = GlobalVariables.MainForm.listView1.Items.Count;
        }
    }
}