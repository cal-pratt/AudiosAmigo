using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace AudiosAmigo.Droid
{
    public class AudioClient : CommandHandler
    {
        private readonly Activity _activity;

        private readonly LinearLayout _normalSliders;

        private readonly LinearLayout _masterSliders;

        private readonly LinearLayout _systemSliders;

        private readonly TextView _status;

        private const float WidthPercent = 0.18f;

        private readonly Dictionary<Tuple<string, int>, AudioProcessController> _processControllers;

        private readonly Dictionary<string, AudioDeviceController> _deviceControllers;

        private readonly Dictionary<string, ImageButton> _deviceButtons;

        private string _activeDevice;

        public AudioClient(Activity activity)
        {
            _activity = activity;
            _activeDevice = "";
            _normalSliders = _activity.FindViewById<LinearLayout>(Resource.Id.normal_sliders);
            _masterSliders = _activity.FindViewById<LinearLayout>(Resource.Id.master_sliders);
            _systemSliders = _activity.FindViewById<LinearLayout>(Resource.Id.system_sliders);
            _status = _activity.FindViewById<TextView>(Resource.Id.status);
            _deviceButtons = new Dictionary<string, ImageButton>();
            _processControllers = new Dictionary<Tuple<string, int>, AudioProcessController>();
            _deviceControllers = new Dictionary<string, AudioDeviceController>();
        }

        public override void UpdateProcess(AudioProcessState state)
        {
            _activity.RunOnUiThread(() =>
            {
                var key = Tuple.Create(state.Device, state.Pid);
                if (!_processControllers.ContainsKey(key))
                {
                    SendGetProcessImageCommand(state);
                    _processControllers.Add(key, null);
                }
                else if (state.IsAlive == false)
                {
                    if (_processControllers[key] != null)
                    {
                        _normalSliders.RemoveView(_processControllers[key].Parent);
                        _processControllers.Remove(key);
                    }

                }
                else
                {
                    _processControllers[key]?.Update(state);
                    UpdateStatus(state.Name, state.Volume, state.Mute);
                }
            });
        }

        public override void UpdateDevice(AudioDeviceState state)
        {
            _activity.RunOnUiThread(() =>
            {
                if (!_deviceButtons.ContainsKey(state.Name))
                {
                    SendGetDeviceImageCommand(state);
                    _deviceButtons.Add(state.Name, null);
                    _deviceControllers.Add(state.Name, null);
                }
                else
                {
                    _deviceControllers[state.Name]?.Update(state);
                    UpdateStatus(state.Name, state.Volume, state.Mute);
                }
            });
        }

        public override void GetProcessImage(AudioProcessState state)
        {
            throw new ApplicationException("Process images cannot be requested from a client");
        }

        public override void GetDeviceImage(AudioDeviceState device)
        {
            throw new ApplicationException("Device images cannot be requested from a client");
        }

        public override void UpdateProcessImage(AudioProcessState state, string image)
        {
            _activity.RunOnUiThread(() =>
            {
                var key = Tuple.Create(state.Device, state.Pid);
                var sliderHeight = _activity.FindViewById(Resource.Id.slider_scroll).Height;
                var sliderWidth = (int)(sliderHeight * WidthPercent);
                if (state.Name == "System Sounds")
                {
                    var systemBitmap = BitmapFactory.DecodeResource(_activity.Resources, Resource.Drawable.audiosrv);
                    _processControllers[key] = new AudioProcessController(
                        _activity, state, sliderWidth, sliderHeight, systemBitmap);
                    _systemSliders.AddView(_processControllers[key].Parent);
                }
                else
                {
                    var imageAsBytes = Base64.Decode(image, Base64Flags.Default);
                    var bitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
                    _processControllers[key] = new AudioProcessController(
                        _activity, state, sliderWidth, sliderHeight, bitmap);
                    _normalSliders.AddView(_processControllers[key].Parent);
                }
                _processControllers[key].Subscribe(SendUpdateProcessCommand);
                _processControllers[key].Subscribe(s => UpdateStatus(s.Name, s.Volume, s.Mute));
                if (_activeDevice != state.Device)
                {
                    _processControllers[key].Parent.Visibility = ViewStates.Gone;
                }
            });
        }

        public override void UpdateDeviceImage(AudioDeviceState state, string image)
        {
            _activity.RunOnUiThread(() =>
            {
                var sliderHeight = _activity.FindViewById(Resource.Id.slider_scroll).Height;
                var sliderWidth = (int)(sliderHeight * WidthPercent);

                var imageAsBytes = Base64.Decode(image, Base64Flags.Default);
                var bitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);

                _deviceControllers[state.Name] = new AudioDeviceController(
                    _activity, state, sliderWidth, sliderHeight, bitmap);

                _deviceControllers[state.Name].Subscribe(SendUpdateDeviceCommand);
                _deviceControllers[state.Name].Subscribe(s => UpdateStatus(s.Name, s.Volume, s.Mute));
                _masterSliders.AddView(_deviceControllers[state.Name].Parent);
                _deviceControllers[state.Name].Parent.Visibility = ViewStates.Gone;

                var inflater = (LayoutInflater)_activity.GetSystemService(Context.LayoutInflaterService);
                var imageButton = (ImageButton)inflater.Inflate(Resource.Layout.device_selector, null);
                imageButton.SetImageBitmap(Bitmap.CreateScaledBitmap(bitmap, sliderWidth, sliderWidth, true));
                var deviceContainer = _activity.FindViewById<LinearLayout>(Resource.Id.device_container);
                deviceContainer.AddView(imageButton);

                var setDeviceSliders = new Action(() =>
                {
                    _activeDevice = state.Name;
                    foreach (var controller in _processControllers.Values)
                    {
                        if (controller != null)
                        {
                            controller.Parent.Visibility = ViewStates.Gone;
                        }
                    }
                    foreach (var controller in _deviceControllers.Values)
                    {
                        if (controller != null)
                        {
                            controller.Parent.Visibility = ViewStates.Gone;
                        }
                    }
                    _deviceControllers[state.Name].Parent.Visibility = ViewStates.Visible;
                    foreach (var controller in _processControllers
                        .Where(entry => entry.Key.Item1 == state.Name)
                        .Select(entry => entry.Value))
                    {
                        if (controller != null)
                        {
                            controller.Parent.Visibility = ViewStates.Visible;
                        }
                    }
                });
                if (state.IsDefault)
                {
                    setDeviceSliders();
                }
                new ObservableClickListener(imageButton).Subscribe(pressed => setDeviceSliders());
            });
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnCompleted()
        {
            throw new NotImplementedException();
        }

        private void UpdateStatus(string name, float volume, bool mute)
        {
            _activity.RunOnUiThread(() =>
            {
                var vol = mute ? 0 : (int)(volume * 100);
                _status.Text = $"{name}, {vol}%";
            });
        }
    }
}
