using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Asm.Net.src.Hardware;
using System.Diagnostics;
using Asm.Net.src;
using Asm.Net;
using System.Threading;
using System.Runtime.InteropServices;
using SpPerfChart;
using System.IO;

namespace executor
{
    public partial class Form1 : Form
    {
        Stopwatch FormSW = Stopwatch.StartNew();
        Processor cpu;
        AsmNet asm = null;
        bool running = false;
        ChartLine chartLine;

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.EnableNotifyMessage, true);

            chartLine = new ChartLine();
            chartLine.AverageComment = "IPS";
            chartLine.DrawLines = true;
            chartLine.PeakComment = "Instructions Per Second";
            chartLine.ShowAverageLine = true;
            perfChart1.ChartLines.Add(chartLine);
        }

        private void loadPortableExecutableToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (running)
            {
                if (MessageBox.Show("Already running do you want to stop ?", "Asm.Net - x86 Emulator", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    hALTToolStripMenuItem_Click(null, null);
                    listViewEx1.ClearInstructions();
                }
            }

            //lets load a real program and try to emulate it at 0.4 MHz lol its even slower then a 8086 cpu what the hell
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Filter = "Portable Executable|*.exe";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    asm = new AsmNet(dialog.FileName);
                }
                else
                {
                    return;
                }
            }

            asm.CurrentInstructionEvent += onCurrentInstruction;
            asm.HALTEvent += onHALT;

            Stopwatch InstructionTicks = Stopwatch.StartNew();
            cpu = asm.InitializeCPU();
            listViewEx1.ClearInstructions();
            Instruction[] instructions = new Instruction[cpu.RamMemory.Instructions.Values.Count];
            cpu.RamMemory.Instructions.Values.CopyTo(instructions, 0);
            listViewEx1.AddInstructions(instructions);
            asm.DebugMode = true;
            toolStripStatusLabel3.Text = "Status: Running";
        }

        private void onCurrentInstruction(Instruction instruction)
        {
            if (FormSW.ElapsedMilliseconds >= 500 || slowToolStripMenuItem.Checked)
            {
                if (asm != null && cpu != null)
                {
                    try
                    {
                        SetInformation();
                        FormSW = Stopwatch.StartNew();
                    }
                    catch { }

                    listViewEx1.SelectedInstruction(instruction, autoScrollToolStripMenuItem.Checked);
                }
            }
        }

        private void onHALT()
        {
            toolStripStatusLabel3.Text = "Status: HALT";
            asm.CurrentInstructionEvent -= onCurrentInstruction;
            asm.HALTEvent -= onHALT;
            running = false;
            SetInformation();
        }

        private void SetInformation()
        {
            label1.Text = "EAX: " + cpu.registers.EAX.ToString("X6");
            label2.Text = "ECX: " + cpu.registers.ECX.ToString("X6");
            label3.Text = "EBX: " + cpu.registers.EBX.ToString("X6");
            label4.Text = "EDX: " + cpu.registers.EDX.ToString("X6");
            label5.Text = "EIP: " + cpu.registers.EIP.ToString("X6");
            label20.Text = "ESP: " + cpu.registers.ESP.ToString("X6");
            label19.Text = "EBP: " + cpu.registers.EBP.ToString("X6");
            label18.Text = "ESI: " + cpu.registers.ESI.ToString("X6");
            label17.Text = "EDI: " + cpu.registers.EDI.ToString("X6");

            label6.Text = "AX: " + cpu.registers.AX.ToString("X4");
            label7.Text = "AH: " + cpu.registers.AH.ToString("X4");
            label8.Text = "AL: " + cpu.registers.AL.ToString("X4");
            label9.Text = "IP: " + cpu.registers.IP.ToString("X4");
            label10.Text = "CX: " + cpu.registers.CX.ToString("X4");
            label11.Text = "DX: " + cpu.registers.DX.ToString("X4");
            label12.Text = "BX: " + cpu.registers.BX.ToString("X4");
            toolStripStatusLabel5.Text = "Total executed instructions: " + cpu.TotalExecutedInstructions;
            toolStripStatusLabel7.Text = "Current Speed: " + cpu.CurrentSpeed;
            toolStripStatusLabel1.Text = "IPS: " + cpu.InstructionsPerSecond;
            perfChart1.AddValue(chartLine, cpu.InstructionsPerSecond);
        }

        private void hALTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(cpu != null)
                cpu.HALT = true;
        }

        protected override void OnNotifyMessage(Message m)
        {
            if (m.Msg != 0x14)
                base.OnNotifyMessage(m);
        }

        private void autoScrollToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoScrollToolStripMenuItem.Checked = !autoScrollToolStripMenuItem.Checked;
        }

        private void nextStepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cpu.NextStep();
        }

        private void runLoopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new Thread(() => { cpu.RunLoop(); }).Start();
        }

        private void loadFromFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using(OpenFileDialog dialog = new OpenFileDialog())
            {
                dialog.Multiselect = false;
                if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (running)
                    {
                        if (MessageBox.Show("Already running do you want to stop ?", "Asm.Net - x86 Emulator", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                        {
                            hALTToolStripMenuItem_Click(null, null);
                        }
                    }

                    listViewEx1.ClearInstructions();
                    asm = new AsmNet(File.ReadAllBytes(dialog.FileName));
                    asm.CurrentInstructionEvent += onCurrentInstruction;
                    asm.HALTEvent += onHALT;

                    Stopwatch InstructionTicks = Stopwatch.StartNew();
                    cpu = asm.InitializeCPU();
                    Instruction[] instructions = new Instruction[cpu.RamMemory.Instructions.Values.Count];
                    cpu.RamMemory.Instructions.Values.CopyTo(instructions, 0);
                    listViewEx1.AddInstructions(instructions);

                    asm.DebugMode = true;
                    toolStripStatusLabel3.Text = "Status: Running";
                    running = true;
                }
            }
        }

        private void tCPServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpcodeWriter writer = new OpcodeWriter();

            WSAData wsaData = new WSAData();
            sockaddr_in sockaddr = new sockaddr_in();
            sockaddr_in Clientsockaddr = new sockaddr_in();
            VirtualAddress wsaDataAddr = writer.dataSection.CreateVariable(wsaData);
            VirtualAddress SockinAddress = writer.dataSection.CreateVariable(sockaddr);
            VirtualAddress ClientSockinAddress = writer.dataSection.CreateVariable(Clientsockaddr);
            VirtualAddress ArrayAddress = writer.dataSection.CreateVariable(ASCIIEncoding.ASCII.GetBytes(":)")); //the data we want to send when a client connects

            //socket initialization
            //set the WSADATA settings
            writer.codeSection.MOV_VARIABLE_VALUE(wsaDataAddr, "HighVersion", (ushort)2);
            writer.codeSection.MOV_VARIABLE_VALUE(wsaDataAddr, "Version", (ushort)2);

            //set the sockaddr_in settings, setting the family IPv4
            writer.codeSection.MOV_VARIABLE_VALUE(SockinAddress, "sin_family", (short)ValueCodes.InterNetworkv4);
            //setting port, we need to encode it first...
            writer.codeSection.PUSH_VALUE(1337); //1337=listen port
            writer.codeSection.CALL(Functions.ws2_32_htons);
            writer.codeSection.MOV_VARIABLE_REGISTER(SockinAddress, "sin_port", Register.EAX);

            writer.codeSection.PUSH_VARIABLE(wsaDataAddr);
            writer.codeSection.PUSH_VALUE(36);
            writer.codeSection.CALL(Functions.ws2_32_WSAStartup);

            //started successfully ?
            writer.codeSection.MOV_ECX(0);
            writer.codeSection.CMP(CmpRegisterOpcodes.CMP_ECX_EAX);
            writer.codeSection.JNE("failed");

        //create a socket
            writer.codeSection.PUSH_VALUE(ValueCodes.Tcp, (int)0);
            writer.codeSection.PUSH_VALUE(ValueCodes.Stream, (int)0);
            writer.codeSection.PUSH_VALUE(ValueCodes.InterNetworkv4, (int)0);
            writer.codeSection.CALL(Functions.ws2_32_socket);
                
        //is socket > 0 ?
            writer.codeSection.MOV_ECX((int)ValueCodes.INVALID_SOCKET);
            writer.codeSection.CMP(CmpRegisterOpcodes.CMP_ECX_EAX);
            writer.codeSection.JE("failed");

        //lets move our socket handle to EBX
            writer.codeSection.MOV(MovRegisterOpcodes.MOV_EBX_EAX);

        //lets bind our socket
            writer.codeSection.PUSH_VALUE(Marshal.SizeOf(sockaddr));
            writer.codeSection.PUSH_VARIABLE(SockinAddress); //our sockaddr_in
            writer.codeSection.PUSH_EBX(); //socket handle
            writer.codeSection.CALL(Functions.ws2_32_bind);

            //ok lets listen at a port
            writer.codeSection.PUSH_VALUE((int)100);
            writer.codeSection.PUSH_EBX(); //socket
            writer.codeSection.CALL(Functions.ws2_32_listen);


            //now a infinite loop for accept our connections but lets setup our console
            writer.codeSection.PUSH_VALUE(-11); //STD_OUTPUT_HANDLE
            writer.codeSection.CALL(Functions.Kernel32_GetStdHandle);
            writer.codeSection.MOV(MovRegisterOpcodes.MOV_EDX_EAX);

            writer.codeSection.CreateLabel("loop");
                //lets accept connections
                writer.codeSection.PUSH_VALUE(Marshal.SizeOf(Clientsockaddr));
                writer.codeSection.PUSH_VARIABLE(ClientSockinAddress);
                writer.codeSection.PUSH_EBX(); //server socket
                writer.codeSection.CALL(Functions.ws2_32_accept);
                writer.codeSection.MOV(MovRegisterOpcodes.MOV_EDI_EAX); //set client socket to EDI


                writer.codeSection.PUSH_VALUE(0);
                writer.codeSection.PUSH_VALUE(0);
                writer.codeSection.PUSH_VALUE(20);//char length
                writer.codeSection.PUSH_STRING("new client accepted\r\n");
                writer.codeSection.PUSH_EDX();
                writer.codeSection.CALL(Functions.Kernel32_WriteConsoleA);

                //lets send a packet
                writer.codeSection.PUSH_VALUE(0);
                writer.codeSection.PUSH_VALUE(2);
                writer.codeSection.PUSH_VARIABLE(ArrayAddress);
                writer.codeSection.PUSH_EDI(); //client socket
                writer.codeSection.CALL(Functions.ws2_32_send);

                //close our connection with the client...
                writer.codeSection.PUSH_EDI();
                writer.codeSection.CALL(Functions.ws2_32_closesocket);

            writer.codeSection.JMP("loop");

            writer.codeSection.PUSH_EBX();
            writer.codeSection.CALL(Functions.ws2_32_closesocket);

            writer.codeSection.CreateLabel("failed");
            writer.codeSection.XOR(XorRegisterOpcodes.XOR_ECX_ECX);
            ExecuteCode(writer);
        }

        private void changeValueAtIndexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpcodeWriter writer = new OpcodeWriter();
            VirtualAddress addr = writer.dataSection.CreateVariable(":)"); //create variable
            writer.codeSection.MOV_VARIABLE_INDEX_VALUE(addr, 1, '('); //change ')' to '('

            //show result
            writer.codeSection.PUSH_VALUE(0);
            writer.codeSection.PUSH_VARIABLE(addr);
            writer.codeSection.PUSH_VARIABLE(addr);
            writer.codeSection.PUSH_VALUE(0);
            writer.codeSection.CALL(Functions.User32_MessageBoxA);
            ExecuteCode(writer);
        }

        private void ExecuteCode(OpcodeWriter writer)
        {
            if (running)
            {
                if (MessageBox.Show("Already running do you want to stop ?", "Asm.Net - x86 Emulator", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                {
                    hALTToolStripMenuItem_Click(null, null);
                }
            }

            listViewEx1.ClearInstructions();
            asm = new AsmNet(writer.Generate(true));
            asm.CurrentInstructionEvent += onCurrentInstruction;
            asm.HALTEvent += onHALT;

            Stopwatch InstructionTicks = Stopwatch.StartNew();
            cpu = asm.InitializeCPU();

            Instruction[] instructions = new Instruction[cpu.RamMemory.Instructions.Values.Count];
            cpu.RamMemory.Instructions.Values.CopyTo(instructions, 0);
            listViewEx1.AddInstructions(instructions);

            asm.DebugMode = true;
            toolStripStatusLabel3.Text = "Status: Running";
            running = true;
        }

        private void slowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            slowToolStripMenuItem.Checked = !slowToolStripMenuItem.Checked;
        }

        private void showMessageBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpcodeWriter writer = new OpcodeWriter();

            writer.codeSection.Lazy(x =>
            {
                x.PUSH_VALUE(0);
                x.PUSH_STRING("Title here");
                x.PUSH_STRING("Hello CodeProject!");
                x.PUSH_VALUE(0);
                x.CALL(Functions.User32_MessageBoxA);
            });

            writer.codeSection.PUSH_VALUE(0).PUSH_STRING("Title here").PUSH_STRING("Hello CodeProject!").PUSH_VALUE(0).CALL(Functions.User32_MessageBoxA);

            ExecuteCode(writer);
        }

        private void mathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpcodeWriter writer = new OpcodeWriter();
            VirtualAddress addr = writer.dataSection.CreateVariable((int)668);
            writer.codeSection.MOV_ECX(2);
            writer.codeSection.MOV_REGISTER_DWORD_PTR(Register.EAX, addr);
            writer.codeSection.MUL(MulRegisterOpcodes.MUL_EAX_ECX); //1336
            writer.codeSection.INC_EAX(); //1337
            writer.codeSection.MOV_DWORD_PTR_EAX(addr); //set value from EAX to variable

            ExecuteCode(writer);
        }

        private void infiniteLoopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpcodeWriter writer = new OpcodeWriter();
            writer.codeSection.CreateLabel("begin");
            writer.codeSection.JMP("begin");
            ExecuteCode(writer);
        }

        private void windowMessageFunToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpcodeWriter writer = new OpcodeWriter();
            writer.codeSection.PUSH_VALUE((int)0);
            writer.codeSection.PUSH_VALUE((int)0);
            writer.codeSection.PUSH_VALUE(ValueCodes.WM_ENABLE, (int)0);
            writer.codeSection.PUSH_VALUE(00010088); //taskbar handle
            writer.codeSection.CALL(Functions.User32_SendMessageA);
            ExecuteCode(writer);
        }

        private void tCPClientToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpcodeWriter writer = new OpcodeWriter();
            WSAData wsaData = new WSAData();
            sockaddr_in sockaddr = new sockaddr_in();
            sockaddr_in Clientsockaddr = new sockaddr_in();
            VirtualAddress wsaDataAddr = writer.dataSection.CreateVariable(wsaData);
            VirtualAddress SockinAddress = writer.dataSection.CreateVariable(sockaddr);
            VirtualAddress ClientSockinAddress = writer.dataSection.CreateVariable(Clientsockaddr);
            VirtualAddress UsernameAddress = writer.dataSection.CreateVariable(ASCIIEncoding.ASCII.GetBytes("****************")); //the data we want to send when a client connects
            VirtualAddress PasswordAddress = writer.dataSection.CreateVariable(ASCIIEncoding.ASCII.GetBytes("****************")); //the data we want to send when a client connects
            VirtualAddress SocketAddress = writer.dataSection.CreateVariable(IntPtr.Zero);

            //socket initialization
            //set the WSADATA settings
            writer.codeSection.MOV_VARIABLE_VALUE(wsaDataAddr, "HighVersion", (ushort)2);
            writer.codeSection.MOV_VARIABLE_VALUE(wsaDataAddr, "Version", (ushort)2);

            //set the sockaddr_in settings, setting the family IPv4
            writer.codeSection.MOV_VARIABLE_VALUE(SockinAddress, "sin_family", (short)ValueCodes.InterNetworkv4);
            //setting port, we need to encode it first...
            writer.codeSection.PUSH_VALUE(1337); //1337=listen port
            writer.codeSection.CALL(Functions.ws2_32_htons);
            writer.codeSection.MOV_VARIABLE_REGISTER(SockinAddress, "sin_port", Register.EAX);

            
            writer.codeSection.PUSH_STRING("127.0.0.1"); //ip
            writer.codeSection.CALL(Functions.ws2_32_inet_addr);
            writer.codeSection.MOV_VARIABLE_REGISTER(SockinAddress, "sin_addr", Register.EAX);

            writer.codeSection.PUSH_VARIABLE(wsaDataAddr);
            writer.codeSection.PUSH_VALUE(36);
            writer.codeSection.CALL(Functions.ws2_32_WSAStartup);

            //started successfully ?
            writer.codeSection.MOV_ECX(0);
            writer.codeSection.CMP(CmpRegisterOpcodes.CMP_ECX_EAX);
            writer.codeSection.JNE("failed");

        //create a socket
            writer.codeSection.PUSH_VALUE(ValueCodes.Tcp, (int)0);
            writer.codeSection.PUSH_VALUE(ValueCodes.Stream, (int)0);
            writer.codeSection.PUSH_VALUE(ValueCodes.InterNetworkv4, (int)0);
            writer.codeSection.CALL(Functions.ws2_32_socket);
                
        //is socket > 0 ?
            writer.codeSection.MOV_ECX((int)ValueCodes.INVALID_SOCKET);
            writer.codeSection.CMP(CmpRegisterOpcodes.CMP_ECX_EAX);
            writer.codeSection.JE("failed");

        //lets move our socket handle to EBX
            writer.codeSection.MOV(MovRegisterOpcodes.MOV_EBX_EAX);

            
            writer.codeSection.PUSH_VALUE(Marshal.SizeOf(new sockaddr_in()));
            writer.codeSection.PUSH_VARIABLE(SockinAddress);
            writer.codeSection.PUSH_EBX();
            writer.codeSection.CALL(Functions.ws2_32_connect);

            
            writer.codeSection.MOV_ECX(0);
            writer.codeSection.CMP(CmpRegisterOpcodes.CMP_ECX_EAX);
            writer.codeSection.JNE("UnableToConnect");
            writer.codeSection.JMP("end");


            writer.codeSection.CreateLabel("failed");
            writer.codeSection.PUSH_VALUE(0);
            writer.codeSection.PUSH_STRING("Something went wrong... unable to connect?");
            writer.codeSection.PUSH_STRING("Something went wrong... unable to connect?");
            writer.codeSection.PUSH_VALUE(ValueCodes.MB_OK, (int)0);
            writer.codeSection.CALL(Functions.User32_MessageBoxA);
            writer.codeSection.JMP("end");

            
            writer.codeSection.CreateLabel("UnableToConnect");
            writer.codeSection.PUSH_VALUE(0);
            writer.codeSection.PUSH_STRING("unable to connect?");
            writer.codeSection.PUSH_STRING("unable to connect?");
            writer.codeSection.PUSH_VALUE(ValueCodes.MB_OK, (int)0);
            writer.codeSection.CALL(Functions.User32_MessageBoxA);
            writer.codeSection.JMP("end");


            writer.codeSection.CreateLabel("end");
            writer.codeSection.XOR(XorRegisterOpcodes.XOR_ECX_ECX);
            ExecuteCode(writer);
        }
    }
}