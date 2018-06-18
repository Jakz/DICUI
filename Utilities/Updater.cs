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

        public Version(string identifier) => this.identifier = identifier;

        public Version(GitHubRelease release)
        {
            this.identifier = release.tag_name;
            this.release = release;
        }

        public bool IsNewerThan(Version reference) => reference.identifier != this.identifier;
        public string URL() => release.assets[0].browser_download_url;
        public override string ToString() => identifier;
    }

    class GitHubRelease
    {
        public string url;
        public string html_url;
        public string tag_name;
        public List<Assets> assets;
        public string zipball_url;

        public class Assets
        {
            public string browser_download_url;
        }
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

                Version latestVersion = new Version(latestRelease);

                return latestVersion;
            }
        }

        private static Tuple<bool, Version, Version> CheckIfUpdateIsRequired()
        {
            Version current = GetCurrentVersion();
            Version latest = GetLatestVersion();

            return Tuple.Create(latest.IsNewerThan(current), current, latest);
        }

        private static string downloadLatestReleaseToTemporary(Version version)
        {
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                string temporaryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                wc.DownloadFile(version.URL(), temporaryPath);

                return temporaryPath;
            }
        }

        private static void extractDowloadedRelease(string archivePath)
        {
            string extractPath = @"C:\Users\Jack\Desktop\foo";

            System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, extractPath);
        }

        public static void test()
        {
            var v = CheckIfUpdateIsRequired();
            var p = downloadLatestReleaseToTemporary(v.Item3);
            extractDowloadedRelease(p);
        }

    }
}
