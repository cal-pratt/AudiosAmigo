using System;
using System.Reactive.Linq;
using Android.Views;

namespace AudiosAmigo.Droid
{
    public class AudioDeviceVolumeSlider : IObservable<AudioDeviceState>
    {
        public View Parent => _slider.Parent;
        
        private readonly VolumeSlider _slider;

        public AudioDeviceState State { get; private set; }

        public AudioDeviceVolumeSlider(AudioDeviceState device, VolumeSlider slider)
        {
            State = device;
            _slider = slider;
            Update(State);
        }

        public void Update(AudioDeviceState device)
        {
            _slider.Volume = device.Volume;
            _slider.Mute = device.Mute;
        }

        public IDisposable Subscribe(IObserver<AudioDeviceState> observer)
        {
            return _slider.Select(tuple => State = State.SetAudio(tuple.Item1, tuple.Item2))
                .Subscribe(observer);
        }
    }
}
