#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace TimeLoopCity.Editor
{
    /// <summary>
    /// Automatically validates project settings against the Build Guide.
    /// Run via Tools > Time Loop City > Validate Project
    /// </summary>
    public class BuildValidator : EditorWindow
    {
        [MenuItem("Time Loop City/Validate Project")]
        public static void ShowWindow()
        {
            GetWindow<BuildValidator>("Build Validator");
        }

        private Vector2 scrollPos;
        private string report = "Click 'Run Validation' to check project settings.";

        private void OnGUI()
        {
            if (GUILayout.Button("Run Validation", GUILayout.Height(40)))
            {
                RunValidation();
            }

            GUILayout.Space(10);
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            GUILayout.TextArea(report);
            GUILayout.EndScrollView();
        }

        private void RunValidation()
        {
            List<string> logs = new List<string>();
            int errors = 0;
            int warnings = 0;

            logs.Add($"Validation Started at {System.DateTime.Now}\n");

            // 1. Check Player Settings
            if (PlayerSettings.companyName == "DefaultCompany")
            {
                logs.Add("[FAIL] Company Name is DefaultCompany. Change in Project Settings > Player.");
                errors++;
            }
            else
            {
                logs.Add($"[PASS] Company Name: {PlayerSettings.companyName}");
            }

            if (PlayerSettings.colorSpace != ColorSpace.Linear)
            {
                logs.Add("[WARN] Color Space is not Linear. Recommended for better lighting.");
                warnings++;
            }
            else
            {
                logs.Add("[PASS] Color Space is Linear.");
            }

            // 2. Check Scenes
            var scenes = EditorBuildSettings.scenes;
            if (scenes.Length == 0)
            {
                logs.Add("[FAIL] No scenes in Build Settings.");
                errors++;
            }
            else
            {
                logs.Add($"[PASS] Found {scenes.Length} scenes in build.");
                foreach (var s in scenes)
                {
                    logs.Add($"  - {s.path} (Enabled: {s.enabled})");
                }
            }

            // 3. Check Quality
            // Note: QualitySettings API is limited in Editor scripts for reading all levels easily without SerializedObject
            // We'll do a basic check
            logs.Add("[INFO] Check Quality Settings manually for Shadow Distance > 100.");

            // 4. Check Layers (Basic check if layers exist)
            // Hard to check specific collisions without deep inspection, but we can remind user.
            logs.Add("[INFO] Verify Layer Collision Matrix in Project Settings > Physics.");

            // 5. Check Scripts
            string[] criticalScripts = new string[] 
            { 
                "TimeLoopManager", "WorldStateManager", "PersistentClueSystem", 
                "MainMenuUI", "SceneLoader", "EndGameManager" 
            };

            foreach (var script in criticalScripts)
            {
                string[] guids = AssetDatabase.FindAssets(script + " t:MonoScript");
                if (guids.Length == 0)
                {
                    logs.Add($"[FAIL] Critical Script Missing: {script}.cs");
                    errors++;
                }
                else
                {
                    logs.Add($"[PASS] Found Script: {script}.cs");
                }
            }

            // Summary
            logs.Add("\n--------------------------------");
            logs.Add($"Validation Complete. Errors: {errors}, Warnings: {warnings}");
            if (errors == 0)
            {
                logs.Add("RESULT: READY TO BUILD (Review warnings)");
            }
            else
            {
                logs.Add("RESULT: FIX ERRORS BEFORE BUILDING");
            }

            report = string.Join("\n", logs);
        }
    }
}
#endif
