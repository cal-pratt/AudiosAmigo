using System;
using System.Reactive.Subjects;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid.Observables
{
    class ObservableOnItemLongClickListener<T> :
        Java.Lang.Object, AdapterView.IOnItemLongClickListener, IObservable<T>
        where T : Java.Lang.Object
    {
        private readonly Subject<T> _click = new Subject<T>();

        public ObservableOnItemLongClickListener(AdapterView adapterView)
        {
            adapterView.OnItemLongClickListener = this;
        }

        bool AdapterView.IOnItemLongClickListener.OnItemLongClick(AdapterView parent, View view, int position, long id)
        {
            _click.OnNext((T)parent.GetItemAtPosition(position));
            return true;
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            return _click.Subscribe(observer);
        }
    }
}
