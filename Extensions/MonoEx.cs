using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class MonoEx
{
    public delegate float ReturnValue<T>(T Obj);

    #region basic types
    public static bool IsTypeOf<T>(this object self)
    {
        return self != null && self is T;
    }

    public static T As<T>(this object self) where T : class
    {
        return self as T;
    }

    public static bool Between(this float x, float x1, float x2)
    {
        return x >= x1 && x <= x2;
    }

    public static bool Between(this int x, int x1, int x2)
    {
        return x >= x1 && x <= x2;
    }

    public static string ToStringPrice(this int x)
    {
        return x.ToString("0,0");
    }

    public static int ToInt(this float self)
    {
        return Mathf.RoundToInt(self);
    }

    public static int ToInt(this char self)
    {
        if (System.Char.IsDigit(self))
            return (int)System.Char.GetNumericValue(self);
        else
            return 0;
    }

    public static bool IsFlagOn(this System.IConvertible self, System.IConvertible flag)
    {
        return ((int)self & (int)flag) != 0;
    }

    public static System.IConvertible AddFlag(this System.IConvertible self, System.IConvertible flag)
    {
        int res = (int)self;
        res |= (int)flag;
        return res;
    }

    public static int GetFlagsCount(this System.IConvertible self)
    {
        var res = 0;
        var values = System.Enum.GetValues(self.GetType());
        foreach (var value in values)
            if (((int)self & (int)value) != 0)
                res++;
        return res;
    }
    #endregion

    #region string
    public static bool IsNullOrEmpty(this string self)
    {
        return string.IsNullOrEmpty(self);
    }

    public static bool HasContent(this string self)
    {
        return !string.IsNullOrEmpty(self);
    }

    public static bool HasContent(this string self, int minLength)
    {
        return !string.IsNullOrEmpty(self) && self.Length >= minLength;
    }

    public static byte[] GetBytes(this string self)
    {
        return System.Text.Encoding.UTF8.GetBytes(self);
    }

    public static string EscapeURL(this string self)
    {
        return WWW.EscapeURL(self).Replace("+", "%20");
    }

    public static string BuildPath(this string self)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            return self.Replace("/", "\\");
        else
            return self.Replace("\\", "/");
    }

    public static string MakeRelative(this string self, string referencePath)
    {
        var fileUri = new System.Uri(self);
        var referenceUri = new System.Uri(referencePath);
        return referenceUri.MakeRelativeUri(fileUri).ToString();
    }

    public static string ExcludeFileExtention(this string self)
    {
        var index = self.LastIndexOf('.');
        return (index > 0) ? self.Remove(index, self.Length - index) : self;
    }

    public static bool ToBoolean(this string self)
    {
        return self.ToLower() == "true";
    }

    public static int ToInt(this string self, int defaultValue = 0)
    {
        if (string.IsNullOrEmpty(self))
            return defaultValue;
        int res = 0;
        if (int.TryParse(self, out res))
            return res;
        else return defaultValue;
    }

    public static uint ToUint(this string self, uint defaultValue = 0)
    {
        if (string.IsNullOrEmpty(self))
            return defaultValue;
        uint res = 0;
        if (uint.TryParse(self, out res))
            return res;
        else return defaultValue;
    }

    public static float ToFloat(this string self, float defaultValue = 0)
    {
        if (string.IsNullOrEmpty(self))
            return defaultValue;
        float res = 0;
        if (float.TryParse(self, out res))
            return res;
        else return defaultValue;
    }

    public static T ToEnum<T>(this string self, T defaultValue)
    {
        if (string.IsNullOrEmpty(self))
            return defaultValue;
        try
        {
            T res = (T)System.Enum.Parse(typeof(T), self, true);
            return System.Enum.IsDefined(typeof(T), res) ? res : defaultValue;
        }
        catch { }
        return defaultValue;
    }

    public static int FindDigit(this string self, int startIndex = 0)
    {
        if (self.HasContent())
            for (int i = 0; i < self.Length; i++)
                if (char.IsDigit(self[i]))
                    return i;
        return -1;
    }

    public static bool IsRtl(this string self)
    {
        return self != null && self.Length > 0 && (int)self[0] > 1000;
    }

    public static bool HasRtl(this string self)
    {
        bool isEnglish = true;
        for (int i = 0; i < self.Length && isEnglish; i++)
            isEnglish = (self[i] < 1000);
        return !isEnglish;
    }

    public static bool ContainsAny(this string self, string[] items)
    {
        if (self.IsNullOrEmpty() || items.IsNullOrEmpty()) return false;
        foreach (var item in items)
            if (self.Contains(item))
                return true;
        return false;
    }

    public static bool ContainsAll(this string self, string[] items)
    {
        if (self.IsNullOrEmpty() || items.IsNullOrEmpty()) return false;
        foreach (var item in items)
            if (self.Contains(item) == false)
                return false;
        return true;
    }

    public static string GetWithoutBOM(this string self)
    {
        if (self.IsNullOrEmpty()) return string.Empty;
        MemoryStream memoryStream = new MemoryStream(self.GetBytes());
        StreamReader streamReader = new StreamReader(memoryStream, true);
        string result = streamReader.ReadToEnd();
        streamReader.Close();
        memoryStream.Close();
        return result;
    }

    public static string ComputeMD5(this string self, string salt)
    {
        var md5 = System.Security.Cryptography.MD5.Create();
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(self + salt);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        var res = new System.Text.StringBuilder();
        for (int i = 0; i < hashBytes.Length; i++)
            res.Append(hashBytes[i].ToString("X2"));

        return res.ToString();
    }

    public static int GetBreakCount(this string self)
    {
        if (self == null) return 0;
        var lines = self.Split('\n');
        return lines.Length;
    }

    public static string CleanFromCode(this string self)
    {
        return (self == null) ? null : self.Replace('"', '\'');
    }

    public static string CleanForPersian(this string self)
    {
        return (self == null) ? null : self.Replace('ي', 'ی');
    }

    public static string Persian(this string self, bool force = true)
    {
        if (!force && !self.IsRtl())
            return self;

        return (self == null) ? null : PersianTextShaper.PersianTextShaper.ShapeText(CleanForPersian(self))
            .Replace("‌", "")
            .Replace("‌", "")
            .Replace('٠', '۰')
            .Replace('١', '۱')
            .Replace('٢', '۲')
            .Replace('٣', '۳')
            .Replace('٤', '۴')
            .Replace('٥', '۵')
            .Replace('٦', '۶')
            .Replace('٧', '۷')
            .Replace('٨', '۸')
            .Replace('٩', '۹');
    }

    private static int GetPersianAlphaForSort(this char self)
    {
        switch (self)
        {
            case 'آ': return 10;
            case 'ا': return 11;
            case 'ب': return 12;
            case 'پ': return 13;
            case 'ت': return 14;
            case 'ث': return 15;
            case 'ج': return 16;
            case 'چ': return 17;
            case 'ح': return 18;
            case 'خ': return 19;
            case 'د': return 20;
            case 'ذ': return 21;
            case 'ر': return 22;
            case 'ز': return 23;
            case 'ژ': return 24;
            case 'س': return 25;
            case 'ش': return 26;
            case 'ص': return 27;
            case 'ض': return 28;
            case 'ط': return 29;
            case 'ظ': return 30;
            case 'ع': return 31;
            case 'غ': return 32;
            case 'ف': return 33;
            case 'ق': return 34;
            case 'ک': return 35;
            case 'گ': return 36;
            case 'ل': return 37;
            case 'م': return 38;
            case 'ن': return 39;
            case 'و': return 40;
            case 'ه': return 41;
            case 'ی': return 42;
            default: return 43 + self;
        }
    }

    public static int PersianCompare(this string self, string with)
    {
        int n = System.Math.Max(self.Length, with.Length);
        int s = 0, w = 0;
        for (int i = 0; i < n; i++)
        {
            s = i < self.Length ? self[i].GetPersianAlphaForSort() : 0;
            w = i < with.Length ? with[i].GetPersianAlphaForSort() : 0;
            if (s == w) continue;
            return s - w;
        }
        return 0;
    }

    #endregion

    #region arrays
    public static bool HasOneItem(this System.Array self)
    {
        return self != null && self.Length == 1;
    }

    public static bool HasItem(this System.Array self)
    {
        return self != null && self.Length > 0;
    }

    public static int LastIndex(this System.Array self)
    {
        return self.Length - 1;
    }

    public static int LastIndex<T>(this List<T> self)
    {
        return self.Count - 1;
    }

    public static T LastOne<T>(this List<T> self)
    {
        return self[self.Count - 1];
    }


    public static bool IsNullOrEmpty(this System.Array self)
    {
        return self == null || self.Length < 1;
    }

    public static bool Contains(this System.Array self, object item)
    {
        if (self.IsNullOrEmpty() || item == null) return false;
        foreach (var i in self)
            if (item.Equals(i))
                return true;
        return false;
    }

    public static T FindMax<T>(this System.Array self, ReturnValue<T> returnValueFunc)
    {
        if (self.IsNullOrEmpty() || self.Length < 1) return default(T);
        object selected = self.GetValue(0);
        foreach (var item in self)
            if (returnValueFunc((T)selected) < returnValueFunc((T)item))
                selected = item;
        return (T)selected;
    }

    public static T FindMin<T>(this System.Array self, ReturnValue<T> returnValueFunc)
    {
        if (self.IsNullOrEmpty() || self.Length < 1) return default(T);
        object selected = self.GetValue(0);
        foreach (var item in self)
            if (returnValueFunc((T)selected) > returnValueFunc((T)item))
                selected = item;
        return (T)selected;
    }

    public static T FindMax<T>(this List<T> self, ReturnValue<T> returnValueFunc)
    {
        if (self == null || self.Count < 1) return default(T);
        T selected = self[0];
        foreach (var item in self)
            if (returnValueFunc(selected) < returnValueFunc(item))
                selected = item;
        return selected;
    }

    public static T FindMin<T>(this List<T> self, ReturnValue<T> returnValueFunc)
    {
        if (self == null || self.Count < 1) return default(T);
        T selected = self[0];
        foreach (var item in self)
            if (returnValueFunc(selected) > returnValueFunc(item))
                selected = item;
        return selected;
    }

    public static T RandomOne<T>(this System.Array self, T defaultValue = default(T))
    {
        return (self != null && self.Length > 1) ? (T)self.GetValue(Random.Range(0, int.MaxValue) % self.Length) : defaultValue;
    }

    public static T RandomOne<T>(this List<T> self, T defaultValue = default(T))
    {
        return (self != null && self.Count > 0) ? self[Random.Range(0, int.MaxValue) % self.Count] : defaultValue;
    }
    #endregion

    #region system
    public static string GetPersianDateTime(this System.DateTime dateTime)
    {
        PersianCalendar pc = new PersianCalendar();
        try { return string.Format("{0}/{1}/{2} {3}:{4}", pc.GetYear(dateTime), pc.GetMonth(dateTime), pc.GetDayOfMonth(dateTime), pc.GetHour(dateTime), pc.GetMinute(dateTime)); }
        catch { }
        return "";
    }

    // clone object using serialize/deserialize in memory. USE ONLY for basic classes
    public static T CloneSerialized<T>(this object self)
    {
        MemoryStream stream = new MemoryStream();
        BinaryFormatter serializer = new BinaryFormatter();
        serializer.Serialize(stream, self);
        stream.Position = 0;
        BinaryFormatter deserializer = new BinaryFormatter();
        return (T)deserializer.Deserialize(stream);
    }
    #endregion

}
