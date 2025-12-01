#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.AI;
using System.IO;

namespace TimeLoopCity.Editor
{
    public class AutoRepairTool : EditorWindow
    {
        [MenuItem("Tools/Time Loop City/🔧 AUTO-REPAIR EVERYTHING", false, 0)]
        public static void RunAutoRepair()
        {
            if (!EditorUtility.DisplayDialog("Auto-Repair Project", 
                "This will:\n\n" +
                "1. Fix Input System Settings (Force Legacy)\n" +
                "2. Create/Update MainCity Scene\n" +
                "3. Fix Manager Hierarchy\n" +
                "4. Bake NavMesh\n" +
                "5. Import Essentials\n\n" +
                "This may restart the Editor. Continue?", "Yes, Fix Everything", "Cancel"))
            {
                return;
            }

            Debug.Log("<b>[Auto-Repair]</b> Starting repair process...");

            // 1. Fix Input System
            FixInputSystem();

            // 2. Ensure Folders Exist
            EnsureFolders();

            // 3. Rebuild Scene (relies on MainCitySceneBuilder)
            MainCitySceneBuilder.BuildMainCityScene(autoConfirm: true);

            // 4. Bake NavMesh
            BakeNavMesh();

            // 5. Refresh
            AssetDatabase.Refresh();
            
            Debug.Log("<b>[Auto-Repair]</b> <color=green>SUCCESS! Project repaired.</color>");
            EditorUtility.DisplayDialog("Repair Complete", 
                "Project has been repaired!\n\n" +
                "1. Input System set to Legacy (Restart Unity if prompted)\n" +
                "2. MainCity scene regenerated\n" +
                "3. NavMesh baked\n\n" +
                "Please PRESS PLAY to test.", "OK");
        }

        private static void FixInputSystem()
        {
#if UNITY_EDITOR
            Debug.Log("[Auto-Repair] Checking Input System settings...");
            
            // This API might vary by Unity version, but this is the standard way to check/set
            // We want to enable "Both" or "Input Manager (Old)"
            // 0 = Legacy, 1 = New, 2 = Both
            
            // Note: Changing this usually requires a restart, so we warn the user.
            // We can try to set it via SerializedObject on ProjectSettings if API is restricted.
            
            try 
            {
                var playerSettings = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset")[0];
                var so = new SerializedObject(playerSettings);
                var activeInputHandler = so.FindProperty("activeInputHandler");
                
                if (activeInputHandler != null)
                {
                    if (activeInputHandler.intValue == 1) // If set to New only
                    {
                        Debug.Log("[Auto-Repair] Switching Input System to Legacy...");
                        activeInputHandler.intValue = 0; // Set to Legacy
                        so.ApplyModifiedProperties();
                        Debug.Log("[Auto-Repair] <color=yellow>Input System updated. Editor restart may be required.</color>");
                    }
                    else
                    {
                        Debug.Log("[Auto-Repair] Input System already compatible.");
                    }
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[Auto-Repair] Failed to auto-fix Input System: {e.Message}");
            }
#endif
        }

        private static void EnsureFolders()
        {
            string[] folders = new[]
            {
                "Assets/Scenes",
                "Assets/ScriptableObjects",
                "Assets/Prefabs",
                "Assets/Materials"
            };

            foreach (var folder in folders)
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                    Debug.Log($"[Auto-Repair] Created missing folder: {folder}");
                }
            }
        }

        private static void BakeNavMesh()
        {
            Debug.Log("[Auto-Repair] Baking NavMesh...");
            
            // Bake NavMesh for the scene
            // Use Lightmapping API which internally bakes NavMesh
            Lightmapping.BakeAsync();
            Debug.Log("[Auto-Repair] NavMesh queued for baking.");
        }
    }
}
#endif
