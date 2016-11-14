using System;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace AudiosAmigo
{
    public class Communicator<T> : IObservable<T>, IObserver<T>
    {
        private readonly INetworkCommunication _communication;

        private readonly object _lock = new object();

        private readonly IConnectableObservable<T> _input;

        public Communicator(INetworkCommunication communication, IScheduler scheduler)
        {
            _communication = communication;
            var running = true;
            _input = Observable.Create<T>(
                observer =>
                {
                    Task.Run(async () =>
                    {
                        await Task.Yield();
                        while (running)
                        {
                            var s = Translate.StringToObject<T>(_communication.Receive());
                            observer.OnNext(s);
                        }
                        observer.OnCompleted();
                    });
                    return Disposable.Create(() =>
                    {
                        running = false;
                        _communication.Dispose();
                    });
                }).ObserveOn(scheduler).Publish();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            var subscription = _input.Subscribe(observer);
            _input.Connect();
            return subscription;
        }

        public void OnNext(T value)
        {
            lock (_lock)
            {
                try
                {
                    _communication.Send(Translate.ObjectToString(value));
                }
                catch (Exception e)
                {
                    OnError(e);
                }
            }
        }

        public void OnError(Exception e)
        {
            Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
        }

        public void OnCompleted()
        {
            // ???
        }
    }
}
