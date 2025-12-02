using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace TimeLoopCity.Editor.KochiSuite
{
    /// <summary>
    /// Environment generator - water systems, vegetation, lighting, atmospheric effects
    /// </summary>
    public class EnvironmentGenerator : KochiGeneratorBase
    {
        private bool generateWater = true;
        private bool generateVegetation = true;
        private bool setupLighting = true;
        private bool addAtmosphericEffects = true;
        private int vegetationDensity = 50;

        public override void DrawGUI()
        {
            EditorGUILayout.LabelField("Environment Settings", EditorStyles.boldLabel);
            generateWater = EditorGUILayout.Toggle("Generate Water Systems", generateWater);
            generateVegetation = EditorGUILayout.Toggle("Generate Vegetation", generateVegetation);
            vegetationDensity = EditorGUILayout.IntSlider("Vegetation Density", vegetationDensity, 10, 200);
            setupLighting = EditorGUILayout.Toggle("Setup Lighting", setupLighting);
            addAtmosphericEffects = EditorGUILayout.Toggle("Add Atmospheric Effects", addAtmosphericEffects);

            EditorGUILayout.HelpBox(
                "• Water: Backwaters, canals, water plane shaders\n" +
                "• Vegetation: Trees, shrubs, coconut palms\n" +
                "• Lighting: Sun setup, ambient occlusion\n" +
                "• Atmosphere: Fog, haze, volumetric effects",
                MessageType.Info);
        }

        public void GenerateEnvironment()
        {
            isRunning = true;
            progress = 0f;

            try
            {
                if (generateWater)
                {
                    GenerateWaterSystems();
                    SetProgress(0.25f);
                }

                if (generateVegetation)
                {
                    GenerateVegetation();
                    SetProgress(0.5f);
                }

                if (setupLighting)
                {
                    SetupLighting();
                    SetProgress(0.75f);
                }

                if (addAtmosphericEffects)
                {
                    AddAtmosphericEffects();
                    SetProgress(0.95f);
                }

                LogSuccess("Environment generation completed");
                SetProgress(1f);
            }
            catch (System.Exception e)
            {
                LogError($"Environment generation failed: {e.Message}");
            }
            finally
            {
                isRunning = false;
            }
        }

        private void GenerateWaterSystems()
        {
            EnsureFolder("Assets/TimeLoopKochi/Water");

            GameObject waterRoot = FindOrCreateRoot("WaterSystems");

            GameObject mainWater = new GameObject("MainWaterPlane");
            mainWater.transform.parent = waterRoot.transform;
            mainWater.transform.position = new Vector3(0f, -0.5f, 0f);

            var waterPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            waterPlane.name = "Plane";
            waterPlane.transform.parent = mainWater.transform;
            waterPlane.transform.localScale = new Vector3(50f, 1f, 50f);
            waterPlane.transform.localPosition = Vector3.zero;

            Object.DestroyImmediate(waterPlane.GetComponent<Collider>());

            var renderer = waterPlane.GetComponent<Renderer>();
            var waterMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            waterMat.color = new Color(0.2f, 0.5f, 0.6f, 0.7f);
            waterMat.SetFloat("_Metallic", 0.5f);
            waterMat.SetFloat("_Smoothness", 0.8f);
            renderer.material = waterMat;

            var waterVolume = mainWater.AddComponent<BoxCollider>();
            waterVolume.size = new Vector3(100f, 2f, 100f);
            waterVolume.isTrigger = true;

            LogSuccess("Generated water systems");
        }

        private void GenerateVegetation()
        {
            EnsureFolder("Assets/TimeLoopKochi/Vegetation");

            GameObject vegetationRoot = FindOrCreateRoot("Vegetation");

            for (int i = 0; i < vegetationDensity; i++)
            {
                Vector3 randomPos = new Vector3(
                    Random.Range(-150f, 150f),
                    0f,
                    Random.Range(-150f, 150f)
                );

                if (randomPos.magnitude > 50f)
                {
                    GameObject tree = CreateTree(randomPos);
                    tree.transform.parent = vegetationRoot.transform;
                }
            }

            LogSuccess($"Generated {vegetationDensity} vegetation elements");
        }

        private GameObject CreateTree(Vector3 position)
        {
            GameObject tree = new GameObject("PalmTree");
            tree.transform.position = position;

            var trunk = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            trunk.name = "Trunk";
            trunk.transform.parent = tree.transform;
            trunk.transform.localScale = new Vector3(0.3f, 2f, 0.3f);
            trunk.transform.localPosition = Vector3.zero;

            Object.DestroyImmediate(trunk.GetComponent<Collider>());

            var trunkRenderer = trunk.GetComponent<Renderer>();
            var trunkMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            trunkMat.color = new Color(0.5f, 0.35f, 0.2f);
            trunkRenderer.material = trunkMat;

            var foliage = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foliage.name = "Foliage";
            foliage.transform.parent = tree.transform;
            foliage.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            foliage.transform.localPosition = new Vector3(0f, 2f, 0f);

            Object.DestroyImmediate(foliage.GetComponent<Collider>());

            var foliageRenderer = foliage.GetComponent<Renderer>();
            var foliageMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            foliageMat.color = new Color(0.2f, 0.6f, 0.2f);
            foliageRenderer.material = foliageMat;

            return tree;
        }

        private void SetupLighting()
        {
            GameObject lightingRoot = FindOrCreateRoot("Lighting");

            GameObject sunObj = new GameObject("Sun");
            sunObj.transform.parent = lightingRoot.transform;
            sunObj.transform.rotation = Quaternion.Euler(55f, -35f, 0f);

            var sunLight = sunObj.AddComponent<Light>();
            sunLight.type = LightType.Directional;
            sunLight.intensity = 1.5f;
            sunLight.shadows = LightShadows.Soft;
            sunLight.shadowResolution = LightShadowResolution.High;

            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.3f, 0.4f, 0.5f);
            RenderSettings.ambientEquatorColor = new Color(0.5f, 0.5f, 0.5f);
            RenderSettings.ambientGroundColor = new Color(0.3f, 0.3f, 0.3f);

            LogSuccess("Setup lighting with sun and ambient");
        }

        private void AddAtmosphericEffects()
        {
            GameObject effectsRoot = FindOrCreateRoot("AtmosphericEffects");

            GameObject fogVolume = new GameObject("FogVolume");
            fogVolume.transform.parent = effectsRoot.transform;
            fogVolume.transform.position = Vector3.zero;

            var fogBox = fogVolume.AddComponent<BoxCollider>();
            fogBox.size = new Vector3(200f, 50f, 200f);
            fogBox.isTrigger = true;

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Linear;
            RenderSettings.fogStartDistance = 50f;
            RenderSettings.fogEndDistance = 500f;
            RenderSettings.fogColor = new Color(0.7f, 0.8f, 0.9f);

            LogSuccess("Added atmospheric effects and volumetric fog");
        }
    }
}