using UnityEngine;
using System.Collections.Generic;
using TimeLoopCity.Managers;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TimeLoopCity.Content
{
    /// <summary>
    /// Auto-generates the "Golden Path" content (Missions, Dialogues, Clues).
    /// Run this from the context menu to populate the project with data.
    /// </summary>
    public class ContentGenerator : MonoBehaviour
    {
#if UNITY_EDITOR
        [ContextMenu("Generate Golden Path Content")]
        public void GenerateContent()
        {
            EnsureDirectories();
            GenerateClues();
            GenerateMissions();
            GenerateDialogues();
            Debug.Log("[ContentGenerator] Golden Path Content Generated!");
        }

        private void EnsureDirectories()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources")) AssetDatabase.CreateFolder("Assets", "Resources");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Missions")) AssetDatabase.CreateFolder("Assets/Resources", "Missions");
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Dialogues")) AssetDatabase.CreateFolder("Assets/Resources", "Dialogues");
        }

        private void GenerateClues()
        {
            // In a real system, Clues might be their own ScriptableObject. 
            // For now, they are string IDs managed by PersistentClueSystem.
            // We just define them here for reference/usage in Missions.
            // "Clue_Newspaper", "Clue_Receipt", "Clue_Memo", "Clue_Keycard"
            // "Clue_TheTruth" - The final piece of evidence.
        }

        private void GenerateMissions()
        {
            // Mission 1: Survive the Loop
            CreateMission("Mission_Survive", "Survive the Loop", "Witness the end of the world.", 
                0, new string[] { }, new string[] { });

            // Mission 2: The Anomaly
            CreateMission("Mission_Anomaly", "The Anomaly", "Find the Scientist's warning.", 
                1, new string[] { "Clue_Newspaper" }, new string[] { });

            // Mission 3: Access Granted
            CreateMission("Mission_Access", "Access Granted", "Find the Lab Keycard.", 
                2, new string[] { "Clue_Keycard" }, new string[] { });
        }

        private void GenerateDialogues()
        {
            CreateDialogue("Dialogue_Scientist_Intro", "Dr. Chronos", 
                new[] { "You... you're awake?", "The loop... it's destabilizing.", "Find my keycard. Save us." },
                0, null, "Mission_Access");
            
            CreateDialogue("Dialogue_Barista_Default", "Barista", 
                new[] { "Same order as always?", "You look like you've seen a ghost." },
                0, null, null);

            CreateDialogue("Dialogue_Oracle_End", "Mysterious Figure",
                new[] { "You have found the Truth.", "The cycle ends now." },
                0, "Clue_TheTruth", null);
        }

        private void CreateMission(string id, string title, string desc, int loopReq, string[] reqClues, string[] rewardClues)
        {
            Managers.MissionData mission = ScriptableObject.CreateInstance<Managers.MissionData>();
            mission.missionId = id;
            mission.missionName = title;
            mission.description = desc;
            mission.requiredLoopCount = loopReq;
            mission.requiredClueIds = new List<string>(reqClues);
            mission.rewardClueIds = new List<string>(rewardClues);

            string path = $"Assets/Resources/Missions/{id}.asset";
            AssetDatabase.CreateAsset(mission, path);
        }

        private void CreateDialogue(string id, string npcName, string[] sentences, int minLoop, string reqClue, string startMissionId)
        {
            Dialogue.DialogueData dialogue = ScriptableObject.CreateInstance<Dialogue.DialogueData>();
            dialogue.dialogueId = id;
            dialogue.npcName = npcName;
            dialogue.sentences.Clear();
            if (sentences != null)
            {
                dialogue.sentences.AddRange(sentences);
            }
            dialogue.minLoopCount = minLoop;
            dialogue.requiredClueId = reqClue;
            
            // Link mission if provided
            if (!string.IsNullOrEmpty(startMissionId))
            {
                dialogue.startMission = AssetDatabase.LoadAssetAtPath<Managers.MissionData>($"Assets/Resources/Missions/{startMissionId}.asset");
            }

            string path = $"Assets/Resources/Dialogues/{id}.asset";
            AssetDatabase.CreateAsset(dialogue, path);
        }
#endif
    }
}
