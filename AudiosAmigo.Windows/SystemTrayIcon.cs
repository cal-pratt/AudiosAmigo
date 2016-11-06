using System;
using System.Windows.Forms;
using AudiosAmigo.Windows.Properties;

namespace AudiosAmigo.Windows
{
    public class SystemTrayIcon
    {
        public event EventHandler SettingsClicked;

        public event EventHandler ExitClicked;

        public SystemTrayIcon()
        {
            var trayMenu = new ContextMenu();
            var trayIcon = new NotifyIcon
            {
                Text = "Audio's Amigo",
                Icon = Resources.AudioIcon,
                ContextMenu = trayMenu,
                Visible = true
            };
            var ipTextMenuItem = new MenuItem
            {
                Text = $"Audio's Amigo {AppSettings.Ip}:{AppSettings.Port}",
                Enabled = false
            };
            trayIcon.Click += (sender, args) =>
            {
                ipTextMenuItem.Text = $"Audio's Amigo {AppSettings.Ip}:{AppSettings.Port}";
            };
            trayMenu.MenuItems.Add(ipTextMenuItem);
            trayMenu.MenuItems.Add("Settings...", (sender, e) =>
            {
                OnSettingsClicked();
            });
            trayMenu.MenuItems.Add("Exit", (sender, e) =>
            {
                trayIcon.Visible = false;
                trayMenu.Dispose();
                OnExitClicked();
            });
        }

        protected virtual void OnSettingsClicked()
        {
            SettingsClicked?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnExitClicked()
        {
            ExitClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
