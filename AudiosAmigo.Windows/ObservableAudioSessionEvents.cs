using System;
using System.Reactive.Subjects;
using System.Threading;
using System.Reactive.Linq;
using System.Threading.Tasks;
using NAudio.CoreAudioApi.Interfaces;
using NAudio.CoreAudioApi;

namespace AudiosAmigo.Windows
{
    public class ObservableAudioSessionEvents : IAudioSessionEventsHandler, IObservable<Tuple<float, bool>>
    {
        private readonly Subject<Tuple<float, bool>> _subject = new Subject<Tuple<float, bool>>();

        private float _volume;

        private bool _mute;

        private int _suppressCounter;

        public ObservableAudioSessionEvents(AudioSessionControl session)
        {
            _volume = session.SimpleAudioVolume.Volume;
            _mute = session.SimpleAudioVolume.Mute;
            session.RegisterEventClient(this);
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

        public void OnChannelVolumeChanged(uint channelCount, IntPtr newVolumes, uint channelIndex){}

        public void OnDisplayNameChanged(string displayName){}

        public void OnGroupingParamChanged(ref Guid groupingId){}

        public void OnIconPathChanged(string iconPath){}

        public void OnSessionDisconnected(AudioSessionDisconnectReason disconnectReason)
        {
            if (disconnectReason == AudioSessionDisconnectReason.DisconnectReasonSessionDisconnected)
            {
                _subject.OnCompleted();
            }
        }

        public void OnStateChanged(AudioSessionState state){}

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
