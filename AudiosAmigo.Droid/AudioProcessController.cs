using System;
using System.Reactive.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;

namespace AudiosAmigo.Droid
{
    public class AudioProcessController : IObservable<AudioProcessState>
    {
        public View Parent => _slider.Parent;

        private readonly VolumeSlider _slider;

        private AudioProcessState _state;
        
        public AudioProcessController(
            Context context,
            AudioProcessState state,
            int width, int height, Bitmap image)
        {
            _state = state;
            _slider = new VolumeSlider(context, state.Volume, state.Mute, width, height, image);
        }

        public void Update(AudioProcessState state)
        {
            _slider.SetVolume(state.Volume);
            _slider.SetMute(state.Mute);
        }

        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            return _slider.Select(
                tuple => _state = new AudioProcessState
                {
                    Name = _state.Name,
                    Device = _state.Device,
                    Pid = _state.Pid,
                    Volume = tuple.Item1,
                    Mute = tuple.Item2,
                    IsAlive = true
                }).Subscribe(observer);
        }
    }
}
