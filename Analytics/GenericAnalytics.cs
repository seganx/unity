using UnityEngine;
using System.Collections;
using System;

public class GenericAnalytics : MonoBehaviour
{

    DateTime sessionStartTime;

    bool firstSession;

    // Use this for initialization
    void Start()
    {

        sessionStartTime = DateTime.Now;

        firstSession = PlayerPrefs.GetInt("FirstSession", 1) == 1;

        float hours = (float)DateTime.Now.TimeOfDay.TotalHours;

        GoogleUniversalAnalytics.Instance.beginHit(GoogleUniversalAnalytics.HitType.Event);
        GoogleUniversalAnalytics.Instance.addEventCategory("Usage");
        GoogleUniversalAnalytics.Instance.addEventAction("Time Of Day");
        GoogleUniversalAnalytics.Instance.addEventLabel(Mathf.RoundToInt((float)hours).ToString());
        GoogleUniversalAnalytics.Instance.addEventValue(Mathf.RoundToInt((float)hours));
        GoogleUniversalAnalytics.Instance.sendHit();


    }

    void OnApplicationPause(bool paused)
    {
        TrackSession(paused);
    }

    void OnApplicationQuit()
    {
        TrackSession(true);
    }

    void TrackSession(bool ending)
    {
        GoogleUniversalAnalytics.Instance.beginHit(GoogleUniversalAnalytics.HitType.Appview);
        GoogleUniversalAnalytics.Instance.addSessionControl(!ending);
        GoogleUniversalAnalytics.Instance.sendHit();

        if (firstSession)
        {
            firstSession = false;
            PlayerPrefs.SetInt("FirstSession", 0);

            GoogleUniversalAnalytics.Instance.beginHit(GoogleUniversalAnalytics.HitType.Event);
            GoogleUniversalAnalytics.Instance.addEventCategory("Usage");
            GoogleUniversalAnalytics.Instance.addEventAction("First Session Length");
            GoogleUniversalAnalytics.Instance.addEventValue(Mathf.RoundToInt((float)(DateTime.Now - sessionStartTime).TotalSeconds));
            GoogleUniversalAnalytics.Instance.sendHit();
        }
    }
}
