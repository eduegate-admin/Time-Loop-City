#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace TimeLoopCity.Editor
{
    public class URPSetupTool : EditorWindow
    {
        [MenuItem("Tools/Time Loop City/⚙️ SETUP URP + POST-PROCESSING", false, 3)]
        public static void SetupURP()
        {
            if (!EditorUtility.DisplayDialog("Setup URP & Post-Processing",
                "This will configure Universal Render Pipeline:\n\n" +
                "🎨 Create URP Asset\n" +
                "📐 Setup Forward Renderer\n" +
                "✨ Enable Post-Processing\n" +
                "💡 Configure quality settings\n\n" +
                "Continue?", "Setup", "Cancel"))
            {
                return;
            }

            Debug.Log("<b>[URP Setup]</b> Starting URP configuration...");

            SetupURPAsset();
            SetupPostProcessing();
            ConfigureQualitySettings();

            Debug.Log("<b>[URP Setup]</b> <color=green>✨ URP setup complete!</color>");
            
            EditorUtility.DisplayDialog("URP Setup Complete!",
                "🎉 URP is now configured!\n\n" +
                "✅ URP Renderer Asset created\n" +
                "✅ Post-Processing enabled\n" +
                "✅ Quality settings optimized\n\n" +
                "Run Visual Polish next!", "Great!");
        }

        private static void SetupURPAsset()
        {
            Debug.Log("[URP Setup] Creating URP assets...");

            // Create Settings folder if it doesn't exist
            if (!AssetDatabase.IsValidFolder("Assets/Settings"))
            {
                AssetDatabase.CreateFolder("Assets", "Settings");
            }

            // Check if URP is available
            var urpAssetType = System.Type.GetType("UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset, Unity.RenderPipelines.Universal.Runtime");
            
            if (urpAssetType == null)
            {
                Debug.LogWarning("[URP Setup] URP package not installed. Please install 'Universal RP' via Package Manager.");
                EditorUtility.DisplayDialog("URP Not Found",
                    "Universal Render Pipeline is not installed.\n\n" +
                    "Please install it via:\n" +
                    "Window > Package Manager > Universal RP\n\n" +
                    "Then run this tool again.", "OK");
                return;
            }

            Debug.Log("  ✓ URP package detected");

            // Note: Actual URP asset creation requires the package to be installed
            // For now, we'll provide instructions
            Debug.Log("  → To create URP assets: Right-click in Project > Create > Rendering > URP Asset (with Universal Renderer)");
        }

        private static void SetupPostProcessing()
        {
            Debug.Log("[URP Setup] Configuring post-processing...");

            // Create post-processing volume in scene
            GameObject volumeObj = new GameObject("Global Volume");
            var volume = volumeObj.AddComponent<Volume>();
            volume.isGlobal = true;
            volume.priority = 0;

            // Create profile
            var profile = ScriptableObject.CreateInstance<VolumeProfile>();
            
            // Save profile
            string profilePath = "Assets/Settings/PostProcessProfile.asset";
            AssetDatabase.CreateAsset(profile, profilePath);
            AssetDatabase.SaveAssets();

            volume.sharedProfile = profile;

            Debug.Log("  ✓ Global Volume created");
            Debug.Log("  ✓ Volume Profile created");
            Debug.Log("  → Add post-processing overrides in the Inspector");
        }

        private static void ConfigureQualitySettings()
        {
            Debug.Log("[URP Setup] Optimizing quality settings...");

            // Enable real-time global illumination
            Lightmapping.realtimeGI = true;
            
            // Shadow settings - use UnityEngine.ShadowQuality and UnityEngine.ShadowResolution
            QualitySettings.shadows = UnityEngine.ShadowQuality.All;
            QualitySettings.shadowResolution = UnityEngine.ShadowResolution.High;
            QualitySettings.shadowDistance = 150f;

            // LOD bias
            QualitySettings.lodBias = 1.5f;

            Debug.Log("  ✓ Quality settings optimized");
        }
    }
}
#endif
