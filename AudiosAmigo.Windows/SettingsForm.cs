using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace AudiosAmigo.Windows
{
    public partial class SettingsForm : Form
    {
        public event EventHandler SettingsApplied;

        private volatile bool _passwordLoaded = true;

        public SettingsForm()
        {
            InitializeComponent();
            ShowInTaskbar = false;
            Deactivate += (sender, args) => Hide();
            FormClosing += (sender, args) =>
            {
                Hide();
                args.Cancel = true;
            };
        }

        public new void Show()
        {
            var screen = Screen.FromPoint(Cursor.Position);
            StartPosition = FormStartPosition.Manual;
            Left = screen.WorkingArea.Left + screen.WorkingArea.Width - Width;
            Top = screen.WorkingArea.Top + screen.WorkingArea.Height - Height;

            // Shameless hack.
            _passwordLoaded = true;
            passwordText.Text = "******";
            passwordText.Click += ResetPasswordText;

            portText.Text = AppSettings.Port.ToString();
            startupCheckBox.Checked = AppSettings.LaunchOnStartup;

            base.Show();
        }

        public void ResetPasswordText(object sender, EventArgs e)
        {
            _passwordLoaded = false;
            passwordText.Text = "";
            passwordText.Click -= ResetPasswordText;
        }

        private void applyButton_Click(object sender, EventArgs e)
        {
            if (!Regex.IsMatch(portText.Text, @"^\d+$"))
            {
                MessageBox.Show("Port value must be a number", "Unable to apply changes", 
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var port = int.Parse(portText.Text);
            if (port < 1024 || port > 65535)
            {
                MessageBox.Show("Port value must be in range 1024 to 65535!", "Unable to apply changes",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            AppSettings.Port = port;
            if (!_passwordLoaded)
            {
                AppSettings.Password = passwordText.Text;
            }
            AppSettings.LaunchOnStartup = startupCheckBox.Checked;
            Hide();
            OnSettingsApplied();
        }

        protected virtual void OnSettingsApplied()
        {
            SettingsApplied?.Invoke(this, EventArgs.Empty);
        }
    }
}
