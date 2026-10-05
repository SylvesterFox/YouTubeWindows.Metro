namespace YouTubeWindows.Metro.Launcher
{
    internal static class Configuration
    {
        public static YouTubeWindows.Metro.Services.Configuration Load(string root)
        {
            return new YouTubeWindows.Metro.Services.SettingsService(root).Load();
        }
    }
}
