using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Tilemaps;

namespace SeganX
{
    public static class MeshEx
    {
        private static List<Vector3> points = new List<Vector3>();
        private static List<Vector3> normals = new List<Vector3>();
        private static List<Vector2> uvs = new List<Vector2>();
        private static List<int> triangles = new List<int>();

        public static MeshFilter Create2D(this MeshFilter self, Vector2[] points)
        {
            var tmp = new Vector3[points.Length];
            for (int i = 0; i < points.Length; i++)
                tmp[i] = points[i];
            return self.Create2D(tmp);
        }

        public static MeshFilter Create2D(this MeshFilter self, Vector3[] points)
        {
            var triangles = new MeshUtilities.EarClippingPolygonTriangulator(points).Triangles;
            if (triangles == null || triangles.Length < 3) return self;

            var mesh = new Mesh();
            int vertexCount = points.Length;

            Vector3[] vertices = new Vector3[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                vertices[i] = points[i];

            Vector3[] normals = new Vector3[vertexCount];
            for (int i = 0; i < vertexCount; i++)
                normals[i] = Vector3.back;

            var uvs = new Vector2[vertexCount];
            for (int i = 0; i < uvs.Length; i++)
                uvs[i] = vertices[i];

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.normals = normals;
            mesh.uv = uvs;

            self.mesh = mesh;
            return self;
        }

        public static MeshFilter CreateRoad(this MeshFilter self, Spline.Spline spline, int resolution, float width, string meshName = null)
        {
            if (self.sharedMesh == null)
                self.sharedMesh = new Mesh() { name = string.IsNullOrEmpty(meshName) ? $"Road_{self.name}" : meshName };

            points.Clear();
            normals.Clear();
            uvs.Clear();
            triangles.Clear();

            var radius = width * 0.5f;
            var yCount = resolution;
            var yLenght = spline.Length / yCount;
            var xCount = Mathf.CeilToInt(width / yLenght);
            var xLength = width / xCount;

            var uvY = (spline.Length / width) / yCount;
            var uvX = 1.0f / xCount;

            for (int i = 0; i <= yCount; i++)
            {
                var point = spline.GetLocalPoint(i * yLenght);
                var cross = Vector3.Cross(point.forward, point.upward).normalized;
                var right = point.position - cross * radius;
                points.Add(right);
                normals.Add(point.upward);

                var t = i * uvY;
                uvs.Add(new Vector2(1, t));

                var tileL = (cross * xLength);
                for (int c = 1; c <= xCount; c++)
                {
                    points.Add(right + tileL * c);
                    normals.Add(point.upward);
                    uvs.Add(new Vector2(1 - uvX * c, t));
                }


            }


            for (int y = 0; y < yCount; y++)
            {
                var a = y * (xCount + 1);
                for (int x = 0; x < xCount; x++)
                {
                    int i = x + a;
                    triangles.Add(i);
                    triangles.Add(i + 1);
                    triangles.Add(i + xCount + 1);

                    triangles.Add(i + 1);
                    triangles.Add(i + xCount + 2);
                    triangles.Add(i + xCount + 1);
                }
            }

            self.sharedMesh.Clear();
            if (points.Count > 3)
            {
                self.sharedMesh.vertices = points.ToArray();
                self.sharedMesh.normals = normals.ToArray();
                self.sharedMesh.uv = uvs.ToArray();
                self.sharedMesh.triangles = triangles.ToArray();
                self.sharedMesh.RecalculateNormals();
            }

            return self;
        }
    }
}
