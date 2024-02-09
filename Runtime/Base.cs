using System.Collections;
using UnityEngine;

namespace SeganX
{
    public class Base : MonoBehaviour
    {
#pragma warning disable IDE1006 // Naming Styles
        public RectTransform rectTransform => transform as RectTransform;
        public RectTransform parentRectTransform => transform.parent as RectTransform;
#pragma warning restore IDE1006 // Naming Styles

        public virtual bool Visible
        {
            get => gameObject.activeSelf;
            set => gameObject.SetActive(value);
        }

        public void DestroyGameObject(float delay = 0)
        {
            Destroy(gameObject, delay);
        }

        public void DestroyScript(float delay)
        {
            Destroy(this, delay);
        }

        public void DelayCall(float seconds, System.Action callback)
        {
            if (seconds > 0.001f)
                StartCoroutine(DoDelayCall(seconds, callback));
            else
                callback();
        }

        private IEnumerator DoDelayCall(float seconds, System.Action callback)
        {
            yield return new WaitForSeconds(seconds);
            callback();
        }
    }
}
