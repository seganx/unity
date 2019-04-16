using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SeganX.Console
{
    public class FPS : MonoBehaviour
    {
        public Text fpsLabel = null;

        private float timeCounter = 0;
        private int frameCounter = 0;
        private float fps = 0;

        // Update is called once per frame
        void Update()
        {
            ++frameCounter;
            timeCounter += Time.unscaledDeltaTime;
            if (timeCounter > 0.5f)
            {
                fps = frameCounter * timeCounter * 4;
                fpsLabel.text = "FPS:\n" + fps.ToString("f1");
                fpsLabel.color = (fps >= 30) ? Color.green : ((fps < 10) ? Color.red : Color.yellow);
                timeCounter = 0;
                frameCounter = 0;
            }
        }

        void OnValidate()
        {
            if (fpsLabel == null)
                fpsLabel = GetComponent<Text>();
        }
    }
}