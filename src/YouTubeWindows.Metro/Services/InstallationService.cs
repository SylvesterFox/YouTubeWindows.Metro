using System;
using System.IO;

namespace YouTubeWindows.Metro.Services
{
    public static class InstallationService
    {
        public static string GetClientPath(string root, Configuration configuration)
        {
            if (String.IsNullOrWhiteSpace(configuration.YouTubeWindowsPath)) return null;
            return Path.GetFullPath(Path.IsPathRooted(configuration.YouTubeWindowsPath) ? configuration.YouTubeWindowsPath : Path.Combine(root, configuration.YouTubeWindowsPath));
        }
        public static bool ClientExists(string root, Configuration configuration)
        {
            var path = GetClientPath(root, configuration);
            return path != null && File.Exists(path);
        }
    }
}
