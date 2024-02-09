using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Widgets
{
    public abstract class ShakerBase : MonoBehaviour
    {
        public enum Mode { Sinus, Perlin }

        [SerializeField] protected Transform target = null;
        [SerializeField] protected Mode mode = Mode.Sinus;
        [SerializeField] private float speed = 8;
        [SerializeField] protected Vector3 scaleRange = Vector3.zero;

        private float timeOffset = 0;
        protected float shakeTime = 0;
        protected Vector3 initialScale;

        protected float Timer => (Time.time + timeOffset) * speed;

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

            enabled = false;
            timeOffset = Random.value * 100;
        }

        private void OnDestroy()
        {
            if (all.Contains(this))
                all.Remove(this);
        }

        protected virtual void Update()
        {
            if (shakeTime < 0) return;
            shakeTime -= Time.unscaledDeltaTime;

            if (shakeTime > 0)
            {
                Shaking();
            }
            else
            {
                AfterShake();
                enabled = false;
            }
        }

        public void Shake(float seconds)
        {
            enabled = true;
            if (shakeTime < Mathf.Epsilon)
                BeforeShake();
            shakeTime = seconds;
        }

        protected virtual void BeforeShake()
        {
            if (IsReferenceAvailable)
                initialScale = target.localScale;
        }

        protected virtual void Shaking()
        {
            if (IsReferenceAvailable && scaleRange.Any())
            {
                switch (mode)
                {
                    case Mode.Sinus:
                        target.localScale = initialScale + Mathf.Sin(Timer * 5) * Mathf.Min(shakeTime, 1) * scaleRange;
                        break;
                    case Mode.Perlin:
                        target.localScale = initialScale + PerlinNoise(Timer, Timer) * Mathf.Min(shakeTime, 1) * scaleRange;
                        break;
                }
            }
        }

        protected virtual void AfterShake()
        {
            if (IsReferenceAvailable && scaleRange.Any())
                target.localScale = initialScale;
        }

        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static readonly List<ShakerBase> all = new List<ShakerBase>(8);

        protected static float PerlinNoise(float x, float y)
        {
            return 2 * Mathf.PerlinNoise(x, y) - 1;
        }

        protected static Vector3 RandomBySinus(float time, Vector3 range)
        {
            Vector3 result;
            result.x = Mathf.Sin(time * 4.5f) * range.x;
            result.y = Mathf.Sin(time * 5.0f) * range.y;
            result.z = Mathf.Sin(time * 5.5f) * range.z;
            return result;
        }

        protected static Vector3 RandomByPerlin(float time, Vector3 range)
        {
            Vector3 result;
            result.x = PerlinNoise(time + 1, time) * range.x;
            result.y = PerlinNoise(time + 2, time) * range.y;
            result.z = PerlinNoise(time + 3, time) * range.z;
            return result;
        }

        public static void ShakeAll(float seconds)
        {
            foreach (var item in all)
                item.Shake(seconds);
        }

        public static void ShakeAll(float seconds, string tag)
        {
            foreach (var item in all)
                if (item.CompareTag(tag))
                    item.Shake(seconds);
        }
    }
}