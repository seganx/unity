
namespace SeganX
{
    public static partial class Online
    {
        public static class Purchase
        {
            public enum Provider { Cafebazaar }

            [System.Serializable]
            private class Data
            {
                public int game_id = 0;
                public string sku = string.Empty;
                public string token = string.Empty;
            }

            // set profile data. pass null or empty parameters to ignore change
            public static void Validate(int gameId, Provider provider, string sku, string token, System.Action<bool, string> callback)
            {
                var post = new Data();
                post.game_id = gameId;
                post.sku = sku;
                post.token = token;
                switch (provider)
                {
                    case Provider.Cafebazaar:
                        {
                            var timeOut = Http.requestTimeout;
                            Http.requestTimeout = 90;
                            DownloadData<string>("validate-bazaar.php", post, (s, p) =>
                            {
                                Http.requestTimeout = timeOut;
                                callback(s, p);
                            });
                        }
                        break;
                    default: callback(true, Core.Salt); break;
                }
            }
        }
    }
}