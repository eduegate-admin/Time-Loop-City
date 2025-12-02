using UnityEngine;

namespace TimeLoopCity.Traffic
{
    public class TrafficWaypoint : MonoBehaviour
    {
        public Transform nextWaypoint;

        private void OnDrawGizmos()
        {
            if (nextWaypoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, nextWaypoint.position);
                Gizmos.DrawSphere(transform.position, 0.5f);
            }
        }
    }
}
