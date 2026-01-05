using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using System.Diagnostics;

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

        public void Check_Update()
        {

        }

    }
}
