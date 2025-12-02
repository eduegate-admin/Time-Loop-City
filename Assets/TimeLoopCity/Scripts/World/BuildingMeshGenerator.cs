using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.World
{
    public static class BuildingMeshGenerator
    {
        public static Mesh GenerateBuildingMesh(float width, float depth, float height, int floors)
        {
            Mesh mesh = new Mesh();
            mesh.name = "ProceduralBuilding";

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            // Simple Box Extrusion for now, but prepared for more complex shapes
            // Pivot at bottom center
            
            float halfWidth = width / 2f;
            float halfDepth = depth / 2f;

            // Bottom vertices (y=0)
            Vector3 v0 = new Vector3(-halfWidth, 0, -halfDepth);
            Vector3 v1 = new Vector3(halfWidth, 0, -halfDepth);
            Vector3 v2 = new Vector3(halfWidth, 0, halfDepth);
            Vector3 v3 = new Vector3(-halfWidth, 0, halfDepth);

            // Top vertices (y=height)
            Vector3 v4 = new Vector3(-halfWidth, height, -halfDepth);
            Vector3 v5 = new Vector3(halfWidth, height, -halfDepth);
            Vector3 v6 = new Vector3(halfWidth, height, halfDepth);
            Vector3 v7 = new Vector3(-halfWidth, height, halfDepth);

            // Add faces
            AddQuad(vertices, triangles, uvs, v0, v4, v5, v1); // Front
            AddQuad(vertices, triangles, uvs, v1, v5, v6, v2); // Right
            AddQuad(vertices, triangles, uvs, v2, v6, v7, v3); // Back
            AddQuad(vertices, triangles, uvs, v3, v7, v4, v0); // Left
            AddQuad(vertices, triangles, uvs, v4, v7, v6, v5); // Top
            // No bottom face needed usually

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static void AddQuad(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, 
            Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br)
        {
            int index = vertices.Count;

            vertices.Add(bl);
            vertices.Add(tl);
            vertices.Add(tr);
            vertices.Add(br);

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));

            triangles.Add(index);
            triangles.Add(index + 1);
            triangles.Add(index + 2);
            triangles.Add(index);
            triangles.Add(index + 2);
            triangles.Add(index + 3);
        }
    }
}
