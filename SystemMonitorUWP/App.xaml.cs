using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
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
using System.Runtime.InteropServices;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.System;
using System.Runtime.CompilerServices;

namespace SystemMonitorUWP
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
        }

        public string filePath;

        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame == null)
            {
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {

                }

                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }

                if (RuntimeInformation.OSArchitecture.ToString() != "Arm")
                {
                    try
                    {
                        Debug.WriteLine("Launching full trust process...");
                        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to launch console app: {ex.Message}");
                    }
                }
                else if (RuntimeInformation.OSArchitecture.ToString() == "Arm")
                {
                    try
                    {
                        Debug.WriteLine("Launching console app for UWP...");
                        IoTCoreLauncher();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to launch console app.: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine("Error.");
                }

                Window.Current.Activate();
            }
        }

       private string AllowExecuteRegString(string exePath)
        {
            if (string.IsNullOrWhiteSpace(exePath))
            {
                throw new ArgumentException("File path cannot be null or empty.", nameof(exePath));
            }
            string EnableCommandLineProcesserRegCommand = $"reg ADD \"HKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\EmbeddedMode\\ProcessLauncher\" /f /v AllowedExecutableFilesList /t REG_MULTI_SZ /d \"{exePath}\\0\"";
            Debug.WriteLine(EnableCommandLineProcesserRegCommand);
            filePath = exePath;
            return EnableCommandLineProcesserRegCommand;
        }

        private void WDPLogin()
        {
            string DefaultHostName = "127.0.0.1";
            string DefaultProtocol = "http";
            string DefaultPort = "8080";
            string DefaultUserName = "Administrator";
            string DefaultPassword = "p@ssw0rd";

            string WdpRunCommandApi = "/api/iot/processmanagement/runcommand";
            string WdpRunCommandWithOutputApi = "/api/iot/processmanagement/runcommandwithoutput";
        }

        private async void IoTCoreLauncher()
        {
            try
            {
                ProcessLauncherOptions options = new ProcessLauncherOptions
                {
                    StandardOutput = null,
                    StandardError = null
                };

                AllowExecuteRegString("C:\\Windows\\System32\\cmd.exe");

                string executablePath = filePath;
                string arguments = "/c echo Hello from IoTCoreLauncher";

                Debug.WriteLine($"Launching process: {executablePath} {arguments}");

                ProcessLauncherResult result = await ProcessLauncher.RunToCompletionAsync(executablePath, arguments, options);

                Debug.WriteLine($"Process exited with code: {result.ExitCode}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Failed to launch process: {ex.Message}");
            }
        }


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            deferral.Complete();
        }
    }
}
