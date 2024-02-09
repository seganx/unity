using UnityEngine;

namespace SeganX.Widgets
{
    public abstract class AutoRotateBase : AutoMotionBase<AutoRotateBase>
    {
        [SerializeField] protected Space space = Space.Self;
        [SerializeField] protected bool additive = false;

        protected Quaternion initialRotation;

        protected override void Reset()
        {
            base.Reset();
            rangeX = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 360));
            rangeY = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 360));
            rangeZ = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 360));
        }

        protected override void BeforAction()
        {
            if (additive)
            {

            }
            else
            {
                switch (space)
                {
                    case Space.World: initialRotation = target.rotation; break;
                    case Space.Self: initialRotation = target.localRotation; break;
                }
            }
        }

        protected override void Action()
        {
            timer += deltaTime;
            if (additive)
            {
                transform.Rotate(rangeX.Evaluate(timer) * deltaTime,
                                 rangeY.Evaluate(timer) * deltaTime,
                                 rangeZ.Evaluate(timer) * deltaTime, space);
            }
            else
            {
                switch (space)
                {
                    case Space.World: transform.rotation = Quaternion.Euler(rangeX.Evaluate(timer), rangeY.Evaluate(timer), rangeZ.Evaluate(timer)); break;
                    case Space.Self: transform.localRotation = Quaternion.Euler(rangeX.Evaluate(timer), rangeY.Evaluate(timer), rangeZ.Evaluate(timer)); break;
                }
            }

            if (curvesAreClamped && timer > curvesMaxTimeLenght)
            {
                Stop();
            }
        }

        protected override void AfterAction()
        {
            timer = 0;
            if (additive)
            {

            }
            else
            {
                switch (space)
                {
                    case Space.World: target.rotation = initialRotation; break;
                    case Space.Self: target.localRotation = initialRotation; break;
                }
            }
        }
    }
}