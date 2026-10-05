using System;
using System.IO;
using YouTubeWindows.Metro.Services;

internal static class Program
{
    private static int Main()
    {
        var root = Path.Combine(Path.GetTempPath(), "YouTubeWindows.Metro.Tests-" + Guid.NewGuid().ToString("N"));
        try
        {
            Directory.CreateDirectory(root);
            var service = new SettingsService(root);
            Assert(typeof(SettingsService).Assembly.GetName().Version.ToString() == "1.0.0.0", "assembly version comes from VERSION");
            var config = service.Load();
            Assert(config.Fullscreen, "default fullscreen setting");
            Assert(config.CloseLauncherAfterStart, "default close-after-start setting");
            config.Arguments = "--allow-auto-hdr --proxy-server=\"http://localhost:8080\"";
            service.Save(config);
            var loaded = service.Load();
            Assert(loaded.Arguments == config.Arguments, "argument string round trip");
            var clientPath = Path.Combine(root, "YouTubeWindows", "YouTubeWindows.exe");
            Directory.CreateDirectory(Path.GetDirectoryName(clientPath));
            File.WriteAllText(clientPath, "test stub");
            Assert(InstallationService.ClientExists(root, loaded), "relative client path detection");
            File.Delete(clientPath);
            Assert(!InstallationService.ClientExists(root, loaded), "missing client detection");
            Console.WriteLine("PASS: defaults, JSON configuration, argument preservation, relative path resolution, client present/missing detection");
            return 0;
        }
        catch (Exception ex) { Console.Error.WriteLine(ex); return 1; }
        finally { if (Directory.Exists(root)) Directory.Delete(root, true); }
    }

    private static void Assert(bool condition, string name)
    {
        if (!condition) throw new InvalidOperationException("Failed: " + name);
    }
}
