/*************************************************************************************************
*************          THIS IS A GENERATED CLASS! DO NOT EDIT DIRECTLY !!!           *************
*************************************************************************************************/

using System;
using System.Reactive.Subjects;

namespace AudiosAmigo
{
    public abstract partial class Client : IObserver<Command>, IObservable<Command>
    {
        public abstract void UpdateProcess(AudioProcessState state);

        public abstract void UpdateDevice(AudioDeviceState state);

        public abstract void UpdateProcessImage(AudioProcessState state, string image);

        public abstract void UpdateDeviceImage(AudioDeviceState state, string image);

        public void SendUpdateProcess(AudioProcessState state) => _SendUpdateProcess(state);

        public void SendUpdateDevice(AudioDeviceState state) => _SendUpdateDevice(state);

        public void SendGetProcessImage(AudioProcessState state) => _SendGetProcessImage(state);

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

        private void _SendUpdateProcess(AudioProcessState state) 
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateProcess",
                Parameters = new string[] { Translate.ObjectToString(state) }
            });
        }

        private void _SendUpdateDevice(AudioDeviceState state) 
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateDevice",
                Parameters = new string[] { Translate.ObjectToString(state) }
            });
        }

        private void _SendGetProcessImage(AudioProcessState state) 
        {
            _subject.OnNext(new Command
            {
                Action = "GetProcessImage",
                Parameters = new string[] { Translate.ObjectToString(state) }
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

