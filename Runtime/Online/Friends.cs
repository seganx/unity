#if SX_ONLINE
using System.Collections.Generic;

namespace SeganX
{
    public static partial class Online
    {
        public static class Friends
        {
            public static void Add(string username, System.Action<bool, Friendship> callback)
            {
                var post = new AddRequest
                {
                    username = username
                };
                DownloadData("friends-add.php", post, callback);
            }

            public static void Get(System.Action<bool, List<Friendship>> callback)
            {
                DownloadData("friends-get.php", null, callback);
            }

            public static void PostData(string username, string data, System.Action<bool> callback)
            {
                var post = new PostDataRequest
                {
                    username = username,
                    data = data
                };
                DownloadData<string>("friends-post-data.php", post, (success, res) => callback(success));
            }

            public static void PickData(int id, System.Action<bool, string> callback)
            {
                var post = new PickDataRequest
                {
                    id = id
                };
                DownloadData("friends-post-data.php", post, callback);
            }

            //////////////////////////////////////////////////////
            /// HELPER CLASSES
            //////////////////////////////////////////////////////
            [System.Serializable]
            public class Friendship
            {
                public string id = string.Empty;
                public string username = string.Empty;
                public string nickname = string.Empty;
                public string status = string.Empty;
                public string avatar = string.Empty;
                public string level = string.Empty;
                public string data = string.Empty;
            }

            [System.Serializable]
            private class AddRequest
            {
                public string username = string.Empty;
            }

            [System.Serializable]
            private class PostDataRequest
            {
                public string username = string.Empty;
                public string data = string.Empty;
            }

            [System.Serializable]
            private class PickDataRequest
            {
                public int id = 0;
            }

        }
    }
}
#endif