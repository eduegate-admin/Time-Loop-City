using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace TimeLoopCity.World
{
    /// <summary>
    /// Configures Post-Processing effects for cinematic visuals
    /// - Bloom for sun glare and water reflections
    /// - ACES Tonemapping for filmic look
    /// - Color Grading for tropical atmosphere
    /// - Vignette for focus
    /// 
    /// Note: This script provides recommended settings. 
    /// To use Post-Processing, install the package via Window > Package Manager.
    /// </summary>
    public class PostProcessingSetup : MonoBehaviour
    {
        [Header("Recommended Settings")]
        [TextArea(8, 15)]
        public string recommendedSettings = @"POST-PROCESSING SETUP (Optional)

To enable Post-Processing:
1. Window > Package Manager
2. Search 'Post Processing'
3. Install 'Post Processing' package

Then create a Post-Process Volume:
- GameObject > Volume > Global Volume
- Create Profile > Add effects:
  • Bloom: Intensity 0.3, Threshold 0.9
  • Tonemapping: ACES
  • Color Grading: Temperature +5, Tint +5
  • Vignette: Intensity 0.25

The city looks great without it, but Post-Processing adds extra polish!";

        public void ShowInstructions()
        {
            Debug.Log(recommendedSettings);
            
#if UNITY_EDITOR
            EditorUtility.DisplayDialog("Post-Processing Setup", 
                "Post-Processing is optional.\n\n" +
                "To install:\n" +
                "1. Window > Package Manager\n" +
                "2. Search 'Post Processing'\n" +
                "3. Install the package\n\n" +
                "See the recommendedSettings field for configuration details.", 
                "OK");
#endif
        }

#if UNITY_EDITOR
        [ContextMenu("Show Setup Instructions")]
        public void ShowInstructionsMenu()
        {
            ShowInstructions();
        }
#endif
    }
}
