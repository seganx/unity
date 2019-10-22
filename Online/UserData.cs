
namespace SeganX
{
    public static partial class Online
    {
        public static class Userdata
        {
            [System.Serializable]
            private class Data
            {
                public string hash = string.Empty;
                public string private_data = string.Empty;
                public string public_data = string.Empty;
            }

            [System.Serializable]
            private class Username
            {
                public string username = string.Empty;
            }

            public static void Set(string hash, string privateData, string publicData, System.Action<bool> callback)
            {
                var post = new Data();
                post.hash = hash;
                post.private_data = privateData;
                post.public_data = publicData;
                DownloadData<string>("data-set.php", post, (success, res) => callback(success));
            }

            public static void Get(System.Action<bool, string, string> callback)
            {
                DownloadData<Data>("data-get.php", null, (success, res) => callback(success, res.private_data, res.public_data));
            }

            public static void GetPublic(string username, System.Action<bool, string> callback)
            {
                var post = new Username();
                post.username = username;
                DownloadData<string>("data-get-public.php", post, callback);
            }
        }
    }
}