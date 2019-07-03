using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;

namespace SeganX
{
    public static class PlayerPrefsEx
    {
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

        public static void SetInt(string key, int value)
        {
            PlayerPrefs.SetInt(EncryptString(key), value);
        }

        public static int GetInt(string key, int defaultValue)
        {
            return PlayerPrefs.GetInt(EncryptString(key), defaultValue);
        }

        public static void SetFloat(string key, float value)
        {
            PlayerPrefs.SetFloat(EncryptString(key), value);
        }

        public static float GetFloat(string key, float defaultValue)
        {
            return PlayerPrefs.GetFloat(EncryptString(key), defaultValue);
        }

        public static void SetString(string key, string value)
        {
            PlayerPrefs.SetString(EncryptString(key), EncryptString(value));
        }

        public static string GetString(string key, string defaultValue)
        {
            var tmp = PlayerPrefs.GetString(EncryptString(key), defaultValue);
            return tmp == defaultValue ? tmp : DecryptString(tmp);
        }

        public static void Serialize(string key, object value)
        {
            MemoryStream stream = new MemoryStream();
            BinaryFormatter fmter = new BinaryFormatter();
            fmter.Serialize(stream, value);
            SaveData(key + ".seganx", Encrypt(stream.GetBuffer(), Core.CryptoKey));
        }

        public static T Deserialize<T>(string key, T defaultValue)
        {
            byte[] data = LoadData(key + ".seganx");
            if (data != null && data.Length > 0)
            {
                MemoryStream stream = new MemoryStream(Decrypt(data, Core.CryptoKey));
                BinaryFormatter fmter = new BinaryFormatter();
                return (T)fmter.Deserialize(stream);
            }
            else return defaultValue;
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