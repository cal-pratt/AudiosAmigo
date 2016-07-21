/*************************************************************************************************
*************          THIS IS A GENERATED CLASS! DO NOT EDIT DIRECTLY !!!           *************
*************************************************************************************************/

using System;
using System.Reactive.Subjects;

namespace AudiosAmigo
{
    public abstract partial class Server : IObserver<Command>, IObservable<Command>
    {
        public abstract void UpdateProcess(AudioProcessState state);

        public abstract void UpdateDevice(AudioDeviceState state);

        public abstract void GetProcessImage(AudioProcessState state);

        public abstract void GetDeviceImage(AudioDeviceState device);

        public void SendUpdateProcess(AudioProcessState state) => _SendUpdateProcess(state);

        public void SendUpdateDevice(AudioDeviceState state) => _SendUpdateDevice(state);

        public void SendUpdateProcessImage(AudioProcessState state, string image) => _SendUpdateProcessImage(state, image);

        public void SendUpdateDeviceImage(AudioDeviceState state, string image) => _SendUpdateDeviceImage(state, image);

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

        private void _SendUpdateProcessImage(AudioProcessState state, string image) 
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateProcessImage",
                Parameters = new string[] { Translate.ObjectToString(state), image }
            });
        }

        private void _SendUpdateDeviceImage(AudioDeviceState state, string image) 
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateDeviceImage",
                Parameters = new string[] { Translate.ObjectToString(state), image }
            });
        }
        #endregion 
    }
}

