using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Gameplay
{
    /// <summary>
    /// Generates Kochi-specific missions and integrates them with the Time Loop system
    /// </summary>
    public class MissionGenerator : MonoBehaviour
    {
        [Header("Mission Spawn Points")]
        public Transform fortKochiZone;
        public Transform willingdonZone;
        public Transform marineDriveZone;
        public Transform mattancherryZone;

        public void GenerateKochiMissions()
        {
            GenerateFortKochiMystery();
            GenerateWillingdonHeist();
            GenerateMissingTourist();
            GenerateAbandonedBoat();
            
            Debug.Log("Kochi-specific missions generated!");
        }

        private void GenerateFortKochiMystery()
        {
            Vector3 noteLocation = new Vector3(-120f, 0.5f, 20f); // Fort Kochi
            
            GameObject clue = GameObject.CreatePrimitive(PrimitiveType.Cube);
            clue.name = "Clue_MysteryNote";
            clue.transform.position = noteLocation + Vector3.up * 0.5f;
            clue.transform.localScale = new Vector3(0.3f, 0.01f, 0.5f); // Flat note
            
            var interactable = clue.AddComponent<TimeLoopCity.Player.InteractableObject>();
            SetInteractableProperties(interactable, "clue_fort_mystery", "Read Mystery Note", 
                "A weathered note mentions a secret meeting at the old fishing nets...");
            
            // Tint yellow for clue
            TintObject(clue, new Color(0.9f, 0.85f, 0.2f));
        }

        private void GenerateWillingdonHeist()
        {
            Vector3 containerLocation = new Vector3(130f, 1f, -30f); // Willingdon Port
            
            GameObject container = GameObject.CreatePrimitive(PrimitiveType.Cube);
            container.name = "Clue_StolenContainer";
            container.transform.position = containerLocation;
            container.transform.localScale = new Vector3(2f, 2f, 6f);
            
            var interactable = container.AddComponent<TimeLoopCity.Player.InteractableObject>();
            SetInteractableProperties(interactable, "clue_willingdon_heist", "Inspect Container", 
                "Container #7742 - Reported stolen. Fresh tire marks nearby.");
            
            TintObject(container, Color.red);
        }

        private void GenerateMissingTourist()
        {
            Vector3 benchLocation = new Vector3(0f, 0.5f, 10f); // Marine Drive walkway
            
            GameObject bench = GameObject.CreatePrimitive(PrimitiveType.Cube);
            bench.name = "Clue_AbandonedBag";
            bench.transform.position = benchLocation;
            bench.transform.localScale = new Vector3(1f, 0.5f, 0.5f);
            
            var interactable = bench.AddComponent<TimeLoopCity.Player.InteractableObject>();
            SetInteractableProperties(interactable, "clue_missing_tourist", "Examine Bag", 
                "A tourist's bag left behind. Ticket stub shows: 'Sunset Cruise - 6 PM'.");
            
            TintObject(bench, new Color(0.8f, 0.6f, 0.3f));
        }

        private void GenerateAbandonedBoat()
        {
            Vector3 boatLocation = new Vector3(-100f, -0.3f, -40f); // Mattancherry waterfront
            
            GameObject boat = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            boat.name = "Clue_AbandonedBoat";
            boat.transform.position = boatLocation;
            boat.transform.rotation = Quaternion.Euler(0, 0, 90); // Horizontal
            boat.transform.localScale = new Vector3(2f, 5f, 2f);
            
            var interactable = boat.AddComponent<TimeLoopCity.Player.InteractableObject>();
            SetInteractableProperties(interactable, "clue_abandoned_boat", "Search Boat", 
                "Empty fishing boat. Name painted over. Smells of diesel.");
            
            TintObject(boat, new Color(0.4f, 0.3f, 0.2f));
        }

        private void SetInteractableProperties(TimeLoopCity.Player.InteractableObject interactable, 
            string clueId, string prompt, string description)
        {
#if UNITY_EDITOR
            var so = new UnityEditor.SerializedObject(interactable);
            so.FindProperty("clueId").stringValue = clueId;
            so.FindProperty("interactionPrompt").stringValue = prompt;
            so.FindProperty("isClue").boolValue = true;
            // Note: Description would need to be added to the ClueDatabase separately
            so.ApplyModifiedPropertiesWithoutUndo();
#endif
        }

        private void TintObject(GameObject obj, Color color)
        {
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                var mat = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                mat.color = color;
                renderer.sharedMaterial = mat;
            }
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Tools/Time Loop City/Generate Kochi Missions")]
        public static void GenerateMissionsFromMenu()
        {
            var generator = FindFirstObjectByType<MissionGenerator>();
            if (generator == null)
            {
                var go = new GameObject("MissionGenerator");
                generator = go.AddComponent<MissionGenerator>();
            }
            generator.GenerateKochiMissions();
        }
#endif
    }
}
