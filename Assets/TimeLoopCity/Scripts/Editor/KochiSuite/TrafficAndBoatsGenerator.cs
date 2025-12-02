using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Unity.AI.Navigation;

namespace TimeLoopCity.Editor.KochiSuite
{
    /// <summary>
    /// Traffic and boats generator - traffic spawners, boat routes, metro system, NavMesh
    /// </summary>
    public class TrafficAndBoatsGenerator : KochiGeneratorBase
    {
        private int trafficSpawnerCount = 10;
        private int boatRouteCount = 3;
        private int metroStationCount = 5;
        private bool bakeNavMesh = true;

        public override void DrawGUI()
        {
            EditorGUILayout.LabelField("Traffic & Boats Settings", EditorStyles.boldLabel);
            trafficSpawnerCount = EditorGUILayout.IntSlider("Traffic Spawners", trafficSpawnerCount, 1, 50);
            boatRouteCount = EditorGUILayout.IntSlider("Boat Routes", boatRouteCount, 1, 10);
            metroStationCount = EditorGUILayout.IntSlider("Metro Stations", metroStationCount, 1, 15);
            bakeNavMesh = EditorGUILayout.Toggle("Bake NavMesh", bakeNavMesh);

            EditorGUILayout.HelpBox(
                "• Traffic Spawners: AI vehicle spawning points\n" +
                "• Boat Routes: Ferry and cargo boat paths\n" +
                "• Metro System: Train stations and tracks\n" +
                "• NavMesh: NPC pathfinding surface",
                MessageType.Info);
        }

        public void GenerateTraffic()
        {
            isRunning = true;
            progress = 0f;

            try
            {
                GenerateTrafficSpawners();
                SetProgress(0.25f);

                GenerateBoatRoutes();
                SetProgress(0.5f);

                GenerateMetroSystem();
                SetProgress(0.75f);

                if (bakeNavMesh)
                {
                    BakeNPCNavigation();
                    SetProgress(0.95f);
                }

                LogSuccess("Traffic and boats generation completed");
                SetProgress(1f);
            }
            catch (System.Exception e)
            {
                LogError($"Traffic generation failed: {e.Message}");
            }
            finally
            {
                isRunning = false;
            }
        }

        private void GenerateTrafficSpawners()
        {
            EnsureFolder("Assets/TimeLoopKochi/Traffic");

            GameObject trafficRoot = FindOrCreateRoot("TrafficSystem");
            GameObject spawnersRoot = new GameObject("TrafficSpawners");
            spawnersRoot.transform.parent = trafficRoot.transform;

            for (int i = 0; i < trafficSpawnerCount; i++)
            {
                Vector3 spawnPos = new Vector3(
                    Random.Range(-200f, 200f),
                    1f,
                    Random.Range(-200f, 200f)
                );

                GameObject spawner = new GameObject($"TrafficSpawner_{i}");
                spawner.transform.parent = spawnersRoot.transform;
                spawner.transform.position = spawnPos;

                var marker = spawner.AddComponent<BoxCollider>();
                marker.size = new Vector3(2f, 1f, 2f);
                marker.isTrigger = true;

                EnsureTag("TrafficSpawner");
                spawner.tag = "TrafficSpawner";
            }

            LogSuccess($"Generated {trafficSpawnerCount} traffic spawners");
        }

        private void GenerateBoatRoutes()
        {
            EnsureFolder("Assets/TimeLoopKochi/Water");

            GameObject boatsRoot = FindOrCreateRoot("BoatSystem");
            GameObject routesRoot = new GameObject("BoatRoutes");
            routesRoot.transform.parent = boatsRoot.transform;

            for (int r = 0; r < boatRouteCount; r++)
            {
                GameObject route = new GameObject($"BoatRoute_{r}");
                route.transform.parent = routesRoot.transform;

                int waypointCount = Random.Range(3, 8);
                for (int w = 0; w < waypointCount; w++)
                {
                    GameObject waypoint = new GameObject($"Waypoint_{w}");
                    waypoint.transform.parent = route.transform;
                    waypoint.transform.position = new Vector3(
                        Random.Range(-150f, 150f),
                        0f,
                        Random.Range(-150f, 150f)
                    );
                }
            }

            LogSuccess($"Generated {boatRouteCount} boat routes with waypoints");
        }

        private void GenerateMetroSystem()
        {
            EnsureFolder("Assets/TimeLoopKochi/Transit");

            GameObject transitRoot = FindOrCreateRoot("TransitSystem");
            GameObject stationsRoot = new GameObject("MetroStations");
            stationsRoot.transform.parent = transitRoot.transform;

            for (int i = 0; i < metroStationCount; i++)
            {
                Vector3 stationPos = new Vector3(
                    (i % 3) * 80f - 80f,
                    0f,
                    (i / 3) * 80f - 80f
                );

                GameObject station = new GameObject($"MetroStation_{i}");
                station.transform.parent = stationsRoot.transform;
                station.transform.position = stationPos;

                var platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
                platform.name = "Platform";
                platform.transform.parent = station.transform;
                platform.transform.localScale = new Vector3(10f, 0.5f, 20f);
                platform.transform.localPosition = Vector3.zero;

                Object.DestroyImmediate(platform.GetComponent<Collider>());

                var renderer = platform.GetComponent<Renderer>();
                var stationMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
                stationMat.color = new Color(0.3f, 0.3f, 0.3f);
                renderer.material = stationMat;

                var shelter = GameObject.CreatePrimitive(PrimitiveType.Cube);
                shelter.name = "Shelter";
                shelter.transform.parent = station.transform;
                shelter.transform.localScale = new Vector3(12f, 3f, 2f);
                shelter.transform.localPosition = new Vector3(0f, 1.5f, -8f);

                Object.DestroyImmediate(shelter.GetComponent<Collider>());

                var shelterRenderer = shelter.GetComponent<Renderer>();
                shelterRenderer.material = stationMat;

                EnsureTag("MetroStation");
                station.tag = "MetroStation";
            }

            LogSuccess($"Generated {metroStationCount} metro stations");
        }

        private void BakeNPCNavigation()
        {
            GameObject navmeshRoot = FindOrCreateRoot("Navigation");

            var navmeshSurface = navmeshRoot.AddComponent<NavMeshSurface>();
            if (navmeshSurface != null)
            {
                navmeshSurface.BuildNavMesh();
                LogSuccess("NavMesh baked for NPC pathfinding");
            }
            else
            {
                LogWarning("NavMeshSurface component not available (requires NavMesh Components)");
            }
        }
    }
}