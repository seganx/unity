using System.Collections;
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

        public static string ToJson(object data, bool pretty = false)
        {
            var safe = new SafeJson();
            safe.data = JsonUtility.ToJson(data);
            safe.signature = safe.data.ComputeMD5(Core.Salt);
            return JsonUtility.ToJson(safe, pretty);
        }

        public static T FromJson<T>(string json, bool validateSignature = true)
        {
            if (json.HasContent(3) == false) return default(T);

            var safe = JsonUtility.FromJson<SafeJson>(json);
            if (validateSignature)
                return safe.signature == safe.data.ComputeMD5(Core.Salt) ? JsonUtility.FromJson<T>(safe.data) : default(T);
            else
                return JsonUtility.FromJson<T>(safe.data);
        }
    }
}