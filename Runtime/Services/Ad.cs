using System;

namespace SeganX.Services
{
    public static class Ad
    {
        private static IAd advertise = null;

        public static bool IsInitialized { get; private set; } = false;
        public static bool IsRewardedLoaded => advertise != null && advertise.IsRewardedLoaded;

        public static void InjectService(IAd instance)
        {
            advertise = instance;
        }

        public static void Initialize()
        {
            advertise?.Initialize(() => IsInitialized = true);
        }

        public static void ShowRewarded(Action<bool> callback)
        {
            if (advertise == null)
                callback?.Invoke(false);
            else
                advertise.ShowRewarded(callback);
        }

        public static void ShowInterstitial(Action callback)
        {
            if (advertise == null)
                callback?.Invoke();
            else
                advertise.ShowInterstitial(callback);
        }

        public static void SetBanner(bool banner)
        {
            advertise?.SetBanner(banner);
        }
    }
}