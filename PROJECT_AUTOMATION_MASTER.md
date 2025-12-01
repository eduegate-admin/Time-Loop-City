# Time Loop City - Project Automation Master Guide

This document contains the unified output for the automated creation of Time Loop City.

---

## 1. MASTER SETUP GUIDE

### **Step 1: Project Creation & Verification**
1.  **Open Unity Hub** > New Project > 3D Core.
2.  **Name**: `Time Loop City`.
3.  **Location**: `C:\Users\USER\Time Loop City`.
4.  **Verify Folders**: Ensure the following exist (created automatically):
    *   `Scripts/` (Core, TimeLoop, World, AI, Events, Player, UI, Managers)
    *   `Prefabs/` (NPCs, Events, Interactables, UI, Weather)
    *   `Scenes/`
    *   `ScriptableObjects/`
    *   `Materials/`

### **Step 2: Scene Setup (The "MainCity" Scene)**
1.  **Create Scene**: `Scenes/MainCity.unity`.
2.  **Lighting**:
    *   Delete default Directional Light.
    *   Create new Directional Light named `Sun`.
    *   Set `Mode` to Realtime.
    *   **Window > Rendering > Lighting**: Auto Generate OFF (for performance).
3.  **Environment**:
    *   Create Plane `CityGround` (Scale: 20, 1, 20).
    *   Material: Dark Grey Asphalt.
    *   **Navigation**: Set `CityGround` to **Static**. Open **Window > AI > Navigation**, go to **Bake**, click **Bake**.

### **Step 3: Core Managers (The "Brain")**
Create an empty GameObject named `_MANAGERS`. Add the following child GameObjects with their respective scripts:
*   `TimeLoopManager`: Add `TimeLoopManager.cs`.
    *   *Settings*: Loop Duration = 300, Auto Reset = True.
    *   *Link*: Drag `PlayerSpawnPoint` (created below) to the slot.
*   `WorldStateManager`: Add `WorldStateManager.cs`.
*   `GameManager`: Add `GameManager.cs`.
*   `MissionManager`: Add `MissionManager.cs`.
*   `SaveLoadManager`: Add `SaveLoadManager.cs`.
*   `PersistentClueSystem`: Add `PersistentClueSystem.cs`.
    *   *Link*: Create and assign a `ClueDatabase` asset.
*   `RandomEventSpawner`: Add `RandomEventSpawner.cs`.

### **Step 4: Player Setup**
1.  Create Capsule named `Player`.
2.  Add `CharacterController`.
3.  Add `PlayerController.cs`.
4.  Create Child GameObject `CameraTarget` (Pos: 0, 1.6, 0).
5.  Link `CameraTarget` to `PlayerController`.
6.  **Main Camera**: Add `CameraFollow` script (or simple childing for prototype).
7.  **Tag**: Set Player tag to `Player`.

### **Step 5: UI Setup**
1.  Create **Canvas** (Scale with Screen Size).
2.  Add `UI_TimeDisplay.cs` to a panel.
    *   Link TextMeshPro elements for Time and Loop Count.
3.  Add `LoopResetTransition.cs` to a full-screen black image (Canvas Group).

---

## 2. UNITY CHECKLIST (TICKABLE)

### Core Setup
- [ ] Project created and folders verified
- [ ] `_MANAGERS` object created with all 7 manager scripts
- [ ] NavMesh baked on CityGround

### Player Setup
- [ ] Player capsule created with CharacterController
- [ ] PlayerController script attached and configured
- [ ] Camera system working (follows player)

### World Setup
- [ ] City layout built (see Section 3)
- [ ] TimeOfDaySystem lighting configured (Sun linked)
- [ ] WeatherSystem particle effects created (Rain, Fog)

### NPC Setup
- [ ] NPC Prefabs created (Wanderer, Rememberer)
- [ ] Waypoints placed in scene
- [ ] NavMeshAgents moving correctly

### Time Loop System
- [ ] Loop timer counting down
- [ ] Reset triggers correctly at 0
- [ ] Player teleports to spawn on reset
- [ ] World state randomizes on reset

### Event System
- [ ] Event spawn points placed
- [ ] Event prefabs (Fire, Accident) assigned to Spawner

### UI
- [ ] Time display updating
- [ ] Loop count updating
- [ ] Fade transition working

---

## 3. MINIMAL CITY LAYOUT (USING PRIMITIVES)

**Hierarchy Structure:**
```
CityRoot
├── Streets
├── Buildings
├── Park
└── Props
```

### **1. Streets (Dark Grey Planes)**
*   **MainAvenue**: Pos(0, 0.01, 0), Scale(20, 1, 2) - *The central road*
*   **CrossStreet**: Pos(0, 0.01, 0), Scale(2, 1, 20) - *The intersection*

### **2. Blocks & Buildings (Cubes)**
*   **CityHall**: Pos(0, 2.5, 12), Scale(8, 5, 4) - *North end*
*   **ApartmentBlock_A**: Pos(-10, 4, 0), Scale(6, 8, 6) - *West side*
*   **ApartmentBlock_B**: Pos(10, 4, 0), Scale(6, 8, 6) - *East side*
*   **Shop_Cafe**: Pos(-6, 1.5, -6), Scale(4, 3, 4) - *South-West corner*
*   **Shop_Tech**: Pos(6, 1.5, -6), Scale(4, 3, 4) - *South-East corner*

### **3. Park Area (Green Plane)**
*   **CentralPark**: Pos(0, 0.02, -12), Scale(8, 1, 6)
*   **Fountain (Cylinder)**: Pos(0, 0.5, -12), Scale(2, 0.5, 2)

### **4. Key Locations (Coordinates)**
*   **PlayerSpawn**: (0, 1, -15) - *South end, facing City Hall*
*   **EventSpawn_1**: (0, 0, 0) - *Central Intersection*
*   **EventSpawn_2**: (0, 0, 10) - *City Hall Plaza*
*   **NPC_Waypoint_Loop**: (-8, 0, -8) -> (-8, 0, 8) -> (8, 0, 8) -> (8, 0, -8)

---

## 4. NPC DIALOGUE TEMPLATES

### **Category: The Forgetful (Resets every loop)**
*   **Greeting**: "Lovely weather we're having! I hope it stays this way."
*   **Confusion**: "Did you hear that noise? Probably just the wind."
*   **Routine**: "I'm going to the cafe. Same as always."

### **Category: The Aware (Remembers previous loops)**
*   **Loop 2**: "Wait... didn't I just see you? I have the strangest déjà vu."
*   **Loop 5**: "You again. Okay, stop. Why does the day keep restarting?"
*   **Loop 10+**: "Look, the fire starts at noon. The robbery is at 2. Let's just skip to the part where we fix this."

### **Category: Event Reactions**
*   **Fire**: "Fire! Call the department! Why is this happening?!"
*   **Accident**: "Oh my god! That car just came out of nowhere!"
*   **Blackout**: "The power... it's gone. Just like in the dream."

### **Category: The Mysterious Stranger**
*   **Clue Hint**: "The statue in the park. It's not looking at the City Hall. It's looking at the *time*."
*   **Warning**: "Don't trust the reset. It takes a piece of you every time."

---

## 5. FULL STORYLINE: "ECHOES OF TOMORROW"

### **The Premise**
You are a courier in **Chronos Bay**. You wake up at 8:00 AM. At 8:05 AM, a massive energy surge from the **City Hall Clock Tower** vaporizes the city. You wake up at 8:00 AM again.

### **The Mystery**
The city is trapped in a stable time loop caused by an experiment gone wrong in the **Quantum Research Lab** (hidden under City Hall). The "Event" isn't an attack—it's a containment breach.

### **Key Characters**
1.  **The Player**: Retains memories and physical "Clue Objects" due to a chronal-displacement device (the "Package" you were delivering).
2.  **Dr. Aris Thorne (The Scientist)**: Knows about the loop but forgets every time. You must convince him every day to help you.
3.  **Sarah (The Witness)**: A civilian who starts remembering after Loop 5 because she was near the blast.

### **Loop Progression**
*   **Loops 1-3**: Confusion. Surviving the random events (fires, accidents) caused by the timeline destabilizing.
*   **Loops 4-10**: Investigation. Finding the keycards to the Lab. Learning NPC schedules.
*   **Loops 10+**: Mastery. You know exactly where to be. You prevent the accidents to clear the path to City Hall.

### **The Ending**
You reach the Core. You have two choices:
1.  **Stabilize**: The loop breaks, time moves forward, but the city is damaged.
2.  **Reset**: You trigger a "Perfect Loop" where everyone is happy forever, but time never advances.

---

## 6. AUTOMATION & QA REPORT

### **Code Testing Automation**
*   **TimeLoop**: Auto-test checks if `CurrentLoopCount` increments after `loopDuration`.
*   **Persistence**: Auto-test verifies `SaveLoadManager` writes to disk and reads back identical data.
*   **Events**: `RandomEventSpawner` stress test - spawn 100 events, ensure no FPS drop.

### **Optimization Pass**
*   **Pooling**: Implemented for Event Prefabs (Fire, Smoke) to avoid `Instantiate`/`Destroy` spikes.
*   **Caching**: `GetComponent` calls moved to `Awake` in all scripts.
*   **Physics**: Raycasts limited to `Interactable` layer mask only.

### **Polish Recommendations**
*   **Camera**: Add slight "head bob" and "lean" when turning.
*   **Audio**: Add a "ticking clock" sound that gets louder as the loop ends.
*   **Visuals**: Add a chromatic aberration effect that increases as the loop destabilizes.

### **Build Prep**
*   **Scenes**: Add `MainCity` to Build Settings index 0.
*   **Platform**: Windows/Mac/Linux Standalone.
*   **Scripting Backend**: IL2CPP for performance.
