using Mono.Unix;
using Octokit;
using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Drone.Hugo
{
    public class HugoDownloader
    {
        private string baseDirectory;
        private string packageFilePath;
        private string packageDirectory;

        public HugoDownloader(string baseDirectory = "")
        {
            this.baseDirectory = string.IsNullOrEmpty(baseDirectory) ? GetTemporaryDirectory() : baseDirectory;

            this.packageFilePath = Path.Combine(this.baseDirectory, "package.tar.gz");
            this.packageDirectory = Path.Combine(this.baseDirectory, "package");
        }

        public async Task<string> Download(string givenVersion, string targetFile = "")
        {
            if (!Directory.Exists(baseDirectory))
            {
                Directory.CreateDirectory(baseDirectory);
            }

            var versionToDownload = givenVersion;

            if (string.IsNullOrEmpty(givenVersion))
            {
                Console.WriteLine("Getting latest Hugo version");
                versionToDownload = await GetLatestVersion();
            }

            if (versionToDownload.StartsWith("v"))
            {
                versionToDownload = versionToDownload.Substring(1);
            }

            await DownloadPackage(versionToDownload);
            var hugoBinaryPath = await ExtractPackage();

            if (!string.IsNullOrEmpty(targetFile))
            {
                Console.WriteLine($"Moving {hugoBinaryPath} to {targetFile}");
                File.Move(hugoBinaryPath, targetFile, true);

                // Clean up as this was probably during the docker build and we should save some MB's
                Console.WriteLine("Cleaning up temporary directories");
                File.Delete(packageFilePath);
                Directory.Delete(packageDirectory, true);
            }

            return hugoBinaryPath;
        }

        private async Task<string> GetLatestVersion()
        {
            var gitHubClient = new GitHubClient(new ProductHeaderValue("hypervtechnics-drone-hugo"));
            var latestRelease = await gitHubClient.Repository.Release.GetLatest("gohugoio", "hugo");
            return latestRelease.Name;
        }

        private async Task<string> DownloadPackage(string version)
        {
            string downloadUrl = $"https://github.com/gohugoio/hugo/releases/download/v{version}/hugo_extended_{version}_Linux-64bit.tar.gz";

            using (var webClient = new WebClient())
            {
                Console.WriteLine($"Downloading from {downloadUrl} to {packageFilePath}");
                await webClient.DownloadFileTaskAsync(downloadUrl, packageFilePath);
            }

            return packageFilePath;
        }

        private async Task<string> ExtractPackage()
        {
            Console.WriteLine($"Extracting package to {packageDirectory}");
            TarHelper.ExtractTarGz(packageFilePath, packageDirectory);

            var hugoBinaryPath = Path.Combine(packageDirectory, "hugo");
            var hugoFile = new FileInfo(hugoBinaryPath);

            if (!hugoFile.Exists)
            {
                throw new FileNotFoundException("The hugo binary could not be found. Maybe it was not correctly downloaded.", hugoBinaryPath);
            }
            else
            {
                Console.WriteLine($"Downloaded hugo binary to {hugoBinaryPath}");
            }

            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Console.WriteLine("Setting permissions on hugo binary");
                var hugoPerm = new UnixFileInfo(hugoBinaryPath);
                hugoPerm.FileAccessPermissions = FileAccessPermissions.UserReadWriteExecute | FileAccessPermissions.GroupRead | FileAccessPermissions.GroupWrite | FileAccessPermissions.OtherRead | FileAccessPermissions.OtherWrite;
            }

            return await Task.FromResult(hugoBinaryPath);
        }

        private string GetTemporaryDirectory()
        {
            string temporaryFile = Path.GetTempFileName();

            // Delete the temporary file and keep only the filename
            File.Delete(temporaryFile);

            return Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(temporaryFile));
        }
    }
}
