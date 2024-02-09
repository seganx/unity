using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeganX
{
    public class Utils
    {
        public struct StopWatch
        {
            private float counter;
            private float unscaledCounter;
            private float realtimeCounter;

            public float Timer => Time.time - counter;
            public float UnscaledTimer => Time.unscaledTime - unscaledCounter;
            public float RealtimeTimer => Time.realtimeSinceStartup - realtimeCounter;

            public static StopWatch Create(float delay = 0) => new StopWatch().Reset(delay);

            public StopWatch Reset(float delay = 0)
            {
                counter = Time.time + delay;
                unscaledCounter = Time.unscaledTime + delay;
                realtimeCounter = Time.realtimeSinceStartup + delay;
                return this;
            }
        }

        public static DateTime UnixTimeToTime(long date)
        {
            DateTime res = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            res = res.AddSeconds(date);
            return res;
        }

        public static DateTime UnixTimeToTime(string date)
        {
            return UnixTimeToTime(long.Parse(date));
        }

        public static DateTime UnixTimeToLocalTime(long date)
        {
            return UnixTimeToTime(date).ToLocalTime();
        }

        public static DateTime UnixTimeToLocalTime(string date)
        {
            return UnixTimeToTime(date).ToLocalTime();
        }

        public static string TimeToString(double time, int decimals)
        {
            int h = (int)time / 3600;
            int m = ((int)time % 3600) / 60;
            double s = time % 60;
            switch (decimals)
            {
                case 1: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.0")) : (m.ToString("00") + ":" + s.ToString("00.0"));
                case 2: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.00")) : (m.ToString("00") + ":" + s.ToString("00.00"));
                case 3: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.000")) : (m.ToString("00") + ":" + s.ToString("00.000"));
            }
            return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00")) : (m.ToString("00") + ":" + s.ToString("00"));
        }

#if SX_ZIP
        public static string CompressString(string text, string failedReturn)
        {
            try
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(text);
                var msi = new System.IO.MemoryStream(bytes);
                var mso = new System.IO.MemoryStream();
                var enc = new SevenZip.Compression.LZMA.Encoder();
                enc.WriteCoderProperties(mso);
                mso.Write(BitConverter.GetBytes(bytes.Length), 0, 4);
                enc.Code(msi, mso, msi.Length, -1, null);
                var res = Convert.ToBase64String(mso.ToArray());
                msi.Close(); msi.Dispose();
                mso.Close(); mso.Dispose();
                return res;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message + " | " + ex.StackTrace);
            }
            return failedReturn;
        }

        public static string DecompressString(string compressedText, string failedReturn)
        {
            if (compressedText.HasContent(10) == false) return failedReturn;
            try
            {
                var msi = new System.IO.MemoryStream(Convert.FromBase64String(compressedText));
                var mso = new System.IO.MemoryStream();
                SevenZip.Compression.LZMA.Decoder dec = new SevenZip.Compression.LZMA.Decoder();
                byte[] props = new byte[5]; msi.Read(props, 0, 5);
                byte[] length = new byte[4]; msi.Read(length, 0, 4);
                int len = BitConverter.ToInt32(length, 0);
                dec.SetDecoderProperties(props);
                dec.Code(msi, mso, msi.Length, len, null);
                var res = System.Text.Encoding.UTF8.GetString(mso.ToArray());
                msi.Close(); msi.Dispose();
                mso.Close(); mso.Dispose();
                return res;
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message + " | " + ex.StackTrace);
            }
            return failedReturn;
        }
#endif

        public static float RandomDoubleHigh(float min, float max)
        {
            if (max < min)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            var delta = max - min;
            var r1 = UnityEngine.Random.Range(min, max + delta);
            var r2 = UnityEngine.Random.Range(min, max + delta);
            var res = (r1 + r2) / 2;
            if (res > max) res = 2 * max - res;
            return res;
        }

        public static int RandomDoubleHigh(int min, int max)
        {
            if (max < min)
            {
                var tmp = min;
                min = max;
                max = tmp;
            }

            var delta = max - min;
            var r1 = UnityEngine.Random.Range(min, max + delta);
            var r2 = UnityEngine.Random.Range(min, max + delta);
            var res = (r1 + r2) / 2;
            if (res > max) res = 2 * max - res;
            return res;
        }

        /// <summary>
        /// Sum of probabilities must equlas to 1;
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dict"></param>
        /// <returns></returns>
        public static T ProbabilityRandom<T>(Dictionary<T, float> dict)
        {
            float probs = 0f;
            float random = UnityEngine.Random.value;
            foreach (KeyValuePair<T, float> pair in dict)
            {
                probs += pair.Value;
                if (probs > random)
                {
                    return pair.Key;
                }
            }
            return dict.ElementAt(UnityEngine.Random.Range(0, dict.Count)).Key;
        }
    }
}
