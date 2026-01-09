using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using Windows.Storage;
using Syroot.Windows.IO;
using System.IO;
using System.IO.Compression;
using Windows.Management.Deployment;

namespace SystemMonitorUWP.Code
{
    public class Updater
    {
        private static readonly Updater _instance = new Updater();
        public static Updater Instance => _instance;

        public bool updateAvailable = false;
        public bool isCheckingForUpdate = false;
        public string latestVersion = "0.0.0";
        public string downloadLink = "";
        public string releaseNotes = "";
        public string releaseDate = "";
        public string currentVersion = PackageVersionHelper.ToFormattedString(Package.Current.Id.Version);
        public string UpdateURL;
        public string UpdateFilePath = "";
        public StorageFile UpdateFile { get; set; }


        public async Task Check_Update()
        {
            isCheckingForUpdate = true;
            HttpClient httpClient = new();
            httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SystemMonitorUWP-Updater");
            Uri requestUri = new("https://api.github.com/repos/Emil215p/SystemMonitorUWP/releases/latest");
            _ = new HttpResponseMessage();
            string httpResponseBody;
            try
            {
                HttpResponseMessage httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
                Debug.WriteLine(httpResponse + httpResponseBody);

                using (JsonDocument doc = JsonDocument.Parse(httpResponseBody))
                {
                    var root = doc.RootElement;
                    string title = root.GetProperty("name").GetString();
                    string description = root.GetProperty("body").GetString();
                    string publishedAt = root.GetProperty("created_at").GetString();
                    string tagName = root.GetProperty("tag_name").GetString();
                    string zipUrl = "";
                    foreach (var asset in root.GetProperty("assets").EnumerateArray())
                    {
                        string assetName = asset.GetProperty("name").GetString();
                        if (assetName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                        {
                            zipUrl = asset.GetProperty("browser_download_url").GetString();
                            break;
                        }
                    }

                    latestVersion = tagName;
                    releaseNotes = description;
                    releaseDate = publishedAt;
                    downloadLink = zipUrl;
                    UpdateURL = zipUrl;

                    Version latest = new Version(latestVersion);
                    Version current = new Version(currentVersion);
                    updateAvailable = latest > current;


                    Debug.WriteLine($"Title: {title}\nDescription: {description}\nZip: {zipUrl}\nDate: {publishedAt}\nUpdate available: {updateAvailable}\nLatest update: {latestVersion}\nCurrent update: {currentVersion}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message);
            }
            finally
            {
                isCheckingForUpdate = false;
            }

            if (updateAvailable == true)
            {
                Debug.WriteLine("Update is available.");
                Download_Update();
            }
        }

        public async void Download_Update()
        {
            Debug.WriteLine("Downloading update from: " + UpdateURL);

            try
            {
                HttpClient httpClient = new();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SystemMonitorUWP-Updater");
                Uri requestUri = new(UpdateURL);
                _ = new HttpResponseMessage();
                string httpResponseBody;

                string fileName = "SystemMonitorUWP_" + latestVersion + ".zip";
                string downloadsPath = Syroot.Windows.IO.KnownFolders.Downloads.Path;
                StorageFile file = await DownloadsFolder.CreateFileAsync(fileName, CreationCollisionOption.GenerateUniqueName);

                using (var response = await httpClient.GetAsync(requestUri))
                {
                    response.EnsureSuccessStatusCode();
                    using (var inputStream = await response.Content.ReadAsStreamAsync())
                    using (var outputStream = await file.OpenStreamForWriteAsync())
                    {
                        await inputStream.CopyToAsync(outputStream);
                    }
                }

                UpdateFilePath = file.Path;
                UpdateFile = file;
                Debug.WriteLine("Update downloaded to: " + UpdateFilePath);
                Unzip_Update();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error downloading update: " + ex.HResult.ToString("X") + " Message: " + ex.Message);
            }
        }

        public async void Unzip_Update()
        {
            Debug.WriteLine("Unzipping...");

            try
            {
                using (var zipStream = await UpdateFile.OpenStreamForReadAsync())
                using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Read))
                {
                    StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                    Debug.WriteLine("Extracting to: " + localFolder.Path);
                    foreach (var entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name))
                            continue;

                        StorageFile outFile = await localFolder.CreateFileAsync(
                            entry.Name, CreationCollisionOption.ReplaceExisting);

                        using (var entryStream = entry.Open())
                        using (var outStream = await outFile.OpenStreamForWriteAsync())
                        {
                            await entryStream.CopyToAsync(outStream);
                        }
                    }
                }

                Debug.WriteLine("Unzipped successfully.");
                Install_Update();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error unzipping update archive: " + ex.HResult.ToString("X") + " Message: " + ex.Message);
            }
        }

        public async void Install_Update()
        {
            try
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                var files = await localFolder.GetFilesAsync();
                var msixFile = files.FirstOrDefault(f => f.FileType.Equals(".msixbundle", StringComparison.OrdinalIgnoreCase));

                if (msixFile == null)
                {
                    Debug.WriteLine("No .msixbundle found in LocalFolder.");
                    return;
                }

                string packagePath = msixFile.Path;
                var packageManager = new PackageManager();
                var deploymentResult = await packageManager.AddPackageAsync(
                    new Uri("file:///" + packagePath.Replace("\\", "/")),
                    null,
                    DeploymentOptions.None
                );

                Debug.WriteLine("Deployment result: " + deploymentResult.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error installing update package: " + ex.HResult.ToString("X") + " Message: " + ex.Message);
            }
        }

    }
}
