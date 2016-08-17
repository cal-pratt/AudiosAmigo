using System;
using System.Reactive.Subjects;
using Android.Views;

namespace AudiosAmigo.Droid.Observables
{
    public class ObservableClickListener : Java.Lang.Object, View.IOnClickListener, IObservable<bool>
    {
        private readonly Subject<bool> _click = new Subject<bool>();

        public ObservableClickListener(View view)
        {
            view.SetOnClickListener(this);
        }

        public void OnClick(View view)
        {
            _click.OnNext(view.Pressed);
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            return _click.Subscribe(observer);
        }
    }
}
