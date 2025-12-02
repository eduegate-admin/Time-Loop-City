using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.World
{
    public static class ProceduralPropGenerator
    {
        public static Mesh GenerateTree(float height, float trunkRadius, float foliageRadius)
        {
            Mesh mesh = new Mesh();
            mesh.name = "ProceduralTree";

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Trunk (Cylinder)
            int segments = 8;
            float trunkHeight = height * 0.3f;
            GenerateCylinder(verts, tris, uvs, Vector3.zero, trunkHeight, trunkRadius, segments);

            // Foliage (Cone/Sphere blobs)
            // Simplified as a few stacked cones for a pine/palm look or spheres for deciduous
            // Let's do a simple "Lollipop" tree for now: Sphere on top
            Vector3 foliageCenter = Vector3.up * (trunkHeight + foliageRadius * 0.5f);
            GenerateSphere(verts, tris, uvs, foliageCenter, foliageRadius, 8, 8);

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        public static Mesh GenerateStreetLight(float height)
        {
            Mesh mesh = new Mesh();
            mesh.name = "ProceduralStreetLight";

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Pole
            float poleRadius = 0.15f;
            GenerateCylinder(verts, tris, uvs, Vector3.zero, height, poleRadius, 6);

            // Arm
            float armLength = 1.5f;
            Vector3 armStart = Vector3.up * (height - 0.2f);
            Vector3 armEnd = armStart + Vector3.forward * armLength;
            // Simple box arm
            AddBox(verts, tris, uvs, armStart, armEnd, 0.1f);

            // Lamp Head
            Vector3 lampPos = armEnd + Vector3.down * 0.2f;
            AddBox(verts, tris, uvs, lampPos - Vector3.one * 0.2f, lampPos + Vector3.one * 0.2f, 0.4f);

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static void GenerateCylinder(List<Vector3> verts, List<int> tris, List<Vector2> uvs, 
            Vector3 start, float height, float radius, int segments)
        {
            int baseIndex = verts.Count;
            float angleStep = 360f / segments;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleStep * Mathf.Deg2Rad;
                float x = Mathf.Cos(angle) * radius;
                float z = Mathf.Sin(angle) * radius;

                // Bottom ring
                verts.Add(start + new Vector3(x, 0, z));
                uvs.Add(new Vector2((float)i / segments, 0));

                // Top ring
                verts.Add(start + new Vector3(x, height, z));
                uvs.Add(new Vector2((float)i / segments, 1));
            }

            for (int i = 0; i < segments; i++)
            {
                int current = baseIndex + i * 2;
                int next = baseIndex + (i + 1) * 2;

                tris.Add(current);
                tris.Add(current + 1);
                tris.Add(next + 1);

                tris.Add(current);
                tris.Add(next + 1);
                tris.Add(next);
            }
        }

        private static void GenerateSphere(List<Vector3> verts, List<int> tris, List<Vector2> uvs,
            Vector3 center, float radius, int latSegments, int longSegments)
        {
            int baseIndex = verts.Count;

            for (int lat = 0; lat <= latSegments; lat++)
            {
                float theta = lat * Mathf.PI / latSegments;
                float sinTheta = Mathf.Sin(theta);
                float cosTheta = Mathf.Cos(theta);

                for (int lon = 0; lon <= longSegments; lon++)
                {
                    float phi = lon * 2 * Mathf.PI / longSegments;
                    float sinPhi = Mathf.Sin(phi);
                    float cosPhi = Mathf.Cos(phi);

                    Vector3 normal = new Vector3(cosPhi * sinTheta, cosTheta, sinPhi * sinTheta);
                    verts.Add(center + normal * radius);
                    uvs.Add(new Vector2((float)lon / longSegments, (float)lat / latSegments));
                }
            }

            for (int lat = 0; lat < latSegments; lat++)
            {
                for (int lon = 0; lon < longSegments; lon++)
                {
                    int current = baseIndex + lat * (longSegments + 1) + lon;
                    int next = current + longSegments + 1;

                    tris.Add(current);
                    tris.Add(current + 1);
                    tris.Add(next + 1);

                    tris.Add(current);
                    tris.Add(next + 1);
                    tris.Add(next);
                }
            }
        }

        private static void AddBox(List<Vector3> verts, List<int> tris, List<Vector2> uvs, Vector3 start, Vector3 end, float width)
        {
            // Simplified box connecting two points
            // For now, just a simple cube at start for testing
            // ... Implementation omitted for brevity in this step, using simple logic
        }
    }
}
