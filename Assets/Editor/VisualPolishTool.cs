#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace TimeLoopCity.Editor
{
    public class VisualPolishTool : EditorWindow
    {
        [MenuItem("Tools/Time Loop City/✨ VISUAL POLISH", false, 1)]
        public static void ApplyVisualPolish()
        {
            if (!EditorUtility.DisplayDialog("Visual Polish", 
                "This will dramatically improve your game's visuals:\n\n" +
                "✨ Cinematic lighting (golden hour)\n" +
                "🎨 Rich materials (metallic buildings)\n" +
                "🌅 Beautiful skybox\n" +
                "💫 Atmospheric fog\n" +
                "⚡ Particle effects\n" +
                "📸 Better camera settings\n\n" +
                "Continue?", "Yes, Make It Beautiful!", "Cancel"))
            {
                return;
            }

            Debug.Log("<b>[Visual Polish]</b> Starting visual enhancement...");

            ApplyLighting();
            ApplyMaterials();
            ApplySkybox();
            ApplyFog();
            ApplyCamera();
            AddAtmosphericEffects();

            Debug.Log("<b>[Visual Polish]</b> <color=green>✨ COMPLETE! Your game looks amazing now!</color>");
            EditorUtility.DisplayDialog("Visual Polish Complete!", 
                "🎉 Your game has been transformed!\n\n" +
                "The scene now has:\n" +
                "✅ Cinematic lighting\n" +
                "✅ Beautiful materials\n" +
                "✅ Skybox & atmosphere\n" +
                "✅ Professional camera setup\n\n" +
                "Press Play to see the difference!", "Awesome!");
        }

        private static void ApplyLighting()
        {
            Debug.Log("[Visual Polish] Applying cinematic lighting...");

            // Find or create Sun
            Light sunLight = GameObject.Find("Sun Light")?.GetComponent<Light>();
            
            if (sunLight != null)
            {
                // Golden hour lighting
                sunLight.color = new Color(1f, 0.95f, 0.8f); // Warm golden color
                sunLight.intensity = 1.5f;
                sunLight.shadows = LightShadows.Soft;
                sunLight.shadowStrength = 0.7f;
                sunLight.transform.rotation = Quaternion.Euler(45f, -30f, 0f); // Better angle
                
                Debug.Log("  ✓ Sun configured with golden hour lighting");
            }

            // Ambient lighting - warm and inviting
            RenderSettings.ambientMode = AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = new Color(0.5f, 0.6f, 0.7f); // Soft blue sky
            RenderSettings.ambientEquatorColor = new Color(0.4f, 0.35f, 0.3f); // Warm horizon
            RenderSettings.ambientGroundColor = new Color(0.2f, 0.2f, 0.25f); // Cool ground

            Debug.Log("  ✓ Ambient lighting configured");
        }

        private static void ApplyMaterials()
        {
            Debug.Log("[Visual Polish] Applying beautiful materials...");

            // Find all buildings
            GameObject[] buildings = GameObject.FindGameObjectsWithTag("Untagged");
            int buildingCount = 0;

            foreach (var obj in buildings)
            {
                if (obj.name.Contains("Building"))
                {
                    MeshRenderer renderer = obj.GetComponent<MeshRenderer>();
                    if (renderer != null)
                    {
                        // Create a nice material
                        Material mat = new Material(Shader.Find("Standard"));
                        
                        // Vary colors for visual interest
                        float hue = Random.Range(0.55f, 0.65f); // Blue-ish range
                        float saturation = Random.Range(0.3f, 0.5f);
                        float value = Random.Range(0.4f, 0.6f);
                        mat.color = Color.HSVToRGB(hue, saturation, value);
                        
                        // Make it look like modern architecture
                        mat.SetFloat("_Metallic", Random.Range(0.3f, 0.7f));
                        mat.SetFloat("_Glossiness", Random.Range(0.5f, 0.8f));
                        
                        renderer.sharedMaterial = mat;
                        buildingCount++;
                    }
                }
            }

            // Improve ground material
            GameObject ground = GameObject.Find("Ground");
            if (ground != null)
            {
                MeshRenderer renderer = ground.GetComponent<MeshRenderer>();
                if (renderer != null)
                {
                    Material groundMat = new Material(Shader.Find("Standard"));
                    groundMat.color = new Color(0.15f, 0.3f, 0.2f); // Dark green grass
                    groundMat.SetFloat("_Metallic", 0f);
                    groundMat.SetFloat("_Glossiness", 0.2f);
                    renderer.sharedMaterial = groundMat;
                    
                    Debug.Log("  ✓ Ground material improved");
                }
            }

            Debug.Log($"  ✓ Enhanced {buildingCount} buildings with modern materials");
        }

        private static void ApplySkybox()
        {
            Debug.Log("[Visual Polish] Creating beautiful skybox...");

            // Create a gradient skybox material
            Material skyboxMat = new Material(Shader.Find("Skybox/Procedural"));
            
            // Sunset/golden hour colors
            skyboxMat.SetColor("_SkyTint", new Color(0.5f, 0.7f, 1f)); // Soft blue
            skyboxMat.SetColor("_GroundColor", new Color(0.4f, 0.3f, 0.25f)); // Warm ground
            skyboxMat.SetFloat("_SunSize", 0.04f);
            skyboxMat.SetFloat("_SunSizeConvergence", 5f);
            skyboxMat.SetFloat("_AtmosphereThickness", 1.5f);
            skyboxMat.SetFloat("_Exposure", 1.3f);
            
            RenderSettings.skybox = skyboxMat;
            
            Debug.Log("  ✓ Beautiful skybox created");
        }

        private static void ApplyFog()
        {
            Debug.Log("[Visual Polish] Adding atmospheric fog...");

            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.ExponentialSquared;
            RenderSettings.fogColor = new Color(0.6f, 0.7f, 0.85f); // Soft blue-grey
            RenderSettings.fogDensity = 0.0015f; // Subtle atmospheric haze
            
            Debug.Log("  ✓ Atmospheric fog configured");
        }

        private static void ApplyCamera()
        {
            Debug.Log("[Visual Polish] Enhancing camera...");

            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                // Better camera settings
                mainCam.farClipPlane = 500f;
                mainCam.nearClipPlane = 0.3f;
                mainCam.fieldOfView = 70f; // Slightly wider for more dramatic feel
                
                // Enable HDR if available
                mainCam.allowHDR = true;
                mainCam.allowMSAA = true;
                
                Debug.Log("  ✓ Camera settings optimized");
            }
        }

        private static void AddAtmosphericEffects()
        {
            Debug.Log("[Visual Polish] Adding atmospheric particles...");

            // Find or create effects holder
            GameObject effectsParent = GameObject.Find("=== VISUAL EFFECTS ===");
            if (effectsParent == null)
            {
                effectsParent = new GameObject("=== VISUAL EFFECTS ===");
            }

            // Add atmospheric dust particles
            GameObject dustSystem = new GameObject("AtmosphericDust");
            dustSystem.transform.SetParent(effectsParent.transform);
            dustSystem.transform.position = Vector3.zero;

            ParticleSystem ps = dustSystem.AddComponent<ParticleSystem>();
            var main = ps.main;
            main.startLifetime = 10f;
            main.startSpeed = 0.5f;
            main.startSize = 0.05f;
            main.startColor = new Color(1f, 1f, 1f, 0.1f); // Very subtle
            main.maxParticles = 100;
            main.loop = true;

            var emission = ps.emission;
            emission.rateOverTime = 5f;

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(50f, 20f, 50f);

            var renderer = ps.GetComponent<ParticleSystemRenderer>();
            renderer.renderMode = ParticleSystemRenderMode.Billboard;
            renderer.material = new Material(Shader.Find("Particles/Standard Unlit"));

            Debug.Log("  ✓ Atmospheric particles added");

            // Add subtle light rays if there's a sun
            Light sun = GameObject.Find("Sun Light")?.GetComponent<Light>();
            if (sun != null)
            {
                GameObject lightShafts = new GameObject("LightShafts");
                lightShafts.transform.SetParent(sun.transform);
                lightShafts.transform.localPosition = Vector3.zero;

                ParticleSystem shafts = lightShafts.AddComponent<ParticleSystem>();
                var shaftsMain = shafts.main;
                shaftsMain.startLifetime = 5f;
                shaftsMain.startSpeed = 0f;
                shaftsMain.startSize = 2f;
                shaftsMain.startColor = new Color(1f, 0.95f, 0.8f, 0.05f);
                shaftsMain.maxParticles = 20;

                var shaftsEmission = shafts.emission;
                shaftsEmission.rateOverTime = 2f;

                Debug.Log("  ✓ Light rays added");
            }
        }
    }
}
#endif
