using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace AudiosAmigo
{
    public class SupressSubject<T> : ISubject<T>
    {
        private readonly Subject<T> _subject;

        private int _suppressCounter;

        private T _last;

        public SupressSubject(Subject<T> subject)
        {
            _subject = subject;
            _last = default(T);
        }

        public void Suppress(int delay)
        {
            Interlocked.Increment(ref _suppressCounter);
            Task.Run(async () =>
            {
                await Task.Delay(delay);
                Interlocked.Decrement(ref _suppressCounter);
            });
        }

        public void OnNext(T next)
        {
            if (_suppressCounter <= 0)
            {
                if (next != null && !next.Equals(_last))
                {
                    _subject.OnNext(next);
                }
            }
            _last = next;
        }

        public void OnError(Exception error)
        {
            _subject.OnError(error);
        }

        public void OnCompleted()
        {
            _subject.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}
