using System;
using System.Text;
using Newtonsoft.Json;

namespace AudiosAmigo
{
    public static class Translate
    {
        public static string ByteArrayToString(byte[] bytes)
        {
            var chars = new char[bytes.Length];
            Encoding.UTF8.GetDecoder().GetChars(bytes, 0, bytes.Length, chars, 0);
            return new string(chars);
        }

        public static byte[] StringToByteArray(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        public static string ByteArrayToBase64String(byte[] bytes)
        {
            return Convert.ToBase64String(bytes);
        }

        public static byte[] Base64StringToByteArray(string str)
        {
            return Convert.FromBase64String(str);
        }

        public static string ObjectToString<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T StringToObject<T>(string str)
        {
            var obj = JsonConvert.DeserializeObject<T>(str);
            return obj;
        }

        public static byte[] ObjectToByteArray<T>(T obj)
        {
            return StringToByteArray(ObjectToString(obj));
        }

        public static T ByteArrayToObject<T>(byte[] bytes)
        {
            return StringToObject<T>(ByteArrayToString(bytes));
        }
    }
}
