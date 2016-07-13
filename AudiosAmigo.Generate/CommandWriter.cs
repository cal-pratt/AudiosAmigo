/*************************************************************************************************
*************          THIS IS A GENERATED CLASS! DO NOT EDIT DIRECTLY !!!           *************
*************************************************************************************************/

using System;
using System.Reactive.Subjects;
namespace AudiosAmigo
{
    public abstract partial class CommandHandler : IObservable<Command>
    {
        private readonly Subject<Command> _subject = new Subject<Command>();

        public IDisposable Subscribe(IObserver<Command> observer)
        {
            return _subject.Subscribe(observer);
        }

        public void SendUpdateProcessCommand(
            AudioProcessState state)
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateProcess",
                Parameters = new string[] 
                {
                    Translate.ObjectToString(state)
                }
            });
        }

        public void SendUpdateDeviceCommand(
            AudioDeviceState state)
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateDevice",
                Parameters = new string[] 
                {
                    Translate.ObjectToString(state)
                }
            });
        }

        public void SendGetProcessImageCommand(
            AudioProcessState state)
        {
            _subject.OnNext(new Command
            {
                Action = "GetProcessImage",
                Parameters = new string[] 
                {
                    Translate.ObjectToString(state)
                }
            });
        }

        public void SendGetDeviceImageCommand(
            AudioDeviceState device)
        {
            _subject.OnNext(new Command
            {
                Action = "GetDeviceImage",
                Parameters = new string[] 
                {
                    Translate.ObjectToString(device)
                }
            });
        }

        public void SendUpdateProcessImageCommand(
            AudioProcessState state,
            string image)
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateProcessImage",
                Parameters = new string[] 
                {
                    Translate.ObjectToString(state),
                    image
                }
            });
        }

        public void SendUpdateDeviceImageCommand(
            AudioDeviceState state,
            string image)
        {
            _subject.OnNext(new Command
            {
                Action = "UpdateDeviceImage",
                Parameters = new string[] 
                {
                    Translate.ObjectToString(state),
                    image
                }
            });
        }
    }
}
