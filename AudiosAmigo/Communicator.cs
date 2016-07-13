using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;

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
            _input = Observable.Create<T>(
                observer =>
                {
                    try
                    {
                        while (true)
                        {
                            var s = Translate.StringToObject<T>(_communication.Receive());
                            observer.OnNext(s);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
                    }
                    observer.OnCompleted();
                    return Disposable.Empty;
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
                    Console.WriteLine($"{e}: {e.Message}\n{e.StackTrace}");
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
