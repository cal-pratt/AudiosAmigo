using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using AudiosAmigo.Droid.Observables;
using AudiosAmigo.Droid.Views;

namespace AudiosAmigo.Droid
{
    public class VolumeSlider : IObservable<Tuple<float, bool>>
    {
        public LinearLayout Parent { get; }

        private readonly SupressSubject<Tuple<float, bool>> _subject = 
            new SupressSubject<Tuple<float, bool>>(new Subject<Tuple<float, bool>>());

        private readonly VerticalSeekBar _seekBar;

        private readonly ImageButton _imageButton;

        private readonly Bitmap _normalImage;

        private readonly Bitmap _muteImage;

        private bool _isMuted;

        public VolumeSlider(
            Context context,
            float volume, bool mute,
            int width, int height, Bitmap image)
        {
            var inflater = (LayoutInflater)context.GetSystemService(Context.LayoutInflaterService);
            var vibrator = (Vibrator)context.GetSystemService(Context.VibratorService);
            Parent = (LinearLayout)inflater.Inflate(Resource.Layout.volume_slider, null);
            const int wc = ViewGroup.LayoutParams.WrapContent;
            Parent.LayoutParameters = new LinearLayout.LayoutParams(wc, wc)
            {
                Height = height,
                Width = width
            };

            _imageButton = Parent.FindViewById<ImageButton>(Resource.Id.process_image);
            _imageButton.LayoutParameters = new LinearLayout.LayoutParams(wc, wc)
            {
                Height = width,
                Width = width
            };

            _normalImage = Bitmap.CreateScaledBitmap(image, width, width, true);

            _isMuted = mute;
            var muteDefault = Bitmap.CreateScaledBitmap(
                BitmapFactory.DecodeResource(context.Resources, Resource.Drawable.muteblue), 
                width, width, true);

            _muteImage = Bitmap.CreateBitmap(muteDefault.Width, muteDefault.Height, muteDefault.GetConfig());
            var canvas = new Canvas(_muteImage);
            canvas.DrawBitmap(_normalImage, new Matrix(), null);
            canvas.DrawBitmap(muteDefault, 0, 0, null);

            SetMute(mute);

            _seekBar = Parent.FindViewById<VerticalSeekBar>(Resource.Id.seek_bar);
            _seekBar.LayoutParameters = new LinearLayout.LayoutParams(wc, wc)
            {
                Height = height - width - width / 2,
                Width = width
            };
            _seekBar.Max = 100;
            _seekBar.Progress = (int) (volume*100);

            var seekbarListener = new ObservableSeekBarListener(_seekBar);
            var volumeStream = seekbarListener
                .Select(progress => progress / 100f)
                .StartWith(volume);

            var muteStream = new ObservableClickListener(_imageButton)
                .Scan(mute, (last, _) => _isMuted = !last)
                .StartWith(mute);

            muteStream.Subscribe(m => _imageButton.SetImageBitmap(m ? _muteImage : _normalImage));

//      if (Settings.System.GetInt(context.ContentResolver, Settings.System.HapticFeedbackEnabled, 0) != 0)
//      {
            _seekBar.Vibrator = vibrator;
            muteStream.Subscribe(_ =>
            {
                vibrator.Vibrate(50);
            });
//      }

            _subject.Suppress(1000);
            volumeStream.CombineLatest(muteStream, Tuple.Create)
                .Sample(TimeSpan.FromMilliseconds(10))
                .Subscribe(_subject.OnNext);
        }

        public void SetVolume(float volume)
        {
            _subject.Suppress(1000);
            _seekBar.Progress = (int)(volume * 100);
        }

        public void SetMute(bool mute)
        {
            _subject.Suppress(1000);
            if (mute != _isMuted)
            {
                _imageButton.PerformClick();
            }
        }

        public IDisposable Subscribe(IObserver<Tuple<float, bool>> observer)
        {
            return _subject.Subscribe(observer);
        }
    }
}
