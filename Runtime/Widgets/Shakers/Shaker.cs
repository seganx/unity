using UnityEngine;

namespace SeganX.Widgets
{
    public class Shaker : ShakerBase
    {
        [SerializeField] private Vector3 rotateRange = Vector3.zero;
        [SerializeField] private Vector3 moveRange = Vector3.zero;

        private Vector3 initialPosition = Vector3.zero;
        private Quaternion initialRotation = Quaternion.identity;

        protected override void BeforeShake()
        {
            if (IsReferenceAvailable)
            {
                base.BeforeShake();
                initialPosition = target.localPosition;
                initialRotation = target.localRotation;
            }
        }

        protected override void Shaking()
        {
            if (IsReferenceAvailable)
            {
                ShakePosition();
                ShakeRotation();
                base.Shaking();
            }
        }

        protected override void AfterShake()
        {
            if (IsReferenceAvailable)
            {
                base.AfterShake();
                if (moveRange.Any())
                    target.localPosition = initialPosition;
                if (rotateRange.Any())
                    target.localRotation = initialRotation;
            }
        }

        private void ShakePosition()
        {
            if (IsReferenceAvailable && moveRange.Any())
            {
                switch (mode)
                {
                    case Mode.Sinus:
                            target.localPosition = initialPosition + Mathf.Min(shakeTime, 1) * RandomBySinus(Timer, moveRange);
                        break;
                    case Mode.Perlin:
                            target.localPosition = initialPosition + Mathf.Min(shakeTime, 1) * RandomByPerlin(Timer, moveRange);
                        break;
                }
            }
        }

        private void ShakeRotation()
        {
            if (IsReferenceAvailable && rotateRange.Any())
            {
                switch (mode)
                {
                    case Mode.Sinus:
                        {
                            Vector3 rot = RandomBySinus(Timer, rotateRange);
                            target.localRotation = initialRotation * Quaternion.Euler(0.5f * Mathf.Min(shakeTime, 1) * rot);
                        }
                        break;
                    case Mode.Perlin:
                        {
                            Vector3 rot = RandomByPerlin(Timer, rotateRange);
                            target.localRotation = initialRotation * Quaternion.Euler(0.5f * Mathf.Min(shakeTime, 1) * rot);
                        }
                        break;
                }
            }
        }
    }
}