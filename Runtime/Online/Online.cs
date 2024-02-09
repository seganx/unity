#if SX_ONLINE
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static partial class Online
    {
        private static readonly Dictionary<string, string> header = new Dictionary<string, string>();

        public static string Error { get; private set; }
        public static bool IsLoggedin => header.ContainsKey("token");

        private static void DownloadData<T>(string uri, object post, System.Action<bool, T> callback, int timeout = 0, int efforts = 1)
        {
            Http.DownloadText($"http://{Core.OnlineDomain}/{uri}", post == null ? null : JsonUtility.ToJson(post), header, (status, resjson) =>
            {
                if (resjson != null)
                {
                    Response<T> res = null;
                    try
                    {
                        res = JsonUtility.FromJson<Response<T>>(resjson);
                    }
                    catch (System.Exception e)
                    {
                        Error = e.Message;
                        Debug.LogError(Error);
                    }

                    if (res != null)
                    {
                        if (res.msg != Message.ok)
                        {
                            Error = res.msg;
                            callback(false, res.data);
                        }
                        else callback(true, res.data);
                    }
                    else callback(false, default(T));
                }
                else
                {
                    Error = Message.TranslateHttp(status);
                    callback(false, default);
                }
            },
            null, timeout, efforts);
        }


        public static void GetTime(System.Action<bool, long> callback)
        {
            DownloadData<long>("time.php", null, callback);
        }

        public static void Login(System.Action<bool> callback)
        {
            Login(Core.GameId, Core.DeviceId, callback);
        }

        public static void Login(string gameId, string deviceId, System.Action<bool> callback)
        {
            var post = new LoginData
            {
                game_id = gameId,
                device_id = deviceId
            };

            DownloadData<string>("login.php", post, (success, res) =>
            {
                if (success)
                    header["token"] = res;
                callback(success);
            });
        }

        public static void TranserAccount(string username, string password, System.Action<bool> callback)
        {
            var post = new TransferData
            {
                username = username,
                password = password
            };

            DownloadData<string>("transfer.php", post, (success, res) =>
            {
                if (success)
                {
                    header["token"] = res;
                    Application.Quit();
                }
                callback(success);
            });
        }


        //////////////////////////////////////////////////////
        /// HELPER CLASSES
        //////////////////////////////////////////////////////
        [System.Serializable]
        private class Response<T>
        {
            public string msg = string.Empty;
            public T data = default(T);
        }

        [System.Serializable]
        private class LoginData
        {
            public string game_id = string.Empty;
            public string device_id = string.Empty;
        }

        [System.Serializable]
        private class TransferData
        {
            public string username = string.Empty;
            public string password = string.Empty;
        }
    }
}
#endif