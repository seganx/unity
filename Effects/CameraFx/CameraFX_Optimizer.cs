using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Effects
{
    public class CameraFX_Optimizer : MonoBehaviour
    {
        private const float sampleTime = 1;

        private float timeCounter = 0;
        private int frameCounter = 0;
        private float fps = 0;
        private float lastfps = 0;

        private void Awake()
        {
            IsRunning = true;
        }

        private IEnumerator Start()
        {
            CameraFX.Resolution = 100;
            CameraFX.Bloom = true;
            yield return new WaitUntil(() => CameraFX.Activated);
            var waitfor = new WaitForSeconds(sampleTime + 0.1f);
            yield return waitfor;
            int counter = 10;
            while (counter-- > 0)
            {
                yield return waitfor;
                if (Mathf.Abs(lastfps - fps) < 5)
                {
                    if (fps < 55)
                    {
                        if (CameraFX_Reflection.Activate)
                        {
                            CameraFX_Reflection.Activate = false;
                            var refcam = FindObjectOfType<CameraFX_Reflection>();
                            if (refcam != null) Destroy(refcam.gameObject);
                        }
                        else if (CameraFX.Resolution > 40)
                            CameraFX.Resolution -= 10;
                        else if (CameraFX.Bloom)
                            CameraFX.Bloom = false;
                        else
                            IsRunning = false;
                    }

                    Debug.Log("Current FPS:" + fps + " Resolution:" + CameraFX.Resolution + " Bloom :" + CameraFX.Bloom);
                }
                lastfps = fps;
            }
            IsRunning = false;
        }

        private void Update()
        {
            ++frameCounter;
            timeCounter += Time.unscaledDeltaTime;
            if (timeCounter >= sampleTime)
            {
                fps = frameCounter / sampleTime;
                timeCounter = 0;
                frameCounter = 0;
            }
        }


        public static bool IsRunning
        {
            get; set;
            //get { return PlayerPrefs.GetInt("CameraFX_Optimizer.IsRunning", 1) > 0; }
            //set { PlayerPrefs.SetInt("CameraFX_Optimizer.IsRunning", value ? 1 : 0); }
        }

    }
}