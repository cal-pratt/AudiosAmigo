using System;
using System.Reactive.Linq;
using Android.Content;
using Android.Graphics;
using Android.Views;

namespace AudiosAmigo.Droid
{
    public class AudioProcessVolumeSlider : IObservable<AudioProcessState>
    {
        public View Parent => _slider.Parent;

        private readonly VolumeSlider _slider;

        private AudioProcessState _state;
        
        public AudioProcessVolumeSlider(
            Context context,
            AudioProcessState process,
            int width, int height, Bitmap image)
        {
            _state = process;
            _slider = new VolumeSlider(context, process.Volume, process.Mute, width, height, image);
        }

        public void Update(AudioProcessState process)
        {
            _slider.SetVolume(process.Volume);
            _slider.SetMute(process.Mute);
        }

        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            return _slider.Select(tuple => _state = _state.SetAudio(tuple.Item1, tuple.Item2))
                .Subscribe(observer);
        }
    }
}
