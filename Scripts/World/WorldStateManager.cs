using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.World
{
    /// <summary>
    /// Manages world state randomization each loop.
    /// Controls what changes happen in the city based on loop count.
    /// </summary>
    public class WorldStateManager : MonoBehaviour
    {
        public static WorldStateManager Instance { get; private set; }

        [Header("World State Settings")]
        [SerializeField] private List<WorldStatePreset> statePresets = new List<WorldStatePreset>();
        [SerializeField] private GameObject[] randomizableObjects;

        [Header("Current State")]
        private WorldState currentState;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            // Subscribe to loop events
            if (TimeLoop.TimeLoopManager.Instance != null)
            {
                TimeLoop.TimeLoopManager.Instance.OnLoopReset.AddListener(RandomizeWorldState);
                TimeLoop.TimeLoopManager.Instance.OnLoopStart.AddListener(ApplyWorldState);
            }

            // Initialize first state
            RandomizeWorldState();
            ApplyWorldState();
        }

        /// <summary>
        /// Randomize world state based on current loop
        /// </summary>
        private void RandomizeWorldState()
        {
            int loopSeed = TimeLoop.TimeLoopManager.Instance.GetLoopSeed();
            Random.InitState(loopSeed); // Deterministic random

            currentState = new WorldState
            {
                loopNumber = TimeLoop.TimeLoopManager.Instance.CurrentLoopCount,
                weatherType = (WeatherType)Random.Range(0, System.Enum.GetValues(typeof(WeatherType)).Length),
                timeOfDay = Random.Range(6f, 18f), // 6 AM to 6 PM
                trafficDensity = Random.Range(0.3f, 1f),
                npcActivityLevel = Random.Range(0.5f, 1f)
            };

            Debug.Log($"[WorldStateManager] World state randomized for loop {loopSeed}: Weather={currentState.weatherType}");
        }

        /// <summary>
        /// Apply the current world state to all systems
        /// </summary>
        private void ApplyWorldState()
        {
            // Apply weather
            World.WeatherSystem weatherSystem = FindObjectOfType<World.WeatherSystem>();
            if (weatherSystem != null)
            {
                weatherSystem.SetWeather(currentState.weatherType);
            }

            // Apply time of day
            World.TimeOfDaySystem timeSystem = FindObjectOfType<World.TimeOfDaySystem>();
            if (timeSystem != null)
            {
                timeSystem.SetTimeOfDay(currentState.timeOfDay);
            }

            // Randomize object positions/states
            RandomizeWorldObjects();

            Debug.Log("[WorldStateManager] World state applied");
        }

        /// <summary>
        /// Randomize positions or states of world objects
        /// </summary>
        private void RandomizeWorldObjects()
        {
            foreach (GameObject obj in randomizableObjects)
            {
                if (obj == null) continue;

                // Example: slightly offset position
                Vector3 randomOffset = new Vector3(
                    Random.Range(-2f, 2f),
                    0f,
                    Random.Range(-2f, 2f)
                );
                obj.transform.position += randomOffset;
            }
        }

        /// <summary>
        /// Get current world state
        /// </summary>
        public WorldState GetCurrentState()
        {
            return currentState;
        }
    }

    /// <summary>
    /// Represents the state of the world for a given loop
    /// </summary>
    [System.Serializable]
    public class WorldState
    {
        public int loopNumber;
        public WeatherType weatherType;
        public float timeOfDay; // Hour of day (0-24)
        public float trafficDensity; // 0-1
        public float npcActivityLevel; // 0-1
    }

    /// <summary>
    /// Preset configurations for specific loops
    /// </summary>
    [System.Serializable]
    public class WorldStatePreset
    {
        public int loopNumber;
        public WeatherType weatherType;
        public string description;
    }

    public enum WeatherType
    {
        Sunny,
        Cloudy,
        Rainy,
        Foggy,
        Stormy
    }
}
