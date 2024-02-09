using UnityEngine;

namespace SeganX
{
    public static class Vibration
    {
        private static bool initialized = false;

#if UNITY_ANDROID && !UNITY_EDITOR
        private static AndroidJavaObject vibrator = null;
        private static AndroidJavaClass effector = null;

        private static bool HasVibrator => vibrator != null;
        private static bool HasAmplfire => effector != null;
#else
        private static bool HasVibrator = false;
#endif

        public static bool Enabled
        {
            get => PlayerPrefs.GetInt("Vibration.Enabled", 1) > 0;
            set => PlayerPrefs.SetInt("Vibration.Enabled", value ? 1 : 0);
        }

        static Vibration()
        {
            if (initialized) return;
            initialized = true;

#if UNITY_ANDROID && !UNITY_EDITOR
            // Add APP VIBRATION PERMISSION to the Manifest
            if (Application.isConsolePlatform)
            {
                Handheld.Vibrate();
            }

            int apiLevel = 1;
            using (AndroidJavaClass androidVersionClass = new AndroidJavaClass("android.os.Build$VERSION"))
            {
                apiLevel = androidVersionClass.GetStatic<int>("SDK_INT");
            }

            using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                using (AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
                {
                    if (currentActivity != null)
                    {
                        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

                        if (apiLevel >= 26)
                            effector = new AndroidJavaClass("android.os.VibrationEffect");

                        if (vibrator.Call<bool>("hasVibrator") == false)
                            vibrator = null;
                        else if (effector != null && vibrator.Call<bool>("hasAmplitudeControl") == false)
                            effector = null;
                    }
                }
            }
#endif
        }

        public static void Vibrate(long milliseconds)
        {
            Vibrate(milliseconds, 0, false);
        }

        public static void Vibrate(long milliseconds, int amplitude = 0, bool cancel = false)
        {
            if (initialized == false || HasVibrator == false || Enabled == false) return;

            if (cancel)
            {
                Cancel();
            }

#if UNITY_ANDROID && !UNITY_EDITOR
            if (HasAmplfire && amplitude > 0)
            {
                amplitude = Mathf.Clamp(amplitude, 1, 255);
                using (AndroidJavaObject effect = effector.CallStatic<AndroidJavaObject>("createOneShot", milliseconds, amplitude))
                {
                    vibrator.Call("vibrate", effect);
                }
            }
            else
            {
                vibrator.Call("vibrate", milliseconds);
            }
#endif
        }

        public static void Cancel()
        {
            if (initialized == false || Enabled == false) return;

#if UNITY_ANDROID && !UNITY_EDITOR
            if (HasVibrator)
            {
                vibrator.Call("cancel");
            }
#endif
        }
    }
}
