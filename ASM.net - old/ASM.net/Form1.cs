using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AsmEngine;
using AsmEngine.DataTypes;
using System.Diagnostics;
using FarsiLibrary.Win;
using ASM.net.src;
using System.Text.RegularExpressions;
using System.IO;
using System.CodeDom.Compiler;
using System.Reflection;
using ASM.net.src.NetFuscator;

namespace ASM.net
{
    public partial class Form1 : Form
    {
        public frmEmulate DebugForm;
        public Form1()
        {
            InitializeComponent();
            GlobalVariables.MainForm = this;
            new Settings();
            CreateNewTab("namespace TestProgram\r\n{\r\n\tpublic class Test\r\n\t{\r\n\t}\r\n}", "new.asmn");
        }

        private void buildToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                Build(GetCurrentTextbox().Text, true, false);
            }
            catch { }
        }

        private void executeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                TextboxHighlighting textbox = GetCurrentTextbox();
                if (textbox != null)
                {
                    AssemblerCompiler compiler = Build(textbox.Text, true, true);
                }
            }
            catch { }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            buildToolStripMenuItem.PerformClick();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            //try
            //{
                if (DebugForm != null)
                {
                    MessageBox.Show("Already debugging", "ASM.net", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                TextboxHighlighting textbox = GetCurrentTextbox();
                if (textbox != null)
                {
                    AssemblerCompiler compiler = Build(textbox.Text, false, false);
                    if (compiler.errors.Count == 0)
                    {
                        AssemblerExecute executor = new AssemblerExecute();
                        DebugForm = new frmEmulate(executor);
                        executor.Execute(compiler.AssemblerBytes.ToArray());
                        DebugForm.Show();
                    }
                }
            /*}
            catch(Exception ex)
            {
                MessageBox.Show(ex.StackTrace + "\r\n" + ex.Message);
            }*/
        }

        private void richTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.C)
                Clipboard.SetText(textBox1.SelectedText);
        }

        private void copyToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TextboxHighlighting textbox = GetCurrentTextbox();
            if (textbox != null)
                Clipboard.SetText(textbox.SelectedText.Replace("\n", "\r\n"));
        }

        private void pasteToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TextboxHighlighting textbox = GetCurrentTextbox();
            if(textbox != null)
                textbox.SelectedText = Clipboard.GetText();
        }


        public AssemblerCompiler Build(string SourceCode, bool save, bool execute)
        {
            Stopwatch sw = Stopwatch.StartNew();
            textBox1.Text = "--------- Build started at " + DateTime.Now + "---------\r\n";
            AssemblerCompiler compiler = new AssemblerCompiler(SourceCode);
            compiler.Build();

            if (compiler.errors.Count > 0)
            {
                textBox1.Text += "Build unsuccessful, " + compiler.errors.Count + " errors\r\n";
                for(int i = 0; i < compiler.errors.Count; i++)
                    textBox1.Text += "line:" + compiler.errors[i].line + ", " + compiler.errors[i].Details + "\r\n";
                return compiler;
            }

            sw.Stop();

            if(sw.Elapsed.Seconds == 0)
                textBox1.Text += "Build complete, It took " + sw.ElapsedMilliseconds + " Milliseconds\r\n";
            else
                textBox1.Text += "Build complete, It took " + sw.Elapsed.Seconds + ":" + sw.ElapsedMilliseconds + " second(s)\r\n";

            if (save)
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.AddExtension = true;
                    dialog.CheckFileExists = false;
                    dialog.Filter = "Executable|*.exe";
                    dialog.Title = "Save the Assembler.net to a executable";

                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine("using System;");
                        sb.AppendLine("using AsmEngine;");
                        sb.AppendLine("using System.Reflection;");
                        sb.AppendLine("using System.Diagnostics;");

                        if (Settings.MergeEngine && Settings.CompressEngine)
                        {
                            sb.AppendLine("using System.IO;");
                            sb.AppendLine("using System.IO.Compression;");
                        }

                        sb.AppendLine("namespace AsmNetConsoleCodedom");
                        sb.AppendLine("{");
                        sb.AppendLine("class Program");
                        sb.AppendLine("{");
                        sb.AppendLine("static void Main(string[] args)");
                        sb.AppendLine("{");

                        //bytes to execute
                        string bytes = BitConverter.ToString(compiler.AssemblerBytes.ToArray());
                        bytes = "0x" + bytes.Replace("-", ", 0x");

                        byte[] Engine = File.ReadAllBytes("AsmEngine.dll");
                        if (Settings.CompressEngine)
                            Engine = Compressor.Compress(Engine);

                        string EngineBytes = BitConverter.ToString(Engine);
                        EngineBytes = "0x" + EngineBytes.Replace("-", ", 0x");

                        if (Settings.MergeEngine)
                        {
                            if (Settings.CompressEngine)
                                sb.AppendLine("byte[] engine = Decompress(new byte[] { " + EngineBytes + " });");
                            else
                                sb.AppendLine("byte[] engine = new byte[] { " + EngineBytes + " };");

                            sb.AppendLine("Assembly asm = Assembly.Load(engine);");
                            sb.AppendLine("Type executor = asm.GetType(\"AsmEngine.AssemblerExecute\");");
                            sb.AppendLine("Object initialized = Activator.CreateInstance(executor);");
                            sb.AppendLine("byte[] arrayzor = new byte[] { " + bytes + " };");
                            sb.AppendLine("executor.InvokeMember(\"Execute\", BindingFlags.Default | BindingFlags.InvokeMethod, null, initialized, new object[] { arrayzor });");
                            sb.AppendLine("PropertyInfo property = executor.GetProperty(\"HALT\");");
                            sb.AppendLine("bool isHalt = false;");
                            sb.AppendLine("do");
                            sb.AppendLine("{");
                            sb.AppendLine("    isHalt = Convert.ToBoolean(property.GetValue(initialized, null));");
                            sb.AppendLine("    if (!isHalt)");
                            sb.AppendLine("        executor.InvokeMember(\"NextStep\", BindingFlags.Default | BindingFlags.InvokeMethod, null, initialized, new object[] { });");
                            sb.AppendLine("}");
                            sb.AppendLine("while (!isHalt);");
                        }
                        else
                        {
                            sb.AppendLine("AssemblerExecute a = new AssemblerExecute();");
                            sb.AppendLine("a.Execute(new byte[]{" + bytes + "});");
                            sb.AppendLine("while(!a.HALT){ a.NextStep();}");
                        }

                        #region "Dynamic calls"
                        /*
                        //WORKING DYNAMIC CALLS
                        //load the AsmEngine
                        Assembly asm = Assembly.Load(File.ReadAllBytes("AsmEngine.dll"));
                        Type executor = asm.GetType("AsmEngine.AssemblerExecute");
                        //Call constructor
                        Object initialized = Activator.CreateInstance(executor);
                        //Call Execute function
                        executor.InvokeMember("Execute", BindingFlags.Default | BindingFlags.InvokeMethod, null, initialized, new object[] { compiler.AssemblerBytes.ToArray() });

                        PropertyInfo property = executor.GetProperty("HALT");
                        bool isHalt = false;
                        do
                        {
                            isHalt = Convert.ToBoolean(property.GetValue(initialized, null));
                            if(!isHalt)
                                executor.InvokeMember("NextStep", BindingFlags.Default | BindingFlags.InvokeMethod, null, initialized, new object[] { });
                        }
                        while (!isHalt);*/
                        #endregion

                        //sb.AppendLine("System.Diagnostics.Process.GetCurrentProcess().WaitForExit();");
                        sb.AppendLine("}");

                        if (Settings.MergeEngine && Settings.CompressEngine)
                        {
                            sb.AppendLine("public static byte[] Decompress(byte[] gzBuffer)");
                            sb.AppendLine("{");
                            sb.AppendLine("    MemoryStream ms = new MemoryStream();");
                            sb.AppendLine("    int msgLength = BitConverter.ToInt32(gzBuffer, 0);");
                            sb.AppendLine("    ms.Write(gzBuffer, 4, gzBuffer.Length - 4);");
                            sb.AppendLine("    byte[] buffer = new byte[msgLength];");
                            sb.AppendLine("    ms.Position = 0;");
                            sb.AppendLine("    GZipStream zip = new GZipStream(ms, CompressionMode.Decompress);");
                            sb.AppendLine("    zip.Read(buffer, 0, buffer.Length);");
                            sb.AppendLine("    return buffer;");
                            sb.AppendLine("}");
                        }

                        sb.AppendLine("}");
                        sb.AppendLine("}");
                        

                        CodeDomProvider codeProvider = CodeDomProvider.CreateProvider("CSharp");

                        System.CodeDom.Compiler.CompilerParameters parameters = new CompilerParameters();
                        parameters.ReferencedAssemblies.Add(Environment.CurrentDirectory + "\\AsmEngine.dll");
                        parameters.ReferencedAssemblies.Add("System.dll");
                        parameters.CompilerOptions = "/unsafe /optimize";
                        if (Settings.buildOutput == BuildOutput.WindowsApp)
                            parameters.CompilerOptions += " /t:winexe";

                        //Make sure we generate an EXE, not a DLL
                        parameters.GenerateExecutable = true;
                        parameters.OutputAssembly = dialog.FileName;

                        try
                        {
                            if (File.Exists(dialog.FileName))
                                File.Delete(dialog.FileName);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return compiler;
                        }

                        CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, sb.ToString());
                        /*
                        FileStream filestream = new FileStream(dialog.FileName, FileMode.Open, FileAccess.Write);
                        filestream.Seek(0xF4, SeekOrigin.Begin);
                        filestream.WriteByte(11);
                        filestream.Flush();
                        filestream.Close();*/
                        textBox1.Text += "Saved file at " + dialog.FileName + "\r\n";

                        if (Settings.Obfuscate)
                        {
                            textBox1.Text += "NetFuscator - Obfuscating assembly\r\n";
                            NetFuscator obfuscator = new NetFuscator();
                            obfuscator.Obfuscate(dialog.FileName);
                        }

                        if (execute)
                        {
                            Process.Start(dialog.FileName);
                        }
                    }
                }
            }
            return compiler;
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Options { }.ShowDialog();
        }

        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.Filter = "Assembler .net|*.asmn";
                dialog.Multiselect = false;
                dialog.Title = "Select a Assembler .net source code file";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(dialog.FileName))
                    {

                        FATabStripItem tab = new FATabStripItem();
                        tab.Title = dialog.FileName;
                        faTabStrip1.AddTab(tab);

                        TextboxHighlighting txtHighlighting = new TextboxHighlighting();
                        txtHighlighting.ContextMenuStrip = contextMenuStrip2;
                        txtHighlighting.UpdateControl();
                        txtHighlighting.Width = tab.Width;
                        txtHighlighting.Height = tab.Height;
                        txtHighlighting.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                        txtHighlighting.Text = File.ReadAllText(dialog.FileName);
                        tab.Controls.Add(txtHighlighting);
                    }
                    else
                    {
                        MessageBox.Show("The file does not exist");
                    }
                }
            }
        }

        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.CheckFileExists = false;
                    dialog.Filter = "Assembler .net|*.asmn";
                    dialog.Title = "Save a Assembler .net source code file";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        TextboxHighlighting textbox = GetCurrentTextbox();
                        if(textbox != null)
                            File.WriteAllText(dialog.FileName, textbox.Text);
                    }
                }
            }
            catch { }
        }

        private void newToolStripButton_Click(object sender, EventArgs e)
        {
            FATabStripItem tab = new FATabStripItem();
            tab.Title = "new.asmn";
            faTabStrip1.AddTab(tab);

            TextboxHighlighting txtHighlighting = new TextboxHighlighting();
            txtHighlighting.ContextMenuStrip = contextMenuStrip2;
            txtHighlighting.UpdateControl();
            txtHighlighting.Width = tab.Width;
            txtHighlighting.Height = tab.Height;
            txtHighlighting.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            tab.Controls.Add(txtHighlighting);
            txtHighlighting.Text = "namespace Sample\r\n{\r\n\tpublic class Sample\r\n\t{\r\n\t}\r\n}";
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FATabStripItem tab = new FATabStripItem();
            tab.Title = "new.asmn";
            faTabStrip1.AddTab(tab);

            TextboxHighlighting txtHighlighting = new TextboxHighlighting();
            txtHighlighting.ContextMenuStrip = contextMenuStrip2;
            txtHighlighting.UpdateControl();
            txtHighlighting.Width = tab.Width;
            txtHighlighting.Height = tab.Height;
            txtHighlighting.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

            tab.Controls.Add(txtHighlighting);
            txtHighlighting.Text = "namespace Sample\r\n{\r\n\tpublic class Sample\r\n\t{\r\n\t}\r\n}";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.Filter = "Assembler .net|*.asmn";
                dialog.Multiselect = false;
                dialog.Title = "Select a Assembler .net source code file";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (File.Exists(dialog.FileName))
                    {
                        FATabStripItem tab = new FATabStripItem();
                        tab.Title = dialog.FileName;
                        faTabStrip1.AddTab(tab);

                        TextboxHighlighting txtHighlighting = new TextboxHighlighting();
                        txtHighlighting.ContextMenuStrip = contextMenuStrip2;
                        txtHighlighting.UpdateControl();
                        txtHighlighting.Width = tab.Width;
                        txtHighlighting.Height = tab.Height;
                        txtHighlighting.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;

                        txtHighlighting.Text = File.ReadAllText(dialog.FileName);
                        tab.Controls.Add(txtHighlighting);
                    }
                    else
                    {
                        MessageBox.Show("The file does not exist");
                    }
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                using (SaveFileDialog dialog = new SaveFileDialog())
                {
                    dialog.CheckFileExists = false;
                    dialog.Filter = "Assembler .net|*.asmn";
                    dialog.Title = "Save a Assembler .net source code file";
                    if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        TextboxHighlighting textbox = GetCurrentTextbox();
                        if(textbox != null)
                            File.WriteAllText(dialog.FileName, textbox.Text);
                    }
                }
            }
            catch { }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private TextboxHighlighting GetCurrentTextbox()
        {
            TextboxHighlighting textbox = (TextboxHighlighting)faTabStrip1.ActiveControl;
            if (textbox != null)
                return textbox;

            try
            {
                textbox = (TextboxHighlighting)faTabStrip1.Controls[faTabStrip1.TabIndex].Controls[0];
                if (textbox != null)
                    return textbox;
                return null;
            }
            catch { }
            return null;
        }

        public void CreateNewTab(string Text, string TabTitle)
        {
            FATabStripItem tab = new FATabStripItem();
            tab.Title = TabTitle;
            faTabStrip1.AddTab(tab);
            TextboxHighlighting txtHighlighting = new TextboxHighlighting();
            txtHighlighting.ContextMenuStrip = contextMenuStrip2;
            txtHighlighting.UpdateControl();
            txtHighlighting.Width = tab.Width;
            txtHighlighting.Height = tab.Height;
            txtHighlighting.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            tab.Controls.Add(txtHighlighting);
            txtHighlighting.Text = Text;
        }

        private void newToolStripButton1_Click(object sender, EventArgs e)
        {
            CreateNewTab("namespace TestProgram\r\n{\r\n\tpublic class Test\r\n\t{\r\n\t}\r\n}", "new.asmn");
        }

        private void drawBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewTab(Properties.Resources.DrawBoxExample, "DrawBox_Example");
        }

        private void loopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewTab(Properties.Resources.LoopExample, "Loop_Example");
        }

        private void beepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewTab(Properties.Resources.BeepExample, "Beep_Example");
        }

        private void messageBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewTab(Properties.Resources.MessageBoxExample, "MessageBox_Example");
        }
    }
}