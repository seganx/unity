using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Widgets
{
    public class MaterialSet : MonoBehaviour
    {
        [SerializeField] private Renderer target;
        public int targetMaterialElement = 0;
        public List<Material> materials;

        private void Reset()
        {
            if (target == null)
                target = GetComponent<Renderer>();
        }

        protected bool IsReferenceAvailable()
        {
#if UNITY_EDITOR
            if (target == null)
                Debug.LogWarning("Target in empty!");
            if (targetMaterialElement >= target.materials.Length)
                Debug.LogWarning("Index out of range!");
#endif
            return target != null && targetMaterialElement < target.materials.Length;
        }

        public void Set(int index)
        {
            if (IsReferenceAvailable())
            {
                Material[] targetMaterials = target.materials;
                targetMaterials[targetMaterialElement] = materials[index];
                target.materials = targetMaterials;
            }
        }
    }
}