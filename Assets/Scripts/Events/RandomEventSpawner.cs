using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Events
{
    /// <summary>
    /// Spawns random events each loop: fires, accidents, robberies, blackouts, parades.
    /// Events are deterministic based on loop seed.
    /// </summary>
    public class RandomEventSpawner : MonoBehaviour
    {
        [Header("Event Settings")]
        [SerializeField] private List<EventSpawnPoint> eventSpawnPoints = new List<EventSpawnPoint>();
        [SerializeField] private int maxEventsPerLoop = 3;
        [SerializeField] private float eventChance = 0.7f; // 70% chance of event spawning

        [Header("Event Prefabs")]
        [SerializeField] private GameObject firePrefab;
        [SerializeField] private GameObject accidentPrefab;
        [SerializeField] private GameObject robberyPrefab;
        [SerializeField] private GameObject blackoutPrefab;
        [SerializeField] private GameObject paradePrefab;

        private List<GameObject> activeEvents = new List<GameObject>();

        private void Start()
        {
            // Subscribe to loop reset
            if (TimeLoop.TimeLoopManager.Instance != null)
            {
                TimeLoop.TimeLoopManager.Instance.OnLoopReset.AddListener(SpawnRandomEvents);
                TimeLoop.TimeLoopManager.Instance.OnLoopStart.AddListener(SpawnRandomEvents);
            }

            SpawnRandomEvents();
        }

        /// <summary>
        /// Spawn random events based on loop seed
        /// </summary>
        private void SpawnRandomEvents()
        {
            // Clear previous events
            ClearEvents();

            if (eventSpawnPoints.Count == 0)
            {
                Debug.LogWarning("[RandomEventSpawner] No event spawn points configured");
                return;
            }

            // Use deterministic random based on loop
            int loopSeed = TimeLoop.TimeLoopManager.Instance?.GetLoopSeed() ?? 0;
            Random.InitState(loopSeed + 1000); // Offset seed for variety

            int eventCount = Random.Range(1, maxEventsPerLoop + 1);

            for (int i = 0; i < eventCount; i++)
            {
                if (Random.value > eventChance) continue;

                // Pick random spawn point
                EventSpawnPoint spawnPoint = eventSpawnPoints[Random.Range(0, eventSpawnPoints.Count)];

                // Pick random event type
                EventType eventType = (EventType)Random.Range(0, System.Enum.GetValues(typeof(EventType)).Length);

                SpawnEvent(eventType, spawnPoint);
            }

            Debug.Log($"[RandomEventSpawner] Spawned {activeEvents.Count} events for loop {loopSeed}");
        }

        /// <summary>
        /// Spawn specific event at location
        /// </summary>
        private void SpawnEvent(EventType eventType, EventSpawnPoint spawnPoint)
        {
            GameObject prefab = GetEventPrefab(eventType);
            if (prefab == null)
            {
                Debug.LogWarning($"[RandomEventSpawner] No prefab for event type: {eventType}");
                return;
            }

            GameObject eventObj = Instantiate(prefab, spawnPoint.position, Quaternion.identity);
            activeEvents.Add(eventObj);

            Debug.Log($"[RandomEventSpawner] Spawned {eventType} at {spawnPoint.position}");

            // Notify nearby NPCs
            NotifyNearbyNPCs(spawnPoint.position, eventType.ToString());
        }

        /// <summary>
        /// Get event prefab by type
        /// </summary>
        private GameObject GetEventPrefab(EventType type)
        {
            switch (type)
            {
                case EventType.Fire: return firePrefab;
                case EventType.Accident: return accidentPrefab;
                case EventType.Robbery: return robberyPrefab;
                case EventType.Blackout: return blackoutPrefab;
                case EventType.Parade: return paradePrefab;
                default: return null;
            }
        }

        /// <summary>
        /// Notify NPCs within range of the event
        /// </summary>
        private void NotifyNearbyNPCs(Vector3 position, string eventType)
        {
            Collider[] colliders = Physics.OverlapSphere(position, 20f);
            foreach (Collider col in colliders)
            {
                AI.NPCController npc = col.GetComponent<AI.NPCController>();
                if (npc != null)
                {
                    npc.OnEventDetected(eventType);
                }
            }
        }

        /// <summary>
        /// Clear all active events
        /// </summary>
        private void ClearEvents()
        {
            foreach (GameObject eventObj in activeEvents)
            {
                if (eventObj != null)
                {
                    Destroy(eventObj);
                }
            }
            activeEvents.Clear();
        }
    }

    [System.Serializable]
    public class EventSpawnPoint
    {
        public Vector3 position;
        public string locationName;
    }

    public enum EventType
    {
        Fire,
        Accident,
        Robbery,
        Blackout,
        Parade
    }
}
