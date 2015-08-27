using System;

using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace FMSC.Controls
{
    /// <summary>A helper object to adjust the sizes of controls based on the DPI.</summary>
    public class DpiHelper
    {
        /// <summary>The real dpi of the device.</summary>
        private static int dpi =
          SafeNativeMethods.GetDeviceCaps(IntPtr.Zero, /*LOGPIXELSX*/88);

        public static bool IsRegularDpi
        {
            get
            {
                if (dpi == 96) return true;
                else return false;
            }
        }

        /// <summary />Adjust the sizes of controls to account
        ///           for the DPI of the device.</summary >
        /// <param name="parent" />The parent node
        ///          of the tree of controls to adjust.</param />
        public static void AdjustAllControls(Control parent)
        {
            if (!IsRegularDpi)
            {
                foreach (Control child in parent.Controls)
                {
                    
                    AdjustAllControls(child);
                    AdjustControl(child);
                }
            }
        }

        public static void AdjustControl(Control control)
        {
            if (control.GetType() == typeof(TabPage)) return;
            switch (control.Dock)
            {
                case DockStyle.None:
                    control.Bounds = new Rectangle(
                        control.Left * dpi / 96,
                        control.Top * dpi / 96,
                        control.Width * dpi / 96,
                        control.Height * dpi / 96);
                    break;
                case DockStyle.Left:
                case DockStyle.Right:
                    control.Bounds = new Rectangle(
                        control.Left,
                        control.Top,
                        control.Width * dpi / 96,
                        control.Height);
                    break;
                case DockStyle.Top:
                case DockStyle.Bottom:
                    control.Bounds = new Rectangle(
                        control.Left,
                        control.Top,
                        control.Width,
                        control.Height * dpi / 96);
                    break;
                case DockStyle.Fill:
                    //Do nothing;
                    break;
            }
        }

        /// <summary />Scale a coordinate to account for the dpi.</summary />
        /// <param name="x" />The number of pixels at 96dpi.</param />
        public static int Scale(int x)
        {
            return x * dpi / 96;
        }

        public static int UnScale(int x)
        {
            return x * 96 / dpi;
        }

        private class SafeNativeMethods
        {
            [DllImport("coredll.dll")]
            static internal extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        }
    }
}
