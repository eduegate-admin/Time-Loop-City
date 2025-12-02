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
            // Avoid running in edit mode (safety)
            if (!Application.isPlaying) return;

            // Ensure EventSystem exists (but only create if truly missing)
            EventSystem[] eventSystems = FindObjectsOfType<EventSystem>();

            if (eventSystems.Length == 0)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                DontDestroyOnLoad(eventSystem);
                Debug.Log("[SystemBootstrapper] Created missing EventSystem.");
                return;
            }

            if (eventSystems.Length > 1)
            {
                // Keep the first active EventSystem, disable others to prevent continuous spam
                EventSystem primary = eventSystems[0];
                int removed = 0;

                for (int i = 1; i < eventSystems.Length; i++)
                {
                    var es = eventSystems[i];
                    if (es == null) continue;

                    // If it's on a DontDestroyOnLoad object or already active, prefer to keep the primary intact
                    if (es.gameObject.scene.isLoaded == false)
                    {
                        es.gameObject.SetActive(false);
                        removed++;
                        continue;
                    }

                    es.gameObject.SetActive(false);
                    removed++;
                }

                Debug.LogWarning($"[SystemBootstrapper] Found {eventSystems.Length} EventSystems. Disabled {removed} duplicate(s).");
            }
        }
    }
}
