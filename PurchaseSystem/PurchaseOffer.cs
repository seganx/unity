using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static class PurchaseOffer
    {
        private const int coolTimerId = -100001;
        private const int offerTimerId = -100002;
        private const int resourceTimerId = -100003;
        private const int purchaseTimerId = -100004;

        private static class Config
        {
            public static int startIndex = 4;
            public static int maxIndex = 2;
            public static int coolTime = 24 * 60 * 60;
            public static int minResource = 2400;
            public static int resourceTime = 3 * 60 * 60;
            public static int offerDuration = 24 * 60 * 60;
            public static int lastPurchaseTime = 5 * 24 * 60 * 60;

            public static int Index
            {
                get { return PlayerPrefsEx.GetInt("PurchaseOffer.Data.Index", startIndex); }
                set { PlayerPrefsEx.SetInt("PurchaseOffer.Data.Index", value); }
            }
        }

        public static int RemainedTime
        {
            get
            {
                // check if offer exist
                if (Online.Timer.Exist(offerTimerId))
                    return Online.Timer.GetRemainSeconds(offerTimerId, Config.offerDuration);
                else
                    return -1;
            }
        }

        public static void Setup(int startIndex, int maxIndex, int offerDurationSeconds, int cooltimeSeconds, int minResource, int minResourceSeconds, int lastPurchaseSeconds)
        {
            Config.maxIndex = maxIndex;
            Config.startIndex = startIndex;
            Config.coolTime = cooltimeSeconds;
            Config.minResource = minResource;
            Config.offerDuration = offerDurationSeconds;
            Config.resourceTime = minResourceSeconds;
            Config.lastPurchaseTime = lastPurchaseSeconds;
        }

        public static int GetOfferIndex(int resource)
        {
            if (IsTimeToShow(resource) == false)
                return -1;

            // check if offer exist
            if (Online.Timer.Exist(offerTimerId))
            {
                // current offer is still exist ?
                if (Online.Timer.GetRemainSeconds(offerTimerId, Config.offerDuration) > 0)
                    return Config.Index;

                // it seems that the player just did not purchased
                Online.Timer.Remove(offerTimerId);
                if (Config.Index == 0)
                {
                    Online.Timer.Remove(coolTimerId);
                    Online.Timer.Remove(resourceTimerId);
                    Online.Timer.Remove(purchaseTimerId);
                    return -1;
                }
                else
                {
                    Config.Index--;
                    return Config.Index;
                }
            }
            else
            {
                Online.Timer.Set(offerTimerId, Config.offerDuration);
                return Config.Index;
            }
        }

        public static void SetPurchaseResult(bool success)
        {
            if (success)
            {
                Online.Timer.Remove(coolTimerId);
                Online.Timer.Remove(resourceTimerId);
                Online.Timer.Set(purchaseTimerId, Config.lastPurchaseTime);

                if (Config.Index < Config.maxIndex)
                    Config.Index++;
            }
        }

        private static bool IsTimeToShow(int resource)
        {
            // check cool time
            if (Online.Timer.GetRemainSeconds(coolTimerId, Config.coolTime) > 0) return false;

            // check resource leaks
            if (Online.Timer.Exist(resourceTimerId))
            {
                if (Online.Timer.GetRemainSeconds(resourceTimerId, Config.resourceTime) <= 0)
                    return true;
            }
            else if (resource < Config.minResource)
            {
                Online.Timer.Set(resourceTimerId, Config.resourceTime);
            }

            // check last purchase
            if (Online.Timer.GetRemainSeconds(-100003, Config.lastPurchaseTime) <= 0)
                return true;

            return false;
        }
    }
}
