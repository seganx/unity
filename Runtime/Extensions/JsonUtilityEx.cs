using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static class JsonUtilityEx
    {
        [System.Serializable]
        private class SafeJson
        {
            public string data = string.Empty;
            public string signature = string.Empty;
        }

        [System.Serializable]
        private class ArrayData<T>
        {
            public T[] list = null;
        }

        [System.Serializable]
        private class ListData<T>
        {
            public List<T> list = null;
        }

        public static string ToSafeJson(object data, bool pretty = false)
        {
            var safe = new SafeJson();
            safe.data = JsonUtility.ToJson(data);
            safe.signature = safe.data.ComputeMD5(Core.salt);
            return JsonUtility.ToJson(safe, pretty);
        }

        public static T FromSafeJson<T>(string json, bool validateSignature = true)
        {
            if (json.HasContent(3) == false) return default(T);

            var safe = JsonUtility.FromJson<SafeJson>(json);
            if (validateSignature)
                return safe.signature == safe.data.ComputeMD5(Core.salt) ? JsonUtility.FromJson<T>(safe.data) : default(T);
            else
                return JsonUtility.FromJson<T>(safe.data);
        }

        public static string ArrayToJson<T>(System.Array array)
        {
            ArrayData<T> data = new ArrayData<T>();
            array.CopyTo(data.list, 0);
            return JsonUtility.ToJson(data);
        }

        public static string ListToJson<T>(List<T> list)
        {
            ListData<T> data = new ListData<T>() { list = list };
            return JsonUtility.ToJson(data);
        }

        public static List<T> ListFromJson<T>(string json)
        {
            var obj = JsonUtility.FromJson<ListData<T>>(json);
            return obj.list;
        }
    }
}