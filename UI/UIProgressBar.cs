using UnityEngine;

namespace SeganX
{
    public class UIProgressBar : Base
    {
        private float initwidth = -1;


        public void Set(float value, float maxValue)
        {
            if (initwidth <= 0)
                initwidth = rectTransform.rect.width;

            if (Mathf.Approximately(maxValue, 0) || Mathf.Approximately(initwidth, 0)) return;

            var width = Mathf.Clamp(value * initwidth / maxValue, 0, initwidth);
            transform.SetAnchordWidth(width);
        }
    }
}

