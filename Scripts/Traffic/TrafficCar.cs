using UnityEngine;

namespace TimeLoopCity.Traffic
{
    /// <summary>
    /// Simple vehicle logic. Moves forward and follows waypoints.
    /// Stops for obstacles (Player, other cars).
    /// </summary>
    public class TrafficCar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float detectionDistance = 5f;
        [SerializeField] private LayerMask obstacleLayer;

        private Transform currentWaypoint;
        private bool isStopped = false;

        public void SetWaypoint(Transform waypoint)
        {
            currentWaypoint = waypoint;
        }

        private void Update()
        {
            CheckForObstacles();
            Move();
        }

        private void CheckForObstacles()
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
            isStopped = Physics.Raycast(ray, detectionDistance, obstacleLayer);
            
            if (isStopped)
            {
                Debug.DrawRay(ray.origin, ray.direction * detectionDistance, Color.red);
            }
        }

        private void Move()
        {
            if (isStopped) return;
            if (currentWaypoint == null) return;

            // Move forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Rotate towards waypoint
            Vector3 direction = (currentWaypoint.position - transform.position).normalized;
            direction.y = 0; // Keep flat
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Check arrival
            if (Vector3.Distance(transform.position, currentWaypoint.position) < 2f)
            {
                GetNextWaypoint();
            }
        }

        private void GetNextWaypoint()
        {
            // Assume waypoint has a script with "Next" reference, or TrafficSystem handles it.
            // For simplicity in this generator, we'll look for a child or sibling.
            // Better: TrafficWaypoint component.
            
            TrafficWaypoint wp = currentWaypoint.GetComponent<TrafficWaypoint>();
using UnityEngine;

namespace TimeLoopCity.Traffic
{
    /// <summary>
    /// Simple vehicle logic. Moves forward and follows waypoints.
    /// Stops for obstacles (Player, other cars).
    /// </summary>
    public class TrafficCar : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float speed = 10f;
        [SerializeField] private float rotationSpeed = 5f;
        [SerializeField] private float detectionDistance = 5f;
        [SerializeField] private LayerMask obstacleLayer;

        private Transform currentWaypoint;
        private bool isStopped = false;

        public void SetWaypoint(Transform waypoint)
        {
            currentWaypoint = waypoint;
        }

        private void Update()
        {
            CheckForObstacles();
            Move();
        }

        private void CheckForObstacles()
        {
            Ray ray = new Ray(transform.position + Vector3.up * 0.5f, transform.forward);
            isStopped = Physics.Raycast(ray, detectionDistance, obstacleLayer);
            
            if (isStopped)
            {
                Debug.DrawRay(ray.origin, ray.direction * detectionDistance, Color.red);
            }
        }

        private void Move()
        {
            if (isStopped) return;
            if (currentWaypoint == null) return;

            // Move forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);

            // Rotate towards waypoint
            Vector3 direction = (currentWaypoint.position - transform.position).normalized;
            direction.y = 0; // Keep flat
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Check arrival
            if (Vector3.Distance(transform.position, currentWaypoint.position) < 2f)
            {
                GetNextWaypoint();
            }
        }

        private void GetNextWaypoint()
        {
            // Assume waypoint has a script with "Next" reference, or TrafficSystem handles it.
            // For simplicity in this generator, we'll look for a child or sibling.
            // Better: TrafficWaypoint component.
            
            TrafficWaypoint wp = currentWaypoint.GetComponent<TrafficWaypoint>();
            if (wp != null && wp.nextWaypoint != null)
            {
                currentWaypoint = wp.nextWaypoint;
            }
            else
            {
                // Return to pool instead of Destroy
                TrafficSystem.Instance?.ReturnCar(this);
            }
        }

        public void ResetCar()
        {
            isStopped = false;
            currentWaypoint = null;
        }
    }
}
