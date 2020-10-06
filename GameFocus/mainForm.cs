using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Threading;

namespace GameFocus
{

    public partial class mainForm : Form
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;

            public static implicit operator Point(POINT point)
            {
                return new Point(point.X, point.Y);
            }
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        private static extern IntPtr GetShellWindow();
        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowRect(IntPtr hwnd, out RECT rc);

        private bool isOn;

        private RECT focusWindowPos;
        private Rectangle primaryMonitorPos;
        private bool fullscreenAppOpen;

        private int trackOpacityValue;


        public mainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.btnOff.Enabled = false;
            this.isOn = false;
            this.trackOpacityValue = trackOpacity.Value;
            this.fullscreenAppOpen = false;

            Thread tFullscreenChecker = new Thread(fullscreenChecker);
            tFullscreenChecker.IsBackground = true;
            tFullscreenChecker.Start();

            Thread tInputBlockerControl = new Thread(inputBlockerControl);
            tInputBlockerControl.IsBackground = true;
            tInputBlockerControl.Start();
        }
        private void inputBlockerControl()
        {
            List<Form> inputBlockers = new List<Form>();
            while (true)
            {
                while (this.isOn)
                {
                    if (this.fullscreenAppOpen)
                    {
                        //window fullscreen so create input block forms
                        foreach (Screen a in Screen.AllScreens)
                        {
                            Rectangle bounds = a.Bounds;
                            // if not primary monitor
                            if (
                                bounds.Top != primaryMonitorPos.Top ||
                                bounds.Left != primaryMonitorPos.Left ||
                                bounds.Bottom != primaryMonitorPos.Bottom ||
                                bounds.Right != primaryMonitorPos.Right
                                )
                            {
                                Form blocker = new UnfocusableForm(bounds, trackOpacityValue / 10);
                                blocker.Show();
                                inputBlockers.Add(blocker);
                            }
                        }

                        while (this.fullscreenAppOpen)
                        {
                            Thread.Sleep(500);
                            Application.DoEvents();
                        }

                        //window not fullscreen so destroy input block forms
                        foreach (Form a in inputBlockers)
                        {
                            a.Close();
                        }
                        inputBlockers.Clear();
                    }

                    Thread.Sleep(500);
                }
                Thread.Sleep(1000);
            }
        }

        private void fullscreenChecker()
        {
            IntPtr hWnd;

            while (true)
            {
                while (this.isOn)
                {
                    primaryMonitorPos = Screen.PrimaryScreen.WorkingArea;
                    hWnd = GetForegroundWindow();
                    if (hWnd != null)
                    {
                        GetWindowRect(hWnd, out focusWindowPos);
                        if (focusWindowPos.Bottom == primaryMonitorPos.Bottom &&
                            focusWindowPos.Top == primaryMonitorPos.Top &&
                            focusWindowPos.Left == primaryMonitorPos.Left &&
                            focusWindowPos.Right == primaryMonitorPos.Right
                            )
                        {
                            this.fullscreenAppOpen = true;
                        } else
                        {
                            this.fullscreenAppOpen = false;
                        }
                    }
                    Thread.Sleep(500);
                }
                Thread.Sleep(1000);
            }
        }

        private void BtnOn_Click(object sender, EventArgs e)
        {
            this.isOn = true;
            this.btnOn.Enabled = false;
            this.btnOff.Enabled = true;
        }

        private void BtnOff_Click(object sender, EventArgs e)
        {
            this.isOn = false;
            this.btnOn.Enabled = true;
            this.btnOff.Enabled = false;
        }
        private void TrackOpacity_Scroll(object sender, EventArgs e)
        {
            trackOpacityValue = trackOpacity.Value;
        }
    }
}
