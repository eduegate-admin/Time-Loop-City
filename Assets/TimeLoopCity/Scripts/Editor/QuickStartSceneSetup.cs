#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

namespace TimeLoopCity.Editor
{
    /// <summary>
    /// Automatically sets up a playable Time Loop City scene.
    /// Run via Tools > Time Loop City > Quick Start Scene Setup
    /// </summary>
    public class QuickStartSceneSetup : EditorWindow
    {
        [MenuItem("Time Loop City/Quick Start Scene Setup")]
        public static void SetupScene()
        {
            if (EditorUtility.DisplayDialog("Quick Start Setup", 
                "This will create all required GameObjects in the current scene. Continue?", 
                "Yes", "Cancel"))
            {
                CreateQuickStartScene();
            }
        }

        private static void CreateQuickStartScene()
        {
            Debug.Log("[Quick Start] Setting up Time Loop City scene...");

            // 1. Create Ground
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.position = Vector3.zero;
            ground.transform.localScale = new Vector3(10, 1, 10);
            ground.isStatic = true;

            // 2. Create Player
            GameObject player = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            player.name = "Player";
            player.tag = "Player";
            player.transform.position = new Vector3(0, 1, 0);
            
            // Add Player Controller
            player.AddComponent<Player.PlayerController>();
            
            // Add Rigidbody
            Rigidbody rb = player.AddComponent<Rigidbody>();
            rb.constraints = RigidbodyConstraints.FreezeRotation;

            // 3. Create Main Camera
            GameObject camObj = new GameObject("Main Camera");
            camObj.tag = "MainCamera";
            Camera cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            camObj.AddComponent<Visuals.CameraShake>();
            camObj.transform.position = new Vector3(0, 5, -10);
            camObj.transform.LookAt(Vector3.zero);

            // 4. Create Directional Light
            GameObject lightObj = new GameObject("Directional Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObj.transform.rotation = Quaternion.Euler(50, -30, 0);

            // 5. Create Managers Parent
            GameObject managers = new GameObject("=== MANAGERS ===");

            // Core Managers
            CreateManager<TimeLoop.TimeLoopManager>("TimeLoopManager", managers.transform);
            CreateManager<World.WorldStateManager>("WorldStateManager", managers.transform);
            CreateManager<Core.PersistentClueSystem>("PersistentClueSystem", managers.transform);
            CreateManager<Managers.GameManager>("GameManager", managers.transform);
            CreateManager<Managers.SaveLoadManager>("SaveLoadManager", managers.transform);
            CreateManager<Managers.MissionManager>("MissionManager", managers.transform);
            CreateManager<Dialogue.DialogueManager>("DialogueManager", managers.transform);
            CreateManager<Managers.InteractionManager>("InteractionManager", managers.transform);
            CreateManager<Managers.EndGameManager>("EndGameManager", managers.transform);

            // World Systems
            GameObject worldSystems = new GameObject("=== WORLD SYSTEMS ===");
            CreateManager<World.TimeOfDaySystem>("TimeOfDaySystem", worldSystems.transform);
            CreateManager<World.WeatherSystem>("WeatherSystem", worldSystems.transform);
            CreateManager<Events.RandomEventSpawner>("RandomEventSpawner", worldSystems.transform);

            // Audio & Visuals
            GameObject audioVisuals = new GameObject("=== AUDIO & VISUALS ===");
            CreateManager<Audio.AudioManager>("AudioManager", audioVisuals.transform);
            CreateManager<Visuals.PostProcessingController>("PostProcessingController", audioVisuals.transform);

            // 6. Create UI Canvas
            GameObject canvasObj = new GameObject("UI Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            // Time Display
            GameObject timeDisplayObj = new GameObject("TimeDisplay");
            timeDisplayObj.transform.SetParent(canvasObj.transform);
            timeDisplayObj.AddComponent<UI.UI_TimeDisplay>();

            // Loop Reset Transition
            GameObject transitionObj = new GameObject("LoopResetTransition");
            transitionObj.transform.SetParent(canvasObj.transform);
            transitionObj.AddComponent<UI.LoopResetTransition>();

            // Dialogue UI
            GameObject dialogueObj = new GameObject("DialogueUI");
            dialogueObj.transform.SetParent(canvasObj.transform);
            dialogueObj.AddComponent<UI.DialogueUI>();

            // Evidence Board
            GameObject evidenceObj = new GameObject("EvidenceBoardUI");
            evidenceObj.transform.SetParent(canvasObj.transform);
            evidenceObj.AddComponent<UI.EvidenceBoardUI>();

            // Credits UI
            GameObject creditsObj = new GameObject("CreditsUI");
            creditsObj.transform.SetParent(canvasObj.transform);
            creditsObj.AddComponent<UI.CreditsUI>();

            // 7. Create City Generator (Optional)
            GameObject cityGen = new GameObject("CityGenerator");
            cityGen.AddComponent<World.CityGenerator>();

            Debug.Log("[Quick Start] Scene setup complete! Press Play to start.");
            EditorUtility.DisplayDialog("Setup Complete", 
                "Time Loop City scene is ready!\n\nPress the Play button to start the game.", 
                "OK");
        }

        private static GameObject CreateManager<T>(string name, Transform parent) where T : Component
        {
            GameObject obj = new GameObject(name);
            obj.transform.SetParent(parent);
            obj.AddComponent<T>();
            return obj;
        }
    }
}
#endif
