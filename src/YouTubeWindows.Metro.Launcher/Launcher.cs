using System;
using System.Diagnostics;
using System.Windows.Forms;
using YouTubeWindows.Metro.Services;

namespace YouTubeWindows.Metro.Launcher
{
    internal static class Launcher
    {
        public static int Run()
        {
            var root = AppDomain.CurrentDomain.BaseDirectory;
            var settings = new SettingsService(root);
            var service = new LauncherService(settings);
            var config = Configuration.Load(root);
            service.WriteLog("Launcher invoked", config.LogLevel);
            try
            {
                using (Process process = service.Start(config))
                {
                    process.WaitForExit();
                    return process.ExitCode;
                }
            }
            catch (Exception ex) { service.WriteLog(ex.ToString(), "Error"); throw; }
        }
    }
}
