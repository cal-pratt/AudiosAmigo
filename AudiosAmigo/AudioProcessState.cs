namespace AudiosAmigo
{
    public class AudioProcessState
    {
        public const string SystemSoundsName = "System Sounds";

        public const int SystemSoundsPid = -1;

        public string Name { get; set; }

        public string Device { get; set; }

        public int Pid { get; set; }

        public float Volume { get; set; }

        public bool Mute { get; set; }

        public bool IsAlive { get; set; }

        public AudioProcessState() { }

        public AudioProcessState(string name, string device, int pid)
        {
            Name = name;
            Device = device;
            Pid = pid;
            Volume = 0;
            Mute = true;
            IsAlive = true;
        }

        public AudioProcessState SetAlive(bool isAlive) => new AudioProcessState
        {
            Name = Name,
            Device = Device,
            Pid = Pid,
            Volume = Volume,
            Mute = Mute,
            IsAlive = isAlive
        };

        public AudioProcessState SetAudio(float volume, bool mute) => new AudioProcessState
        {
            Name = Name,
            Device = Device,
            Pid = Pid,
            Volume = volume,
            Mute = mute,
            IsAlive = IsAlive
        };

        protected bool Equals(AudioProcessState other)
        {
            return string.Equals(Name, other.Name) && string.Equals(Device, other.Device) && Pid == other.Pid;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AudioProcessState) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name.GetHashCode();
                hashCode = (hashCode*397) ^ Device.GetHashCode();
                hashCode = (hashCode*397) ^ Pid;
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()}: {Name}, {Device}, {Pid}, {Volume}, {Mute}, {(IsAlive ? "ALIVE" : "DEAD")}";
        }
    }
}
