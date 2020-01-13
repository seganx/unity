using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static partial class Online
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

            private void Start()
            {
                DownloadData<long>("time.php", null, (success, res) =>
                {
                    if (success)
                    {
                        Synced = true;
                        deltaTime = UnixTimeToLocalTime(res) - DateTime.Now;
                        Invoke("Start", 120);
                    }
                    else Invoke("Start", 5);
                });
            }

            ////////////////////////////////////////////////////////
            /// STATIC MEMBER
            ////////////////////////////////////////////////////////
            private static Timer instance = null;
            private static List<TimeData> data = new List<TimeData>();
            private static TimeSpan deltaTime;

            public static bool Synced { get; private set; }
            public static DateTime CurrentTime { get { return DateTime.Now + deltaTime; } }
            public static long CurrentSeconds { get { return CurrentTime.Ticks / TimeSpan.TicksPerSecond; } }

            public static void Set(int timerId, float duration, long startTime = 0)
            {
                var timer = data.Find(x => x.id == timerId);
                if (timer == null)
                {
                    timer = new TimeData();
                    timer.id = timerId;
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
                DateTime res = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                res = res.AddSeconds(date);
                return res.ToLocalTime();
            }

            private static void Save()
            {
                PlayerPrefsEx.SetObject("SeganX.Online.Timer", data);
            }

            private static void Load()
            {
                data = PlayerPrefsEx.GetObject("SeganX.Online.Timer", data);
            }

            internal static void Init()
            {
                if (instance == null)
                    DontDestroyOnLoad(instance = Game.Instance.gameObject.AddComponent<Timer>());
            }
        }


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void InitializeTimer()
        {
            Timer.Init();
        }
    }
}