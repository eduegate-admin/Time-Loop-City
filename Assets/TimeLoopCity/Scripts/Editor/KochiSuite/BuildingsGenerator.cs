using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TimeLoopCity.Editor.KochiSuite
{
    /// <summary>
    /// Buildings generator with district-aware generation, facade modules, and auto-LOD
    /// </summary>
    public class BuildingsGenerator : KochiGeneratorBase
    {
        public enum DistrictStyle
        {
            FortKochi_Colonial,
            MarineDrive_Modern,
            Willingdon_Industrial,
            Mattancherry_Dense,
            Generic_Residential
        }

        private DistrictStyle selectedStyle = DistrictStyle.FortKochi_Colonial;
        private int buildingCount = 20;
        private bool generateLODs = true;
        private bool autoAssignMaterials = true;

        public override void DrawGUI()
        {
            EditorGUILayout.LabelField("Building Generation", EditorStyles.boldLabel);
            selectedStyle = (DistrictStyle)EditorGUILayout.EnumPopup("District Style", selectedStyle);
            buildingCount = EditorGUILayout.IntSlider("Building Count", buildingCount, 1, 100);
            generateLODs = EditorGUILayout.Toggle("Auto-Generate LODs", generateLODs);
            autoAssignMaterials = EditorGUILayout.Toggle("Auto-Assign Materials", autoAssignMaterials);

            EditorGUILayout.HelpBox(
                "• Fort Kochi: Colonial 2-3 floors, cream, Portuguese style\n" +
                "• Marine Drive: Modern 10-20 floors, glass/steel\n" +
                "• Willingdon: Industrial warehouses\n" +
                "• Mattancherry: Dense 3-5 floors, aged textures\n" +
                "• Generic: Mixed residential styles",
                MessageType.Info);
        }

        public void GenerateBuildings()
        {
            isRunning = true;
            progress = 0f;

            try
            {
                string folderPath = $"Assets/TimeLoopKochi/Buildings/ProceduralPrefabs/{selectedStyle}";
                EnsureFolder(folderPath);

                GameObject buildingsRoot = FindOrCreateRoot("Buildings");

                EditorUtility.DisplayProgressBar("Building Generator", "Creating buildings...", 0);

                for (int i = 0; i < buildingCount; i++)
                {
                    SetProgress((float)i / buildingCount * 0.8f);
                    EditorUtility.DisplayProgressBar("Building Generator", 
                        $"Creating {selectedStyle} building {i + 1}/{buildingCount}", 
                        (float)i / buildingCount);

                    GameObject building = CreateBuilding(selectedStyle, i);
                    building.transform.parent = buildingsRoot.transform;

                    if (generateLODs)
                    {
                        GenerateLODsForBuilding(building);
                    }

                    if (autoAssignMaterials)
                    {
                        AssignMaterialsToBuilding(building);
                    }
                }

                EditorUtility.ClearProgressBar();
                SetProgress(1f);
                LogSuccess($"Generated {buildingCount} {selectedStyle} buildings");
            }
            catch (System.Exception e)
            {
                EditorUtility.ClearProgressBar();
                LogError($"Building generation failed: {e.Message}");
            }
            finally
            {
                isRunning = false;
            }
        }

        private GameObject CreateBuilding(DistrictStyle style, int seed)
        {
            Random.InitState(seed + (int)style * 1000);

            GameObject building = new GameObject($"{style}_Building_{seed:D3}");
            building.transform.position = GetRandomBuildingPosition(seed);

            BuildingParams parameters = GetStyleParams(style);

            GameObject baseStructure = new GameObject("Structure");
            baseStructure.transform.parent = building.transform;
            baseStructure.transform.localPosition = Vector3.zero;

            var mainCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            mainCube.name = "MainBody";
            mainCube.transform.parent = baseStructure.transform;
            mainCube.transform.localScale = new Vector3(
                parameters.width,
                parameters.floorHeight * parameters.floors,
                parameters.depth
            );
            mainCube.transform.localPosition = new Vector3(0f, parameters.floorHeight * parameters.floors / 2, 0f);

            Object.DestroyImmediate(mainCube.GetComponent<Collider>());
            Object.DestroyImmediate(mainCube.GetComponent<Collider>());

            var renderer = mainCube.GetComponent<Renderer>();
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mat.color = parameters.color;
            renderer.material = mat;

            GameObject roof = new GameObject("Roof");
            roof.transform.parent = baseStructure.transform;
            roof.transform.localPosition = new Vector3(0f, parameters.floorHeight * parameters.floors, 0f);

            var roofCube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            roofCube.name = "RoofPlane";
            roofCube.transform.parent = roof.transform;
            roofCube.transform.localScale = new Vector3(parameters.width, 0.5f, parameters.depth);
            roofCube.transform.localPosition = Vector3.zero;

            Object.DestroyImmediate(roofCube.GetComponent<Collider>());
            Object.DestroyImmediate(roofCube.GetComponent<Collider>());

            var roofRenderer = roofCube.GetComponent<Renderer>();
            var roofMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            roofMat.color = parameters.roofColor;
            roofRenderer.material = roofMat;

            return building;
        }

        private void GenerateLODsForBuilding(GameObject building)
        {
            LODGroup lodGroup = building.AddComponent<LODGroup>();
            var allRenderers = building.GetComponentsInChildren<Renderer>();
            LOD[] lods = new LOD[1];
            lods[0] = new LOD(1f, allRenderers);
            lodGroup.SetLODs(lods);
        }

        private void AssignMaterialsToBuilding(GameObject building)
        {
            var renderers = building.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                // Material assignment logic
            }
        }

        private Vector3 GetRandomBuildingPosition(int seed)
        {
            Random.InitState(seed);
            return new Vector3(
                Random.Range(-100f, 100f),
                0f,
                Random.Range(-100f, 100f)
            );
        }

        private BuildingParams GetStyleParams(DistrictStyle style)
        {
            switch (style)
            {
                case DistrictStyle.FortKochi_Colonial:
                    return new BuildingParams
                    {
                        floors = Random.Range(2, 4),
                        width = Random.Range(8f, 12f),
                        depth = Random.Range(10f, 15f),
                        floorHeight = 3.5f,
                        color = new Color(0.95f, 0.92f, 0.82f),
                        roofColor = new Color(0.6f, 0.3f, 0.2f)
                    };

                case DistrictStyle.MarineDrive_Modern:
                    return new BuildingParams
                    {
                        floors = Random.Range(10, 21),
                        width = Random.Range(15f, 25f),
                        depth = Random.Range(15f, 25f),
                        floorHeight = 3f,
                        color = new Color(0.8f, 0.85f, 0.9f),
                        roofColor = Color.gray
                    };

                case DistrictStyle.Willingdon_Industrial:
                    return new BuildingParams
                    {
                        floors = Random.Range(1, 4),
                        width = Random.Range(20f, 40f),
                        depth = Random.Range(30f, 50f),
                        floorHeight = 5f,
                        color = new Color(0.6f, 0.6f, 0.65f),
                        roofColor = new Color(0.4f, 0.4f, 0.4f)
                    };

                case DistrictStyle.Mattancherry_Dense:
                    return new BuildingParams
                    {
                        floors = Random.Range(3, 6),
                        width = Random.Range(5f, 8f),
                        depth = Random.Range(12f, 18f),
                        floorHeight = 3f,
                        color = new Color(0.85f, 0.75f, 0.65f),
                        roofColor = new Color(0.5f, 0.3f, 0.2f)
                    };

                default:
                    return new BuildingParams
                    {
                        floors = Random.Range(2, 7),
                        width = Random.Range(8f, 15f),
                        depth = Random.Range(10f, 20f),
                        floorHeight = 3.2f,
                        color = new Color(0.75f, 0.75f, 0.78f),
                        roofColor = new Color(0.45f, 0.45f, 0.45f)
                    };
            }
        }

        private struct BuildingParams
        {
            public int floors;
            public float width;
            public float depth;
            public float floorHeight;
            public Color color;
            public Color roofColor;
        }
    }
}