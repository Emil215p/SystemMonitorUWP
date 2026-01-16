using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI.Core;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

namespace SystemMonitorUWP.Code
{
    public class Shared
    {
        private static readonly Shared _instance = new();
        public static Shared Instance => _instance;
        public string filePath;

        private Shared()
        {
    
        }

        public async Task FullTrustLauncher(LaunchActivatedEventArgs e)
        {
            if (Window.Current.Content is not Frame rootFrame)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e != null && e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    
                }

                Window.Current.Content = rootFrame;
            }

            if (e != null)
            {
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
            else
            {
                Debug.WriteLine("LaunchActivatedEventArgs is null. Skipping navigation and argument-dependent logic.");

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


        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        public async Task RunProcess()
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
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception Thrown:" + ex.Message + "\n");
                    Debug.WriteLine(ex.StackTrace + "\n");
                }
            });
        }

    }
}