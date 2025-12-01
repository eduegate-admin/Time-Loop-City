#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.AI;
using UnityEditor;

namespace TimeLoopCity.Editor
{
    public class CityGeneratorTool : EditorWindow
    {
        [MenuItem("Tools/Time Loop City/🏙️ GENERATE PROCEDURAL CITY", false, 2)]
        public static void GenerateCity()
        {
            if (!EditorUtility.DisplayDialog("Generate Procedural City",
                "This will replace the current city with a procedural layout:\n\n" +
                "🏗️ Modular road network\n" +
                "🏢 Realistic buildings with windows\n" +
                "🚦 Traffic lights at intersections\n" +
                "💡 Street lamps\n" +
                "🪑 Street furniture\n\n" +
                "Continue?", "Generate", "Cancel"))
            {
                return;
            }

            Debug.Log("<b>[City Generator]</b> Starting procedural generation...");

            // Find or create generator using reflection to avoid assembly reference issues
            var generatorType = System.Type.GetType("TimeLoopCity.Environment.ProceduralCityGenerator");
            var generator = Object.FindFirstObjectByType(generatorType) as MonoBehaviour;
            
            if (generator == null)
            {
                GameObject genObj = new GameObject("CityGenerator");
                generator = genObj.AddComponent(generatorType) as MonoBehaviour;
                Debug.Log("[City Generator] Created new CityGenerator component");
            }

            // Generate the city using reflection
            var generateMethod = generatorType.GetMethod("GenerateCity");
            if (generateMethod != null)
            {
                generateMethod.Invoke(generator, new object[] { 42 });
            }

            // Mark scene dirty
            UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();

            Debug.Log("<b>[City Generator]</b> <color=green>✨ City generation complete!</color>");
            
            EditorUtility.DisplayDialog("City Generated!",
                "🎉 Your procedural city is ready!\n\n" +
                "✅ Roads and sidewalks\n" +
                "✅ Buildings with windows\n" +
                "✅ Street lamps\n" +
                "✅ Traffic lights\n" +
                "✅ Street furniture\n\n" +
                "Press Play to explore!", "Awesome!");
        }
    }
}
#endif
