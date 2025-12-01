using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.World
{
    /// <summary>
    /// Automates the creation of the city layout using primitives.
    /// Attach to an empty GameObject and click "Build City" in context menu.
    /// </summary>
    public class CityGenerator : MonoBehaviour
    {
        [Header("Materials")]
        [SerializeField] private Material asphaltMat;
        [SerializeField] private Material buildingMat;
        [SerializeField] private Material glassMat;
        [SerializeField] private Material parkMat;
        [SerializeField] private Material waterMat;
        [SerializeField] private Material propMat; // New material for props

        [ContextMenu("Build City Layout")]
        public void BuildCity()
        {
            // Clear existing
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }

            BuildDistricts();
            BuildGround();
            Debug.Log("[CityGenerator] City Layout Built!");
        }

        private void BuildGround()
        {
            CreatePrimitive(PrimitiveType.Plane, "CityGround", new Vector3(0, 0, 0), new Vector3(50, 1, 50), asphaltMat, transform);
        }

        private void BuildDistricts()
        {
            GameObject districtsRoot = new GameObject("Districts");
            districtsRoot.transform.parent = transform;

            BuildCentralDistrict(districtsRoot.transform);
            BuildWestDistrict(districtsRoot.transform);
            BuildEastDistrict(districtsRoot.transform);
            BuildNorthDistrict(districtsRoot.transform);
            
            BuildProps(districtsRoot.transform); // New props step
            
            BuildRoadsAndTraffic(districtsRoot.transform); // New Traffic step
        }

        private void BuildCentralDistrict(Transform parent)
        {
            GameObject root = new GameObject("District_Central");
            root.transform.parent = parent;

            // City Hall Base
            CreatePrimitive(PrimitiveType.Cube, "CityHall_Base", new Vector3(0, 5, 25), new Vector3(12, 10, 8), buildingMat, root.transform);
            // Tower
            CreatePrimitive(PrimitiveType.Cube, "CityHall_Tower", new Vector3(0, 15, 28), new Vector3(4, 20, 4), buildingMat, root.transform);
            // Clock
            CreatePrimitive(PrimitiveType.Cylinder, "ClockFace", new Vector3(0, 22, 26), new Vector3(3, 0.1f, 3), glassMat, root.transform).transform.rotation = Quaternion.Euler(90, 0, 0);

            // Fountain
            CreatePrimitive(PrimitiveType.Cylinder, "Fountain_Base", new Vector3(0, 0.2f, 0), new Vector3(6, 0.4f, 6), buildingMat, root.transform);
            CreatePrimitive(PrimitiveType.Cylinder, "Fountain_Water", new Vector3(0, 0.5f, 0), new Vector3(5, 0.1f, 5), waterMat, root.transform);

            // Player Apartment (Start Point)
            GameObject apartment = new GameObject("Player_Apartment");
            apartment.transform.parent = root.transform;
            apartment.transform.position = new Vector3(0, 0, -15);
            
            // Floor
            CreatePrimitive(PrimitiveType.Cube, "Floor", new Vector3(0, 0.1f, -15), new Vector3(8, 0.2f, 8), buildingMat, apartment.transform);
            // Walls
            CreatePrimitive(PrimitiveType.Cube, "Wall_Back", new Vector3(0, 2, -11), new Vector3(8, 4, 0.5f), buildingMat, apartment.transform);
            CreatePrimitive(PrimitiveType.Cube, "Wall_Left", new Vector3(-4, 2, -15), new Vector3(0.5f, 4, 8), buildingMat, apartment.transform);
            CreatePrimitive(PrimitiveType.Cube, "Wall_Right", new Vector3(4, 2, -15), new Vector3(0.5f, 4, 8), buildingMat, apartment.transform);
            // Bed
            CreatePrimitive(PrimitiveType.Cube, "Bed", new Vector3(-2, 0.5f, -13), new Vector3(2, 0.5f, 3), propMat, apartment.transform);
            // Table with Radio
            CreatePrimitive(PrimitiveType.Cube, "Table", new Vector3(2, 0.8f, -12), new Vector3(2, 0.8f, 1), propMat, apartment.transform);

            // Newspaper Clue (On the table)
            GameObject newspaper = CreatePrimitive(PrimitiveType.Cube, "Clue_Newspaper", new Vector3(2, 0.85f, -12), new Vector3(0.5f, 0.05f, 0.4f), propMat, apartment.transform);
            var clueComp = newspaper.AddComponent<Player.ClueInteractable>();
            // We need to set fields via reflection or serialized object in editor, 
            // but for runtime generation without prefabs, we can try to set public properties if we expose them, 
            // or just rely on the component being there and manual setup in Inspector for now.
            // *Extreme Mode Optimization*: In a real pipeline, we'd use a Prefab. 
            // For now, we'll leave it as a placeholder for the user to configure the text.
        }

        private void BuildWestDistrict(Transform parent)
        {
            GameObject root = new GameObject("District_West");
            root.transform.parent = parent;

            // Cafe
            CreatePrimitive(PrimitiveType.Cube, "Building_Cafe", new Vector3(-15, 3, 5), new Vector3(10, 6, 10), buildingMat, root.transform);
            // Tech Shop
            CreatePrimitive(PrimitiveType.Cube, "Building_TechShop", new Vector3(-15, 4, -10), new Vector3(10, 8, 12), glassMat, root.transform);
        }

        private void BuildEastDistrict(Transform parent)
        {
            GameObject root = new GameObject("District_East");
            root.transform.parent = parent;

            // Power Station
            CreatePrimitive(PrimitiveType.Cylinder, "PowerStation_Reactor", new Vector3(20, 4, 5), new Vector3(8, 8, 8), buildingMat, root.transform);
            // Warehouse
            CreatePrimitive(PrimitiveType.Cube, "Warehouse", new Vector3(20, 5, -15), new Vector3(12, 10, 15), buildingMat, root.transform);
        }

        private void BuildNorthDistrict(Transform parent)
        {
            GameObject root = new GameObject("District_North");
            root.transform.parent = parent;

            // Park Ground
            CreatePrimitive(PrimitiveType.Plane, "Park_Grass", new Vector3(0, 0.02f, 40), new Vector3(2, 1, 2), parkMat, root.transform); // Plane scale is 10x
            
            // Statue
            CreatePrimitive(PrimitiveType.Cube, "Statue_Pedestal", new Vector3(0, 1, 40), new Vector3(2, 2, 2), buildingMat, root.transform);
            CreatePrimitive(PrimitiveType.Capsule, "Statue", new Vector3(0, 3, 40), new Vector3(1, 2, 1), glassMat, root.transform);
        }

        private void BuildProps(Transform parent)
        {
            GameObject root = new GameObject("City_Props");
            root.transform.parent = parent;

            // Vending Machines
            CreateProp(PrimitiveType.Cube, "VendingMachine_1", new Vector3(-8, 1, 5), new Vector3(1, 2, 1), propMat, root.transform);
            CreateProp(PrimitiveType.Cube, "VendingMachine_2", new Vector3(8, 1, -5), new Vector3(1, 2, 1), propMat, root.transform);

            // Trash Cans (Potential clue spots)
            CreateProp(PrimitiveType.Cylinder, "TrashCan_1", new Vector3(5, 0.5f, 5), new Vector3(0.6f, 1, 0.6f), propMat, root.transform);
            CreateProp(PrimitiveType.Cylinder, "TrashCan_2", new Vector3(-5, 0.5f, -5), new Vector3(0.6f, 1, 0.6f), propMat, root.transform);
        }

        private void CreateProp(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
        {
            GameObject obj = CreatePrimitive(type, name, pos, scale, mat, parent);
            
            // Add InteractableObject component
            var interactable = obj.AddComponent<Player.InteractableObject>();
            // We can't easily set private fields via AddComponent without reflection or a public Init method.
            // For this generator, we'll just add the component. 
            // In a real scenario, we'd use a Prefab.
            
            // Tag it so InteractionManager can find it (if using tags, though we use layers)
            obj.layer = LayerMask.NameToLayer("Default"); // Should be "Interactable" in real setup
        }

        private void BuildRoadsAndTraffic(Transform parent)
        {
            GameObject roadsRoot = new GameObject("Roads");
            roadsRoot.transform.parent = parent;

            // Simple Loop Road: Central -> West -> North -> East -> Central
            // Coordinates roughly: (0,0,0) -> (-15,0,0) -> (0,0,40) -> (20,0,0) -> (0,0,0)
            
            // Create Road Visuals (Black Planes)
            CreatePrimitive(PrimitiveType.Plane, "Road_MainLoop", new Vector3(0, 0.05f, 10), new Vector3(6, 1, 6), asphaltMat, roadsRoot.transform);

            // Create Traffic Waypoints
            GameObject trafficRoot = new GameObject("Traffic_System");
            trafficRoot.transform.parent = parent;
            
            // Ensure TrafficSystem exists
            var trafficSys = trafficRoot.AddComponent<Traffic.TrafficSystem>();

            // Create Waypoints in a loop
            var wp1 = CreateTrafficWaypoint(new Vector3(0, 0, -10), trafficRoot.transform);
            var wp2 = CreateTrafficWaypoint(new Vector3(-20, 0, 5), trafficRoot.transform);
            var wp3 = CreateTrafficWaypoint(new Vector3(-10, 0, 30), trafficRoot.transform);
            var wp4 = CreateTrafficWaypoint(new Vector3(20, 0, 30), trafficRoot.transform);
            var wp5 = CreateTrafficWaypoint(new Vector3(25, 0, 0), trafficRoot.transform);

            // Link them
            wp1.nextWaypoint = wp2.transform;
            wp2.nextWaypoint = wp3.transform;
            wp3.nextWaypoint = wp4.transform;
            wp4.nextWaypoint = wp5.transform;
            wp5.nextWaypoint = wp1.transform; // Loop back

            // Register spawn point
            trafficSys.RegisterSpawnPoint(wp1);
        }

        private Traffic.TrafficWaypoint CreateTrafficWaypoint(Vector3 pos, Transform parent)
        {
            GameObject obj = new GameObject("Traffic_WP");
            obj.transform.position = pos;
            obj.transform.parent = parent;
            return obj.AddComponent<Traffic.TrafficWaypoint>();
        }

        private GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
        {
            GameObject obj = GameObject.CreatePrimitive(type);
            obj.name = name;
            obj.transform.position = pos;
            obj.transform.localScale = scale;
            obj.transform.parent = parent;
            
            if (mat != null)
            {
                obj.GetComponent<Renderer>().material = mat;
            }

            // Ensure static for NavMesh
            obj.isStatic = true;

            return obj;
        }
    }
}
