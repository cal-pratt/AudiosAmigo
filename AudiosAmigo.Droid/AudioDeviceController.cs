using System;
using System.Reactive.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;

namespace AudiosAmigo.Droid
{
    public class AudioDeviceController : IObservable<AudioDeviceState>
    {
        public View Parent => _slider.Parent;

        private readonly VolumeSlider _slider;

        private AudioDeviceState _state;

        public AudioDeviceController(
            Context context,
            AudioDeviceState state,
            int width, int height, Bitmap image)
        {
            _state = state;
            _slider = new VolumeSlider(context, state.Volume, state.Mute, width, height, image);
        }

        public void Update(AudioDeviceState state)
        {
            _slider.SetVolume(state.Volume);
            _slider.SetMute(state.Mute);
        }

        public IDisposable Subscribe(IObserver<AudioDeviceState> observer)
        {
            return _slider.Select(
                tuple => _state = new AudioDeviceState
                {
                    Name = _state.Name,
                    Volume = tuple.Item1,
                    Mute = tuple.Item2,
                    IsDefault = _state.IsDefault
                }).Subscribe(observer);
        }
    }
}
