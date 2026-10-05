using System;
using System.Windows.Forms;

namespace YouTubeWindows.Metro
{
    internal static class App
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
