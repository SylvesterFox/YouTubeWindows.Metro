using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace YouTubeWindows.Metro.Services
{
    [DataContract]
    public sealed class Configuration
    {
        [DataMember] public string YouTubeWindowsPath { get; set; }
        [DataMember] public string Arguments { get; set; }
        [DataMember] public bool Fullscreen { get; set; }
        [DataMember] public bool CloseLauncherAfterStart { get; set; }
        [DataMember] public string LogLevel { get; set; }

        public static Configuration CreateDefault()
        {
            return new Configuration { YouTubeWindowsPath = @"YouTubeWindows\YouTubeWindows.exe", Arguments = "", Fullscreen = true, CloseLauncherAfterStart = true, LogLevel = "Information" };
        }
    }

    public sealed class SettingsService
    {
        public string RootDirectory { get; private set; }
        public string ConfigPath { get { return Path.Combine(RootDirectory, "config.json"); } }

        public SettingsService(string rootDirectory) { RootDirectory = rootDirectory; }

        public Configuration Load()
        {
            if (!File.Exists(ConfigPath)) return Configuration.CreateDefault();
            try
            {
                using (var stream = File.OpenRead(ConfigPath))
                    return (Configuration)new DataContractJsonSerializer(typeof(Configuration)).ReadObject(stream) ?? Configuration.CreateDefault();
            }
            catch (Exception ex) { throw new InvalidDataException("Could not read config.json: " + ex.Message, ex); }
        }

        public void Save(Configuration configuration)
        {
            Directory.CreateDirectory(RootDirectory);
            using (var stream = File.Create(ConfigPath))
                new DataContractJsonSerializer(typeof(Configuration)).WriteObject(stream, configuration);
        }
    }
}
