// Analytics integration example for GoogleUniversalAnalytics helper class.
//
// Copyright 2013 Jetro Lauha (Strobotnik Ltd)
// http://strobotnik.com
// http://jet.ro
//
// $Revision: 533 $
//
// File version history:
// 2013-04-26, 1.0.0 - Initial version
// 2013-05-03, 1.0.1 - Automatic events with OnLevelWasLoaded.
// 2013-09-01, 1.1.1 - Granularized some of the system info statistics.
//                     Different way to generate client ID for Android.
//                     Send 1st launch data only when network is reachable.
// 2013-09-25, 1.1.3 - Unity 3.5 support.
// 2013-12-12, 1.1.4 - Fix for trying to use Handheld class on Windows 8.
// 2013-12-17, 1.2.0 - Added disableAnalyticsByUserOptOut in PlayerPrefs
//                     and use of gua.analyticsDisabled.
// 2014-02-11, 1.3.0 - Changed trackingID to be invalid dummy by default.

using UnityEngine;

namespace SeganX
{

    public class GoogleAnalytics : MonoBehaviour
    {
        public string trackingID_bzr = "UA-123159528-1";
        public string appName_bzr = "Tank Online";
        public string trackingID_ggl = "UA-123159528-1";
        public string appName_ggl = "Tank Arena";


        public string newLevelAnalyticsEventPrefix = "level-";
        public bool useHTTPS = false;

        public string TrackingID { get { return LocalizationService.IsPersian ? trackingID_bzr : trackingID_ggl; } }
        public string AppName { get { return LocalizationService.IsPersian ? appName_bzr : appName_ggl; } }

        private static GoogleUniversalAnalytics gua { get { return GoogleUniversalAnalytics.Instance; } }

        // Private default instance.
        private static GoogleAnalytics instance = null;
        // The default instance as a property.
        public static GoogleAnalytics Instance { get { return instance; } }

        private const string disableAnalyticsByUserOptOutPrefKey = "GoogleUniversalAnalytics_optOut";


        int getPOSIXTime()
        {
            return (int)(System.DateTime.UtcNow - new System.DateTime(1970, 1, 1)).TotalSeconds;
        }

        // Example of a helper method. See commented-out
        // exampleAnalyticsTestHits() at end of this file for
        // more ideas of what to track, and what you could
        // make helper methods for.
        public static void changeScreen(string newScreenName)
        {
            if (gua == null) return;
            gua.sendAppScreenHit(newScreenName);
        }

        public static void Event(string category, string action, string label = null)
        {
            if (gua == null || category.IsNullOrEmpty() || action.IsNullOrEmpty()) return;
            gua.sendEventHit(category, action, label);
        }

        // If analyticsDisabled is true, all analytics is disabled = no hits are sent.
        // If analyticsDisabled is false, analytics are enabled and hits will be sent
        // unless there is no Internet reachability.
        // This setting is persistent (saved to PlayerPrefs).
        public static void setPlayerPref_disableAnalyticsByUserOptOut(bool analyticsDisabled)
        {
            if (gua != null)
                gua.analyticsDisabled = analyticsDisabled;
            PlayerPrefs.SetInt(disableAnalyticsByUserOptOutPrefKey, analyticsDisabled ? 1 : 0);
            PlayerPrefs.Save();
        }


        void Start()
        {
            string clientID = Core.DeviceId;
            Debug.Log("Initialize Google Analytics. ClientID: " + clientID);

            // bool useStringEscaping = true; // see the docs about this

            string appVersion = Application.version;

            gua.initialize(TrackingID, clientID, AppName, appVersion, useHTTPS);
            //gua.setStringEscaping(useStringEscaping); // see the docs about this

            if (PlayerPrefs.HasKey(disableAnalyticsByUserOptOutPrefKey))
                gua.analyticsDisabled = (PlayerPrefs.GetInt(disableAnalyticsByUserOptOutPrefKey, 0) != 0);

            if (!gua.analyticsDisabled)
            {
                // Start by sending a hit with some generic info, including an app
                // screen hit with the first level name, since first scene doesn't get
                // automatically call to OnLevelWasLoaded.
                gua.beginHit(GoogleUniversalAnalytics.HitType.Appview);
                gua.addApplicationVersion();
                gua.addScreenResolution(Screen.currentResolution.width, Screen.currentResolution.height);
                gua.addViewportSize(Screen.width, Screen.height);
                // Note: this adds language e.g. as "English", although Google API example has "en-us".
                gua.addUserLanguage(Application.systemLanguage.ToString());
                //gua.addCustomDimension(1, "ScreenDPI");
                //gua.addCustomMetric(1, (int)Screen.dpi);
                //gua.addContentDescription(newLevelAnalyticsEventPrefix + Application.loadedLevelName);

                gua.sendHit();


                // Next, client SystemInfo statistics are submitted ONCE on the first
                // launch when internet is reachable.

                // If you make a few version upgrades and at some point want to get
                // fresh statistics of your active users, update the category string
                // below and after next update users will re-submit SystemInfo once.
                const string category = "SystemInfo_since_v001";
                const string prefKey = "GoogleUniversalAnalytics_" + category;

                // Existing pref key could be deleted with following command:
                //// PlayerPrefs.DeleteKey(prefKey);
                // Warning: Do not enable that code row here (except for single time
                //          testing). Otherwise all following single time statistics
                //          hits would be sent on each launch.

                if (Application.internetReachability != NetworkReachability.NotReachable &&
                    !PlayerPrefs.HasKey(prefKey))
                {
                    gua.sendEventHit(category, "ScreenDPI", ((int)Screen.dpi).ToString(), (int)Screen.dpi);

                    gua.sendEventHit(category, "operatingSystem", SystemInfo.operatingSystem);
                    gua.sendEventHit(category, "processorType", SystemInfo.processorType);
                    gua.sendEventHit(category, "processorCount", SystemInfo.processorCount.ToString(), SystemInfo.processorCount);
                    // round down to 128MB chunks for label
                    gua.sendEventHit(category, "systemMemorySize", (128 * (SystemInfo.systemMemorySize / 128)).ToString(), SystemInfo.systemMemorySize);
                    // round down to 16MB chunks for label
                    gua.sendEventHit(category, "graphicsMemorySize", (16 * (SystemInfo.graphicsMemorySize / 16)).ToString(), SystemInfo.graphicsMemorySize);
                    gua.sendEventHit(category, "graphicsDeviceName", SystemInfo.graphicsDeviceName);
                    gua.sendEventHit(category, "graphicsDeviceVendor", SystemInfo.graphicsDeviceVendor);
                    gua.sendEventHit(category, "graphicsDeviceID", SystemInfo.graphicsDeviceID.ToString(), SystemInfo.graphicsDeviceID);
                    gua.sendEventHit(category, "graphicsDeviceVendorID", SystemInfo.graphicsDeviceVendorID.ToString(), SystemInfo.graphicsDeviceVendorID);
                    gua.sendEventHit(category, "graphicsDeviceVersion", SystemInfo.graphicsDeviceVersion);
                    gua.sendEventHit(category, "graphicsShaderLevel", SystemInfo.graphicsShaderLevel.ToString(), SystemInfo.graphicsShaderLevel);
                    gua.sendEventHit(category, "deviceType", SystemInfo.deviceType.ToString());
                    // round down to 512 chunks for label
#if !UNITY_3_5
                    gua.sendEventHit(category, "maxTextureSize", (512 * (SystemInfo.maxTextureSize / 512)).ToString(), SystemInfo.maxTextureSize);
                    gua.sendEventHit(category, "supports3DTextures", SystemInfo.supports3DTextures ? "yes" : "no", SystemInfo.supports3DTextures ? 1 : 0);
                    gua.sendEventHit(category, "supportsComputeShaders", SystemInfo.supportsComputeShaders ? "yes" : "no", SystemInfo.supportsComputeShaders ? 1 : 0);
                    gua.sendEventHit(category, "supportsInstancing", SystemInfo.supportsInstancing ? "yes" : "no", SystemInfo.supportsInstancing ? 1 : 0);
                    gua.sendEventHit(category, "npotSupport", SystemInfo.npotSupport.ToString());
#endif
                    gua.sendEventHit(category, "supportsShadows", SystemInfo.supportsShadows ? "yes" : "no", SystemInfo.supportsShadows ? 1 : 0);
                    gua.sendEventHit(category, "supportedRenderTargetCount", SystemInfo.supportedRenderTargetCount.ToString(), SystemInfo.supportedRenderTargetCount);

                    gua.sendEventHit(category, "deviceModel", SystemInfo.deviceModel);

                    gua.sendEventHit(category, "supportsAccelerometer", SystemInfo.supportsAccelerometer ? "yes" : "no", SystemInfo.supportsAccelerometer ? 1 : 0);
                    gua.sendEventHit(category, "supportsGyroscope", SystemInfo.supportsGyroscope ? "yes" : "no", SystemInfo.supportsGyroscope ? 1 : 0);
                    gua.sendEventHit(category, "supportsLocationService", SystemInfo.supportsLocationService ? "yes" : "no", SystemInfo.supportsLocationService ? 1 : 0);
                    gua.sendEventHit(category, "supportsVibration", SystemInfo.supportsVibration ? "yes" : "no", SystemInfo.supportsVibration ? 1 : 0);
                    gua.sendEventHit(category, "supportsImageEffects", SystemInfo.supportsImageEffects ? "yes" : "no", SystemInfo.supportsImageEffects ? 1 : 0);
                    PlayerPrefs.SetInt(prefKey, getPOSIXTime());
                    PlayerPrefs.Save();
                }

            } // !gua.analyticsDisabled
        } // Awake
    }
}
