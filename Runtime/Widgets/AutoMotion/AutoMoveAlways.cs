using UnityEngine;

namespace SeganX.Widgets
{
    public class AutoMoveAlways : AutoMoveBase
    {
        private void Update()
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