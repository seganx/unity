using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace SeganX.Console
{
    public class FPS : MonoBehaviour
    {
        [SerializeField] private Text fpsLabel = null;

        private float timeCounter = 0;
        private int frameCounter = 0;
        public static float current = 0;

        // Update is called once per frame
        void Update()
        {
            ++frameCounter;
            timeCounter += Time.unscaledDeltaTime;
            if (timeCounter > 0.5f)
            {
                current = frameCounter * timeCounter * 4;
                fpsLabel.text = "FPS:\n" + current.ToString("f1");
                fpsLabel.color = (current >= 30) ? Color.green : ((current < 10) ? Color.red : Color.yellow);
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