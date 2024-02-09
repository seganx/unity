using UnityEngine;

namespace SeganX.Widgets
{
    public class UiShaker : ShakerBase
    {
        [SerializeField] private Vector2 sizeRange = Vector2.zero;

        private RectTransform rectTransform;
        private Vector2 initialSize;

        protected override void BeforeShake()
        {
            if (IsReferenceAvailable)
            {
                base.BeforeShake();
                rectTransform = target as RectTransform;
                initialSize = rectTransform.sizeDelta;
            }
        }

        protected override void Shaking()
        {
            base.Shaking();

            if (IsReferenceAvailable && sizeRange.Any())
            {

                switch (mode)
                {
                    case Mode.Sinus:
                        rectTransform.SetAnchordSize(initialSize + 0.5f * Mathf.Min(shakeTime, 1) * Mathf.Sin(Timer * 5) * sizeRange);
                        break;
                    case Mode.Perlin:
                        rectTransform.SetAnchordSize(initialSize + (Mathf.PerlinNoise(Timer, Timer) - 0.5f) * Mathf.Min(shakeTime, 1) * sizeRange);
                        break;
                }

            }
        }

        protected override void AfterShake()
        {
            base.AfterShake();
            if (IsReferenceAvailable && sizeRange.Any())
                rectTransform.SetAnchordSize(initialSize);
        }
    }
}