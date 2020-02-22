﻿
namespace SeganX
{
    public static partial class Online
    {
        public static class Stats
        {
            [System.Serializable]
            private class Data
            {
                public int gems = 0;
                public int skill = 0;
                public int level = 0;
            }

            public static void Set(int gems, int skill, int level, System.Action<bool> callback)
            {
                var post = new Data();
                post.gems = gems;
                post.skill = skill;
                post.level = level;
                DownloadData<string>("stats-set.php", post, (success, res) => callback(success));
            }
        }
    }
}