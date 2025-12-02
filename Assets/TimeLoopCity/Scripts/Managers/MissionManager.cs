using UnityEngine;
using System.Collections.Generic;
using TimeLoopCity.Core;
using TimeLoopCity.TimeLoop;
using TimeLoopCity.UI;

namespace TimeLoopCity.Managers
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

        public IReadOnlyList<MissionData> ActiveMissions => activeMissions;
        public IReadOnlyList<MissionData> CompletedMissions => completedMissions;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnEnable()
        {
            if (TimeLoopManager.Instance != null)
            {
                TimeLoopManager.Instance.OnLoopStart.AddListener(OnLoopStart);
            }
        }

        private void OnDisable()
        {
            if (TimeLoopManager.Instance != null)
            {
                TimeLoopManager.Instance.OnLoopStart.RemoveListener(OnLoopStart);
            }
        }

        private void Start()
        {
            CheckMissionPrerequisites();
        }

        private void OnLoopStart()
        {
            CheckMissionPrerequisites();
        }

        /// <summary>
        /// Check if any new missions should be started.
        /// </summary>
        public void CheckMissionPrerequisites()
        {
            foreach (var mission in allMissions)
            {
                if (mission == null) continue;
                if (activeMissions.Contains(mission) || completedMissions.Contains(mission)) continue;

                if (CanStartMission(mission))
                {
                    StartMission(mission);
                }
            }
        }

        private bool CanStartMission(MissionData mission)
        {
            if (TimeLoopManager.Instance != null && TimeLoopManager.Instance.CurrentLoopCount < mission.requiredLoopCount)
            {
                return false;
            }

            if (PersistentClueSystem.Instance != null)
            {
                foreach (string clueId in mission.requiredClueIds)
                {
                    if (!PersistentClueSystem.Instance.HasClue(clueId))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void StartMission(MissionData mission)
        {
            if (mission == null || activeMissions.Contains(mission)) return;

            activeMissions.Add(mission);
            Debug.Log($"[MissionManager] Started Mission: {mission.missionName}");
            NotificationUI.Instance?.ShowNotification("New Mission", mission.missionName);
        }

        public void CompleteMission(MissionData mission)
        {
            if (mission == null || !activeMissions.Contains(mission)) return;

            activeMissions.Remove(mission);
            if (!completedMissions.Contains(mission))
            {
                completedMissions.Add(mission);
            }

            Debug.Log($"[MissionManager] Completed Mission: {mission.missionName}");
            NotificationUI.Instance?.ShowNotification("Mission Complete", mission.missionName);
            GrantRewards(mission);
        }

        private void GrantRewards(MissionData mission)
        {
            if (mission == null || PersistentClueSystem.Instance == null) return;

            foreach (string clueId in mission.rewardClueIds)
            {
                if (string.IsNullOrEmpty(clueId)) continue;
                PersistentClueSystem.Instance.DiscoverClue(clueId);
            }
        }

        /// <summary>
        /// Called by external systems when an objective might be completed.
        /// </summary>
        public void OnObjectiveEvent(string targetId, MissionObjectiveType type)
        {
            foreach (var mission in activeMissions)
            {
                if (mission == null) continue;
                Debug.Log($"[MissionManager] Checking objective {targetId} ({type}) for mission {mission.missionName}");
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
