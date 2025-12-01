#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;

namespace TimeLoopCity.Editor
{
    /// <summary>
    /// Builds the entire MainCity scene with a single click.
    /// Adds managers, systems, basic geometry, UI, NPCs, and example interactables.
    /// Accessible via Tools > Time Loop City > Build MainCity Scene.
    /// </summary>
    public static class MainCitySceneBuilder
    {
        private const string ClueDatabasePath = "Assets/ScriptableObjects/GeneratedClueDatabase.asset";

        [MenuItem("Tools/Time Loop City/Build MainCity Scene")]
        public static void BuildMainCityScene()
        {
            BuildMainCityScene(false);
        }

        public static void BuildMainCityScene(bool autoConfirm)
        {
            if (!autoConfirm && !EditorUtility.DisplayDialog(
                    "Rebuild MainCity Scene",
                    "This will wipe the current scene and generate a fresh MainCity layout.\n\nContinue?",
                    "Build", "Cancel"))
            {
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                return;
            }

            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            var context = new SceneBuildContext
            {
                Root = new GameObject("MainCity_Root")
            };

            SetupLighting(context);
            SetupEnvironment(context);
            SetupPlayerAndCamera(context);
            SetupManagers(context);
            SetupWorldSystems(context);
            SetupNPCs(context);
            SetupUI(context);
            SetupExampleClue(context);

            Selection.activeGameObject = context.Player;
            SceneView.lastActiveSceneView?.FrameSelected();
            EditorSceneManager.MarkSceneDirty(scene);

            Debug.Log("[MainCity Builder] MainCity scene generated successfully.");
        }

        private static void SetupLighting(SceneBuildContext context)
        {
            var lightingRoot = CreateChild("Lighting", context.Root.transform);
            var sunObj = new GameObject("Sun Light");
            sunObj.transform.SetParent(lightingRoot.transform);
            sunObj.transform.rotation = Quaternion.Euler(55f, -35f, 0f);

            var light = sunObj.AddComponent<Light>();
            light.type = LightType.Directional;
            light.intensity = 1.15f;
            light.shadows = LightShadows.Soft;
            context.Sun = light;

            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.35f, 0.38f, 0.45f);
            RenderSettings.fog = true;
            RenderSettings.fogColor = new Color(0.7f, 0.78f, 0.9f);
            RenderSettings.fogDensity = 0.002f;
        }

        private static void SetupEnvironment(SceneBuildContext context)
        {
            var envRoot = CreateChild("Environment", context.Root.transform);

            var ground = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ground.name = "Ground";
            ground.transform.SetParent(envRoot.transform);
            ground.transform.localScale = new Vector3(20f, 1f, 20f);
            ground.isStatic = true; // Important for NavMesh
            TintRenderer(ground.GetComponent<MeshRenderer>(), new Color(0.2f, 0.42f, 0.2f));

            var buildingsRoot = CreateChild("Buildings", envRoot.transform);
            var rnd = new System.Random(1337);
            for (int x = -2; x <= 2; x++)
            {
                for (int z = -2; z <= 2; z++)
                {
                    if (x == 0 && z == 0) continue; // central plaza

                    float height = Mathf.Lerp(8f, 22f, (float)rnd.NextDouble());
                    var building = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    building.name = $"Building_{x}_{z}";
                    building.transform.SetParent(buildingsRoot.transform);
                    building.transform.position = new Vector3(x * 10f, height / 2f, z * 10f);
                    building.transform.localScale = new Vector3(7f, height, 7f);
                    building.isStatic = true; // Important for NavMesh

                    var color = Color.Lerp(new Color(0.4f, 0.4f, 0.55f), new Color(0.2f, 0.2f, 0.8f), (float)rnd.NextDouble());
                    TintRenderer(building.GetComponent<MeshRenderer>(), color);
                    context.Buildings.Add(building);
                }
            }
        }

        private static void SetupPlayerAndCamera(SceneBuildContext context)
        {
            var spawn = new GameObject("PlayerSpawnPoint");
            spawn.transform.SetParent(context.Root.transform);
            spawn.transform.position = new Vector3(0f, 0f, -12f);
            context.PlayerSpawn = spawn.transform;

            var player = new GameObject("Player");
            player.tag = "Player";
            player.transform.SetParent(context.Root.transform);
            player.transform.position = spawn.transform.position + Vector3.up * 0.1f;
            context.Player = player;

            var controller = player.AddComponent<CharacterController>();
            controller.height = 1.8f;
            controller.radius = 0.4f;
            controller.center = new Vector3(0f, 0.9f, 0f);

            var playerController = player.AddComponent<TimeLoopCity.Player.PlayerController>();

            var visual = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            visual.name = "PlayerVisual";
            visual.transform.SetParent(player.transform);
            visual.transform.localPosition = Vector3.zero;
            Object.DestroyImmediate(visual.GetComponent<Collider>());

            var cameraTarget = new GameObject("CameraTarget");
            cameraTarget.transform.SetParent(player.transform);
            cameraTarget.transform.localPosition = new Vector3(0f, 1.6f, 0f);
            context.CameraTarget = cameraTarget.transform;

            var playerControllerSO = new SerializedObject(playerController);
            var camTargetProp = playerControllerSO.FindProperty("cameraTarget");
            if (camTargetProp != null)
            {
                camTargetProp.objectReferenceValue = cameraTarget.transform;
                playerControllerSO.ApplyModifiedPropertiesWithoutUndo();
            }

            var cameraRig = new GameObject("CameraRig");
            cameraRig.transform.SetParent(player.transform);
            cameraRig.transform.localPosition = Vector3.zero;

            var mainCamera = new GameObject("Main Camera");
            mainCamera.transform.SetParent(cameraRig.transform);
            mainCamera.transform.localPosition = new Vector3(0f, 3f, -7f);
            mainCamera.transform.LookAt(cameraTarget.transform);
            mainCamera.tag = "MainCamera";

            var camera = mainCamera.AddComponent<Camera>();
            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 1000f;
            mainCamera.AddComponent<AudioListener>();
        }

        private static void SetupManagers(SceneBuildContext context)
        {
            // Managers that use DontDestroyOnLoad MUST be at the root
            // We do NOT parent them to managersRoot or context.Root
            
            // 1. TimeLoopManager (Root)
            context.TimeLoopManager = CreateManager<TimeLoopCity.TimeLoop.TimeLoopManager>("TimeLoopManager", null);
            var timeLoopSO = new SerializedObject(context.TimeLoopManager);
            var spawnProp = timeLoopSO.FindProperty("playerSpawnPoint");
            if (spawnProp != null)
            {
                spawnProp.objectReferenceValue = context.PlayerSpawn;
                timeLoopSO.ApplyModifiedPropertiesWithoutUndo();
            }

            // 2. SaveLoadManager (Root)
            CreateManager<TimeLoopCity.Managers.SaveLoadManager>("SaveLoadManager", null);

            // 3. GameManager (Root)
            CreateManager<TimeLoopCity.Managers.GameManager>("GameManager", null);

            // Other managers can be grouped
            var managersRoot = CreateChild("=== MANAGERS ===", context.Root.transform);
            context.ManagersRoot = managersRoot;

            var persistent = CreateManager<TimeLoopCity.Core.PersistentClueSystem>("PersistentClueSystem", managersRoot.transform);
            var clueDb = EnsureClueDatabaseAsset();
            var persistentSO = new SerializedObject(persistent);
            var dbProp = persistentSO.FindProperty("clueDatabase");
            if (dbProp != null)
            {
                dbProp.objectReferenceValue = clueDb;
                persistentSO.ApplyModifiedPropertiesWithoutUndo();
            }

            CreateManager<TimeLoopCity.Managers.InteractionManager>("InteractionManager", managersRoot.transform);
            CreateManager<TimeLoopCity.Managers.MissionManager>("MissionManager", managersRoot.transform);
            CreateManager<TimeLoopCity.Dialogue.DialogueManager>("DialogueManager", managersRoot.transform);
            CreateManager<TimeLoopCity.Managers.EndGameManager>("EndGameManager", managersRoot.transform);
        }

        private static void SetupWorldSystems(SceneBuildContext context)
        {
            var systemsRoot = CreateChild("=== WORLD SYSTEMS ===", context.Root.transform);
            context.WorldSystemsRoot = systemsRoot;

            var timeOfDay = CreateChild("TimeOfDaySystem", systemsRoot.transform)
                .AddComponent<TimeLoopCity.World.TimeOfDaySystem>();
            context.TimeOfDaySystem = timeOfDay;

            var weather = CreateChild("WeatherSystem", systemsRoot.transform)
                .AddComponent<TimeLoopCity.World.WeatherSystem>();
            context.WeatherSystem = weather;

            var worldState = CreateChild("WorldStateManager", systemsRoot.transform)
                .AddComponent<TimeLoopCity.World.WorldStateManager>();
            context.WorldStateManager = worldState;

            var eventSpawner = CreateChild("RandomEventSpawner", systemsRoot.transform)
                .AddComponent<TimeLoopCity.Events.RandomEventSpawner>();
            context.EventSpawner = eventSpawner;

            ConfigureTimeOfDaySystem(context);
            ConfigureWeatherSystem(context);
            ConfigureWorldStateManager(context);
            ConfigureRandomEventSpawner(context);
        }

        private static void SetupNPCs(SceneBuildContext context)
        {
            var npcRoot = CreateChild("NPCs", context.Root.transform);
            var npcDefinitions = new[]
            {
                new NPCDefinition("npc_city_guard", "City Guard", new Vector3(-5f, 0f, -5f), new []
                {
                    Vector3.zero,
                    new Vector3(-5f, 0f, 10f),
                    new Vector3(5f, 0f, 10f),
                }),
                new NPCDefinition("npc_reporter", "Loop Reporter", new Vector3(12f, 0f, 8f), new []
                {
                    Vector3.zero,
                    new Vector3(-6f, 0f, -4f),
                    new Vector3(-10f, 0f, 2f),
                }),
                new NPCDefinition("npc_vendor", "Street Vendor", new Vector3(-12f, 0f, 6f), new []
                {
                    Vector3.zero,
                    new Vector3(4f, 0f, 4f),
                    new Vector3(-4f, 0f, 6f),
                })
            };

            foreach (var npcDef in npcDefinitions)
            {
                var npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
                npc.name = npcDef.DisplayName;
                npc.transform.SetParent(npcRoot.transform);
                npc.transform.position = npcDef.StartPosition + Vector3.up * 1f;
                TintRenderer(npc.GetComponent<MeshRenderer>(), new Color(0.95f, 0.75f, 0.2f));

                var collider = npc.GetComponent<Collider>();
                if (collider != null)
                {
                    collider.isTrigger = false;
                }

                var agent = npc.AddComponent<NavMeshAgent>();
                agent.speed = 3.5f;
                agent.angularSpeed = 240f;
                agent.acceleration = 12f;

                var controller = npc.AddComponent<TimeLoopCity.AI.NPCController>();
                AssignNpcData(controller, npcDef);
            }
        }

        private static void SetupUI(SceneBuildContext context)
        {
            var canvasObj = new GameObject("MainCityUI");
            canvasObj.transform.SetParent(context.Root.transform);
            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            canvasObj.AddComponent<GraphicRaycaster>();
            context.UiRoot = canvasObj;

            EnsureEventSystemExists();

            var hudPanel = CreateUIElement("TimeLoopHUD", canvasObj.transform);
            var hudRect = hudPanel.GetComponent<RectTransform>();
            hudRect.anchorMin = new Vector2(0f, 1f);
            hudRect.anchorMax = new Vector2(0f, 1f);
            hudRect.pivot = new Vector2(0f, 1f);
            hudRect.anchoredPosition = new Vector2(30f, -30f);
            hudRect.sizeDelta = new Vector2(340f, 160f);

            var hudBackground = hudPanel.AddComponent<Image>();
            hudBackground.color = new Color(0f, 0f, 0f, 0.45f);
            hudBackground.raycastTarget = false;

            var timeLabel = CreateText("TimeLabel", hudPanel.transform, "TIME OF DAY", 20, FontStyles.UpperCase);
            PositionRect(timeLabel.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, -10f), new Vector2(300f, 24f));

            var timeValue = CreateText("TimeValue", hudPanel.transform, "8:00 AM", 36, FontStyles.Bold);
            timeValue.alignment = TextAlignmentOptions.Left;
            PositionRect(timeValue.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, -50f), new Vector2(320f, 40f));

            var loopValue = CreateText("LoopValue", hudPanel.transform, "Loop: 0", 24, FontStyles.Normal);
            PositionRect(loopValue.rectTransform, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(0f, -90f), new Vector2(320f, 32f));

            var progressRoot = CreateUIElement("LoopProgressBar", hudPanel.transform);
            var progressRect = progressRoot.GetComponent<RectTransform>();
            progressRect.anchorMin = new Vector2(0f, 0f);
            progressRect.anchorMax = new Vector2(0f, 0f);
            progressRect.pivot = new Vector2(0f, 0f);
            progressRect.anchoredPosition = new Vector2(10f, 20f);
            progressRect.sizeDelta = new Vector2(300f, 20f);

            var progressBg = progressRoot.AddComponent<Image>();
            progressBg.color = new Color(1f, 1f, 1f, 0.15f);
            progressBg.raycastTarget = false;

            var fill = CreateUIElement("Fill", progressRoot.transform);
            var fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.one * 2f;
            fillRect.offsetMax = -Vector2.one * 2f;

            var fillImage = fill.AddComponent<Image>();
            fillImage.type = Image.Type.Filled;
            fillImage.fillMethod = Image.FillMethod.Horizontal;
            fillImage.fillAmount = 0.5f;
            fillImage.color = new Color(0.2f, 0.8f, 1f, 0.9f);
            fillImage.raycastTarget = false;

            var timeDisplay = hudPanel.AddComponent<TimeLoopCity.UI.UI_TimeDisplay>();
            var timeDisplaySO = new SerializedObject(timeDisplay);
            AssignReference(timeDisplaySO, "timeText", timeValue);
            AssignReference(timeDisplaySO, "loopCountText", loopValue);
            AssignReference(timeDisplaySO, "timeProgressBar", fillImage);
            timeDisplaySO.ApplyModifiedPropertiesWithoutUndo();

            var fadeRoot = CreateUIElement("FadeUI", canvasObj.transform);
            var fadeRect = fadeRoot.GetComponent<RectTransform>();
            fadeRect.anchorMin = Vector2.zero;
            fadeRect.anchorMax = Vector2.one;
            fadeRect.offsetMin = Vector2.zero;
            fadeRect.offsetMax = Vector2.zero;

            var fadeImage = fadeRoot.AddComponent<Image>();
            fadeImage.color = new Color(0f, 0f, 0f, 0f);
            fadeImage.raycastTarget = false;

            var fadeCanvasGroup = fadeRoot.AddComponent<CanvasGroup>();

            var fadeTransition = fadeRoot.AddComponent<TimeLoopCity.UI.LoopResetTransition>();
            var fadeSO = new SerializedObject(fadeTransition);
            AssignReference(fadeSO, "fadeImage", fadeImage);
            AssignReference(fadeSO, "canvasGroup", fadeCanvasGroup);
            fadeSO.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void SetupExampleClue(SceneBuildContext context)
        {
            var clue = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clue.name = "Clue_AbandonedWatch";
            clue.transform.SetParent(context.Root.transform);
            clue.transform.position = new Vector3(6f, 0.5f, 4f);
            clue.transform.localScale = new Vector3(0.6f, 0.2f, 0.6f);
            TintRenderer(clue.GetComponent<MeshRenderer>(), new Color(0.9f, 0.85f, 0.2f));

            var interactable = clue.AddComponent<TimeLoopCity.Player.InteractableObject>();
            var interactableSO = new SerializedObject(interactable);
            AssignString(interactableSO, "interactionPrompt", "Inspect Watch");
            AssignBool(interactableSO, "isClue", true);
            AssignString(interactableSO, "clueId", "clue_abandoned_watch");
            interactableSO.ApplyModifiedPropertiesWithoutUndo();
        }

        #region Helper Methods

        private static void ConfigureTimeOfDaySystem(SceneBuildContext context)
        {
            var so = new SerializedObject(context.TimeOfDaySystem);
            AssignReference(so, "directionalLight", context.Sun);

            var gradientProp = so.FindProperty("lightColorGradient");
            if (gradientProp != null)
            {
                var gradient = new Gradient();
                gradient.colorKeys = new[]
                {
                    new GradientColorKey(new Color(0.2f, 0.2f, 0.5f), 0f),
                    new GradientColorKey(new Color(1f, 0.85f, 0.6f), 0.3f),
                    new GradientColorKey(new Color(1f, 1f, 0.9f), 0.5f),
                    new GradientColorKey(new Color(0.9f, 0.4f, 0.3f), 0.8f)
                };
                gradient.alphaKeys = new[]
                {
                    new GradientAlphaKey(0.15f, 0f),
                    new GradientAlphaKey(1f, 0.4f),
                    new GradientAlphaKey(0.4f, 1f)
                };
                gradientProp.gradientValue = gradient;
            }

            var curveProp = so.FindProperty("lightIntensityCurve");
            if (curveProp != null)
            {
                var curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
                curveProp.animationCurveValue = curve;
            }

            var skybox = RenderSettings.skybox;
            if (skybox == null)
            {
                skybox = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Skybox.mat");
                RenderSettings.skybox = skybox;
            }
            AssignReference(so, "skyboxMaterial", skybox);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureWeatherSystem(SceneBuildContext context)
        {
            var parent = context.WeatherSystem.transform;
            var rain = CreateWeatherEffect("RainEffect", parent, new Color(0.6f, 0.7f, 1f, 0.8f), 0.05f);
            var fog = CreateWeatherEffect("FogEffect", parent, new Color(0.85f, 0.85f, 0.9f, 0.4f), 0.3f);
            var snow = CreateWeatherEffect("SnowEffect", parent, new Color(1f, 1f, 1f, 0.9f), 0.08f);

            var so = new SerializedObject(context.WeatherSystem);
            AssignReference(so, "rainEffect", rain);
            AssignReference(so, "fogEffect", fog);
            AssignReference(so, "snowEffect", snow);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureWorldStateManager(SceneBuildContext context)
        {
            var so = new SerializedObject(context.WorldStateManager);

            var presets = so.FindProperty("statePresets");
            if (presets != null)
            {
                presets.arraySize = 2;
                var preset0 = presets.GetArrayElementAtIndex(0);
                AssignInt(preset0, "loopNumber", 0);
                AssignEnum(preset0, "weatherType", (int)TimeLoopCity.World.WeatherType.Sunny);
                AssignString(preset0, "description", "Bright launch morning");

                var preset1 = presets.GetArrayElementAtIndex(1);
                AssignInt(preset1, "loopNumber", 3);
                AssignEnum(preset1, "weatherType", (int)TimeLoopCity.World.WeatherType.Stormy);
                AssignString(preset1, "description", "Storm warning loop");
            }

            var randomizable = so.FindProperty("randomizableObjects");
            if (randomizable != null)
            {
                randomizable.arraySize = context.Buildings.Count;
                for (int i = 0; i < context.Buildings.Count; i++)
                {
                    randomizable.GetArrayElementAtIndex(i).objectReferenceValue = context.Buildings[i];
                }
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureRandomEventSpawner(SceneBuildContext context)
        {
            var prefabsRoot = CreateChild("EventPrefabs", context.Root.transform);
            prefabsRoot.SetActive(false);

            var firePrefab = CreateEventPrefab("FireEventPrefab", PrimitiveType.Cylinder, new Color(1f, 0.4f, 0.1f), prefabsRoot.transform);
            var accidentPrefab = CreateEventPrefab("AccidentEventPrefab", PrimitiveType.Cube, new Color(0.7f, 0.1f, 0.1f), prefabsRoot.transform);
            var robberyPrefab = CreateEventPrefab("RobberyEventPrefab", PrimitiveType.Capsule, new Color(0.2f, 0.2f, 0.2f), prefabsRoot.transform);
            var blackoutPrefab = CreateEventPrefab("BlackoutEventPrefab", PrimitiveType.Sphere, new Color(0f, 0f, 0f), prefabsRoot.transform);
            var paradePrefab = CreateEventPrefab("ParadeEventPrefab", PrimitiveType.Capsule, new Color(0.8f, 0.2f, 0.8f), prefabsRoot.transform);

            var so = new SerializedObject(context.EventSpawner);

            AssignReference(so, "firePrefab", firePrefab);
            AssignReference(so, "accidentPrefab", accidentPrefab);
            AssignReference(so, "robberyPrefab", robberyPrefab);
            AssignReference(so, "blackoutPrefab", blackoutPrefab);
            AssignReference(so, "paradePrefab", paradePrefab);
            AssignInt(so, "maxEventsPerLoop", 3);
            AssignFloat(so, "eventChance", 0.8f);

            var spawnPoints = new[]
            {
                (new Vector3(-15f, 0f, -15f), "Old Town"),
                (new Vector3(14f, 0f, -6f), "Finance Plaza"),
                (new Vector3(-6f, 0f, 16f), "Harbor Approach"),
                (new Vector3(10f, 0f, 12f), "Festival Grounds")
            };

            var pointsProp = so.FindProperty("eventSpawnPoints");
            if (pointsProp != null)
            {
                pointsProp.arraySize = spawnPoints.Length;
                for (int i = 0; i < spawnPoints.Length; i++)
                {
                    var element = pointsProp.GetArrayElementAtIndex(i);
                    AssignVector3(element, "position", spawnPoints[i].Item1);
                    AssignString(element, "locationName", spawnPoints[i].Item2);
                }
            }

            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void AssignNpcData(TimeLoopCity.AI.NPCController controller, NPCDefinition definition)
        {
            var so = new SerializedObject(controller);
            AssignString(so, "npcId", definition.Id);
            AssignString(so, "npcName", definition.DisplayName);

            var defaultWaypoints = so.FindProperty("defaultWaypoints");
            if (defaultWaypoints != null)
            {
                defaultWaypoints.arraySize = definition.Waypoints.Length;
                for (int i = 0; i < definition.Waypoints.Length; i++)
                {
                    var waypoint = new GameObject($"{definition.DisplayName}_Waypoint_{i + 1}");
                    waypoint.transform.SetParent(controller.transform);
                    waypoint.transform.position = definition.StartPosition + definition.Waypoints[i];
                    defaultWaypoints.GetArrayElementAtIndex(i).objectReferenceValue = waypoint.transform;
                }
            }

            AssignBool(so, "randomizeRoutine", false);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static ParticleSystem CreateWeatherEffect(string name, Transform parent, Color startColor, float startSize)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;

            var system = go.AddComponent<ParticleSystem>();
            var main = system.main;
            main.startColor = startColor;
            main.startSize = startSize;
            main.startSpeed = 10f;
            main.maxParticles = 1000;
            main.duration = 5f;

            var shape = system.shape;
            shape.shapeType = ParticleSystemShapeType.Box;
            shape.scale = new Vector3(40f, 10f, 40f);

            system.Stop();
            return system;
        }

        private static GameObject CreateEventPrefab(string name, PrimitiveType type, Color color, Transform parent)
        {
            var prefab = GameObject.CreatePrimitive(type);
            prefab.name = name;
            prefab.transform.SetParent(parent);
            prefab.transform.localScale = Vector3.one * 0.75f;
            TintRenderer(prefab.GetComponent<MeshRenderer>(), color);
            Object.DestroyImmediate(prefab.GetComponent<Collider>());
            prefab.SetActive(false);
            return prefab;
        }

        private static void EnsureEventSystemExists()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;

            var eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        private static GameObject CreateChild(string name, Transform parent)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            return go;
        }

        private static TimeLoopCity.Core.ClueDatabase EnsureClueDatabaseAsset()
        {
            var folderPath = Path.GetDirectoryName(ClueDatabasePath)?.Replace('\\', '/');
            if (!string.IsNullOrEmpty(folderPath) && !AssetDatabase.IsValidFolder(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                AssetDatabase.Refresh();
            }

            var database = AssetDatabase.LoadAssetAtPath<TimeLoopCity.Core.ClueDatabase>(ClueDatabasePath);
            if (database == null)
            {
                database = ScriptableObject.CreateInstance<TimeLoopCity.Core.ClueDatabase>();
                AssetDatabase.CreateAsset(database, ClueDatabasePath);
                AssetDatabase.SaveAssets();
            }

            return database;
        }

        private static TextMeshProUGUI CreateText(string name, Transform parent, string text, int fontSize, FontStyles style)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.fontStyle = style;
            tmp.alignment = TextAlignmentOptions.Left;
            tmp.raycastTarget = false;
            return tmp;
        }

        private static GameObject CreateUIElement(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go;
        }

        private static void PositionRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot, Vector2 anchoredPosition, Vector2 sizeDelta)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;
            rect.anchoredPosition = anchoredPosition;
            rect.sizeDelta = sizeDelta;
        }

        private static T CreateManager<T>(string name, Transform parent) where T : Component
        {
            var go = new GameObject(name);
            if (parent != null)
            {
                go.transform.SetParent(parent);
            }
            return go.AddComponent<T>();
        }

        private static void TintRenderer(Renderer renderer, Color color)
        {
            if (renderer == null) return;

            var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
            var mat = new Material(shader) { color = color };
            renderer.sharedMaterial = mat;
        }

        private static void AssignReference(SerializedObject so, string propertyName, Object value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null) return;
            prop.objectReferenceValue = value;
        }

        private static void AssignReference(SerializedProperty parent, string propertyName, Object value)
        {
            var prop = parent.FindPropertyRelative(propertyName);
            if (prop == null) return;
            prop.objectReferenceValue = value;
        }

        private static void AssignString(SerializedObject so, string propertyName, string value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null) return;
            prop.stringValue = value;
        }

        private static void AssignString(SerializedProperty parent, string propertyName, string value)
        {
            var prop = parent.FindPropertyRelative(propertyName);
            if (prop == null) return;
            prop.stringValue = value;
        }

        private static void AssignBool(SerializedObject so, string propertyName, bool value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null) return;
            prop.boolValue = value;
        }

        private static void AssignInt(SerializedObject so, string propertyName, int value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null) return;
            prop.intValue = value;
        }

        private static void AssignInt(SerializedProperty parent, string propertyName, int value)
        {
            var prop = parent.FindPropertyRelative(propertyName);
            if (prop == null) return;
            prop.intValue = value;
        }

        private static void AssignFloat(SerializedObject so, string propertyName, float value)
        {
            var prop = so.FindProperty(propertyName);
            if (prop == null) return;
            prop.floatValue = value;
        }

        private static void AssignEnum(SerializedProperty parent, string propertyName, int enumValue)
        {
            var prop = parent.FindPropertyRelative(propertyName);
            if (prop == null) return;
            prop.enumValueIndex = enumValue;
        }

        private static void AssignVector3(SerializedProperty parent, string propertyName, Vector3 value)
        {
            var prop = parent.FindPropertyRelative(propertyName);
            if (prop == null) return;
            prop.vector3Value = value;
        }
        #endregion

        private class SceneBuildContext
        {
            public GameObject Root;
            public Light Sun;
            public List<GameObject> Buildings = new List<GameObject>();
            public Transform PlayerSpawn;
            public GameObject Player;
            public Transform CameraTarget;
            public GameObject ManagersRoot;
            public GameObject WorldSystemsRoot;
            public GameObject UiRoot;
            public TimeLoopCity.World.TimeOfDaySystem TimeOfDaySystem;
            public TimeLoopCity.World.WeatherSystem WeatherSystem;
            public TimeLoopCity.World.WorldStateManager WorldStateManager;
            public TimeLoopCity.TimeLoop.TimeLoopManager TimeLoopManager;
            public TimeLoopCity.Events.RandomEventSpawner EventSpawner;
        }

        private readonly struct NPCDefinition
        {
            public NPCDefinition(string id, string displayName, Vector3 startPosition, Vector3[] waypoints)
            {
                Id = id;
                DisplayName = displayName;
                StartPosition = startPosition;
                Waypoints = waypoints;
            }

            public string Id { get; }
            public string DisplayName { get; }
            public Vector3 StartPosition { get; }
            public Vector3[] Waypoints { get; }
        }
    }
}
#endif
