using UnityEngine;

namespace SeganX.Widgets
{
    public abstract class AutoScaleBase : AutoMotionBase<AutoScaleBase>
    {
        [SerializeField] protected Space space = Space.Self;

        protected Vector3 initialScale;

        protected override void Reset()
        {
            base.Reset();
            rangeX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
            rangeY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
            rangeZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 0));
        }

        protected override void BeforAction()
        {
            initialScale = target.localScale;
        }

        protected override void Action()
        {
            timer += deltaTime;
            var range = new Vector3(rangeX.Evaluate(timer), rangeY.Evaluate(timer), rangeZ.Evaluate(timer));
            target.localScale = initialScale + range;

            if (curvesAreClamped && timer > curvesMaxTimeLenght)
            {
                Stop();
            }
        }

        protected override void AfterAction()
        {
            timer = 0;
            target.localScale = initialScale;
        }
    }
}