using System;
using System.Runtime.InteropServices;
using System.Text;

namespace FMSC.Controls
{

    public class Win32
    {
        public const int BS_MULTILINE = 0x00002000; // Button Style Multiline

        #region Structures 
        [StructLayout(LayoutKind.Sequential)]
        public struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        };

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORYSTATUS
        {
            public UInt32 dwLength;
            public UInt32 dwMemoryLoad;
            public UInt32 dwTotalPhys;
            public UInt32 dwAvailPhys;
            public UInt32 dwTotalPageFile;
            public UInt32 dwAvailPageFile;
            public UInt32 dwTotalVirtual;
            public UInt32 dwAvailVirtual;
        };

        [StructLayout(LayoutKind.Sequential)]
        public class SYSTEM_POWER_STATUS_EX
        {
            public byte ACLineStatus;
            public byte BatteryFlag;
            public byte BatteryLifePercent;
            public byte Reserved1;
            public uint BatteryLifeTime;
            public uint BatteryFullLifeTime;
            public byte Reserved2;
            public byte BackupBatteryFlag;
            public byte BackupBatteryLifePercent;
            public byte Reserved3;
            public uint BackupBatteryLifeTime;
            public uint BackupBatteryFullLifeTime;
        }

        #endregion

        [Flags]
        public enum QueueStatusFlags : uint
        {
            QS_KEY = 0x0001,
            QS_MOUSEMOVE = 0x0002,
            QS_MOUSEBUTTON = 0x0004,
            QS_POSTMESSAGE = 0x0008,
            QS_TIMER = 0x0010,
            QS_PAINT = 0x0020,
            QS_SENDMESSAGE = 0x0040,
            QS_HOTKEY = 0x0080,
            QS_ALLPOSTMESSAGE = 0x0100,
            QS_RAWINPUT = 0x0400,
            QS_MOUSE = QS_MOUSEMOVE | QS_MOUSEBUTTON,
            QS_INPUT = QS_MOUSE | QS_KEY | QS_RAWINPUT,
            QS_ALLEVENTS = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY,
            QS_ALLINPUT = QS_INPUT | QS_POSTMESSAGE | QS_TIMER | QS_PAINT | QS_HOTKEY | QS_SENDMESSAGE
        };


        [Flags]
        public enum SHGetFileInfoFlags : uint
        {
            SHGFI_LARGEICON =           0x000000000, //modify SHGFI_ICON to retrieve large icon, SHGFI_ICON must be set too
            SHGFI_SMALLICON =           0x000000001,
            //SHGFI_OPENICON=         0x000000002, //modify SHGFI_ICON to return the file's open icon
            //SHGFI_SHELLICONSIZE =   0x000000004,
            SHGFI_PIDL =                0x000000008,
            SHGFI_USEFILEATTRIBUTES =   0x000000010,
            //SHGFI_ADDOVERLAYS =     0x000000020, //apply overlays to file icons
            //SHGFI_OVERLAYINDEX=     0x000000040,
            SHGFI_ICON =                0x000000100, //retrieve handle to icon + index of icon in System image list
            SHGFI_DISPLAYNAME =         0x000000200, //retrieve display name of file
            SHGFI_TYPENAME =            0x000000400, //retrieve type name
            SHGFI_ATTRIBUTES =          0x000000800, //retrieve a items attributes
            SHGFI_ICONLOCATION =        0x000001000, //retrieve name of file containing icon + index of icon in file
            //SHGFI_EXETYPE=          0x000002000, //retrieve type of the exe file if path is exe
            SHGFI_SYSICONINDEX =        0x000004000, //retrieve system icon index
            //SHGFI_LINKOVERLAY=      0x000008000, //modify SHGFI_ICON to add link overlay to icons, SHGFI_ICON must be set too
            //SHGFI_SELECTED =        0x000010000, 
            //SHGFI_ATTR_SPECIFIED =  0x000020000, //indicates that desired attributes are specified in SHFILEINO struct
        };


        #region Codes
        //ListView Message Codes
        //Reference : < http://msdn.microsoft.com/en-us/library/windows/desktop/ff485961(v=vs.85).aspx >
        public const int LVM_SETIMAGELIST = 0x1003;    //List view Set image list
        //List View Style
        //Reference : < C:\Program Files\Windows Mobile 5.0 SDK R2\PocketPC\Include\Armv4i\commctrl.h : line#2013 >
        public const int LVS_SHAREIMAGELISTS = 0x0040;

        //ComboBox Messages codes
        //more CB message reference < http://msdn.microsoft.com/en-us/library/ms907150.aspx >
        public const uint CB_SHOWDROPDOWN = 0x014f;//335
        public const uint CB_GETDROPPEDSTATE = 0x0157;//343
        public const uint CB_GETEDITSEL = 0x0140;//320
        public const uint CB_SETEDITSEL = 0x0142;//322

        //SetWindowLong / GetWindowLong field offsets
        //Reference : < http://msdn.microsoft.com/en-us/library/windows/desktop/ms633591(v=vs.85).aspx >
        public const int GWL_STYLE = -16;


        public const uint MB_ICONEXCLAMATION = 0x00000030;// in C:\Program Files\Windows Mobile 6 SDK\PocketPC\Include\Armv4i\winuser.h
        public const uint MB_ICONHAND = 0x00000010;
        public const uint MB_ICONQUESTION = 0x00000020;
        public const uint MB_ICONASTERISK = 0x00000040;

        //Parameters for SystemParametersInfo http://msdn.microsoft.com/en-us/library/aa932539.aspx
        public const int SPI_GETPLATFORMTYPE = 257;
        public const int SPI_GETOEMINFO = 258;
        //public const int SPI_GETPROJECTNAME = 259//WM 6.0 or newer
        //public const int SPI_GETPLATFORMNAME = 260;//WM 6.0 or newer
        //public const int SPI_GETPLATFORMMANUFACTURER = 262;//WM 6.0 or newer
        #endregion

        public static IntPtr MAKELPARAM(int low, int high)
        {
            return (IntPtr)(high << 16 | low & (int)ushort.MaxValue);
        }

        

#if NET_CF
        [DllImport("coredll.dll", EntryPoint = "SendMessage", SetLastError = true)]
        public static extern int SendMessage(IntPtr hWnd, uint msg, int wParam, int lParam);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, String lParam);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, String lParam);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, byte[] lParam);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, byte[] lParam);

        [DllImport("coredll.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("coredll.dll", EntryPoint = "SetWindowLong")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("coredll.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("CoreDll.dll")]
        public static extern bool MessageBeep(uint code);

        [DllImport("coredll.dll", EntryPoint = "SystemParametersInfo", SetLastError = true)]
        public static extern int SystemParametersInfo(int uiAction, int uiParam, StringBuilder pvParam, int fWinIni);

        [DllImport("coredll", EntryPoint = "GlobalMemoryStatus", SetLastError = false)]
        public static extern void GlobalMemoryStatus
        (
            ref MEMORYSTATUS lpBuffer
        );

        [DllImport("coredll")]
        public static extern uint GetSystemPowerStatusEx
        (
            SYSTEM_POWER_STATUS_EX lpSystemPowerStatus,
            bool fUpdate
        );
#else
        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, int lParam);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, IntPtr lParam);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, String lParam);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, String lParam);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, byte[] lParam);

        [DllImport("User32.dll", SetLastError = true)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, byte[] lParam);

        //[DllImport("Shell32.dll", CharSet = CharSet.Auto)]
        //public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref SHFILEINFO psfi, uint cbFileInfo, uint uFlags);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GlobalMemoryStatus(ref MEMORYSTATUS lpBuffer);
#endif

        public static int MeasureTextWidth(System.Windows.Forms.Control c, string text)
        {
            if (c == null)
            { return -1; }
            using (System.Drawing.Graphics g = c.CreateGraphics())
            {
                return (int)Math.Ceiling(g.MeasureString(text, c.Font).Width);
            }
        }
    }


}
