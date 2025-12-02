# Time Loop City: Kochi Edition - World Building Guide

This document serves as the **Level Design Document** for constructing the expanded Time Loop City, now set in a stylized **Kochi, Kerala**. Follow these coordinate-precise instructions to build the world in Unity.

---

## 🗺️ World Overview: "Cochin Harbour"

The city is divided into **4 Districts** based on real Kochi geography.
*   **Central**: Marine Drive (Spawn & City Hub)
*   **West**: Fort Kochi (History & Cafe)
*   **East**: Willingdon Island (Industrial & Port)
*   **North**: Mangalavanam (Nature & Secrets)

### Global Settings
*   **Ground Plane**: Scale (50, 1, 50) at (0, 0, 0). Material: Asphalt/Pavement.
*   **Skybox**: Tropical Overcast (will be controlled by `TimeOfDaySystem`).
*   **NavMesh**: Re-bake after placing all static structures.

---

## 🏙️ District 1: Marine Drive (Central)
*The iconic promenade facing the backwaters. The Rainbow Bridge is the centerpiece.*

### 1. The High Court & Clock Tower
*   **Base**: Cube at (0, 5, 25). Scale (12, 10, 8). Color: Red/White Brick (Indo-Saracenic style).
*   **Tower**: Cube at (0, 15, 28). Scale (4, 20, 4). Color: Red/White.
*   **Clock Face**: Cylinder at (0, 22, 26). Scale (3, 0.1, 3). Rotation (90, 0, 0). Color: Gold.
*   **The "Event" Emitter**: Empty GameObject at (0, 25, 28). Tag: `EventSource`.

### 2. Rainbow Bridge (Spawn Area)
*   **Base**: Cylinder at (0, 0.2, 0). Scale (6, 0.4, 6). Color: Concrete.
*   **Arches**: 3 Torus shapes (half-buried) at (0, 2, 0). Color: Rainbow Gradient.
*   **Player Spawn Point**: Empty GameObject at (0, 1, -5). Rotation (0, 0, 0).

---

## ☕ District 2: Fort Kochi (West)
*Colonial history meets modern cafes. Famous for the Chinese Fishing Nets.*

### 1. "Kashi Art Cafe" (Enterable Interior)
*   **Building Shell**: Cube at (-15, 3, 5). Scale (10, 6, 10). Color: White Plaster (Colonial).
*   **Doorway**: Frame a door at (-10, 1.5, 5).
*   **Interior Floor**: Plane at (-15, 0.1, 5). Scale (1, 1, 1). Color: Terracotta Tiles.
*   **Counter**: Cube at (-18, 1, 5). Scale (2, 1, 8). Color: Dark Wood.
*   **Tables**: 3 Cylinders at (-13, 0.5, 3), (-13, 0.5, 7), (-16, 0.5, 5). Scale (1.5, 0.1, 1.5).
*   **NPC Spawn**: `NPC_Barista` at (-17, 1, 5).

### 2. Chinese Fishing Nets (Landmark)
*   **Structure**: 4 long Cylinders (Poles) leaning out over the "water" at (-25, 5, 0).
*   **Net**: Plane (rotated) hanging from poles. Scale (5, 5, 1). Texture: Netting.
*   **Sign**: TextMeshPro "Cheenavala" at (-20, 2, 0).

---

## 🏭 District 3: Willingdon Island (East)
*The Port area. Industrial, shipping containers, and the Naval Base.*

### 1. Port Trust Power Station
*   **Main Reactor**: Cylinder at (20, 4, 5). Scale (8, 8, 8). Color: Concrete Grey.
*   **Cranes**: 2 Tall Cubes (Cranes) at (25, 10, 0) and (25, 10, 10). Color: Yellow.
*   **Fence**: Cube walls surrounding area. Scale (0.2, 3, 20).
*   **Event Spawn**: `Event_Blackout` at (20, 0, 5).

### 2. The Godown (Fire Hazard)
*   **Building**: Cube at (20, 5, -15). Scale (12, 10, 15). Color: Rusted Metal/Corrugated Sheet.
*   **Spice Sacks**: Scattered small spheres/cubes around (18, 0.5, -12).
*   **Event Spawn**: `Event_Fire` at (22, 0, -15).

---

## 🌳 District 4: Mangalavanam (North)
*The "Green Lung of Kochi". Dense mangroves and secrets.*

### 1. The Old Watchtower
*   **Pedestal**: Cube at (0, 1, 40). Scale (2, 2, 2). Color: Mossy Stone.
*   **Structure**: Cylinder at (0, 8, 40). Scale (3, 15, 3). Color: Wood.
*   **Clue**: `Interactable_OldPlaque` attached to Pedestal.

### 2. The Metro Pillar Bunker (Hidden)
*   **Location**: Behind a Metro Pillar at (10, 0, 45).
*   **Pillar**: Cylinder at (8, 5, 45). Scale (2, 10, 2). Color: Concrete with "Kochi Metro" logo.
*   **Door**: Cube at (10, 0, 45). Scale (2, 0.1, 2). Rotation (-45, 0, 0). Color: Steel.
*   **Interaction**: Locked Door. Requires `Keycard_Lab`.

---

## 📍 Points of Interest & Clues

### Clue Locations (Persistent Items)
1.  **Malayala Manorama Newspaper**: On a bench in **Marine Drive** (2, 0.5, -2).
    *   *Text*: "Monsoon Anomalies Predicted"
2.  **Chaya Receipt**: On a table in **Kashi Art Cafe** (-13, 0.6, 3).
    *   *Text*: "Time: 7:55 AM. Order: Sulaimani Tea."
3.  **Burned Manifest**: Near **The Godown** (16, 0.1, -12).
    *   *Text*: "Spice shipment delayed. Cooling failure."
4.  **Metro Card**: Hidden on top of **Fishing Net** structure (-25, 8, 0).
    *   *Requirement*: Need to jump/climb to reach.

---

## 🚦 World Events Setup

### 1. The Traffic Block (Loop Time: 1:00)
*   **Location**: Intersection at (0, 0, 0) (Menaka Junction).
*   **Setup**: Two Auto-Rickshaw prefabs collide.
*   **Trigger**: `RandomEventSpawner` spawns `Event_Accident` prefab here.

### 2. The Godown Fire (Loop Time: 2:30)
*   **Location**: The Godown (22, 0, -15).
*   **Setup**: Fire particles, smoke.
*   **Trigger**: `RandomEventSpawner` spawns `Event_Fire` prefab here.

### 3. The Blackout (Loop Time: 4:00)
*   **Location**: Port Trust Power Station.
*   **Effect**: `TimeOfDaySystem` sets light intensity to 0.1. Street lamps turn off.

---

## 🛠️ Unity Editor Instructions

1.  **Create Parent Objects**:
    *   Create Empty `Districts`.
    *   Create Empty `District_MarineDrive`, `District_FortKochi`, `District_Willingdon`, `District_Mangalavanam`.
2.  **Build Primitives**:
    *   Follow coordinates above.
    *   Use temporary materials (Red for Fort Kochi, Grey for Willingdon).
3.  **Bake Navigation**:
    *   Select `CityGround` and all Buildings.
    *   Window > AI > Navigation > Bake.
