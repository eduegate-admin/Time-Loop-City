using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Missions
{
    [CreateAssetMenu(fileName = "NewMission", menuName = "TimeLoopCity/Mission Data")]
    public class MissionData : ScriptableObject
    {
        [Header("Mission Info")]
        public string missionId;
        public string missionName;
        [TextArea] public string description;

        [Header("Requirements")]
        public int requiredLoopCount = 0;
        public List<string> requiredClueIds = new List<string>();

        [Header("Rewards")]
        public List<string> rewardClueIds = new List<string>();

        [Header("Objectives")]
        public List<MissionObjective> objectives = new List<MissionObjective>();
    }

    [System.Serializable]
    public class MissionObjective
    {
        public string description;
        public MissionObjectiveType type;
        public string targetId;
        public int countRequired = 1;
    }
}
