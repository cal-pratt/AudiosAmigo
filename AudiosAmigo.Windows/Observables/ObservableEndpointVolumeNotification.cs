using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NAudio.CoreAudioApi;

namespace AudiosAmigo.Windows.Observables
{
    public class ObservableEndpointVolumeNotification : IObservable<Tuple<float, bool>>
    {
        private readonly SupressSubject<Tuple<float, bool>> _subject =
            new SupressSubject<Tuple<float, bool>>(new Subject<Tuple<float, bool>>());

        private float _volume;

        private bool _mute;

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
            _subject.Suppress(delay);
        }

        public void OnVolumeChanged(float volume, bool mute)
        {
            if (Math.Abs(_volume - volume) > 0.001 || _mute != mute)
            {
                _subject.OnNext(Tuple.Create(volume, mute));
            }
            _volume = volume;
            _mute = mute;
        }
    }
}
