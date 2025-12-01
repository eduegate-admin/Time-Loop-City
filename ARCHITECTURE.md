# Time Loop City - Architecture Documentation

## Overview

Time Loop City uses a **modular, event-driven architecture** with **loose coupling** between systems. This design allows easy expansion and modification without breaking existing functionality.

---

## Core Architecture Principles

### 1. **Event-Driven Communication**
Systems communicate through Unity Events, not direct references.

**Benefits:**
- Loose coupling
- Easy to add/remove listeners
- No circular dependencies
- Debuggable in Inspector

**Example:**
```csharp
// TimeLoopManager fires event
OnLoopReset.Invoke();

// WorldStateManager listens
TimeLoopManager.Instance.OnLoopReset.AddListener(RandomizeWorldState);
```

### 2. **Singleton Managers**
Core managers use singleton pattern for global access.

**Managers:**
- `TimeLoopManager` - Central loop control
- `WorldStateManager` - World randomization
- `PersistentClueSystem` - Clue tracking
- `SaveLoadManager` - Persistence

**Pattern:**
```csharp
public static TimeLoopManager Instance { get; private set; }

private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);
}
```

### 3. **Component-Based Design**
GameObjects composed of modular components.

**Example - NPC:**
```
NPC GameObject
├── NavMeshAgent (Unity)
├── NPCController (movement/routine)
└── NPCMemoryComponent (optional memory)
```

### 4. **Data-Driven Configuration**
Use ScriptableObjects for data, not hard-coded values.

**Benefits:**
- Designers can modify
- No recompilation
- Version control friendly
- Runtime modification in editor

---

## System Architecture

### High-Level System Diagram

```
┌─────────────────────────────────────────────────────────┐
│                     PLAYER INPUT                         │
└────────────────────┬────────────────────────────────────┘
                     │
         ┌───────────▼──────────┐
         │  PlayerController    │
         │  - Movement          │
         │  - Interaction       │
         └───────────┬──────────┘
                     │
    ┌────────────────┼────────────────┐
    │                │                │
┌───▼──────┐  ┌──────▼─────┐  ┌──────▼──────┐
│ Camera   │  │Interactable│  │   Physics   │
│ System   │  │  Objects   │  │   System    │
└──────────┘  └──────┬─────┘  └─────────────┘
                     │
              ┌──────▼──────┐
              │  Persistent │
              │Clue System  │
              └──────┬──────┘
                     │
         ┌───────────▼──────────────┐
         │   TimeLoopManager        │
         │   (Central Hub)          │
         │   - Loop Timer           │
         │   - Reset Trigger        │
         │   - Persistent Data      │
         └───────────┬──────────────┘
                     │
       ┌─────────────┼─────────────┐
       │             │             │
┌──────▼─────┐ ┌────▼──────┐ ┌────▼──────┐
│   World    │ │    NPC    │ │  Event    │
│   State    │ │  System   │ │  System   │
│  Manager   │ │           │ │           │
└──────┬─────┘ └────┬──────┘ └────┬──────┘
       │            │             │
  ┌────┼────┐  ┌────┼─────┐  ┌────┼─────┐
  │    │    │  │    │     │  │    │     │
┌─▼┐ ┌─▼──┐ │┌─▼┐ ┌─▼──┐  │┌─▼┐ ┌─▼──┐ │
│Time│Weather││AI││Memory│ ││Fire││Accident│
│Day │System │││  ││Comp  │ ││   ││Events │
└───┘ └─────┘│└──┘ └─────┘ │└───┘ └─────┘
            │             │
         ┌──▼──┐      ┌──▼──┐
         │ UI  │      │Audio│
         └─────┘      └─────┘
```

---

## System Responsibilities

### Core Systems

#### TimeLoopManager
**Responsibilities:**
- Track current loop count
- Manage loop timer
- Trigger loop resets
- Store persistent clues
- Provide deterministic seed

**Dependencies:**
- None (root system)

**Dependents:**
- All other systems listen to its events

#### WorldStateManager
**Responsibilities:**
- Randomize world state each loop
- Apply weather changes
- Control time of day
- Manage world object states

**Dependencies:**
- `TimeLoopManager` (loop events)
- `WeatherSystem`
- `TimeOfDaySystem`

**Dependents:**
- None

#### PersistentClueSystem
**Responsibilities:**
- Track discovered clues
- Provide clue data
- Show clue notifications

**Dependencies:**
- `TimeLoopManager` (clue storage)
- `ClueDatabase` (ScriptableObject)

**Dependents:**
- `InteractableObject` (registers clues)

#### SaveLoadManager
**Responsibilities:**
- Save clues to disk
- Load clues on startup
- Manage save file

**Dependencies:**
- `TimeLoopManager` (clue data)

**Dependents:**
- None

---

### World Systems

#### TimeOfDaySystem
**Responsibilities:**
- Cycle day/night
- Control sunlight
- Update ambient lighting

**Dependencies:**
- `TimeLoopManager` (time progress)

**Dependents:**
- Visual systems

#### WeatherSystem
**Responsibilities:**
- Switch weather types
- Control particle effects (rain, fog)
- Adjust fog settings

**Dependencies:**
- `WorldStateManager` (weather commands)

**Dependents:**
- Visual/audio systems

---

### AI Systems

#### NPCController
**Responsibilities:**
- Patrol waypoints
- React to events
- Change routine per loop

**Dependencies:**
- `TimeLoopManager` (loop reset)
- Unity NavMeshAgent

**Dependents:**
- `NPCMemoryComponent`

#### NPCMemoryComponent
**Responsibilities:**
- Track loop count for NPC
- Provide memory dialogue
- Reveal secrets

**Dependencies:**
- `TimeLoopManager` (loop count)

**Dependents:**
- Dialogue system (future)

---

### Event Systems

#### RandomEventSpawner
**Responsibilities:**
- Spawn random events
- Despawn events on reset
- Notify NPCs

**Dependencies:**
- `TimeLoopManager` (loop seed)

**Dependents:**
- Event prefabs

---

### Player Systems

#### PlayerController
**Responsibilities:**
- Player movement
- Camera control
- Interaction input

**Dependencies:**
- Unity CharacterController
- Camera

**Dependents:**
- `InteractableObject`

#### InteractableObject
**Responsibilities:**
- Handle player interaction
- Register clues
- Trigger custom behaviors

**Dependencies:**
- `PersistentClueSystem` (clue registration)

**Dependents:**
- None (base class to extend)

---

### UI Systems

#### UI_TimeDisplay
**Responsibilities:**
- Display loop count
- Show time of day
- Update progress bar

**Dependencies:**
- `TimeLoopManager` (loop data)
- `TimeOfDaySystem` (time data)

**Dependents:**
- None

#### LoopResetTransition
**Responsibilities:**
- Fade screen on reset
- Visual feedback

**Dependencies:**
- `TimeLoopManager` (reset event)

**Dependents:**
- None

---

## Data Flow

### Loop Reset Flow

```
1. Loop timer expires in TimeLoopManager
   │
   ├─▶ Increment loop count
   │
   ├─▶ Fire OnLoopReset event
   │   │
   │   ├─▶ WorldStateManager.RandomizeWorldState()
   │   │   ├─▶ WeatherSystem.SetWeather()
   │   │   └─▶ TimeOfDaySystem.SetTimeOfDay()
   │   │
   │   ├─▶ RandomEventSpawner.SpawnRandomEvents()
   │   │   └─▶ Notify nearby NPCs
   │   │
   │   ├─▶ NPCController.OnLoopReset()
   │   │   ├─▶ Reset position
   │   │   └─▶ Reinitialize routine
   │   │
   │   └─▶ LoopResetTransition.PlayTransition()
   │
   ├─▶ Teleport player to spawn
   │
   └─▶ Fire OnLoopStart event (after delay)
       └─▶ Systems re-initialize
```

### Clue Discovery Flow

```
1. Player presses E near InteractableObject
   │
   ├─▶ InteractableObject.Interact()
   │   │
   │   └─▶ PersistentClueSystem.DiscoverClue()
   │       │
   │       ├─▶ TimeLoopManager.AddClue()
   │       │   │
   │       │   └─▶ SaveLoadManager.SaveClues()
   │       │
   │       └─▶ Show UI notification
```

### NPC Memory Flow

```
1. Loop count reaches threshold
   │
   ├─▶ NPCMemoryComponent detects via OnLoopCountChanged
   │
   ├─▶ Check if NPC can remember
   │
   └─▶ If yes:
       ├─▶ Activate secret object
       ├─▶ Change dialogue
       └─▶ Trigger special actions
```

---

## Expandability

### Adding New Features

#### 1. Dialogue System

**Integration Points:**
- Extend `InteractableObject` for NPCs
- Use `NPCMemoryComponent.GetDialogue()`
- Create `DialogueManager` singleton
- Subscribe to clue events for dialogue changes

**Architecture:**
```
DialogueManager
├── DialogueUI
├── DialogueDatabase (ScriptableObject)
└── Integration with NPCMemoryComponent
```

#### 2. Mission System

**Integration Points:**
- Create `MissionManager` singleton
- Subscribe to clue discovery events
- Track objectives across loops
- Store progress in `TimeLoopManager`

**Architecture:**
```
MissionManager
├── MissionDatabase (ScriptableObject)
├── Objective Tracker
└── Integration with PersistentClueSystem
```

#### 3. Cars/Driving

**Integration Points:**
- New `VehicleController` component
- Switch player control modes
- Integrate with `TimeLoopManager` for reset
- Add to `WorldStateManager` for traffic

**Architecture:**
```
VehicleController
├── Physics-based driving
├── Enter/Exit interaction
└── Reset position on loop
```

#### 4. Combat (Optional)

**Integration Points:**
- Create `CombatSystem` namespace
- Add `HealthComponent` to player/NPCs
- Integrate with loop reset (reset health)
- Track combat events for NPC reactions

**Architecture:**
```
CombatSystem
├── HealthComponent
├── DamageSystem
├── WeaponController
└── Combat Events
```

#### 5. Building Interiors

**Integration Points:**
- Create `InteriorManager`
- Load/unload scenes
- Randomize interiors via `WorldStateManager`
- Door interactions

**Architecture:**
```
InteriorManager
├── Scene loading
├── Interior randomization
└── Door transitions
```

---

## Best Practices

### 1. Adding New Systems

**Steps:**
1. Create system class
2. Add to appropriate namespace
3. Subscribe to `TimeLoopManager` events if needed
4. Reset state on loop reset
5. Use ScriptableObjects for config
6. Document dependencies

### 2. Modifying Existing Systems

**Guidelines:**
- Check dependency graph
- Update events, not direct calls
- Test loop reset behavior
- Maintain backwards compatibility
- Update documentation

### 3. Performance Considerations

**Optimization:**
- Pool event prefabs
- Limit NPC pathfinding updates
- Use LOD for distant objects
- Optimize particle systems
- Cache component references

### 4. Debugging

**Tools:**
- Console logs for system events
- Unity Events visible in Inspector
- Gizmos for waypoints/ranges
- Debug UI panel

---

## Common Patterns

### 1. Listening to Loop Events

```csharp
private void Start()
{
    if (TimeLoopManager.Instance != null)
    {
        TimeLoopManager.Instance.OnLoopReset.AddListener(OnLoopReset);
    }
}

private void OnLoopReset()
{
    // Reset your system
}
```

### 2. Deterministic Randomization

```csharp
int seed = TimeLoopManager.Instance.GetLoopSeed();
Random.InitState(seed);
float value = Random.value; // Deterministic per loop
```

### 3. Persistent Data

```csharp
// Add clue
TimeLoopManager.Instance.AddClue("clue_id");

// Check clue
bool hasClue = TimeLoopManager.Instance.HasClue("clue_id");
```

### 4. Extending InteractableObject

```csharp
public class Door : InteractableObject
{
    protected override void OnInteract(PlayerController player)
    {
        // Custom door logic
        OpenDoor();
    }
}
```

---

## File Organization

```
Scripts/
├── Core/
│   ├── PersistentClueSystem.cs
│   └── GameSettings.cs (future)
├── TimeLoop/
│   └── TimeLoopManager.cs
├── World/
│   ├── WorldStateManager.cs
│   ├── TimeOfDaySystem.cs
│   └── WeatherSystem.cs
├── AI/
│   ├── NPCController.cs
│   └── NPCMemoryComponent.cs
├── Events/
│   └── RandomEventSpawner.cs
├── Player/
│   ├── PlayerController.cs
│   └── InteractableObject.cs
├── UI/
│   ├── UI_TimeDisplay.cs
│   └── LoopResetTransition.cs
└── Managers/
    └── SaveLoadManager.cs
```

---

## Next Steps

1. **Polish core systems** - Add visual/audio feedback
2. **Create content** - More clues, events, NPCs
3. **Add dialogue** - Build dialogue system
4. **Implement missions** - Create mission framework
5. **Expand world** - More locations, interiors
6. **Performance** - Optimize for larger city
7. **Testing** - Build automated tests

---

## Summary

Time Loop City's architecture is designed for:
- ✅ **Modularity** - Systems are independent
- ✅ **Expandability** - Easy to add features
- ✅ **Maintainability** - Clear responsibilities
- ✅ **Testability** - Systems can be tested independently
- ✅ **Flexibility** - Data-driven configuration

The event-driven approach ensures loose coupling, making it easy to add new systems without breaking existing functionality.
