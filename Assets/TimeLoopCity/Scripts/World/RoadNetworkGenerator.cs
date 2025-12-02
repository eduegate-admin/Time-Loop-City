using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.World
{
    public class RoadNetworkGenerator : MonoBehaviour
    {
        private Material roadMat;
        private Material bridgeMat;

        public void Initialize(Material road, Material bridge)
        {
            roadMat = road;
            bridgeMat = bridge;
        }

        public void GenerateNetwork()
        {
            GameObject roadsRoot = new GameObject("RoadNetwork");
            roadsRoot.transform.parent = transform;

            // Define key nodes for Kochi layout
            Vector3 marineDrive = new Vector3(0, 0.6f, 0);
            Vector3 fortKochi = new Vector3(-120, 0.6f, 20);
            Vector3 willingdon = new Vector3(120, 0.6f, -20);
            Vector3 mangalavanam = new Vector3(20, 0.6f, 100);

            // Connect districts with bridges/roads
            CreateRoadSegment(marineDrive, fortKochi, "Bridge_Marine_Fort", roadsRoot.transform, true);
            CreateRoadSegment(marineDrive, willingdon, "Bridge_Marine_Willingdon", roadsRoot.transform, true);
            CreateRoadSegment(marineDrive, mangalavanam, "Road_Marine_Mangalavanam", roadsRoot.transform, false);
            
            // Internal District Loops
            CreateDistrictLoop(marineDrive, 30, "Loop_MarineDrive", roadsRoot.transform);
            CreateDistrictLoop(fortKochi, 20, "Loop_FortKochi", roadsRoot.transform);
            CreateDistrictLoop(willingdon, 25, "Loop_Willingdon", roadsRoot.transform);
        }

        private void CreateRoadSegment(Vector3 start, Vector3 end, string name, Transform parent, bool isBridge)
        {
            Vector3 midPoint = (start + end) / 2f;
            float distance = Vector3.Distance(start, end);
            Vector3 direction = (end - start).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);

            GameObject road = new GameObject(name);
            road.transform.parent = parent;
            road.transform.position = midPoint;
            road.transform.rotation = rotation;

            // Generate Detailed Mesh
            MeshFilter mf = road.AddComponent<MeshFilter>();
            MeshRenderer mr = road.AddComponent<MeshRenderer>();
            
            float roadWidth = 8f;
            float sidewalkWidth = 2f;
            float curbHeight = 0.2f;
            
            mf.mesh = GenerateRoadMesh(distance, roadWidth, sidewalkWidth, curbHeight);
            
            // Collider
            BoxCollider bc = road.AddComponent<BoxCollider>();
            bc.size = new Vector3(roadWidth + sidewalkWidth * 2, 0.5f, distance);
            bc.center = Vector3.zero;

            if (isBridge)
            {
                // Raise bridges
                road.transform.position += Vector3.up * 2f;
                CreateBridgePillar(midPoint, parent);
                CreateBridgePillar(Vector3.Lerp(start, end, 0.25f), parent);
                CreateBridgePillar(Vector3.Lerp(start, end, 0.75f), parent);

                if (bridgeMat != null) mr.material = bridgeMat;
                else TintRenderer(mr, Color.gray);
            }
            else
            {
                if (roadMat != null) mr.material = roadMat;
                else TintRenderer(mr, Color.black);
            }
            
            road.isStatic = true;

            // Traffic Waypoints (Keep existing logic)
            var trafficSys = Traffic.TrafficSystem.Instance;
            if (trafficSys != null)
            {
                GameObject wpObj = new GameObject($"WP_{name}_Start");
                wpObj.transform.parent = parent;
                wpObj.transform.position = start + Vector3.up * 0.5f;
                wpObj.transform.LookAt(end);
                var wp = wpObj.AddComponent<Traffic.TrafficWaypoint>();
                
                GameObject wpEndObj = new GameObject($"WP_{name}_End");
                wpEndObj.transform.parent = parent;
                wpEndObj.transform.position = end + Vector3.up * 0.5f;
                wpEndObj.transform.LookAt(end + direction);
                var wpEnd = wpEndObj.AddComponent<Traffic.TrafficWaypoint>();
                
                wp.nextWaypoint = wpEnd.transform;
                trafficSys.RegisterSpawnPoint(wp);
            }
        }

        private Mesh GenerateRoadMesh(float length, float roadWidth, float sidewalkWidth, float curbHeight)
        {
            Mesh mesh = new Mesh();
            mesh.name = "ProceduralRoad";

            List<Vector3> verts = new List<Vector3>();
            List<int> tris = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float halfLen = length / 2f;
            float halfRoad = roadWidth / 2f;
            float totalWidth = halfRoad + sidewalkWidth;

            // Profile X coordinates: -Total, -Road, Road, Total
            // Y coordinates: 0 (Road), CurbHeight (Sidewalk)

            // Left Sidewalk Top
            AddQuad(verts, tris, uvs, 
                new Vector3(-totalWidth, curbHeight, -halfLen),
                new Vector3(-totalWidth, curbHeight, halfLen),
                new Vector3(-halfRoad, curbHeight, halfLen),
                new Vector3(-halfRoad, curbHeight, -halfLen));

            // Left Curb Face
            AddQuad(verts, tris, uvs,
                new Vector3(-halfRoad, curbHeight, -halfLen),
                new Vector3(-halfRoad, curbHeight, halfLen),
                new Vector3(-halfRoad, 0, halfLen),
                new Vector3(-halfRoad, 0, -halfLen));

            // Road Surface
            AddQuad(verts, tris, uvs,
                new Vector3(-halfRoad, 0, -halfLen),
                new Vector3(-halfRoad, 0, halfLen),
                new Vector3(halfRoad, 0, halfLen),
                new Vector3(halfRoad, 0, -halfLen));

            // Right Curb Face
            AddQuad(verts, tris, uvs,
                new Vector3(halfRoad, 0, -halfLen),
                new Vector3(halfRoad, 0, halfLen),
                new Vector3(halfRoad, curbHeight, halfLen),
                new Vector3(halfRoad, curbHeight, -halfLen));

            // Right Sidewalk Top
            AddQuad(verts, tris, uvs,
                new Vector3(halfRoad, curbHeight, -halfLen),
                new Vector3(halfRoad, curbHeight, halfLen),
                new Vector3(totalWidth, curbHeight, halfLen),
                new Vector3(totalWidth, curbHeight, -halfLen));

            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.RecalculateNormals();
            
            return mesh;
        }

        private void AddQuad(List<Vector3> vertices, List<int> triangles, List<Vector2> uvs, 
            Vector3 bl, Vector3 tl, Vector3 tr, Vector3 br)
        {
            int index = vertices.Count;
            vertices.Add(bl); vertices.Add(tl); vertices.Add(tr); vertices.Add(br);
            uvs.Add(new Vector2(0, 0)); uvs.Add(new Vector2(0, 1)); uvs.Add(new Vector2(1, 1)); uvs.Add(new Vector2(1, 0));
            triangles.Add(index); triangles.Add(index + 1); triangles.Add(index + 2);
            triangles.Add(index); triangles.Add(index + 2); triangles.Add(index + 3);
        }

        private void CreateBridgePillar(Vector3 pos, Transform parent)
        {
            GameObject pillar = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pillar.name = "BridgePillar";
            pillar.transform.parent = parent;
            pillar.transform.position = new Vector3(pos.x, 0, pos.z); // Ground level
            pillar.transform.localScale = new Vector3(2, 5, 2); // Height 5
            TintRenderer(pillar.GetComponent<Renderer>(), Color.gray);
        }

        private void CreateDistrictLoop(Vector3 center, float radius, string name, Transform parent)
        {
            // Create a square loop of roads around the center
            Vector3 p1 = center + new Vector3(radius, 0, radius);
            Vector3 p2 = center + new Vector3(radius, 0, -radius);
            Vector3 p3 = center + new Vector3(-radius, 0, -radius);
            Vector3 p4 = center + new Vector3(-radius, 0, radius);

            CreateRoadSegment(p1, p2, $"{name}_East", parent, false);
            CreateRoadSegment(p2, p3, $"{name}_South", parent, false);
            CreateRoadSegment(p3, p4, $"{name}_West", parent, false);
            CreateRoadSegment(p4, p1, $"{name}_North", parent, false);
        }

        private void TintRenderer(Renderer renderer, Color color)
        {
            if (renderer == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader) { color = color };
            renderer.sharedMaterial = mat;
        }
    }
}
