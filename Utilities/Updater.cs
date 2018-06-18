using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DICUI.Utilities
{
    class Version
    {
        private readonly string identifier;
        private readonly GitHubRelease release;

        public Version(string identifier) : this(identifier, null)
        {

        }

        public Version(string identifier, GitHubRelease release)
        {
            this.identifier = identifier;
            this.release = release;
        }

        public bool IsNewerThan(Version reference)
        {
            return reference.identifier != this.identifier;
        }

        public override string ToString() 
        {
            return identifier;
        }

    }

    class GitHubRelease
    {
        public string url;
        public string html_url;
        public string tag_name;
        public string zipball_url;
    }

    class Updater
    {
        private const string GITHUB_LAST_RELEASE_URL = "https://api.github.com/repos/reignstumble/DICUI/releases/latest";

        private static Version GetCurrentVersion()
        {
            return new Version("1.06");
        }

        private static Version GetLatestVersion()
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                var json = wc.DownloadString(GITHUB_LAST_RELEASE_URL);

                GitHubRelease latestRelease = JsonConvert.DeserializeObject<GitHubRelease>(json);

                Version latestVersion = new Version(latestRelease.tag_name);

                return latestVersion;
            }
        }

        private static Tuple<bool, Version, Version> CheckIfUpdateIsRequired()
        {
            Version current = GetCurrentVersion();
            Version latest = GetLatestVersion();

            return Tuple.Create(latest.IsNewerThan(current), current, latest);
        }

        public static void downloadLatestReleaseToTemporary(Version version)
        {
            string temporaryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        }

    }
}
