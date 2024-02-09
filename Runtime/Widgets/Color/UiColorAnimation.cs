using UnityEngine;
using UnityEngine.UI;

namespace SeganX.Widgets
{
    public class UiColorAnimation : ColorAnimationBase<Graphic>
    {
        protected Color initialColor;

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

            Animating();
        }

        protected override void SetColor()
        {
            switch (copyMode)
            {
                case CopyMode.All:
                    target.color = color.Evaluate(time);
                    break;

                case CopyMode.RGB:
                    {
                        var targetColor = color.Evaluate(time);
                        targetColor.a = target.color.a;
                        target.color = targetColor;
                    }
                    break;

                case CopyMode.Alpha:
                    {
                        var targetColor = target.color;
                        targetColor.a = color.Evaluate(time).a;
                        target.color = targetColor;
                    }
                    break;
            }
        }

        protected override void AfterAnimate()
        {
            if (IsReferenceAvailable)
                target.color = initialColor;
        }

        protected override void BeforeAnimate()
        {
            if (IsReferenceAvailable)
                initialColor = target.color;
        }
    }
}