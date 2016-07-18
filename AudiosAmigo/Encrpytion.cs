using System.Security.Cryptography;

namespace AudiosAmigo
{
    public class Encrpytion
    {
        private readonly ICryptoTransform _decryptor;

        private readonly ICryptoTransform _encryptor;

        public Encrpytion(string password, string iv)
        {
            var md5 = new MD5CryptoServiceProvider();
            var passwordHash = md5.ComputeHash(Translate.StringToByteArray(password));
            var ivBytes = Translate.StringToByteArray(iv);

            var cipher = new RijndaelManaged();
            _decryptor = cipher.CreateDecryptor(passwordHash, ivBytes);
            _encryptor = cipher.CreateEncryptor(passwordHash, ivBytes);
        }

        public byte[] Decrypt(byte[] buffer)
        {
            return _decryptor.TransformFinalBlock(buffer, 0, buffer.Length);
        }

        public byte[] Encrypt(byte[] buffer)
        {
            return _encryptor.TransformFinalBlock(buffer, 0, buffer.Length);
        }
    }
}