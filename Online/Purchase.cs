using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static partial class Online
    {
        public static class Purchase
        {
            public enum Provider { Cafebazaar, Gateway }

            [System.Serializable]
            private class StartPost
            {
                public int game_id = 0;
                public string provider = string.Empty;
            }

            [System.Serializable]
            private class BazaarValidation
            {
                public string developerPayload = string.Empty;
                public long purchaseTime = 0;
            }

            private static string bazaar_access_token
            {
                get { return PlayerPrefsEx.GetString("Online.Purchase.bazaar_access_token", string.Empty); }
                set { PlayerPrefsEx.SetString("Online.Purchase.bazaar_access_token", value); }
            }

            public static void Start(Provider provider, System.Action callback)
            {
                var post = new StartPost();
                post.game_id = Core.GameId;

                if (provider == Provider.Cafebazaar)
                {
                    post.provider = "bazaar";
                    DownloadData<string>("purchase-start.php", post, (succeed, token) =>
                    {
                        if (succeed) bazaar_access_token = token;
                        callback();
                    });
                }
                else if (provider == Provider.Gateway)
                {

                }
            }

            public static void End(Provider provider, string sku, string token, System.Action<bool, string> callback)
            {
                if (provider == Provider.Cafebazaar)
                {
                    Verify(provider, sku, token, callback);
                }
                else if (provider == Provider.Gateway)
                {

                }
            }

            public static void Verify(Provider provider, string sku, string token, System.Action<bool, string> callback)
            {
                if (provider == Provider.Cafebazaar)
                {
                    if (bazaar_access_token.HasContent())
                    {
                        var url = "https://pardakht.cafebazaar.ir/devapi/v2/api/validate/" + Application.identifier + "/inapp/" + sku + "/purchases/" + token + "/";
                        var tmp = new Dictionary<string, string>();
                        tmp.Add("Authorization", bazaar_access_token);

                        var timeOut = Http.requestTimeout;
                        Http.requestTimeout = 90;
                        Http.DownloadText(url, null, tmp, resjson =>
                        {
                            Http.requestTimeout = timeOut;
                            var res = JsonUtility.FromJson<BazaarValidation>(resjson);
                            callback(res.purchaseTime > 0, res.developerPayload);
                        });
                    }
                    else
                    {
                        callback(true, Core.Salt);
                    }
                }
                else if (provider == Provider.Gateway)
                {
                    
                }
            }

        }
    }
}