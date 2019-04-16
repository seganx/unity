using UnityEngine;

namespace SeganX
{
    [ExecuteInEditMode]
    public class UIContentSizeFitter : MonoBehaviour
    {
        [System.Flags]
        public enum Option : int
        {
            Automatic = 1 << 0,
            ItemPosition = 1 << 1,
            ContentSize = 1 << 2
        }

        [EnumFlag]
        public Option options;

        public float padding = 0;
        public float space = 0;
        public float vPading = 0;

        private void Perform()
        {
            if (options.IsFlagOn(Option.ItemPosition))
                UpdatePositions();

            if (options.IsFlagOn(Option.ContentSize))
                FitSize();
        }

        public void UpdatePositions()
        {
            float left = 0;
            float height = padding;
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform rt = transform.GetChild(i) as RectTransform;
                if (rt.gameObject.activeInHierarchy)
                {
                    if (options.IsFlagOn(Option.ItemPosition))
                    {
                        var pos = rt.anchoredPosition;
                        pos.x = rt.anchorMax.x > 0.5f ? -left : left;
                        pos.y = rt.anchorMax.y > 0.5f ? -height : height;
                        rt.anchoredPosition = pos;
                    }
                    left += vPading;
                    height += rt.rect.height + space;
                }
            }
        }

        public void FitSize()
        {
            float height = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                RectTransform rt = transform.GetChild(i) as RectTransform;
                if (rt.gameObject.activeInHierarchy)
                {
                    var r = rt.rect;
                    var dh = Mathf.Abs(r.max.y - r.min.y - rt.anchoredPosition.y);
                    if (height < dh) height = dh;
                }
            }
            transform.SetAnchordHeight(height + padding);
        }

        void Update()
        {
#if UNITY_EDITOR
            Perform();
#else
            if (options.IsFlagOn(Option.Automatic))
                Perform();
#endif
        }
    }
}
