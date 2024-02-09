#if SX_CAMFX
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
        private float lastfps = 0;

        private void Awake()
        {
            IsRunning = true;
        }

        private IEnumerator Start()
        {
            CameraFX.Resolution = 100;
            CameraFX.Bloom = true;
            CameraFX.Quality = 0;
            Step = 1;
            Fps = 0;

            yield return new WaitUntil(() => CameraFX.Activated);

            var waitfor = new WaitForSeconds(sampleTime + 0.1f);
            yield return waitfor;
            int counter = 10;
            while (IsRunning && counter-- > 0)
            {
                yield return waitfor;
                if (Mathf.Abs(lastfps - Fps) < 5)
                {
                    Step++;
                    if (Fps < 45)
                    {
                        if (CameraFX_Reflection.Activate)
                        {
                            CameraFX_Reflection.Activate = false;
                            var refcam = FindObjectOfType<CameraFX_Reflection>();
                            if (refcam != null) Destroy(refcam.gameObject);
                        }
                        else if (CameraFX.Resolution > 80)
                            CameraFX.Resolution -= 10;
                        else if (CameraFX.Quality == 0)
                            CameraFX.Quality = 1;
                        else if (CameraFX.Bloom)
                            CameraFX.Bloom = false;
                        else if (CameraFX.Resolution > 60)
                            CameraFX.Resolution -= 10;
                        else
                            IsRunning = false;

                        Debug.Log("Current FPS:" + Fps + " Resolution:" + CameraFX.Resolution + " Quality :" + CameraFX.Quality + " Bloom :" + CameraFX.Bloom);
                    }
                    else IsRunning = false;

                }
                lastfps = Fps;
            }
            IsRunning = false;
        }

        private void Update()
        {
            ++frameCounter;
            timeCounter += Time.unscaledDeltaTime;
            if (timeCounter >= sampleTime)
            {
                Fps = frameCounter / sampleTime;
                timeCounter = 0;
                frameCounter = 0;
            }
        }

        ////////////////////////////////////////////////////////////
        /// STATIC MEMBERS
        ////////////////////////////////////////////////////////////
        public static bool IsRunning { get; set; }
        public static float Fps { get; set; }
        public static int Step { get; set; }

    }
}
#endif