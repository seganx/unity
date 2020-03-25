using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

namespace SeganX
{
    public class Http : MonoBehaviour
    {
        public enum Status { Ready, Downloading, NetworkError, ServerError }

        private void Awake()
        {
            instance = this;
        }

        private IEnumerator Start()
        {
            var assembly = AppDomain.CurrentDomain.Load("Assembly-CSharp");
            userheader = assembly.ManifestModule.ModuleVersionId + ".";
            var req = UnityWebRequest.Get(assembly.CodeBase);
            yield return req.SendWebRequest();
            if (req.isDone && string.IsNullOrEmpty(req.error))
            {
                var mem = new MemoryStream(req.downloadHandler.data);
                var sha = new SHA1Managed();
                byte[] checksum = sha.ComputeHash(mem);
                userheader += BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

        private void DownloadText(string url, string postData, Dictionary<string, string> header, Action<UnityWebRequest> callback, System.Action<float> onProgressCallback = null)
        {
            StartCoroutine(DoDownloadText(url, postData, header, callback, onProgressCallback));
        }

        private IEnumerator DoDownloadText(string url, string postdata, Dictionary<string, string> header, Action<UnityWebRequest> callback, Action<float> onProgressCallback = null)
        {
            UnityWebRequest req = new UnityWebRequest(url);
            req.downloadHandler = new DownloadHandlerBuffer();

            if (postdata != null)
            {
                req.method = UnityWebRequest.kHttpVerbPOST;
                req.uploadHandler = new UploadHandlerRaw(postdata.GetBytes());
                req.uploadHandler.contentType = "application/json";
            }

            req.SetRequestHeader("Accept", "*/*");
            req.SetRequestHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            if (header != null)
                foreach (var item in header)
                    req.SetRequestHeader(item.Key, item.Value);

            //  print the package
            {
                string debug = req.method + " " + url;
                if (header != null) debug += "\nHeader: " + header.GetStringDebug();
                if (postdata != null) debug += "\nPostData:" + postdata;
                Debug.Log(debug);
            }

            //req.timeout = requestTimeout;
            yield return req.SendWebRequest();

            //  print the result
            Debug.Log(
                "Downloaded " + req.downloadedBytes + " Bytes from " + req.method + " " + url +
                "\nHeader: " + req.GetResponseHeaders().GetStringDebug() +
                "\nError: " + (req.error.HasContent() ? req.error : "No error") +
                "\n" + req.downloadHandler.text);

            callback(req);
            req.Dispose();
        }


        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static Status status = Status.Ready;
        public static string userheader = string.Empty;
        public static int requestTimeout = 15;
        private static Http instance = null;

        internal static Http Instance
        {
            get
            {
                if (instance == null && Game.Instance)
                {
                    DontDestroyOnLoad(instance = Game.Instance.gameObject.AddComponent<Http>());
                }
                return instance;
            }
        }

        public static void DownloadText(string url, string postdata, Dictionary<string, string> header, Action<string> callback, Action<float> onProgress = null)
        {
            if (Application.internetReachability != NetworkReachability.NotReachable)
            {
                status = Status.Downloading;
                Instance.DownloadText(url, postdata, header, wr => PostDownloadText(wr, callback));
            }
            else
            {
                status = Status.NetworkError;
                callback(null);
            }
        }

        private static void PostDownloadText(UnityWebRequest req, Action<string> callback)
        {
            if (req.isDone && req.responseCode == 200)
            {
                status = Status.Ready;
                callback(req.downloadHandler.text.GetWithoutBOM());
            }
            else if (req.isHttpError)
            {
                status = Status.ServerError;
                callback(null);
            }
            else
            {
                status = Status.NetworkError;
                callback(null);
            }
            req.Dispose();
        }
    }
}