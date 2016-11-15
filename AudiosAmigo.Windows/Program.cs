using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace AudiosAmigo.Windows
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            var sessionManager = new NetworkSessionManager(AppSettings.Port, AppSettings.Password);
            var systemTrayIcon = new SystemTrayIcon();
            var settingsForm = new SettingsForm();
            systemTrayIcon.ExitClicked += (sender, args) =>
            {
                sessionManager.Dispose();
                Application.Exit();
                Environment.Exit(0);
            };
            systemTrayIcon.SettingsClicked += (sender, args) =>
            {
                settingsForm.Show();
            };
            settingsForm.SettingsApplied += (sender, args) =>
            {
                sessionManager.Refresh(AppSettings.Port, AppSettings.Password);
            };
            Application.Run();
        }
    }
}
