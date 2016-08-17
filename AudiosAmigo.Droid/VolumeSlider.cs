using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AudiosAmigo.Droid.Observables;
using AudiosAmigo.Droid.Views;
using static Android.Views.ViewGroup.LayoutParams;

namespace AudiosAmigo.Droid
{
    public class VolumeSlider : IObservable<Tuple<float, bool>>
    {
        public class Builder
        {
            private readonly LayoutInflater _inflater;

            private readonly Vibrator _vibrator;

            private readonly int _width;

            private readonly int _height;

            private readonly Bitmap _mute;

            public Builder(
                LayoutInflater inflater,
                Vibrator vibrator,
                int width, int height,
                Bitmap mute)
            {
                _inflater = inflater;
                _vibrator = vibrator;
                _width = width;
                _height = height;
                _mute = Bitmap.CreateScaledBitmap(mute, _width, _width, true);
            }

            public VolumeSlider Build(Bitmap icon)
            {
                var layout = (LinearLayout)_inflater.Inflate(Resource.Layout.volume_slider, null);
                layout.LayoutParameters = new LinearLayout.LayoutParams(WrapContent, WrapContent)
                {
                    Height = _height,
                    Width = _width
                };

                var imageButton = layout.FindViewById<ImageButton>(Resource.Id.process_image);
                imageButton.LayoutParameters = new LinearLayout.LayoutParams(WrapContent, WrapContent)
                {
                    Height = _width,
                    Width = _width
                };

                var seekBar = layout.FindViewById<VerticalSeekBar>(Resource.Id.seek_bar);
                seekBar.LayoutParameters = new LinearLayout.LayoutParams(WrapContent, WrapContent)
                {
                    Height = _height - _width - _width / 2,
                    Width = _width
                };
                seekBar.Vibrator = _vibrator;
                seekBar.Max = 100;

                var iconScaled = Bitmap.CreateScaledBitmap(icon, _width, _width, true);

                var muteIcon = Bitmap.CreateBitmap(_width, _width, _mute.GetConfig());
                var canvas = new Canvas(muteIcon);
                canvas.DrawBitmap(iconScaled, new Matrix(), null);
                canvas.DrawBitmap(_mute, 0, 0, null);

                return new VolumeSlider(layout, imageButton, seekBar, iconScaled, muteIcon);
            }
        }

        public View Parent { get; }

        public float Volume
        {
            get { return _seekBar.Progress / 100f; }
            set
            {
                _subject.Suppress(1000);
                _seekBar.Progress = (int)(value * 100);
            }
        }

        public bool Mute
        {
            get { return _isMuted; }
            set
            {
                _subject.Suppress(1000);
                if (value != _isMuted)
                {
                    _imageButton.PerformClick();
                }
            }
        }

        private readonly ImageButton _imageButton;

        private readonly SeekBar _seekBar;

        private bool _isMuted;

        private readonly SupressSubject<Tuple<float, bool>> _subject =
            new SupressSubject<Tuple<float, bool>>(new Subject<Tuple<float, bool>>());

        private VolumeSlider(
            LinearLayout parent,
            ImageButton imageButton,
            SeekBar seekBar,
            Bitmap normalImage,
            Bitmap muteImage)
        {
            Parent = parent;
            _imageButton = imageButton;
            _seekBar = seekBar;

            var volumeStream = new ObservableSeekBarListener(_seekBar)
                .Select(progress => progress/100f)
                .StartWith(0);
            var muteStream = new ObservableClickListener(_imageButton)
                .Scan(_isMuted, (last, _) => _isMuted = !last)
                .StartWith(_isMuted);
            muteStream.Subscribe(mute => _imageButton.SetImageBitmap(mute ? muteImage : normalImage));

            volumeStream.CombineLatest(muteStream, Tuple.Create).Skip(1)
                .Sample(TimeSpan.FromMilliseconds(10))
                .Subscribe(_subject.OnNext);
        }

        public IDisposable Subscribe(IObserver<Tuple<float, bool>> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}
