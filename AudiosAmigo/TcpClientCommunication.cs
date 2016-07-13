using System;
using System.Net.Sockets;

namespace AudiosAmigo
{
    public class TcpClientCommunication : INetworkCommunication
    {
        private readonly NetworkStream _stream;

        public TcpClientCommunication(TcpClient client)
        {
            client.NoDelay = true;
            _stream = client.GetStream();
        }

        public string Receive()
        {
            var sizeBytes = new byte[4];
            if (_stream.Read(sizeBytes, 0, sizeBytes.Length) == -1)
            {
                return "";
            }
            var size = BitConverter.ToInt32(sizeBytes, 0);
            if (size <= 0)
            {
                return "";
            }
            var bytes = new byte[size];
            var bytesRemaining = size;
            var offset = 0;
            while (bytesRemaining > 0)
            {
                var readCount = _stream.Read(bytes, offset, bytesRemaining);
                if (readCount == -1)
                {
                    readCount = _stream.Read(bytes, offset, bytesRemaining);
                    if (readCount == -1)
                    {
                        break;
                    }
                }
                offset += readCount;
                bytesRemaining -= readCount;
            }
            return Translate.ByteArrayToString(bytes);
        }

        public void Close()
        {
            _stream.Close();
        }

        public void Send(string buffer)
        {
            var sizeBytes = BitConverter.GetBytes(buffer.Length);
            _stream.Write(sizeBytes, 0, sizeBytes.Length);
            var bytes = Translate.StringToByteArray(buffer);
            _stream.Write(bytes, 0, bytes.Length);
        }
    }
}
