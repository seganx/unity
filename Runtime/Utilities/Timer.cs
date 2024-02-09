using System;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class Timer : MonoBehaviour
    {
        [Serializable]
        private class TimeData
        {
            public int id = 0;
            public long startTime = 0;
            public float duration = 0;
        }

        private void Awake()
        {
            instance = this;
            Load();
        }

        private void OnApplicationQuit()
        {
            Save();
        }

        private void OnApplicationPause(bool pause)
        {
            Save();
        }

        ////////////////////////////////////////////////////////
        /// STATIC MEMBER
        ////////////////////////////////////////////////////////
        private static Timer instance = null;
        private static List<TimeData> data = new List<TimeData>();
        private static TimeSpan deltaTime = new TimeSpan(0);

        public static DateTime CurrentTime => DateTime.Now + deltaTime;
        public static long CurrentSeconds => CurrentTime.Ticks / TimeSpan.TicksPerSecond;

        public static void SyncTimer(long unixTime)
        {
            if (unixTime < 1646141462) return;
            deltaTime = UnixTimeToLocalTime(unixTime) - DateTime.Now;
            if (Mathf.Abs((float)deltaTime.TotalDays) > 5 * 12 * 30)
                Debug.Log(deltaTime);
        }

        public static void Set(int timerId, float duration, long startTime = 0)
        {
            var timer = data.Find(x => x.id == timerId);
            if (timer == null)
            {
                timer = new TimeData() { id = timerId };
                data.Add(timer);
            }

            timer.duration = duration;
            timer.startTime = startTime == 0 ? CurrentSeconds : startTime;

            Save();
        }

        public static int GetRemainSeconds(int timerId, float duration)
        {
            var timer = data.Find(x => x.id == timerId);
            if (timer == null)
            {
                Set(timerId, duration);
                timer = data.Find(x => x.id == timerId);
            }

            return Mathf.FloorToInt(timer.startTime - CurrentSeconds + timer.duration);
        }

        public static bool Exist(int timerId)
        {
            return data.Exists(x => x.id == timerId);
        }

        public static void Remove(int timerId)
        {
            data.RemoveAll(x => x.id == timerId);
        }

        public static DateTime UnixTimeToLocalTime(long date)
        {
            try
            {
                DateTime res = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                res = res.AddSeconds(date);
                return res.ToLocalTime();
            }
            catch
            {
                return DateTime.Now;
            }
        }

        private static void Save()
        {
            PlayerPrefsEx.SetObject("SeganX.Timer", data);
        }

        private static void Load()
        {
            data = PlayerPrefsEx.GetObject("SeganX.Timer", data);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeTimer()
        {
            if (instance != null) return;
            instance = new GameObject("Timer").AddComponent<Timer>();
            DontDestroyOnLoad(instance);
        }
    }
}
