using UnityEngine;

namespace TimeLoopCity.World
{
    /// <summary>
    /// Controls weather effects and transitions.
    /// Weather changes each loop based on WorldStateManager.
    /// </summary>
    public class WeatherSystem : MonoBehaviour
    {
        [Header("Weather Effects")]
        [SerializeField] private ParticleSystem rainEffect;
        [SerializeField] private ParticleSystem fogEffect;
        [SerializeField] private ParticleSystem snowEffect;

        [Header("Settings")]
        [SerializeField] private WeatherType currentWeather = WeatherType.Sunny;
        [SerializeField] private float transitionSpeed = 1f;

        private void Start()
        {
            // Initialize with current weather
            ApplyWeather(currentWeather, instant: true);
        }

        /// <summary>
        /// Set weather type (called by WorldStateManager)
        /// </summary>
        public void SetWeather(WeatherType weatherType)
        {
            if (currentWeather == weatherType) return;

            Debug.Log($"[WeatherSystem] Changing weather to {weatherType}");
            currentWeather = weatherType;
            ApplyWeather(weatherType, instant: false);
        }

        /// <summary>
        /// Apply weather effects
        /// </summary>
        private void ApplyWeather(WeatherType weather, bool instant)
        {
            // Disable all effects first
            if (rainEffect != null) rainEffect.Stop();
            if (fogEffect != null) fogEffect.Stop();
            if (snowEffect != null) snowEffect.Stop();

            // Enable specific weather effect
            float targetFog = 0f;

            switch (weather)
            {
                case WeatherType.Rainy:
                case WeatherType.Stormy:
                    if (rainEffect != null)
                    {
                        rainEffect.Play();
                        var emission = rainEffect.emission;
                        emission.rateOverTime = weather == WeatherType.Stormy ? 500 : 200;
                    }
                    targetFog = 0.01f;
                    break;

                case WeatherType.Foggy:
                    if (fogEffect != null) fogEffect.Play();
                    targetFog = 0.05f;
                    break;

                case WeatherType.Cloudy:
                    targetFog = 0.005f;
                    // Reduce light intensity
                    Light sun = FindFirstObjectByType<Light>();
                    if (sun != null) sun.intensity *= 0.7f;
                    break;

                case WeatherType.Sunny:
                default:
                    targetFog = 0f;
                    break;
            }

            float lerpFactor = instant ? 1f : Mathf.Clamp01(Time.deltaTime * transitionSpeed);
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, targetFog, lerpFactor);

            // Enable fog rendering
            RenderSettings.fog = weather != WeatherType.Sunny;
        }

        public WeatherType GetCurrentWeather() => currentWeather;
    }
}
