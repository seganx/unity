using System;
using System.IO;
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
    }
}
