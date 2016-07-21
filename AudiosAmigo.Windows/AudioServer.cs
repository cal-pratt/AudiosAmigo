using System;
using System.Drawing;

namespace AudiosAmigo.Windows
{
    public class AudioServer : Server
    {
        private readonly AudioController _controller = new AudioController();

        public AudioServer()
        {
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

        public override void UpdateProcess(AudioProcessState state)
        {
            _controller.UpdateProcess(state);
        }

        public override void UpdateDevice(AudioDeviceState state)
        {
            _controller.UpdateDevice(state);
        }

        public override void GetProcessImage(AudioProcessState state)
        {
            var image = _controller.GetProcessImage(state);
            var converter = new ImageConverter();
            var imageBytes = (byte[]) converter.ConvertTo(image, typeof(byte[]));
            if (imageBytes != null)
            {
                var base64String = Convert.ToBase64String(imageBytes);
                SendUpdateProcessImage(state, base64String);
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
