/**
 *  This file is used to autogenerate a command interface.
 *  CommandHandler both obsersves and emits Command objects which 
 *  can then be serialized and streamed over the network.
 *  
 *  For simplicity, link this build with CommandReader and CommandWriter using this tool:
 *  https://visualstudiogallery.msdn.microsoft.com/ecb123bf-44bb-4ae3-91ee-a08fc1b9770e
 *  tt files with can be read easier with this tool:
 *  https://www.devart.com/t4-editor/download.html
 **/

namespace AudiosAmigo
{
    public abstract partial class CommandHandler
    {
        public abstract void UpdateProcess(AudioProcessState state);

        public abstract void UpdateDevice(AudioDeviceState state);

        public abstract void GetProcessImage(AudioProcessState state);

        public abstract void GetDeviceImage(AudioDeviceState device);

        public abstract void UpdateProcessImage(AudioProcessState state, string image);

        public abstract void UpdateDeviceImage(AudioDeviceState state, string image);
    }
}
