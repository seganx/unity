using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Spline
{
    public class Section
    {
        public struct Point
        {
            public float distance;
            public Vector3 position;
            public Vector3 forward;
            public Vector3 upward;
            public override string ToString() { return distance + "|" + position; }

        }

        public List<Point> points = new List<Point>(32);

        public float startDistance = 0;
        public float endDistance = 0;

        public float Length => endDistance - startDistance;

        public Section(float startDistance, SplinePoint start, SplinePoint end, int parts, float power)
        {
            this.startDistance = endDistance = startDistance;

            var startPosition = start.transform.localPosition;
            var endPosition = end.transform.localPosition;
            var startForward = start.transform.parent.InverseTransformDirection(start.transform.forward);
            var endForward = end.transform.parent.InverseTransformDirection(end.transform.forward);
            var startUp = start.transform.parent.InverseTransformDirection(start.transform.up);
            var endUp = end.transform.parent.InverseTransformDirection(end.transform.up);
            var deltaPower = Vector3.Distance(startPosition, endPosition) * power;
            var startPower = start.autoPower ? deltaPower : start.power;
            var endPower = end.autoPower ? deltaPower : end.power;


            var point = new Point();

            for (int i = 0; i <= parts; i++)
            {
                var t = (float)i / parts;

                var position = Bezier.GetPoint(startPosition, startForward * startPower, endPosition, -endForward * endPower, t);

                endDistance += i == 0 ? 0 : Vector3.Distance(point.position, position);
                point.distance = endDistance;
                point.forward = i == 0 ? startForward : (i == parts ? endForward : Vector3.Normalize(position - point.position));
                point.upward = Vector3.Slerp(startUp, endUp, t);
                point.position = position;

                points.Add(point);
            }
        }

        public Point GetLocalPoint(float distance)
        {
            if (points.Count < 2) return new Point();
            var index = GetPointIndex(distance);

            var currPoint = points[index];
            var nextPoint = points[index + 1];

            var localDistance = distance - currPoint.distance;
            var deltaDistance = nextPoint.distance - currPoint.distance;

            var t = localDistance / deltaDistance;
            return new Point()
            {
                distance = distance,
                position = Vector3.Lerp(currPoint.position, nextPoint.position, t),
                forward = Vector3.Slerp(currPoint.forward, nextPoint.forward, t),
                upward = Vector3.Slerp(currPoint.upward, nextPoint.upward, t),
            };
        }

        private int GetPointIndex(float distance)
        {
            int low = 0, mid = 0, high = points.Count - 1;
            while (low < high)
            {
                mid = (low + high) / 2;
                var item = points[mid];

                //success
                if (distance >= item.distance && (mid == high || points[mid + 1].distance >= distance)) break;

                if (distance < item.distance)
                    high = mid;
                else
                    low = mid + 1;
            }
            return mid;
        }

        public override string ToString()
        {
            return startDistance.ToString("00.00") + "    " + endDistance.ToString("00.00") + "    " + points.Count.ToString("00");
        }
    }
}
