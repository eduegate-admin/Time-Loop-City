using UnityEngine;
using UnityEngine.Rendering;
// using UnityEngine.Rendering.Universal; // Assuming URP, but keeping generic for compatibility

namespace TimeLoopCity.Visuals
{
    /// <summary>
    /// Controls visual atmosphere based on loop progression.
    /// Changes lighting, fog, and post-processing over time.
    /// </summary>
    public class PostProcessingController : MonoBehaviour
    {
        [Header("Atmosphere Settings")]
        [SerializeField] private Gradient dayNightFogColor;
        [SerializeField] private AnimationCurve fogDensityCurve;
        [SerializeField] private Gradient lightColor;
        [SerializeField] private AnimationCurve lightIntensityCurve;

        [Header("Loop Effects")]
        [SerializeField] private float glitchIntensity = 0f;
        [SerializeField] private Volume globalVolume; // Reference to Post-Process Volume

        private Light sunLight;

        private void Start()
        {
            sunLight = RenderSettings.sun;
            
            if (TimeLoop.TimeLoopManager.Instance != null)
            {
                TimeLoop.TimeLoopManager.Instance.OnLoopTimeUpdated.AddListener(UpdateVisuals);
                TimeLoop.TimeLoopManager.Instance.OnLoopReset.AddListener(OnLoopReset);
            }
        }

        private void UpdateVisuals(float time)
        {
            if (TimeLoop.TimeLoopManager.Instance == null) return;

            float progress = TimeLoop.TimeLoopManager.Instance.LoopProgress;

            // Update Fog
            RenderSettings.fogColor = dayNightFogColor.Evaluate(progress);
            RenderSettings.fogDensity = fogDensityCurve.Evaluate(progress);

            // Update Sun
            if (sunLight != null)
            {
                sunLight.color = lightColor.Evaluate(progress);
                sunLight.intensity = lightIntensityCurve.Evaluate(progress);
                
                // Rotate sun
                float angle = Mathf.Lerp(-10f, 170f, progress);
                sunLight.transform.rotation = Quaternion.Euler(angle, 30f, 0f);
            }

            // Increase "glitch" or distortion as loop nears end
            if (progress > 0.8f)
            {
                float instability = (progress - 0.8f) * 5f; // 0 to 1
                SetGlitchEffect(instability);
                
                // Occasional shake
                if (Random.value < 0.01f * instability)
                {
                    CameraShake.Instance?.Shake(0.2f, 0.1f * instability);
                }
            }
        }

        private void SetGlitchEffect(float intensity)
        {
            // In a real URP/HDRP project, we would access the VolumeProfile here.
            // Example:
            // if (globalVolume.profile.TryGet(out ChromaticAberration ca)) ca.intensity.value = intensity;
            // if (globalVolume.profile.TryGet(out LensDistortion ld)) ld.intensity.value = intensity * -0.5f;
            
            // For this script to compile without pipeline dependencies, we will just log 
            // or modify standard camera settings if possible.
            
            // Simulating glitch by shaking camera slightly (if we had reference)
            // or just changing FOV slightly
            if (Camera.main != null)
            {
                Camera.main.fieldOfView = 60f + (Mathf.Sin(Time.time * 50f) * intensity * 2f);
            }
            
            glitchIntensity = intensity;
        }

        private void OnLoopReset()
        {
            SetGlitchEffect(0f);
        }
    }
}
