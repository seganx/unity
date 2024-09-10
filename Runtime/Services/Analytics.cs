namespace SeganX.Services
{
    public static class Analytics 
    {
        private static IAnalytics analytics = null;
        public static void InjectService(IAnalytics instance) => analytics = instance;

        public static void Event(params string[] args) => analytics?.Event(args);
        public static void LevelStart(params string[] args) => analytics?.LevelStart(args);
        public static void LevelEnd(bool isWinner, params string[] args) => analytics?.LevelEnd(isWinner, args);
        public static void Earned(string currency, int amount, string gate, string item) => analytics?.Earned(currency, amount, gate, item);
        public static void Spent(string currency, int amount, string gate, string item) => analytics?.Spent(currency, amount, gate, item);
        public static void Purchased(string currency, int amount, string itemType, string itemId, string cartType) => analytics?.Purchased(currency, amount, itemType, itemId, cartType);
        public static string GetRemoteConfig(string key, string defaultValue) => analytics != null ? analytics.GetRemoteConfig(key, defaultValue) : defaultValue;
    }
}