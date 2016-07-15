using System;
using System.Diagnostics.CodeAnalysis;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AudiosAmigo.Windows
{
    [SuppressMessage("ReSharper", "LocalizableElement")]
    public static class Program
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                UdpUtil.ReceivePortInfo(Constants.ServerBroadcastListenerPort)
                    .ObserveOn(NewThreadScheduler.Default)
                    .SubscribeOn(NewThreadScheduler.Default)
                    .Subscribe(UdpUtil.IntWriter(Constants.DefaultServerTcpListenerPort));

                var counter = 0;
                ClientAcceptor.From(Constants.DefaultServerTcpListenerPort)
                    .ObserveOn(NewThreadScheduler.Default).SubscribeOn(NewThreadScheduler.Default)
                    .Select(client => new TcpClientCommunication(client))
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
                    .Subscribe(_ => Console.WriteLine($"Client No {counter++}, Created."));
            }
            catch (Exception e)
            {
                Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
            }
            Console.WriteLine("Started");
        }
    }
}
