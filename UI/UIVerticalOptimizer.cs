using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class UIVerticalOptimizer : MonoBehaviour
    {
        [SerializeField] private bool manageChild = true;

        private RectTransform rectTransform = null;
        private RectTransform parent = null;

        private void Start()
        {
            rectTransform = transform as RectTransform;
            parent = transform.parent as RectTransform;
        }

        private void Update()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i).AsRectTransform();

                var top = t.anchoredPosition.y;
                var bot = top - t.rect.height;

                var topVisible = bot + rectTransform.anchoredPosition.y < 0;
                var botVisible = top + rectTransform.anchoredPosition.y + parent.rect.height > 0;

                if (manageChild)
                    t.transform.SetChilderenActive(topVisible && botVisible);
                else
                    t.gameObject.SetActive(topVisible && botVisible);
            }
        }
    }
}
