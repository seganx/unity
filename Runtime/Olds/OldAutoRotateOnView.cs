using UnityEngine;

namespace Fun.Mono
{
    [System.Obsolete()]
    [RequireComponent(typeof(Renderer))]
    public class OldAutoRotateOnView : MonoBehaviour
    {
        [SerializeField] private Space space = Space.Self;
        [SerializeField] private Vector3 rotation = Vector3.zero;

        private void OnWillRenderObject()
        {
            transform.Rotate(rotation.x * Time.deltaTime, rotation.y * Time.deltaTime, rotation.z * Time.deltaTime, space);
        }
    }
}
