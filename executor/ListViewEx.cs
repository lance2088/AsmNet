using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Asm.Net.src;
using Asm.Net.src.Interfaces;
using System.Runtime.InteropServices;

namespace executor
{
    public partial class ListViewEx : ListView
    {
        [DllImport("user32")]
        public static extern int LockWindowUpdate(IntPtr hwnd);

        private ImageList imageList;

        ///<summary>
        ///     This Listview is coded by DragonHunter, 
        ///     This ListView is used for showing the assembly code easier for debugging
        ///</summary>
        public ListViewEx()
        {
            InitializeComponent();
            imageList = new ImageList();
            imageList.ColorDepth = ColorDepth.Depth32Bit;
            imageList.ImageSize = new System.Drawing.Size(10, 10);
            imageList.Images.Add(new Bitmap(1, 1));
            imageList.Images.Add(Properties.Resources.line);
            imageList.Images.Add(Properties.Resources.line2);
            imageList.Images.Add(Properties.Resources.line3);
            
            Bitmap bmp = (Bitmap)Properties.Resources.line2.Clone();
            bmp.RotateFlip(RotateFlipType.Rotate180FlipX);
            imageList.Images.Add(bmp);

            Bitmap bmp2 = (Bitmap)Properties.Resources.line3.Clone();
            bmp2.RotateFlip(RotateFlipType.Rotate180FlipX);
            imageList.Images.Add(bmp2);

            this.SmallImageList = imageList;

            this.Columns.AddRange(new ColumnHeader[]
            {
                new ColumnHeader { Text = "Address", Width = 75 },
                new ColumnHeader { Text = "Bytes", Width = 118 },
                new ColumnHeader { Text = "Instruction", Width = 205 },
                new ColumnHeader { Text = "Extra Information", Width = 148 }
            });

            this.View = System.Windows.Forms.View.Details;
            this.FullRowSelect = true;
            this.GridLines = true;
        }

        protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
        {
            for (int i = 0; i < this.Items.Count; i++)
                this.Items[i].ImageIndex = 0;

            if (e.Item.Tag.GetType().GetInterfaces()[0] == typeof(IJump))
            {
                int JmpAddr = ((IJump)e.Item.Tag).JumpAddress;
                int JmpIndex = e.ItemIndex;
                int DestIndex = 0;

                for (int i = 0; i < this.Items.Count; i++)
                {
                    if (((Instruction)this.Items[i].Tag).VirtualAddress.Address == JmpAddr)
                    {
                        DestIndex = i;
                        break;
                    }
                }

                if (JmpIndex < DestIndex)
                {
                    for (int i = JmpIndex; i < DestIndex; i++)
                    {
                        this.Items[i].ImageIndex = 1;
                    }
                    this.Items[JmpIndex].ImageIndex = 4;
                    this.Items[DestIndex].ImageIndex = 5;
                }
                else
                {
                    for (int i = DestIndex; i < JmpIndex; i++)
                    {
                        this.Items[i].ImageIndex = 1;
                    }
                    this.Items[JmpIndex].ImageIndex = 2;
                    this.Items[DestIndex].ImageIndex = 3;
                }
            }
            base.OnItemSelectionChanged(e);
        }

        public void SelectedInstruction(Instruction instruction, bool ScrollToInstruction)
        {
            LockWindowUpdate(this.Handle);
            for (int i = 0; i < this.Items.Count; i++)
            {
                if (this.Items[i].Tag == instruction)
                {
                    this.Items[i].BackColor = Color.Green;
                    if (ScrollToInstruction)
                    {
                        this.Items[i].EnsureVisible();
                    }
                }
                else
                {
                    this.Items[i].BackColor = this.BackColor;
                }
            }
            LockWindowUpdate(IntPtr.Zero);
        }

        public void AddInstruction(Instruction instruction)
        {
            ListViewItem item = new ListViewItem(new string[]
            {
                "0x" + instruction.VirtualAddress.Address.ToString("X"),
                BitConverter.ToString(instruction.ToByteArray()).Replace("-", " "),
                instruction.ToString(),
                instruction.ExtraInformation
            });
            item.Tag = instruction;
            this.Items.Add(item);
        }
        public void AddInstructions(Instruction[] instructions)
        {
            for (int i = 0; i < instructions.Length; i++)
                AddInstruction(instructions[i]);
        }

        public void ClearInstructions()
        {
            this.Items.Clear();
        }
    }
}