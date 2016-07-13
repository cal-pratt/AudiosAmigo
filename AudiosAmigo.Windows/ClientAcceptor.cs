using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AudiosAmigo.Windows
{
    public static class ClientAcceptor
    {
        public static IObservable<TcpClient> From(int tcpPort)
        {
            var serverSocket = new TcpListener(IPAddress.Any, tcpPort);
            var counter = 0;
            serverSocket.Start();

            return Observable.Create<TcpClient>(
                observer =>
                {
                    var client = serverSocket.AcceptTcpClient();
                    Console.WriteLine($"!! Client {client} No: {counter++} started !!");
                    observer.OnNext(client);
                    observer.OnCompleted();
                    return Disposable.Empty;
                }).Repeat();
        }
    }
}
