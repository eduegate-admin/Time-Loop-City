using UnityEngine;
using TimeLoopCity.Core;
using TimeLoopCity.TimeLoop;
using TimeLoopCity.UI;

namespace TimeLoopCity.Managers
{
    /// <summary>
    /// Manages the end game state and win condition.
    /// </summary>
    public class EndGameManager : MonoBehaviour
    {
        public static EndGameManager Instance { get; private set; }

        [Header("Win Condition")]
        [SerializeField] private string truthClueId = "Clue_TheTruth";
        
        private bool gameEnded = false;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        /// <summary>
        /// Attempt to trigger the ending.
        /// Called by a special dialogue or interaction.
        /// </summary>
        public void TryTriggerEnding()
        {
            if (gameEnded) return;

            if (PersistentClueSystem.Instance != null && PersistentClueSystem.Instance.HasClue(truthClueId))
            {
                StartEndingSequence();
            }
            else
            {
                Debug.Log("[EndGameManager] Missing the Truth. Cannot break the loop yet.");
                // Optional: Trigger "Not yet" dialogue
            }
        }

        private void StartEndingSequence()
        {
            gameEnded = true;
            Debug.Log("[EndGameManager] THE LOOP IS BROKEN.");

            // 1. Stop the Loop
            TimeLoopManager.Instance?.StopLoop();

            // 2. Play Cutscene
            StartCoroutine(PlayEndingCutscene());
        }

        private System.Collections.IEnumerator PlayEndingCutscene()
        {
            // Fade out UI
            if (Dialogue.DialogueManager.Instance != null) Dialogue.DialogueManager.Instance.EndDialogue();

            // Move Camera to Sky
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                float duration = 5f;
                float timer = 0f;
                Vector3 startPos = mainCam.transform.position;
                Quaternion startRot = mainCam.transform.rotation;
                
                // Target: Look up at the sky/city
                Vector3 targetPos = startPos + Vector3.up * 10f;
                Quaternion targetRot = Quaternion.LookRotation(Vector3.up);

                while (timer < duration)
                {
                    timer += Time.deltaTime;
                    float t = timer / duration;
                    // Smooth step
                    t = t * t * (3f - 2f * t);

                    mainCam.transform.position = Vector3.Lerp(startPos, targetPos, t);
                    mainCam.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
                    yield return null;
                }
            }

            // 3. Roll Credits
            CreditsUI.Instance?.RollCredits();
        }
    }
}
