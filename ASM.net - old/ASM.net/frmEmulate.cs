using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AsmEngine.Events;
using AsmEngine.DataTypes;
using AsmEngine.NetEngine;
using AsmEngine;
using System.Threading;
using AsmEngine.Instructions;
using System.Diagnostics;
using AsmEngine.Instructions.Jump;
using AsmEngine.Instructions.Interrupts;
using System.Runtime.InteropServices;
using ASM.net.src;

namespace ASM.net
{
    public unsafe partial class frmEmulate : Form
    {
        public AssemblerExecute asm;
        public Thread ExecuteThread;
        public int StepDelay;
        private int InstructionMsec;
        private Stopwatch sw;
        public VirtualScreen virtualScreen;
        private bool RunDebug;

        public frmEmulate(AssemblerExecute asm)
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            ParsedOpcodeEvent.ParsedOpcode += onParsedOpcode;
            CurrentOpcodeEvent.CurrentOpcode += onCurrentOpcode;
            RegisterUpdateEvent.RegisterUpdate += onRegisterUpdate;
            StackUpdateEvent.StackUpdate += onStackUpdate;
            SetListviewBackColorEvent.SetListviewBackColor += onSetListviewBackColor;
            ProcessStatusUpateEvent.ProcessUpdate += onProcessStatusUpdate;
            FlagUpdateEvent.FlagUpdate += onFlagUpdate;
            this.asm = asm;
            virtualScreen = new VirtualScreen(new Size(0, 0));
            StepDelay = 100;
        }

        private void onParsedOpcode(ParsedOpcodeEventArgs e)
        {
            Type opcodeType = e.opcode.GetType();
            string type = "";

            if (opcodeType == typeof(PUSH))
                type = "PUSH";
            else if (opcodeType == typeof(INC))
                type = "INC";
            else if (opcodeType == typeof(DEC))
                type = "DEC";
            else if (opcodeType.IsSubclassOf(typeof(Acall)))
                type = "CALL";
            else if (opcodeType == typeof(MOV))
                type = "MOV";
            else if (opcodeType == typeof(JMP))
                type = "JMP";
            else if (opcodeType == typeof(INT10) || opcodeType == typeof(INT16) || opcodeType == typeof(INT21))
                type = "INT";
            else if (opcodeType == typeof(CMP))
                type = "CMP";
            else if (opcodeType == typeof(JNZ))
                type = "JNZ";
            else if (opcodeType == typeof(JZ))
                type = "JZ";
            else if (opcodeType == typeof(JE))
                type = "JE";
            else if (opcodeType == typeof(JNE))
                type = "JNE";

            string[] str = new string[3];
            str[0] = "0x" + e.MemAddress.ToString("X"); 
            str[1] = type;
            str[2] = e.AsmCode;
            ListViewItem item = new ListViewItem(str);
            listView1.Items.Add(item);
        }

        private void onCurrentOpcode(CurrentOpcodeEventArgs e)
        {
            try
            {
                for (int i = 0; i < listView1.Items.Count; i++)
                    listView1.Items[i].BackColor = Color.White;
                listView1.Items[e.CurrentLocation].BackColor = Color.Green;
                listView1.EnsureVisible(e.CurrentLocation);
                toolStripStatusLabel5.Text = "Took " + sw.ElapsedMilliseconds + " msec to execute instruction";
                if (listView1.Items.Count - 1 > asm.CurrentOpcodeLocId - 1 && !asm.HALT)
                    toolStripStatusLabel3.Text = "Current Instruction: " + listView1.Items[asm.CurrentOpcodeLocId].SubItems[2].Text;
            }
            catch { }
        }

        private void onRegisterUpdate()
        {
            try
            {
                //32-bit
                label45.Text = asm.register.EAX.ToString("X8");
                label44.Text = asm.register.ECX.ToString("X8");
                label52.Text = asm.register.EBX.ToString("X8");
                label53.Text = asm.register.EDX.ToString("X8");
                label54.Text = asm.register.EIP.ToString("X8");

                //16-bit
                label42.Text = asm.register.AL.ToString("X2");
                label41.Text = asm.register.AH.ToString("X2");
                label43.Text = asm.register.IP.ToString("X4");
                label47.Text = asm.register.CX.ToString("X4");
                label49.Text = asm.register.DX.ToString("X4");
                label51.Text = asm.register.BX.ToString("X4");
            }
            catch { }
        }

        private void onFlagUpdate()
        {
            try
            {
                label16.Text = asm.flag.CarryFlag ? "1" : "0";
                label18.Text = asm.flag.ParityFlag ? "1" : "0";
                label20.Text = asm.flag.CarryFourBitsFlag ? "1" : "0";
                label22.Text = asm.flag.ZeroFlag ? "1" : "0";
                label24.Text = asm.flag.SignFlag ? "1" : "0";
                label26.Text = asm.flag.InterruptFlag ? "1" : "0";
                label28.Text = asm.flag.DirectionFlag ? "1" : "0";
                label30.Text = asm.flag.OverflowFlag ? "1" : "0";
                label32.Text = asm.flag.IOFlag ? "1" : "0";
                label34.Text = asm.flag.NestedTaskFlag ? "1" : "0";
                label36.Text = asm.flag.ResumeFlag ? "1" : "0";
                label38.Text = asm.flag.Virtual8086Flag ? "1" : "0";
                label40.Text = asm.flag.AlignmentFlag ? "1" : "0";
            }
            catch { }
        }

        private void onSetListviewBackColor(SetListviewBackColorEventArgs e)
        {
            listView1.Items[e.index].BackColor = e.BackColor;
            listView1.EnsureVisible(e.index);
        }

        private void onStackUpdate(StackUpdateEventArgs e)
        {
            listView2.Items.Clear();
            for (int i = 0; i < e.stack.Count; i++)
            {
                string[] str = new string[2];
                str[0] = i.ToString();
                str[1] = e.stack[i].value.ToString();
                ListViewItem item = new ListViewItem(str);
                listView2.Items.Add(item);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NextStep();
        }

        private void frmEmulate_FormClosing(object sender, FormClosingEventArgs e)
        {
            
            try
            {
                onHalt();
                if (ExecuteThread.ThreadState == System.Threading.ThreadState.Running)
                    ExecuteThread.Suspend();
                ExecuteThread.Abort();
            }
            catch { }

            ParsedOpcodeEvent.ParsedOpcode -= onParsedOpcode;
            CurrentOpcodeEvent.CurrentOpcode -= onCurrentOpcode;
            RegisterUpdateEvent.RegisterUpdate -= onRegisterUpdate;
            StackUpdateEvent.StackUpdate -= onStackUpdate;
            SetListviewBackColorEvent.SetListviewBackColor -= onSetListviewBackColor;
            ProcessStatusUpateEvent.ProcessUpdate -= onProcessStatusUpdate;
            FlagUpdateEvent.FlagUpdate -= onFlagUpdate;
            HaltEvent.Halt -= onHalt;
            if (virtualScreen != null)
                virtualScreen.Close();
            asm = null;
            GlobalVariables.MainForm.DebugForm = null;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Run();
        }

        public void ExecuteDebug()
        {
            while (!asm.HALT && RunDebug)
            {
                if(StepDelay > 0)
                    Thread.Sleep(StepDelay);
                try
                {
                    sw = Stopwatch.StartNew();
                    asm.NextStep();
                    sw.Stop();
                    InstructionMsec = (int)sw.ElapsedMilliseconds;
                }
                catch { }
            }
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            label10.Text = "Step delay ms: " + trackBar1.Value;
            StepDelay = trackBar1.Value;
        }

        private void frmEmulate_Load(object sender, EventArgs e)
        {
            if (asm.TotalOpcodes == 0)
            {
                toolStripStatusLabel1.Text = "Process Status: No instructions (HALT)";
                trackBar1.Enabled = false;
                button1.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                prevToolStripMenuItem.Enabled = false;
                nextStepToolStripMenuItem.Enabled = false;
                runToolStripMenuItem.Enabled = false;
                virtualScreenToolStripMenuItem.Enabled = false;
            }
            else
                HaltEvent.Halt += onHalt;
        }

        private void onHalt()
        {
            Thread.Sleep(500);
            for (int i = 0; i < listView1.Items.Count; i++)
                listView1.Items[i].BackColor = Color.White;
            if (listView1.Items.Count - 1 > asm.CurrentOpcodeLocId - 1 && !asm.HALT)
                toolStripStatusLabel3.Text = "Current Instruction: " + listView1.Items[asm.CurrentOpcodeLocId].SubItems[2].Text;

            toolStripStatusLabel1.Text = "Process Status: No futher instructions (HALT)";
            trackBar1.Enabled = false;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            prevToolStripMenuItem.Enabled = false;
            nextStepToolStripMenuItem.Enabled = false;
            runToolStripMenuItem.Enabled = false;
            virtualScreenToolStripMenuItem.Enabled = false;
        }

        private void onProcessStatusUpdate(ProcessStatusUpdateEventArgs e)
        {
            toolStripStatusLabel1.Text = "Process Status: " + e.Status;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            PrevStep();
        }

        private void exitDebuggerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void PrevStep()
        {
            if (asm.CurrentOpcodeLocId > 1)
            {
                Stopwatch sw = Stopwatch.StartNew();
                asm.CurrentOpcodeLocId -= 2;
                asm.NextStep();
                sw.Stop();
                toolStripStatusLabel5.Text = "Took " + sw.ElapsedMilliseconds + " msec to execute instruction";
                if (listView1.Items.Count > asm.CurrentOpcodeLocId - 1 && !asm.HALT)
                    toolStripStatusLabel3.Text = "Current Instruction: " + listView1.Items[asm.CurrentOpcodeLocId].SubItems[2].Text;
            }
        }

        private void NextStep()
        {
            sw = Stopwatch.StartNew();
            asm.NextStep();
            sw.Stop();
            InstructionMsec = (int)sw.ElapsedMilliseconds;
        }

        private void Run()
        {
            if (button3.Text == "Run")
            {
                RunDebug = true;
                ExecuteThread = new Thread(new ThreadStart(ExecuteDebug));
                ExecuteThread.Start();
                runToolStripMenuItem.Checked = true;
                runToolStripMenuItem.CheckState = CheckState.Checked;

                button3.Text = "Pause";
            }
            else if (button3.Text == "Pause")
            {
                RunDebug = false;
                button3.Text = "Run";
                runToolStripMenuItem.Checked = false;
                runToolStripMenuItem.CheckState = CheckState.Unchecked;
            }
        }

        private void virtualScreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!virtualScreen.KnownVideoMode)
                System.Windows.Forms.MessageBox.Show("Invalid video mode", "ASM.net", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void nextStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextStep();
        }

        private void prevToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PrevStep();
        }
    }
}