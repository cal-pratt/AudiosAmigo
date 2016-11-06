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
            var subscriptions = new CompositeDisposable
            {
                Begin(AppSettings.Port, AppSettings.Password)
            };
            var systemTrayIcon = new SystemTrayIcon();
            var settingsForm = new SettingsForm();
            systemTrayIcon.ExitClicked += (sender, args) =>
            {
                subscriptions.Dispose();
                Application.Exit();
                Environment.Exit(0);
            };
            systemTrayIcon.SettingsClicked += (sender, args) =>
            {
                settingsForm.Show();
            };
            settingsForm.SettingsApplied += (sender, args) =>
            {
                subscriptions.Clear();
                subscriptions.Add(Begin(AppSettings.Port, AppSettings.Password));
            };
            Application.Run();
        }

        public static IDisposable Begin(int serverTcpListenerPort, string password)
        {
            var counter = 0;
            return new CompositeDisposable
            {
                UdpUtil.ReceivePortInfo(Constants.ServerBroadcastListenerPort)
                    .Subscribe(UdpUtil.IntWriter(serverTcpListenerPort)),

                ClientAcceptor.From(serverTcpListenerPort)
                    .Select(client => new SecureTcpClientCommunication(
                        new TcpClientCommunication(client), new Encrpytion(password, Constants.EncrpytionInitVector)))
                    .Scan((INetworkCommunication) null, (last, next) =>
                    {
                        last?.Close();
                        return next;
                    })
                    .Select(communication => new Communicator<Command>(communication, NewThreadScheduler.Default))
                    .Select(communicator =>
                    {
                        var handler = new AudioServer(new AudioController());
                        return new CompositeDisposable
                        {
                            communicator.SubscribeOn(NewThreadScheduler.Default).Subscribe(handler),
                            handler.SubscribeOn(NewThreadScheduler.Default).Subscribe(communicator)
                        };
                    })
                    .Scan((IDisposable) null, (last, next) =>
                    {
                        last?.Dispose();
                        return next;
                    })
                    .Subscribe(_ => Console.WriteLine($"Client No {counter++}, Created."))
            };
        }
    }
}
