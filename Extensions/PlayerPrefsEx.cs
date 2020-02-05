using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace SeganX
{
    public static class PlayerPrefsEx
    {
        [System.Serializable]
        private class ObjectData<T>
        {
            public T obj = default(T);
        }

        private static byte[] Encrypt(byte[] data, byte[] key)
        {
            var res = new byte[data.Length];
            for (int i = 0; i < data.Length; ++i)
                res[i] = (byte)(data[i] + key[i % key.Length]);
            return res;
        }

        private static byte[] Decrypt(byte[] data, byte[] key)
        {
            var res = new byte[data.Length];
            for (int i = 0; i < data.Length; ++i)
                res[i] = (byte)(data[i] - key[i % key.Length]);
            return res;
        }

        public static string EncryptString(string value)
        {
            return System.Convert.ToBase64String(Encrypt(System.Text.Encoding.UTF8.GetBytes(value), Core.CryptoKey));
        }

        public static string DecryptString(string value)
        {
            return System.Text.Encoding.UTF8.GetString(Decrypt(System.Convert.FromBase64String(value), Core.CryptoKey));
        }

        public static string EncryptName(string value)
        {
            return EncryptString(value).Replace('+', '_').Replace('/', '_').Replace('=', '_');
        }

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(EncryptName(key), value);
        }

        public static int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(EncryptName(key), defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(EncryptName(key), value);
        }

        public static float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(EncryptName(key), defaultValue);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(EncryptName(key), EncryptString(value));
        }

        public static string GetString(string key, string defaultValue)
        {
            var tmp = PlayerPrefs.GetString(EncryptName(key), defaultValue);
            return tmp == defaultValue ? tmp : DecryptString(tmp);
        }

        public static void SetObject<T>(string key, T value)
        {
            var tmp = new ObjectData<T>();
            tmp.obj = value;
            var json = JsonUtility.ToJson(tmp);
            var filename = EncryptName(key) + ".seganx";
            SaveData(filename, Encrypt(json.GetBytes(), Core.CryptoKey));
        }

        public static T GetObject<T>(string key, T defaultValue)
        {
            var filename = EncryptName(key) + ".seganx";
            byte[] data = LoadData(filename);

            if (data != null && data.Length > 0)
            {
                string json = System.Text.Encoding.UTF8.GetString(Decrypt(data, Core.CryptoKey));
                var tmp = JsonUtility.FromJson<ObjectData<T>>(json);
                return tmp.obj;
            }
            else // last version compatiblity
            {
                filename = EncryptString(key) + ".seganx";
                data = LoadData(filename);

                if (data != null && data.Length > 0)
                {
                    string json = System.Text.Encoding.UTF8.GetString(Decrypt(data, Core.CryptoKey));
                    return JsonUtility.FromJson<T>(json);
                }
            }

            return defaultValue;
        }

        public static void SaveData(string path, byte[] data)
        {
            path = Application.persistentDataPath + "/" + path;
            var dir = Path.GetDirectoryName(path);
            if (Directory.Exists(dir) == false)
                Directory.CreateDirectory(dir);
            try
            {
                File.WriteAllBytes(path, data);
            }
            catch (System.Exception e)
            {
                Debug.LogError("Can't save the file: " + e.Message);
            }
        }

        public static byte[] LoadData(string path)
        {
            byte[] res = null;
            path = Application.persistentDataPath + "/" + path;
            if (File.Exists(path))
            {
                try
                {
                    res = File.ReadAllBytes(path);
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Can't read file: " + e.Message);
                }
            }
            return res;
        }

        public static void Delete(string key)
        {
            var path = Application.persistentDataPath + "/" + key + ".seganx";
            if (File.Exists(path))
                File.Delete(path);
            else
                PlayerPrefs.DeleteKey(EncryptName(key));
        }

        public static void ClearData()
        {
            var files = Directory.GetFiles(Application.persistentDataPath);
            foreach (var item in files)
                try { File.Delete(item); }
                catch { };

            var dirs = Directory.GetDirectories(Application.persistentDataPath);
            foreach (var item in dirs)
                try { Directory.Delete(item, true); }
                catch { };

            files = Directory.GetFiles(Application.temporaryCachePath);
            foreach (var item in files)
                try { File.Delete(item); }
                catch { };

            dirs = Directory.GetDirectories(Application.temporaryCachePath);
            foreach (var item in dirs)
                try { Directory.Delete(item, true); }
                catch { };
        }
    }
}