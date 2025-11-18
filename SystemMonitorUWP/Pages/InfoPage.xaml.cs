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
        public string SysArch { get; set; }

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
            this.OSName = "OS Name: " + RuntimeInformation.OSDescription;
            this.OSVersion = "OS Version: " + SystemInformation.Instance.OperatingSystemVersion.ToString();
            this.OSArch = "OS Architecture: " + RuntimeInformation.OSArchitecture;
            this.SysArch = "Processor Architecture: " + GetProcessorArchitecture();
            this.NETVersion = ".NET Version: " + RuntimeInformation.FrameworkDescription;
        }

        private string GetProcessorArchitecture()
        {
            SYSTEM_INFO systemInfo = new SYSTEM_INFO();
            GetNativeSystemInfo(ref systemInfo);

            switch (systemInfo.wProcessorArchitecture)
            {
                case 0: return "x86";
                case 9: return "x64";
                case 5: return "ARM";
                case 12: return "ARM64";
                default: return "Unknown";
            }
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
