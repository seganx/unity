using UnityEngine;

namespace Fun.Mono
{
    [System.Obsolete()]
    public class OldAutoRotateAlways : MonoBehaviour
    {
        [SerializeField] private Space space = Space.Self;
        [SerializeField] private Vector3 rotation = Vector3.zero;

        private void Update()
        {
            transform.Rotate(rotation.x * Time.deltaTime, rotation.y * Time.deltaTime, rotation.z * Time.deltaTime, space);
        }
    }
}