using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.AI
{
    /// <summary>
    /// Defines a schedule entry for an NPC.
    /// </summary>
    [System.Serializable]
    public struct ScheduleEntry
    {
        public float hour; // 0-24
        public string locationName; // Matches a waypoint name
        public string activityName; // e.g., "Work", "Relax"
    }

    /// <summary>
    /// Controls high-level NPC behavior based on time of day.
    /// Overrides basic patrolling when a schedule triggers.
    /// </summary>
    public class NPCBrain : MonoBehaviour
    {
        [Header("Schedule")]
        [SerializeField] private List<ScheduleEntry> dailySchedule = new List<ScheduleEntry>();
        
        private NPCController controller;
        private int currentScheduleIndex = -1;
        private World.TimeOfDaySystem subscribedTimeSystem;

        private void Awake()
        {
            controller = GetComponent<NPCController>();
        }

        private void Start()
        {
            dailySchedule.Sort((a, b) => a.hour.CompareTo(b.hour));
            SubscribeToTimeSystem();
        }

        private void Update()
        {
            if (subscribedTimeSystem == null)
            {
                SubscribeToTimeSystem();
            }
        }

        private void OnDestroy()
        {
            if (subscribedTimeSystem != null)
            {
                subscribedTimeSystem.OnHourChanged.RemoveListener(CheckSchedule);
            }
        }

        private void CheckSchedule(int hour)
        {
            for (int i = 0; i < dailySchedule.Count; i++)
            {
                if (hour >= dailySchedule[i].hour)
                {
                    if (currentScheduleIndex != i)
                    {
                        currentScheduleIndex = i;
                        ExecuteSchedule(dailySchedule[i]);
                    }
                }
            }
        }

        private void ExecuteSchedule(ScheduleEntry entry)
        {
            Debug.Log($"[NPCBrain] {name} starting schedule: {entry.activityName} at {entry.locationName}");
            
            // Find waypoint by name (Simple search for now, could be optimized with a Manager)
            GameObject target = GameObject.Find(entry.locationName);
            if (target != null)
            {
                // Override NPCController's patrol
                // We need to expose a method in NPCController to set a temporary destination or new patrol route
                // For now, we'll assume we can clear waypoints and add this one
                controller.SetSingleDestination(target.transform);
            }
        }

        private void SubscribeToTimeSystem()
        {
            if (subscribedTimeSystem != null) return;

            subscribedTimeSystem = World.TimeOfDaySystem.Instance ?? FindFirstObjectByType<World.TimeOfDaySystem>();
            if (subscribedTimeSystem != null)
            {
                subscribedTimeSystem.OnHourChanged.AddListener(CheckSchedule);
            }
        }
    }
}
