using SeganX.Spline;
using UnityEngine;

namespace SeganX.Widgets
{
    public class AutoMoveOnPath : BroadcastableBase<AutoMoveOnPath>
    {
        [SerializeField] protected bool autoStart = true;
        [SerializeField] protected Transform target = null;
        [SerializeField] protected Spline.Spline path;
        [SerializeField] protected float speed = 1;
        [SerializeField] protected bool lookAtPathDirection = true;

        protected Vector3 initialPosition;
        private float distanceInSpline;

        protected bool IsReferenceAvailable
        {
            get
            {
#if UNITY_EDITOR
                if (target == null)
                    Debug.LogWarning($"{name}:{GetType().Name}: Target in empty!");
#endif
                return target != null;
            }
        }

        protected virtual void Reset()
        {
            if (target == null)
                target = transform;
        }

        protected virtual void Start()
        {
            if (all.Contains(this) == false)
                all.Add(this);

            if (autoStart)
                Play();
            else
                enabled = false;
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

        protected void OnDestroy()
        {
            if (all.Contains(this))
                all.Remove(this);
        }

        public override void Play(bool reset = false)
        {
            if (IsReferenceAvailable)
            {
                BeforeAction();
                enabled = true;
                if (reset) distanceInSpline = 0;
            }
        }

        public override void Stop()
        {
            enabled = false;
            if (IsReferenceAvailable)
                AfterAction();
        }

        protected void BeforeAction()
        {
            distanceInSpline = 0;
            if (IsReferenceAvailable)
                initialPosition = target.localPosition;
        }

        protected void Action()
        {
            distanceInSpline += deltaTime * speed;
            SetPositionByDistance();
        }

        protected void AfterAction()
        {
            distanceInSpline = 0;
            target.localPosition = initialPosition;
        }

        private void SetPositionByDistance()
        {
            var point = path.GetPoint(distanceInSpline);
            target.position = point.position;
            if (lookAtPathDirection) target.LookDirection(point.forward, point.upward);
            if (distanceInSpline > path.Length) distanceInSpline = 0;
        }
    }
}