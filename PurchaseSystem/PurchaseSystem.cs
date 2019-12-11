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

        public static void Consume(string sku)
        {
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
                    if (res.DeveloperPayload == Core.Salt)
                        Online.Purchase.End(Online.Purchase.Provider.Cafebazaar, res.ProductId, res.PurchaseToken, (success, payload) => callbackCaller.Call(success && payload == Core.Salt, res.PurchaseToken));
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
                };

                BazaarPlugin.IABEventManager.consumePurchaseFailedEvent = (error) =>
                {
                    Debug.LogError("Consume failed: " + error);
                };

                BazaarPlugin.BazaarIAB.init(key);
            }

            public static void Purchase(string sku)
            {
                Debug.Log("Purchase started for " + sku);
                BazaarPlugin.BazaarIAB.purchaseProduct(sku, Core.Salt);
            }

            public static void Consume(string sku)
            {
                BazaarPlugin.BazaarIAB.consumeProduct(sku);
            }
        }
#endif

    }
}