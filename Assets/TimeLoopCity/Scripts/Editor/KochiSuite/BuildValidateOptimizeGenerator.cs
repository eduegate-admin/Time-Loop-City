using UnityEngine;
using UnityEditor;

namespace TimeLoopCity.Editor.KochiSuite
{
    public class BuildValidateOptimizeGenerator : KochiGeneratorBase
    {
        public override void DrawGUI()
        {
            EditorGUILayout.LabelField("Scene Build & Validation", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Use this tab to validate the scene and prepare for build.", MessageType.Info);
        }
    }
}
