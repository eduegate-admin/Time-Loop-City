#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using TimeLoopCity.Dialogue;
using TimeLoopCity.Managers;
using TimeLoopCity.Core;
using System.IO;

namespace TimeLoopCity.Editor
{
    /// <summary>
    /// Generates default dialogue, missions, and clues for the game.
    /// Run from Tools > Time Loop City > Generate Default Content
    /// </summary>
    public class DefaultContentGenerator : EditorWindow
    {
        [MenuItem("Tools/Time Loop City/Generate Default Content")]
        public static void GenerateContent()
        {
            if (!EditorUtility.DisplayDialog("Generate Default Content",
                "This will create default dialogues, missions, and clues.\n\n" +
                "Continue?", "Generate", "Cancel"))
            {
                return;
            }

            GenerateClues();
            GenerateDialogues();
            GenerateMissions();

            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Success", "Default content generated!", "OK");
        }

        private static void GenerateClues()
        {
            string cluesFolder = "Assets/ScriptableObjects/Clues";
            EnsureFolder(cluesFolder);

            // Create individual clues
            CreateClue(cluesFolder, "clue_mysterious_note", "Mysterious Note", 
                "A cryptic note found at the scene. It reads: 'The scientist knows the truth.'");

            CreateClue(cluesFolder, "clue_scientist_location", "Scientist Location",
                "The scientist can be found at the Research Lab in the East District.");

            CreateClue(cluesFolder, "clue_robbery_witness", "Robbery Witness",
                "A witness saw someone fleeing the robbery scene heading north.");

            CreateClue(cluesFolder, "clue_time_loop_proof", "Time Loop Proof",
                "Evidence that the time loop actually exists and is repeating.");

            Debug.Log("[DefaultContentGenerator] Generated clue data");
        }

        private static void GenerateDialogues()
        {
            string dialoguesFolder = "Assets/ScriptableObjects/Dialogues";
            EnsureFolder(dialoguesFolder);

            // Guard initial dialogue
            var guardDialogue = ScriptableObject.CreateInstance<DialogueData>();
            guardDialogue.dialogueId = "dialogue_guard_intro";
            guardDialogue.npcName = "City Guard";
            guardDialogue.sentences.Add("Good morning, citizen.");
            guardDialogue.sentences.Add("Have you heard about the robbery downtown?");
            guardDialogue.sentences.Add("The chief wants everyone to be extra vigilant today.");
            guardDialogue.minLoopCount = 0;
            guardDialogue.giveClueId = "clue_mysterious_note";
            AssetDatabase.CreateAsset(guardDialogue, $"{dialoguesFolder}/Guard_Intro.asset");

            // Reporter dialogue  
            var reporterDialogue = ScriptableObject.CreateInstance<DialogueData>();
            reporterDialogue.dialogueId = "dialogue_reporter_intro";
            reporterDialogue.npcName = "Loop Reporter";
            reporterDialogue.sentences.Add("Another day, another story...");
            reporterDialogue.sentences.Add("Wait, something feels familiar about this.");
            reporterDialogue.sentences.Add("Have we met before?");
            reporterDialogue.minLoopCount = 2;
            AssetDatabase.CreateAsset(reporterDialogue, $"{dialoguesFolder}/Reporter_Intro.asset");

            // Scientist dialogue
            var scientistDialogue = ScriptableObject.CreateInstance<DialogueData>();
            scientistDialogue.dialogueId = "dialogue_scientist_intro";
            scientistDialogue.npcName = "The Scientist";
            scientistDialogue.sentences.Add("You found me...");
            scientistDialogue.sentences.Add("I've discovered something extraordinary.");
            scientistDialogue.sentences.Add("We're stuck in a temporal loop - the day repeats endlessly.");
            scientistDialogue.minLoopCount = 3;
            scientistDialogue.giveClueId = "clue_time_loop_proof";
            AssetDatabase.CreateAsset(scientistDialogue, $"{dialoguesFolder}/Scientist_Intro.asset");

            Debug.Log("[DefaultContentGenerator] Generated dialogues");
        }

        private static void GenerateMissions()
        {
            string missionsFolder = "Assets/ScriptableObjects/Missions";
            EnsureFolder(missionsFolder);

            // Mission 1: Find the Note
            var mission1 = ScriptableObject.CreateInstance<MissionData>();
            mission1.missionId = "mission_find_note";
            mission1.missionName = "Find the Mysterious Note";
            mission1.description = "A note has been discovered at the robbery scene. Find it and examine it.";
            mission1.requiredLoopCount = 0;
            mission1.requiredClueIds.Clear();
            
            var objective1 = new MissionObjective
            {
                description = "Find the mysterious note",
                type = MissionObjectiveType.FindClue,
                targetId = "clue_mysterious_note",
                countRequired = 1
            };
            mission1.objectives.Add(objective1);
            
            AssetDatabase.CreateAsset(mission1, $"{missionsFolder}/Mission_FindNote.asset");

            // Mission 2: Talk to the Scientist
            var mission2 = ScriptableObject.CreateInstance<MissionData>();
            mission2.missionId = "mission_scientist";
            mission2.missionName = "Talk to the Scientist";
            mission2.description = "Find the scientist in the East District and learn about the loop.";
            mission2.requiredLoopCount = 2;
            mission2.requiredClueIds.Add("clue_mysterious_note");
            
            var objective2 = new MissionObjective
            {
                description = "Speak with the scientist",
                type = MissionObjectiveType.TalkToNPC,
                targetId = "npc_scientist",
                countRequired = 1
            };
            mission2.objectives.Add(objective2);
            
            AssetDatabase.CreateAsset(mission2, $"{missionsFolder}/Mission_Scientist.asset");

            // Mission 3: Investigate Robbery
            var mission3 = ScriptableObject.CreateInstance<MissionData>();
            mission3.missionId = "mission_robbery";
            mission3.missionName = "Investigate the Robbery";
            mission3.description = "A crime has occurred. Interview witnesses and find clues about who did it.";
            mission3.requiredLoopCount = 1;
            mission3.requiredClueIds.Clear();
            
            var objective3 = new MissionObjective
            {
                description = "Find witnesses to the robbery",
                type = MissionObjectiveType.TalkToNPC,
                targetId = "npc_city_guard",
                countRequired = 1
            };
            mission3.objectives.Add(objective3);
            
            AssetDatabase.CreateAsset(mission3, $"{missionsFolder}/Mission_Robbery.asset");

            Debug.Log("[DefaultContentGenerator] Generated missions");
        }

        private static void CreateClue(string folder, string clueId, string clueName, string description)
        {
            // Clue data is saved as part of ClueDatabase, not as separate assets
            // This method is placeholder for future clue asset creation
            Debug.Log($"[DefaultContentGenerator] Would create clue: {clueName}");
        }

        private static void EnsureFolder(string folderPath)
        {
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                string[] parts = folderPath.Split('/');
                string currentPath = parts[0];
                
                for (int i = 1; i < parts.Length; i++)
                {
                    string newPath = $"{currentPath}/{parts[i]}";
                    if (!AssetDatabase.IsValidFolder(newPath))
                    {
                        AssetDatabase.CreateFolder(currentPath, parts[i]);
                    }
                    currentPath = newPath;
                }
            }
        }
    }
}
#endif
