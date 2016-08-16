using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace AudiosAmigo
{
    public static class UdpUtil
    {
        public static IObservable<IPEndPoint> ReceivePortInfo(int port)
        {
            var running = true;
            var listener = new UdpClient(port);
            return Observable.Create<IPEndPoint>(
                subscriber =>
                {
                    Task.Run(async () =>
                    {
                        await Task.Yield();
                        while (running)
                        {
                            var endpoint = new IPEndPoint(IPAddress.Any, port);
                            var bytes = listener.Receive(ref endpoint);
                            Console.WriteLine($"Received broadcast from {endpoint}");
                            subscriber.OnNext(new IPEndPoint(endpoint.Address, BitConverter.ToInt32(bytes, 0)));
                        }
                        subscriber.OnCompleted();
                    });
                    return Disposable.Create(() =>
                    {
                        running = false;
                        listener.Close();
                    });
                });
        }

        public static Action<IPEndPoint> IntWriter(int i)
        {
            return endpoint =>
            {
                var sender = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp)
                {
                    EnableBroadcast = true
                };
                try
                {
                    var bytes = BitConverter.GetBytes(i);
                    sender.SendTo(bytes, endpoint);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                }
                finally
                {
                    sender.Close();
                }
            };
        }
    }
}
