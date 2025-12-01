using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;
using TimeLoopCity.Managers;

namespace TimeLoopCity.TimeLoop
{
    /// <summary>
    /// Central manager for time loop mechanics.
    /// Handles time progression, loop resets, and persistent data across loops.
    /// </summary>
    public class TimeLoopManager : MonoBehaviour
    {
        public static TimeLoopManager Instance { get; private set; }

        [Header("Loop Settings")]
        [SerializeField] private float loopDurationSeconds = 300f; // 5 minutes per loop
        [SerializeField] private bool autoResetOnComplete = true;
        [SerializeField] private Transform playerSpawnPoint;

        [Header("State")]
        [SerializeField] private int currentLoopCount = 0;
        [SerializeField] private float currentLoopTime = 0f;
        [SerializeField] private bool isResetting = false;

        [Header("Events")]
        public UnityEvent OnLoopStart = new UnityEvent();
        public UnityEvent OnLoopReset = new UnityEvent();
        public UnityEvent<int> OnLoopCountChanged = new UnityEvent<int>();
        public UnityEvent<float> OnLoopTimeUpdated = new UnityEvent<float>();

        // Persistent data across loops
        private HashSet<string> discoveredClues = new HashSet<string>();

        public int CurrentLoopCount => currentLoopCount;
        public float CurrentLoopTime => currentLoopTime;
        public float LoopProgress => currentLoopTime / loopDurationSeconds;
        public bool IsResetting => isResetting;

        private void Awake()
        {
            // Singleton pattern
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            
            // Ensure this GameObject is a root object before DontDestroyOnLoad
            if (transform.parent != null)
            {
                Debug.LogWarning("[TimeLoopManager] DontDestroyOnLoad only works on root GameObjects. " +
                    "Please move TimeLoopManager to the scene root (not as a child of another GameObject).");
                transform.SetParent(null);
            }
            
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            StartLoop();
        }

        private void Update()
        {
            if (isResetting) return;

            currentLoopTime += Time.deltaTime;
            OnLoopTimeUpdated?.Invoke(currentLoopTime);

            // Check if loop time is complete
            if (currentLoopTime >= loopDurationSeconds && autoResetOnComplete)
            {
                ResetLoop();
            }
        }

        /// <summary>
        /// Starts a new loop (called on game start or after reset)
        /// </summary>
        private void StartLoop()
        {
            currentLoopTime = 0f;
            isResetting = false;
            OnLoopStart?.Invoke();
            Debug.Log($"[TimeLoopManager] Loop {currentLoopCount} started");
        }

        /// <summary>
        /// Resets the loop - triggers all reset behaviors
        /// </summary>
        public void ResetLoop()
        {
            if (isResetting) return;
            
            isResetting = true;
            Debug.Log($"[TimeLoopManager] Resetting loop {currentLoopCount}...");

            // Increment loop count
            currentLoopCount++;
            OnLoopCountChanged?.Invoke(currentLoopCount);

            // Trigger reset event (listened to by WorldStateManager, NPCs, etc.)
            OnLoopReset?.Invoke();

            // Teleport player back to spawn
            TeleportPlayerToSpawn();

            // Wait for transition, then restart loop
            Invoke(nameof(StartLoop), 1f); // 1 second delay for transition
        }

        /// <summary>
        /// Manually trigger loop reset (e.g., player death or choice)
        /// </summary>
        public void ForceResetLoop()
        {
            ResetLoop();
        }

        /// <summary>
        /// Stops the loop permanently (e.g., for End Game)
        /// </summary>
        public void StopLoop()
        {
            autoResetOnComplete = false;
            // Optionally disable update logic or set time scale to 0 if we want to freeze time
            // For now, we just prevent the reset.
            Debug.Log("[TimeLoopManager] Loop Stopped. The cycle is broken.");
        }

        /// <summary>
        /// Teleport player back to spawn point
        /// </summary>
        private void TeleportPlayerToSpawn()
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null && playerSpawnPoint != null)
            {
                player.transform.position = playerSpawnPoint.position;
                player.transform.rotation = playerSpawnPoint.rotation;
                Debug.Log("[TimeLoopManager] Player teleported to spawn");
            }
        }

        /// <summary>
        /// Add a discovered clue (persists across loops)
        /// </summary>
        public void AddClue(string clueId)
        {
            if (discoveredClues.Add(clueId))
            {
                Debug.Log($"[TimeLoopManager] Clue discovered: {clueId}");
                // Save immediately
                SaveLoadManager.Instance?.SaveClues(discoveredClues);
            }
        }

        /// <summary>
        /// Check if a clue has been discovered
        /// </summary>
        public bool HasClue(string clueId)
        {
            return discoveredClues.Contains(clueId);
        }

        /// <summary>
        /// Load clues from save system
        /// </summary>
        public void LoadClues(HashSet<string> clues)
        {
            discoveredClues = clues;
        }

        /// <summary>
        /// Get all discovered clues
        /// </summary>
        public HashSet<string> GetAllClues()
        {
            return new HashSet<string>(discoveredClues);
        }

        /// <summary>
        /// Get deterministic random seed based on loop count
        /// Used by WorldStateManager for consistent randomization
        /// </summary>
        public int GetLoopSeed()
        {
            return currentLoopCount;
        }
    }
}
