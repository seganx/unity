using System.Collections.Generic;
using UnityEngine;

namespace SeganX
{
    public static class MeshUtilities
    {
        public class EarClippingPolygonTriangulator
        {
            private struct PolyPoint
            {
                public int NextP;
                public int PrevP;
                public int NextEar;
                public int PrevEar;
                public int NextRefL;
                public int PrevRefL;
                public bool isEar;
            }

            private int pointsCount = 0;
            private Vector3[] points = null;
            private PolyPoint[] polyPoints = null;
            private List<Vector3Int> trianglePoints = new List<Vector3Int>();

            public int[] Triangles { get; private set; } = null;

            public EarClippingPolygonTriangulator(Vector3[] rawPoints)
            {
                points = rawPoints;
                pointsCount = rawPoints.Length;
                polyPoints = new PolyPoint[pointsCount + 1];
                trianglePoints = new List<Vector3Int>();
                try
                {
                    FillPolyPoints();
                    Triangulate();
                    CreateResult();
                }
                catch { }
            }

            /* three doubly linked lists (points list,reflective points list, ears list) are
             * maintained in the "PolyPointList" array points list is a cyclic list while other 
             * two aren't 0 index of the Point list is kept only for entering the lists -1 means undefined link */
            private void FillPolyPoints()
            {
                PolyPoint p = new PolyPoint() { NextP = 1, PrevP = -1, NextEar = -1, PrevEar = -1, NextRefL = -1, PrevRefL = -1, isEar = false };
                polyPoints[0] = p;

                int reflective = -1;
                int convex = -1;
                for (int i = 1; i <= pointsCount; i++)
                {
                    polyPoints[i] = p;
                    polyPoints[i].PrevP = i == 1 ? pointsCount : i - 1;
                    polyPoints[i].NextP = i % pointsCount + 1;

                    if (IsReflective(i))
                    {
                        polyPoints[i].PrevRefL = reflective;
                        polyPoints[reflective == -1 ? 0 : reflective].NextRefL = i;
                        reflective = i;
                        polyPoints[i].NextRefL = -1;
                        polyPoints[i].PrevEar = -1;
                        polyPoints[i].NextEar = -1;
                    }
                    else
                    {
                        polyPoints[i].PrevRefL = -1;
                        polyPoints[i].NextRefL = -1;
                        polyPoints[i].isEar = true;
                        polyPoints[i].PrevEar = convex;
                        polyPoints[convex == -1 ? 0 : convex].NextEar = i;
                        convex = i;
                        polyPoints[i].NextEar = -1;
                    }
                }

                int con = polyPoints[0].NextEar;
                while (con != -1)
                {
                    if (!IsCleanEar(con))
                        RemoveEar(con);
                    con = polyPoints[con].NextEar;
                }
            }

            // "Ear Clipping" is used for Polygon triangulation
            private void Triangulate()
            {
                while (pointsCount > 3)
                {
                    /* The Two-Ears Theorem: "Except for triangles every simple polygon 
                     * has at least two non-overlapping ears so there i will always have a value */
                    int i = polyPoints[0].NextEar;
                    int prevP = polyPoints[i].PrevP;
                    int nextP = polyPoints[i].NextP;

                    trianglePoints.Add(new Vector3Int(prevP, i, nextP));

                    RemoveEar(i);
                    RemoveP(i);

                    if (!IsReflective(prevP))
                    {
                        if (IsCleanEar(prevP))
                        {
                            if (!polyPoints[prevP].isEar)
                                AddEar(prevP);
                        }
                        else
                        {
                            if (polyPoints[prevP].isEar)
                                RemoveEar(prevP);
                        }
                    }

                    if (!IsReflective(nextP))
                    {
                        if (IsCleanEar(nextP))
                        {
                            if (!polyPoints[nextP].isEar)
                                AddEar(nextP);
                        }
                        else
                        {
                            if (polyPoints[nextP].isEar)
                                RemoveEar(nextP);
                        }
                    }
                }

                int y = polyPoints[0].NextP;
                int x = polyPoints[y].PrevP;
                int z = polyPoints[y].NextP;
                trianglePoints.Add(new Vector3Int(x, y, z));
            }

            private void CreateResult()
            {
                Triangles = new int[trianglePoints.Count * 3];
                for (int i = 0, j = 0; i < trianglePoints.Count; i++, j += 3)
                {
                    Triangles[j] = (int)(trianglePoints[i].x - 1);
                    Triangles[j + 1] = (int)(trianglePoints[i].y - 1);
                    Triangles[j + 2] = (int)(trianglePoints[i].z - 1);
                }
            }

            // vector cross product is used to determine the reflectiveness of vertices because "Sin" values of angles are always - if the angle > 180
            private bool IsReflective(int i)
            {
                Vector2 v0 = points[polyPoints[i].PrevP - 1] - points[i - 1];
                Vector2 v1 = points[polyPoints[i].NextP - 1] - points[i - 1];
                Vector3 a = Vector3.Cross(v0, v1);
                return a.z < 0;
            }

            // Barycentric Technique is used to test if the reflective vertices are in selected ears
            private bool IsCleanEar(int ear)
            {
                Vector2 v0 = points[polyPoints[ear].PrevP - 1] - points[ear - 1];
                Vector2 v1 = points[polyPoints[ear].NextP - 1] - points[ear - 1];

                int i = polyPoints[0].NextRefL;
                while (i != -1)
                {
                    Vector2 v2 = points[i - 1] - points[ear - 1];

                    float dot00 = Vector2.Dot(v0, v0);
                    float dot01 = Vector2.Dot(v0, v1);
                    float dot02 = Vector2.Dot(v0, v2);
                    float dot11 = Vector2.Dot(v1, v1);
                    float dot12 = Vector2.Dot(v1, v2);

                    float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
                    float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
                    float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

                    if (u > 0 && v > 0 && u + v < 1)
                        return false;

                    i = polyPoints[i].NextRefL;
                }

                return true;
            }

            private void RemoveEar(int ear)
            {
                int prevEar = polyPoints[ear].PrevEar;
                int nextEar = polyPoints[ear].NextEar;
                polyPoints[ear].isEar = false;
                polyPoints[prevEar == -1 ? 0 : prevEar].NextEar = nextEar;
                if (nextEar != -1) polyPoints[nextEar].PrevEar = prevEar;
            }

            private void AddEar(int ear)
            {
                int nextEar = polyPoints[0].NextEar;
                polyPoints[0].NextEar = ear;
                polyPoints[ear].PrevEar = -1;
                polyPoints[ear].NextEar = nextEar;
                polyPoints[ear].isEar = true;
                if (nextEar != -1) polyPoints[nextEar].PrevEar = ear;
            }

            private void RemoveP(int p)
            {
                int nextP = polyPoints[p].NextP;
                int prevP = polyPoints[p].PrevP;
                polyPoints[prevP].NextP = nextP;
                polyPoints[nextP].PrevP = prevP;
                if (polyPoints[0].NextP == p) polyPoints[0].NextP = nextP;
                --pointsCount;
            }

        }
    }
}
