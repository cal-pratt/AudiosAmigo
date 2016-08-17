namespace AudiosAmigo
{
    public class AudioDeviceState
    {
        public string Name { get; set; }
        
        public float Volume { get; set; }

        public bool Mute { get; set; }

        public bool IsDefault { get; set; }

        public AudioDeviceState() { }

        public AudioDeviceState(string name, bool isDefault)
        {
            Name = name;
            Volume = 0;
            Mute = true;
            IsDefault = isDefault;
        }

        public AudioDeviceState SetAudio(float volume, bool mute) => new AudioDeviceState
        {
            Name = Name,
            Volume = volume,
            Mute = mute,
            IsDefault = IsDefault
        };

        public AudioDeviceState SetDefault(bool isDefault) => new AudioDeviceState
        {
            Name = Name,
            Volume = Volume,
            Mute = Mute,
            IsDefault = isDefault
        };

        protected bool Equals(AudioDeviceState other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((AudioDeviceState)obj);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override string ToString()
        {
            return $"{base.ToString()}: {Name}, {Volume}, {Mute}, {(IsDefault ? "DEFAULT" : "NON-DEFAULT")}";
        }
    }
}
