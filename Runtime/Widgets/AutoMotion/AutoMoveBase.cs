using UnityEngine;

namespace SeganX.Widgets
{
    public abstract class AutoMoveBase : AutoMotionBase<AutoMoveBase>
    {
        [SerializeField] protected Space space = Space.Self;

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
            switch (space)
            {
                case Space.World: initialPosition = target.position; break;
                case Space.Self: initialPosition = target.localPosition; break;
            }
        }

        protected override void Action()
        {
            timer += deltaTime;
            var range = new Vector3(rangeX.Evaluate(timer), rangeY.Evaluate(timer), rangeZ.Evaluate(timer));
            switch (space)
            {
                case Space.World: target.position = initialPosition + range; break;
                case Space.Self: target.localPosition = initialPosition + range; break;
            }

            if (curvesAreClamped && timer > curvesMaxTimeLenght)
            {
                Stop();
            }
        }

        protected override void AfterAction()
        {
            timer = 0;
            switch (space)
            {
                case Space.World: target.position = initialPosition; break;
                case Space.Self: target.localPosition = initialPosition; break;
            }
        }
    }
}