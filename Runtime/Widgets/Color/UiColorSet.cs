using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SeganX.Widgets
{
    public class UiColorSet : MonoBehaviour
    {
        [SerializeField] private Graphic target;
        [SerializeField] private List<Color> list;

        private void Reset()
        {
            if (target == null)
                target = GetComponent<Graphic>();
        }

        protected bool IsReferenceAvailable()
        {
#if UNITY_EDITOR
            if (target == null)
                Debug.LogWarning("Target in empty!");
#endif
            return target != null;
        }

        public void Set(int index)
        {
            if (IsReferenceAvailable())
                target.color = list[index];
        }
    }
}