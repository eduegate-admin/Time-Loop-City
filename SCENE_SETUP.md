# Time Loop City - Complete Setup Guide

This is the master guide for setting up Time Loop City. Follow the guides in order to fix all critical issues and properly configure your game.

## 🚨 Critical Fixes Required

Your game currently has several critical issues that prevent it from running correctly. Follow these guides **in order** to fix them:

### 1. Input System Fix (REQUIRED FIRST)
**File**: [INPUT_SYSTEM_SETUP.md](INPUT_SYSTEM_SETUP.md)

**Problem**: All input is broken due to Input System configuration mismatch.

**Fix**: Switch Player Settings to use legacy Input Manager.

**Time**: 2 minutes

---

### 2. Manager Setup (REQUIRED SECOND)
**File**: [MANAGER_SETUP.md](MANAGER_SETUP.md)

**Problem**: DontDestroyOnLoad warnings, managers not persisting across scenes.

**Fix**: Ensure TimeLoopManager, GameManager, and SaveLoadManager are root GameObjects.

**Time**: 5 minutes

---

### 3. NavMesh Setup (REQUIRED THIRD)
**File**: [NAVMESH_SETUP.md](NAVMESH_SETUP.md)

**Problem**: NPCs cannot navigate, "Failed to create agent" errors.

**Fix**: Bake NavMesh for your city scene.

**Time**: 10-15 minutes

---

### 4. Camera Setup (OPTIONAL)
**File**: [CAMERA_SETUP.md](CAMERA_SETUP.md)

**Problem**: Camera warnings (if using URP).

**Fix**: Add Universal Additional Camera Data component.

**Time**: 2 minutes

---

## Quick Start Checklist

Follow this checklist to get your game running:

- [ ] **Step 1**: Fix Input System ([INPUT_SYSTEM_SETUP.md](INPUT_SYSTEM_SETUP.md))
  - Change Player Settings to "Input Manager (Old)"
  - Restart Unity Editor
  - Verify no Input errors in Console

- [ ] **Step 2**: Setup Managers ([MANAGER_SETUP.md](MANAGER_SETUP.md))
  - Move TimeLoopManager to scene root
  - Move GameManager to scene root
  - Move SaveLoadManager to scene root
  - Verify no DontDestroyOnLoad warnings

- [ ] **Step 3**: Bake NavMesh ([NAVMESH_SETUP.md](NAVMESH_SETUP.md))
  - Mark ground as Navigation Static
  - Configure NavMesh settings
  - Bake NavMesh
  - Verify blue NavMesh appears in Scene view

- [ ] **Step 4**: Test Everything
  - Run game in Play mode
  - Check Console for errors
  - Test player movement
  - Test NPC navigation
  - Verify time loop resets work

---

## Detailed Scene Setup (After Fixes)

Once you've completed the critical fixes above, use this guide to build out your game scene.

### Scene Creation

#### 1. Create Main Scene
1. In Unity, create a new scene: `File > New Scene`
2. Save it as `MainCity.unity` in the `Scenes/` folder

#### 2. Lighting Setup

**Directional Light (Sun)**
- Create: `GameObject > Light > Directional Light`
- Name: `Sun`
- Position: (0, 10, 0)
- Rotation: (50, -30, 0)
- Intensity: 1.0
- Color: White

**Environment**
- Window > Rendering > Lighting Settings
- Skybox Material: Create or use default
- Environment Lighting: Skybox
- Ambient Intensity: 1.0

### Core Systems Setup

#### TimeLoopManager (See MANAGER_SETUP.md)
1. Create empty GameObject at root level
2. Name: `TimeLoopManager`
3. Add component: `TimeLoopManager.cs`
4. Configure:
   - Loop Duration Seconds: 300 (5 minutes)
   - Auto Reset On Complete: ✓
   - Create player spawn point (see below)

#### Player Spawn Point
1. Create empty GameObject
2. Name: `PlayerSpawnPoint`
3. Position: (0, 1, 0)
4. Drag to TimeLoopManager's "Player Spawn Point" field

#### WorldStateManager
1. Create empty GameObject
2. Name: `WorldStateManager`
3. Add component: `WorldStateManager.cs`

#### GameManager (See MANAGER_SETUP.md)
1. Create empty GameObject at root level
2. Name: `GameManager`
3. Add component: `GameManager.cs`

#### SaveLoadManager (See MANAGER_SETUP.md)
1. Create empty GameObject at root level
2. Name: `SaveLoadManager`
3. Add component: `SaveLoadManager.cs`

#### PersistentClueSystem
1. Create empty GameObject
2. Name: `PersistentClueSystem`
3. Add component: `PersistentClueSystem.cs`

### World Setup

#### Terrain/City Ground
**Option A: Plane**
1. Create: `GameObject > 3D Object > Plane`
2. Scale: (10, 1, 10) for 100x100 units
3. Add material
4. **IMPORTANT**: Mark as "Navigation Static" for NavMesh

**Option B: Terrain**
1. Create: `GameObject > 3D Object > Terrain`
2. Use terrain tools to sculpt city landscape
3. **IMPORTANT**: Mark as "Navigation Static" for NavMesh

#### City Buildings (Placeholder)
1. Create cubes for buildings: `GameObject > 3D Object > Cube`
2. Scale to building sizes (e.g., 3x5x3)
3. Position around the city
4. Add different materials
5. Group under empty GameObject named `Buildings`
6. **IMPORTANT**: Mark buildings as "Navigation Static" with "Not Walkable" area

### Player Setup

#### Player GameObject
1. Create: `GameObject > 3D Object > Capsule`
2. Name: `Player`
3. Tag: `Player`
4. Position: (0, 1, 0)
5. Add component: `Character Controller`
   - Radius: 0.5
   - Height: 2
   - Center: (0, 1, 0)
6. Add component: `PlayerController.cs`

#### Camera Target
1. Create empty child of Player: `CameraTarget`
2. Position: (0, 1.5, 0) - head height
3. Assign to PlayerController's "Camera Target" field

#### Main Camera (See CAMERA_SETUP.md)
1. Select Main Camera
2. Position: (0, 5, -5)
3. Look at player
4. If using URP, add "Universal Additional Camera Data" component

### NPC Setup (See NAVMESH_SETUP.md)

#### Create NPC Prefab
1. Create: `GameObject > 3D Object > Capsule`
2. Name: `NPC_Wanderer`
3. Scale: (0.5, 1, 0.5) - slightly smaller than player
4. Add component: `Nav Mesh Agent`
5. Add component: `NPCController.cs`
6. Configure NPC ID and name

#### Create Waypoints
1. Create empty GameObjects for waypoints
2. Name: `Waypoint_01`, `Waypoint_02`, etc.
3. **Position on NavMesh** (blue areas in Scene view)
4. Group under `Waypoints` parent GameObject

**Assign to NPC:**
- Drag waypoints to NPCController's `Default Waypoints` list
- Create alternate route and assign to `Alternate Waypoints`

5. Drag NPC to `Prefabs/` folder to create prefab
6. Place instances in scene

### UI Setup

#### Canvas
1. Create: `GameObject > UI > Canvas`
2. Canvas Scaler:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080

#### Time Display
1. Create: `GameObject > UI > Text - TextMeshPro`
2. Name: `TimeDisplay`
3. Make child of Canvas
4. Position: Top-right corner
5. Text: "00:00 AM"

#### Loop Count Display
1. Create another Text: `LoopCountDisplay`
2. Position: Top-left corner
3. Text: "Loop: 0"

### Final Scene Hierarchy

```
MainCity
├── TimeLoopManager          ← ROOT LEVEL (REQUIRED)
├── GameManager              ← ROOT LEVEL (REQUIRED)
├── SaveLoadManager          ← ROOT LEVEL (REQUIRED)
├── WorldStateManager
├── PersistentClueSystem
├── RandomEventSpawner
├── Lighting
│   └── Sun
├── Environment
│   ├── Ground (Navigation Static: Walkable)
│   └── Buildings (Navigation Static: Not Walkable)
├── Player
│   └── CameraTarget
├── NPCs
│   ├── NPC_Wanderer_01
│   └── NPC_Wanderer_02
├── Waypoints (on NavMesh)
│   ├── Waypoint_01
│   └── Waypoint_02
├── Main Camera
└── UI Canvas
    ├── TimeDisplay
    └── LoopCountDisplay
```

---

## Testing Checklist

After completing all setup:

### Console Verification
- [ ] ✅ No "InvalidOperationException" Input errors
- [ ] ✅ No "DontDestroyOnLoad only works for root GameObjects" warnings
- [ ] ✅ No "Failed to create agent because there is no valid NavMesh" errors
- [ ] ✅ No "There can be only one active Event System" warnings
- [ ] ✅ See "[TimeLoopManager] Loop 0 started" message
- [ ] ✅ See "[SystemBootstrapper] Created missing EventSystem" message (once)

### Gameplay Testing
- [ ] Player can move with WASD
- [ ] Camera rotates with mouse
- [ ] Can interact with objects (E key)
- [ ] Can pause game (Escape)
- [ ] Time progresses (check UI)
- [ ] Loop resets after duration
- [ ] Player teleports to spawn on reset
- [ ] NPCs patrol waypoints
- [ ] NPCs avoid obstacles
- [ ] Random events spawn each loop

---

## Troubleshooting

### Still seeing errors?

1. **Input Errors**: Did you restart Unity after changing Input Settings?
2. **Manager Warnings**: Are managers at root level (not children)?
3. **NavMesh Errors**: Did you bake the NavMesh? Is it visible (blue) in Scene view?
4. **Camera Warnings**: Are you using URP? Add Universal Additional Camera Data component.

### Need more help?

Check the detailed guides:
- [INPUT_SYSTEM_SETUP.md](INPUT_SYSTEM_SETUP.md)
- [MANAGER_SETUP.md](MANAGER_SETUP.md)
- [NAVMESH_SETUP.md](NAVMESH_SETUP.md)
- [CAMERA_SETUP.md](CAMERA_SETUP.md)

---

## Next Steps

Once everything is working:

1. Create event prefabs (Fire, Accident, etc.)
2. Design city layout
3. Add more interactable objects
4. Create dialogue system
5. Add missions/quests
6. Polish visuals
7. Add audio

See [BUILD_GUIDE.md](BUILD_GUIDE.md) for building and deployment instructions.
