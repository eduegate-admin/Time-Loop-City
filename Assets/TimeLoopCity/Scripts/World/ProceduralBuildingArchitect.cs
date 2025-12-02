using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.World
{
    public static class ProceduralBuildingArchitect
    {
        public enum BuildingStyle { Modern, Colonial, Slum }

        public static Mesh GenerateBuilding(float width, float depth, float height, int floors, BuildingStyle style)
        {
            Mesh mesh = new Mesh();
            mesh.name = $"Building_{style}";

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float floorHeight = height / floors;

            // Generate base structure
            // We'll create a "stack" of floors
            for (int f = 0; f < floors; f++)
            {
                float currentY = f * floorHeight;
                GenerateFloor(vertices, triangles, uvs, width, depth, floorHeight, currentY, style, f == 0, f == floors - 1);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();
            
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static void GenerateFloor(List<Vector3> verts, List<int> tris, List<Vector2> uvs, 
            float width, float depth, float height, float yPos, BuildingStyle style, bool isGround, bool isRoof)
        {
            float hw = width / 2f;
            float hd = depth / 2f;

            // 4 Corners
            Vector3 bl = new Vector3(-hw, yPos, -hd);
            Vector3 br = new Vector3(hw, yPos, -hd);
            Vector3 tr = new Vector3(hw, yPos, hd);
            Vector3 tl = new Vector3(-hw, yPos, hd);

            Vector3 bl_t = new Vector3(-hw, yPos + height, -hd);
            Vector3 br_t = new Vector3(hw, yPos + height, -hd);
            Vector3 tr_t = new Vector3(hw, yPos + height, hd);
            Vector3 tl_t = new Vector3(-hw, yPos + height, hd);

            // Inset for windows/details if not ground floor
            float inset = (style == BuildingStyle.Modern && !isGround) ? 0.1f : 0f;
            
            // Front Face
            AddWallFace(verts, tris, uvs, bl, bl_t, br_t, br, inset, style);
            // Right Face
            AddWallFace(verts, tris, uvs, br, br_t, tr_t, tr, inset, style);
            // Back Face
            AddWallFace(verts, tris, uvs, tr, tr_t, tl_t, tl, inset, style);
            // Left Face
            AddWallFace(verts, tris, uvs, tl, tl_t, bl_t, bl, inset, style);

            // Roof (only on top floor)
            if (isRoof)
            {
                AddQuad(verts, tris, uvs, bl_t, tl_t, tr_t, br_t);
                
                // Add parapet or roof details
                if (style == BuildingStyle.Colonial)
                {
                    // Simple pitched roof logic could go here, for now just a parapet
                    // ...
                }
            }
            
            // Floor/Ceiling (optional, usually hidden)
        }

        private static void AddWallFace(List<Vector3> verts, List<int> tris, List<Vector2> uvs, 
            Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br, float inset, BuildingStyle style)
        {
            // If inset > 0, we extrude windows inwards
            if (inset > 0)
            {
                // Divide wall into 3 vertical sections: Sill, Window, Header
                float h = Vector3.Distance(bl, tl);
                float w = Vector3.Distance(bl, br);
                
                Vector3 up = (tl - bl).normalized;
                Vector3 right = (br - bl).normalized;
                Vector3 normal = Vector3.Cross(up, right).normalized;

                float sillH = h * 0.3f;
                float winH = h * 0.5f;
                // float headH = h * 0.2f;

                // Sill
                AddQuad(verts, tris, uvs, bl, bl + up * sillH, br + up * sillH, br);

                // Window (Inset)
                Vector3 winBL = bl + up * sillH + right * (w * 0.1f);
                Vector3 winBR = br + up * sillH - right * (w * 0.1f);
                Vector3 winTL = bl + up * (sillH + winH) + right * (w * 0.1f);
                Vector3 winTR = br + up * (sillH + winH) - right * (w * 0.1f);
                
                Vector3 winBL_in = winBL - normal * inset;
                Vector3 winBR_in = winBR - normal * inset;
                Vector3 winTL_in = winTL - normal * inset;
                Vector3 winTR_in = winTR - normal * inset;

                // Window Frame Sides
                AddQuad(verts, tris, uvs, winBL, winTL, winTL_in, winBL_in); // Left
                AddQuad(verts, tris, uvs, winBR_in, winTR_in, winTR, winBR); // Right
                AddQuad(verts, tris, uvs, winTL, winTR, winTR_in, winTL_in); // Top
                AddQuad(verts, tris, uvs, winBL_in, winBR_in, winBR, winBL); // Bottom
                
                // Window Glass
                AddQuad(verts, tris, uvs, winBL_in, winTL_in, winTR_in, winBR_in);

                // Wall sides around window
                AddQuad(verts, tris, uvs, bl + up * sillH, winBL, winTL, bl + up * (sillH + winH)); // Left Col
                AddQuad(verts, tris, uvs, winBR, br + up * sillH, br + up * (sillH + winH), winTR); // Right Col

                // Header
                AddQuad(verts, tris, uvs, bl + up * (sillH + winH), tl, tr, br + up * (sillH + winH));
            }
            else
            {
                AddQuad(verts, tris, uvs, bl, tl, tr, br);
            }
        }

        private static void AddQuad(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, 
            Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br)
        {
            int index = vertices.Count;

            vertices.Add(bl);
            vertices.Add(tl);
            vertices.Add(tr);
            vertices.Add(br);

            // Simple UVs 0-1
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
