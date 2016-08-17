using System;
using System.Drawing;
using System.Reactive.Linq;

namespace AudiosAmigo.Windows
{
    public class AudioServer : Server
    {
        private readonly AudioController _controller;

        public AudioServer(AudioController controller)
        {
            _controller = controller;
            _controller.Subscribe<AudioProcessState>(SendUpdateProcess);
            _controller.Subscribe<AudioDeviceState>(SendUpdateDevice);
        }

        public void Close()
        {
            _controller.Close();
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnCompleted()
        {

        }

        public override void GetAllDevices()
        {
            _controller.DeviceStates.Subscribe(SendUpdateDevice);
        }

        public override void GetAllProcesses(AudioDeviceState device)
        {
            _controller.ProcessStates.Where(state => state.Device == device.Name)
                .Subscribe(SendUpdateProcess);
        }

        public override void UpdateProcess(AudioProcessState process)
        {
            _controller.UpdateProcess(process);
        }

        public override void UpdateDevice(AudioDeviceState device)
        {
            _controller.UpdateDevice(device);
        }

        public override void GetProcessImage(AudioProcessState process)
        {
            var image = _controller.GetProcessImage(process);
            var converter = new ImageConverter();
            var imageBytes = (byte[]) converter.ConvertTo(image, typeof(byte[]));
            if (imageBytes != null)
            {
                var base64String = Convert.ToBase64String(imageBytes);
                SendUpdateProcessImage(process, base64String);
            }
        }

        public override void GetDeviceImage(AudioDeviceState device)
        {
            var converter = new ImageConverter();
            var image = _controller.GetDeviceImage(device);
            var imageBytes = (byte[])converter.ConvertTo(image, typeof(byte[]));
            if (imageBytes != null)
            {
                var base64String = Convert.ToBase64String(imageBytes);
                SendUpdateDeviceImage(device, base64String);
            }
        }
    }
}
