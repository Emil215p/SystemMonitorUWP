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
using SystemMonitorUWP.Code;
using System.Diagnostics;
using Windows.Storage;
using System.Security.AccessControl;
using Windows.Storage.Provider;

namespace SystemMonitorUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonitoringPage : Page
    {
        public App AppInstance => (App)Application.Current;
        public Shared SharedInstance { get; } = Shared.Instance;
        public string[] CsvValues { get; private set; }
        public string MemoryLoad { get; set; }
        public string TotalMemory { get; set; }
        public string AvailableMemory { get; set; }
        public string TotalPageFile { get; set; }
        public string AvailablePageFile { get; set; }
        public string TotalVirtualMemory { get; set; }
        public string AvailableVirtualMemory { get; set; }
        public string AvailableExtendedMemory { get; set; }

        private DispatcherTimer _refreshTimer;

        public MonitoringPage()
        {
            this.InitializeComponent();
            StartRefreshTimer();
        }

        private void StartRefreshTimer()
        {
            _refreshTimer = new DispatcherTimer();
            _refreshTimer.Interval = TimeSpan.FromSeconds(5);
            _refreshTimer.Tick += RefreshTimer_Tick;
            _refreshTimer.Start();
        }

        private async void RefreshTimer_Tick(object sender, object e)
        {
            await Shared.Instance.FullTrustLauncher(null);
            ReadCSV("Common");
        }

        private async void ReadCSV(string CSV)
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync(CSV + ".csv");
                string[] lines = (await FileIO.ReadTextAsync(file)).Split([',']);
                CsvValues = lines;
                foreach (var line in CsvValues)
                {
                    Debug.WriteLine($"Value: {line}");
                }
                Debug.WriteLine(CsvValues.Length);
                MemoryLoad = "Memory usage: " + CsvValues[0] + "%";
                TotalMemory = "Total memory: " + CsvValues[1] + " kb";
                AvailableMemory = "Available memory: " + CsvValues[2] + " kb";
                TotalPageFile = "Page file: " + CsvValues[3] + " kb";
                AvailablePageFile = "Available page file: " + CsvValues[4] + " kb";
                TotalVirtualMemory = "Total virtual memory: " + CsvValues[5] + " kb";
                AvailableVirtualMemory = "Available virtual memory: " + CsvValues[6] + " kb";
                AvailableExtendedMemory = "Total extended memory: " + CsvValues[7] + " kb";
                this.Bindings.Update();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error reading " + CSV + ".csv: " + ex.Message);
            }
        }
    }
}
