using System.Security.Cryptography;

namespace AudiosAmigo
{
    public static class PasswordUtil
    {
        public static byte[] PbkdfHash(byte[] password, byte[] salt, int iterations, int length)
        {
            using (var deriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
            {
                return deriveBytes.GetBytes(length);
            }
        }
    }
}
