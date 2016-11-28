using Newtonsoft.Json;

namespace AudiosAmigo
{
    public class TcpConnectionInfo
    {
        public string Name { get; }

        public string Host { get; }

        public int Port { get; }

        public string PassHash { get; }

        [JsonConstructor]
        public TcpConnectionInfo(string name, string host, int port, string passHash)
        {
            Name = name;
            Host = host;
            Port = port;
            PassHash = passHash;
        }

        public static string GenerateHash(string value)
        {
            if (value == "")
            {
                value = Constants.DefaultSessionPassword;
            }
            return Translate.ByteArrayToBase64String(
                PasswordUtil.PbkdfHash(
                    Translate.StringToByteArray(value),
                    Constants.PasswordSalt,
                    Constants.PasswordIterations,
                    Constants.PasswordLength));
        }

        public override string ToString()
        {
            return $"{Name}, {Host}:{Port}";
        }
    }
}