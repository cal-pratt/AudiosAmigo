/*************************************************************************************************
*************          THIS IS A GENERATED CLASS! DO NOT EDIT DIRECTLY !!!           *************
*************************************************************************************************/

using System;
using System.Reactive.Subjects;

namespace AudiosAmigo
{    public abstract class Client : IObserver<Command>, IObservable<Command>
    {
        public abstract void UpdateProcess(AudioProcessState process);

        public abstract void UpdateDevice(AudioDeviceState device);

        public abstract void UpdateProcessImage(AudioProcessState process, string image);

        public abstract void UpdateDeviceImage(AudioDeviceState device, string image);

        public void SendGetAllDevices() => _SendGetAllDevices();

        public void SendGetAllProcesses(AudioDeviceState device) => _SendGetAllProcesses(device);

        public void SendUpdateProcess(AudioProcessState process) => _SendUpdateProcess(process);

        public void SendUpdateDevice(AudioDeviceState device) => _SendUpdateDevice(device);

        public void SendGetProcessImage(AudioProcessState process) => _SendGetProcessImage(process);

        public void SendGetDeviceImage(AudioDeviceState device) => _SendGetDeviceImage(device);

        public abstract void OnError(Exception error);

        public abstract void OnCompleted();

        #region internal
        public void OnNext(Command command)
        {
            switch (command?.Action)
            {
                case "UpdateProcess":
                    UpdateProcess(Translate.StringToObject<AudioProcessState>(command.Parameters[0]));
                    break;
                case "UpdateDevice":
                    UpdateDevice(Translate.StringToObject<AudioDeviceState>(command.Parameters[0]));
                    break;
                case "UpdateProcessImage":
                    UpdateProcessImage(Translate.StringToObject<AudioProcessState>(command.Parameters[0]), command.Parameters[1]);
                    break;
                case "UpdateDeviceImage":
                    UpdateDeviceImage(Translate.StringToObject<AudioDeviceState>(command.Parameters[0]), command.Parameters[1]);
                    break;
            }
        }

        private readonly Subject<Command> _subject = new Subject<Command>();

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            return _subject.Subscribe(observer);
        } 
        private void _SendGetAllDevices() 
        {
            _subject.OnNext(new Command
            {
                Action = "GetAllDevices",
                Parameters = new string[] {  }
            });
        }
        private void _SendGetAllProcesses(AudioDeviceState device) 
        {
            _subject.OnNext(new Command
            {
                Action = "GetAllProcesses",
                Parameters = new string[] { Translate.ObjectToString(device) }
            });
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
        private void _SendGetProcessImage(AudioProcessState process) 
        {
            _subject.OnNext(new Command
            {
                Action = "GetProcessImage",
                Parameters = new string[] { Translate.ObjectToString(process) }
            });
        }
        private void _SendGetDeviceImage(AudioDeviceState device) 
        {
            _subject.OnNext(new Command
            {
                Action = "GetDeviceImage",
                Parameters = new string[] { Translate.ObjectToString(device) }
            });
        }
        #endregion 
    }
}

