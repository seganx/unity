using System.Collections.Generic;

namespace SeganX
{
    public static partial class Online
    {
        public static class League
        {
            [System.Serializable]
            public class Data
            {
                public long start_time = 0;
                public long duration = 1;
                public int score = 0;
                public int rank = 0;
                public int end_score = 0;
                public int end_rank = 0;
            }

            [System.Serializable]
            public class Profile
            {
                public string username = string.Empty;
                public string nickname = string.Empty;
                public string status = string.Empty;
                public string avatar = string.Empty;
                public int score = 0;
                public int rank = 0;
            }

            [System.Serializable]
            public class Leaderboard
            {
                public List<Profile> last = new List<Profile>();
                public List<Profile> current = new List<Profile>();
            }

            [System.Serializable]
            private class Id
            {
                public int id = 0;
            }

            [System.Serializable]
            private class LeagueScore : Id
            {
                public int score = 0;
                public int value = 0;
                public string hash = string.Empty;
            }

            [System.Serializable]
            private class LeaderboardRequest : Id
            {
                public int from = 0;
                public int count = 0;
            }

            private class Cache<T>
            {
                public int id = 0;
                public System.DateTime cacheTime = new System.DateTime();
                public T data = default(T);
            }

            private static int cacheDataDuration = 30;
            private static List<Cache<Data>> cacheData = new List<Cache<Data>>();
            private static int cacheLeaderboardDuration = 300;
            private static List<Cache<Leaderboard>> cacheLeaderboard = new List<Cache<Leaderboard>>();

            public static void SetScore(int id, int score, int value, string hash, System.Action<bool, int> callback)
            {
                var post = new LeagueScore();
                post.id = id;
                post.score = score;
                post.value = value;
                post.hash = hash;
                DownloadData<int>("league-set-score.php", post, callback);
            }

            public static void SetRewarded(int id, System.Action<bool> callback)
            {
                var post = new Id();
                post.id = id;
                DownloadData<string>("league-set-rewarded.php", post, (success, res) => callback(success));
            }

            public static void GetData(int id, System.Action<bool, Data> callback)
            {
                var cache = GetDataFromCache(id);
                if (cache == null)
                {
                    var post = new Id();
                    post.id = id;
                    DownloadData<Data>("league-get.php", post, (success, data) => callback(success, AddToChache(id, data)));
                }
                else callback(true, cache);

            }

            public static void GetLeaderboard(int id, int from, int count, System.Action<bool, Leaderboard> callback)
            {
                var cache = GetLeaderboardFromCache(id);
                if (cache == null)
                {
                    var post = new LeaderboardRequest();
                    post.id = id;
                    post.from = from;
                    post.count = count;
                    DownloadData<Leaderboard>("league-get-leaderboard.php", post, (success, data) => callback(success, AddToChache(id, data)));
                }
                else callback(true, cache);
            }

            public static long GetRemainedSeconds(long startSeconds, long duration, long currSeconds)
            {
                var delta = currSeconds - startSeconds;
                return duration - (delta % duration);
            }

            public static void SetCacheDuration(int data, int leaderboard)
            {
                cacheDataDuration = data;
                cacheLeaderboardDuration = leaderboard;
            }

            public static void ClearCache()
            {
                cacheData.Clear();
                cacheLeaderboard.Clear();
            }

            private static System.DateTime UnixTimeToLocalTime(long date)
            {
                var res = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                res = res.AddSeconds(date);
                return res.ToLocalTime();
            }

            private static Data AddToChache(int id, Data data)
            {
                if (data == null) return data;
                data.start_time = UnixTimeToLocalTime(data.start_time).Ticks / System.TimeSpan.TicksPerSecond;
                var res = new Cache<Data>();
                res.id = id;
                res.cacheTime = System.DateTime.Now;
                res.data = data;
                cacheData.Add(res);
                return data;
            }

            private static Data GetDataFromCache(int id)
            {
                var res = cacheData.Find(x => x.id == id);
                if (res == null) return null;
                if ((System.DateTime.Now - res.cacheTime).TotalSeconds > cacheDataDuration)
                {
                    cacheData.Remove(res);
                    return null;
                }
                return res.data;
            }

            private static Leaderboard AddToChache(int id, Leaderboard data)
            {
                if (data == null) return data;
                var res = new Cache<Leaderboard>();
                res.id = id;
                res.cacheTime = System.DateTime.Now;
                res.data = data;
                cacheLeaderboard.Add(res);
                return data;
            }

            private static Leaderboard GetLeaderboardFromCache(int id)
            {
                var res = cacheLeaderboard.Find(x => x.id == id);
                if (res == null) return null;
                if ((System.DateTime.Now - res.cacheTime).TotalSeconds > cacheLeaderboardDuration)
                {
                    cacheLeaderboard.Remove(res);
                    return null;
                }
                return res.data;
            }
        }
    }
}