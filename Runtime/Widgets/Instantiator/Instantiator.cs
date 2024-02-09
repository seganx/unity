using UnityEngine;

namespace SeganX.Widgets
{
    public class Instantiator : MonoBehaviour
    {
        [SerializeField] private bool instantiateOnStart = false;
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject prefab;

        private void Start()
        {
            if (instantiateOnStart)
            {
                InstantiateObject();
            }
        }

        private void Reset()
        {
            if (parent == null && transform.parent != null)
                parent = transform.parent;
        }

        public void InstantiateObject()
        {
            Instantiate(prefab, transform.position, transform.rotation, parent);
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, .5f);
        }
#endif
    }
}