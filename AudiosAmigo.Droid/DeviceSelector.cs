using System;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AudiosAmigo.Droid.Observables;

namespace AudiosAmigo.Droid
{
    public class DeviceSelector : IObservable<bool>
    {
        public class Builder
        {
            private readonly LayoutInflater _inflater;

            private readonly int _width;

            private readonly int _height;

            public Builder(LayoutInflater inflater, int width, int height)
            {
                _inflater = inflater;
                _width = width;
                _height = height;
            }

            public DeviceSelector Build(Bitmap image)
            {
                var imageButton = (ImageButton)_inflater.Inflate(Resource.Layout.device_selector, null);
                imageButton.SetImageBitmap(Bitmap.CreateScaledBitmap(image, _width, _height, true));
                return new DeviceSelector(imageButton);
            }
        }

        public View Parent => _imageButton;

        private readonly ImageButton _imageButton;
        
        private DeviceSelector(ImageButton imageButton)
        {
            _imageButton = imageButton;
        }

        public IDisposable Subscribe(IObserver<bool> observer)
        {
            return new ObservableClickListener(_imageButton).Subscribe(observer);
        }
    }
}
