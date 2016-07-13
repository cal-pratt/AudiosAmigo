using System;
using System.Drawing;

namespace AudiosAmigo.Windows
{
    public class AudioServer : CommandHandler
    {
        private readonly AudioController _controller = new AudioController();

        public AudioServer()
        {
            _controller.Subscribe<AudioProcessState>(SendUpdateProcessCommand);
            _controller.Subscribe<AudioDeviceState>(SendUpdateDeviceCommand);
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
                SendUpdateProcessImageCommand(state, base64String);
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
                SendUpdateDeviceImageCommand(device, base64String);
            }
        }

        public override void UpdateProcessImage(AudioProcessState state, string image)
        {
            throw new ApplicationException("Server images cannot be updated");
        }

        public override void UpdateDeviceImage(AudioDeviceState state, string image)
        {
            throw new ApplicationException("Server images cannot be updated");
        }
    }
}
