using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileJump.Network
{
    public static class ProgramSettings
    {

        public static bool RunOnStartUp { get; set; } = true;
        public static string StorageFolderPath { get; set; }

        public static string DeviceName { get; set; } = "TEST_PC1";

        public static NetworkDeviceType DeviceType { get; set; } = NetworkDeviceType.MobilePhone;

        /*
        public static void SetStartup()
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (RunOnStartUp)
                rk.SetValue(AppName, Application.ExecutablePath);
            else
                rk.DeleteValue(AppName, false);

        }
        */
    }
}
