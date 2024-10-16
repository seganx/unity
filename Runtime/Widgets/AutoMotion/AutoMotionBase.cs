﻿using UnityEngine;

namespace SeganX
{
    public abstract class AutoMotionBase<T> : BroadcastableBase<AutoMotionBase<T>> where T : AutoMotionBase<T>
    {
        [SerializeField] protected bool autoStart = true;

        [SerializeField] protected Transform target = null;
        [SerializeField] protected ParticleSystem.MinMaxCurve rangeX, rangeY, rangeZ;
        //[SerializeField] protected AnimationCurve rangeX, rangeY, rangeZ;

        protected float timer = 0;
        protected bool modesAreCurve = false;
        protected bool curvesAreClamped = false;
        protected float curvesMaxTimeLenght = 0;

        public virtual bool IsPlaying { get; protected set; } = false;

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
            curvesMaxTimeLenght = Mathf.Max(GetTimeLenght(rangeX.curve), GetTimeLenght(rangeY.curve), GetTimeLenght(rangeZ.curve));
            curvesAreClamped = CurveIsClamped(rangeX.curve.postWrapMode) && CurveIsClamped(rangeY.curve.postWrapMode) && CurveIsClamped(rangeZ.curve.postWrapMode);
            modesAreCurve = rangeX.mode == ParticleSystemCurveMode.Curve || rangeY.mode == ParticleSystemCurveMode.Curve || rangeZ.mode == ParticleSystemCurveMode.Curve;

            if (all.Contains(this) == false)
                all.Add(this);

            if (autoStart)
                Play();
            else
                enabled = false;
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
                IsPlaying = true;
                BeforAction();
                enabled = true;
                if (reset) timer = 0;
            }
        }

        public override void Stop()
        {
            if (IsPlaying == false) return;
            IsPlaying = false;

            enabled = false;
            if (IsReferenceAvailable)
                AfterAction();
        }

        protected abstract void BeforAction();
        protected abstract void Action();
        protected abstract void AfterAction();


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        protected static float GetTimeLenght(AnimationCurve curve)
        {
            return curve.keys.Length > 0 ? curve.keys[curve.length - 1].time : 0;
        }

        protected static bool CurveIsClamped(WrapMode wrapMode)
        {
            return wrapMode == WrapMode.ClampForever || wrapMode == WrapMode.Clamp || wrapMode == WrapMode.Once;
        }
    }
}