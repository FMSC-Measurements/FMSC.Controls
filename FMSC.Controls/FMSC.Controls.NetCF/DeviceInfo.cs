using System.Text;
using Microsoft.Win32;
using System;

namespace FMSC.Controls
{
    public enum PlatformType { Unknown = 0, Win32Windows =  1, WinCE = 3, WM = 7 }; 

    public static class DeviceInfo
    {
        private static PlatformType _devicePlatform = PlatformType.Unknown;

        public static PlatformType DevicePlatform
        {
            get
            {
                if (_devicePlatform == PlatformType.Unknown)
                {
                    _devicePlatform = GetPlatformType();
                }
                return _devicePlatform;
            }
        }

        public static string UserName
        {
            get
            {
                return GetUserName();
            }
        }

        public static string SerialNumber
        {
            get
            {
                return GetSerialNumber();
            }
        }

        public static string MachineName
        {
            get
            {
                return GetMachineName();
            }
        }

        #region Memory status methods
        public static uint GetAppVirtualMemoryLoadPercent()
        {
            FMSC.Controls.Win32.MEMORYSTATUS memStatus = new FMSC.Controls.Win32.MEMORYSTATUS();
            FMSC.Controls.Win32.GlobalMemoryStatus(ref memStatus);
            return memStatus.dwAvailVirtual / memStatus.dwTotalVirtual;
        }

        public static uint GetPhysicalMemoryLoadPercent()
        {
            //UInt32 storePages = 0;
            //UInt32 ramPages = 0;
            //UInt32 pageSize = 0;
            //int res = GetSystemMemoryDivision(ref storePages, 
            // ref ramPages, ref pageSize);

            // Call the native GlobalMemoryStatus method
            // with the defined structure.
            FMSC.Controls.Win32.MEMORYSTATUS memStatus = new FMSC.Controls.Win32.MEMORYSTATUS();
            FMSC.Controls.Win32.GlobalMemoryStatus(ref memStatus);

            return memStatus.dwMemoryLoad;
        }
        #endregion

        #region Power status methods
        public static uint GetMainBatteryChargePercent()
        {
            FMSC.Controls.Win32.SYSTEM_POWER_STATUS_EX status = new FMSC.Controls.Win32.SYSTEM_POWER_STATUS_EX();

            if (FMSC.Controls.Win32.GetSystemPowerStatusEx(status, false) == 1)
            {
                return status.BatteryLifePercent;
            }

            return 0;
        }

        public static uint GetBackupBatteryChargePercent()
        {
            FMSC.Controls.Win32.SYSTEM_POWER_STATUS_EX status = new FMSC.Controls.Win32.SYSTEM_POWER_STATUS_EX();

            if (FMSC.Controls.Win32.GetSystemPowerStatusEx(status, false) == 1)
            {
                return status.BackupBatteryLifePercent; // careful. returns 255 if no backup battery
            }

            return 0;
        }
        #endregion

        #region Device Info methods
        public static PlatformType GetPlatformType()
        {
            //try
            //{
            //    Microsoft.WindowsCE.Forms.InputPanel sip = new Microsoft.WindowsCE.Forms.InputPanel();
            //    return PlatformType.WM;
            //}
            //catch
            //{
            //    return PlatformType.WinCE;
            //}

            switch (System.Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    return PlatformType.Win32Windows;
                case PlatformID.WinCE:
                    return CheckWinCEPlatform();
                default:
                    return PlatformType.Unknown;
            }
        }

        private static PlatformType CheckWinCEPlatform()
        {
            StringBuilder strbuild = new StringBuilder(200);
            FMSC.Controls.Win32.SystemParametersInfo(FMSC.Controls.Win32.SPI_GETPLATFORMTYPE, 200, strbuild, 0);
            string str = strbuild.ToString();
            switch (str)
            {
                case "PocketPC":                    
                case "SmartPhone":
                    return PlatformType.WM;
                default:
                    return PlatformType.WinCE;
            }
        }


        public static string GetOEMInfo()
        {
            throw new NotImplementedException();
            //try
            //{
            //    StringBuilder sb = new StringBuilder(256);

            //    if (FMSC.Controls.Win32.SystemParametersInfoString(FMSC.Controls.Win32.SPI_GETOEMINFO, sb.Capacity, sb, 0) != 0)
            //    {
            //        return sb.ToString();
            //    }
            //}
            //catch
            //{
            //}

            //return "<Unknown>";
        }

        private static string GetSerialNumber()
        {
            string machineName = GetMachineName();
            string serialNumber = "<Unknown>";
            // Try and glean the serial number (it might be in the machine name).
            if (machineName != "<Unknown>")
            {
                System.Text.RegularExpressions.Match match=
                    System.Text.RegularExpressions.Regex.Match(machineName, @"\s+[a-fA-F]*[0-9]+$");//find number of hex value
                if(match != null && match.Success && 
                    (match.Length < 5 || match.Length > 10))// Serial numbers should be between 5 and 8 digits (Juniper's seem to be all 5 digits).
                {
                    serialNumber = match.Value;
                }
                else
                {
                    serialNumber = "<Unknown>";
                }
            }
            return serialNumber;
        }

        /// <summary>
        /// Get the username
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > Owner Information > Identification > Name)</returns>
        private static string GetUserName()
        {
            string str = "<Unknown>";

            try
            {
                RegistryKey key = Registry.CurrentUser.OpenSubKey(@"ControlPanel\Owner");
                if (key == null) { return str; }

                str = (string)key.GetValue("Name", string.Empty);
                if (str != string.Empty)
                {
                    return str;
                }

                byte[] bytes = (byte[])key.GetValue("Owner", null);

                if (bytes != null)
                {
                    str = Encoding.Unicode.GetString(bytes, 0, 0x48);
                    str = str.Substring(0, str.IndexOf("\0"));
                }
            }
            catch
            {
            }

            return str;
        }

        /// <summary>
        /// This should be where the user enters the device's serial number, e.g. "AllegroMX_77404"
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > System > About > Device ID > Device Name)</returns>
        private static string GetMachineName()
        {
            string str = "<Unknown>";

            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Ident");
                str = key.GetValue("Name").ToString();                
                key.Close();
            }
            catch
            {
            }

            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Whatever has been entered into (Settings > System > About > Device ID > Description)</returns>
        public static string GetMachineDescription()
        {
            string str = "<Unknown>";

            try
            {
                RegistryKey key = Registry.LocalMachine.OpenSubKey("Ident");
                str = key.GetValue("Desc").ToString();
                key.Close();
            }

            catch
            {
            }

            return str;
        }
        #endregion
    }
}
