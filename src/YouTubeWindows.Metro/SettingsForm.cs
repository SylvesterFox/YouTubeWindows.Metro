using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YouTubeWindows.Metro.Services;

namespace YouTubeWindows.Metro
{
    public sealed class SettingsForm : Form
    {
        private readonly TextBox path;
        private readonly TextBox arguments;
        private readonly CheckBox fullscreen;
        private readonly CheckBox closeAfterStart;
        private readonly TextBox logLevel;
        public Configuration Configuration { get; private set; }

        public SettingsForm(Configuration configuration)
        {
            Configuration = configuration;
            Text = "YouTube TV Settings"; StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog; MinimizeBox = false; MaximizeBox = false; ClientSize = new Size(680, 340); BackColor = Color.FromArgb(32, 32, 32); ForeColor = Color.White;
            var grid = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(24), ColumnCount = 3, RowCount = 6 }; grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190)); grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); grid.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90));
            path = new TextBox { Text = configuration.YouTubeWindowsPath ?? "", Dock = DockStyle.Fill }; arguments = new TextBox { Text = configuration.Arguments ?? "", Dock = DockStyle.Fill };
            fullscreen = new CheckBox { Text = "Fullscreen launcher window", Checked = configuration.Fullscreen, AutoSize = true };
            closeAfterStart = new CheckBox { Text = "Close launcher while client is running", Checked = configuration.CloseLauncherAfterStart, AutoSize = true };
            logLevel = new TextBox { Text = configuration.LogLevel ?? "Information", Dock = DockStyle.Left, Width = 140 };
            AddRow(grid, 0, "YouTubeWindows.exe", path, "Browse…", Browse); AddRow(grid, 1, "Arguments", arguments, "", null); grid.Controls.Add(fullscreen, 1, 2); grid.Controls.Add(closeAfterStart, 1, 3); grid.Controls.Add(new Label { Text = "Log level", AutoSize = true, ForeColor = ForeColor, Anchor = AnchorStyles.Left }, 0, 4); grid.Controls.Add(logLevel, 1, 4);
            var save = new Button { Text = "Save", DialogResult = DialogResult.OK, Width = 110, Height = 42 }; save.Click += SaveConfiguration;
            var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Width = 110, Height = 42 };
            var actions = new FlowLayoutPanel { FlowDirection = FlowDirection.RightToLeft, Dock = DockStyle.Fill }; actions.Controls.Add(cancel); actions.Controls.Add(save); grid.Controls.Add(actions, 1, 5); Controls.Add(grid); AcceptButton = save; CancelButton = cancel;
        }

        private static void AddRow(TableLayoutPanel grid, int row, string label, Control input, string buttonText, EventHandler action)
        {
            grid.Controls.Add(new Label { Text = label, AutoSize = true, ForeColor = Color.White, Anchor = AnchorStyles.Left }, 0, row); grid.Controls.Add(input, 1, row);
            if (!String.IsNullOrEmpty(buttonText)) { var button = new Button { Text = buttonText, Dock = DockStyle.Fill }; button.Click += action; grid.Controls.Add(button, 2, row); }
        }

        private void Browse(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog { Filter = "YouTubeWindows.exe|YouTubeWindows.exe|Executable files|*.exe", FileName = "YouTubeWindows.exe", CheckFileExists = true })
                if (dialog.ShowDialog(this) == DialogResult.OK) path.Text = dialog.FileName;
        }

        private void SaveConfiguration(object sender, EventArgs e)
        {
            Configuration = new Configuration { YouTubeWindowsPath = path.Text.Trim(), Arguments = arguments.Text, Fullscreen = fullscreen.Checked, CloseLauncherAfterStart = closeAfterStart.Checked, LogLevel = String.IsNullOrWhiteSpace(logLevel.Text) ? "Information" : logLevel.Text.Trim() };
        }
    }
}
