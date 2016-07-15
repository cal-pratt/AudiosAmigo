using System;
using System.Net;
using System.Net.Sockets;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AudiosAmigo
{
    public static class UdpUtil
    {
        public static IObservable<IPEndPoint> ReceivePortInfo(int port)
        {
            return Observable.Create<IPEndPoint>(
                subscriber =>
                {
                    UdpClient listener = null;
                    try
                    {
                        listener = new UdpClient(port);
                        for (;;)
                        {
                            var endpoint = new IPEndPoint(IPAddress.Any, port);
                            var bytes = listener.Receive(ref endpoint);
                            var i = BitConverter.ToInt32(bytes, 0);
                            Console.WriteLine("Received broadcast from {0}", endpoint.ToString());
                            subscriber.OnNext(new IPEndPoint(endpoint.Address, i));
                        }
                    }
                    catch (SocketException e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        listener?.Close();
                    }
                    return Disposable.Empty;
                }).Repeat();
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
