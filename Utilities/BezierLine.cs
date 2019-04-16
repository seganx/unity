using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    [ExecuteInEditMode, RequireComponent(typeof(LineRenderer))]
    public class BezierLine : MonoBehaviour
    {
        public LineRenderer liner = null;

        [SerializeField]
        private Vector3 startPoint;
        [SerializeField]
        private Vector3 startDirection;
        [SerializeField]
        private Vector3 endPoint;
        [SerializeField]
        private Vector3 endDirection;

        public Vector3 StartPoint { get { return startPoint; } set { startPoint = value; Remake(); } }
        public Vector3 StartDirection { get { return startDirection; } set { startDirection = value; Remake(); } }
        public Vector3 EndPoint { get { return endPoint; } set { endPoint = value; Remake(); } }
        public Vector3 EndDirection { get { return endDirection; } set { endDirection = value; Remake(); } }

        void Awake()
        {
            if (liner == null)
                liner = GetComponent<LineRenderer>();
        }

        public void Remake()
        {
            BezierLineRenderer(liner, startPoint, startDirection, endPoint, endDirection);
        }

#if UNITY_EDITOR
        void Update()
        {
            Remake();
        }
#endif

        public static void BezierLineRenderer(LineRenderer liner, Vector3 startPoint, Vector3 startDirection, Vector3 endPoint, Vector3 endDirection)
        {
            float n = liner.positionCount - 1;
            for (int i = 0; i <= n; i++)
            {
                float t = i / n;
                liner.SetPosition(i, CalculateBezierPoint(t, startPoint, startPoint + startDirection, endPoint + endDirection, endPoint));
            }
        }

        public static Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
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
