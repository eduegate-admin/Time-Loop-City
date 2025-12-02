using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TimeLoopCity.Core;
using TimeLoopCity.Dialogue;
using TimeLoopCity.Player;
using TimeLoopCity.TimeLoop;

namespace TimeLoopCity.AI
{
    /// <summary>
    /// Controls NPC patrol routines, reacts to loop resets,
    /// and routes interaction requests to the dialogue system.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCController : InteractableObject
    {
        [Header("NPC Identity")]
        [SerializeField] private string npcId = "npc_default";
        [SerializeField] private string npcName = "Citizen";

        [Header("Movement")]
        [SerializeField] private float moveSpeed = 3.5f;
        [SerializeField] private float waypointTolerance = 0.5f;
        [SerializeField] private float waypointWaitTime = 2f;
        [SerializeField] private bool randomizeRoutine = true;

        [Header("Waypoints")]
        [SerializeField] private List<Transform> defaultWaypoints = new List<Transform>();
        [SerializeField] private List<Transform> alternateWaypoints = new List<Transform>();

        [Header("Dialogue")]
        [SerializeField] private DialogueData defaultDialogue;
        [SerializeField] private List<DialogueData> loopSpecificDialogues = new List<DialogueData>();

        private readonly List<Transform> overrideWaypoints = new List<Transform>(1);
        private List<Transform> currentWaypoints;
        private NavMeshAgent agent;
        private NPCMemoryComponent memory;
        private int currentWaypointIndex;
        private bool isWaiting;
        private float waitTimer;
        private bool loopEventsSubscribed;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
            memory = GetComponent<NPCMemoryComponent>();

            agent.speed = moveSpeed;
            agent.stoppingDistance = waypointTolerance * 0.5f;
            SortLoopSpecificDialogues();
        }

        private void OnEnable()
        {
            TrySubscribeLoopEvents();
        }

        private void Start()
        {
            InitializeRoutine();
        }

        private void OnDisable()
        {
            TryUnsubscribeLoopEvents();
        }

        private void Update()
        {
            TrySubscribeLoopEvents();

            if (agent == null || currentWaypoints == null || currentWaypoints.Count == 0) return;

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

            if (!agent.pathPending && agent.remainingDistance <= waypointTolerance)
            {
                StartWaiting();
            }
        }

        private void OnLoopStart()
        {
            InitializeRoutine();
        }

        private void OnLoopReset()
        {
            InitializeRoutine();
        }

        private void InitializeRoutine()
        {
            currentWaypoints = SelectWaypointList();
            currentWaypointIndex = 0;
            isWaiting = false;

            if (currentWaypoints.Count > 0)
            {
                agent.Warp(currentWaypoints[0].position);
                MoveToWaypoint(currentWaypointIndex);
            }
        }

        private List<Transform> SelectWaypointList()
        {
            if (!randomizeRoutine || alternateWaypoints.Count == 0)
            {
                return defaultWaypoints;
            }

            int loopSeed = TimeLoopManager.Instance?.GetLoopSeed() ?? 0;
            Random.InitState(loopSeed + npcId.GetHashCode());

            bool useAlternate = Random.value > 0.5f;
            return useAlternate ? alternateWaypoints : defaultWaypoints;
        }

        private void MoveToWaypoint(int index)
        {
            if (currentWaypoints == null || currentWaypoints.Count == 0 || agent == null) return;
            index = Mathf.Clamp(index, 0, currentWaypoints.Count - 1);

            Transform target = currentWaypoints[index];
            if (target == null) return;

            currentWaypointIndex = index;
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(target.position);
        }

        private void MoveToNextWaypoint()
        {
            if (currentWaypoints == null || currentWaypoints.Count == 0) return;

            currentWaypointIndex = (currentWaypointIndex + 1) % currentWaypoints.Count;
            MoveToWaypoint(currentWaypointIndex);
        }

        private void StartWaiting()
        {
            isWaiting = true;
            waitTimer = waypointWaitTime;
            agent.isStopped = true;
        }

        /// <summary>
        /// Override patrol and send the NPC to a single destination.
        /// Used by NPCBrain or scripted events.
        /// </summary>
        public void SetSingleDestination(Transform target)
        {
            if (target == null) return;

            overrideWaypoints.Clear();
            overrideWaypoints.Add(target);
            currentWaypoints = overrideWaypoints;
            currentWaypointIndex = 0;
            MoveToWaypoint(0);

            Debug.Log($"[NPCController] {npcName} moving to override target {target.name}");
        }

        /// <summary>
        /// Called when RandomEventSpawner alerts nearby NPCs.
        /// </summary>
        public void OnEventDetected(string eventType)
        {
            Debug.Log($"[NPCController] {npcName} detected event: {eventType}");

            switch (eventType)
            {
                case "Fire":
                    agent.speed = moveSpeed * 1.5f;
                    break;
                case "Accident":
                    agent.isStopped = true;
                    Invoke(nameof(ResumeMovement), 5f);
                    break;
            }
        }

        private void ResumeMovement()
        {
            agent.isStopped = false;
            agent.speed = moveSpeed;
        }

        public string GetDialogue()
        {
            if (memory != null && memory.RemembersPreviousLoops())
            {
                return memory.GetMemoryDialogue();
            }

            return "Hello there!";
        }

        protected override void OnInteract(PlayerController player)
        {
            base.OnInteract(player);

            if (agent != null)
            {
                agent.isStopped = true;
            }

            if (player != null)
            {
                transform.LookAt(player.transform);
            }

            DialogueData dialogue = GetBestDialogue();
            if (DialogueManager.Instance != null && dialogue != null)
            {
                DialogueManager.Instance.StartDialogue(dialogue);
            }
            else
            {
                Debug.Log($"[NPCController] {npcName} says: {GetDialogue()}");
            }

            Invoke(nameof(ResumeMovement), 5f);
        }

        private DialogueData GetBestDialogue()
        {
            int currentLoop = TimeLoopManager.Instance?.CurrentLoopCount ?? 0;

            foreach (DialogueData data in loopSpecificDialogues)
            {
                if (data == null) continue;
                if (currentLoop < data.minLoopCount) continue;

                if (string.IsNullOrEmpty(data.requiredClueId) ||
                    (PersistentClueSystem.Instance != null && PersistentClueSystem.Instance.HasClue(data.requiredClueId)))
                {
                    return data;
                }
            }

            return defaultDialogue;
        }

        private void SortLoopSpecificDialogues()
        {
            if (loopSpecificDialogues == null) return;
            loopSpecificDialogues.Sort((a, b) =>
            {
                int aLoop = a != null ? a.minLoopCount : int.MinValue;
                int bLoop = b != null ? b.minLoopCount : int.MinValue;
                return bLoop.CompareTo(aLoop);
            });
        }

        private void TrySubscribeLoopEvents()
        {
            if (loopEventsSubscribed || TimeLoopManager.Instance == null) return;

            TimeLoopManager.Instance.OnLoopReset.AddListener(OnLoopReset);
            TimeLoopManager.Instance.OnLoopStart.AddListener(OnLoopStart);
            loopEventsSubscribed = true;
        }

        private void TryUnsubscribeLoopEvents()
        {
            if (!loopEventsSubscribed || TimeLoopManager.Instance == null) return;

            TimeLoopManager.Instance.OnLoopReset.RemoveListener(OnLoopReset);
            TimeLoopManager.Instance.OnLoopStart.RemoveListener(OnLoopStart);
            loopEventsSubscribed = false;
        }

        private void OnDrawGizmosSelected()
        {
            if (defaultWaypoints == null || defaultWaypoints.Count < 2) return;

            Gizmos.color = Color.cyan;
            for (int i = 0; i < defaultWaypoints.Count - 1; i++)
            {
                if (defaultWaypoints[i] == null || defaultWaypoints[i + 1] == null) continue;
                Gizmos.DrawLine(defaultWaypoints[i].position, defaultWaypoints[i + 1].position);
            }
        }
    }
}
