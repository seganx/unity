using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Scripting;

namespace SeganX
{
    public class Http : MonoBehaviour
    {
        private void Download(Request request)
        {
            StartCoroutine(DoDownload(request));
        }

        private IEnumerator DoDownload(Request request)
        {
            while (request.efforts-- > 0)
            {
                var web = new UnityWebRequest(request.url) { downloadHandler = new DownloadHandlerBuffer() };

                if (request.postData != null)
                {
                    web.method = UnityWebRequest.kHttpVerbPOST;
                    web.uploadHandler = new UploadHandlerRaw(request.postData.GetBytes()) { contentType = "application/json" };
                }

                web.SetRequestHeader("Accept", "*/*");
                web.SetRequestHeader("Cache-Control", "no-cache, no-store, must-revalidate");
                if (request.header != null)
                    foreach (var item in request.header)
                        web.SetRequestHeader(item.Key, item.Value);

                //  print the package
                {
                    string debug = "\n" + web.method + " " + request.url;
                    if (request.header != null) debug += "\nHeader: " + request.header.GetStringDebug();
                    if (request.postData != null) debug += "\nPostData:" + request.postData;
                    Debug.Log(debug + "\n");
                }

                var startTime = Time.unscaledTime;
                request.status = Status.Downloading;
                var operation = web.SendWebRequest();
                while (operation.isDone == false)
                {
                    request.onProgress?.Invoke(operation.progress);

                    if (request.timeout > 0)
                    {
                        var delta = Time.unscaledTime - startTime;
                        if (delta > request.timeout) break;
                    }

                    yield return null;
                }

                //  print the result
                Debug.Log(
                    "\nDownloaded " + web.downloadedBytes + " Bytes from " + web.method + " " + request.url +
                    "\nResponseCode: " + web.responseCode +
                    "\nHeader: " + web.GetResponseHeaders().GetStringDebug() +
                    "\nError: " + (web.error.HasContent() ? web.error : "No error") +
                    "\n" + web.downloadHandler.text + "\n");

                if (web.isDone && web.responseCode >= 200 && web.responseCode < 300)
                {
                    request.efforts = 0;
                    request.status = Status.Done;
                    request.onCompletedText?.Invoke(request.status, web.downloadHandler.text.GetWithoutBOM());
                    request.onCompleted?.Invoke(request.status, web.downloadHandler);
                }
                else
                {
#if UNITY_2020_1_OR_NEWER
                    request.status = web.result == UnityWebRequest.Result.ProtocolError ? Status.ServerError : Status.NetworkError;
#else
                    request.status = web.isHttpError ? Status.ServerError : Status.NetworkError;
#endif
                    if (request.efforts-- > 0)
                    {
                        yield return new WaitForSeconds(1);
                    }
                    else
                    {
                        request.onCompletedText?.Invoke(request.status, null);
                        request.onCompleted?.Invoke(request.status, null);
                    }
                }

                web.Dispose();
            }
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        private static Http instance = null;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            instance = new GameObject("Internet").AddComponent<Http>();
            DontDestroyOnLoad(instance);
        }

        public static void Download(string url, Action<Status, DownloadHandler> onCompleted, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                onCompleted = onCompleted
            };
            instance.Download(request);
        }

        public static void Download(string url, string postdata, Dictionary<string, string> header, Action<Status, DownloadHandler> onCompleted, Action<float> onProgress = null, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                postData = postdata,
                header = header,
                onCompleted = onCompleted,
                onProgress = onProgress
            };
            instance.Download(request);
        }

        public static void DownloadText(string url, string postdata, Dictionary<string, string> header, Action<Status, string> onCompleted, Action<float> onProgress = null, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                postData = postdata,
                header = header,
                onCompletedText = onCompleted,
                onProgress = onProgress
            };
            instance.Download(request);
        }

        public static void DownloadText(string url, string postdata, Dictionary<string, string> header, Action<string> onCompleted, Action<float> onProgress = null, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                postData = postdata,
                header = header,
                onCompletedText = (st, txt) => onCompleted(txt),
                onProgress = onProgress
            };
            instance.Download(request);
        }

        public static void DownloadText(string url, Action<string> onCompleted, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                onCompletedText = (st, txt) => onCompleted(txt),
            };
            instance.Download(request);
        }

        public static void DownloadText(string url, string postdata, Action<string> onCompleted, int timeout = 0, int efforts = 1)
        {
            var request = new Request
            {
                timeout = timeout,
                efforts = efforts,
                url = url,
                postData = postdata,
                onCompletedText = (st, txt) => onCompleted(txt),
            };
            instance.Download(request);
        }

        //////////////////////////////////////////////////////
        /// HELPER CLASSES
        //////////////////////////////////////////////////////
        public enum Status
        {
            Ready,
            Downloading,
            Done,
            NetworkError,
            ServerError
        }

        private class Request
        {
            public Status status = Status.Ready;
            public int timeout = 0;
            public int efforts = 1;
            public string url = string.Empty;
            public string postData = null;
            public Dictionary<string, string> header = null;
            public Action<Status, string> onCompletedText = null;
            public Action<Status, DownloadHandler> onCompleted = null;
            public Action<float> onProgress = null;
        }
    }
}