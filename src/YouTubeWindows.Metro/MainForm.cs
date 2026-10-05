using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YouTubeWindows.Metro.Services;

namespace YouTubeWindows.Metro
{
    public sealed class MainForm : Form
    {
        private readonly SettingsService settings;
        private readonly LauncherService launcher;
        private Configuration configuration;
        private readonly Label status;
        private readonly Button start;
        private readonly Button settingsButton;
        private readonly Button exitButton;

        public MainForm()
        {
            settings = new SettingsService(AppDomain.CurrentDomain.BaseDirectory);
            launcher = new LauncherService(settings);
            try { configuration = settings.Load(); }
            catch (Exception ex) { configuration = Configuration.CreateDefault(); MessageBox.Show(ex.Message, "Settings error", MessageBoxButtons.OK, MessageBoxIcon.Warning); }
            Text = "YouTube TV"; BackColor = Color.FromArgb(24, 24, 24); ForeColor = Color.White; StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = configuration.Fullscreen ? FormBorderStyle.None : FormBorderStyle.Sizable; WindowState = configuration.Fullscreen ? FormWindowState.Maximized : FormWindowState.Normal;
            KeyPreview = true; MinimumSize = new Size(640, 400);
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, Padding = new Padding(48), BackColor = BackColor };
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 23)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 27)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 18)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 16)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 16));
            var logo = new Label { Text = "▶", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.FromArgb(238, 0, 0), Font = new Font("Segoe UI", 50, FontStyle.Bold) };
            var title = new Label { Text = "YouTube TV", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.White, Font = new Font("Segoe UI Light", 34) };
            status = new Label { Text = "Ready to start", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gainsboro, Font = new Font("Segoe UI", 18) };
            start = MakeButton("Start", Color.FromArgb(238, 0, 0)); start.Click += StartClient;
            var buttons = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, Anchor = AnchorStyles.None };
            settingsButton = MakeButton("Settings", Color.FromArgb(65, 65, 65)); settingsButton.Click += OpenSettings;
            exitButton = MakeButton("Exit", Color.FromArgb(65, 65, 65)); exitButton.Click += delegate { Close(); };
            buttons.Controls.Add(start); buttons.Controls.Add(settingsButton); buttons.Controls.Add(exitButton);
            layout.Controls.Add(logo, 0, 0); layout.Controls.Add(title, 0, 1); layout.Controls.Add(status, 0, 2); layout.Controls.Add(buttons, 0, 3);
            var hint = new Label { Text = "Use arrow keys and Enter · Esc to exit", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, ForeColor = Color.Gray, Font = new Font("Segoe UI", 10) }; layout.Controls.Add(hint, 0, 4);
            Controls.Add(layout); KeyDown += HandleNavigation; AcceptButton = start;
        }

        private static Button MakeButton(string text, Color color) { return new Button { Text = text, Width = 190, Height = 62, Margin = new Padding(12), FlatStyle = FlatStyle.Flat, BackColor = color, ForeColor = Color.White, Font = new Font("Segoe UI", 16), TabStop = true }; }

        private void StartClient(object sender, EventArgs e)
        {
            try
            {
                status.Text = "Starting YouTube TV…";
                var clientPath = launcher.ResolveClientPath(configuration);
                if (!File.Exists(clientPath)) throw new FileNotFoundException("YouTubeWindows.exe was not found. Place the TGSAN client in the YouTubeWindows folder or choose it in Settings.", clientPath);
                launcher.WriteLog("Starting launcher for " + clientPath, configuration.LogLevel);
                var helperPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "YouTubeWindows.Metro.Launcher.exe");
                if (!File.Exists(helperPath)) throw new FileNotFoundException("The launcher component is missing from this installation.", helperPath);
                var process = Process.Start(new ProcessStartInfo { FileName = helperPath, WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory, UseShellExecute = false });
                if (configuration.CloseLauncherAfterStart)
                {
                    Hide(); process.EnableRaisingEvents = true;
                    process.Exited += delegate { if (!IsDisposed) BeginInvoke((Action)Close); };
                    if (process.HasExited && !IsDisposed) BeginInvoke((Action)Close);
                }
                else status.Text = "YouTube TV is running. Press Esc to return here.";
            }
            catch (Exception ex) { status.Text = "Could not start YouTube TV"; launcher.WriteLog(ex.ToString(), "Error"); MessageBox.Show(ex.Message, "YouTube TV", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void OpenSettings(object sender, EventArgs e)
        {
            using (var dialog = new SettingsForm(configuration))
                if (dialog.ShowDialog(this) == DialogResult.OK) { configuration = dialog.Configuration; settings.Save(configuration); WindowState = configuration.Fullscreen ? FormWindowState.Maximized : FormWindowState.Normal; FormBorderStyle = configuration.Fullscreen ? FormBorderStyle.None : FormBorderStyle.Sizable; }
        }

        private void HandleNavigation(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) { Close(); e.Handled = true; e.SuppressKeyPress = true; }
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Up || e.KeyCode == Keys.Right || e.KeyCode == Keys.Down)
            {
                bool forward = e.KeyCode == Keys.Right || e.KeyCode == Keys.Down;
                SelectNextControl(ActiveControl, forward, true, true, true);
                e.Handled = true; e.SuppressKeyPress = true;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                var focusedButton = ActiveControl as Button;
                (focusedButton ?? start).PerformClick();
                e.Handled = true; e.SuppressKeyPress = true;
            }
        }
    }
}
