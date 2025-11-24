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
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Core;

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
            if (Window.Current.Content is not Frame rootFrame)
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

                Debug.WriteLine(RuntimeInformation.OSArchitecture.ToString());
                if (RuntimeInformation.OSArchitecture.ToString() != "Arm")
                {
                    try
                    {
                        Debug.WriteLine("Launching full trust process...");
                        await FullTrustProcessLauncher.LaunchFullTrustProcessForCurrentAppAsync();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to launch console app: {ex.Message}");
                    }
                }
                else if (RuntimeInformation.OSArchitecture.ToString() == "Arm")
                {
                    try
                    {
                        Debug.WriteLine("Launching console app via processlauncher...");
                        await RunProcess();
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine($"Failed to launch console app.: {ex.Message}");
                    }
                }
                else
                {
                    Debug.WriteLine("Error.");
                }

                Window.Current.Activate();
            }
        }

        private async Task RunProcess()
        {
            var options = new ProcessLauncherOptions();
            var standardOutput = new InMemoryRandomAccessStream();
            var standardError = new InMemoryRandomAccessStream();
            options.StandardOutput = standardOutput;
            options.StandardError = standardError;

            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    var result = await ProcessLauncher.RunToCompletionAsync("SystemMonitorUWP.Console.exe", "", options);

                    Debug.WriteLine("Process Exit Code: " + result.ExitCode);

                    using (var outStreamRedirect = standardOutput.GetInputStreamAt(0))
                    {
                        var size = standardOutput.Size;
                        using var dataReader = new DataReader(outStreamRedirect);
                        var bytesLoaded = await dataReader.LoadAsync((uint)size);
                        var stringRead = dataReader.ReadString(bytesLoaded);
                        Debug.WriteLine(stringRead);
                    }

                    using var errStreamRedirect = standardError.GetInputStreamAt(0);
                    using (var dataReader = new DataReader(errStreamRedirect))
                    {
                        var size = standardError.Size;
                        var bytesLoaded = await dataReader.LoadAsync((uint)size);
                        var stringRead = dataReader.ReadString(bytesLoaded);
                        Debug.WriteLine(stringRead);
                    }
                }
                catch (UnauthorizedAccessException uex)
                {
                    Debug.WriteLine("Exception Thrown: " + uex.Message + "\n");
                    Debug.Write("\nMake sure you're allowed to run the specified exe; either\n" +
                                         "\t1) Add the exe to the AppX package, or\n" +
                                         "\t2) Add the absolute path of the exe to the allow list:\n" +
                                         "\t\tHKLM\\SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\EmbeddedMode\\ProcessLauncherAllowedExecutableFilesList.\n\n" +
                                         "Also, make sure the <iot:Capability Name=\"systemManagement\" /> has been added to the AppX manifest capabilities.\n");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception Thrown:" + ex.Message + "\n");
                    Debug.WriteLine(ex.StackTrace + "\n");
                }
            });
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
