using System;
using System.Reactive.Subjects;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid.Observables
{
    public class ObservableOnItemClickListener<T> : 
        Java.Lang.Object, AdapterView.IOnItemClickListener, IObservable<T>
        where T : Java.Lang.Object
    {
        private readonly Subject<T> _click = new Subject<T>();

        public ObservableOnItemClickListener(AdapterView adapterView)
        {
            adapterView.OnItemClickListener = this;
        }

        public void OnItemClick(AdapterView parent, View view, int position, long id)
        {
            _click.OnNext((T) parent.GetItemAtPosition(position));
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _click.Subscribe(observer);
        }
    }
}
