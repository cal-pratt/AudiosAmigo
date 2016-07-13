using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace AudiosAmigo.Windows
{
    public class AudioProcessController : IObservable<AudioProcessState>
    {
        public string Name { get; }

        public string Device { get; }

        public int Pid { get; }

        public Image Image { get; }

        private readonly AudioSessionControl _session;

        private readonly ObservableAudioSessionEvents _events;

        public AudioProcessController(string device, AudioSessionControl session)
        {
            Device = device;
            _session = session;
            Pid = (int)_session.GetProcessID;

            _events = new ObservableAudioSessionEvents(_session);

            if (_session.IsSystemSoundsSession)
            {
                Name = "System Sounds";
                Image = SystemIcons.WinLogo.ToBitmap();
            }
            else
            {
                var process = Process.GetProcessById(Pid);
                Name = process.ProcessName;
                Image = Icon.ExtractAssociatedIcon(process.MainModule.FileName)?.ToBitmap();
                process.EnableRaisingEvents = true;
                process.Exited += (sender, args) =>
                {
                    _events.OnSessionDisconnected(
                        AudioSessionDisconnectReason.DisconnectReasonSessionDisconnected);
                };
            }
        }

        public void Close()
        {
            _events.Complete();
        }

        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            var states = _events
                .Select(volumeEvent =>
                new AudioProcessState
                {
                    Name = Name,
                    Device = Device,
                    Pid = Pid,
                    Volume = volumeEvent.Item1,
                    Mute = volumeEvent.Item2,
                    IsAlive = true
                }).Sample(TimeSpan.FromMilliseconds(10));
            return states.Concat(states.LastAsync()
                .Select(state =>
                {
                    state.IsAlive = false;
                    return state;
                }))
                .Subscribe(observer);
        }

        public void SetVolume(float volume)
        {
            _events.Suppress(1000);
            _session.SimpleAudioVolume.Volume = volume;
        }

        public void SetMute(bool mute)
        {
            _events.Suppress(1000);
            _session.SimpleAudioVolume.Mute = mute;
        }
    }
}
