namespace AudiosAmigo
{
    public class SecureClientCommunication : INetworkCommunication
    {
        private readonly Encrpytion _encrpytion;

        private readonly INetworkCommunication _communication;

        public SecureClientCommunication(INetworkCommunication communication, Encrpytion encrpytion)
        {
            _encrpytion = encrpytion;
            _communication = communication;
        }

        public string Receive()
        {
            return Translate.ByteArrayToString(ReceiveBytes());
        }

        public byte[] ReceiveBytes()
        {
            return _encrpytion.Decrypt(_communication.ReceiveBytes());
        }

        public void Send(string text)
        {
            SendBytes(Translate.StringToByteArray(text));
        }

        public void SendBytes(byte[] buffer)
        {
            _communication.SendBytes(_encrpytion.Encrypt(buffer));
        }

        public void Dispose()
        {
            _communication.Dispose();
        }
    }
}
