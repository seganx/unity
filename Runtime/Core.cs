using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SeganX
{
    public class Core : ScriptableObject
    {
        [SerializeField] private SecurityOptions securityOptions = new SecurityOptions();
#if SX_ONLINE
        [Space]
        [SerializeField] private OnlineOptions onlineOptions = new OnlineOptions();
#endif

#if UNITY_EDITOR
        [Space]
        [SerializeField] private TestDevices testDevices = new TestDevices();
#endif

        private string baseDeviceId = string.Empty;
        private string deviceId = string.Empty;
        private string hashsalt = string.Empty;
        private byte[] cryptoKey = null;


        protected void OnInitialize()
        {
#if UNITY_EDITOR
            if (testDevices.active)
                baseDeviceId = deviceId = testDevices.list[testDevices.index];
            else
#endif
            {
                baseDeviceId = SystemInfo.deviceUniqueIdentifier;
                deviceId = ComputeMD5(baseDeviceId, securityOptions.salt);
            }
            hashsalt = ComputeMD5(securityOptions.salt, securityOptions.salt);
            cryptoKey = System.Text.Encoding.ASCII.GetBytes(securityOptions.cryptokey + hashsalt);

#if !UNITY_EDITOR
            securityOptions.cryptokey = string.Empty;
            securityOptions.salt = string.Empty;
            GameName = GetGameName(Application.identifier, Application.productName);
#endif
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        private static Core instance = default;

        public static string Version => "7.1.8";
        public static string BaseDeviceId => Instance.baseDeviceId;
        public static string DeviceId => Instance.deviceId;
        public static string Salt => Instance.hashsalt;
        public static byte[] CryptoKey => Instance.cryptoKey;
#if !UNITY_EDITOR
        public static string GameName { get; private set; }
#else
        public static string GameName => GetGameName(Application.identifier, Application.productName);
#endif

#if SX_ONLINE
        public static string OnlineDomain => Instance.onlineOptions.onlineDomain;
        public static string GameId => Instance.onlineOptions.gameId;
#endif

        public static Core Instance
        {
            get
            {
#if UNITY_EDITOR
                if (instance == null) CreateMe(CreateInstance<Core>(), typeof(Core).Name);
#endif
                if (instance == null)
                {
                    instance = Resources.Load<Core>("Configs/" + typeof(Core).Name);
                    instance.OnInitialize();
                }
                return instance;
            }
        }

        public static string ComputeMD5(string str, string salt)
        {
            var md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(str + salt);
            byte[] hashBytes = md5.ComputeHash(inputBytes);
            var res = new System.Text.StringBuilder();
            for (int i = 0; i < hashBytes.Length; i++)
                res.Append(hashBytes[i].ToString("X2"));
            return res.ToString();
        }

        public static byte[] Encrypt(byte[] data)
        {
            var res = new byte[data.Length];
            for (int i = 0; i < data.Length; ++i)
                res[i] = (byte)(data[i] + CryptoKey[i % CryptoKey.Length]);
            return res;
        }

        public static byte[] Decrypt(byte[] data)
        {
            var res = new byte[data.Length];
            for (int i = 0; i < data.Length; ++i)
                res[i] = (byte)(data[i] - CryptoKey[i % CryptoKey.Length]);
            return res;
        }

        public static string GetGameName(string identifier, string productName)
        {
            var baseFilename = identifier.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
            return baseFilename.Length > 0 ? baseFilename.Last() : productName.Replace(" ", string.Empty).ToLower();
        }

#if UNITY_EDITOR
        private static void CreateMe(Object instance, string name)
        {
            var path = "/Resources/Configs/";
            var appath = Application.dataPath + path;
            var fileName = path + name + ".asset";
            if (System.IO.File.Exists(Application.dataPath + fileName)) return;
            if (!System.IO.Directory.Exists(appath)) System.IO.Directory.CreateDirectory(appath);
            UnityEditor.AssetDatabase.CreateAsset(instance, "Assets" + fileName);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif


        //////////////////////////////////////////////////////
        /// HELPER CLASSES
        //////////////////////////////////////////////////////
        [System.Serializable]
        public class OnlineOptions
        {
            public string gameId = string.Empty;
            public string onlineDomain = "seganx.com";
        }

        [System.Serializable]
        public class SecurityOptions
        {
            public string cryptokey = "replace crypto key here";
            public string salt = "replace salt";
        }

#if UNITY_EDITOR
        [System.Serializable]
        public class TestDevices
        {
            public bool active = false;
            public int index = 0;
            [Header("List of devices for test"), Space]
            public string[] list = new string[0];
        }
#endif

    }
}