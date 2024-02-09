#if SX_IAP
namespace SeganX
{
    public static class PurchaseOffer
    {
        private const int firstDelayTimerId = -200000;
        private const int resourceTimerId = -200003;
        private const int offerTimerId = -200002;
        private const int coolTimerId = -200001;
        private const int purchaseTimerId = -200004;

        private static class Config
        {
            public static int startIndex = 4;
            public static int maxIndex = 2;
            public static int firstDelay = 24 * 60 * 60;
            public static int minResource = 2400;
            public static int resourceTime = 3 * 60 * 60;
            public static int offerDuration = 24 * 60 * 60;
            public static int coolTime = 48 * 60 * 60;
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

        public static void Setup(int startIndex, int count, int firstDelaySeconds, int offerDurationSeconds, int cooltimeSeconds, int minResource, int minResourceSeconds, int lastPurchaseSeconds)
        {
            Config.maxIndex = count - 1;
            Config.startIndex = startIndex;
            Config.firstDelay = firstDelaySeconds;
            Config.minResource = minResource;
            Config.offerDuration = offerDurationSeconds;
            Config.resourceTime = minResourceSeconds;
            Config.coolTime = cooltimeSeconds;
            Config.lastPurchaseTime = lastPurchaseSeconds;
        }

        public static int GetOfferIndex(int resource)
        {
            // check if offer exist
            if (Online.Timer.Exist(offerTimerId))
            {
                // current offer is still exist ?
                if (Online.Timer.GetRemainSeconds(offerTimerId, Config.offerDuration) > 0)
                    return Config.Index;

                // it seems that the player just did not purchased
                Online.Timer.Remove(offerTimerId);
                Online.Timer.Remove(resourceTimerId);
                Online.Timer.Set(coolTimerId, Config.coolTime);

                if (Config.Index > 0)
                    Config.Index--;
            }

            if (IsTimeToShow(resource))
            {
                Online.Timer.Set(offerTimerId, Config.offerDuration);
                return Config.Index;
            }

            return -1;
        }

        public static void SetPurchaseResult(bool success)
        {
            if (success)
            {
                Online.Timer.Remove(offerTimerId);
                Online.Timer.Remove(resourceTimerId);
                Online.Timer.Set(coolTimerId, Config.coolTime);
                Online.Timer.Set(purchaseTimerId, Config.lastPurchaseTime);

                if (Config.Index < Config.maxIndex)
                    Config.Index++;
            }
        }

        private static bool IsTimeToShow(int resource)
        {
            // check cool time
            if (Online.Timer.GetRemainSeconds(firstDelayTimerId, Config.firstDelay) > 0)
                return false;

            // check cool time
            if (Online.Timer.Exist(coolTimerId) && Online.Timer.GetRemainSeconds(coolTimerId, Config.coolTime) > 0)
                return false;

            // check resource leaks
            if (Online.Timer.Exist(resourceTimerId))
            {
                if (Online.Timer.GetRemainSeconds(resourceTimerId, Config.resourceTime) <= 0)
                    return true;
            }
            else if (resource < Config.minResource)
            {
                Online.Timer.Set(resourceTimerId, Config.resourceTime);
                return false;
            }

            // check last purchase
            if (Online.Timer.GetRemainSeconds(purchaseTimerId, Config.lastPurchaseTime) <= 0)
                return true;

            return false;
        }
    }
}
#endif