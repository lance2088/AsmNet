using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using AsmEngine.Events;
using System.Threading;
using AsmEngine.Video;
using ASM.net.src;

namespace ASM.net
{
    public partial class VirtualScreen : Form
    {
        public bool KnownVideoMode;
        private GraphicsDevice device;

        public VirtualScreen(Size size)
        {
            InitializeComponent();
            this.Size = size;
            this.Text = "ASM.net - VirtualScreen - [" + size.Width + "*" + size.Height + "]";
            DrawPixelEvent.DrawPixel += onDrawPixel;
            SetVideoModeEvent.SetVideoMode += onSetVideoMode;
        }

        private void onDrawPixel(DrawPixelEventArgs e)
        {
            try
            {
                device.SetPixel(e.color, new Point(e.column, e.row));
            }
            catch { }
        }

        private void onSetVideoMode(SetVideoModeEventArgs e)
        {
            try
            {
                this.Size = new Size(e.width, e.height);
                this.Text = "ASM.net - VirtualScreen - [" + e.width + "*" + e.height + "]";
                device = new GraphicsDevice(this.Handle, this.Size, Settings.VirtualScreenFPS);
                device.StartDrawLoop();
                KnownVideoMode = true;
                this.ShowDialog();
            }
            catch { }
        }

        private void VirtualScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            device = null;
            DrawPixelEvent.DrawPixel -= onDrawPixel;
            SetVideoModeEvent.SetVideoMode -= onSetVideoMode;
        }
    }
}