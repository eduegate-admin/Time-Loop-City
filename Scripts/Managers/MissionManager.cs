using UnityEngine;
using System.Collections.Generic;
using TimeLoopCity.TimeLoop;
using TimeLoopCity.Core;
using TimeLoopCity.UI;

namespace TimeLoopCity.Missions
{
    /// <summary>
    /// Manages active missions, objectives, and rewards.
    /// Integrates with TimeLoopManager to track progress across loops.
    /// </summary>
    public class MissionManager : MonoBehaviour
    {
        public static MissionManager Instance { get; private set; }

        [Header("Mission Database")]
        [SerializeField] private List<MissionData> allMissions = new List<MissionData>();

        [Header("Current State")]
        [SerializeField] private List<MissionData> activeMissions = new List<MissionData>();
        [SerializeField] private List<MissionData> completedMissions = new List<MissionData>();

        public List<MissionData> ActiveMissions => activeMissions;
        public List<MissionData> CompletedMissions => completedMissions;

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
            // Check for mission triggers on start
            CheckMissionPrerequisites();
            
            // Subscribe to events
            // Subscribe to events
            if (TimeLoopManager.Instance != null)
            {
                TimeLoopManager.Instance.OnLoopStart.AddListener(OnLoopStart);
            }
        }

        private void OnLoopStart()
        {
            // Some missions might fail or reset on loop start
            CheckMissionPrerequisites();
        }

        /// <summary>
        /// Check if any new missions should be started
        /// </summary>
        public void CheckMissionPrerequisites()
        {
            foreach (var mission in allMissions)
            {
                if (activeMissions.Contains(mission) || completedMissions.Contains(mission)) continue;

                if (CanStartMission(mission))
                {
                    StartMission(mission);
                }
            }
        }

        private bool CanStartMission(MissionData mission)
        {
            // Check loop count requirement
            // Check loop count requirement
            if (TimeLoopManager.Instance != null)
            {
                if (TimeLoopManager.Instance.CurrentLoopCount < mission.requiredLoopCount) return false;
            }

            // Check clue requirements
            // Check clue requirements
            if (PersistentClueSystem.Instance != null)
            {
                foreach (string clueId in mission.requiredClueIds)
                {
                    if (!PersistentClueSystem.Instance.HasClue(clueId)) return false;
                }
            }

            return true;
        }

        public void StartMission(MissionData mission)
        {
            activeMissions.Add(mission);
            Debug.Log($"[MissionManager] Started Mission: {mission.missionName}");
            // Show UI notification
            NotificationUI.Instance?.ShowNotification("New Mission", mission.missionName);
        }

        public void CompleteMission(MissionData mission)
        {
            if (activeMissions.Contains(mission))
            {
                activeMissions.Remove(mission);
                completedMissions.Add(mission);
                Debug.Log($"[MissionManager] Completed Mission: {mission.missionName}");
                
                NotificationUI.Instance?.ShowNotification("Mission Complete", mission.missionName);

                // Grant rewards
                GrantRewards(mission);
            }
        }

        private void GrantRewards(MissionData mission)
        {
            foreach (string clueId in mission.rewardClueIds)
            {
            foreach (string clueId in mission.rewardClueIds)
            {
                PersistentClueSystem.Instance?.DiscoverClue(clueId);
            }
            }
        }

        /// <summary>
        /// Called by external systems when an objective might be completed
        /// </summary>
        public void OnObjectiveEvent(string targetId, MissionObjectiveType type)
        {
            foreach (var mission in activeMissions)
            {
                // Check objectives (simplified for this starter)
                // In a full system, we'd track individual objective state
                Debug.Log($"[MissionManager] Checking objective {targetId} for mission {mission.missionName}");
            }
        }
    }

    public enum MissionObjectiveType
    {
        FindClue,
        TalkToNPC,
        VisitLocation
    }
}
