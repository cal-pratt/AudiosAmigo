namespace AudiosAmigo
{
    public interface INetworkCommunication
    {
        string Receive();

        byte[] ReceiveBytes();

        void Send(string text);

        void SendBytes(byte[] buffer);

        void Close();
    }
}
