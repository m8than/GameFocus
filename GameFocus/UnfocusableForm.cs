﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GameFocus
{
    class UnfocusableForm : Form
    {
        private const int WS_EX_NOACTIVATE = 0x08000000;

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                cp.ExStyle |= WS_EX_NOACTIVATE;

                return cp;
            }
        }
        public UnfocusableForm(Rectangle bounds, double opacity)
        {
            this.Width = bounds.Right - bounds.Left;
            this.Height = bounds.Bottom - bounds.Top;
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(bounds.Left, bounds.Top);
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.Black;
            this.Opacity = opacity;
        }
    }
}
