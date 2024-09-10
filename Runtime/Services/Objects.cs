using System;
using System.Collections.Generic;

namespace SeganX.Services
{
    public interface IAd
    {
        bool IsRewardedLoaded { get; }
        void Initialize(Action callback);
        void ShowRewarded(Action<bool> callback);
    }

    public interface IAnalytics
    {
        void Event(params string[] args);
        void LevelStart(params string[] args);
        void LevelEnd(bool isWinner, params string[] args);
        void Earned(string currency, int amount, string gate, string item);
        void Spent(string currency, int amount, string gate, string item);
        void Purchased(string currency, int amount, string itemType, string itemId, string cartType);
        string GetRemoteConfig(string key, string defaultValue);
    }

    public interface ILocalPush
    {
        string LastNotificationUserdata { get; }
        void Schedule(int delaySeconds, string title, string message, string userdata = null, string largIcon = "app_icon");
    }

    public interface IBilling
    {
        bool IsSupported { get; }
        void Initialize(string rsaKey, Action callback);
        void Purchase(string sku, int price, string payload, Action<bool, string> callback);
        void Consume(string sku, int price, string token, Action<bool> callback);
        void GetSkuDetails(string[] skus, Action<List<SkuDetails>> callback);
        void GetPurchases(Action<bool, List<PurchaseData>> callback);
    }

    public class PurchaseData
    {
        public string sku;
        public int price;
        public string token;
        public string payload;

        override public string ToString() => $"sku: {sku}, token: {token}, payload: {payload}";
    }

    public class SkuDetails
    {
        public string sku;
        public string title;
        public string price;
        public string description;

        override public string ToString() => $"sku: {sku}, title: {title}, price: {price}, description: {description}";

#if MYKET || BAZAAR || IRGOOGLE || SIPAPP
        public int ExtractPrice(int defaultValue)
        {
            if (string.IsNullOrEmpty(price)) return defaultValue;
            var label = ConvertPersianDigit(price);
            string temp = string.Empty;
            for (int i = 0; i < label.Length; i++)
                if (char.IsDigit(label[i]) || label[i] == '.')
                    temp += label[i];
            var result = temp.ToInt(defaultValue);
            return price.Contains("ریال") ? result / 10 : result;
        }

        private string ConvertPersianDigit(string label)
        {
            return label
                .Replace('٠', '0').Replace('١', '1').Replace('٢', '2').Replace('٣', '3').Replace('٤', '4')
                .Replace('٥', '5').Replace('٦', '6').Replace('٧', '7').Replace('٨', '8').Replace('٩', '9')
                .Replace('۰', '0').Replace('۱', '1').Replace('۲', '2').Replace('۳', '3').Replace('۴', '4')
                .Replace('۵', '5').Replace('۶', '6').Replace('۷', '7').Replace('۸', '8').Replace('۹', '9');
        }
#else
        public float ExtractPrice(float defaultValue)
        {
            if (string.IsNullOrEmpty(price)) return defaultValue;
            string temp = string.Empty;
            for (int i = 0; i < price.Length; i++)
                if (char.IsDigit(price[i]) || price[i] == '.')
                    temp += price[i];
            return temp.ToFloat(defaultValue);
        }
#endif
    }
}