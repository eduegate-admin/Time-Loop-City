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
            // Ensure EventSystem
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
                DontDestroyOnLoad(eventSystem);
                Debug.Log("[SystemBootstrapper] Created missing EventSystem.");
            }
        }
    }
}
