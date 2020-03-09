using System.Collections.Generic;

namespace SeganX
{
    public static partial class Online
    {
        public static class Friends
        {
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

            private class Cache<T>
            {
                public System.DateTime cacheTime = new System.DateTime();
                public T data = default(T);
            }

            private static int cacheFriendsDuration = 300;
            private static Cache<List<Friendship>> cacheFriends = new Cache<List<Friendship>>();

            public static void Add(string username, System.Action<bool, Friendship> callback)
            {
                var post = new AddRequest();
                post.username = username;
                DownloadData("friends-add.php", post, callback);
                ClearCache();
            }

            public static void Get(System.Action<bool, List<Friendship>> callback)
            {
                var cache = GetFromCache();
                if (cache == null)
                    DownloadData<List<Friendship>>("friends-get.php", null, (success, data) => callback(success, success ? AddToChache(data) : new List<Friendship>()));
                else
                    callback(true, cache);
            }

            public static void PostData(string username, string data, System.Action<bool> callback)
            {
                var post = new PostDataRequest();
                post.username = username;
                post.data = data;
                DownloadData<string>("friends-post-data.php", post, (success, res) => callback(success));
            }

            public static void PickData(int id, System.Action<bool, string> callback)
            {
                var post = new PickDataRequest();
                post.id = id;
                DownloadData("friends-post-data.php", post, callback);
            }

            public static void SetCacheDuration(int seconds)
            {
                cacheFriendsDuration = seconds;
            }

            public static void ClearCache()
            {
                cacheFriends = new Cache<List<Friendship>>();
            }

            private static List<Friendship> AddToChache(List<Friendship> data)
            {
                if (data == null) return data;
                cacheFriends.cacheTime = System.DateTime.Now;
                cacheFriends.data = data;
                return data;
            }

            private static List<Friendship> GetFromCache()
            {
                if ((System.DateTime.Now - cacheFriends.cacheTime).TotalSeconds > cacheFriendsDuration)
                    return null;
                return cacheFriends.data;
            }
        }
    }
}