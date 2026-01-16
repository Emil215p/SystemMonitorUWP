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
using System.Diagnostics;
using SystemMonitorUWP.Code;
using Windows.Storage;
using Windows.System.Profile;
using Windows.UI;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace SystemMonitorUWP.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        public SettingsPage()
        {
            ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            this.InitializeComponent();
        }
        public string DevFam = AnalyticsInfo.VersionInfo.DeviceFamily;

        private void Update_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    Updater.Instance.AutoUpdateEnabled = true;
                    Debug.WriteLine("Auto update toggled on");
                }
                else if (toggleSwitch.IsOn == false)
                {
                    Updater.Instance.AutoUpdateEnabled = false;
                    Debug.WriteLine("Auto update toggled off");
                }
            }
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (ThemeSelector.SelectedItem as ComboBoxItem)?.Content.ToString();

            if (selected == "Light")
            {
                Debug.WriteLine("Light theme enabled");
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
            }
            else if (selected == "Dark")
            {
                Debug.WriteLine("Dark theme enabled");
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
            }
            else if (selected == "System Default")
            {
                Debug.WriteLine("System default theme enabled");
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Default;
            }
            else
            {
                Debug.WriteLine("Unknown theme");
            }
        }

        private void New_Monitor_Desktop_Switch_Toggled(object sender, RoutedEventArgs e)
        {
            if (sender is ToggleSwitch toggleSwitch)
            {
                if (toggleSwitch.IsOn == true)
                {
                    if (DevFam != "Windows.Desktop")
                    {
                        Debug.WriteLine("Unsupported option.");
                        toggleSwitch.IsOn = false;
                        return;
                    }
                    Debug.WriteLine("New monitor enabled");
                }
                else if (toggleSwitch.IsOn == false)
                {
                    Debug.WriteLine("New monitor is disabled.");
                }
            }
        }
    }
}
