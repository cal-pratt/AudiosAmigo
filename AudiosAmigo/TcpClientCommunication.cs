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
            return Translate.ByteArrayToString(ReceiveBytes());
        }

        public byte[] ReceiveBytes()
        {
            var sizeBytes = new byte[4];
            if (_stream.Read(sizeBytes, 0, sizeBytes.Length) == -1)
            {
                return new byte[] {};
            }
            var size = BitConverter.ToInt32(sizeBytes, 0);
            if (size <= 0)
            {
                return new byte[] {};
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
            return bytes;
        } 

        public void Send(string text)
        {
            SendBytes(Translate.StringToByteArray(text));
        }

        public void SendBytes(byte[] buffer)
        {
            var sizeBytes = BitConverter.GetBytes(buffer.Length);
            _stream.Write(sizeBytes, 0, sizeBytes.Length);
            _stream.Write(buffer, 0, buffer.Length);
        }

        public void Close()
        {
            _stream.Close();
        }
    }
}
