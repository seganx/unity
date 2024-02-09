using UnityEngine;

namespace SeganX
{
    public static class Bezier
    {
        public static Vector3 GetPoint(Transform start, Transform end, float power, float t)
        {
            var startPoint = start.position;
            var endPoint = end.position;
            var delta = Vector3.Distance(startPoint, endPoint) * power;
            return GetPoint(startPoint, start.forward * delta, endPoint, -end.forward * delta, t);
        }

        public static Vector3 GetPoint(Transform startTransform, float startPower, Transform endTransform, float endPower, float t)
        {
            return GetPoint(startTransform.position, startTransform.forward * startPower, endTransform.position, -endTransform.forward * endPower, t);
        }

        public static Vector3 GetPoint(Vector3 startPoint, Vector3 startDirection, Vector3 endPoint, Vector3 endDirection, float t)
        {
            return GetPoint(t, startPoint, startPoint + startDirection, endPoint + endDirection, endPoint);
        }

        public static Vector3 GetPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;
            Vector3 p = uuu * p0;
            p += 3 * uu * t * p1;
            p += 3 * u * tt * p2;
            p += ttt * p3;
            return p;
        }
    }

}
