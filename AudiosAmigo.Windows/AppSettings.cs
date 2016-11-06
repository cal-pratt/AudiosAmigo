using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;
using AudiosAmigo.Windows.Properties;
using Microsoft.Win32;

namespace AudiosAmigo.Windows
{
    public static class AppSettings
    {
        public static string Ip
        {
            get
            {
                if (System.Net.NetworkInformation.NetworkInterface.GetIsNetworkAvailable())
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    var selectedIp = host.AddressList.FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                    if (selectedIp != null)
                    {
                        return selectedIp.ToString();
                    }
                }
                return "127.0.0.1";
            }
        }

        public static int Port
        {
            get
            {
                if (Settings.Default.SessionPort == 0)
                {
                    Port = Constants.DefaultServerTcpListenerPort;
                }
                return Settings.Default.SessionPort;
            }
            set
            {
                Settings.Default.SessionPort = value;
                Settings.Default.Save();
            }
        }

        public static string Password
        {
            get
            {
                if (Settings.Default.SessionPassword == "")
                {
                    Password = Constants.DefaultSessionPassword;
                }
                return Settings.Default.SessionPassword;
            }
            set
            {
                if (value == "")
                {
                    Password = Constants.DefaultSessionPassword;
                    return;
                }
                Settings.Default.SessionPassword = Translate.ByteArrayToBase64String(
                    PasswordUtil.PbkdfHash(
                        Translate.StringToByteArray(value),
                        Constants.PasswordSalt,
                        Constants.PasswordIterations,
                        Constants.PasswordLength));
                Settings.Default.Save();
            }
        }

        public static bool LaunchOnStartup
        {
            get { return Settings.Default.LaunchOnStartup; }
            set
            {
                Settings.Default.LaunchOnStartup = value;
                var key = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                if (key == null)
                {
                    return;
                }
                if (value)
                {
                    key.SetValue("AudiosAmigo", Application.ExecutablePath);
                }
                else
                {
                    key.DeleteValue("AudiosAmigo", false);
                }
                Settings.Default.Save();
            }
        }
    }
}
