using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using TimeLoopCity.TimeLoop;
using TimeLoopCity.Dialogue;
using TimeLoopCity.Core;

namespace TimeLoopCity.AI
{
    /// <summary>
    /// Controls NPC behavior with routines that change each loop.
    /// Uses waypoint-based patrol with loop-dependent behavior.
    /// Supports dialogue interactions.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCController : Player.InteractableObject
    {
        [Header("NPC Identity")]
        [SerializeField] private string npcId;
        [SerializeField] private string npcName = "Citizen";

        [Header("Dialogue")]
        [SerializeField] private DialogueData defaultDialogue;
        [SerializeField] private List<DialogueData> loopSpecificDialogues;

        [Header("Movement")]
        [SerializeField] private List<Transform> defaultWaypoints;
        [SerializeField] private List<Transform> alternateWaypoints;
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float waypointWaitTime = 2f;
        [SerializeField] private bool randomizeRoutine = true;

        [Header("Memory")]
        [SerializeField] private NPCMemory memory;

        private NavMeshAgent agent;
        private List<Transform> currentWaypoints;
        private int currentWaypointIndex = 0;
        private bool isWaiting = false;
        private float waitTimer = 0f;
        private bool navMeshAvailable = false;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            
            // Check if NavMesh is available
            navMeshAvailable = CheckNavMeshAvailability();
            
            if (!navMeshAvailable)
            {
                Debug.LogWarning($"[NPCController] {npcName} - NavMesh not available. NPC will not move. " +
                    "Please bake a NavMesh in the Navigation window (Window > AI > Navigation).");
            }
            else
            {
                agent.speed = moveSpeed;
            }
        }

        private void Start()
        {
            // Subscribe to loop events
            if (TimeLoopManager.Instance != null)
            {
                TimeLoopManager.Instance.OnLoopStart.AddListener(OnLoopStart);
                TimeLoopManager.Instance.OnLoopReset.AddListener(OnLoopReset);
            }

            // Initialize routine
            InitializeRoutine();
        }

        private void Update()
        {
            if (!navMeshAvailable || agent == null) return;

            // Handle waiting at waypoints
            if (isWaiting)
            {
                waitTimer -= Time.deltaTime;
                if (waitTimer <= 0f)
                {
                    isWaiting = false;
                    MoveToNextWaypoint();
                }
                return;
            }

            // Check if reached waypoint
            if (!agent.pathPending && agent.isOnNavMesh && agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    StartWaiting();
                }
            }
        }

        /// <summary>
        /// Check if NavMesh is available for this agent
        /// </summary>
        private bool CheckNavMeshAvailability()
        {
            if (agent == null) return false;

            // Try to sample the NavMesh at the current position
            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 2.0f, NavMesh.AllAreas))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Called when loop starts
        /// </summary>
        private void OnLoopStart()
        {
            InitializeRoutine();
        }

        /// <summary>
        /// Initialize NPC routine based on current loop
        /// </summary>
        private void InitializeRoutine()
        {
            if (!navMeshAvailable)
            {
                Debug.LogWarning($"[NPCController] {npcName} - Cannot initialize routine without NavMesh");
                return;
            }

            if (randomizeRoutine)
            {
                // Choose waypoints based on loop count
                int loopSeed = TimeLoopManager.Instance?.GetLoopSeed() ?? 0;
                Random.InitState(loopSeed + npcId.GetHashCode());

                bool useAlternate = Random.value > 0.5f;
                currentWaypoints = useAlternate && alternateWaypoints.Count > 0 ? alternateWaypoints : defaultWaypoints;

                Debug.Log($"[NPCController] {npcName} using {(useAlternate ? "alternate" : "default")} routine for loop {loopSeed}");
            }
            else
            {
                currentWaypoints = defaultWaypoints;
            }

            // Start patrol
            if (currentWaypoints != null && currentWaypoints.Count > 0)
            {
                // Warp to first waypoint if NavMesh is available
                if (agent.isOnNavMesh)
                {
                    agent.Warp(currentWaypoints[0].position);
                }
                else
                {
                    // Try to place on NavMesh
                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(currentWaypoints[0].position, out hit, 5.0f, NavMesh.AllAreas))
                    {
                        agent.Warp(hit.position);
                    }
                }
                
                MoveToWaypoint(0);
            }
        }

        /// <summary>
        /// Reset NPC when loop resets
        /// </summary>
        private void OnLoopReset()
        {
            Debug.Log($"[NPCController] {npcName} resetting for new loop");

            if (!navMeshAvailable) return;

            // Reset position to first waypoint
            if (currentWaypoints != null && currentWaypoints.Count > 0 && agent.isOnNavMesh)
            {
                agent.Warp(currentWaypoints[0].position);
            }

            currentWaypointIndex = 0;
            isWaiting = false;
            waitTimer = 0f;

            // Reinitialize routine for new loop
            InitializeRoutine();
        }

        /// <summary>
        /// Move to specific waypoint
        /// </summary>
        private void MoveToWaypoint(int index)
        {
            if (!navMeshAvailable || currentWaypoints == null || currentWaypoints.Count == 0) return;
            if (!agent.isOnNavMesh) return;

            currentWaypointIndex = index;
            agent.isStopped = false;
            agent.SetDestination(currentWaypoints[currentWaypointIndex].position);
        }

        /// <summary>
        /// Move to next waypoint in sequence
        /// </summary>
        private void MoveToNextWaypoint()
        {
            if (currentWaypoints == null || currentWaypoints.Count == 0) return;
            currentWaypointIndex = (currentWaypointIndex + 1) % currentWaypoints.Count;
            MoveToWaypoint(currentWaypointIndex);
        }

        /// <summary>
        /// Override patrol and go to a specific target (used by NPCBrain)
        /// </summary>
        public void SetSingleDestination(Transform target)
        {
            if (!navMeshAvailable || target == null) return;
            
            // Clear current patrol for now
            currentWaypoints = new List<Transform> { target };
            currentWaypointIndex = 0;
            MoveToWaypoint(0);
            
            Debug.Log($"[NPCController] {npcName} moving to {target.name}");
        }

        /// <summary>
        /// Start waiting at waypoint
        /// </summary>
        private void StartWaiting()
        {
            isWaiting = true;
            waitTimer = waypointWaitTime;
        }

        /// <summary>
        /// Called when NPC detects an event
        /// </summary>
        public void OnEventDetected(string eventType)
        {
            Debug.Log($"[NPCController] {npcName} detected event: {eventType}");

            if (!navMeshAvailable || !agent.isOnNavMesh) return;

            // React to event (can be customized per NPC)
            switch (eventType)
            {
                case "Fire":
                    // Run away
                    agent.speed = moveSpeed * 1.5f;
                    break;
                case "Accident":
                case "Blackout":
                    // Stop and look
                    agent.isStopped = true;
                    Invoke(nameof(ResumeMovement), 5f);
                    break;
            }
        }

        private void ResumeMovement()
        {
            if (!navMeshAvailable || !agent.isOnNavMesh) return;
            
            agent.isStopped = false;
            agent.speed = moveSpeed;
        }

        /// <summary>
        /// Get dialogue based on memory state
        /// </summary>
        public string GetDialogue()
        {
            if (memory != null && memory.RemembersPreviousLoops())
            {
                return memory.GetMemoryDialogue();
            }
            return "Hello there!";
        }

        /// <summary>
        /// Override Interact to start dialogue
        /// </summary>
        protected override void OnInteract(Player.PlayerController player)
        {
            base.OnInteract(player);
            
            // Stop moving while talking
            if (navMeshAvailable && agent != null && agent.isOnNavMesh)
            {
                agent.isStopped = true;
            }
            
            transform.LookAt(player.transform);

            // Select best dialogue
            DialogueData dialogueToPlay = GetBestDialogue();
            
            if (DialogueManager.Instance != null && dialogueToPlay != null)
            {
                DialogueManager.Instance.StartDialogue(dialogueToPlay);
            }
            else
            {
                Debug.Log($"[NPCController] {npcName} says: {GetDialogue()}");
            }

            // Resume movement after delay (simple version)
            // In a real game, DialogueManager would trigger an event when done
            Invoke(nameof(ResumeMovement), 5f); 
        }

        private DialogueData GetBestDialogue()
        {
            int currentLoop = TimeLoopManager.Instance?.CurrentLoopCount ?? 0;
            
            // Check loop specific dialogues first (reverse order to find highest matching requirement)
            if (loopSpecificDialogues != null)
            {
                // Sort by min loop count descending
                loopSpecificDialogues.Sort((a, b) => b.minLoopCount.CompareTo(a.minLoopCount));
                
                foreach (var d in loopSpecificDialogues)
                {
                    if (d != null && currentLoop >= d.minLoopCount)
                    {
                        // Check clue requirement
                        if (string.IsNullOrEmpty(d.requiredClueId) || 
                            (PersistentClueSystem.Instance != null && PersistentClueSystem.Instance.HasClue(d.requiredClueId)))
                        {
                            return d;
                        }
                    }
                }
            }

            return defaultDialogue;
        }

        private void OnDrawGizmos()
        {
            // Draw waypoint path
            if (defaultWaypoints != null && defaultWaypoints.Count > 1)
            {
                Gizmos.color = Color.blue;
                for (int i = 0; i < defaultWaypoints.Count - 1; i++)
                {
                    if (defaultWaypoints[i] != null && defaultWaypoints[i + 1] != null)
                    {
                        Gizmos.DrawLine(defaultWaypoints[i].position, defaultWaypoints[i + 1].position);
                    }
                }
            }
        }
    }
}
