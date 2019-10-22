namespace SeganX
{
    public static partial class Online
    {
        public static class Message
        {
            public const string ok = "ok";
            public const string unknown = "unknown";
            public const string net_inaccessable = "net_inaccessable";
            public const string server_maintenance = "server_maintenance";
            public const string invalid_token = "invalid_token";
            public const string invalid_params = "invalid_params";
            public const string account_transfered = "account_transfered";

            internal static string TranslateHttp(Http.Status status)
            {
                switch (status)
                {
                    case Http.Status.NetworkError: return net_inaccessable;
                    case Http.Status.ServerError: return server_maintenance;
                }
                return unknown;
            }
        }
    }
}
