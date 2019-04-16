using UnityEngine;
using System.Collections;

namespace SeganX
{
    public class SocialAndSharing
    {
#if UNITY_ANDROID
        private class IntentClass
        {
            public AndroidJavaClass iClass = null;
            public AndroidJavaObject iObject = null;
            public AndroidJavaObject currentActivity = null;

            public IntentClass()
            {
                iClass = new AndroidJavaClass("android.content.Intent");
                iObject = new AndroidJavaObject("android.content.Intent");
                AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");
            }

            public IntentClass SetText(string message, string title = "")
            {
                iObject.Call<AndroidJavaObject>("setAction", iClass.GetStatic<string>("ACTION_SEND"));
                if (string.IsNullOrEmpty(title) == false)
                    iObject.Call<AndroidJavaObject>("putExtra", iClass.GetStatic<string>("EXTRA_TITLE"), title);
                iObject.Call<AndroidJavaObject>("putExtra", iClass.GetStatic<string>("EXTRA_TEXT"), message);
                iObject.Call<AndroidJavaObject>("setType", "text/plain");
                return this;
            }

            public IntentClass SetImageFile(string imagePath)
            {
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                iObject.Call<AndroidJavaObject>("setAction", iClass.GetStatic<string>("ACTION_SEND"));
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
                iObject.Call<AndroidJavaObject>("putExtra", iClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
                iObject.Call<AndroidJavaObject>("setType", "image/*");
                return this;
            }

            public IntentClass SetImageUrl(string imageUrl)
            {
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                iObject.Call<AndroidJavaObject>("setAction", iClass.GetStatic<string>("ACTION_SEND"));
                AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", imageUrl);
                iObject.Call<AndroidJavaObject>("putExtra", iClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
                iObject.Call<AndroidJavaObject>("setType", "image/*");
                return this;
            }

            public IntentClass SetRateUs(string marketPackage, string uri)
            {
                Debug.Log("RateUs: " + marketPackage + " " + uri);
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                iObject.Call<AndroidJavaObject>("setAction", iClass.GetStatic<string>("ACTION_EDIT"));
                iObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", uri));
                if (marketPackage.Contains("."))
                    iObject.Call<AndroidJavaObject>("setPackage", marketPackage);
                return this;
            }

            public IntentClass SetView(string packName, string uri)
            {
                AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
                iObject.Call<AndroidJavaObject>("setAction", iClass.GetStatic<string>("ACTION_VIEW"));
                iObject.Call<AndroidJavaObject>("setData", uriClass.CallStatic<AndroidJavaObject>("parse", uri));
                if (packName.Contains("."))
                    iObject.Call<AndroidJavaObject>("setPackage", packName);
                return this;
            }

            public IntentClass Start()
            {
                currentActivity.Call("startActivity", iObject);
                return this;
            }
        }
#endif

        public static void ShareText(string message, string title = "")
        {
#if UNITY_ANDROID
            new IntentClass().SetText(message, title).Start();
#endif
        }

        public static void ShareTextAndImageFile(string message, string title, string filename)
        {
#if UNITY_ANDROID
            new IntentClass().SetText(message, title).SetImageFile(filename).Start();
#endif
        }

        public static void ShareTextAndImageUrl(string message, string title, string imageUrl)
        {
#if UNITY_ANDROID
            new IntentClass().SetText(message, title).SetImageUrl(imageUrl).Start();
#endif
        }

        public static void RateUs(string marketPackage, string rateUsLink)
        {
#if UNITY_ANDROID
            if (marketPackage.IsNullOrEmpty())
#if BAZAAR
                marketPackage = "com.farsitel.bazaar";
#else
                Application.OpenURL(rateUsLink);
            else 
#endif

            if (marketPackage == "view")
                new IntentClass().SetView(marketPackage, rateUsLink).Start();
            else
                new IntentClass().SetRateUs(marketPackage, rateUsLink).Start();
#endif
        }

        public static void OpenTelegram(string username)
        {
            Application.OpenURL("https://telegram.me/" + username);
        }

        public static void SendEmail(string emailAddress, string subject, string body)
        {
            if (emailAddress.Length < 5) return;
            Application.OpenURL("mailto:" + emailAddress + "?subject=" + subject.EscapeURL() + "&body=" + body.EscapeURL());
        }
    }
}