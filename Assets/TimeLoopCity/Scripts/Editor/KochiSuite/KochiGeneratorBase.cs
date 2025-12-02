using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TimeLoopCity.Editor.KochiSuite
{
    /// <summary>
    /// Abstract base class for all Kochi generators - provides shared utilities and progress tracking
    /// </summary>
    public abstract class KochiGeneratorBase
    {
        protected bool isRunning = false;
        protected float progress = 0f;

        /// <summary>
        /// Draw generator-specific GUI in the suite window
        /// </summary>
        public abstract void DrawGUI();

        /// <summary>
        /// Update progress (0-1 range)
        /// </summary>
        protected virtual void SetProgress(float value)
        {
            progress = Mathf.Clamp01(value);
        }

        /// <summary>
        /// Log success message with emoji
        /// </summary>
        protected virtual void LogSuccess(string message)
        {
            Debug.Log($"✅ {message}");
        }

        /// <summary>
        /// Log warning message with emoji
        /// </summary>
        protected virtual void LogWarning(string message)
        {
            Debug.LogWarning($"⚠️ {message}");
        }

        /// <summary>
        /// Log error message with emoji
        /// </summary>
        protected virtual void LogError(string message)
        {
            Debug.LogError($"❌ {message}");
        }

        /// <summary>
        /// Ensure a folder exists in the asset database
        /// </summary>
        protected virtual void EnsureFolder(string folderPath)
        {
            string[] parts = folderPath.Split('/');
            string currentPath = "";
            foreach (string part in parts)
            {
                if (part == "" || part == "Assets")
                {
                    currentPath = "Assets";
                    continue;
                }
                string newPath = $"{currentPath}/{part}";
                if (!AssetDatabase.IsValidFolder(newPath))
                {
                    AssetDatabase.CreateFolder(currentPath, part);
                }
                currentPath = newPath;
            }
        }

        /// <summary>
        /// Find or create a root GameObject in the scene
        /// </summary>
        protected virtual GameObject FindOrCreateRoot(string rootName)
        {
            GameObject existing = GameObject.Find(rootName);
            if (existing != null)
                return existing;

            GameObject newRoot = new GameObject(rootName);
            return newRoot;
        }

        /// <summary>
        /// Find or add a component to a GameObject
        /// </summary>
        protected virtual T FindOrAddComponent<T>(GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            if (component == null)
                component = obj.AddComponent<T>();
            return component;
        }
        /// <summary>
        /// Ensure a tag exists in the project
        /// </summary>
        protected void EnsureTag(string tagName)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            bool found = false;
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                SerializedProperty t = tagsProp.GetArrayElementAtIndex(i);
                if (t.stringValue.Equals(tagName)) { found = true; break; }
            }

            if (!found)
            {
                tagsProp.InsertArrayElementAtIndex(0);
                SerializedProperty n = tagsProp.GetArrayElementAtIndex(0);
                n.stringValue = tagName;
                tagManager.ApplyModifiedProperties();
                LogSuccess($"Created new tag: {tagName}");
            }
        }
    }
}