using System;
using System.Collections.Generic;

namespace SeganX.Services
{
    public static class Billing
    {
        private static IBilling billing = null;

        public static bool IsSupported => billing != null && billing.IsSupported;

        public static void InjectService(IBilling instance)
        {
            billing = instance;
        }

        public static void Initialize(string rsaKey, Action callback)
        {
            billing?.Initialize(rsaKey, callback);
        }

        public static void StartPurchase(string sku, int price, string payload, Action<bool, string> callback)
        {
            billing?.StartPurchase(sku, price, payload, (succeed, tokenOrTransactionId) =>
            {
                if (succeed)
                    callback?.SafeInvoke(true, tokenOrTransactionId);
                else
                    callback?.SafeInvoke(false, string.Empty);
            });

        }

        public static void EndPurchase(string tokenOrTransactionId, int price, Action<bool> callback)
        {
            billing?.Consume(tokenOrTransactionId, price, succeed => callback?.SafeInvoke(succeed));

        }

        public static void GetSkuDetails(string[] skus, Action<List<SkuDetails>> callback)
        {
            billing?.GetSkuDetails(skus, list => callback?.SafeInvoke(list));

        }


        public static void GetPurchases(Action<bool, List<PurchaseData>> callback)
        {
            billing?.GetPurchases((succeed, list) => callback?.SafeInvoke(succeed, list));
        }
    }
}