using UnityEngine;

namespace SeganX.Widgets
{
    [RequireComponent(typeof(Renderer))]
    public class AutoScaleOnView : AutoScaleBase
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

            if (IsReferenceAvailable)
                Action();
        }
    }
}