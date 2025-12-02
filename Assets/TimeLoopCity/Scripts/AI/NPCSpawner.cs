using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

namespace TimeLoopCity.AI
{
    [System.Serializable]
    public struct NPCSpawnData
    {
        public string npcId;
        public string npcName;
        public Vector3 spawnPosition;
        public List<Vector3> waypointPositions;
        public Material skinMaterial;
    }

    public class NPCSpawner : MonoBehaviour
    {
        [SerializeField] private List<NPCSpawnData> npcSpawns = new List<NPCSpawnData>();
        [SerializeField] private GameObject npcPrefab;
        [SerializeField] private bool autoSpawnOnStart = true;
        private List<GameObject> spawnedNPCs = new List<GameObject>();

        private void Start()
        {
            if (autoSpawnOnStart)
            {
                SpawnAllNPCs();
            }
        }

        public void SpawnAllNPCs()
        {
            ClearNPCs();
            foreach (var spawnData in npcSpawns)
            {
                SpawnNPC(spawnData);
            }
            Debug.Log($"[NPCSpawner] Spawned {spawnedNPCs.Count} NPCs");
        }

        private void SpawnNPC(NPCSpawnData data)
        {
            if (npcPrefab == null)
            {
                Debug.LogError("[NPCSpawner] NPC Prefab not assigned");
                return;
            }

            GameObject npcInstance = Instantiate(npcPrefab, data.spawnPosition, Quaternion.identity, transform);
            npcInstance.name = data.npcName;

            NPCController controller = npcInstance.GetComponent<NPCController>();
            if (controller != null)
            {
                var so = new UnityEditor.SerializedObject(controller);
                so.FindProperty("npcId").stringValue = data.npcId;
                so.FindProperty("npcName").stringValue = data.npcName;
                so.ApplyModifiedPropertiesWithoutUndo();
            }

            Transform waypointsParent = new GameObject("Waypoints").transform;
            waypointsParent.SetParent(npcInstance.transform);
            waypointsParent.localPosition = Vector3.zero;
            List<Transform> waypoints = new List<Transform>();

            foreach (var waypointPos in data.waypointPositions)
            {
                GameObject waypointObj = new GameObject("Waypoint");
                waypointObj.transform.SetParent(waypointsParent);
                waypointObj.transform.position = data.spawnPosition + waypointPos;
                waypoints.Add(waypointObj.transform);
            }

            if (controller != null && waypoints.Count > 0)
            {
                var so = new UnityEditor.SerializedObject(controller);
                var defaultWaypoints = so.FindProperty("defaultWaypoints");
                if (defaultWaypoints != null)
                {
                    defaultWaypoints.arraySize = waypoints.Count;
                    for (int i = 0; i < waypoints.Count; i++)
                    {
                        defaultWaypoints.GetArrayElementAtIndex(i).objectReferenceValue = waypoints[i];
                    }
                    so.ApplyModifiedPropertiesWithoutUndo();
                }
            }

            if (npcInstance.GetComponent<NavMeshAgent>() == null)
            {
                npcInstance.AddComponent<NavMeshAgent>();
            }

            if (data.skinMaterial != null)
            {
                var renderer = npcInstance.GetComponentInChildren<Renderer>();
                if (renderer != null)
                {
                    renderer.material = data.skinMaterial;
                }
            }

            spawnedNPCs.Add(npcInstance);
        }

        public void ClearNPCs()
        {
            foreach (GameObject npc in spawnedNPCs)
            {
                if (npc != null)
                {
                    Destroy(npc);
                }
            }
            spawnedNPCs.Clear();
        }

        public List<GameObject> GetSpawnedNPCs() => spawnedNPCs;
    }
}
