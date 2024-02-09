using UnityEngine;

namespace SeganX.Widgets
{
    public abstract class TransformerBase : MonoBehaviour
    {
        private Space space;
        private Vector3 startPosition, targetPosition;
        private Quaternion startRotation, targetRotation;
        private Vector3 startScale, targetScale;
        private AnimationCurve easingCurve;

        protected float timer = 0;
        protected float curveDuration = 0;

        protected virtual void Start()
        {
            enabled = false;
        }

        protected virtual void Update()
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime;

                if (timer > 0)
                {
                    var localTimer = curveDuration - timer;
                    var position = Vector3.LerpUnclamped(startPosition, targetPosition, easingCurve.Evaluate(localTimer));
                    var rotation = Quaternion.SlerpUnclamped(startRotation, targetRotation, easingCurve.Evaluate(localTimer));
                    var scale = Vector3.LerpUnclamped(startScale, targetScale, easingCurve.Evaluate(localTimer));
                    SetTransform(position, rotation, scale);
                }
                else
                {
                    SetTransform(targetPosition, targetRotation, targetScale);
                    enabled = false;
                }
            }
        }

        public void StartTransform(Vector3 position, Quaternion rotaion, Vector3 scale, Space space, AnimationCurve easing)
        {
            switch (space)
            {
                case Space.World:
                    startPosition = transform.position;
                    startRotation = transform.rotation;
                    break;
                case Space.Self:
                    startPosition = transform.localPosition;
                    startRotation = transform.localRotation;
                    break;
            }
            startScale = transform.localScale;

            targetPosition = position;
            targetRotation = rotaion;
            targetScale = scale;

            this.space = space;
            easingCurve = easing;
            curveDuration = easingCurve.length > 0 ? easingCurve[easingCurve.length - 1].time : 0;
            timer = curveDuration;
            enabled = true;
        }

        protected void SetTransform(Vector3 position, Quaternion rotaion, Vector3 localScale)
        {
            switch (space)
            {
                case Space.World:
                    transform.SetPositionAndRotation(position, rotaion);
                    break;
                case Space.Self:
                    transform.localPosition = position;
                    transform.localRotation = rotaion;
                    break;
            }

            transform.localScale = localScale;
        }
    }
}