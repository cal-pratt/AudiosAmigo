using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Graphics;
using Android.Views;

namespace AudiosAmigo.Droid
{
    public class AudioController : IObservable<AudioProcessState>, IObservable<AudioDeviceState>
    {
        private readonly AudioDeviceController.Builder _audioDeviceControllerBuilder;

        private readonly DeviceSelector.Builder _deviceSelectorBuilder;

        private readonly ViewGroup _deviceContainer;

        private readonly Dictionary<string, AudioDeviceController> _controllers =
            new Dictionary<string, AudioDeviceController>();

        private readonly Subject<AudioDeviceController> _subject =
            new Subject<AudioDeviceController>();

        public AudioController(
            AudioDeviceController.Builder audioDeviceControllerBuilder,
            DeviceSelector.Builder deviceSelectorBuilder,
            ViewGroup deviceContainer)
        {
            _audioDeviceControllerBuilder = audioDeviceControllerBuilder;
            _deviceSelectorBuilder = deviceSelectorBuilder;
            _deviceContainer = deviceContainer;
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
                _controllers[device.Name] = _audioDeviceControllerBuilder.Build(device, image);

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

                var deviceSelector = _deviceSelectorBuilder.Build(image);
                _deviceContainer.AddView(deviceSelector.Parent);
                deviceSelector.Subscribe(pressed => setDeviceSliders());

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
