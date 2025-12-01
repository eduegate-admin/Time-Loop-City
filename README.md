# Time Loop City - Getting Started

Welcome to **Time Loop City**! This README will help you get started with this Unity project.

---

## Project Overview

**Time Loop City** is a time-loop based open-world city game where:
- The player explores a small city
- Every loop (5 minutes), the world resets with changes
- NPC routines shift, random events spawn, weather changes
- Some NPCs remember previous loops
- Players discover clues that persist across loops
- Build your understanding of the city's mysteries across multiple loops

---

## Features

✅ **Time Loop System** - Automatic resets with deterministic randomization  
✅ **Persistent Clue System** - Discoveries carry across loops  
✅ **Dynamic World State** - Weather, time of day, and events change each loop  
✅ **NPC AI** - Patrol-based NPCs with loop-dependent routines  
✅ **NPC Memory** - Special NPCs remember previous loops  
✅ **Random Events** - Fires, accidents, robberies, blackouts, parades  
✅ **Third-Person Player Controller** - WASD movement, mouse camera, E interaction  
✅ **Save/Load System** - Clues persist across game sessions  
✅ **Modular Architecture** - Easy to expand with new features  

---

## Quick Start

### 1. Project Structure

```
Time Loop City/
├── Scripts/           # All C# scripts
│   ├── Core/         # Core systems
│   ├── TimeLoop/     # Time loop manager
│   ├── World/        # Weather, time of day
│   ├── AI/           # NPC controllers
│   ├── Events/       # Random event spawner
│   ├── Player/       # Player controller
│   ├── UI/           # UI controllers
│   └── Managers/     # Save/load
├── Prefabs/          # Reusable prefabs
├── Scenes/           # Unity scenes
├── ScriptableObjects/# Data assets
├── Materials/        # Materials
├── Animations/       # Animations (future)
└── Audio/           # Audio clips (future)
```

### 2. Setup Steps

**Follow these guides in order:**

1. **[SCENE_SETUP.md](./SCENE_SETUP.md)** - Set up the main Unity scene
2. **[PREFAB_SETUP.md](./PREFAB_SETUP.md)** - Create reusable prefabs
3. **[SCRIPTABLEOBJECTS.md](./SCRIPTABLEOBJECTS.md)** - Create data assets

### 3. Important Documentation

- **[ARCHITECTURE.md](./ARCHITECTURE.md)** - System architecture and design patterns
- **[EVENT_FLOWS.md](./EVENT_FLOWS.md)** - Visual event flow diagrams

---

## Core Scripts

### Time Loop System
- **TimeLoopManager.cs** - Central loop control, timer, resets
- **WorldStateManager.cs** - Randomizes world state each loop
- **PersistentClueSystem.cs** - Tracks discovered clues
- **SaveLoadManager.cs** - Save/load clues to disk

### World Systems
- **TimeOfDaySystem.cs** - Day-night cycle
- **WeatherSystem.cs** - Weather effects (rain, fog, etc.)

### AI Systems
- **NPCController.cs** - NPC patrol and routines
- **NPCMemoryComponent.cs** - NPCs that remember loops
- **RandomEventSpawner.cs** - Spawns random city events

### Player Systems
- **PlayerController.cs** - Third-person movement and camera
- **InteractableObject.cs** - Base class for interactable objects

### UI Systems
- **UI_TimeDisplay.cs** - Shows loop count and time
- **LoopResetTransition.cs** - Screen fade on reset

---

## Controls

| Input | Action |
|-------|--------|
| **W/A/S/D** | Move player |
| **Mouse** | Rotate camera |
| **Left Shift** | Sprint |
| **E** | Interact with objects |
| **Esc** | Pause (future) |

---

## How the Time Loop Works

### During a Loop
1. Player explores the city (5 minutes per loop)
2. NPCs patrol on randomized routes
3. Random events occur (fires, accidents, etc.)
4. Player can discover clues by interacting with objects
5. Weather and time of day are set for this loop

### When Loop Resets
1. Screen fades to black
2. Player teleports back to spawn
3. World state randomizes (deterministic per loop number)
   - New weather
   - New time of day
   - Different NPC routines
   - New random events
4. **Clues persist** - Player keeps all discovered clues
5. Loop count increases

### NPC Memory (After Loop 3)
- Some NPCs start "remembering" previous loops
- They provide different dialogue
- May reveal secrets or new clues

---

## Testing Checklist

After setup, verify:

- [ ] Player can move with WASD and camera with mouse
- [ ] Time progresses (check UI in top corners)
- [ ] Loop resets after ~5 minutes
- [ ] Screen fades on reset
- [ ] Player returns to spawn on reset
- [ ] NPCs patrol between waypoints
- [ ] Random events spawn each loop
- [ ] Weather changes each loop
- [ ] Can interact with objects (press E)
- [ ] Clues save and load across game sessions

---

## Expanding the Project

This framework is designed to be expanded. See **[ARCHITECTURE.md](./ARCHITECTURE.md)** for details on adding:

- **Dialogue System** - NPC conversations
- **Mission System** - Quests and objectives
- **Building Interiors** - Enter buildings
- **Driving** - Cars and vehicles
- **Combat** (optional) - Action gameplay
- **Cutscenes** - Story moments

---

## Architecture Highlights

### Event-Driven
- Systems communicate via Unity Events
- Loose coupling between components
- Easy to add new listeners

### Modular
- Each system has clear responsibilities
- No circular dependencies
- Components can be added/removed independently

### Data-Driven
- ScriptableObjects for configuration
- No hardcoded values
- Designers can modify without code changes

### Deterministic
- Loop randomization uses seeded Random
- Same loop number = same world state
- Reproducible for testing

---

## File Naming Conventions

- **Scripts**: PascalCase (e.g., `TimeLoopManager.cs`)
- **Prefabs**: PascalCase with type (e.g., `NPC_Wanderer.prefab`)
- **ScriptableObjects**: PascalCase (e.g., `ClueDatabase.asset`)
- **Scenes**: PascalCase (e.g., `MainCity.unity`)

---

## Tips for Development

### 1. Always Test Loop Resets
- Reset behavior is core to the game
- Make sure everything cleans up properly
- Check console for errors on reset

### 2. Use Deterministic Random
```csharp
int seed = TimeLoopManager.Instance.GetLoopSeed();
Random.InitState(seed);
```

### 3. Subscribe to Events Properly
```csharp
TimeLoopManager.Instance.OnLoopReset.AddListener(MyResetMethod);
```

### 4. Make Interactables Obvious
- Add highlight effects
- Use emissive materials
- Add particle systems

### 5. Debug with Console Logs
- Scripts already include useful debug logs
- Watch console during loop resets
- Check for event spawning

---

## Common Issues

### NPCs Not Moving
- Check NavMesh is baked
- Verify waypoints are assigned
- Check NavMeshAgent component settings

### Events Not Spawning
- Verify event prefabs are assigned in RandomEventSpawner
- Check spawn point positions are set
- Look for errors in console

### Clues Not Saving
- Check ClueDatabase is assigned to PersistentClueSystem
- Verify save path is accessible (Application.persistentDataPath)
- Check console for save/load errors

### Loop Not Resetting
- Verify TimeLoopManager is in scene
- Check loop duration setting
- Ensure player spawn point is assigned

---

## Performance Tips

For larger cities:
- Use object pooling for events
- Implement NPC LOD system
- Optimize particle systems
- Use occlusion culling
- Implement async scene loading for interiors

---

## Next Steps

1. **✅ Follow setup guides** - Get the scene running
2. **🎨 Create visual assets** - Replace primitive shapes
3. **🎵 Add audio** - Sound effects and music
4. **📝 Write story** - Create clue descriptions and NPC dialogue
5. **🗺️ Design city** - Layout roads, buildings, landmarks
6. **🎬 Add polish** - Animations, VFX, UI improvements
7. **🎮 Playtest** - Test loop mechanics and balance
8. **🚀 Expand** - Add missions, dialogue, interiors

---

## Resources

- **Unity NavMesh Documentation**: [docs.unity3d.com/Manual/Navigation.html](https://docs.unity3d.com/Manual/Navigation.html)
- **Unity Events**: [docs.unity3d.com/Manual/UnityEvents.html](https://docs.unity3d.com/Manual/UnityEvents.html)
- **ScriptableObjects**: [docs.unity3d.com/Manual/class-ScriptableObject.html](https://docs.unity3d.com/Manual/class-ScriptableObject.html)

---

## Support

If you encounter issues:
1. Check console for error messages
2. Review relevant documentation file
3. Verify setup steps were followed correctly
4. Check that all required components are assigned in Inspector

---

## License

This is a starter framework for game development. Feel free to modify and expand for your project!

---

**Good luck building Time Loop City! 🎮🔁🏙️**
