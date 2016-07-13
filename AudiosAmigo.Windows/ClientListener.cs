using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AudiosAmigo.Windows
{
    public class ClientListener
    {
        public static IObservable<IPEndPoint> UdpPoller(UdpClient client, int udpPort)
        {
            return Observable.Create<IPEndPoint>(
                subscriber =>
                {
                    try
                    {
                        var endpoint = new IPEndPoint(IPAddress.Any, udpPort);
                        client.Receive(ref endpoint);
                        Console.WriteLine($"Received broadcast from {endpoint}");
                        subscriber.OnNext(endpoint);
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.Message);
                        client?.Close();
                        throw;
                    }
                    subscriber.OnCompleted();
                    return Disposable.Empty;
                }).Repeat();
        }

        public static Action<IPEndPoint> PortReporter(int serverPort)
        {
            return endpoint =>
            {
                var sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                try
                {
                    var sizeBytes = BitConverter.GetBytes(serverPort);
                    sender.SendTo(sizeBytes, new IPEndPoint(endpoint.Address, endpoint.Port));
                }
                catch (FormatException e)
                {
                    Console.WriteLine(e.Message);
                }
                catch (SocketException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    sender.Close();
                }
            };
        }
    }
}
