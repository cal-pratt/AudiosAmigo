using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reactive.Linq;
using NAudio.CoreAudioApi;

namespace AudiosAmigo.Windows
{
    public class AudioController : IObservable<AudioProcessState>, IObservable<AudioDeviceState>
    {
        private readonly Dictionary<string, AudioDeviceController> _controllerMap =
            new Dictionary<string, AudioDeviceController>();

        private readonly object _lock = new object();

        public AudioController()
        {
            lock (_lock)
            {
                var deviceEnumerator = new MMDeviceEnumerator();
                var defaultName = deviceEnumerator.GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia).FriendlyName;
                foreach (var endPoint in deviceEnumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
                {
                    var name = endPoint.FriendlyName;
                    if (_controllerMap.ContainsKey(name))
                    {
                        for (var i = 1; ; i++)
                        {
                            var nameNumbered = $"{name} ({i})";
                            if (!_controllerMap.ContainsKey(nameNumbered))
                            {
                                name = nameNumbered;
                                break;
                            }
                        }
                    }
                    var controller = new AudioDeviceController(name, defaultName == name, endPoint);
                    _controllerMap.Add(name, controller);
                }
            }
        }

        internal void Close()
        {
            lock (_lock)
            {
                foreach (var controller in _controllerMap.Values)
                {
                    controller.Close();
                }
            }
        }

        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            lock (_lock)
            {
                return _controllerMap.Values.Merge<AudioProcessState>().Subscribe(observer);
            }
        }

        public IDisposable Subscribe(IObserver<AudioDeviceState> observer)
        {
            lock (_lock)
            {
                return _controllerMap.Values.Merge<AudioDeviceState>().Subscribe(observer);
            }
        }

        public void UpdateProcess(AudioProcessState state)
        {
            lock (_lock)
            {
                if (_controllerMap.ContainsKey(state.Device))
                {
                    _controllerMap[state.Device].UpdateAudioProcess(state);
                }
            }
        }

        public void UpdateDevice(AudioDeviceState state)
        {
            lock (_lock)
            {
                if (_controllerMap.ContainsKey(state.Name))
                {
                    _controllerMap[state.Name].SetVolume(state.Volume);
                    _controllerMap[state.Name].SetMute(state.Mute);
                }
            }
        }

        public Image GetProcessImage(AudioProcessState state)
        {
            lock (_lock)
            {
                if (_controllerMap.ContainsKey(state.Device))
                {
                    return _controllerMap[state.Device].GetAudioProcessImage(state);
                }
                return null;
            }
        }

        public Image GetDeviceImage(AudioDeviceState state)
        {
            lock (_lock)
            {
                if (_controllerMap.ContainsKey(state.Name))
                {
                    return _controllerMap[state.Name].Image;
                }
                return null;
            }
        }
    }
}
