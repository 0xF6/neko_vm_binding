namespace nekoc
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Flurl.Http;
    using Konsole;
    using Newtonsoft.Json;

    public class GithubClient
    {
        private readonly string _owner;
        private readonly string _repo;
        private readonly string _version;

        public GithubClient(string owner, string repo, string version)
        {
            _owner = owner;
            _repo = repo;
            _version = version;
        }

        public string GetFile()
            => $"neko-{_version}-{AppState.GetOS()}64.{(AppState.GetOS() == "win" ? "zip" : "tar.gz")}";

        public async ValueTask<FileInfo> DownloadAsync()
        {
            var releases = await $"https://api.github.com/repos/{_owner}/{_repo}/releases"
                .WithHeader("User-Agent", $"NekoC .NET/{AppState.GetVersion()}")
                .GetJsonAsync<GithubRelease[]>();
            var release = releases.First();
            var targetFile = GetFile();

            var asset = release.Assets.FirstOrDefault(x => x.Name == targetFile);

            if (asset is null)
                throw new Exception($"Failed find {targetFile} in latest release in '{_owner}/{_repo}'");


            using var handler = HttpClientDownloadWithProgress.Create(asset.BrowserDownloadUrl,
                new FileInfo(Path.Combine(AppState.GetFolderForCache().FullName, targetFile)));
            Console.WriteLine($"{":page_with_curl:".Emoji()} Download {asset.BrowserDownloadUrl}..");
            var pb = new ProgressBar(100, 0, '=');

            handler.ProgressChanged += (size, downloaded, percentage) =>
            {
                if (percentage != null)
                    pb.Refresh((int)percentage.Value, $"");
            };

            await handler.StartDownload();


            Console.WriteLine();

            return new FileInfo(Path.Combine(AppState.GetFolderForCache().FullName, targetFile));
        }
    }
    public class GithubRelease
    {
        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("target_commitish")]
        public string TargetCommitish { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("published_at")]
        public DateTimeOffset PublishedAt { get; set; }

        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }
    }

    public class Asset
    {
        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("node_id")]
        public string NodeId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("content_type")]
        public string ContentType { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("size")]
        public long Size { get; set; }

        [JsonProperty("download_count")]
        public long DownloadCount { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
    }
}