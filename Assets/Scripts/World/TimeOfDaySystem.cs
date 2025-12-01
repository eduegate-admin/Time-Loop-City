using UnityEngine;
using UnityEngine.Events;

namespace TimeLoopCity.World
{
    /// <summary>
    /// Controls day-night cycle and exposes events when the hour changes.
    /// </summary>
    public class TimeOfDaySystem : MonoBehaviour
    {
        public static TimeOfDaySystem Instance { get; private set; }

        [Header("Lighting")]
        [SerializeField] private Light directionalLight;
        [SerializeField] private Gradient lightColorGradient;
        [SerializeField] private AnimationCurve lightIntensityCurve;

        [Header("Time Settings")]
        [SerializeField] private float currentHour = 8f;
        [SerializeField] private float timeSpeed = 1f;

        [Header("Skybox")]
        [SerializeField] private Material skyboxMaterial;

        [Header("Events")]
        public UnityEvent<int> OnHourChanged = new UnityEvent<int>();

        private int lastBroadcastHour = -1;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Update()
        {
            if (TimeLoop.TimeLoopManager.Instance != null && !TimeLoop.TimeLoopManager.Instance.IsResetting)
            {
                float loopProgress = TimeLoop.TimeLoopManager.Instance.LoopProgress;
                currentHour = Mathf.Lerp(6f, 20f, loopProgress);
            }
            else
            {
                currentHour += timeSpeed * Time.deltaTime;
                currentHour = Mathf.Repeat(currentHour, 24f);
            }

            UpdateLighting();
            BroadcastHourIfNeeded();
        }

        private void UpdateLighting()
        {
            if (directionalLight == null) return;

            float normalizedTime = currentHour / 24f;
            float sunAngle = (normalizedTime * 360f) - 90f;
            directionalLight.transform.rotation = Quaternion.Euler(sunAngle, 170f, 0f);

            if (lightColorGradient != null)
            {
                directionalLight.color = lightColorGradient.Evaluate(normalizedTime);
            }

            if (lightIntensityCurve != null)
            {
                directionalLight.intensity = lightIntensityCurve.Evaluate(normalizedTime);
                RenderSettings.ambientLight = Color.Lerp(Color.black, Color.white, directionalLight.intensity * 0.5f);
            }

            if (skyboxMaterial != null)
            {
                RenderSettings.skybox = skyboxMaterial;
            }
        }

        public void SetTimeOfDay(float hour)
        {
            currentHour = Mathf.Clamp(hour, 0f, 24f);
            UpdateLighting();
            BroadcastHourIfNeeded();
        }

        public float GetCurrentHour() => currentHour;
        public bool IsNight() => currentHour < 6f || currentHour > 20f;

        private void BroadcastHourIfNeeded()
        {
            int roundedHour = Mathf.FloorToInt(currentHour);
            if (roundedHour == lastBroadcastHour) return;

            lastBroadcastHour = roundedHour;
            OnHourChanged?.Invoke(roundedHour);
        }
    }
}
