using System;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

namespace SeganX
{
    public static class Utilities
    {
        public static string lastErrorMessage = string.Empty;

        public static XmlReader ParseXml(string xmlText)
        {
            if (string.IsNullOrEmpty(xmlText))
            {
                lastErrorMessage = "Can't parse empty text!";
                return null;
            }

            //  check to see if XML is valid
            var document = new XmlDocument();
            try { document.LoadXml(xmlText); }
            catch (XmlException e)
            {
                lastErrorMessage = e.Message;
                return null;
            }

            try
            {
                XmlTextReader reader = new XmlTextReader(new StringReader(xmlText));
                reader.WhitespaceHandling = WhitespaceHandling.None;
                return reader;
            }
            catch (Exception e)
            {
                lastErrorMessage = e.Message;
                return null;
            }
        }

        public static DateTime UnixTimeToTime(string date)
        {
            DateTime res = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            res = res.AddSeconds(double.Parse(date));
            return res;
        }

        public static DateTime UnixTimeToLocalTime(string date)
        {
            DateTime res = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            res = res.AddSeconds(double.Parse(date));
            return res.ToLocalTime();
        }

        public static DateTime UnixTimeToLocalTime(long date)
        {
            DateTime res = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            res = res.AddSeconds(date);
            return res.ToLocalTime();
        }

        public static string TimeToString(TimeSpan span)
        {
            if (span.Hours < 0) return string.Empty;

            DateTime time = DateTime.Now;
            try
            {
                time = DateTime.MinValue + span;
            }
            catch (Exception e)
            {
                Debug.LogError(e + "\nSpan:" + span);
            }
            int months = time.Month - 1;
            int days = time.Day - 1;

            if (months > 0)
                return string.Format(LocalizationService.Get(111010), months, days);
            else if (days > 0)
                return string.Format(LocalizationService.Get(111011), days, span.Hours);
            else if (span.Hours > 0)
                return string.Format(LocalizationService.Get(111012), span.Hours, span.Minutes);
            else
                return string.Format(LocalizationService.Get(111013), span.Minutes, span.Seconds);
        }

        public static string TimeToString(float time, int decimals)
        {
            int h = (int)time / 3600;
            int m = ((int)time % 3600) / 60;
            float s = time % 60;
            switch (decimals)
            {
                case 1: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.0")) : (m.ToString("00") + ":" + s.ToString("00.0"));
                case 2: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.00")) : (m.ToString("00") + ":" + s.ToString("00.00"));
                case 3: return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00.000")) : (m.ToString("00") + ":" + s.ToString("00.000"));
            }
            return (h > 0) ? (h.ToString("00") + ":" + m.ToString("00") + ":" + s.ToString("00")) : (m.ToString("00") + ":" + s.ToString("00"));
        }

        public static string CompressString(string text, string failedReturn)
        {
            try
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                MemoryStream msi = new MemoryStream(bytes);
                MemoryStream mso = new MemoryStream();
                SevenZip.Compression.LZMA.Encoder enc = new SevenZip.Compression.LZMA.Encoder();
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
                MemoryStream msi = new MemoryStream(Convert.FromBase64String(compressedText));
                MemoryStream mso = new MemoryStream();
                SevenZip.Compression.LZMA.Decoder dec = new SevenZip.Compression.LZMA.Decoder();
                byte[] props = new byte[5]; msi.Read(props, 0, 5);
                byte[] length = new byte[4]; msi.Read(length, 0, 4);
                int len = BitConverter.ToInt32(length, 0);
                dec.SetDecoderProperties(props);
                dec.Code(msi, mso, msi.Length, len, null);
                var res = Encoding.UTF8.GetString(mso.ToArray());
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

        public static float RandomDoubleHigh(float min, float max)
        {
            var r1 = UnityEngine.Random.Range(min, max);
            var r2 = UnityEngine.Random.Range(min, max);
            var res = r1 + r2;
            if (res > max) res = 2 * max - res;
            return res;
        }

        public static int RandomDoubleHigh(int min, int max)
        {
            var r1 = UnityEngine.Random.Range(min, max);
            var r2 = UnityEngine.Random.Range(min, max);
            var res = r1 + r2;
            if (res > max) res = 2 * max - res;
            return res;
        }
    }
}
