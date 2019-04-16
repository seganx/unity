using UnityEngine;
using System.Collections;
using System.IO;

namespace SeganX.Console
{
    public class Screenshot : MonoBehaviour
    {
        public void CaptureAndShare()
        {
            StopCoroutine(DoCaptureAndShare());
            StartCoroutine(DoCaptureAndShare());
        }

        IEnumerator DoCaptureAndShare()
        {
            var filename = Application.temporaryCachePath + "/ConsoleScreenshot.png";
            try { if (File.Exists(filename)) File.Delete(filename); }
            catch { }

            yield return new WaitForEndOfFrame();
            ScreenCapture.CaptureScreenshot(filename);

            while (File.Exists(filename) == false)
                yield return new WaitForEndOfFrame();
            Share(filename);
        }

        private void Share(string filename)
        {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            Application.OpenURL(filename);

#endif

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            //instantiate the class Intent
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");

            //instantiate the object Intent
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

            //call setAction setting ACTION_SEND as parameter
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));

            //instantiate the class Uri
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");

            //instantiate the object Uri with the parse of the url's file
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + filename);

            //call putExtra with the uri object of the file
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

            //set the type of file
            intentObject.Call<AndroidJavaObject>("setType", "image/png");

            //instantiate the class UnityPlayer
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");

            //instantiate the object currentActivity
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            //call the activity with our Intent
            currentActivity.Call("startActivity", intentObject);
        }
        catch { }
#elif UNITY_IOS && !UNITY_EDITOR
#endif
        }
    }
}