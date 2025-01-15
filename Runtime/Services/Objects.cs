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
        void StartPurchase(string sku, int price, string payload, Action<bool, string> callback);
        void Consume(string token, int price, Action<bool> callback);
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
    }
}
