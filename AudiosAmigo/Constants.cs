﻿namespace AudiosAmigo
{
    public static class Constants
    {
        public static int ServerBroadcastListenerPort => 12345;

        public static int ClientBroadcastListenerPort => 12346;

        public static int DefaultServerTcpListenerPort => 12347;

        public static string EncrpytionInitVector => "AudiosAmigoIVStr";

        public static string DefaultSessionPassword => "AudiosAmigoDefault";

        public static byte[] PasswordSalt => Translate.StringToByteArray("AudiosAmigoSalt");

        public static int PasswordIterations => 100;

        public static int PasswordLength => 50;
    }
}
