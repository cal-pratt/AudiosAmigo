using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace AudiosAmigo.Windows
{
    public class NetworkSessionManager : IDisposable
    {
        private readonly CompositeDisposable _generalSubscriptions = new CompositeDisposable();

        private readonly CompositeDisposable _sessionSubscriptions = new CompositeDisposable();

        private int _counter;

        public NetworkSessionManager(int serverTcpListenerPort, string password)
        {
            Refresh(serverTcpListenerPort, password);
        }

        public void Refresh(int serverTcpListenerPort, string password)
        {
            _generalSubscriptions.Clear();
            _sessionSubscriptions.Clear();
            _generalSubscriptions.Add(UdpUtil.ReceivePortInfo(Constants.ServerBroadcastListenerPort)
                .Subscribe(UdpUtil.IntWriter(serverTcpListenerPort)));

            var tcpCommunication = ClientAcceptor.From(serverTcpListenerPort)
                .Select(client => new SecureClientCommunication(
                    new TcpClientCommunication(client),
                    new Encrpytion(password, Constants.EncrpytionInitVector)));
            _generalSubscriptions.Add(tcpCommunication.Subscribe(SetSession));
        }

        private void SetSession(INetworkCommunication communication)
        {
            _sessionSubscriptions.Clear();
            _sessionSubscriptions.Add(communication);
            var communicator = new Communicator<Command>(communication, NewThreadScheduler.Default);
            var handler = new AudioServer(new AudioController());
            _sessionSubscriptions.Add(communicator.Subscribe(handler.OnNext, 
                Console.WriteLine, () => Console.WriteLine("communicator to handler complete")));
            _sessionSubscriptions.Add(handler.Subscribe(communicator.OnNext, 
                Console.WriteLine, () => Console.WriteLine("handler to communicator complete")));
            Console.WriteLine($"Client No {_counter++}, Created.");
        }

        public void Dispose()
        {
            _sessionSubscriptions.Dispose();
            _generalSubscriptions.Dispose();
        }
    }
}
