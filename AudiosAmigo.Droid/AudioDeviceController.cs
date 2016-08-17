using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid
{
    public class AudioDeviceController : IObservable<AudioProcessState>, IObservable<AudioDeviceState>
    {
        public class Builder
        {
            private readonly VolumeSlider.Builder _volumeSliderBuilder;

            private readonly Bitmap _systemBitmap;

            private readonly ViewGroup _processSliderContainer;

            private readonly ViewGroup _deviceSliderContainer;

            private readonly ViewGroup _systemSliderContainer;

            private readonly TextView _status;

            public Builder(
                VolumeSlider.Builder volumeSliderBuilder,
                ViewGroup processSliderContainer,
                ViewGroup deviceSliderContainer,
                ViewGroup systemSliderContainer,
                TextView status,
                Bitmap systemBitmap)
            {
                _volumeSliderBuilder = volumeSliderBuilder;
                _systemBitmap = systemBitmap;
                _processSliderContainer = processSliderContainer;
                _deviceSliderContainer = deviceSliderContainer;
                _systemSliderContainer = systemSliderContainer;
                _status = status;
            }

            public AudioDeviceController Build(AudioDeviceState device, Bitmap image)
            {
                var systemState = new AudioProcessState(
                    AudioProcessState.SystemSoundsName,
                    device.Name,
                    AudioProcessState.SystemSoundsPid);

                return new AudioDeviceController(
                    _volumeSliderBuilder,
                    device, image,
                    systemState, _systemBitmap,
                    _processSliderContainer,
                    _deviceSliderContainer,
                    _systemSliderContainer,
                    _status);
            }
        }

        private readonly VolumeSlider.Builder _volumeSliderBuilder;

        private readonly ViewGroup _processSliderContainer;

        private readonly TextView _status;

        private readonly AudioDeviceVolumeSlider _deviceVolumeSlider;

        private readonly Dictionary<AudioProcessState, AudioProcessVolumeSlider> _processVolumeSliders = 
            new Dictionary<AudioProcessState, AudioProcessVolumeSlider>();
        
        private readonly Subject<IObservable<AudioProcessState>> _subject =
            new Subject<IObservable<AudioProcessState>>();

        private AudioDeviceController(
            VolumeSlider.Builder volumeSliderBuilder,
            AudioDeviceState deviceState, Bitmap deviceBitmap,
            AudioProcessState systemState, Bitmap systemBitmap,
            ViewGroup processSliderContainer, 
            ViewGroup deviceSliderContainer, 
            ViewGroup systemSliderContainer, 
            TextView status)
        {
            _volumeSliderBuilder = volumeSliderBuilder;
            _processSliderContainer = processSliderContainer;
            _status = status;

            _deviceVolumeSlider = new AudioDeviceVolumeSlider(deviceState, _volumeSliderBuilder.Build(deviceBitmap));
            _deviceVolumeSlider.Parent.Visibility = ViewState;
            deviceSliderContainer.AddView(_deviceVolumeSlider.Parent);
            _deviceVolumeSlider.Subscribe(state => UpdateStatus(state.Name, state.Volume, state.Mute));

            var systemController = new AudioProcessVolumeSlider(systemState, _volumeSliderBuilder.Build(systemBitmap));
            systemController.Parent.Visibility = ViewState;
            systemSliderContainer.AddView(systemController.Parent);
            systemController.Subscribe(state => UpdateStatus(state.Name, state.Volume, state.Mute));
            _processVolumeSliders[systemState] = systemController;
        }

        private ViewStates _viewState = ViewStates.Gone;

        public ViewStates ViewState
        {
            get { return _viewState; }
            set
            {
                _deviceVolumeSlider.Parent.Visibility = value;
                foreach (var controller in _processVolumeSliders.Values)
                {
                    controller.Parent.Visibility = value;
                }
                _viewState = value;
            }
        }

        public void UpdateDevice(AudioDeviceState device)
        {
            _deviceVolumeSlider.Update(device);
            UpdateStatus(device.Name, device.Volume, device.Mute);
        }

        public bool ContainsProcess(AudioProcessState process)
        {
            return _processVolumeSliders.ContainsKey(process);
        }

        public void UpdateProcess(AudioProcessState process)
        {
            if (_processVolumeSliders.ContainsKey(process))
            {
                if (!process.IsAlive)
                {
                    _processSliderContainer.RemoveView(_processVolumeSliders[process].Parent);
                    _processVolumeSliders.Remove(process);
                }
                else
                {
                    _processVolumeSliders[process].Update(process);
                    UpdateStatus(process.Name, process.Volume, process.Mute);
                }
            }
        }

        public void CreateProcess(AudioProcessState process, Bitmap bitmap)
        {
            var processController = new AudioProcessVolumeSlider(process, _volumeSliderBuilder.Build(bitmap));
            processController.Parent.Visibility = ViewState;
            _processSliderContainer.AddView(processController.Parent);
            _subject.OnNext(processController);
            processController.Subscribe(s => UpdateStatus(s.Name, s.Volume, s.Mute));
            _processVolumeSliders[process] = processController;
        }

        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            return _processVolumeSliders.Values.Merge()
                .Merge(_subject.Merge())
                .Subscribe(observer);
        }

        public IDisposable Subscribe(IObserver<AudioDeviceState> observer)
        {
            return _deviceVolumeSlider.Subscribe(observer);
        }

        private void UpdateStatus(string name, float volume, bool mute)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {
                var vol = (int) (mute ? 0 : volume*100);
                _status.Text = $"{name}, {vol}%";
            });
        }
    }
}
