#if SX_ONLINE
using System.Collections.Generic;

namespace SeganX
{
    public static partial class Online
    {
        public static class League
        {
            public static void GetInfo(string name, System.Action<bool, LeagueInfo> callback)
            {
                var post = new PostName
                {
                    name = name
                };
                DownloadData("league-get-info.php", post, callback);
            }

            public static void GetLeaderboard(string name, int from, int count, System.Action<bool, Leaderboard> callback)
            {
                var post = new PostRange
                {
                    name = name,
                    from = from,
                    count = count
                };
                DownloadData("league-get-leaderboard.php", post, callback);
            }

            public static void SetScore(string name, int score, int value, string salt, System.Action<bool, int> callback)
            {
                var post = new PostScore
                {
                    name = name,
                    score = score,
                    value = value,
                    hash = $"seganx_{score}!&!{value}#({name}{salt}".ComputeMD5(string.Empty).ToLower()
                };
                DownloadData("league-set-score.php", post, callback);
            }

            public static void SetRewarded(string name, System.Action<bool> callback)
            {
                var post = new PostName
                {
                    name = name
                };
                DownloadData<string>("league-set-rewarded.php", post, (success, res) => callback(success));
            }

            //////////////////////////////////////////////////////
            ///  HELPER CLASSES
            //////////////////////////////////////////////////////
            [System.Serializable]
            public class LeagueInfo
            {
                public int score = 0;
                public int rank = 0;
                public int end_score = 0;
                public int end_rank = 0;
            }

            [System.Serializable]
            private class PostName
            {
                public string name = string.Empty;
            }

            [System.Serializable]
            private class PostScore : PostName
            {
                public int score = 0;
                public int value = 0;
                public string hash = string.Empty;
            }

            [System.Serializable]
            private class PostRange : PostName
            {
                public int from = 0;
                public int count = 0;
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
        }
    }
}
#endif