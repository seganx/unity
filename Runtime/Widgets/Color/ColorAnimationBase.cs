using UnityEngine;

namespace SeganX.Widgets
{
    public abstract class ColorAnimationBase<T> : BroadcastableBase<ColorAnimationBase<T>>
    {
        public enum AnimationMode
        {
            PingPong,
            Loop,
            Once
        }

        public enum CopyMode
        {
            All,
            RGB,
            Alpha
        }

        [SerializeField] protected bool autoStart = true;
        [SerializeField] protected AnimationMode animationMode = AnimationMode.PingPong;
        [SerializeField] protected CopyMode copyMode = CopyMode.All;
        [SerializeField] protected ParticleSystem.MinMaxGradient color;
        [SerializeField] protected float speed = 1;
        [SerializeField] protected T target;

        protected float time = 0;
        protected float timeDirection = 1;

        public virtual bool IsPlaying { get; protected set; } = false;

        protected virtual bool IsReferenceAvailable
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

        private void Reset()
        {
            if (target == null)
                target = GetComponent<T>();
        }

        private void Start()
        {
            if (all.Contains(this) == false && IsReferenceAvailable)
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
                BeforeAnimate();
                enabled = true;
                if (reset) time = 0;
            }
        }

        public override void Stop()
        {
            if (IsPlaying == false) return;
            IsPlaying = false;

            enabled = false;
            if (IsReferenceAvailable)
                AfterAnimate();
        }

        protected void SetTimer()
        {
            switch (animationMode)
            {
                case AnimationMode.PingPong:
                    time = Mathf.Clamp(time + timeDirection * speed * deltaTime, 0, 1);
                    if (time >= 1 || time <= 0) timeDirection *= -1;
                    break;

                case AnimationMode.Loop:
                    time += speed * deltaTime;
                    if (time > 1) time = 0;
                    break;

                case AnimationMode.Once:
                    if (time < 1)
                        time = Mathf.Clamp(time + speed * deltaTime, 0, 1);
                    break;
            }
        }

        protected abstract void SetColor();

        protected abstract void BeforeAnimate();

        protected virtual void Animating()
        {
            if (IsReferenceAvailable)
            {
                SetTimer();
                SetColor();
            }
        }

        protected abstract void AfterAnimate();
    }
}