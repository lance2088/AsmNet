using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using ASM.net.src;

namespace ASM.net
{
    public partial class TextboxHighlighting : UserControl
    {
        [DllImport("user32")]
        public static extern int LockWindowUpdate(IntPtr hwnd);

        public SortedList<string, Color> highlights;

        public TextboxHighlighting()
        {
            InitializeComponent();
            //lower case
            highlights = new SortedList<string, Color>();
            highlights.Add(RegexPatterns.Accessor.ToLower(), Color.YellowGreen);
            highlights.Add(RegexPatterns.Keyword.ToLower(), Color.Green);
            highlights.Add(RegexPatterns.String, Color.Purple);
            highlights.Add(RegexPatterns.Assembler.ToLower(), Color.Red);
            highlights.Add(RegexPatterns.Caster.ToLower(), Color.Chocolate);
            highlights.Add(RegexPatterns.DataTypes.ToLower(), Color.Cyan);
            highlights.Add(RegexPatterns.Registers.ToLower(), Color.Lime);
            
            //upper case
            highlights.Add(RegexPatterns.Accessor.ToUpper(), Color.YellowGreen);
            highlights.Add(RegexPatterns.Keyword.ToUpper(), Color.Green);
            highlights.Add(RegexPatterns.Assembler.ToUpper(), Color.Red);
            highlights.Add(RegexPatterns.Caster.ToUpper(), Color.Chocolate);
            highlights.Add(RegexPatterns.DataTypes.ToUpper(), Color.Cyan);
            highlights.Add(RegexPatterns.Registers.ToUpper(), Color.Lime);
        }

        public void UpdateControl()
        {
            //update line numbers
            numberLabel.Text = "";
            numberLabel.Height += (richTextBox1.Lines.Length * 2);

            for (int i = 1; i <= richTextBox1.Lines.Length; i++)
                numberLabel.Text += i + "\n";

            UpdateSyntaxHighlighting();
        }

        public void UpdateSyntaxHighlighting()
        {
            LockWindowUpdate(this.Handle);
            int selPos = richTextBox1.SelectionStart;

            richTextBox1.SelectAll();
            richTextBox1.SelectionColor = richTextBox1.ForeColor;

            foreach (string pattern in highlights.Keys)
            {
                Regex keyWords = new Regex(pattern);
                foreach (Match keyWordMatch in keyWords.Matches(richTextBox1.Text))
                {
                    richTextBox1.SelectionStart = keyWordMatch.Index;
                    richTextBox1.SelectionLength = keyWordMatch.Length;
                    richTextBox1.SelectionColor = highlights[pattern];
                }
            }

            richTextBox1.SelectionStart = selPos;
            richTextBox1.SelectionLength = 0;
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
            LockWindowUpdate(IntPtr.Zero);
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            UpdateControl();
        }

        private void richTextBox1_VScroll(object sender, EventArgs e)
        {
            int d = richTextBox1.GetPositionFromCharIndex(0).Y % (richTextBox1.Font.Height + 1);
            numberLabel.Location = new Point(0, d);
        }
        
        private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyData == (Keys.C | Keys.Control))
                {
                    richTextBox1.Copy();
                    e.Handled = true;
                }
                else if (e.KeyData == (Keys.V | Keys.Control))
                {
                    richTextBox1.Paste();
                    e.Handled = true;
                }
                else if (e.KeyData == (Keys.X | Keys.Control))
                {
                    richTextBox1.Cut();
                    e.Handled = true;
                }
                else if (e.KeyData == (Keys.Z | Keys.Control))
                {
                    richTextBox1.Undo();
                    e.Handled = true;
                }
                else if (e.KeyData == (Keys.A | Keys.Control))
                {
                    richTextBox1.SelectAll();
                    e.Handled = false;
                }
                else if (e.KeyData == Keys.Enter)
                {
                    int prevLine = richTextBox1.GetLineFromCharIndex(SelectionStart) - 1;

                    int tabCount = 0;
                    for (int i = 0; i < Lines[prevLine].Length; i++)
                    {
                        if (Lines[prevLine][i] == '\t')
                        {
                            if (Lines[prevLine].Length - 1 != i)
                                tabCount++;
                        }
                        else if (Lines[prevLine][i] != '{')
                        {
                            tabCount--;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }

                    for (int i = 0; i <= tabCount; i++)
                        SelectedText += "\t";
                    e.Handled = true;
                }
            }
            catch { }
        }

#region Properties
        public new Point Location
        {
            get { return this.richTextBox1.Location; }
            set { this.richTextBox1.Location = value; }
        }
        public string SelectedText
        {
            get { return this.richTextBox1.SelectedText; }
            set { this.richTextBox1.SelectedText = value; }
        }
        public string[] Lines
        {
            get { return this.richTextBox1.Lines; }
            set { this.richTextBox1.Lines = value; }
        }
        public int SelectionStart
        {
            get { return this.richTextBox1.SelectionStart; }
            set { this.richTextBox1.SelectionStart = value; }
        }
        public int SelectionLength
        {
            get { return this.richTextBox1.SelectionLength; }
            set { this.richTextBox1.SelectionLength = value; }
        }
        public Color SelectionColor
        {
            get { return this.richTextBox1.SelectionColor; }
            set { this.richTextBox1.SelectionColor = value; }
        }
        public bool WordWrap
        {
            get { return this.richTextBox1.WordWrap; }
            set { this.richTextBox1.WordWrap = value; }
        }
        public bool AcceptsTab
        {
            get { return this.richTextBox1.AcceptsTab; }
            set { this.richTextBox1.AcceptsTab = value; }
        }
        public new string Text
        {
            get { return this.richTextBox1.Text; }
            set
            {
                this.richTextBox1.Text = value;
                UpdateControl();
            }
        }
        public new DockStyle Dock
        {
            get { return this.richTextBox1.Dock; }
            set { this.richTextBox1.Dock = value; }
        }
        public new Font Font
        {
            get { return this.richTextBox1.Font; }
            set { this.richTextBox1.Font = value; }
        }
        public new Color BackColor
        {
            get { return this.richTextBox1.BackColor; }
            set { this.richTextBox1.BackColor = value; }
        }
        public new ContextMenuStrip ContextMenuStrip
        {
            get { return this.richTextBox1.ContextMenuStrip; }
            set { this.richTextBox1.ContextMenuStrip = value; }
        }
        public bool ReadOnly
        {
            get { return this.richTextBox1.ReadOnly; }
            set { this.richTextBox1.ReadOnly = value; }
        }
#endregion

        private void richTextBox1_Resize(object sender, EventArgs e)
        {
            richTextBox1_VScroll(null, null);
        }
    }
}