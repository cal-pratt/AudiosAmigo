using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using NAudio.CoreAudioApi;

namespace AudiosAmigo.Windows
{
    public class AudioDeviceController : IObservable<AudioProcessState>, IObservable<AudioDeviceState>
    {
        private readonly string _name;

        private readonly bool _isDefault;

        private readonly MMDevice _endpoint;

        private readonly Dictionary<int, AudioProcessController> _controllerMap =
            new Dictionary<int, AudioProcessController>();

        public Image Image { get; }

        private readonly Subject<IObservable<AudioProcessState>> _subject =
            new Subject<IObservable<AudioProcessState>>();

        private readonly ObservableEndpointVolumeNotification _notifications;

        public AudioDeviceController(string name, bool isDefault, MMDevice endpoint)
        {
            _endpoint = endpoint;
            _name = name;
            _isDefault = isDefault;

            var iconAddress = endpoint.IconPath.Split(',');
            IntPtr piLargeVersion, piSmallVersion;
            IconExtract.ExtractIconEx(
                iconAddress[0], 
                int.Parse(iconAddress[1]), 
                out piLargeVersion, 
                out piSmallVersion, 1);

            Image = Icon.FromHandle(piLargeVersion).ToBitmap();

            Refresh();

            _notifications = new ObservableEndpointVolumeNotification(endpoint.AudioEndpointVolume);

            _endpoint.AudioSessionManager.OnSessionCreated += (sender, session) =>
            {
                Refresh();
            };
        }

        public void Close()
        {
            foreach (var controller in _controllerMap.Values)
            {
                controller.Close();
            }
            _subject.OnCompleted();
            _endpoint.AudioSessionManager.Dispose();
        }

        public IDisposable Subscribe(IObserver<AudioProcessState> observer)
        {
            return _controllerMap.Values.Merge().Merge(_subject.Merge()).Subscribe(observer);
        }

        public void UpdateAudioProcess(AudioProcessState state)
        {
            if (_controllerMap.ContainsKey(state.Pid))
            {
                _controllerMap[state.Pid].SetVolume(state.Volume);
                _controllerMap[state.Pid].SetMute(state.Mute);
            }
        }

        public void SetVolume(float volume)
        {
            _notifications.Suppress(1000);
            _endpoint.AudioEndpointVolume.MasterVolumeLevelScalar = volume;
        }

        public void SetMute(bool mute)
        {
            _notifications.Suppress(1000);
            _endpoint.AudioEndpointVolume.Mute = mute;
        }

        public Image GetAudioProcessImage(AudioProcessState state)
        {
            if (_controllerMap.ContainsKey(state.Pid))
            {
                return _controllerMap[state.Pid]?.Image;
            }
            return null;
        }

        private void Refresh()
        {
            var sessionMap = SessionMap();

            var missing = _controllerMap.Keys.Where(key => !sessionMap.Keys.Contains(key)).ToList();

            var recent = sessionMap.Where(entry => !_controllerMap.Keys.Contains(entry.Key)).ToList();

            foreach (var key in missing)
            {
                _controllerMap.Remove(key);
            }

            foreach (var entry in recent)
            {
                var controller = new AudioProcessController(_name, entry.Value);
                _controllerMap.Add(entry.Key, controller);
                _subject.OnNext(controller);
            }
        }

        private Dictionary<int, AudioSessionControl> SessionMap()
        {
            var sessionMap = new Dictionary<int, AudioSessionControl>();
            _endpoint.AudioSessionManager.RefreshSessions();
            var sessions = _endpoint.AudioSessionManager.Sessions;
            for (var i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (ProcessExists(session.GetProcessID) && session.IsSystemSoundsSession)
                {
                    if (!sessionMap.ContainsKey((int)session.GetProcessID))
                    {
                        sessionMap.Add((int)session.GetProcessID, session);
                        break;
                    }
                }
            }
            for (var i = 0; i < sessions.Count; i++)
            {
                var session = sessions[i];
                if (ProcessExists(session.GetProcessID) && !session.IsSystemSoundsSession)
                {
                    if (!sessionMap.ContainsKey((int)session.GetProcessID))
                    {
                        sessionMap.Add((int)session.GetProcessID, session);
                    }
                }
            }
            return sessionMap;
        }

        private static bool ProcessExists(uint processId)
        {
            return Process.GetProcesses().Any(x => x.Id == processId);
        }

        public IDisposable Subscribe(IObserver<AudioDeviceState> observer)
        {
            return _notifications
                .Select(volumeEvent =>
                new AudioDeviceState
                {
                    Name = _name,
                    Volume = volumeEvent.Item1,
                    Mute = volumeEvent.Item2,
                    IsDefault = _isDefault
                }).Sample(TimeSpan.FromMilliseconds(10))
                .Subscribe(observer);
        }
    }
}
