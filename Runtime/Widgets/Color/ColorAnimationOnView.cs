using UnityEngine;

namespace SeganX.Widgets
{
    public class ColorAnimationOnView : ColorAnimation3D
    {
        private void OnWillRenderObject()
        {
            switch (timeMode)
            {
                case TimeScaleMode.ScaledTime:
                    deltaTime = Time.deltaTime;
                    break;
                case TimeScaleMode.UnScaledTime:
                    deltaTime = Time.unscaledDeltaTime;
                    break;
            }

            Animating();
        }
    }
}