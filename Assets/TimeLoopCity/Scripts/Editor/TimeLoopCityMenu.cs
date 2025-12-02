using UnityEngine;
using UnityEditor;
using TimeLoopCity.Editor.KochiSuite;

namespace TimeLoopCity.Editor
{
    public class TimeLoopCityMenu
    {
        [MenuItem("Tools/Time Loop City/Build Kochi Scene", false, 0)]
        public static void OpenSceneBuilder()
        {
            // We'll need to create a window or call the generator here
            // For now, let's just log
            Debug.Log("Opening Scene Builder...");
            // TODO: Connect to KochiGeneratorBase or specific window
            KochiSuiteWindow.ShowWindow();
        }

        [MenuItem("Tools/Time Loop City/Generate Real-World Kochi", false, 1)]
        public static void GenerateRealWorld()
        {
             KochiSuiteWindow.ShowWindow();
        }

        [MenuItem("Tools/Time Loop City/Generate PBR Materials", false, 2)]
        public static void GenerateMaterials()
        {
            // This logic was in PhotorealUpgradeGenerator
             KochiSuiteWindow.ShowWindow();
        }

        [MenuItem("Tools/Time Loop City/Auto-Repair Project", false, 10)]
        public static void AutoRepair()
        {
            // Call the repair tool
            ProjectAutoFixer.RunAutoRepair();
        }

        [MenuItem("Tools/Time Loop City/Validate Scene", false, 11)]
        public static void ValidateScene()
        {
            // Call validation
            Debug.Log("Validating Scene...");
        }
    }
}
