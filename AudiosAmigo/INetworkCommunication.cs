namespace AudiosAmigo
{
    public interface INetworkCommunication
    {
        void Send(string buffer);

        string Receive();

        void Close();
    }
}
