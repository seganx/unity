using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public class UIVerticalOptimizer : MonoBehaviour
    {
        //private RectTransform parentOfParent = null;
        private RectTransform parent = null;

        private void Start()
        {
            parent = transform.parent.AsRectTransform();
            //parentOfParent = parent.parent.AsRectTransform();
        }

        private void Update()
        {

            for (int i = 0; i < transform.childCount; i++)
            {
                var t = transform.GetChild(i).AsRectTransform();

                var top = t.anchoredPosition.y;
                var bot = top - t.rect.height;

                var topVisible = bot + parent.anchoredPosition.y < 0;
                var botVisible = top + parent.anchoredPosition.y + parent.rect.height > 0;

                t.gameObject.SetActive(topVisible && botVisible);
            }
        }
    }
}
