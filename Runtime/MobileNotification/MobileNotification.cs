using UnityEngine;

#if UNITY_ANDROID
using Unity.Notifications.Android;
#elif UNITY_IOS
using System.Collections;
using Unity.Notifications.iOS;
#endif

namespace SeganX
{
    public class MobileNotification : MonoBehaviour
    {
        [System.Serializable]
        public class UserData
        {
            public int delayTime = 0;
            public string title = string.Empty;
            public string body = string.Empty;
        }

        private const int minDelay = 10;
#if PLAY_INSTANT
#else
#if UNITY_ANDROID
        private void Start()
        {
            var recievedData = AndroidNotificationCenter.GetLastNotificationIntent();
            if (recievedData != null && string.IsNullOrEmpty(recievedData.Notification.IntentData) == false)
            {
                try
                {
                    receivedUserData = JsonUtility.FromJson<UserData>(recievedData.Notification.IntentData);
                }
                catch (System.Exception e)
                {
                    receivedUserData = null;
                    Debug.LogException(e);
                }
            }

            AndroidNotificationCenter.CancelAllNotifications();

            var c = new AndroidNotificationChannel()
            {
                Id = Application.identifier,
                Name = Application.productName,
                Importance = Importance.High,
                Description = "Generic notifications",
            };
            AndroidNotificationCenter.RegisterNotificationChannel(c);
        }
#elif UNITY_IOS
        private IEnumerator Start()
        {
            var authorizationOption = AuthorizationOption.Alert | AuthorizationOption.Badge;
            using (var req = new AuthorizationRequest(authorizationOption, true))
            {
                while (!req.IsFinished)
                {
                    yield return null;
                };

                string res = "\n RequestAuthorization:";
                res += "\n finished: " + req.IsFinished;
                res += "\n granted :  " + req.Granted;
                res += "\n error:  " + req.Error;
                res += "\n deviceToken:  " + req.DeviceToken;
                Debug.Log(res);

                if (req.Granted)
                    iOSNotificationCenter.RemoveAllScheduledNotifications();
            }
        }
#endif

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                ScheduleLocalPush();
            }
            else
            {

#if UNITY_ANDROID
                AndroidNotificationCenter.CancelAllNotifications();
#elif UNITY_IOS
                iOSNotificationCenter.RemoveAllScheduledNotifications();
#endif
                localPushScheduled = false;
            }
        }

        private void OnApplicationQuit()
        {
            ScheduleLocalPush();
        }

        private void ScheduleLocalPush()
        {
            if (localPushScheduled) return;
            localPushScheduled = true;
            OnScheduleNotification?.Invoke();
        }
#endif
        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static bool localPushScheduled = false;

        public static UserData receivedUserData = null;
        public static event System.Action OnScheduleNotification = null;

        public static void SendNotification(int delaySeconds, string text, string largIcon = "app_icon")
        {
            if (string.IsNullOrEmpty(text)) return;
            var parts = text.Split('|');
            if (parts.Length < 2 || parts[0].IsNullOrEmpty() || parts[1].IsNullOrEmpty()) return;
            SendNotification(delaySeconds, parts[0], parts[1], largIcon);
        }

        public static void SendNotification(int delaySeconds, string title, string message, string largIcon = "app_icon")
        {
            if (delaySeconds < minDelay) return;
#if PLAY_INSTANT
#else
#if UNITY_ANDROID
            var notification = new AndroidNotification()
            {
                Title = title,
                Text = message,
                FireTime = System.DateTime.Now.AddSeconds(delaySeconds),
                LargeIcon = largIcon,
                IntentData = JsonUtility.ToJson(new UserData() { delayTime = delaySeconds, title = title, body = message })
            };

            AndroidNotificationCenter.SendNotification(notification, Application.identifier);
            Debug.Log($"[LocalPush] Scheduled notification delay {delaySeconds}:{title}|{message}");
#elif UNITY_IOS
            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = System.TimeSpan.FromSeconds(delaySeconds),
                Repeats = false
            };

            var notification = new iOSNotification()
            {
                // You can specify a custom identifier which can be used to manage the notification later.
                // If you don't provide one, a unique string will be generated automatically.
                Title = title,
                Body = message,
                Subtitle = string.Empty,
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = Application.productName,
                ThreadIdentifier = "thread1",
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
#endif
#endif
        }

#if PLAY_INSTANT
#else
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        public static void Initialize()
        {
            if (FindObjectOfType<MobileNotification>() == null)
            {
                var go = new GameObject(nameof(MobileNotification), typeof(MobileNotification));
                //go.hideFlags = HideFlags.HideInHierarchy;
                DontDestroyOnLoad(go);
            }
        }

        [Console("game", "notif", "(delayInSeconds) : Test local notification system")]
        public static void TestNotif(int delayInSeconds)
        {
            if (delayInSeconds < minDelay)
                Debug.Log($"Delay must be greater that {minDelay} !");
            else
                SendNotification(delayInSeconds, "This is a test!");
        }
#endif
    }
}