using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TimeLoopCity.Editor.KochiSuite
{
    public class KochiSuiteWindow : EditorWindow
    {
        private Vector2 scrollPosition;
        private int selectedTab = 0;
        private string[] tabs = { "Scene", "Roads", "Buildings", "Terrain", "Traffic", "Environment", "Gameplay", "Photoreal" };

        private KochiGeneratorBase[] generators;

        [MenuItem("Tools/Time Loop City/Open Kochi Suite", false, 0)]
        public static void ShowWindow()
        {
            GetWindow<KochiSuiteWindow>("Kochi Suite");
        }

        private void OnEnable()
        {
            // Initialize generators
            generators = new KochiGeneratorBase[]
            {
                new BuildValidateOptimizeGenerator(), // Placeholder for Scene
                new RoadsAndOSMGenerator(),
                new BuildingsGenerator(),
                new TerrainGenerator(),
                new TrafficAndBoatsGenerator(),
                new EnvironmentGenerator(),
                new GameplayMissionsGenerator(),
                new PhotorealUpgradeGenerator()
            };
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Time Loop City - Kochi Generator Suite", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            selectedTab = GUILayout.Toolbar(selectedTab, tabs);
            EditorGUILayout.Space();

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            if (selectedTab >= 0 && selectedTab < generators.Length)
            {
                var generator = generators[selectedTab];
                if (generator != null)
                {
                    generator.DrawGUI();
                    EditorGUILayout.Space();
                    if (GUILayout.Button("Generate " + tabs[selectedTab]))
                    {
                        // We need to reflectively call the specific generate method or add a common one to base
                        // For now, we'll use a switch or cast
                        RunGenerator(selectedTab);
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("Generator not initialized.", MessageType.Warning);
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void RunGenerator(int index)
        {
            switch (index)
            {
                case 1: ((RoadsAndOSMGenerator)generators[index]).GenerateRoads(); break;
                case 2: ((BuildingsGenerator)generators[index]).GenerateBuildings(); break;
                case 3: ((TerrainGenerator)generators[index]).GenerateTerrain(); break;
                case 4: ((TrafficAndBoatsGenerator)generators[index]).GenerateTraffic(); break;
                case 5: ((EnvironmentGenerator)generators[index]).GenerateEnvironment(); break;
                case 6: ((GameplayMissionsGenerator)generators[index]).GenerateGameplay(); break;
                case 7: ((PhotorealUpgradeGenerator)generators[index]).GeneratePhotoreal(); break;
                default: Debug.Log("Generator logic not linked yet."); break;
            }
        }
    }
}
