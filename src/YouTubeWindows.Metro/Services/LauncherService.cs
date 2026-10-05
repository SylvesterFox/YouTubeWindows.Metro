using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace YouTubeWindows.Metro.Services
{
    public sealed class LauncherService
    {
        private readonly SettingsService settings;
        public LauncherService(SettingsService settingsService) { settings = settingsService; }

        public string ResolveClientPath(Configuration configuration)
        {
            if (String.IsNullOrWhiteSpace(configuration.YouTubeWindowsPath)) throw new InvalidOperationException("YouTubeWindowsPath is empty. Open Settings and select YouTubeWindows.exe.");
            return Path.GetFullPath(Path.IsPathRooted(configuration.YouTubeWindowsPath) ? configuration.YouTubeWindowsPath : Path.Combine(settings.RootDirectory, configuration.YouTubeWindowsPath));
        }

        public Process Start(Configuration configuration)
        {
            var executable = ResolveClientPath(configuration);
            if (!File.Exists(executable)) throw new FileNotFoundException("YouTubeWindows.exe was not found. Place the TGSAN client in the YouTubeWindows folder or choose it in Settings.", executable);
            var start = new ProcessStartInfo { FileName = executable, Arguments = configuration.Arguments ?? "", WorkingDirectory = Path.GetDirectoryName(executable), UseShellExecute = false };
            var process = Process.Start(start);
            if (process == null) throw new InvalidOperationException("Windows did not create the YouTubeWindows process.");
            return process;
        }

        public void WriteLog(string message, string level)
        {
            var file = Path.Combine(settings.RootDirectory, "logs", "launcher.log");
            Directory.CreateDirectory(Path.GetDirectoryName(file));
            File.AppendAllText(file, DateTime.UtcNow.ToString("o") + " [" + (level ?? "Information") + "] " + message + Environment.NewLine, Encoding.UTF8);
        }
    }
}
