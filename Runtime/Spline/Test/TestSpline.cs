using UnityEngine;

namespace SeganX.Spline
{
    [ExecuteAlways]
    public class TestSpline : MonoBehaviour
    {
        [SerializeField] private Spline spline = null;
        [SerializeField] private float distance = 0;

        // Update is called once per frame
        void Update()
        {
            if (spline == null) return;
            var point = spline.GetPoint(distance);
            transform.SetPositionAndRotation(point.position, Quaternion.LookRotation(point.forward, point.upward));
        }
    }
}
