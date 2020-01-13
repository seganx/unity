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

        private void DownloadText(string url, string postData, Dictionary<string, string> header, Action<WWW> callback, System.Action<float> onProgressCallback = null)
        {
            StartCoroutine(DoDownloadText(url, postData, header, callback, onProgressCallback));
        }

        private IEnumerator DoDownloadText(string url, string postdata, Dictionary<string, string> header, Action<WWW> callback, Action<float> onProgressCallback = null)
        {
            WWW res = null;

            // handle reqest delay time
            {
                var deltaTime = Time.time - requestTime;
                requestTime = Time.time;
                yield return new WaitForSeconds(Mathf.Clamp01(1.5f - deltaTime));
            }

            if (postdata.HasContent() || header != null)
            {
                if (header == null)
                    header = new Dictionary<string, string>();

                if (header.ContainsKey("Content-Type") == false)
                {
                    header.Add("Accept", "*/*");
                    header.Add("Content-Type", "application/json");
                    header.Add("Cache-Control", "no-cache, no-store, must-revalidate");
                }

                Debug.Log("Getting data from " + url + "\nHeader: " + header.GetStringDebug() + "\nPostData:" + postdata);
                res = new WWW(url, postdata.HasContent() ? postdata.GetBytes() : null, header);
            }
            else
            {
                Debug.Log("Getting data from " + url);
                res = new WWW(url);
            }

            var ret = Time.time;
            if (onProgressCallback != null)
            {
                while (res.keepWaiting && (Time.time - ret) < requestTimeout)
                {
                    onProgressCallback(res.progress);
                    yield return null;
                }
            }
            else yield return new WaitUntil(() => res.isDone || (Time.time - ret) > requestTimeout);

            if (res.isDone)
                Debug.Log("Received " + res.bytesDownloaded + " bytes from " + url + ":\nHeader:" + res.responseHeaders.GetStringDebug() + "\n" + res.text);
            else
                Debug.Log("Failed to download from " + url);
            callback(res);
#if OFF
            UnityWebRequest res = postdata == null ? UnityWebRequest.Get(url) : UnityWebRequest.Post(url, postdata);
            if (res.method == UnityWebRequest.kHttpVerbPOST)
            {
                var uploader = new UploadHandlerRaw(postdata.GetBytes());
                uploader.contentType = "application/json";
                res.uploadHandler = uploader;
            }
            res.SetRequestHeader("Accept", "text/html,application/json,application/xml");
            res.SetRequestHeader("Content-Type", "application/json");
            res.SetRequestHeader("Cache-Control", "no-cache");
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
#endif
        }


        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static Status status = Status.Ready;
        public static string userheader = string.Empty;
        public static int requestTimeout = 15;
        private static float requestTime = 0;
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

        private static void PostDownloadText(WWW ws, Action<string> callback)
        {
            if (ws.isDone && ws.responseHeaders != null && ws.responseHeaders.ContainsKey("STATUS") && ws.responseHeaders["STATUS"].Contains("200"))
            {
                status = Status.Ready;
                callback(ws.text.GetWithoutBOM());
            }
            else if (ws.error.HasContent() && ws.error[0] == '5')
            {
                status = Status.ServerError;
                callback(null);
            }
            else
            {
                status = Status.NetworkError;
                callback(null);
            }
            ws.Dispose();
        }
    }
}