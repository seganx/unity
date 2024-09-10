using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

namespace SeganX.Services
{
    public static class Billing
    {
        private static IBilling billing = null;
        private static volatile bool isOpenToRequest = false;
        private static List<PurchaseData> purchased = new List<PurchaseData>();

        private static bool IsBillingSupported => billing != null && billing.IsSupported;

        public static Func<string, int, string> OnGetPayload = (sku, number) => $"{sku}|{number}|{Application.version}";

        private static int Number
        {
            get => PlayerPrefs.GetInt("Billing.Number", 0);
            set => PlayerPrefs.SetInt("Billing.Number", value);
        }

        public static void InjectService(IBilling instance)
        {
            billing = instance;
        }

        public static void Initialize(string rsaKey, Action callback)
        {
            if (billing != null)
            {
                billing.Initialize(rsaKey, () =>
                {
                    isOpenToRequest = true;
                    callback?.Invoke();
                });
            }
            else
            {
                isOpenToRequest = true;
                callback?.Invoke();
            }
        }

        public static void Purchase(string sku, int price, Action<bool, string> callback)
        {
#if UNITY_EDITOR
            if (IsBillingSupported == false)
            {
                callback?.SafeInvoke(true, "editor.token");
                return;
            }
#endif
            var number = Number + 1;
            var payload = OnGetPayload.Invoke(sku, number);
            if (IsBillingSupported)
            {
                billing?.Purchase(sku, price, payload, (succeed, tokenString) =>
                {
                    if (succeed)
                    {
                        Number = number;
                        purchased.Add(new PurchaseData() { sku = sku, price = price, payload = payload, token = tokenString });
                        callback?.SafeInvoke(true, tokenString);
                    }
                    else callback?.SafeInvoke(false, string.Empty);
                });
            }
            else
            {
                Payment.Purchase(sku, price * 10, payload, (succeed, tokenString) =>
                {
                    if (succeed)
                    {
                        Number = number;
                        purchased.Add(new PurchaseData() { sku = sku, price = price, payload = payload, token = tokenString });
                        callback?.SafeInvoke(true, tokenString);
                    }
                    else callback?.SafeInvoke(false, string.Empty);
                });
            }
        }

        public static void Consume(string sku, int price, Action<bool> callback)
        {
#if UNITY_EDITOR_1
            if (IsBillingSupported == false)
            {
                callback?.SafeInvoke(false);
                return;
            }
#endif

            purchased.RemoveAll(x => string.IsNullOrEmpty(x.token));
            var item = purchased.Find(x => x.sku == sku);
            if (item == null) return;

            if (IsBillingSupported)
            {
                billing?.Consume(sku, price, item.token, succeed =>
                {
                    if (succeed)
                        purchased.Remove(item);
                    callback?.SafeInvoke(succeed);
                });
            }
            else
            {
                Payment.Consume(sku, succeed =>
                {
                    if (succeed)
                    {
                        Online.Purchased(item.token, sku, price);
                        purchased.Remove(item);
                    }
                    callback?.SafeInvoke(succeed);
                });
            }
        }

        public static void GetSkuDetails(string[] skus, Action<List<SkuDetails>> callback)
        {
            if (IsBillingSupported)
                AsyncGetSkuDetails(skus, callback);
            else
                callback?.Invoke(new List<SkuDetails>());
        }

        private static async void AsyncGetSkuDetails(string[] skus, Action<List<SkuDetails>> callback)
        {
            while (isOpenToRequest == false)
                await Task.Yield();
            isOpenToRequest = false;

            billing?.GetSkuDetails(skus, list =>
            {
                callback?.SafeInvoke(list);
                isOpenToRequest = true;
            });
        }


        public static void GetPurchases(Action<bool, List<PurchaseData>> callback)
        {
            if (IsBillingSupported)
            {
                billing?.GetPurchases((succeed, list) =>
                {
                    purchased = list;
                    callback?.SafeInvoke(succeed, list);
                });
            }
            else
            {
                Payment.Inventory((succeed, list) =>
                {
                    purchased = new List<PurchaseData>();
                    if (succeed && list != null)
                    {
                        foreach (var item in list)
                        {
                            purchased.Add(new PurchaseData() { sku = item.sku, token = item.Token });
                        }
                    }
                    callback?.SafeInvoke(succeed, purchased);
                });
            }
        }
    }
}