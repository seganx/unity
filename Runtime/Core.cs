using UnityEngine;

namespace SeganX
{
    public class Core : StaticConfig<Core>
    {
#if UNITY_EDITOR
        [SerializeField] private TestDevices testDevices = new();
#endif
        protected override void OnInitialize()
        {
#if UNITY_EDITOR
            deviceId = Instance.testDevices.DeviceId;
            if (string.IsNullOrEmpty(deviceId))
                deviceId = ComputeMD5(SystemInfo.deviceUniqueIdentifier, Application.identifier);
#else
            deviceId = ComputeMD5(SystemInfo.deviceUniqueIdentifier, Application.identifier);
#endif

            salt = ComputeMD5(deviceId, Application.identifier);
            cryptoKey = System.Text.Encoding.ASCII.GetBytes(ComputeMD5(Application.identifier + deviceId, salt) + deviceId + SystemInfo.deviceUniqueIdentifier);

            var versions = Application.version.Split('.');
            versionMajor = versions[0].ToInt();
            versionMinor = versions[1].ToInt();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeOnLoad()
        {
            var instance = Instance;
            Debug.Log($"[Core] initialized version:{instance.version}");
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static string deviceId;
        public static string salt;
        public static byte[] cryptoKey;
        public static int versionMajor;
        public static int versionMinor;

        private static string ComputeMD5(string str, string salt)
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
                res[i] = (byte)(data[i] + cryptoKey[i % cryptoKey.Length]);
            return res;
        }

        public static byte[] Decrypt(byte[] data)
        {
            var res = new byte[data.Length];
            for (int i = 0; i < data.Length; ++i)
                res[i] = (byte)(data[i] - cryptoKey[i % cryptoKey.Length]);
            return res;
        }

        public static string GetGameName(string identifier, string productName)
        {
            var baseFilename = identifier.Split(new char[] { '.' }, System.StringSplitOptions.RemoveEmptyEntries);
            return baseFilename.Length > 0 ? baseFilename[baseFilename.Length - 1] : productName.Replace(" ", string.Empty).ToLower();
        }


        //////////////////////////////////////////////////////
        /// NESTED MEMBERS
        //////////////////////////////////////////////////////
#if UNITY_EDITOR
        [System.Serializable]
        public class TestDevices
        {
            [SerializeField] private int selectedIndex = -1;
            [SerializeField] private DeviceInfo[] devices = new DeviceInfo[0];
            public string DeviceId => (selectedIndex >= 0) ? devices[selectedIndex].deviceId : null;

            //////////////////////////////////////////////////////
            /// NESTED MEMBERS
            //////////////////////////////////////////////////////
            [System.Serializable]
            public class DeviceInfo
            {
                [TextArea]
                public string description;
                public string deviceId;
            }
        }
#endif

    }
}