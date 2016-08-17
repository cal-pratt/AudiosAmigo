using System;
using System.Reactive.Linq;
using Android.Views;

namespace AudiosAmigo.Droid
{
    public class AudioProcessVolumeSlider : IObservable<AudioProcessState>
    {
        public View Parent => _slider.Parent;

        private readonly VolumeSlider _slider;

        public AudioProcessState State { get; private set; }

        public AudioProcessVolumeSlider(AudioProcessState process, VolumeSlider slider)
        {
            State = process;
            _slider = slider;
            Update(State);
        }

        public void Update(AudioProcessState process)
        {
            _slider.Volume = process.Volume;
            _slider.Mute = process.Mute;
        }

        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            return _slider.Select(tuple => State = State.SetAudio(tuple.Item1, tuple.Item2))
                .Subscribe(observer);
        }
    }
}
