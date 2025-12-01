using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "TimeLoopCity/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        public string npcName;
        [TextArea(3, 10)]
        public List<string> sentences;
        
        [Header("Requirements")]
        public int minLoopCount = 0;
        public string requiredClueId;
        
        [Header("After Dialogue")]
        public string giveClueId;
        public string triggerEventId;
        public Missions.MissionData startMission;
        public Missions.MissionData completeMission;

        [Header("Choices")]
        public List<DialogueChoice> choices;
    }

    [System.Serializable]
    public struct DialogueChoice
    {
        public string choiceText;
        public DialogueData nextDialogue;
        public string triggerEventId;
    }
}
