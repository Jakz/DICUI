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
        private class Utilities
        {
            public static WebClient CreateHTTPClient()
            {
                WebClient wc = new WebClient();
                // this is needed because GitHub requires an user agent
                wc.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                return wc;
            }

            public static string GetTemporaryPath() => Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

            public static void DownloadFile(string url, string destination)
            {
                using (WebClient wc = CreateHTTPClient())
                {
                    wc.DownloadFile(url, destination);
                }
            }
        }


        private const string DICUI_LAST_RELEASE_URL = "https://api.github.com/repos/reignstumble/DICUI/releases/latest";
        private const string DIC_LAST_RELEASE_URL = "https://api.github.com/repos/saramibreak/DiscImageCreator/releases/latest";

        private static Version GetCurrentDICUIVersion()
        {
            return new Version("1.06");
        }

        private static Version GetLatestDICUIVersion()
        {
            using (WebClient wc = Utilities.CreateHTTPClient())
            {
                var json = wc.DownloadString(DICUI_LAST_RELEASE_URL);

                GitHubRelease latestRelease = JsonConvert.DeserializeObject<GitHubRelease>(json);
                Version latestVersion = new Version(latestRelease);

                return latestVersion;
            }
        }

        private static Tuple<bool, Version, Version> CheckIfDICUIUpdateIsRequired()
        {
            Version current = GetCurrentDICUIVersion();
            Version latest = GetLatestDICUIVersion();

            return Tuple.Create(latest.IsNewerThan(current), current, latest);
        }

        private static string downloadLatestDICUIReleaseToTemporary(Version version)
        {
            string temporary = Utilities.GetTemporaryPath();
            Utilities.DownloadFile(version.URL(), temporary);
            return temporary;
        }

        private static void extractDowloadedRelease(string archivePath)
        {
            string extractPath = @"C:\Users\Jack\Desktop\foo";

            System.IO.Compression.ZipFile.ExtractToDirectory(archivePath, extractPath);
        }

        public static void test()
        {
            var v = CheckIfDICUIUpdateIsRequired();
            var p = downloadLatestDICUIReleaseToTemporary(v.Item3);
            extractDowloadedRelease(p);
        }

    }
}
