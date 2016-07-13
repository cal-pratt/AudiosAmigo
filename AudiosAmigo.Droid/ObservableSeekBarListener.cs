using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Android.Widget;

namespace AudiosAmigo.Droid
{
    public class ObservableSeekBarListener : Java.Lang.Object, SeekBar.IOnSeekBarChangeListener, IObservable<int>
    {
        private readonly Subject<int> _progressUpdate = new Subject<int>();

        private int _progress;

        private int _suppressCounter;
        
        public ObservableSeekBarListener(SeekBar seekbar)
        {
            _progress = seekbar.Progress;
            seekbar.SetOnSeekBarChangeListener(this);
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

        public void OnProgressChanged(SeekBar seekBar, int progress, bool fromUser)
        {
            Emit(seekBar);
        }

        public void OnStartTrackingTouch(SeekBar seekBar)
        {
            Emit(seekBar);
        }

        public void OnStopTrackingTouch(SeekBar seekBar)
        {
            Emit(seekBar);
        }

        private void Emit(SeekBar seekBar)
        {
            if (_suppressCounter <= 0)
            {
                if (_progress != seekBar.Progress)
                {
                    _progressUpdate.OnNext(seekBar.Progress);
                }
            }
            _progress = seekBar.Progress;
        }

        public IDisposable Subscribe(IObserver<int> observer)
        {
            return _progressUpdate.StartWith(_progress).Subscribe(observer);
        }
    }
}
