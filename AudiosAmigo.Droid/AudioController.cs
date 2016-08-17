using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AudiosAmigo.Droid.Observables;

namespace AudiosAmigo.Droid
{
    public class AudioController : IObservable<AudioProcessState>, IObservable<AudioDeviceState>
    {
        private readonly Context _context;

        private readonly int _sliderWidth;

        private readonly int _sliderHeight;

        private readonly ViewGroup _processSliderContainer;

        private readonly ViewGroup _deviceSliderContainer;

        private readonly ViewGroup _systemSliderContainer;

        private readonly ViewGroup _deviceContainer;

        private readonly TextView _status;

        private readonly Dictionary<string, AudioDeviceController> _controllers =
            new Dictionary<string, AudioDeviceController>();

        private readonly Subject<AudioDeviceController> _subject =
            new Subject<AudioDeviceController>();

        public AudioController(
            Context context,
            int sliderWidth,
            int sliderHeight,
            ViewGroup processSliderContainer,
            ViewGroup deviceSliderContainer,
            ViewGroup systemSliderContainer,
            ViewGroup deviceContainer,
            TextView status)
        {
            _context = context;
            _sliderWidth = sliderWidth;
            _sliderHeight = sliderHeight;
            _processSliderContainer = processSliderContainer;
            _deviceSliderContainer = deviceSliderContainer;
            _systemSliderContainer = systemSliderContainer;
            _deviceContainer = deviceContainer;
            _status = status;
        }

        public bool UpdateProcess(AudioProcessState process)
        {
            if (_controllers.ContainsKey(process.Device))
            {
                if (!_controllers[process.Device].ContainsProcess(process))
                {
                    return false;
                }
                _controllers[process.Device].UpdateProcess(process);
            }
            return true;
        }

        public bool UpdateDevice(AudioDeviceState device)
        {
            if (!_controllers.ContainsKey(device.Name))
            {
                return false;
            }
             _controllers[device.Name].UpdateDevice(device);
            return true;
        }

        public void UpdateProcessImage(AudioProcessState process, Bitmap image)
        {
            if (_controllers.ContainsKey(process.Device))
            {
                if (!_controllers[process.Device].ContainsProcess(process))
                {
                    _controllers[process.Device].CreateProcess(process, image);
                }
            }
        }

        public void UpdateDeviceImage(AudioDeviceState device, Bitmap image)
        {
            if (!_controllers.ContainsKey(device.Name))
            {
                var systemBitmap = BitmapFactory.DecodeResource(_context.Resources, Resource.Drawable.audiosrv);
                var systemState = new AudioProcessState
                {
                    Name = AudioProcessState.SystemSoundsName,
                    Pid = AudioProcessState.SystemSoundsPid,
                    Device = device.Name
                };
                _controllers[device.Name] = new AudioDeviceController(_context, _sliderWidth, _sliderHeight,
                    device, image, systemState, systemBitmap,
                    _processSliderContainer, _deviceSliderContainer, _systemSliderContainer, _status);

                var inflater = (LayoutInflater)_context.GetSystemService(Context.LayoutInflaterService);
                var imageButton = (ImageButton)inflater.Inflate(Resource.Layout.device_selector, null);
                imageButton.SetImageBitmap(Bitmap.CreateScaledBitmap(image, _sliderWidth, _sliderWidth, true));

                _deviceContainer.AddView(imageButton);

                var setDeviceSliders = new Action(() =>
                {
                    foreach (var row in _controllers.Values)
                    {
                        row.ViewState = ViewStates.Gone;
                    }
                    _controllers[device.Name].ViewState = ViewStates.Visible;
                });

                if (device.IsDefault)
                {
                    setDeviceSliders();
                }

                new ObservableClickListener(imageButton).Subscribe(pressed => setDeviceSliders());
                _subject.OnNext(_controllers[device.Name]);
            }
        }
        
        public IDisposable Subscribe(IObserver<AudioDeviceState> observer)
        {
            return _controllers.Values.Merge<AudioDeviceState>()
                .Merge(_subject.Merge<AudioDeviceState>())
                .Subscribe(observer);
        }
        
        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            return _controllers.Values.Merge<AudioProcessState>()
                .Merge(_subject.Merge<AudioProcessState>())
                .Subscribe(observer);
        }
    }
}
