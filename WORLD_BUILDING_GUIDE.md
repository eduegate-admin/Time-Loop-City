# Time Loop City - World Building Guide

This document serves as the **Level Design Document** for constructing the expanded Time Loop City. Follow these coordinate-precise instructions to build the world in Unity using standard Primitives (Cube, Plane, Cylinder).

---

## 🗺️ World Overview: "Chronos Bay"

The city is divided into **4 Districts**.
*   **Central**: Chronos Plaza (Spawn & City Hall)
*   **West**: The Loop (Commercial & Cafe)
*   **East**: The Grid (Industrial & Power)
*   **North**: Echo Park (Nature & Secrets)

### Global Settings
*   **Ground Plane**: Scale (50, 1, 50) at (0, 0, 0). Material: Asphalt.
*   **Skybox**: Default Day (will be controlled by `TimeOfDaySystem`).
*   **NavMesh**: Re-bake after placing all static structures.

---

## 🏙️ District 1: Chronos Plaza (Central)
*The heart of the city. The Clock Tower is visible from everywhere.*

### 1. City Hall & Clock Tower
*   **Base**: Cube at (0, 5, 25). Scale (12, 10, 8). Color: White Marble.
*   **Tower**: Cube at (0, 15, 28). Scale (4, 20, 4). Color: White Marble.
*   **Clock Face**: Cylinder at (0, 22, 26). Scale (3, 0.1, 3). Rotation (90, 0, 0). Color: Gold.
*   **The "Event" Emitter**: Empty GameObject at (0, 25, 28). Tag: `EventSource`.

### 2. The Fountain (Spawn Area)
*   **Base**: Cylinder at (0, 0.2, 0). Scale (6, 0.4, 6). Color: Stone.
*   **Water**: Cylinder at (0, 0.5, 0). Scale (5, 0.1, 5). Color: Blue (Transparent).
*   **Centerpiece**: Cube at (0, 1.5, 0). Scale (1, 3, 1). Color: Gold.
*   **Player Spawn Point**: Empty GameObject at (0, 1, -5). Rotation (0, 0, 0).

---

## ☕ District 2: The Loop (West)
*Commercial district with shops and the Cafe.*

### 1. "Deja Brew" Cafe (Enterable Interior)
*   **Building Shell**: Cube at (-15, 3, 5). Scale (10, 6, 10). Color: Brick Red.
*   **Doorway**: Create a "hole" or use 3 cubes to frame a door at (-10, 1.5, 5).
*   **Interior Floor**: Plane at (-15, 0.1, 5). Scale (1, 1, 1). Color: Wood.
*   **Counter**: Cube at (-18, 1, 5). Scale (2, 1, 8). Color: Dark Wood.
*   **Tables**: 3 Cylinders at (-13, 0.5, 3), (-13, 0.5, 7), (-16, 0.5, 5). Scale (1.5, 0.1, 1.5).
*   **NPC Spawn**: `NPC_Barista` at (-17, 1, 5).

### 2. Tech Shop "Circuit Breaker"
*   **Building**: Cube at (-15, 4, -10). Scale (10, 8, 12). Color: Blue/Glass.
*   **Sign**: TextMeshPro "CIRCUIT BREAKER" at (-10, 5, -10).

---

## 🏭 District 3: The Grid (East)
*Industrial zone. Site of the Fire and Blackout events.*

### 1. Power Station
*   **Main Reactor**: Cylinder at (20, 4, 5). Scale (8, 8, 8). Color: Concrete Grey.
*   **Cooling Towers**: 2 Cylinders at (25, 6, 0) and (25, 6, 10). Scale (3, 12, 3).
*   **Fence**: Cube walls surrounding area. Scale (0.2, 3, 20).
*   **Event Spawn**: `Event_Blackout` at (20, 0, 5).

### 2. The Warehouse (Fire Hazard)
*   **Building**: Cube at (20, 5, -15). Scale (12, 10, 15). Color: Rusted Metal.
*   **Crates**: Scattered small cubes around (18, 0.5, -12).
*   **Event Spawn**: `Event_Fire` at (22, 0, -15).

---

## 🌳 District 4: Echo Park (North)
*Nature area behind City Hall. Secrets hidden here.*

### 1. The Ancient Statue
*   **Pedestal**: Cube at (0, 1, 40). Scale (2, 2, 2). Color: Mossy Stone.
*   **Statue**: Capsule at (0, 3, 40). Scale (1, 2, 1). Color: Gold.
*   **Clue**: `Interactable_OldPlaque` attached to Pedestal.

### 2. The Bunker Entrance (Hidden)
*   **Location**: Behind a bush at (10, 0, 45).
*   **Door**: Cube at (10, 0, 45). Scale (2, 0.1, 2). Rotation (-45, 0, 0). Color: Steel.
*   **Interaction**: Locked Door. Requires `Keycard_Lab`.

---

## 📍 Points of Interest & Clues

### Clue Locations (Persistent Items)
1.  **Newspaper**: On a bench in **Chronos Plaza** (2, 0.5, -2).
    *   *Text*: "Scientist Warns of Anomalies"
2.  **Coffee Receipt**: On a table in **Deja Brew** (-13, 0.6, 3).
    *   *Text*: "Time: 7:55 AM. Order: Infinite Espresso."
3.  **Burned Memo**: Near **The Warehouse** (16, 0.1, -12).
    *   *Text*: "The cooling system is failing. Reset imminent."
4.  **Lab Keycard**: Hidden on top of **Tech Shop** roof (-15, 8.1, -10).
    *   *Requirement*: Need to jump/climb to reach.

---

## 🚦 World Events Setup

### 1. The Accident (Loop Time: 1:00)
*   **Location**: Intersection at (0, 0, 0).
*   **Setup**: Two Car prefabs collide.
*   **Trigger**: `RandomEventSpawner` spawns `Event_Accident` prefab here.

### 2. The Fire (Loop Time: 2:30)
*   **Location**: The Warehouse (22, 0, -15).
*   **Setup**: Fire particles, smoke.
*   **Trigger**: `RandomEventSpawner` spawns `Event_Fire` prefab here.

### 3. The Blackout (Loop Time: 4:00)
*   **Location**: Power Station.
*   **Effect**: `TimeOfDaySystem` sets light intensity to 0.1. Street lamps turn off.

---

## 🛠️ Unity Editor Instructions

1.  **Create Parent Objects**:
    *   Create Empty `Districts`.
    *   Create Empty `District_Central`, `District_West`, `District_East`, `District_North` inside.
2.  **Build Primitives**:
    *   Follow coordinates above to create Cubes/Cylinders.
    *   Name them clearly (e.g., `Building_Cafe`).
    *   Set all large structures to **Static**.
3.  **Bake Navigation**:
    *   Select `CityGround` and all Buildings.
    *   Window > AI > Navigation > Bake.
4.  **Place Spawners**:
    *   Create Empty `EventSpawners`.
    *   Place child objects at Event coordinates.
    *   Assign these transforms to `RandomEventSpawner` script.

This layout provides a dense, interconnected world suitable for a 5-minute time loop exploration.
