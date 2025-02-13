using System;
using System.Collections.Generic;

namespace SeganX.Services
{
    public interface IAd
    {
        bool IsRewardedLoaded { get; }
        void Initialize(Action callback);
        void SetBanner(bool banner);
        void ShowRewarded(Action<bool> callback);
        void ShowInterstitial(Action callback);
    }

    public interface IAnalytics
    {
        void Event(params string[] args);
        void LevelStart(params string[] args);
        void LevelEnd(bool isWinner, params string[] args);
        void Earned(string currency, int amount, string gate, string item);
        void Spent(string currency, int amount, string gate, string item);
        void Purchased(string currency, int amount, string itemType, string itemId, string cartType);
        void GameAnalyticsEvent(string eventId, float value);
        string GetRemoteConfig(string key, string defaultValue);
        string GetAbTestVariantId(string defaultValue);
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
        void StartPurchase(string sku, int price, string payload, Action<bool, string> callback);
        void FinishPurchase(string sku, int price, string token, Action<bool> callback);
        void GetSkuDetails(string[] skus, Action<List<SkuDetails>> callback);
        void GetPurchases(Action<bool, List<PurchaseData>> callback);
    }

    public class PurchaseData
    {
        public string sku;
        public int price;
        public string token;
        public string payload;

        override public string ToString() => $"sku:{sku}, price:{price}, token:{token}, payload:{payload}";
    }

    public class SkuDetails
    {
        public string sku;
        public string title;
        public string description;
        public string priceFormatted;
        public string priceCurrency;
        public float priceAmount;

        override public string ToString() => $"sku:{sku}, title:{title}, priceAmount:{priceAmount}, priceFormatted:{priceFormatted}, priceCurrency:{priceCurrency}, description:{description}";
    }
}