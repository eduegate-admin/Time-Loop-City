using UnityEngine;
using UnityEditor;

namespace TimeLoopCity.Editor.KochiSuite
{
    /// <summary>
    /// Gameplay missions generator - Fort Kochi missions, clues, time-loop integration, NPC spawns
    /// </summary>
    public class GameplayMissionsGenerator : KochiGeneratorBase
    {
        private int missionCount = 10;
        private int clueCount = 50;
        private int npcSpawnerCount = 15;
        private bool setupTimeLoopIntegration = true;

        public override void DrawGUI()
        {
            EditorGUILayout.LabelField("Gameplay & Missions Settings", EditorStyles.boldLabel);
            missionCount = EditorGUILayout.IntSlider("Fort Kochi Missions", missionCount, 1, 20);
            clueCount = EditorGUILayout.IntSlider("Clue Spawns", clueCount, 5, 100);
            npcSpawnerCount = EditorGUILayout.IntSlider("NPC Spawners", npcSpawnerCount, 1, 50);
            setupTimeLoopIntegration = EditorGUILayout.Toggle("Setup Time Loop Integration", setupTimeLoopIntegration);

            EditorGUILayout.HelpBox(
                "• Missions: Quest chains in Fort Kochi\n" +
                "• Clues: Investigation items and hints\n" +
                "• NPCs: Character spawning points\n" +
                "• Time Loop: Integration with loop manager",
                MessageType.Info);
        }

        public void GenerateGameplay()
        {
            isRunning = true;
            progress = 0f;

            try
            {
                GenerateFortKochiMissions();
                SetProgress(0.25f);

                GenerateClueSpawns();
                SetProgress(0.5f);

                GenerateNPCSpawns();
                SetProgress(0.75f);

                if (setupTimeLoopIntegration)
                {
                    SetupTimeLoopIntegration();
                    SetProgress(0.95f);
                }

                LogSuccess("Gameplay generation completed");
                SetProgress(1f);
            }
            catch (System.Exception e)
            {
                LogError($"Gameplay generation failed: {e.Message}");
            }
            finally
            {
                isRunning = false;
            }
        }

        private void GenerateFortKochiMissions()
        {
            EnsureFolder("Assets/TimeLoopKochi/Gameplay/Missions");

            GameObject gameplayRoot = FindOrCreateRoot("Gameplay");
            GameObject missionsRoot = new GameObject("Missions");
            missionsRoot.transform.parent = gameplayRoot.transform;

            string[] missionNames = new string[]
            {
                "TheAntiqueDealer",
                "FishingMystery",
                "PalaceIntrigue",
                "SpiceTraderSecret",
                "BackwaterRiddle",
                "CulturalHeritage",
                "TimeTravelerTrace",
                "VeniceOfEast",
                "HiddenManuscript",
                "EternalCycle"
            };

            for (int i = 0; i < missionCount; i++)
            {
                GameObject mission = new GameObject($"Mission_{(i % missionNames.Length < missionNames.Length ? missionNames[i % missionNames.Length] : i.ToString())}");
                mission.transform.parent = missionsRoot.transform;
                mission.transform.position = new Vector3(
                    Random.Range(-150f, 150f),
                    0f,
                    Random.Range(-150f, 150f)
                );

                var missionMarker = mission.AddComponent<SphereCollider>();
                missionMarker.radius = 5f;
                missionMarker.isTrigger = true;

                EnsureTag("MissionMarker");
                mission.tag = "MissionMarker";
            }

            LogSuccess($"Generated {missionCount} Fort Kochi missions");
        }

        private void GenerateClueSpawns()
        {
            EnsureFolder("Assets/TimeLoopKochi/Gameplay/Clues");

            GameObject gameplayRoot = FindOrCreateRoot("Gameplay");
            GameObject cluesRoot = new GameObject("Clues");
            cluesRoot.transform.parent = gameplayRoot.transform;

            for (int i = 0; i < clueCount; i++)
            {
                GameObject clue = new GameObject($"Clue_{i}");
                clue.transform.parent = cluesRoot.transform;
                clue.transform.position = new Vector3(
                    Random.Range(-200f, 200f),
                    0.5f,
                    Random.Range(-200f, 200f)
                );

                var clueGeo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                clueGeo.name = "Geometry";
                clueGeo.transform.parent = clue.transform;
                clueGeo.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                clueGeo.transform.localPosition = Vector3.zero;

                Object.DestroyImmediate(clueGeo.GetComponent<Collider>());

                var renderer = clueGeo.GetComponent<Renderer>();
                var clueMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                clueMat.color = new Color(1f, 0.8f, 0f);
                renderer.material = clueMat;

                var clueTrigger = clue.AddComponent<SphereCollider>();
                clueTrigger.radius = 2f;
                clueTrigger.isTrigger = true;

                EnsureTag("Clue");
                clue.tag = "Clue";
            }

            LogSuccess($"Generated {clueCount} clue spawns");
        }

        private void GenerateNPCSpawns()
        {
            EnsureFolder("Assets/TimeLoopKochi/Gameplay/NPCs");

            GameObject gameplayRoot = FindOrCreateRoot("Gameplay");
            GameObject npcsRoot = new GameObject("NPCSpawners");
            npcsRoot.transform.parent = gameplayRoot.transform;

            string[] npcTypes = new string[]
            {
                "Merchant", "Scholar", "Tourist", "LocalGuide",
                "Fisherman", "Boatman", "ArtisanCrafts", "TraditionKeeper"
            };

            for (int i = 0; i < npcSpawnerCount; i++)
            {
                string npcType = npcTypes[i % npcTypes.Length];
                GameObject spawner = new GameObject($"NPCSpawner_{npcType}_{i}");
                spawner.transform.parent = npcsRoot.transform;
                spawner.transform.position = new Vector3(
                    Random.Range(-150f, 150f),
                    0f,
                    Random.Range(-150f, 150f)
                );

                var spawnMarker = spawner.AddComponent<BoxCollider>();
                spawnMarker.size = new Vector3(2f, 2f, 2f);
                spawnMarker.isTrigger = true;

                EnsureTag("NPCSpawner");
                spawner.tag = "NPCSpawner";
            }

            LogSuccess($"Generated {npcSpawnerCount} NPC spawners");
        }

        private void SetupTimeLoopIntegration()
        {
            GameObject gameplayRoot = FindOrCreateRoot("Gameplay");
            GameObject loopManagerObj = new GameObject("TimeLoopManager");
            loopManagerObj.transform.parent = gameplayRoot.transform;

            var loopConfig = loopManagerObj.AddComponent<BoxCollider>();
            loopConfig.size = Vector3.one;
            loopConfig.isTrigger = true;

            LogSuccess("Setup time-loop integration manager");
        }
    }
}