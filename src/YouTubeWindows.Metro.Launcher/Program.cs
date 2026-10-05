using System;
using System.Windows.Forms;

namespace YouTubeWindows.Metro.Launcher
{
    internal static class Program
    {
        [STAThread]
        private static int Main()
        {
            try { return Launcher.Run(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "YouTube TV launcher", MessageBoxButtons.OK, MessageBoxIcon.Error); return 1; }
        }
    }
}
