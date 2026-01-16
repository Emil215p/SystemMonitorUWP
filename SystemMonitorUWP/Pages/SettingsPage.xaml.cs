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
            this.InitializeComponent();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (localSettings.Values["AppTheme"] == null)
            {
                localSettings.Values["AppTheme"] = "System Default";
            }
            if (localSettings.Values["Auto_Update"] == null)
            {
                localSettings.Values["Auto_Update"] = false;
            }

            if (localSettings.Values["Auto_Update"] is bool autoUpdate && autoUpdate == true)
            {
                Check_Updates_Switch.IsOn = true;
            }
            if (localSettings.Values["AppTheme"].ToString() == "Light")
            {
                ThemeSelector.SelectedIndex = 0;
            }
            else if (localSettings.Values["AppTheme"].ToString() == "Dark")
            {
                ThemeSelector.SelectedIndex = 1;
            }
            else
            {
                ThemeSelector.SelectedIndex = 2;
            }
        }
        public string DevFam = AnalyticsInfo.VersionInfo.DeviceFamily;

        private void Update_Toggled(object sender, RoutedEventArgs e)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (sender is ToggleSwitch toggleSwitch)
            {
                localSettings.Values["Auto_Update"] = toggleSwitch.IsOn;
                Updater.Instance.AutoUpdateEnabled = toggleSwitch.IsOn;
                Debug.WriteLine($"Auto update toggled {(toggleSwitch.IsOn ? "on" : "off")}");
            }
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = (ThemeSelector.SelectedItem as ComboBoxItem)?.Content.ToString();
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (selected == "Light")
            {
                Debug.WriteLine("Light theme enabled");
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Light;
                localSettings.Values["AppTheme"] = "Light";
            }
            else if (selected == "Dark")
            {
                Debug.WriteLine("Dark theme enabled");
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Dark;
                localSettings.Values["AppTheme"] = "Dark";
            }
            else if (selected == "System Default")
            {
                Debug.WriteLine("System default theme enabled");
                ((Frame)Window.Current.Content).RequestedTheme = ElementTheme.Default;
                localSettings.Values["AppTheme"] = "System Default";
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
