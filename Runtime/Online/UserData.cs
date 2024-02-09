#if SX_ONLINE
namespace SeganX
{
    public static partial class Online
    {
        public static class Userdata
        {
            public static void Set(string privateData, string publicData, System.Action<bool> callback)
            {
                if (string.IsNullOrEmpty(privateData) && string.IsNullOrEmpty(publicData))
                {
                    callback(header.ContainsKey("token"));
                    return;
                }

                var post = new PostData
                {
                    private_data = privateData,
                    public_data = publicData
                };
                DownloadData<string>("data-set.php", post, (success, res) => callback(success));
            }

            public static void GetPrivate(System.Action<bool, string> callback)
            {
                DownloadData("data-get-private.php", null, callback);
            }


            public static void GetPublic(string username, System.Action<bool, string> callback)
            {
                var post = new Username { username = username };
                DownloadData("data-get-public.php", post, callback);
            }


            //////////////////////////////////////////////////////
            ///  HELPER CLASSES
            //////////////////////////////////////////////////////
            [System.Serializable]
            private class PostData
            {
                public string private_data = string.Empty;
                public string public_data = string.Empty;
            }

            [System.Serializable]
            private class Username
            {
                public string username = string.Empty;
            }
        }
    }
}
#endif