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

        public InfoPage()
        {
            this.InitializeComponent();

            try
            {
                Debug.WriteLine(RuntimeInformation.ProcessArchitecture);
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
            this.NETVersion = ".NET Version: " + RuntimeInformation.FrameworkDescription;
        }
    }
}
