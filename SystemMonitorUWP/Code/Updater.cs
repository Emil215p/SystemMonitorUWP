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

                    
                    Debug.WriteLine($"Title: {title}\nDescription: {description}\nZip: {zipUrl}\nDate: {publishedAt}\nUpdate available: {updateAvailable}\nLatest update: {latestVersion}\nCurrent update: {currentVersion }");
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
        }

        public async void Download_Update()
        {
             if (updateAvailable == false)
            {
                Debug.WriteLine("No update available to download.");
                return;
            } 
            else
            {
                Debug.WriteLine("Downloading update from: " + UpdateURL);
            }
        }

    }
}
