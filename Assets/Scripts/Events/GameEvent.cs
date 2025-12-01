using UnityEngine;

namespace TimeLoopCity.Events
{
    /// <summary>
    /// Base class for game events that occur during a loop.
    /// Events can be fires, accidents, robberies, etc.
    /// </summary>
    public abstract class GameEvent : MonoBehaviour
    {
        [SerializeField] protected string eventId;
        [SerializeField] protected string eventName;
        [SerializeField] protected Vector3 eventPosition;
        [SerializeField] protected float eventDuration = 30f;
        [SerializeField] protected float alertRadius = 20f;

        protected float eventTimer;
        protected bool isActive;

        protected virtual void Start()
        {
            eventPosition = transform.position;
            eventTimer = eventDuration;
            isActive = true;
            OnEventStart();
        }

        protected virtual void Update()
        {
            if (!isActive) return;

            eventTimer -= Time.deltaTime;
            if (eventTimer <= 0)
            {
                EndEvent();
            }

            UpdateEvent();
        }

        /// <summary>Called when event starts</summary>
        protected abstract void OnEventStart();

        /// <summary>Called each frame while event is active</summary>
        protected abstract void UpdateEvent();

        /// <summary>Called when event ends</summary>
        protected virtual void EndEvent()
        {
            isActive = false;
            OnEventEnd();
            Destroy(gameObject);
        }

        /// <summary>Called on event end, override for cleanup</summary>
        protected abstract void OnEventEnd();

        /// <summary>Notify NPCs in range</summary>
        protected void AlertNearbyNPCs()
        {
            Collider[] colliders = Physics.OverlapSphere(eventPosition, alertRadius);
            foreach (Collider col in colliders)
            {
                AI.NPCController npc = col.GetComponent<AI.NPCController>();
                if (npc != null)
                {
                    npc.OnEventDetected(eventName);
                }
            }
        }

        public string GetEventName() => eventName;
        public Vector3 GetEventPosition() => eventPosition;
        public bool IsActive => isActive;
    }

    /// <summary>Fire event - NPCs flee, fire visual/audio</summary>
    public class FireEvent : GameEvent
    {
        [SerializeField] private ParticleSystem fireParticles;
        [SerializeField] private Light fireLight;
        [SerializeField] private AudioClip fireSound;
        private AudioSource audioSource;

        protected override void OnEventStart()
        {
            Debug.Log($"[FireEvent] Fire started at {eventPosition}");
            AlertNearbyNPCs();

            if (fireParticles != null)
                fireParticles.Play();

            if (fireLight != null)
                fireLight.enabled = true;

            if (fireSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.clip = fireSound;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        protected override void UpdateEvent()
        {
            // Fire spreads or pulses
            if (fireLight != null)
            {
                fireLight.intensity = Mathf.Sin(Time.time * 2f) * 0.5f + 1.5f;
            }
        }

        protected override void OnEventEnd()
        {
            Debug.Log("[FireEvent] Fire extinguished");
            if (fireLight != null)
                fireLight.enabled = false;
            if (audioSource != null)
                Destroy(audioSource);
        }
    }

    /// <summary>Accident event - traffic noise, NPCs stop to watch</summary>
    public class AccidentEvent : GameEvent
    {
        [SerializeField] private AudioClip carCrashSound;
        [SerializeField] private AudioClip emergencySirenSound;
        private AudioSource audioSource;

        protected override void OnEventStart()
        {
            Debug.Log($"[AccidentEvent] Car accident at {eventPosition}");
            AlertNearbyNPCs();

            if (carCrashSound != null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.PlayOneShot(carCrashSound);
            }
        }

        protected override void UpdateEvent()
        {
            // Sirens wail in distance
        }

        protected override void OnEventEnd()
        {
            Debug.Log("[AccidentEvent] Scene cleared");
        }
    }

    /// <summary>Robbery event - fast audio, NPCs scared</summary>
    public class RobberyEvent : GameEvent
    {
        [SerializeField] private AudioClip alarmSound;

        protected override void OnEventStart()
        {
            Debug.Log($"[RobberyEvent] Robbery in progress at {eventPosition}");
            AlertNearbyNPCs();
        }

        protected override void UpdateEvent()
        {
        }

        protected override void OnEventEnd()
        {
            Debug.Log("[RobberyEvent] Robbery concluded");
        }
    }

    /// <summary>Blackout event - lights dim, fog increases</summary>
    public class BlackoutEvent : GameEvent
    {
        private float originalFogDensity;
        private Light[] lights;

        protected override void OnEventStart()
        {
            Debug.Log($"[BlackoutEvent] Blackout at {eventPosition}");
            AlertNearbyNPCs();

            originalFogDensity = RenderSettings.fogDensity;
            RenderSettings.fogDensity *= 2f;

            lights = FindObjectsByType<Light>(FindObjectsSortMode.None);
            foreach (Light light in lights)
            {
                if (Vector3.Distance(light.transform.position, eventPosition) < alertRadius)
                {
                    light.intensity *= 0.3f;
                }
            }
        }

        protected override void UpdateEvent()
        {
        }

        protected override void OnEventEnd()
        {
            Debug.Log("[BlackoutEvent] Power restored");
            RenderSettings.fogDensity = originalFogDensity;

            foreach (Light light in lights)
            {
                light.intensity *= (1f / 0.3f); // Restore
            }
        }
    }

    /// <summary>Parade event - festive music, NPCs gather</summary>
    public class ParadeEvent : GameEvent
    {
        [SerializeField] private AudioClip paradeMusic;

        protected override void OnEventStart()
        {
            Debug.Log($"[ParadeEvent] Parade starting at {eventPosition}");
            AlertNearbyNPCs();
        }

        protected override void UpdateEvent()
        {
        }

        protected override void OnEventEnd()
        {
            Debug.Log("[ParadeEvent] Parade ended");
        }
    }
}
