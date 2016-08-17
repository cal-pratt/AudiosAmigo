using System;
using System.Diagnostics;
using System.Drawing;
using System.Reactive.Linq;
using AudiosAmigo.Windows.Observables;
using NAudio.CoreAudioApi;
using NAudio.CoreAudioApi.Interfaces;

namespace AudiosAmigo.Windows
{
    public class AudioProcessController : IObservable<AudioProcessState>
    {
        public AudioProcessState State { get; private set; }

        public Image Image { get; }

        private readonly AudioSessionControl _session;

        private readonly ObservableAudioSessionEvents _events;

        public AudioProcessController(string device, AudioSessionControl session)
        {
            _session = session;

            _events = new ObservableAudioSessionEvents(_session);

            int pid;
            string name;

            if (_session.IsSystemSoundsSession)
            {
                pid = AudioProcessState.SystemSoundsPid;
                name = AudioProcessState.SystemSoundsName;
                Image = SystemIcons.WinLogo.ToBitmap();
            }
            else
            {
                pid = (int)_session.GetProcessID;
                var process = Process.GetProcessById(pid);
                name = process.ProcessName;
                Image = Icon.ExtractAssociatedIcon(process.MainModule.FileName)?.ToBitmap();
                process.EnableRaisingEvents = true;
                process.Exited += (sender, args) =>
                {
                    _events.OnSessionDisconnected(
                        AudioSessionDisconnectReason.DisconnectReasonSessionDisconnected);
                };
            }

            State = new AudioProcessState(name, device, pid);
        }

        public void Close()
        {
            _events.Complete();
        }

        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            var states = _events
                .Select(vol => State = State.SetAudio(vol.Item1, vol.Item2))
                .Sample(TimeSpan.FromMilliseconds(10));
            return states.Concat(states.LastAsync()
                .Select(state => State = state.SetAlive(false)))
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
