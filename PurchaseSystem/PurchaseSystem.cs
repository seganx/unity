using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public enum PurchaseProvider : int
    {
        Null = 0,
        Bazaar = 1,
        Gateway = 3
    }

    public class PurchaseSystem : MonoBehaviour
    {
        public delegate void Callback(bool success, string msg);

        private class CallbackCaller
        {
            public bool invoke = false;
            public Callback callbackFunc = null;
            public bool success = false;
            public string msg = string.Empty;

            public void Setup(Callback callback)
            {
                invoke = false;
                callbackFunc = callback;
                success = false;
                msg = string.Empty;
            }

            public void Call(bool result, string resmsg)
            {
                success = result;
                msg = resmsg;
                invoke = true;
            }
        }

        public void Update()
        {
            if (callbackCaller.invoke)
            {
                callbackCaller.invoke = false;
                if (callbackCaller.callbackFunc != null)
                    callbackCaller.callbackFunc(callbackCaller.success, callbackCaller.msg);
            }
        }


        ////////////////////////////////////////////////////////
        /// STATIC MEMBER
        ////////////////////////////////////////////////////////
        private static PurchaseProvider purchaseProvider = PurchaseProvider.Null;
        private static CallbackCaller callbackCaller = new CallbackCaller();

        private static string StoreUrl { get; set; }
        public static bool IsInitialized { get; private set; }
        public static PurchaseProvider LastProvider { get { return purchaseProvider; } }

        static PurchaseSystem()
        {
            DontDestroyOnLoad(Game.Instance.gameObject.AddComponent<PurchaseSystem>());
        }

        public static void Initialize(string bazaarKey, string storeUrl, Callback callback)
        {
            StoreUrl = storeUrl;
            if (IsInitialized == false)
            {
                callbackCaller.Setup(callback);
#if BAZAAR
                Bazaar.Initialize(bazaarKey);
#endif
            }
            else callback(true, string.Empty);
        }

        public static void Purchase(PurchaseProvider provider, string sku, Callback callback)
        {
            purchaseProvider = provider;
            callbackCaller.Setup(callback);

#if UNITY_EDITOR
            provider = PurchaseProvider.Null;
#endif

            switch (provider)
            {
#if BAZAAR
                case PurchaseProvider.Bazaar:
                    if (Bazaar.Supported)
                    {
                        Online.Purchase.Start(Online.Purchase.Provider.Cafebazaar, () => Bazaar.Purchase(sku));
                    }
                    else
                    {
                        Application.OpenURL(StoreUrl);
                        callback(false, "Bazaar Not Supported!");
                    }
                    break;
#endif
                case PurchaseProvider.Gateway:
                    callback(false, "faketoken");
                    break;

#if UNITY_EDITOR
                default:
                    callback(true, "faketoken");
                    break;
#endif
            }

        }

        public static void Consume(string sku, Callback callback)
        {
            callbackCaller.Setup(callback);

            switch (purchaseProvider)
            {
                case PurchaseProvider.Bazaar:
#if BAZAAR
                    Bazaar.Consume(sku);
#endif
                    break;

                case PurchaseProvider.Gateway:
                    break;
            }

        }


        ////////////////////////////////////////////////////////////////////////////////////////////
        //  implementations
        ////////////////////////////////////////////////////////////////////////////////////////////
        public static class Payload
        {
            private static List<string> list = new List<string>();

            static Payload()
            {
                list = PlayerPrefsEx.GetObject("PurchaseSystem.Payload.list", new List<string>());
            }

            public static string Get(string salt)
            {
                var res = System.DateTime.Now.Ticks.ToString().ComputeMD5(salt);
                list.Add(res);
                PlayerPrefsEx.SetObject("PurchaseSystem.Payload.list", list);
                return res;
            }

            public static bool IsValid(string payload)
            {
                return list.IndexOf(payload) >= 0;
            }

            public static bool Remove(string payload)
            {
                var res = IsValid(payload);
                list.Remove(payload);
                PlayerPrefsEx.SetObject("PurchaseSystem.Payload.list", list);
                return res;
            }
        }

#if BAZAAR
        private static class Bazaar
        {
            public static bool Supported { get; private set; }

            public static void Initialize(string key)
            {
                Supported = true;

                BazaarPlugin.IABEventManager.billingSupportedEvent = () => callbackCaller.Call(Supported = true, string.Empty);
                BazaarPlugin.IABEventManager.billingNotSupportedEvent = (error) => callbackCaller.Call(Supported = false, error);

                BazaarPlugin.IABEventManager.purchaseSucceededEvent = (res) =>
                {
                    Debug.Log("Verifying purchase: " + res);
                    if (Payload.IsValid(res.DeveloperPayload))
                        Online.Purchase.End(Online.Purchase.Provider.Cafebazaar, res.ProductId, res.PurchaseToken, (success, payload) => callbackCaller.Call(success && payload == res.DeveloperPayload, res.PurchaseToken));
                    else
                        callbackCaller.Call(false, res.PurchaseToken);
                };

                BazaarPlugin.IABEventManager.purchaseFailedEvent = (error) =>
                {
                    Debug.LogError("Purachse Failed: " + error);
                    callbackCaller.Call(false, error);
                };

                BazaarPlugin.IABEventManager.consumePurchaseSucceededEvent = (res) =>
                {
                    Debug.Log("Consume succeeded: " + res);
                    callbackCaller.Call(Payload.Remove(res.DeveloperPayload), res.PurchaseToken);
                };

                BazaarPlugin.IABEventManager.consumePurchaseFailedEvent = (error) =>
                {
                    Debug.LogError("Consume failed: " + error);
                    callbackCaller.Call(false, error);
                };

                BazaarPlugin.BazaarIAB.init(key);
            }

            public static void Purchase(string sku)
            {
                Debug.Log("Purchase started for " + sku);
                BazaarPlugin.BazaarIAB.purchaseProduct(sku, Payload.Get(Core.Salt));
            }

            public static void Consume(string sku)
            {
                BazaarPlugin.BazaarIAB.consumeProduct(sku);
            }
        }
#endif

    }
}