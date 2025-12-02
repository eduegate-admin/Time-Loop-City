using UnityEngine;
using UnityEditor;

namespace TimeLoopCity.Editor.KochiSuite
{
    /// <summary>
    /// Terrain generator - heightmaps (multiple LODs), backwaters, coastal terrain
    /// </summary>
    public class TerrainGenerator : KochiGeneratorBase
    {
        private int heightmapResolution = 512;
        private float noiseScale = 0.05f;
        private int noiseOctaves = 4;
        private bool generateBackwaters = true;
        private bool generateCoastal = true;

        public override void DrawGUI()
        {
            EditorGUILayout.LabelField("Terrain Settings", EditorStyles.boldLabel);
            
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Heightmap Resolution");
            if (GUILayout.Button("512")) heightmapResolution = 512;
            if (GUILayout.Button("1024")) heightmapResolution = 1024;
            if (GUILayout.Button("2048")) heightmapResolution = 2048;
            EditorGUILayout.EndHorizontal();

            noiseScale = EditorGUILayout.Slider("Noise Scale", noiseScale, 0.01f, 0.2f);
            noiseOctaves = EditorGUILayout.IntSlider("Noise Octaves", noiseOctaves, 1, 8);
            generateBackwaters = EditorGUILayout.Toggle("Generate Backwaters", generateBackwaters);
            generateCoastal = EditorGUILayout.Toggle("Generate Coastal Features", generateCoastal);

            EditorGUILayout.HelpBox(
                "• Heightmaps: Multi-LOD terrain (512/1024/2048)\n" +
                "• Backwaters: Water bodies and topology\n" +
                "• Coastal: Beach, cliffs, erosion effects",
                MessageType.Info);
        }

        public void GenerateTerrain()
        {
            isRunning = true;
            progress = 0f;

            try
            {
                GenerateHeightmapLOD();
                SetProgress(0.33f);

                if (generateBackwaters)
                {
                    GenerateBackwaters();
                    SetProgress(0.66f);
                }

                if (generateCoastal)
                {
                    GenerateCoastalTerrain();
                    SetProgress(0.95f);
                }

                LogSuccess("Terrain generation completed");
                SetProgress(1f);
            }
            catch (System.Exception e)
            {
                LogError($"Terrain generation failed: {e.Message}");
            }
            finally
            {
                isRunning = false;
            }
        }

        private void GenerateHeightmapLOD()
        {
            EnsureFolder("Assets/TimeLoopKochi/Terrain");

            GameObject terrainRoot = FindOrCreateRoot("TerrainSystem");
            GameObject heightmapRoot = new GameObject("Heightmaps");
            heightmapRoot.transform.parent = terrainRoot.transform;

            int[] resolutions = { 512, 1024, 2048 };

            foreach (int resolution in resolutions)
            {
                Terrain terrain = CreateTerrainAtResolution(heightmapRoot, resolution);
                terrain.name = $"Terrain_LOD_{resolution}";
                
                if (resolution == heightmapResolution)
                {
                    terrain.gameObject.SetActive(true);
                }
                else
                {
                    terrain.gameObject.SetActive(false);
                }
            }

            LogSuccess($"Generated heightmap LODs (512, 1024, 2048)");
        }

        private Terrain CreateTerrainAtResolution(GameObject parent, int resolution)
        {
            TerrainData terrainData = new TerrainData();
            terrainData.heightmapResolution = resolution;
            terrainData.size = new Vector3(500f, 100f, 500f);

            float[,] heights = new float[resolution, resolution];

            for (int y = 0; y < resolution; y++)
            {
                for (int x = 0; x < resolution; x++)
                {
                    float xCoord = (float)x / resolution * noiseScale;
                    float yCoord = (float)y / resolution * noiseScale;

                    float perlin = 0f;
                    float amplitude = 1f;
                    float frequency = 1f;
                    float maxValue = 0f;

                    for (int i = 0; i < noiseOctaves; i++)
                    {
                        perlin += Mathf.PerlinNoise(xCoord * frequency, yCoord * frequency) * amplitude;
                        maxValue += amplitude;
                        amplitude *= 0.5f;
                        frequency *= 2f;
                    }

                    heights[y, x] = perlin / maxValue;
                }
            }

            terrainData.SetHeights(0, 0, heights);

            GameObject terrainObj = Terrain.CreateTerrainGameObject(terrainData);
            terrainObj.name = $"Terrain_{resolution}";
            terrainObj.transform.parent = parent.transform;
            terrainObj.transform.position = Vector3.zero;

            return terrainObj.GetComponent<Terrain>();
        }

        private void GenerateBackwaters()
        {
            GameObject terrainRoot = FindOrCreateRoot("TerrainSystem");
            GameObject backwatersRoot = new GameObject("Backwaters");
            backwatersRoot.transform.parent = terrainRoot.transform;

            for (int i = 0; i < 5; i++)
            {
                GameObject waterBody = new GameObject($"BackwaterBody_{i}");
                waterBody.transform.parent = backwatersRoot.transform;
                waterBody.transform.position = new Vector3(
                    Random.Range(-200f, 200f),
                    -0.5f,
                    Random.Range(-200f, 200f)
                );

                var plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                plane.name = "Surface";
                plane.transform.parent = waterBody.transform;
                plane.transform.localScale = new Vector3(Random.Range(5f, 20f), 1f, Random.Range(5f, 20f));
                plane.transform.localPosition = Vector3.zero;

                Object.DestroyImmediate(plane.GetComponent<Collider>());

                var renderer = plane.GetComponent<Renderer>();
                var waterMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                waterMat.color = new Color(0.1f, 0.4f, 0.5f, 0.6f);
                waterMat.SetFloat("_Metallic", 0.3f);
                renderer.material = waterMat;

                var volume = waterBody.AddComponent<BoxCollider>();
                volume.size = new Vector3(plane.transform.localScale.x * 10f, 2f, plane.transform.localScale.z * 10f);
                volume.isTrigger = true;
            }

            LogSuccess("Generated 5 backwater bodies");
        }

        private void GenerateCoastalTerrain()
        {
            GameObject terrainRoot = FindOrCreateRoot("TerrainSystem");
            GameObject coastalRoot = new GameObject("CoastalFeatures");
            coastalRoot.transform.parent = terrainRoot.transform;

            // Generate beach
            GameObject beach = new GameObject("Beach");
            beach.transform.parent = coastalRoot.transform;
            beach.transform.position = new Vector3(-200f, 0f, 0f);

            var beachPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            beachPlane.name = "BeachPlane";
            beachPlane.transform.parent = beach.transform;
            beachPlane.transform.localScale = new Vector3(30f, 1f, 50f);
            beachPlane.transform.localPosition = Vector3.zero;

            Object.DestroyImmediate(beachPlane.GetComponent<Collider>());

            var beachRenderer = beachPlane.GetComponent<Renderer>();
            var sandMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            sandMat.color = new Color(0.9f, 0.8f, 0.6f);
            beachRenderer.material = sandMat;

            // Generate cliffs
            for (int i = 0; i < 3; i++)
            {
                GameObject cliff = new GameObject($"Cliff_{i}");
                cliff.transform.parent = coastalRoot.transform;
                cliff.transform.position = new Vector3(-150f + i * 50f, 0f, -150f);

                var cliffGeo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cliffGeo.name = "CliffFace";
                cliffGeo.transform.parent = cliff.transform;
                cliffGeo.transform.localScale = new Vector3(40f, 30f, 5f);
                cliffGeo.transform.localPosition = Vector3.zero;

                Object.DestroyImmediate(cliffGeo.GetComponent<Collider>());

                var cliffRenderer = cliffGeo.GetComponent<Renderer>();
                var rockMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                rockMat.color = new Color(0.5f, 0.5f, 0.45f);
                cliffRenderer.material = rockMat;
            }

            LogSuccess("Generated coastal features (beach, cliffs)");
        }
    }
}