using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AudiosAmigo.Windows
{
    public static class ClientAcceptor
    {
        public static IObservable<TcpClient> From(int tcpPort)
        {
            var serverSocket = new TcpListener(IPAddress.Any, tcpPort);
            serverSocket.Start();

            var counter = 0;
            var running = true;
            return Observable.Create<TcpClient>(observer =>
            {
                Task.Run(async () =>
                {
                    await Task.Yield();
                    while (running)
                    {
                        var client = serverSocket.AcceptTcpClient();
                        Console.WriteLine($"!! Client {client} No: {counter++} started !!");
                        observer.OnNext(client);
                    }
                    observer.OnCompleted();
                });
                return Disposable.Create(() =>
                {
                    running = false;
                    serverSocket.Stop();
                });
            });
        }
    }
}
