using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;
using System.Linq;

namespace TimeLoopCity.Editor
{
    public class ProjectAutoFixer
    {
        [MenuItem("GameObject/Time Loop City/Fix Material (URP)", false, 10)]
        public static void FixSelectedMaterial()
        {
            GameObject obj = Selection.activeGameObject;
            if (obj == null) return;

            Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
            
            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (mat != null) mat.shader = urpLit;
                }
            }
            Debug.Log($"Fixed materials on {obj.name}");
        }

        public static void RunAutoRepair()
        {
            FixMaterials();
            CleanupScene();
            Debug.Log("✅ Project Auto-Repair Complete!");
        }

        private static void FixMaterials()
        {
            string[] guids = AssetDatabase.FindAssets("t:Material");
            int fixedCount = 0;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

                if (mat != null)
                {
                    if (mat.shader.name == "Standard" || mat.shader.name == "Hidden/InternalErrorShader" || mat.shader.name.Contains("Magenta"))
                    {
                        Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");
                        if (urpLit != null)
                        {
                            mat.shader = urpLit;
                            mat.color = mat.color; // Refresh color
                            fixedCount++;
                        }
                    }
                }
            }
            Debug.Log($"✅ Fixed {fixedCount} material assets to URP Lit.");
            
            FixSceneMaterials();
        }

        private static void FixSceneMaterials()
        {
            Renderer[] renderers = Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None);
            int fixedCount = 0;
            Shader urpLit = Shader.Find("Universal Render Pipeline/Lit");

            if (urpLit == null)
            {
                Debug.LogError("❌ Could not find URP Lit shader!");
                return;
            }

            foreach (Renderer r in renderers)
            {
                foreach (Material mat in r.sharedMaterials)
                {
                    if (mat != null && (mat.shader.name == "Standard" || mat.shader.name == "Hidden/InternalErrorShader" || mat.shader.name.Contains("Magenta")))
                    {
                        mat.shader = urpLit;
                        fixedCount++;
                    }
                }
            }
            Debug.Log($"✅ Fixed {fixedCount} scene materials to URP Lit.");
        }

        private static void CleanupScene()
        {
            // Fix EventSystems
            var eventSystems = Object.FindObjectsByType<UnityEngine.EventSystems.EventSystem>(FindObjectsSortMode.None);
            if (eventSystems.Length > 1)
            {
                for (int i = 1; i < eventSystems.Length; i++)
                {
                    Object.DestroyImmediate(eventSystems[i].gameObject);
                }
                Debug.Log($"✅ Removed {eventSystems.Length - 1} duplicate EventSystems.");
            }

            // Fix AudioListeners
            var listeners = Object.FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
            if (listeners.Length > 1)
            {
                for (int i = 1; i < listeners.Length; i++)
                {
                    Object.DestroyImmediate(listeners[i]);
                }
                Debug.Log($"✅ Removed {listeners.Length - 1} duplicate AudioListeners.");
            }

            // Fix Cameras (Tag MainCamera)
            var cameras = GameObject.FindGameObjectsWithTag("MainCamera");
            if (cameras.Length > 0)
            {
                // Remove duplicates
                if (cameras.Length > 1)
                {
                    GameObject keeper = cameras[0];
                    for (int i = 1; i < cameras.Length; i++)
                    {
                        if (cameras[i].GetComponent("CinemachineBrain") != null && keeper.GetComponent("CinemachineBrain") == null)
                        {
                            Object.DestroyImmediate(keeper);
                            keeper = cameras[i];
                        }
                        else
                        {
                            Object.DestroyImmediate(cameras[i]);
                        }
                    }
                    Debug.Log($"✅ Removed {cameras.Length - 1} duplicate MainCameras.");
                }

                // Fix URP Camera Data
                GameObject mainCam = GameObject.FindGameObjectWithTag("MainCamera");
                if (mainCam != null)
                {
                    var camData = mainCam.GetComponent<UniversalAdditionalCameraData>();
                    if (camData == null)
                    {
                        mainCam.AddComponent<UniversalAdditionalCameraData>();
                        Debug.Log("✅ Added missing UniversalAdditionalCameraData to Main Camera.");
                    }
                }
            }
        }
    }
}
