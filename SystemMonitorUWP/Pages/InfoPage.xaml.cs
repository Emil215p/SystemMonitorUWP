using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Uwp.Helpers;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Windows.Security.ExchangeActiveSyncProvisioning;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SystemMonitorUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InfoPage : Page
    {
        public string OSName { get; set; }
        public string OSVersion { get; set; }
        public string NETVersion { get; set; }
        public string OSArch { get; set; }
        public string CPUArch { get; set; }
        public string CPURevision { get; set; }
        public string CPULevel { get; set; }
        public string CPUType { get; set; }
        public string CPUCores { get; set; }
        public string DeviceID { get; set; }
        public string OperatingSystem { get; set; }
        public string FriendlyName { get; set; }
        public string SystemManufacturer { get; set; }
        public string SystemProductName { get; set; }
        public string SystemSku { get; set; }

        public InfoPage()
        {
            this.InitializeComponent();
            try
            {
                LoadInfo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error loading system info: " + ex.Message);
            }

            this.DataContext = this;
        }

        public void LoadInfo()
        {
            SYSTEM_INFO systemInfo = new();
            GetNativeSystemInfo(ref systemInfo);

            EasClientDeviceInformation CurrentDeviceInfor = new();
            this.DeviceID = "Device ID: " + CurrentDeviceInfor.Id.ToString();
            this.OperatingSystem = "Operating System: " + CurrentDeviceInfor.OperatingSystem;
            this.FriendlyName = "Device Name: " + CurrentDeviceInfor.FriendlyName;
            this.SystemManufacturer = "Device Manufacturer: " + CurrentDeviceInfor.SystemManufacturer;
            this.SystemProductName = "Product Name: " + CurrentDeviceInfor.SystemProductName;
            this.SystemSku = "System SKU: " + CurrentDeviceInfor.SystemSku;

            this.OSName = "OS Name: " + RuntimeInformation.OSDescription;
            this.OSVersion = "OS Version: " + SystemInformation.Instance.OperatingSystemVersion.ToString();
            this.OSArch = "OS Architecture: " + RuntimeInformation.OSArchitecture;
            this.CPUArch = "Processor Architecture: " + GetProcessorArchitecture();
            this.CPURevision = "CPU Revision: " + GetProcessorRevision(systemInfo.wProcessorRevision);
            this.CPULevel = "CPU Level: " + GetProcessorLevel(systemInfo.wProcessorLevel);
            this.CPUType = "CPU Type: " + GetProcessorType(systemInfo.dwProcessorType);
            this.CPUCores = "Number of CPU Threads: " + GetNumberOfProcessors(systemInfo.dwNumberOfProcessors);
            this.NETVersion = ".NET Version: " + RuntimeInformation.FrameworkDescription;
        }

        private string GetProcessorArchitecture()
        {
            SYSTEM_INFO systemInfo = new();
            GetNativeSystemInfo(ref systemInfo);

            return systemInfo.wProcessorArchitecture switch
            {
                0 => "x86",
                5 => "ARM",
                6 => "Itanium",
                9 => "x64",
                12 => "ARM64",
                _ => "Unknown",
            };
        }

        private string GetProcessorRevision(ushort revision)
        {
            byte major = (byte)(revision >> 8);
            byte minor = (byte)(revision & 0xFF);
            return $"{major:X2}{minor:X2}";
        }

        private string GetProcessorLevel(uint level)
        {
            return level.ToString();
        }


        private string GetProcessorType(uint type)
        {
            return type.ToString();
        }

        private string GetNumberOfProcessors(uint number)
        {
            return number.ToString();
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEM_INFO
        {
            public ushort wProcessorArchitecture;
            public ushort wReserved;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public IntPtr dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [DllImport("kernel32.dll")]
        private static extern void GetNativeSystemInfo(ref SYSTEM_INFO lpSystemInfo);
    }
}
