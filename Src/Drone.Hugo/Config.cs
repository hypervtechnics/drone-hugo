using System;

namespace Drone.Hugo
{
    public class Config
    {
        private Config()
        {
        }

        public static Config Load()
        {
            return new Config().FromEnvironment();
        }

        private Config FromEnvironment()
        {
            Validate = bool.TryParse(Environment.GetEnvironmentVariable("PLUGIN_VALIDATE"), out bool validate) ? validate : false;

            HugoVersion = Environment.GetEnvironmentVariable("PLUGIN_HUGO_VERSION") ?? "";

            BaseUrl = Environment.GetEnvironmentVariable("PLUGIN_URL") ?? "";
            Theme = Environment.GetEnvironmentVariable("PLUGIN_THEME") ?? "";

            BuildDrafts = bool.TryParse(Environment.GetEnvironmentVariable("PLUGIN_BUILDDRAFTS"), out bool drafts) ? drafts : false;
            BuildFuture = bool.TryParse(Environment.GetEnvironmentVariable("PLUGIN_BUILDFUTURE"), out bool future) ? future : false;
            BuildExpired = bool.TryParse(Environment.GetEnvironmentVariable("PLUGIN_BUILDEXPIRED"), out bool expired) ? expired : false;
            Minify = bool.TryParse(Environment.GetEnvironmentVariable("PLUGIN_MINIFY"), out bool minify) ? minify : false;

            ConfigurationFile = Environment.GetEnvironmentVariable("PLUGIN_CONFIG") ?? "";
            CacheDirectory = Environment.GetEnvironmentVariable("PLUGIN_CACHE") ?? "";
            ContentDirectory = Environment.GetEnvironmentVariable("PLUGIN_CONTENT") ?? "";
            LayoutDirectory = Environment.GetEnvironmentVariable("PLUGIN_LAYOUT") ?? "";
            SourceDirectory = Environment.GetEnvironmentVariable("PLUGIN_SOURCE") ?? "";
            OutputDirectory = Environment.GetEnvironmentVariable("PLUGIN_OUTPUT") ?? "";

            return this;
        }

        public bool Validate { get; set; }

        public string HugoVersion { get; set; }

        public string BaseUrl { get; set; }
        public string Theme { get; set; }

        public bool BuildDrafts { get; set; }
        public bool BuildFuture { get; set; }
        public bool BuildExpired { get; set; }
        public bool Minify { get; set; }

        public string ConfigurationFile { get; set; }
        public string CacheDirectory { get; set; }
        public string ContentDirectory { get; set; }
        public string LayoutDirectory { get; set; }
        public string SourceDirectory { get; set; }
        public string OutputDirectory { get; set; }
    }
}
