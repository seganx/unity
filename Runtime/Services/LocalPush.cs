namespace SeganX.Services
{
    public static class LocalPush
    {
        private static ILocalPush localPush = null;

        // Allows retrieving the notification used to open the app
        public static string LastNotificationUserdata => localPush?.LastNotificationUserdata;

        // Schedule a new notification
        public static void Schedule(int delaySeconds, string title, string message, string userdata = null, string largIcon = "app_icon") => localPush.Schedule(delaySeconds, title, message, userdata, largIcon);

        public static void InjectService(ILocalPush instance) => localPush = instance;
    }

} // namespace