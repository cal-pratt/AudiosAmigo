namespace AudiosAmigo
{
    public class AudioProcessState
    {
        public string Name { get; set; }

        public string Device { get; set; }

        public int Pid { get; set; }

        public float Volume { get; set; }

        public bool Mute { get; set; }

        public bool IsAlive { get; set; }

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
