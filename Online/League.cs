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
                public long duration = 0;
                public int score = 0;
                public int position = 0;
                public int reward_score = 0;
                public int reward_position = 0;
            }

            [System.Serializable]
            public class Profile
            {
                public string username = string.Empty;
                public string nickname = string.Empty;
                public string status = string.Empty;
                public string avatar = string.Empty;
                public string score = string.Empty;
                public string position = string.Empty;
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
            private class Leaderboard : Id
            {
                public int from = 0;
                public int count = 0;
            }

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
                var post = new Id();
                post.id = id;
                DownloadData("league-get.php", post, callback);
            }

            public static void GetLeaderboard(int id, int from, int count, System.Action<bool, List<Profile>> callback)
            {
                var post = new Leaderboard();
                post.id = id;
                post.from = from;
                post.count = count;
                DownloadData("league-get-leaderboard.php", post, callback);
            }
        }
    }
}