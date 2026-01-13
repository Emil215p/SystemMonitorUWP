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
using System.ServiceModel;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SystemMonitorUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
            Full_Update();
            CurrentVersionText.Text = "Current Version: " + Updater.Instance.currentVersion;
        }

        public async void Full_Update()
        {
            Debug.WriteLine(Updater.Instance.AutoUpdateEnabled.ToString());
            if (Updater.Instance.AutoUpdateEnabled == false)
            {
                Debug.WriteLine("Auto update is disabled.");
                UpdateStatusText.Text = "Auto update is disabled.";
                return;
            }
            if (Updater.Instance.isNetworkConnected == true)
            {
                Debug.WriteLine("Checking for updates...");
                UpdateStatusText.Text = "Checking for updates...";
                await Updater.Instance.Check_Update();
                if (Updater.Instance.updateAvailable == false) 
                { 
                    UpdateStatusText.Text = "No updates available.";
                    return;
                }
            }
            else
            {
                Debug.WriteLine("No internet connection. Cannot check for updates.");
                UpdateStatusText.Text = "No internet connection. Cannot check for updates.";
            }

            if (Updater.Instance.updateAvailable == true)
            {
                Updater.Instance.updateAvailable = false;
                UpdateStatusText.Text = "Update available: " + Updater.Instance.latestVersion + " Downloading...";
                Debug.WriteLine("Update is downloading...");
                await Updater.Instance.Download_Update();
            }
            else
            {
                Debug.WriteLine("No updates available.");
                return;
            }
            if (Updater.Instance.isUpdateDownloaded == true)
            {
                Updater.Instance.isUpdateDownloaded = false;
                UpdateStatusText.Text = "Update downloaded. Unzipping...";
                Debug.WriteLine("Update is unzipping...");
                await Updater.Instance.Unzip_Update();
            }
            if (Updater.Instance.isUpdateUnzipped == true)
            {
                Updater.Instance.isUpdateUnzipped = false;
                UpdateStatusText.Text = "Update is installing... App will restart soon.";
                Debug.WriteLine("Update is installing...");
                await Updater.Instance.Install_Update();
            }
        }

        public async Task Update_Check_Manual()
        {
            Update_Button.IsEnabled = false;
            Debug.WriteLine("Manual update check initiated.");

            if (Updater.Instance.isNetworkConnected == false)
            {
                Debug.WriteLine("No internet connection. Cannot check for updates.");
                UpdateStatusText.Text = "No internet connection. Cannot check for updates.";
                return;
            }
            UpdateStatusText.Text = "Checking for updates...";
            await Updater.Instance.Check_Update();

            if (Updater.Instance.updateAvailable == false)
            {
                Debug.WriteLine("No updates available.");
                UpdateStatusText.Text = "No updates available.";
                return;
            }
            UpdateStatusText.Text = "Update available.";
            Update_Button.IsEnabled = true;
            Update_Button.Content = "Download Update";

            Updater.Instance.Download_Update();
        }

        private void Updates_Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clicked Update.");
        }

        private void Changelog_Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("Clicked Changelog.");
        }
    }
}
