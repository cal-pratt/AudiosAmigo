/*************************************************************************************************
*************          THIS IS A GENERATED CLASS! DO NOT EDIT DIRECTLY !!!           *************
*************************************************************************************************/

using System;
namespace AudiosAmigo
{
    public abstract partial class CommandHandler : IObserver<Command>
    {
        public abstract void OnError(Exception error);

        public abstract void OnCompleted();

        public void OnNext(Command command)
        {
            switch (command?.Action)
            {
                case "UpdateProcess":
                    UpdateProcess(
                        Translate.StringToObject<AudioProcessState>(command.Parameters[0]));
                    break;

                case "UpdateDevice":
                    UpdateDevice(
                        Translate.StringToObject<AudioDeviceState>(command.Parameters[0]));
                    break;

                case "GetProcessImage":
                    GetProcessImage(
                        Translate.StringToObject<AudioProcessState>(command.Parameters[0]));
                    break;

                case "GetDeviceImage":
                    GetDeviceImage(
                        Translate.StringToObject<AudioDeviceState>(command.Parameters[0]));
                    break;

                case "UpdateProcessImage":
                    UpdateProcessImage(
                        Translate.StringToObject<AudioProcessState>(command.Parameters[0]),
                        command.Parameters[1]);
                    break;

                case "UpdateDeviceImage":
                    UpdateDeviceImage(
                        Translate.StringToObject<AudioDeviceState>(command.Parameters[0]),
                        command.Parameters[1]);
                    break;
            }
        }
    }
}
