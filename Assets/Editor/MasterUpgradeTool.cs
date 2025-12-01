#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace TimeLoopCity.Editor
{
    /// <summary>
    /// MASTER UPGRADE TOOL - Executes all improvements in sequence
    /// Transforms the game from prototype to professional quality
    /// </summary>
    public class MasterUpgradeTool : EditorWindow
    {
        [MenuItem("Tools/Time Loop City/🚀 MASTER UPGRADE (RUN THIS)", false, 0)]
        public static void RunMasterUpgrade()
        {
            if (!EditorUtility.DisplayDialog("MASTER UPGRADE",
                "This will transform your game into a professional product:\n\n" +
                "🏙️ Procedural city with roads & buildings\n" +
                "✨ Enhanced lighting & post-processing\n" +
                "🎮 Smooth player controller & camera\n" +
                "💡 Street lamps & traffic lights\n" +
                "🎨 Professional materials\n" +
                "🌆 Skybox & atmospheric effects\n\n" +
                "This will replace the current scene. Continue?",
                "YES - UPGRADE EVERYTHING", "Cancel"))
            {
                return;
            }

            Debug.Log("<b><color=cyan>═══════════════════════════════════════</color></b>");
            Debug.Log("<b><color=cyan>🚀 MASTER UPGRADE - STARTING...</color></b>");
            Debug.Log("<b><color=cyan>═══════════════════════════════════════</color></b>");

            int step = 1;

            // STEP 1: Save current scene
            EditorUtility.DisplayProgressBar("Master Upgrade", "Step 1/6: Saving scene...", 0.1f);
            Debug.Log($"<b>[Step {step++}]</b> Saving current scene...");
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorUtility.ClearProgressBar();
                return;
            }

            // STEP 2: Run Auto-Repair (Input System, Managers, Folders)
            EditorUtility.DisplayProgressBar("Master Upgrade", "Step 2/6: Running auto-repair...", 0.2f);
            Debug.Log($"<b>[Step {step++}]</b> Running auto-repair...");
            AutoRepairTool.RunAutoRepair();

            // STEP 3: Generate Procedural City
            EditorUtility.DisplayProgressBar("Master Upgrade", "Step 3/6: Generating procedural city...", 0.4f);
            Debug.Log($"<b>[Step {step++}]</b> Generating procedural city...");
            CityGeneratorTool.GenerateCity();

            // STEP 4: Apply Visual Polish
            EditorUtility.DisplayProgressBar("Master Upgrade", "Step 4/6: Applying visual polish...", 0.6f);
            Debug.Log($"<b>[Step {step++}]</b> Applying visual polish...");
            VisualPolishTool.ApplyVisualPolish();

            // STEP 5: Upgrade Player Controller
            EditorUtility.DisplayProgressBar("Master Upgrade", "Step 5/6: Upgrading player controller...", 0.8f);
            Debug.Log($"<b>[Step {step++}]</b> Upgrading player controller...");
            UpgradePlayerController();

            // STEP 6: Final Setup
            EditorUtility.DisplayProgressBar("Master Upgrade", "Step 6/6: Final setup...", 0.9f);
            Debug.Log($"<b>[Step {step++}]</b> Finalizing...");
            FinalSetup();

            EditorUtility.ClearProgressBar();

            Debug.Log("<b><color=green>═══════════════════════════════════════</color></b>");
            Debug.Log("<b><color=green>✨ MASTER UPGRADE COMPLETE!</color></b>");
            Debug.Log("<b><color=green>═══════════════════════════════════════</color></b>");

            EditorUtility.DisplayDialog("🎉 UPGRADE COMPLETE!",
                "Your game has been transformed!\n\n" +
                "✅ Procedural city with roads\n" +
                "✅ Realistic buildings with windows\n" +
                "✅ Street lamps & traffic lights\n" +
                "✅ Enhanced player controller\n" +
                "✅ Cinematic lighting\n" +
                "✅ Professional materials\n\n" +
                "PRESS PLAY to see the difference!\n\n" +
                "It won't look like cubes anymore - it's a REAL CITY now! 🏙️",
                "AWESOME! LET'S PLAY!");
        }

        private static void UpgradePlayerController()
        {
            GameObject player = GameObject.Find("Player");
            
            if (player == null)
            {
                Debug.LogWarning("[Master Upgrade] Player not found, skipping controller upgrade");
                return;
            }

            // Ensure PlayerController exists
            var playerController = player.GetComponent<TimeLoopCity.Player.PlayerController>();
            if (playerController == null)
            {
                playerController = player.AddComponent<TimeLoopCity.Player.PlayerController>();
                Debug.Log("  ✓ PlayerController added");
            }
            else
            {
                Debug.Log("  ✓ PlayerController already present");
            }
            
            var newController = playerController;

            // Ensure required components
            if (player.GetComponent<CharacterController>() == null)
            {
                player.AddComponent<CharacterController>();
                Debug.Log("  ✓ CharacterController added");
            }

            // Set camera target
            Transform cameraTarget = player.transform.Find("CameraTarget");
            if (cameraTarget != null)
            {
                var so = new SerializedObject(newController);
                so.FindProperty("cameraTarget").objectReferenceValue = cameraTarget;
                so.ApplyModifiedPropertiesWithoutUndo();
                Debug.Log("  ✓ Camera target assigned");
            }
        }

        private static void FinalSetup()
        {
            // Ensure lighting is baked
            Lightmapping.BakeAsync();
            
            // Mark scene dirty
            EditorSceneManager.MarkAllScenesDirty();

            // Refresh
            AssetDatabase.Refresh();

            Debug.Log("  ✓ Scene marked dirty");
            Debug.Log("  ✓ Assets refreshed");
            Debug.Log("  ✓ Lightmaps queued for baking");
        }
    }
}
#endif
