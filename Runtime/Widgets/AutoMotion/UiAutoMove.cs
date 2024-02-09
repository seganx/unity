using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Widgets
{
    public class UiAutoMove : AutoMotionBase<UiAutoMove>
    {
        protected Vector3 initialPosition;

        protected override void Reset()
        {
            base.Reset();
            rangeX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
            rangeY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
            rangeZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        }

        protected override void BeforAction()
        {
            initialPosition = target.As<RectTransform>().anchoredPosition;
        }

        protected override void Action()
        {
            timer += deltaTime;
            var range = new Vector3(rangeX.Evaluate(timer), rangeY.Evaluate(timer), rangeZ.Evaluate(timer));
            target.As<RectTransform>().anchoredPosition = initialPosition + range;
        }

        protected override void AfterAction()
        {
            timer = 0;
            target.As<RectTransform>().anchoredPosition = initialPosition;
        }

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