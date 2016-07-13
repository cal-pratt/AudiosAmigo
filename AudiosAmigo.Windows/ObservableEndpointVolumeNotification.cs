using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using NAudio.CoreAudioApi;

namespace AudiosAmigo.Windows
{
    public class ObservableEndpointVolumeNotification : IObservable<Tuple<float, bool>>
    {
        private readonly Subject<Tuple<float, bool>> _subject = new Subject<Tuple<float, bool>>();

        private float _volume;

        private bool _mute;

        private int _suppressCounter;

        public ObservableEndpointVolumeNotification(AudioEndpointVolume endpointVolume)
        {
            _volume = endpointVolume.MasterVolumeLevelScalar;
            _mute = endpointVolume.Mute;
            endpointVolume.OnVolumeNotification += data =>
            {
                OnVolumeChanged(data.MasterVolume, data.Muted);
            };
        }

        internal void Complete()
        {
            _subject.OnCompleted();
        }

        public IDisposable Subscribe(IObserver<Tuple<float, bool>> observer)
        {
            return _subject.StartWith(Tuple.Create(_volume, _mute)).Subscribe(observer);
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

        public void OnVolumeChanged(float volume, bool mute)
        {
            if (_suppressCounter <= 0)
            {
                if (Math.Abs(_volume - volume) > 0.001 || _mute != mute)
                {
                    _subject.OnNext(Tuple.Create(volume, mute));
                }
            }
            _volume = volume;
            _mute = mute;
        }
    }
}
