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

namespace SystemMonitorUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MonitoringPage : Page
    {
        public App AppInstance => (App)Application.Current;
        public Shared SharedInstance { get; } = Shared.Instance;

        public MonitoringPage()
        {
            this.InitializeComponent();
            ReadAndLogCommonCsv();
        }

        private async void ReadAndLogCommonCsv()
        {
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.GetFileAsync("Common.csv");
                string[] lines = (await FileIO.ReadTextAsync(file)).Split([',']);
                foreach (var line in lines)
                {
                    Debug.WriteLine($"Value: {line}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error reading Common.csv: " + ex.Message);
            }
        }
    }
}
