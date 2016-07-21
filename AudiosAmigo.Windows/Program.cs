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
            var passHash = Translate.ByteArrayToBase64String(PasswordUtil.PbkdfHash(
                Translate.StringToByteArray(Constants.DefaultSessionPassword),
                Constants.PasswordSalt,
                Constants.PasswordIterations,
                Constants.PasswordLength));
            var subscriptions = Begin(Constants.DefaultServerTcpListenerPort, passHash);
            Application.Run();
        }

        public static IDisposable Begin(int serverTcpListenerPort, string password)
        {
            var counter = 0;
            return new CompositeDisposable
            {
                UdpUtil.ReceivePortInfo(Constants.ServerBroadcastListenerPort)
                    .ObserveOn(NewThreadScheduler.Default)
                    .SubscribeOn(NewThreadScheduler.Default)
                    .Subscribe(UdpUtil.IntWriter(serverTcpListenerPort)),

                ClientAcceptor.From(serverTcpListenerPort)
                    .ObserveOn(NewThreadScheduler.Default).SubscribeOn(NewThreadScheduler.Default)
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
                        var handler = new AudioServer();
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
