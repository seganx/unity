using System.Collections.Generic;
using UnityEngine;

namespace SeganX.Spline
{
    [ExecuteAlways]
    public class Spline : MonoBehaviour
    {
        [SerializeField] private bool auto = true;
        [SerializeField] private int parts = 10;
        [SerializeField] private float power = 0.5f;

        private readonly List<Section> sections = new List<Section>(32);

        public float StartDistance { get; set; } = 0;
        public float Length { get; private set; } = 0;

        private void Start()
        {
            Create();
        }

        public void Create()
        {
            Length = 0;
            sections.Clear();

            // collect curve points
            temp.Clear();
            transform.GetComponentsInChildren(temp);
            int count = temp.Count;
            if (count < 2) return;

            // tranverse through curve points
            for (int i = 0; i < count - 1; i++)
            {
                var start = temp[i];
                var end = temp[i + 1];
                int sectionParts;
                float sectionPower;

                if (auto)
                {
                    var distance = Vector3.Distance(start.transform.position, end.transform.position);
                    var dotvalue = Vector3.Dot(start.transform.forward, -end.transform.forward) * 0.5f + 0.5f;

                    var distanceFactor = (distance + 30) / (distance + 0.1f) * 0.3f;
                    sectionParts = Mathf.RoundToInt(distanceFactor * distance * (dotvalue * 0.75f + 1));
                    sectionPower = 0.25f + dotvalue * 0.5f;
                }
                else
                {
                    sectionParts = parts;
                    sectionPower = power;
                }

                var section = new Section(Length, temp[i], temp[i + 1], sectionParts - 1, sectionPower);
                Length += section.Length;
                sections.Add(section);
            }
        }

        public Section.Point GetLocalPoint(float distance)
        {
            if (sections.Count < 1) Create();
            if (sections.Count < 1) return new Section.Point();
            return GetSection(distance).GetLocalPoint(distance);
        }

        public Section.Point GetPoint(float distance)
        {
            var res = GetLocalPoint(distance - StartDistance);
            res.distance = distance;
            res.position = transform.TransformPoint(res.position);
            res.forward = transform.TransformDirection(res.forward);
            res.upward = transform.TransformDirection(res.upward);
            return res;
        }

        private Section GetSection(float distance)
        {
            int low = 0, mid = 0, high = sections.Count;
            while (low < high)
            {
                mid = (low + high) / 2;
                var section = sections[mid];

                //  success
                if (section.startDistance <= distance && distance <= section.endDistance) break;

                if (distance < section.startDistance)
                    high = mid;
                else
                    low = mid + 1;
            }
            return sections[mid];
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            foreach (var section in sections)
            {
                var count = section.points.Count;
                var high = count - 1;

                for (int i = 0; i < count; i++)
                {
                    var p = transform.TransformPoint(section.points[i].position);
                    var f = transform.TransformDirection(section.points[i].forward) * 0.25f;
                    var u = transform.TransformDirection(section.points[i].upward) * 0.25f;

                    Gizmos.color = Color.yellow;
                    if (i < high)
                        Gizmos.DrawLine(p, transform.TransformPoint(section.points[i + 1].position));

                    Gizmos.color = Color.blue;
                    Gizmos.DrawRay(p, f);
                    Gizmos.color = Color.green;
                    Gizmos.DrawRay(p, u);
                }
            }
        }
#endif


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        private static readonly List<SplinePoint> temp = new List<SplinePoint>(16);
    }
}
