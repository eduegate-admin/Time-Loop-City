using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Environment
{
    /// <summary>
    /// Procedural city generator that creates a realistic city layout
    /// with roads, sidewalks, buildings, and street furniture.
    /// </summary>
    public class ProceduralCityGenerator : MonoBehaviour
    {
        [Header("City Layout")]
        [SerializeField] private int cityWidth = 5;
        [SerializeField] private int cityDepth = 5;
        [SerializeField] private float blockSize = 40f;
        [SerializeField] private float roadWidth = 8f;
        [SerializeField] private float sidewalkWidth = 2f;

        [Header("Buildings")]
        [SerializeField] private float minBuildingHeight = 10f;
        [SerializeField] private float maxBuildingHeight = 30f;
        [SerializeField] private Vector2 buildingFootprintRange = new Vector2(8f, 15f);
        [SerializeField] private int buildingsPerBlock = 4;

        [Header("Street Furniture")]
        [SerializeField] private bool generateStreetLamps = true;
        [SerializeField] private float streetLampSpacing = 15f;
        [SerializeField] private bool generateTrafficLights = true;
        [SerializeField] private bool generateBenches = true;

        [Header("Materials - Assign in Inspector")]
        [SerializeField] private Material roadMaterial;
        [SerializeField] private Material sidewalkMaterial;
        [SerializeField] private Material buildingMaterial;
        [SerializeField] private Material windowMaterial;

        private Transform cityParent;
        private System.Random rng;

        public void GenerateCity(int seed = 0)
        {
            // Create parent for organization
            if (cityParent == null)
            {
                cityParent = new GameObject("=== PROCEDURAL CITY ===").transform;
            }

            // Clear existing city
            foreach (Transform child in cityParent)
            {
                DestroyImmediate(child.gameObject);
            }

            rng = new System.Random(seed == 0 ? System.DateTime.Now.Millisecond : seed);

            Debug.Log($"[CityGenerator] Generating {cityWidth}x{cityDepth} city...");

            GenerateRoadNetwork();
            GenerateBuildings();
            if (generateStreetLamps) GenerateStreetLamps();
            if (generateTrafficLights) GenerateTrafficLights();
            if (generateBenches) GenerateStreetFurniture();

            Debug.Log("[CityGenerator] City generation complete!");
        }

        private void GenerateRoadNetwork()
        {
            Transform roadsParent = CreateChild("Roads", cityParent);

            // Generate horizontal roads
            for (int z = 0; z <= cityDepth; z++)
            {
                float posZ = z * blockSize;
                CreateRoad(roadsParent, Vector3.zero + Vector3.forward * posZ, 
                    cityWidth * blockSize + blockSize, roadWidth, 0f);
                
                // Add sidewalks on both sides
                CreateSidewalk(roadsParent, 
                    new Vector3(0, 0.05f, posZ - roadWidth/2 - sidewalkWidth/2),
                    cityWidth * blockSize + blockSize, sidewalkWidth, 0f);
                CreateSidewalk(roadsParent,
                    new Vector3(0, 0.05f, posZ + roadWidth/2 + sidewalkWidth/2),
                    cityWidth * blockSize + blockSize, sidewalkWidth, 0f);
            }

            // Generate vertical roads
            for (int x = 0; x <= cityWidth; x++)
            {
                float posX = x * blockSize;
                CreateRoad(roadsParent, Vector3.zero + Vector3.right * posX,
                    roadWidth, cityDepth * blockSize + blockSize, 90f);
                
                // Add sidewalks on both sides
                CreateSidewalk(roadsParent,
                    new Vector3(posX - roadWidth/2 - sidewalkWidth/2, 0.05f, 0),
                    sidewalkWidth, cityDepth * blockSize + blockSize, 90f);
                CreateSidewalk(roadsParent,
                    new Vector3(posX + roadWidth/2 + sidewalkWidth/2, 0.05f, 0),
                    sidewalkWidth, cityDepth * blockSize + blockSize, 90f);
            }
        }

        private void CreateRoad(Transform parent, Vector3 position, float width, float depth, float rotation)
        {
            GameObject road = GameObject.CreatePrimitive(PrimitiveType.Cube);
            road.name = "Road_Segment";
            road.transform.SetParent(parent);
            road.transform.position = position + new Vector3(width / 2, 0, depth / 2);
            road.transform.localScale = new Vector3(width, 0.1f, depth);
            road.transform.Rotate(0, rotation, 0);
            road.isStatic = true;

            if (roadMaterial != null)
            {
                road.GetComponent<MeshRenderer>().material = roadMaterial;
            }
            else
            {
                // Default dark asphalt
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(0.15f, 0.15f, 0.15f);
                mat.SetFloat("_Metallic", 0f);
                mat.SetFloat("_Glossiness", 0.3f);
                road.GetComponent<MeshRenderer>().material = mat;
            }
        }

        private void CreateSidewalk(Transform parent, Vector3 position, float width, float depth, float rotation)
        {
            GameObject sidewalk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sidewalk.name = "Sidewalk_Segment";
            sidewalk.transform.SetParent(parent);
            sidewalk.transform.position = position + new Vector3(width / 2, 0, depth / 2);
            sidewalk.transform.localScale = new Vector3(width, 0.15f, depth);
            sidewalk.transform.Rotate(0, rotation, 0);
            sidewalk.isStatic = true;

            if (sidewalkMaterial != null)
            {
                sidewalk.GetComponent<MeshRenderer>().material = sidewalkMaterial;
            }
            else
            {
                // Default light concrete
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(0.7f, 0.7f, 0.7f);
                mat.SetFloat("_Metallic", 0f);
                mat.SetFloat("_Glossiness", 0.2f);
                sidewalk.GetComponent<MeshRenderer>().material = mat;
            }
        }

        private void GenerateBuildings()
        {
            Transform buildingsParent = CreateChild("Buildings", cityParent);

            for (int x = 0; x < cityWidth; x++)
            {
                for (int z = 0; z < cityDepth; z++)
                {
                    Vector3 blockCenter = new Vector3(
                        x * blockSize + blockSize / 2,
                        0,
                        z * blockSize + blockSize / 2
                    );

                    GenerateBlockBuildings(buildingsParent, blockCenter, x, z);
                }
            }
        }

        private void GenerateBlockBuildings(Transform parent, Vector3 blockCenter, int blockX, int blockZ)
        {
            Transform blockParent = CreateChild($"Block_{blockX}_{blockZ}", parent);

            // Create 2-4 buildings per block with varied sizes
            int count = rng.Next(2, buildingsPerBlock + 1);

            for (int i = 0; i < count; i++)
            {
                float height = Mathf.Lerp(minBuildingHeight, maxBuildingHeight, (float)rng.NextDouble());
                float width = Mathf.Lerp(buildingFootprintRange.x, buildingFootprintRange.y, (float)rng.NextDouble());
                float depth = Mathf.Lerp(buildingFootprintRange.x, buildingFootprintRange.y, (float)rng.NextDouble());

                // Random Position within block (leave space from edges)
                float offsetRange = (blockSize - roadWidth) / 2 - width;
                Vector3 offset = new Vector3(
                    ((float)rng.NextDouble() - 0.5f) * offsetRange,
                    height / 2,
                    ((float)rng.NextDouble() - 0.5f) * offsetRange
                );

                CreateBuilding(blockParent, blockCenter + offset, width, height, depth);
            }
        }

        private void CreateBuilding(Transform parent, Vector3 position, float width, float height, float depth)
        {
            GameObject building = GameObject.CreatePrimitive(PrimitiveType.Cube);
            building.name = $"Building_{parent.childCount}";
            building.transform.SetParent(parent);
            building.transform.position = position;
            building.transform.localScale = new Vector3(width, height, depth);
            building.isStatic = true;

            if (buildingMaterial != null)
            {
                building.GetComponent<MeshRenderer>().material = buildingMaterial;
            }
            else
            {
                // Create nice modern building material
                Material mat = new Material(Shader.Find("Standard"));
                
                // Varied colors - blues, greys
                float hue = Mathf.Lerp(0.55f, 0.65f, (float)rng.NextDouble());
                float sat = Mathf.Lerp(0.2f, 0.4f, (float)rng.NextDouble());
                float val = Mathf.Lerp(0.4f, 0.6f, (float)rng.NextDouble());
                mat.color = Color.HSVToRGB(hue, sat, val);
                
                mat.SetFloat("_Metallic", Mathf.Lerp(0.3f, 0.6f, (float)rng.NextDouble()));
                mat.SetFloat("_Glossiness", Mathf.Lerp(0.5f, 0.8f, (float)rng.NextDouble()));
                
                building.GetComponent<MeshRenderer>().material = mat;
            }

            // Add windows (simple emissive planes)
            AddWindows(building.transform, width, height, depth);
        }

        private void AddWindows(Transform building, float width, float height, float depth)
        {
            // Simple window grid on front face
            int rows = Mathf.FloorToInt(height / 3f);
            int cols = Mathf.FloorToInt(width / 2f);

            for (int r = 1; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    GameObject window = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    window.name = "Window";
                    window.transform.SetParent(building);
                    
                    float x = -width/2 + (c + 0.5f) * (width / cols);
                    float y = -height/2 + (r + 0.5f) * (height / rows);
                    float z = depth/2 + 0.01f;
                    
                    window.transform.localPosition = new Vector3(x, y, z);
                    window.transform.localScale = Vector3.one * 0.8f;
                    DestroyImmediate(window.GetComponent<Collider>());

                    // Window material (emissive at night)
                    Material winMat = new Material(Shader.Find("Standard"));
                    winMat.color = new Color(0.8f, 0.9f, 1f);
                    winMat.SetFloat("_Metallic", 0f);
                    winMat.SetFloat("_Glossiness", 0.9f);
                    winMat.EnableKeyword("_EMISSION");
                    winMat.SetColor("_EmissionColor", new Color(1f, 0.9f, 0.7f) * 0.3f);
                    window.GetComponent<MeshRenderer>().material = winMat;
                }
            }
        }

        private void GenerateStreetLamps()
        {
            Transform lampsParent = CreateChild("StreetLamps", cityParent);

            // Place lamps along roads
            for (int x = 0; x <= cityWidth; x++)
            {
                for (float z = 0; z < cityDepth * blockSize; z += streetLampSpacing)
                {
                    float posX = x * blockSize;
                    CreateStreetLamp(lampsParent, new Vector3(posX + roadWidth/2 + sidewalkWidth + 0.5f, 0, z));
                }
            }
        }

        private void CreateStreetLamp(Transform parent, Vector3 position)
        {
            GameObject lamp = new GameObject("StreetLamp");
            lamp.transform.SetParent(parent);
            lamp.transform.position = position;

            // Pole
            GameObject pole = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            pole.name = "Pole";
            pole.transform.SetParent(lamp.transform);
            pole.transform.localPosition = new Vector3(0, 3, 0);
            pole.transform.localScale = new Vector3(0.2f, 3f, 0.2f);
            
            Material poleMat = new Material(Shader.Find("Standard"));
            poleMat.color = new Color(0.2f, 0.2f, 0.2f);
            poleMat.SetFloat("_Metallic", 0.8f);
            poleMat.SetFloat("_Glossiness", 0.6f);
            pole.GetComponent<MeshRenderer>().material = poleMat;

            // Light fixture
            GameObject fixture = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            fixture.name = "Fixture";
            fixture.transform.SetParent(lamp.transform);
            fixture.transform.localPosition = new Vector3(0, 6.5f, 0);
            fixture.transform.localScale = Vector3.one * 0.5f;
            
            Material fixtureMat = new Material(Shader.Find("Standard"));
            fixtureMat.color = new Color(1f, 0.9f, 0.7f);
            fixtureMat.EnableKeyword("_EMISSION");
            fixtureMat.SetColor("_EmissionColor", new Color(1f, 0.9f, 0.7f) * 2f);
            fixture.GetComponent<MeshRenderer>().material = fixtureMat;

            // Point light
            Light light = fixture.AddComponent<Light>();
            light.type = LightType.Point;
            light.color = new Color(1f, 0.9f, 0.7f);
            light.intensity = 2f;
            light.range = 15f;
            light.shadows = LightShadows.Soft;
        }

        private void GenerateTrafficLights()
        {
            Transform lightsParent = CreateChild("TrafficLights", cityParent);

            // Place at intersections
            for (int x = 0; x <= cityWidth; x++)
            {
                for (int z = 0; z <= cityDepth; z++)
                {
                    Vector3 pos = new Vector3(x * blockSize, 0, z * blockSize);
                    CreateTrafficLight(lightsParent, pos + new Vector3(3, 0, 3));
                }
            }
        }

        private void CreateTrafficLight(Transform parent, Vector3 position)
        {
            GameObject trafficLight = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trafficLight.name = "TrafficLight";
            trafficLight.transform.SetParent(parent);
            trafficLight.transform.position = position + Vector3.up * 2.5f;
            trafficLight.transform.localScale = new Vector3(0.3f, 2.5f, 0.3f);
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.1f, 0.1f, 0.1f);
            trafficLight.GetComponent<MeshRenderer>().material = mat;
        }

        private void GenerateStreetFurniture()
        {
            Transform furnitureParent = CreateChild("StreetFurniture", cityParent);

            // Randomly place benches on sidewalks
            for (int i = 0; i < cityWidth * cityDepth * 2; i++)
            {
                Vector3 randomPos = new Vector3(
                    (float)rng.NextDouble() * cityWidth * blockSize,
                    0.3f,
                    (float)rng.NextDouble() * cityDepth * blockSize
                );
                CreateBench(furnitureParent, randomPos);
            }
        }

        private void CreateBench(Transform parent, Vector3 position)
        {
            GameObject bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bench.name = "Bench";
            bench.transform.SetParent(parent);
            bench.transform.position = position;
            bench.transform.localScale = new Vector3(1.5f, 0.4f, 0.5f);
            
            Material mat = new Material(Shader.Find("Standard"));
            mat.color = new Color(0.4f, 0.3f, 0.2f); // Wood color
            bench.GetComponent<MeshRenderer>().material = mat;
        }

        private GameObject CreateChild(string name, Transform parent)
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.transform.localPosition = Vector3.zero;
            return obj;
        }
    }
}
