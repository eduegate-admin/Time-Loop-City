using UnityEngine;
using System.Collections.Generic;
using TimeLoopCity.Core;

namespace TimeLoopCity.Traffic
{
    /// <summary>
    /// Manages traffic spawning and density.
    /// </summary>
    public class TrafficSystem : MonoBehaviour
    {
        public static TrafficSystem Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private GameObject carPrefab; // In generator, we might create this at runtime
        [SerializeField] private int maxCars = 10;
        [SerializeField] private float spawnInterval = 3f;

        [Header("Waypoints")]
        [SerializeField] private List<TrafficWaypoint> spawnPoints = new List<TrafficWaypoint>();

        private float timer;
        private int currentCars;
        private ObjectPool<TrafficCar> carPool;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            InitializePool();
        }

        private void InitializePool()
        {
            if (carPrefab == null)
            {
                // Create a temporary prefab for the pool if none assigned
                GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Cube);
                temp.AddComponent<TrafficCar>();
                temp.SetActive(false);
                carPool = new ObjectPool<TrafficCar>(temp.GetComponent<TrafficCar>(), maxCars, transform);
                Destroy(temp); // Destroy the template, pool has copies
            }
            else
            {
                carPool = new ObjectPool<TrafficCar>(carPrefab.GetComponent<TrafficCar>(), maxCars, transform);
            }
        }

        private void Update()
        {
            if (spawnPoints.Count == 0) return;

            timer += Time.deltaTime;
            if (timer >= spawnInterval && currentCars < maxCars)
            {
                SpawnCar();
                timer = 0f;
            }
        }

        private void SpawnCar()
        {
            if (carPool == null) return;

            TrafficWaypoint spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            
            TrafficCar carScript = carPool.Get();
            carScript.transform.position = spawnPoint.transform.position;
            carScript.transform.rotation = spawnPoint.transform.rotation;
            carScript.ResetCar();
            carScript.SetWaypoint(spawnPoint.nextWaypoint);

            currentCars++;
        }

        public void ReturnCar(TrafficCar car)
        {
            if (car == null || carPool == null) return;

            carPool.Return(car);
            currentCars = Mathf.Max(0, currentCars - 1);
        }

        public void RegisterSpawnPoint(TrafficWaypoint wp)
        {
            if (wp != null && !spawnPoints.Contains(wp))
            {
                spawnPoints.Add(wp);
            }
        }
    }
}
