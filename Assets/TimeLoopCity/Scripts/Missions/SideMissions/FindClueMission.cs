// Simple side mission script for "Find the clue in Fort Kochi".
// Place under Assets/TimeLoopKochi/SideMissions/FindClueMission.cs
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "TimeLoopKochi/Missions/FindClueMission", fileName = "FindClueMission")] 
public class FindClueMission : ScriptableObject
{
    public string missionName = "Find the clue in Fort Kochi";
    public string description = "Explore Fort Kochi and locate the hidden historical clue.";
    public Vector3 cluePosition = new Vector3(10, 0, -15);
    public GameObject cluePrefab;
    public UnityEvent onClueFound;

    // Call this from a trigger when player reaches the clue area.
    public void AttemptComplete(Vector3 playerPos)
    {
        if (Vector3.Distance(playerPos, cluePosition) < 2f)
        {
            Debug.Log($"[Kochi Pack] Mission completed: {missionName}");
            onClueFound?.Invoke();
        }
    }
}
