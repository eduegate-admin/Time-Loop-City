# NavMesh Setup Guide

This guide explains how to bake a NavMesh for NPC navigation in Time Loop City.

## What is NavMesh?

NavMesh (Navigation Mesh) is Unity's pathfinding system that allows NPCs to navigate around obstacles in your game world.

## Prerequisites

- Your city environment must have colliders on walkable surfaces
- NPCs must have `NavMeshAgent` component (already added via `NPCController`)

## Step-by-Step Setup

### 1. Open Navigation Window

1. **Go to** `Window` → `AI` → `Navigation`
2. The Navigation window will open (usually docked next to Inspector)

### 2. Mark Walkable Surfaces

1. **Select** all ground/floor objects in your scene (roads, sidewalks, floors)
2. In the **Navigation window**, go to the **Object** tab
3. **Check** "Navigation Static"
4. Set **Navigation Area** to `Walkable`
5. **Click "Apply"**

### 3. Mark Obstacles

1. **Select** all obstacle objects (buildings, walls, props)
2. In the **Navigation window**, go to the **Object** tab
3. **Check** "Navigation Static"
4. Set **Navigation Area** to `Not Walkable`
5. **Click "Apply"**

### 4. Configure Agent Settings

1. In the **Navigation window**, go to the **Agents** tab
2. Configure the **Humanoid** agent (or create a new agent type):

```
Agent Radius:      0.5     (how wide the NPC is)
Agent Height:      2.0     (how tall the NPC is)
Step Height:       0.4     (max step/curb height)
Max Slope:         45      (max walkable slope in degrees)
```

### 5. Configure Bake Settings

1. In the **Navigation window**, go to the **Bake** tab
2. Configure these settings:

```
Agent Radius:           0.5
Agent Height:           2.0
Max Slope:              45
Step Height:            0.4

Generated Off Mesh Links:
  Drop Height:          0
  Jump Distance:        0

Advanced:
  Manual Voxel Size:    ☐ (unchecked)
  Min Region Area:      2
  Height Mesh:          ☐ (unchecked)
```

### 6. Bake the NavMesh

1. **Click the "Bake" button** at the bottom of the Navigation window
2. **Wait** for Unity to process (may take a few seconds to minutes depending on scene size)
3. You should see a **blue overlay** on walkable surfaces in the Scene view

### 7. Verify NavMesh

In the Scene view, you should see:
- **Blue areas**: Walkable NavMesh
- **No blue**: Non-walkable areas (obstacles, walls)

The blue mesh should cover:
- ✅ Roads and sidewalks
- ✅ Building interiors (if NPCs should enter)
- ✅ Open spaces where NPCs can walk

The blue mesh should NOT cover:
- ❌ Buildings (unless interiors are accessible)
- ❌ Walls
- ❌ Water (unless NPCs can walk on it)

## Testing

### 1. Test NPC Navigation

1. **Run the game** in Unity Editor
2. **Watch the Console** - you should see:
   - ✅ `[NPCController] City Guard using default routine for loop 0`
   - ✅ `[NPCController] Loop Reporter using alternate routine for loop 0`
   - ❌ NO "Failed to create agent because there is no valid NavMesh" errors

3. **Watch NPCs in Scene view**:
   - NPCs should move along their waypoints
   - NPCs should avoid obstacles
   - NPCs should follow the NavMesh (blue areas)

### 2. Check NavMesh Availability

If you still see warnings:
1. Select an NPC in Hierarchy
2. Look at the `NavMeshAgent` component in Inspector
3. Make sure the NPC is positioned on the NavMesh (blue area)

## Common Issues

### Issue: "Failed to create agent because there is no valid NavMesh"

**Causes**:
- NavMesh not baked
- NPC spawned outside NavMesh area
- NavMesh doesn't cover NPC's starting position

**Solutions**:
1. **Bake the NavMesh** (follow steps above)
2. **Move NPC spawn points** onto blue NavMesh areas
3. **Expand NavMesh coverage** by marking more surfaces as "Navigation Static"

### Issue: NPCs walk through walls

**Solution**:
- Mark walls/obstacles as "Navigation Static" with "Not Walkable" area
- Re-bake the NavMesh

### Issue: NPCs can't reach waypoints

**Solution**:
- Make sure waypoints are on the NavMesh (blue areas)
- Check that there's a continuous NavMesh path between waypoints
- Increase Agent Radius if NPCs are too wide for narrow passages

### Issue: NavMesh has holes or gaps

**Solution**:
- Decrease "Min Region Area" in Bake settings
- Make sure all ground surfaces are marked "Navigation Static"
- Check for overlapping colliders

## NPC Waypoint Setup

After baking NavMesh, set up NPC waypoints:

1. **Create empty GameObjects** for waypoints
2. **Position them** on the NavMesh (blue areas)
3. **Name them** descriptively (e.g., "Waypoint_StreetCorner")
4. **Assign to NPCs**:
   - Select NPC in Hierarchy
   - Find `NPCController` component
   - Drag waypoints into "Default Waypoints" list
   - (Optional) Set up "Alternate Waypoints" for variety

## Performance Tips

- **Bake once**, not every frame
- **Use simpler geometry** for NavMesh obstacles when possible
- **Limit NavMesh area** to where NPCs actually go
- **Don't mark decorative objects** as Navigation Static if NPCs don't interact with them

## Next Steps

After setting up NavMesh:
- Test NPC movement in Play mode
- Adjust waypoint positions if needed
- Configure NPC speeds and behaviors in `NPCController`
- Proceed to [Camera Setup](CAMERA_SETUP.md)
