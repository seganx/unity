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
            var ws = new WWW(assembly.CodeBase);
            yield return ws;
            if (ws.isDone && string.IsNullOrEmpty(ws.error))
            {
                var mem = new MemoryStream(ws.bytes);
                var sha = new SHA1Managed();
                byte[] checksum = sha.ComputeHash(mem);
                userheader += BitConverter.ToString(checksum).Replace("-", String.Empty);
            }
        }

        private void DownloadText(string url, string postData, Dictionary<string, string> header, Action<UnityWebRequest> callback)
        {
            StartCoroutine(DoDownloadText(url, postData, header, callback));
        }

        private IEnumerator DoDownloadText(string url, string postdata, Dictionary<string, string> header, Action<UnityWebRequest> callback)
        {
            UnityWebRequest res = postdata == null ? UnityWebRequest.Get(url) : UnityWebRequest.Post(url, postdata);

            if (res.method == UnityWebRequest.kHttpVerbPOST)
            {
                var uploader = new UploadHandlerRaw(postdata.GetBytes());
                uploader.contentType = "application/json";
                res.uploadHandler = uploader;
            }
            res.SetRequestHeader("Accept", "text/html,application/json,application/xml");
            res.SetRequestHeader("Content-Type", "application/json");
            res.SetRequestHeader("Cache-Control", "no-cache, no-store, must-revalidate");
            if (header != null)
                foreach (var item in header)
                    res.SetRequestHeader(item.Key, item.Value);

            //  print the package
            {
                string debug = res.method + " " + url;
                if (header != null) debug += "\nHeader: " + header.GetStringDebug();
                if (postdata != null) debug += "\nPostData:" + postdata;
                Debug.Log(debug);
            }

            var ret = Time.time;
            var req = res.SendWebRequest();
            yield return new WaitUntil(() => req.isDone || (Time.time - ret) > requestTimeout);

            //  print the result
            Debug.Log(
                "Downloaded " + res.downloadedBytes + " Bytes from " + res.method + " " + url +
                "\nHeader: " + res.GetResponseHeaders().GetStringDebug() +
                "\nError" + (res.error.HasContent() ? res.error : "No error") +
                "\n" + res.downloadHandler.text);

            callback(res);
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
                if (instance == null)
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

        private static void PostDownloadText(UnityWebRequest wr, Action<string> callback)
        {
            if (wr.isDone && wr.responseCode == 200)
            {
                status = Status.Ready;
                callback(wr.downloadHandler.text.GetWithoutBOM());
            }
            else if (wr.isHttpError || wr.responseCode != 200)
            {
                status = Status.ServerError;
                callback(null);
            }
            else
            {
                status = Status.NetworkError;
                callback(null);
            }
            wr.Dispose();
        }
    }
}