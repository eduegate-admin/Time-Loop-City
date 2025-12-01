using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Missions
{
    /// <summary>
    /// Helper component to track specific objectives in the scene.
    /// Attaches to objects that are targets of missions (e.g., "Go to the Park").
    /// </summary>
    public class MissionObjectiveTrigger : MonoBehaviour
    {
        [Header("Objective Settings")]
        [SerializeField] private string targetId;
        [SerializeField] private MissionObjectiveType objectiveType;
        [SerializeField] private bool triggerOnEnter = true;
        [SerializeField] private bool triggerOnInteract = false;

        private void OnTriggerEnter(Collider other)
        {
            if (triggerOnEnter && other.CompareTag("Player"))
            {
                ReportProgress();
            }
        }

        public void ReportProgress()
        {
            if (MissionManager.Instance != null)
            {
                MissionManager.Instance.OnObjectiveEvent(targetId, objectiveType);
                Debug.Log($"[MissionObjectiveTrigger] Reported progress for {targetId}");
            }
        }
    }
}
