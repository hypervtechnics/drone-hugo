using CliWrap;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Drone.Hugo
{
    static class Program
    {
        static async Task Main(string[] args)
        {
            if (args != null && args.Length == 2 && args[0] == "download") // download x.y.z
            {
                var downloader = new HugoDownloader();
                await downloader.Download(args[1], GetLocalHugoPath());

                return; // Exit as this may be executed as part of a docker build
            }

            // This is the part for drone
            var config = Config.Load();
            var hugoPath = await HandleHugoBinary(config);

            if (config.Validate)
            {
                await DoValidation(config, hugoPath);
            }

            await DoBuild(config, hugoPath);
        }

        private static async Task<string> HandleHugoBinary(Config config)
        {
            // Take the local one if there is one - no matter the version
            var localHugoPath = GetLocalHugoPath();
            if (File.Exists(localHugoPath))
            {
                return localHugoPath;
            }

            // If there is no local binary then download the latest or the given one by the user
            var downloader = new HugoDownloader();
            return await downloader.Download(config.HugoVersion);
        }

        private static async Task DoValidation(Config config, string hugoPath)
        {
            Console.WriteLine("Validating");

            var cli = Cli.Wrap(hugoPath)
                .SetStandardOutputCallback(Console.WriteLine)
                .SetStandardErrorCallback(Console.Error.WriteLine);
            var arguments = new List<string>();
            arguments.Add("check");

            // Configuration file
            if (!string.IsNullOrEmpty(config.ConfigurationFile))
            {
                arguments.Add("--config");
                arguments.Add(config.ConfigurationFile);
            }

            cli.SetArguments(arguments);
            var result = await cli.ExecuteAsync();
            Console.WriteLine($"Validation finished in {result.RunTime.TotalMilliseconds} ms");
        }

        private static async Task DoBuild(Config config, string hugoPath)
        {
            Console.WriteLine("Building");

            var cli = Cli.Wrap(hugoPath)
                .SetStandardOutputCallback(Console.WriteLine)
                .SetStandardErrorCallback(Console.Error.WriteLine);
            var arguments = new List<string>();

            // Base URL
            if (!string.IsNullOrEmpty(config.BaseUrl))
            {
                arguments.Add("--baseURL");
                arguments.Add(config.BaseUrl);
            }

            // Theme
            if (!string.IsNullOrEmpty(config.Theme))
            {
                arguments.Add("--theme");
                arguments.Add(config.Theme);
            }

            // Build drafts
            if (config.BuildDrafts)
            {
                arguments.Add("--buildDrafts");
            }

            // Build future
            if (config.BuildFuture)
            {
                arguments.Add("--buildFuture");
            }

            // Build expired
            if (config.BuildExpired)
            {
                arguments.Add("--buildExpired");
            }

            // Minify
            if (config.Minify)
            {
                arguments.Add("--minify");
            }

            // Configuration file
            if (!string.IsNullOrEmpty(config.ConfigurationFile))
            {
                arguments.Add("--config");
                arguments.Add(config.ConfigurationFile);
            }

            // Cache directory
            if (!string.IsNullOrEmpty(config.CacheDirectory))
            {
                arguments.Add("--cacheDir");
                arguments.Add(config.CacheDirectory);
            }

            // Content directory
            if (!string.IsNullOrEmpty(config.ContentDirectory))
            {
                arguments.Add("--contentDir");
                arguments.Add(config.ContentDirectory);
            }

            // Layout directory
            if (!string.IsNullOrEmpty(config.LayoutDirectory))
            {
                arguments.Add("--layoutDir");
                arguments.Add(config.LayoutDirectory);
            }

            // Source directory
            if (!string.IsNullOrEmpty(config.SourceDirectory))
            {
                arguments.Add("--source");
                arguments.Add(config.SourceDirectory);
            }

            // Output directory
            if (!string.IsNullOrEmpty(config.OutputDirectory))
            {
                arguments.Add("--destination");
                arguments.Add(config.OutputDirectory);
            }

            cli.SetArguments(arguments);
            var result = await cli.ExecuteAsync();
            Console.WriteLine($"Build finished in {result.RunTime.TotalMilliseconds} ms");
        }

        private static string GetLocalHugoPath()
        {
            var applicationPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            return Path.Combine(applicationPath, "assets", "hugo");
        }
    }
}
