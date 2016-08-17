/*************************************************************************************************
*************          THIS IS A GENERATED CLASS! DO NOT EDIT DIRECTLY !!!           *************
*************************************************************************************************/

using System;
using System.Reactive.Subjects;

namespace AudiosAmigo
{    public abstract class Server : IObserver<Command>, IObservable<Command>
    {
        public abstract void GetAllDevices();

        public abstract void GetAllProcesses(AudioDeviceState device);

        public abstract void UpdateProcess(AudioProcessState process);

        public abstract void UpdateDevice(AudioDeviceState device);

        public abstract void GetProcessImage(AudioProcessState process);

        public abstract void GetDeviceImage(AudioDeviceState device);

        public void SendUpdateProcess(AudioProcessState process) => _SendUpdateProcess(process);

        public void SendUpdateDevice(AudioDeviceState device) => _SendUpdateDevice(device);

        public void SendUpdateProcessImage(AudioProcessState process, string image) => _SendUpdateProcessImage(process, image);

        public void SendUpdateDeviceImage(AudioDeviceState device, string image) => _SendUpdateDeviceImage(device, image);

        public abstract void OnError(Exception error);

        public abstract void OnCompleted();

        #region internal
        public void OnNext(Command command)
        {
            switch (command?.Action)
            {
                case "GetAllDevices":
                    GetAllDevices();
                    break;
                case "GetAllProcesses":
                    GetAllProcesses(Translate.StringToObject<AudioDeviceState>(command.Parameters[0]));
                    break;
                case "UpdateProcess":
                    UpdateProcess(Translate.StringToObject<AudioProcessState>(command.Parameters[0]));
                    break;
                case "UpdateDevice":
                    UpdateDevice(Translate.StringToObject<AudioDeviceState>(command.Parameters[0]));
                    break;
                case "GetProcessImage":
                    GetProcessImage(Translate.StringToObject<AudioProcessState>(command.Parameters[0]));
                    break;
                case "GetDeviceImage":
                    GetDeviceImage(Translate.StringToObject<AudioDeviceState>(command.Parameters[0]));
                    break;
            }
        }

        private readonly Subject<Command> _subject = new Subject<Command>();

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            return _subject.Subscribe(observer);
        } 
        private void _SendUpdateProcess(AudioProcessState process) 
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateProcess",
                Parameters = new string[] { Translate.ObjectToString(process) }
            });
        }
        private void _SendUpdateDevice(AudioDeviceState device) 
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateDevice",
                Parameters = new string[] { Translate.ObjectToString(device) }
            });
        }
        private void _SendUpdateProcessImage(AudioProcessState process, string image) 
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateProcessImage",
                Parameters = new string[] { Translate.ObjectToString(process), image }
            });
        }
        private void _SendUpdateDeviceImage(AudioDeviceState device, string image) 
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateDeviceImage",
                Parameters = new string[] { Translate.ObjectToString(device), image }
            });
        }
        #endregion 
    }
}

