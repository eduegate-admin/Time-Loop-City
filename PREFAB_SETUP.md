# Time Loop City - Prefab Setup Guide

## Core Prefabs

### 1. Player Prefab

**Setup:**
1. Create capsule GameObject
2. Add components:
   - Character Controller
   - PlayerController.cs
3. Add child: CameraTarget (empty, positioned at head height)
4. Save as: `Prefabs/Player.prefab`

**Configuration:**
- Move Speed: 5
- Sprint Speed: 8
- Camera Distance: 5
- Interaction Range: 3
- Interactable Layer: "Interactable"

---

### 2. NPC Prefabs

#### NPC_Wanderer (Basic Citizen)

**Setup:**
1. Create capsule (smaller than player)
2. Add components:
   - Nav Mesh Agent
   - NPCController.cs
3. Optional: NPCMemoryComponent.cs
4. Save as: `Prefabs/NPCs/NPC_Wanderer.prefab`

**Configuration:**
- NPC Id: Unique string (e.g., "npc_citizen_01")
- NPC Name: "Citizen"
- Move Speed: 3.5
- Waypoint Wait Time: 2
- Randomize Routine: ✓

**Memory Component (if added):**
- Can Remember: ✓ (for special NPCs only)
- Loop Threshold: 3
- Add dialogue lines
- Optional: Secret Object to reveal

#### NPC_Rememberer (Special NPC)

Same as wanderer, but:
- Can Remember: ✓
- Loop Threshold: 3
- Memory Dialogue: Custom lines like "I remember you..."
- Reveals Secret After Memory: ✓

Save as: `Prefabs/NPCs/NPC_Rememberer.prefab`

---

### 3. Event Prefabs

#### Fire Event

**Setup:**
1. Create empty GameObject: `Event_Fire`
2. Add child: Particle System (fire effect)
3. Add child: Point Light (orange, flickering)
4. Add audio source (crackling sound)
5. Save as: `Prefabs/Events/Event_Fire.prefab`

**Particle System Settings:**
- Start Color: Orange to red gradient
- Start Speed: 3-5
- Emission: 50
- Shape: Cone
- Add noise and velocity over lifetime

#### Accident Event

**Setup:**
1. Create parent GameObject: `Event_Accident`
2. Add children:
   - 2-3 car models (tilted/crashed)
   - Particle system (steam/smoke)
   - Debris objects
3. Save as: `Prefabs/Events/Event_Accident.prefab`

#### Robbery Event

**Setup:**
1. Create: `Event_Robbery`
2. Add:
   - Area trigger (BoxCollider, Is Trigger)
   - Visual indicator (pulsing red light)
   - NPCs running away (optional)
3. Save as: `Prefabs/Events/Event_Robbery.prefab`

#### Blackout Event

**Setup:**
1. Create: `Event_Blackout`
2. Script to disable nearby lights
3. Darkness particle effect
4. Save as: `Prefabs/Events/Event_Blackout.prefab`

#### Parade Event

**Setup:**
1. Create: `Event_Parade`
2. Add:
   - Confetti particles
   - Music audio source
   - Crowd NPCs (optional)
3. Save as: `Prefabs/Events/Event_Parade.prefab`

---

### 4. Interactable Prefabs

#### Clue Object

**Setup:**
1. Create small object (cube, sphere, or custom model)
2. Add component: InteractableObject.cs
3. Configure:
   - Is Clue: ✓
   - Clue Id: Unique string
4. Add highlight effect:
   - Particle system (subtle glow)
   - Or emissive material
5. Set Layer: "Interactable"
6. Save as: `Prefabs/Interactables/Clue_*.prefab`

**Example Clues:**
- `Clue_MysteriousNote.prefab`
- `Clue_Photo.prefab`
- `Clue_KeyItem.prefab`

#### Door

**Setup:**
1. Create door model/object
2. Add component: InteractableObject.cs (extend for door logic)
3. Add animation for opening
4. Save as: `Prefabs/Interactables/Door.prefab`

---

### 5. UI Prefabs

#### Clue Notification Popup

**Setup:**
1. Create UI panel
2. Add:
   - Background image
   - Clue icon
   - Title text
   - Description text
3. Add animation (slide in from right)
4. Save as: `Prefabs/UI/ClueNotification.prefab`

---

## Prefab Variants

### NPC Variants

Create variants for different NPC types:

1. **NPC_Wanderer** (base)
   - Generic citizen

2. **NPC_Shopkeeper** (variant)
   - Stays at shop waypoints
   - Different dialogue

3. **NPC_Detective** (variant)
   - Always remembers
   - Special interactions

**Creating Variants:**
1. Drag base prefab into scene
2. Modify properties
3. Right-click prefab in scene > Prefab > Create Variant
4. Save in appropriate folder

---

## Weather Effect Prefabs

### Rain Particle System

**Setup:**
1. Create Particle System
2. Configure:
   - Shape: Box (large area above player)
   - Emission: 200-500
   - Start Speed: 10
   - Start Size: 0.1
   - Lifetime: 1-2
   - Gravity Modifier: 2
3. Save as: `Prefabs/Weather/Rain.prefab`

### Fog Particle System

**Setup:**
1. Create Particle System
2. Configure:
   - Shape: Sphere
   - Emission: 50
   - Start Speed: 0.5
   - Start Size: 5-10
   - Lifetime: 10
   - Color: Light gray with alpha
3. Save as: `Prefabs/Weather/Fog.prefab`

---

## Waypoint Prefab

**Setup:**
1. Create empty GameObject
2. Add Gizmo sphere script (for editor visibility)
3. Save as: `Prefabs/Waypoint.prefab`

**Usage:**
- Instantiate multiple waypoints
- Arrange in patterns for NPC routes
- Assign to NPCs

---

## Prefab Organization

```
Prefabs/
├── Player.prefab
├── NPCs/
│   ├── NPC_Wanderer.prefab
│   ├── NPC_Rememberer.prefab
│   ├── NPC_Shopkeeper.prefab
│   └── NPC_Detective.prefab
├── Events/
│   ├── Event_Fire.prefab
│   ├── Event_Accident.prefab
│   ├── Event_Robbery.prefab
│   ├── Event_Blackout.prefab
│   └── Event_Parade.prefab
├── Interactables/
│   ├── Clue_MysteriousNote.prefab
│   ├── Clue_Photo.prefab
│   ├── Door.prefab
│   └── Item.prefab
├── Weather/
│   ├── Rain.prefab
│   └── Fog.prefab
├── UI/
│   └── ClueNotification.prefab
└── Waypoint.prefab
```

---

## Testing Prefabs

### Player
- [ ] Movement works
- [ ] Camera follows
- [ ] Can interact

### NPCs
- [ ] Patrol waypoints
- [ ] Reset on loop
- [ ] Memory component works
- [ ] Detect events

### Events
- [ ] Spawn correctly
- [ ] Visual/audio effects work
- [ ] NPCs react

### Interactables
- [ ] Can be interacted with (E key)
- [ ] Clues register
- [ ] Highlight effect shows

---

## Next Steps

1. Create visual assets/models
2. Add animations to NPCs
3. Create event scripts for specific behaviors
4. Add audio clips
5. Polish effects
