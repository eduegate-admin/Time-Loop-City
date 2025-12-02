using UnityEngine;
using System.Collections.Generic;
using TimeLoopCity.Managers;

namespace TimeLoopCity.Dialogue
{
    [CreateAssetMenu(fileName = "NewDialogue", menuName = "TimeLoopCity/Dialogue Data")]
    public class DialogueData : ScriptableObject
    {
        [Header("Identity")]
        public string dialogueId;
        public string npcName;

        [Header("Lines")]
        [TextArea(3, 10)]
        public List<string> sentences = new List<string>();

        [Header("Requirements")]
        public int minLoopCount = 0;
        public string requiredClueId;

        [Header("After Dialogue")]
        public string giveClueId;
        public string triggerEventId;
        public MissionData startMission;
        public MissionData completeMission;

        [Header("Choices")]
        public List<DialogueChoice> choices = new List<DialogueChoice>();
    }

    [System.Serializable]
    public struct DialogueChoice
    {
        public string choiceText;
        public DialogueData nextDialogue;
        public string triggerEventId;
    }
}
