
namespace SeganX
{
    public static partial class Online
    {
        public static class Profile
        {
            [System.Serializable]
            public class Data
            {
                public string username = string.Empty;
                public string password = string.Empty;
                public string nickname = string.Empty;
                public string status = string.Empty;
                public string datahash = string.Empty;
            }

            [System.Serializable]
            private class Nickname
            {
                public string nickname = string.Empty;
            }

            [System.Serializable]
            private class Status
            {
                public string status = string.Empty;
            }

            public static void Get(System.Action<bool, Data> callback)
            {
                DownloadData("profile-get.php", null, callback);
            }

            public static void SetNickname(string nickname, System.Action<bool> callback)
            {
                var post = new Nickname();
                post.nickname = nickname;
                DownloadData<string>("profile-set-nickname.php", post, (success, res) => callback(success));
            }

            public static void SetStatus(string status, System.Action<bool> callback)
            {
                var post = new Status();
                post.status = status;
                DownloadData<string>("profile-set-status.php", post, (success, res) => callback(success));
            }
        }
    }
}