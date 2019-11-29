using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace SeganX
{
    public class Core : StaticConfig<Core>
    {
        [System.Serializable]
        public class SecurityOptions
        {
            public string cryptokey = string.Empty;
            public string salt = string.Empty;
        }

        [SerializeField] private string onlineDomain = "seganx.ir";
        [SerializeField] private SecurityOptions securityOptions = new SecurityOptions();

        private string baseDeviceId = string.Empty;
        private string deviceId = string.Empty;
        private string hashsalt = string.Empty;
        private byte[] cryptoKey = null;

        protected override void OnInitialize()
        {
#if UNITY_EDITOR
            baseDeviceId = TestDevices.DeviceId;
#else
            baseDeviceId = SystemInfo.deviceUniqueIdentifier;
#endif
            deviceId = ComputeMD5(baseDeviceId, securityOptions.salt);
            hashsalt = ComputeMD5(securityOptions.salt, securityOptions.salt);
            cryptoKey = System.Text.Encoding.ASCII.GetBytes(securityOptions.cryptokey);

#if UNITY_EDITOR
#else
            securityOptions.cryptokey = string.Empty;
            securityOptions.salt = string.Empty;
#endif
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static string BaseDeviceId { get { return Instance.baseDeviceId; } }
        public static string DeviceId { get { return Instance.deviceId; } }
        public static string Salt { get { return Instance.hashsalt; } }
        public static byte[] CryptoKey { get { return Instance.cryptoKey; } }
        public static string OnlineDomain { get { return Instance.onlineDomain; } }

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
    }
}