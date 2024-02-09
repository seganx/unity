using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SeganX
{
    public static class MonoEx
    {
        public delegate float ReturnValue<T>(T Obj);

        #region basic types
        public static bool IsSameTypes(System.Type type, System.Type baseType)
        {
            if (type == baseType) return true;
            return type.BaseType == null ? false : IsSameTypes(type.BaseType, baseType);
        }

        public static bool IsSameTypes<T, BASE>()
        {
            return IsSameTypes(typeof(T), typeof(BASE));
        }

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

        public static System.IConvertible RemoveFlag(this System.IConvertible self, System.IConvertible flag)
        {
            int res = (int)self;
            int flg = (int)flag;
            if ((res & flg) != 0)
                res &= ~flg;
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
            return UnityEngine.Networking.UnityWebRequest.EscapeURL(self).Replace("+", "%20");
        }

        public static string PreparePath(this string self, bool checkEndSeparator = false)
        {
            if (checkEndSeparator)
            {
                var res = self.Replace("\\", "/");
                return res.EndsWith("/") ? res : (res + "/");
            }
            else return self.Replace("\\", "/");
        }

        public static string MakeRelative(this string self, string referencePath)
        {
            var fileUri = new System.Uri($"file://{self.PreparePath()}");
            var referenceUri = new System.Uri($"file://{referencePath.PreparePath(true)}");
            return referenceUri.MakeRelativeUri(fileUri).ToString().Replace("%20", " ");
            //var fileUri = new System.Uri(self.PreparePath());
            //var referenceUri = new System.Uri(referencePath.PreparePath(true));
            //return referenceUri.MakeRelativeUri(fileUri).ToString().Replace("%20", " ");
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

        public static byte ToByte(this string self, byte defaultValue = 0)
        {
            if (string.IsNullOrEmpty(self))
                return defaultValue;
            byte res = 0;
            if (byte.TryParse(self, out res))
                return res;
            else return defaultValue;
        }

        public static ushort ToUshort(this string self, ushort defaultValue = 0)
        {
            if (string.IsNullOrEmpty(self))
                return defaultValue;
            ushort res = 0;
            if (ushort.TryParse(self, out res))
                return res;
            else return defaultValue;
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

        public static int CountOf(this string self, char what)
        {
            if (self == null) return 0;
            int res = 0;
            for (int i = self.Length - 1; i >= 0; i--)
                if (self[i] == what)
                    res++;
            return res;
        }

        public static bool IsLetterOrDigit(this string self)
        {
            if (self == null || self.Length < 1) return false;
            for (int i = 0; i < self.Length; i++)
            {
                var isNotLetter = self[i] != ' ' && char.IsLetterOrDigit(self[i]) == false;
                var isSymbol = char.IsSymbol(self[i]);
                var isControl = char.IsControl(self[i]);
                if (isNotLetter || isSymbol || isControl)
                    return false;
            }
            return true;
        }

        public static string SubString(this string self, int startIndex, int count)
        {
            if (self == null) return string.Empty;
            if (startIndex >= self.Length) return string.Empty;
            if (startIndex + count >= self.Length) count = self.Length - startIndex;
            return self.Substring(startIndex, count);
        }

        public static string CleanFromCode(this string self)
        {
            return (self == null) ? null : self.Replace('"', '\'');
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

        public static List<T> Shuffle<T>(this List<T> self, System.Random randomer = null)
        {
            int n = self.Count;
            while (n > 1)
            {
                int k = randomer == null ? Random.Range(0, n--) : randomer.Next(n--);
                T value = self[k];
                self[k] = self[n];
                self[n] = value;
            }
            return self;
        }

        public static T LastOne<T>(this List<T> self)
        {
            return self.Count > 0 ? self[self.Count - 1] : default;
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
        public static void SafeInvoke(this System.Action action)
        {
            if (action == null) return;
            var methods = action.GetInvocationList();
            foreach (var method in methods)
            {
                try
                {
                    method.DynamicInvoke();
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static void SafeInvoke<T>(this System.Action<T> action, T obj)
        {
            if (action == null) return;
            var methods = action.GetInvocationList();
            foreach (var method in methods)
            {
                try
                {
                    method.DynamicInvoke(obj);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }

        public static void SafeInvoke<T1, T2>(this System.Action<T1, T2> action, T1 obj1, T2 obj2)
        {
            if (action == null) return;
            var methods = action.GetInvocationList();
            foreach (var method in methods)
            {
                try
                {
                    method.DynamicInvoke(obj1, obj2);
                }
                catch (System.Exception e)
                {
                    Debug.LogException(e);
                }
            }
        }
        #endregion
    }
}
