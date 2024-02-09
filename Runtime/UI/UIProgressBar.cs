using UnityEngine;

namespace SeganX
{
    public class UIProgressBar : Base
    {
        [SerializeField] RectTransform anchor = null;
        [SerializeField] RectTransform bar = null;
        [SerializeField, VectorLabels("Left", "Right")] Vector2 barBound = new Vector2(5, 5);
        [SerializeField] float value = 0.5f;

        public float Value
        {
            get
            {
                return value;
            }

            set
            {
                this.value = Mathf.Clamp01(value);
                var width = anchor.rect.width - barBound.y;
                var newsize = new Vector2(width * this.value, bar.sizeDelta.y);
                if (newsize.x > barBound.x)
                {
                    bar.gameObject.SetActive(true);
                    bar.sizeDelta = newsize;
                }
                else bar.gameObject.SetActive(false);
            }
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            Value = value;
        }

        private void OnRectTransformDimensionsChange()
        {
            OnValidate();
        }
#endif
    }
}

