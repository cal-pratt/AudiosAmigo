using System;

namespace AudiosAmigo
{
    public interface INetworkCommunication : IDisposable
    {
        string Receive();

        byte[] ReceiveBytes();

        void Send(string text);

        void SendBytes(byte[] buffer);
    }
}
