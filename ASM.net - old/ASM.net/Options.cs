using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ASM.net.src;

namespace ASM.net
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private Control CurrentControl;

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            panel2.Visible = false;
            panel4.Visible = false;

            switch (e.Node.FullPath)
            {
                case "Text editor":
                    panel2.Visible = true;
                    break;
                case "Properties":
                    panel4.Visible = true;
                    break;

            }
        }

        private void TextEditor_Click()
        {
            using (ColorDialog dialog = new ColorDialog())
            {
                dialog.AllowFullOpen = true;
                dialog.AnyColor = true;
                dialog.FullOpen = true;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    CurrentControl.BackColor = dialog.Color;
                    UpdateSyntax();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < GlobalVariables.MainForm.faTabStrip1.Controls.Count; i++)
            {
                TextboxHighlighting textbox = (TextboxHighlighting)GlobalVariables.MainForm.faTabStrip1.Controls[i].Controls[0];
                textbox.BackColor = pictureBox1.BackColor;

                textbox.richTextBox1.ForeColor = pictureBox4.BackColor;
                textbox.highlights.Clear();
                textbox.BackColor = pictureBox1.BackColor;
                textbox.highlights.Add(RegexPatterns.Keyword, pictureBox2.BackColor);
                textbox.highlights.Add(RegexPatterns.Accessor, pictureBox3.BackColor);
                textbox.highlights.Add(RegexPatterns.Assembler, pictureBox5.BackColor);
                textbox.highlights.Add(RegexPatterns.String, pictureBox6.BackColor);
                textbox.UpdateControl();
            }
            Settings.VirtualScreenFPS = (int)numericUpDown1.Value;

            if (comboBox1.Text == "Console Application")
                Settings.buildOutput = BuildOutput.Console;
            else if (comboBox1.Text == "Windows Application")
                Settings.buildOutput = BuildOutput.WindowsApp;

            Settings.MergeEngine = checkBox1.Checked;
            Settings.CompressEngine = checkBox2.Checked;
            Settings.Obfuscate = checkBox3.Checked;

            this.Close();
        }

        private void UpdateSyntax()
        {
            textboxHighlighting1.richTextBox1.ForeColor = pictureBox4.BackColor;
            textboxHighlighting1.highlights.Clear();
            textboxHighlighting1.BackColor = pictureBox1.BackColor;
            textboxHighlighting1.highlights.Add("(namespace)", pictureBox2.BackColor);
            textboxHighlighting1.highlights.Add("(public|private|internal|protected)", pictureBox3.BackColor);
            textboxHighlighting1.highlights.Add("(push|nop|mov|add|xor|call)", pictureBox5.BackColor);
            textboxHighlighting1.highlights.Add("\"(.*?)\"", pictureBox6.BackColor);
            textboxHighlighting1.UpdateControl();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            CurrentControl = pictureBox1;
            TextEditor_Click();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            CurrentControl = pictureBox2;
            TextEditor_Click();
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            CurrentControl = pictureBox3;
            TextEditor_Click();
        }

        private void pictureBox4_Click(object sender, EventArgs e)
        {
            CurrentControl = pictureBox4;
            TextEditor_Click();
        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
            CurrentControl = pictureBox5;
            TextEditor_Click();
        }

        private void pictureBox6_Click(object sender, EventArgs e)
        {
            CurrentControl = pictureBox6;
            TextEditor_Click();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox2.Enabled = true;
            }
            else
            {
                checkBox2.Enabled = false;
                checkBox2.Checked = false;
            }
        }
    }
}
