namespace AudiosAmigo
{
    public class AudioDeviceState
    {
        public string Name { get; set; }
        
        public float Volume { get; set; }

        public bool Mute { get; set; }

        public bool IsDefault { get; set; }

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
