using UnityEngine;

namespace TimeLoopCity.World
{
    /// <summary>
    /// Controls day-night cycle and time progression within a loop.
    /// </summary>
    public class TimeOfDaySystem : MonoBehaviour
    {
        [Header("Lighting")]
        [SerializeField] private Light directionalLight;
        [SerializeField] private Gradient lightColorGradient;
        [SerializeField] private AnimationCurve lightIntensityCurve;

        [Header("Time Settings")]
        [SerializeField] private float currentHour = 8f; // Start at 8 AM
        [SerializeField] private float timeSpeed = 1f; // Real-time multiplier

        [Header("Skybox")]
        [SerializeField] private Material skyboxMaterial;

        private void Update()
        {
            // Progress time based on loop time
            if (TimeLoop.TimeLoopManager.Instance != null && !TimeLoop.TimeLoopManager.Instance.IsResetting)
            {
                float loopProgress = TimeLoop.TimeLoopManager.Instance.LoopProgress;
                currentHour = Mathf.Lerp(6f, 20f, loopProgress); // 6 AM to 8 PM over loop
            }

            UpdateLighting();
        }

        /// <summary>
        /// Update lighting based on time of day
        /// </summary>
        private void UpdateLighting()
        {
            if (directionalLight == null) return;

            // Calculate normalized time (0 = midnight, 0.5 = noon, 1 = midnight)
            float normalizedTime = currentHour / 24f;

            // Rotate sun
            float sunAngle = (normalizedTime * 360f) - 90f;
            directionalLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

            // Update light color and intensity
            if (lightColorGradient != null)
            {
                directionalLight.color = lightColorGradient.Evaluate(normalizedTime);
            }

            if (lightIntensityCurve != null)
            {
                directionalLight.intensity = lightIntensityCurve.Evaluate(normalizedTime);
            }

            // Update ambient light
            RenderSettings.ambientLight = Color.Lerp(Color.black, Color.white, lightIntensityCurve.Evaluate(normalizedTime) * 0.5f);
        }

        /// <summary>
        /// Set time of day (called by WorldStateManager)
        /// </summary>
        public void SetTimeOfDay(float hour)
        {
            currentHour = Mathf.Clamp(hour, 0f, 24f);
            UpdateLighting();
        }

        public float GetCurrentHour() => currentHour;
        public bool IsNight() => currentHour < 6f || currentHour > 20f;
    }
}
