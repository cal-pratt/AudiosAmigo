using System;
using System.Reactive.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;

namespace AudiosAmigo.Droid
{
    public class AudioDeviceVolumeSlider : IObservable<AudioDeviceState>
    {
        public View Parent => _slider.Parent;
        
        private readonly VolumeSlider _slider;

        public AudioDeviceState State { get; private set; }

        public AudioDeviceVolumeSlider(
            Context context,
            AudioDeviceState device,
            int width, int height, Bitmap image)
        {
            State = device;
            _slider = new VolumeSlider(context, device.Volume, device.Mute, width, height, image);
        }

        public void Update(AudioDeviceState device)
        {
            _slider.SetVolume(device.Volume);
            _slider.SetMute(device.Mute);
        }

        public IDisposable Subscribe(IObserver<AudioDeviceState> observer)
        {
            return _slider.Select(tuple => State = State.SetAudio(tuple.Item1, tuple.Item2))
                .Subscribe(observer);
        }
    }
}
