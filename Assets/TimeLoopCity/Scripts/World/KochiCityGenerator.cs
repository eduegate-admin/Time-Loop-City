using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.World
{
    public class KochiCityGenerator : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Vector2 citySize = new Vector2(500, 500);
        [SerializeField] private int seed = 1337;

        [Header("Materials")]
        [SerializeField] private Material waterMat;
        [SerializeField] private Material groundMat;
        [SerializeField] private Material roadMat;
        [SerializeField] private Material bridgeMat;

        private RoadNetworkGenerator roadGenerator;

        public void Generate()
        {
            Random.InitState(seed);
            Cleanup();
            LoadMaterials();

            BuildBackwaters();
            BuildIslands();
            BuildBuildings();
            BuildProps();

            if (roadGenerator == null) roadGenerator = gameObject.AddComponent<RoadNetworkGenerator>();
            roadGenerator.Initialize(roadMat, bridgeMat);
            roadGenerator.GenerateNetwork();
        }

        private void LoadMaterials()
        {
#if UNITY_EDITOR
            if (waterMat == null) waterMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Kochi/Water_Mat.mat");
            if (groundMat == null) groundMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Kochi/Ground_Mat.mat");
            if (roadMat == null) roadMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Kochi/Road_Mat.mat");
            // Use Building_Mat for generic buildings if not specified
#endif
        }

        private void Cleanup()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        private void BuildBackwaters()
        {
            GameObject water = GameObject.CreatePrimitive(PrimitiveType.Plane);
            water.name = "Backwaters";
            water.transform.parent = transform;
            water.transform.localScale = new Vector3(citySize.x / 10f, 1, citySize.y / 10f);
            water.transform.position = Vector3.zero;
            
            if (waterMat != null)
                water.GetComponent<Renderer>().material = waterMat;
            else
                TintRenderer(water.GetComponent<Renderer>(), new Color(0.1f, 0.3f, 0.4f, 0.8f));
        }

        private void BuildIslands()
        {
            // Marine Drive (Central)
            CreateIsland("District_MarineDrive", new Vector3(0, 0.5f, 0), new Vector3(80, 1, 80));
            BuildMarineDriveLandmarks();

            // Fort Kochi (West)
            CreateIsland("District_FortKochi", new Vector3(-120, 0.5f, 20), new Vector3(60, 1, 100));
            BuildFortKochiLandmarks();

            // Willingdon Island (East)
            CreateIsland("District_Willingdon", new Vector3(120, 0.5f, -20), new Vector3(70, 1, 90));
            BuildWillingdonLandmarks();

            // Mangalavanam (North)
            CreateIsland("District_Mangalavanam", new Vector3(20, 0.5f, 100), new Vector3(50, 1, 50));
            
            // Bolgatty Island (New)
            CreateIsland("District_Bolgatty", new Vector3(50, 0.5f, 50), new Vector3(40, 1, 60));
            BuildBolgattyLandmarks();
            
            // Edappally (Lulu Mall area - North East)
            CreateIsland("District_Edappally", new Vector3(80, 0.5f, 80), new Vector3(60, 1, 60));
            BuildLuluMall();
        }

        private void BuildMarineDriveLandmarks()
        {
            // High Court & Clock Tower
            GameObject highCourt = GameObject.CreatePrimitive(PrimitiveType.Cube);
            highCourt.name = "Landmark_HighCourt";
            highCourt.transform.parent = transform;
            highCourt.transform.position = new Vector3(0, 5, 25);
            highCourt.transform.localScale = new Vector3(12, 10, 8);
            TintRenderer(highCourt.GetComponent<Renderer>(), new Color(0.8f, 0.3f, 0.3f)); // Red Brick

            // Rainbow Bridge (Simplified)
            GameObject bridge = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            bridge.name = "Landmark_RainbowBridge";
            bridge.transform.parent = transform;
            bridge.transform.position = new Vector3(0, 0.5f, -10);
            bridge.transform.localScale = new Vector3(6, 0.5f, 6);
            TintRenderer(bridge.GetComponent<Renderer>(), Color.white);
        }

        private void BuildFortKochiLandmarks()
        {
            // Chinese Fishing Nets
            for (int i = 0; i < 3; i++)
            {
                GameObject net = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                net.name = $"Landmark_FishingNet_{i}";
                net.transform.parent = transform;
                net.transform.position = new Vector3(-140, 2, 0 + (i * 15));
                net.transform.rotation = Quaternion.Euler(45, -90, 0);
                net.transform.localScale = new Vector3(1, 8, 1);
                TintRenderer(net.GetComponent<Renderer>(), new Color(0.4f, 0.2f, 0.1f)); // Wood
            }

            // Kashi Art Cafe
            GameObject cafe = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cafe.name = "Landmark_KashiArtCafe";
            cafe.transform.parent = transform;
            cafe.transform.position = new Vector3(-120, 3, 20);
            cafe.transform.localScale = new Vector3(10, 6, 10);
            TintRenderer(cafe.GetComponent<Renderer>(), Color.white);
        }

        private void BuildWillingdonLandmarks()
        {
            // Port Cranes
            GameObject crane = GameObject.CreatePrimitive(PrimitiveType.Cube);
            crane.name = "Landmark_PortCrane";
            crane.transform.parent = transform;
            crane.transform.position = new Vector3(140, 10, -20);
            crane.transform.localScale = new Vector3(2, 20, 2);
            TintRenderer(crane.GetComponent<Renderer>(), Color.yellow);
            
            // Containers
            for (int i = 0; i < 5; i++)
            {
                GameObject container = GameObject.CreatePrimitive(PrimitiveType.Cube);
                container.name = $"Container_{i}";
                container.transform.parent = transform;
                container.transform.position = new Vector3(130 + i*3, 1, -30);
                container.transform.localScale = new Vector3(2, 2, 6);
                TintRenderer(container.GetComponent<Renderer>(), Color.blue);
            }
        }
        
        private void BuildBolgattyLandmarks()
        {
            // Bolgatty Palace
            GameObject palace = GameObject.CreatePrimitive(PrimitiveType.Cube);
            palace.name = "Landmark_BolgattyPalace";
            palace.transform.parent = transform;
            palace.transform.position = new Vector3(50, 4, 50);
            palace.transform.localScale = new Vector3(15, 8, 15);
            TintRenderer(palace.GetComponent<Renderer>(), new Color(0.9f, 0.9f, 0.8f)); // Cream
        }
        
        private void BuildLuluMall()
        {
            // Lulu Mall (Massive Block)
            GameObject mall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mall.name = "Landmark_LuluMall";
            mall.transform.parent = transform;
            mall.transform.position = new Vector3(80, 6, 80);
            mall.transform.localScale = new Vector3(30, 12, 30);
            TintRenderer(mall.GetComponent<Renderer>(), new Color(0.8f, 0.8f, 0.9f)); // Modern Glassy
        }

        private void BuildBuildings()
        {
            // Simple grid-based building placement for Marine Drive
            PlaceBuildingsGrid(new Vector3(0, 0, 0), new Vector2(80, 80), 15, "MarineDrive_Apt");
            
            // Fort Kochi (Lower density)
            PlaceBuildingsGrid(new Vector3(-120, 0, 20), new Vector2(60, 100), 20, "FortKochi_House");
        }

        private void PlaceBuildingsGrid(Vector3 center, Vector2 size, float spacing, string prefix)
        {
            int rows = Mathf.FloorToInt(size.x / spacing);
            int cols = Mathf.FloorToInt(size.y / spacing);
            Vector3 start = center - new Vector3(size.x / 2, 0, size.y / 2);

            for (int x = 0; x < rows; x++)
            {
                for (int z = 0; z < cols; z++)
                {
                    Vector3 pos = start + new Vector3(x * spacing + spacing/2, 0, z * spacing + spacing/2);
                    
                    // Simple check to avoid center (Landmarks)
                    if (Vector3.Distance(pos, center) < 15) continue;

                    // Get district-specific style
                    var district = DistrictManager.GetDistrictAtPosition(pos);
                    var style = DistrictManager.GetStyleForDistrict(district);
                    float height = DistrictManager.GetBuildingHeightForDistrict(district);
                    int floors = Mathf.Max(1, Mathf.FloorToInt(height / 3f)); // ~3m per floor
                    
                    // Use Procedural Mesh with District Style
                    GameObject building = new GameObject($"{prefix}_{district}_{x}_{z}");
                    building.transform.parent = transform;
                    building.transform.position = pos; // Pivot is bottom
                    
                    MeshFilter mf = building.AddComponent<MeshFilter>();
                    MeshRenderer mr = building.AddComponent<MeshRenderer>();
                    
                    float width = spacing * 0.7f;
                    float depth = spacing * 0.7f;
                    mf.mesh = ProceduralBuildingArchitect.GenerateBuilding(width, depth, height, floors, style);
                    
                    // Add Collider for NavMesh
                    BoxCollider bc = building.AddComponent<BoxCollider>();
                    bc.size = new Vector3(width, height, depth);
                    bc.center = new Vector3(0, height/2, 0);

#if UNITY_EDITOR
                    Material bMat = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/Kochi/Building_Mat.mat");
                    if (bMat != null)
                        mr.material = bMat;
                    else
                        TintRenderer(mr, GetColorForStyle(style));
#else
                    TintRenderer(mr, GetColorForStyle(style));
#endif
                    building.isStatic = true;
                }
            }
        }
        
        private Color GetColorForStyle(ProceduralBuildingArchitect.BuildingStyle style)
        {
            switch (style)
            {
                case ProceduralBuildingArchitect.BuildingStyle.Colonial:
                    return new Color(0.95f, 0.9f, 0.7f); // Cream/Yellow
                case ProceduralBuildingArchitect.BuildingStyle.Modern:
                    return new Color(0.8f, 0.85f, 0.9f); // Blue-grey glass
                case ProceduralBuildingArchitect.BuildingStyle.Slum:
                    return new Color(0.6f, 0.55f, 0.5f); // Brownish
                default:
                    return Color.gray;
            }
        }

        private void BuildProps()
        {
            // Mangalavanam (Nature) - Place Trees
            PlaceTrees(new Vector3(20, 0, 100), new Vector2(50, 50), 20);
            
            // Street Lights along Marine Drive (Simplified line)
            for (int z = -40; z < 40; z += 10)
            {
                CreateStreetLight(new Vector3(10, 0, z));
            }
        }

        private void PlaceTrees(Vector3 center, Vector2 size, int count)
        {
            for (int i = 0; i < count; i++)
            {
                Vector3 pos = center + new Vector3(
                    Random.Range(-size.x/2, size.x/2),
                    0,
                    Random.Range(-size.y/2, size.y/2)
                );
                
                GameObject tree = new GameObject($"Tree_{i}");
                tree.transform.parent = transform;
                tree.transform.position = pos;
                
                MeshFilter mf = tree.AddComponent<MeshFilter>();
                MeshRenderer mr = tree.AddComponent<MeshRenderer>();
                
                float height = Random.Range(6f, 10f);
                mf.mesh = ProceduralPropGenerator.GenerateTree(height, 0.3f, 2.5f);
                
                TintRenderer(mr, new Color(0.2f, 0.5f, 0.2f));
            }
        }

        private void CreateStreetLight(Vector3 pos)
        {
            GameObject lightObj = new GameObject("StreetLight");
            lightObj.transform.parent = transform;
            lightObj.transform.position = pos;
            
            MeshFilter mf = lightObj.AddComponent<MeshFilter>();
            MeshRenderer mr = lightObj.AddComponent<MeshRenderer>();
            
            mf.mesh = ProceduralPropGenerator.GenerateStreetLight(6f);
            
            TintRenderer(mr, new Color(0.3f, 0.3f, 0.3f));
            
            // Add actual light component
            GameObject lamp = new GameObject("Lamp");
            lamp.transform.parent = lightObj.transform;
            lamp.transform.localPosition = new Vector3(0, 6f, 1.5f);
            Light light = lamp.AddComponent<Light>();
            light.type = LightType.Point;
            light.range = 15f;
            light.intensity = 0.8f;
            light.color = new Color(1f, 0.9f, 0.7f);
        }

        private void CreateIsland(string name, Vector3 pos, Vector3 size)
        {
            GameObject island = GameObject.CreatePrimitive(PrimitiveType.Cube);
            island.name = name;
            island.transform.parent = transform;
            island.transform.position = pos;
            island.transform.localScale = size;
            
            if (groundMat != null)
                island.GetComponent<Renderer>().material = groundMat;
            else
                TintRenderer(island.GetComponent<Renderer>(), new Color(0.2f, 0.25f, 0.2f));
                
            island.isStatic = true; // For NavMesh
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
