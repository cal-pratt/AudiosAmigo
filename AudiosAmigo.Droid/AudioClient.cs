using System;
using Android.Graphics;
using Android.OS;
using Android.Util;

namespace AudiosAmigo.Droid
{
    public class AudioClient : Client
    {
        private readonly AudioController _controller;
        
        public AudioClient(AudioController controller)
        {
            _controller = controller;
            _controller.Subscribe<AudioProcessState>(SendUpdateProcess);
            _controller.Subscribe<AudioDeviceState>(SendUpdateDevice);
        }

        public override void UpdateProcess(AudioProcessState process)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {
                if (!_controller.UpdateProcess(process))
                {
                    SendGetProcessImage(process);
                }
            });
        }

        public override void UpdateDevice(AudioDeviceState device)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {
                if (!_controller.UpdateDevice(device))
                {
                    SendGetDeviceImage(device);
                }
            });
        }

        public override void UpdateProcessImage(AudioProcessState process, string image)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {
                var imageAsBytes = Base64.Decode(image, Base64Flags.Default);
                var bitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
                _controller.UpdateProcessImage(process, bitmap);
            });
        }

        public override void UpdateDeviceImage(AudioDeviceState device, string image)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {
                var imageAsBytes = Base64.Decode(image, Base64Flags.Default);
                var bitmap = BitmapFactory.DecodeByteArray(imageAsBytes, 0, imageAsBytes.Length);
                _controller.UpdateDeviceImage(device, bitmap);
                SendGetAllProcesses(device);
            });
        }

        public override void OnError(Exception error)
        {
            new Handler(Looper.MainLooper).Post(() =>
            {

            });
        }

        public override void OnCompleted()
        {
            new Handler(Looper.MainLooper).Post(() =>
            {

            });
        }
    }
}
