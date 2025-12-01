using UnityEngine;
using UnityEngine.EventSystems;

namespace TimeLoopCity.Managers
{
    /// <summary>
    /// Ensures essential Unity systems exist in the scene.
    /// Auto-creates EventSystem if missing.
    /// </summary>
    public class SystemBootstrapper : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            GameObject systems = new GameObject("SystemBootstrapper");
            DontDestroyOnLoad(systems);
            systems.AddComponent<SystemBootstrapper>();
        }

        private void Awake()
        {
            // Ensure EventSystem exists (but only create if truly missing)
            EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();
            
            if (eventSystems.Length == 0)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                DontDestroyOnLoad(eventSystem);
                Debug.Log("[SystemBootstrapper] Created missing EventSystem.");
            }
            else if (eventSystems.Length > 1)
            {
                Debug.LogWarning($"[SystemBootstrapper] Found {eventSystems.Length} EventSystems. " +
                    "There should only be one. Removing duplicates...");
                
                // Keep the first one, destroy the rest
                for (int i = 1; i < eventSystems.Length; i++)
                {
                    Destroy(eventSystems[i].gameObject);
                }
            }
        }
    }
}
