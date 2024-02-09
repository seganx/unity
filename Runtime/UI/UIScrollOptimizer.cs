using UnityEngine;

namespace SeganX
{
    public class UIScrollOptimizer : MonoBehaviour
    {
        private enum ScrollDirection
        {
            Horisontal,
            Vertical
        }

        [SerializeField] private Vector2 extraOffset = Vector2.zero;
        [SerializeField] private bool manageChild = true;
        [SerializeField] private ScrollDirection scrollDirection = ScrollDirection.Vertical;

        private RectTransform rectTransform = null;
        private RectTransform parent = null;

        private void Start()
        {
            rectTransform = transform as RectTransform;
            parent = transform.parent as RectTransform;
        }

        private void OnEnable()
        {
            if (this != null && gameObject != null && gameObject.activeSelf)
            {
                UpdateChilderen();
                Invoke("OnEnable", 0.25f);
            }
        }

        private void UpdateChilderen()
        {
            if (rectTransform == null || parent == null) return;

            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i).AsRectTransform();

                var topOrLeft = scrollDirection == ScrollDirection.Vertical ? t.anchoredPosition.y : t.anchoredPosition.x;
                var botOrRight = topOrLeft + (scrollDirection == ScrollDirection.Vertical ? -t.rect.height : t.rect.width);

                var topOrLeftVisible = (scrollDirection == ScrollDirection.Vertical ? botOrRight + rectTransform.anchoredPosition.y < 0 : botOrRight + rectTransform.anchoredPosition.x > 0);
                var botVisible = (scrollDirection == ScrollDirection.Vertical ? topOrLeft + rectTransform.anchoredPosition.y + (parent.rect.height + extraOffset.y) > 0 : topOrLeft + rectTransform.anchoredPosition.x - (parent.rect.width + extraOffset.x) < 0);

                if (manageChild)
                    t.transform.SetChilderenActive(topOrLeftVisible && botVisible);
                else
                    t.gameObject.SetActive(topOrLeftVisible && botVisible);
            }
        }
    }
}
