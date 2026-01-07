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
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
                Debug.WriteLine(httpResponseBody);
            }
            isCheckingForUpdate = false;
        }

    }
}
