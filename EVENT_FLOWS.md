# Time Loop City - Event Flow Diagrams

This document provides visual diagrams of key event flows in the Time Loop City system.

---

## 1. Game Initialization Flow

```
START GAME
    │
    ├─▶ Load Scene
    │
    ├─▶ Awake() Phase
    │   ├─▶ TimeLoopManager creates singleton
    │   ├─▶ WorldStateManager creates singleton
    │   ├─▶ SaveLoadManager creates singleton
    │   └─▶ PersistentClueSystem creates singleton
    │
    ├─▶ Start() Phase
    │   │
    │   ├─▶ SaveLoadManager.LoadGame()
    │   │   └─▶ TimeLoopManager.LoadClues()
    │   │
    │   ├─▶ Systems subscribe to events
    │   │   ├─▶ WorldStateManager → OnLoopReset
    │   │   ├─▶ RandomEventSpawner → OnLoopReset
    │   │   ├─▶ NPCController → OnLoopReset
    │   │   └─▶ LoopResetTransition → OnLoopReset
    │   │
    │   ├─▶ TimeLoopManager.StartLoop()
    │   │   └─▶ Fire OnLoopStart event
    │   │
    │   └─▶ WorldStateManager initializes first state
    │
    └─▶ Game Running
```

---

## 2. Loop Reset Flow (Core Mechanic)

```
LOOP TIMER EXPIRES (or manual reset)
    │
    ├─▶ TimeLoopManager.ResetLoop()
    │   │
    │   ├─▶ Set isResetting = true
    │   ├─▶ Increment loopCount
    │   ├─▶ Fire OnLoopCountChanged(loopCount)
    │   │
    │   └─▶ Fire OnLoopReset Event
    │       │
    │       ├─────────────────┬─────────────────┬─────────────────┐
    │       │                 │                 │                 │
    │       ▼                 ▼                 ▼                 ▼
    │   WorldStateManager  NPCController   RandomEventSpawner  LoopResetTransition
    │       │                 │                 │                 │
    │       │                 │                 │                 │
    │   ┌───▼───┐         ┌───▼───┐         ┌───▼───┐         ┌───▼───┐
    │   │Randomize│       │ Reset │         │Clear  │         │ Fade  │
    │   │ World   │       │Position│        │Events │         │Screen │
    │   │ State   │       │       │         │       │         │       │
    │   └───┬───┘         └───┬───┘         └───┬───┘         └───────┘
    │       │                 │                 │
    │       ├─▶ Weather       ├─▶ Reinit       └─▶ Spawn new
    │       │   Change        │   Routine           events
    │       │                 │
    │       └─▶ Time of Day   └─▶ Start patrol
    │           Reset
    │
    ├─▶ TimeLoopManager.TeleportPlayerToSpawn()
    │   └─▶ Player returns to start position
    │
    └─▶ Wait 1 second
        │
        └─▶ TimeLoopManager.StartLoop()
            ├─▶ currentLoopTime = 0
            ├─▶ isResetting = false
            └─▶ Fire OnLoopStart event
                │
                └─▶ Systems reinitialize
```

---

## 3. Player Interaction Flow

```
PLAYER APPROACHES OBJECT
    │
    ├─▶ Player presses E key
    │
    ├─▶ PlayerController.HandleInteraction()
    │   │
    │   ├─▶ Raycast from camera
    │   │
    │   └─▶ Hit InteractableObject?
    │       │
    │       ├─ NO ──▶ Nothing happens
    │       │
    │       └─ YES ──▶ InteractableObject.Interact()
    │                   │
    │                   ├─▶ Check canInteract flag
    │                   │   │
    │                   │   ├─ NO ──▶ Return
    │                   │   │
    │                   │   └─ YES ──▶ Continue
    │                   │
    │                   ├─▶ Is this a clue?
    │                   │   │
    │                   │   ├─ NO ──▶ OnInteract() (custom behavior)
    │                   │   │
    │                   │   └─ YES ──▶ PersistentClueSystem.DiscoverClue()
    │                   │               │
    │                   │               ├─▶ Add to discovered set
    │                   │               │
    │                   │               ├─▶ TimeLoopManager.AddClue()
    │                   │               │   │
    │                   │               │   └─▶ SaveLoadManager.SaveClues()
    │                   │               │       └─▶ Write to disk
    │                   │               │
    │                   │               └─▶ Show UI notification
    │                   │                   └─▶ "New Clue Discovered!"
    │                   │
    │                   └─▶ OnInteract() (custom behavior)
```

---

## 4. NPC AI Flow

```
NPC SPAWNED IN SCENE
    │
    ├─▶ Awake() - Get components
    │   ├─▶ NavMeshAgent
    │   └─▶ NPCMemoryComponent
    │
    ├─▶ Start()
    │   ├─▶ Subscribe to OnLoopReset
    │   └─▶ InitializeRoutine()
    │       │
    │       ├─▶ Get loop seed from TimeLoopManager
    │       ├─▶ Random.InitState(seed)
    │       ├─▶ Choose waypoint set (default or alternate)
    │       └─▶ Start patrol
    │
    ├─▶ Update() - Patrol Loop
    │   │
    │   ├─▶ Is waiting at waypoint?
    │   │   │
    │   │   ├─ YES ──▶ Decrease wait timer
    │   │   │           │
    │   │   │           └─▶ Timer expired?
    │   │   │               └─▶ Move to next waypoint
    │   │   │
    │   │   └─ NO ──▶ Check if reached waypoint
    │   │               │
    │   │               └─▶ Distance < 0.5?
    │   │                   └─▶ Start waiting
    │   │
    │   └─▶ Event detected?
    │       │
    │       └─▶ OnEventDetected(eventType)
    │           │
    │           ├─▶ Fire ──▶ Run away (increase speed)
    │           ├─▶ Accident ──▶ Stop and look (5 sec)
    │           └─▶ Other ──▶ Custom reaction
    │
    └─▶ On Loop Reset
        │
        └─▶ OnLoopReset()
            ├─▶ Warp to first waypoint
            ├─▶ Reset waypoint index
            └─▶ Reinitialize routine (new random)
```

---

## 5. NPC Memory System Flow

```
NPC WITH MEMORY COMPONENT
    │
    ├─▶ Start()
    │   └─▶ Subscribe to OnLoopCountChanged
    │
    ├─▶ Loop Count Changes
    │   │
    │   └─▶ OnLoopCountChanged(newCount)
    │       │
    │       └─▶ RemembersPreviousLoops()?
    │           │
    │           ├─▶ Check: canRemember == true?
    │           ├─▶ Check: loopCount >= threshold?
    │           │
    │           ├─ YES ──▶ RevealSecret()
    │           │           │
    │           │           ├─▶ Activate secretObject
    │           │           ├─▶ Set hasRevealed = true
    │           │           └─▶ ShowMemoryEffect()
    │           │
    │           └─ NO ──▶ Use normal dialogue
    │
    └─▶ Player Talks to NPC
        │
        └─▶ GetDialogue()
            │
            ├─▶ RemembersPreviousLoops()?
            │   │
            │   ├─ YES ──▶ Return memory dialogue
            │   │           "I remember you from before..."
            │   │
            │   └─ NO ──▶ Return normal dialogue
            │               "Hello there!"
```

---

## 6. Random Event Spawning Flow

```
LOOP RESET TRIGGERED
    │
    └─▶ RandomEventSpawner.SpawnRandomEvents()
        │
        ├─▶ ClearEvents()
        │   └─▶ Destroy all active event objects
        │
        ├─▶ Get loop seed
        ├─▶ Random.InitState(seed)
        │
        ├─▶ Determine event count (1 to maxEventsPerLoop)
        │
        └─▶ For each event:
            │
            ├─▶ Roll eventChance (70%)
            │   │
            │   ├─ FAIL ──▶ Skip this event
            │   │
            │   └─ SUCCESS ──▶ Continue
            │
            ├─▶ Pick random spawn point
            ├─▶ Pick random event type
            │   ├─▶ Fire
            │   ├─▶ Accident
            │   ├─▶ Robbery
            │   ├─▶ Blackout
            │   └─▶ Parade
            │
            ├─▶ SpawnEvent(type, spawnPoint)
            │   │
            │   ├─▶ Get event prefab
            │   ├─▶ Instantiate at location
            │   ├─▶ Add to activeEvents list
            │   │
            │   └─▶ NotifyNearbyNPCs(position, eventType)
            │       │
            │       └─▶ OverlapSphere(position, 20)
            │           │
            │           └─▶ For each NPC in range:
            │               └─▶ NPC.OnEventDetected(eventType)
            │
            └─▶ Log: "Spawned {count} events"
```

---

## 7. World State Randomization Flow

```
LOOP RESET EVENT
    │
    └─▶ WorldStateManager.RandomizeWorldState()
        │
        ├─▶ Get loop seed from TimeLoopManager
        ├─▶ Random.InitState(seed)
        │
        ├─▶ Create new WorldState:
        │   ├─▶ loopNumber = current loop
        │   ├─▶ weatherType = Random(WeatherType)
        │   ├─▶ timeOfDay = Random(6-18) hours
        │   ├─▶ trafficDensity = Random(0.3-1.0)
        │   └─▶ npcActivityLevel = Random(0.5-1.0)
        │
        └─▶ ApplyWorldState()
            │
            ├─▶ WeatherSystem.SetWeather(weatherType)
            │   │
            │   ├─▶ Stop all weather effects
            │   │
            │   └─▶ Start specific effect:
            │       ├─▶ Sunny ──▶ Clear sky
            │       ├─▶ Rainy ──▶ Rain particles
            │       ├─▶ Foggy ──▶ Fog particles + fog density
            │       └─▶ Stormy ──▶ Heavy rain + dark
            │
            ├─▶ TimeOfDaySystem.SetTimeOfDay(hour)
            │   │
            │   ├─▶ Update sun rotation
            │   ├─▶ Update light color
            │   ├─▶ Update light intensity
            │   └─▶ Update ambient light
            │
            └─▶ RandomizeWorldObjects()
                │
                └─▶ For each randomizable object:
                    └─▶ Apply random offset to position
```

---

## 8. Save/Load Flow

```
GAME START
    │
    └─▶ SaveLoadManager.LoadGame()
        │
        ├─▶ Check if save file exists
        │   │
        │   ├─ NO ──▶ Log: "No save, starting fresh"
        │   │
        │   └─ YES ──▶ Read JSON file
        │               │
        │               ├─▶ Deserialize SaveData
        │               │
        │               └─▶ TimeLoopManager.LoadClues(clues)
        │                   └─▶ Restore HashSet<string>
        │
        └─▶ Game continues with loaded data

───────────────────────────────────────────────

CLUE DISCOVERED
    │
    └─▶ TimeLoopManager.AddClue(clueId)
        │
        ├─▶ Add to discoveredClues set
        │
        └─▶ SaveLoadManager.SaveClues(discoveredClues)
            │
            ├─▶ Create SaveData object
            │   ├─▶ discoveredClues = List from HashSet
            │   └─▶ saveTimestamp = current time
            │
            ├─▶ JsonUtility.ToJson(data)
            │
            └─▶ File.WriteAllText(savePath, json)
                └─▶ Saved to persistent data path
```

---

## 9. UI Update Flow

```
EVERY FRAME
    │
    └─▶ UI_TimeDisplay.Update()
        │
        ├─▶ Get loop data from TimeLoopManager
        │   ├─▶ currentLoopCount
        │   └─▶ loopProgress (0-1)
        │
        ├─▶ Update Loop Count Text
        │   └─▶ "Loop: 5"
        │
        ├─▶ Update Progress Bar
        │   └─▶ fillAmount = loopProgress
        │
        └─▶ Get time from TimeOfDaySystem
            ├─▶ currentHour (0-24)
            │
            └─▶ Update Time Text
                ├─▶ Calculate hours:minutes
                │
                └─▶ Display "3:45 PM"

───────────────────────────────────────────────

LOOP RESET
    │
    └─▶ LoopResetTransition.PlayResetTransition()
        │
        └─▶ FadeTransition() coroutine
            │
            ├─▶ Fade In (0 → 1 alpha)
            │   └─▶ Over fadeDuration seconds
            │
            ├─▶ Hold (0.5 seconds)
            │
            └─▶ Fade Out (1 → 0 alpha)
                └─▶ Over fadeDuration seconds
```

---

## 10. Complete Game Loop Cycle

```
┌─────────────────────────────────────────────┐
│          PLAYER STARTS GAME                 │
└──────────────┬──────────────────────────────┘
               │
               ├─▶ Load save data (clues)
               ├─▶ Initialize all systems
               └─▶ Start Loop 0
                   │
    ┌──────────────┴──────────────┐
    │      LOOP IN PROGRESS       │
    │  ┌────────────────────┐     │
    │  │ Player explores    │     │
    │  │ NPCs patrol        │     │
    │  │ Events active      │     │
    │  │ Time progresses    │     │
    │  │ Weather active     │     │
    │  └──────┬─────────────┘     │
    │         │                   │
    │    ┌────▼─────┐             │
    │    │ Discovers│             │
    │    │  Clue?   │             │
    │    └────┬─────┘             │
    │      YES│  NO               │
    │    ┌────▼─────┐             │
    │    │Save Clue │             │
    │    │Persists! │             │
    │    └──────────┘             │
    │                             │
    │    Timer expires or         │
    │    manual reset             │
    └──────────┬──────────────────┘
               │
               ├─▶ LOOP RESET
               │   ├─▶ Screen fades
               │   ├─▶ World randomizes
               │   ├─▶ NPCs reset
               │   ├─▶ Events respawn
               │   ├─▶ Player teleports
               │   └─▶ Clues PERSIST
               │
               ├─▶ Loop count + 1
               │
               └─▶ Check NPC memory
                   │
                   ├─ Loop >= 3?
                   │   └─▶ Some NPCs remember!
                   │       └─▶ Reveal secrets
                   │
                   └─▶ Start next loop
                       │
                       └─▶ (Back to top)
```

---

## Summary

These event flows show how the systems interact to create the time loop mechanic:

1. **Initialization** - Systems set up and load data
2. **Loop Reset** - Central event that triggers all changes
3. **Player Interaction** - Clue discovery and persistence
4. **NPC Behavior** - Patrol, memory, and event reactions
5. **World State** - Deterministic randomization each loop
6. **Events** - Random spawning and NPC notifications
7. **UI** - Real-time updates and transitions
8. **Save/Load** - Persistent clue storage

All flows are event-driven and loosely coupled, making the system modular and expandable.
