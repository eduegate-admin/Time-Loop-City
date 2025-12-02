using UnityEngine;

namespace TimeLoopCity.UI
{
    /// <summary>
    /// Simple UI to show active missions and objectives.
    /// </summary>
    public class UI_MissionDisplay : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI missionText;
        
        private void Start()
        {
            UpdateDisplay();
            // Subscribe to updates if MissionManager has events (TODO: Add events to MissionManager)
        }

        private void Update()
        {
            // For prototype, just update periodically or on event
            // Here we'll just pull data for simplicity in this step
            UpdateDisplay();
        }

        private void UpdateDisplay()
        {
            if (Managers.MissionManager.Instance == null || missionText == null) return;

            var missions = Managers.MissionManager.Instance.ActiveMissions;
            if (missions == null || missions.Count == 0)
            {
                missionText.text = "No Active Missions";
                return;
            }

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine("<b>Current Missions:</b>");
            foreach (var mission in missions)
            {
                sb.AppendLine($"- {mission.missionName}");
                foreach (var obj in mission.objectives)
                {
                    sb.AppendLine($"  o {obj.description}");
                }
            }
            missionText.text = sb.ToString();
        }
    }
}
