using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TimeLoopCity.World
{
    /// <summary>
    /// Configures lighting for realistic Kochi atmosphere
    /// - Tropical sun (warm, high intensity)
    /// - Humid morning fog
    /// - Ambient occlusion
    /// </summary>
    public class LightingSetup : MonoBehaviour
    {
        [Header("Directional Light (Sun)")]
        public Light sun;
        public Color sunColor = new Color(1f, 0.95f, 0.85f); // Warm tropical sun
        public float sunIntensity = 1.3f;
        [Range(0, 360)] public float sunRotationY = -30f;
        [Range(0, 90)] public float sunRotationX = 55f;

        [Header("Fog Settings")]
        public bool enableFog = true;
        public FogMode fogMode = FogMode.ExponentialSquared;
        public Color fogColor = new Color(0.75f, 0.8f, 0.9f);
        public float fogDensity = 0.005f;

        [Header("Ambient Light")]
        public Color ambientSkyColor = new Color(0.4f, 0.45f, 0.5f);
        public Color ambientEquatorColor = new Color(0.35f, 0.35f, 0.4f);
        public Color ambientGroundColor = new Color(0.2f, 0.22f, 0.25f);

        public void ApplySettings()
        {
            // Sun
            if (sun == null) sun = FindFirstObjectByType<Light>();
            if (sun != null)
            {
                sun.type = LightType.Directional;
                sun.color = sunColor;
                sun.intensity = sunIntensity;
                sun.shadows = LightShadows.Soft;
                sun.transform.rotation = Quaternion.Euler(sunRotationX, sunRotationY, 0);
            }

            // Fog
            RenderSettings.fog = enableFog;
            RenderSettings.fogMode = fogMode;
            RenderSettings.fogColor = fogColor;
            RenderSettings.fogDensity = fogDensity;

            // Ambient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Trilight;
            RenderSettings.ambientSkyColor = ambientSkyColor;
            RenderSettings.ambientEquatorColor = ambientEquatorColor;
            RenderSettings.ambientGroundColor = ambientGroundColor;

            Debug.Log("Kochi Lighting Applied!");
        }

#if UNITY_EDITOR
        [ContextMenu("Apply Lighting Settings")]
        public void ApplyInEditor()
        {
            ApplySettings();
            EditorUtility.SetDirty(gameObject);
        }
#endif
    }
}
