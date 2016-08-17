using System;
using System.Reactive.Subjects;
using Android.Views;

namespace AudiosAmigo.Droid.Observables
{
    public class ObservableOnTouchListener : Java.Lang.Object, View.IOnTouchListener, IObservable<bool>
    {
        private readonly Subject<bool> _touch = new Subject<bool>();

        public ObservableOnTouchListener(View view)
        {
            view.SetOnTouchListener(this);
        }
        
        public bool OnTouch(View view, MotionEvent e)
        {
            _touch.OnNext(view.IsInTouchMode);
            return false;
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            return _touch.Subscribe(observer);
        }
    }
}
