# Manager Setup Guide

This guide explains how to properly set up the manager GameObjects in your Unity scene to avoid DontDestroyOnLoad warnings.

## Overview

The following managers must be **root GameObjects** (not children of other objects) in your scene:
- TimeLoopManager
- GameManager
- SaveLoadManager

## Setup Steps

### 1. Create Manager GameObjects

In your main game scene (e.g., `GameScene`):

1. **Right-click in Hierarchy** → `Create Empty`
2. **Rename** to `TimeLoopManager`
3. **Repeat** for `GameManager` and `SaveLoadManager`

### 2. Attach Scripts

For each manager GameObject:

1. **Select the GameObject** in Hierarchy
2. **Click "Add Component"** in Inspector
3. **Search for and add** the corresponding script:
   - `TimeLoopManager` → Add `TimeLoopManager.cs`
   - `GameManager` → Add `GameManager.cs`
   - `SaveLoadManager` → Add `SaveLoadManager.cs`

### 3. Configure TimeLoopManager

Select `TimeLoopManager` in Hierarchy and configure in Inspector:

- **Loop Duration Seconds**: `300` (5 minutes)
- **Auto Reset On Complete**: ✓ (checked)
- **Player Spawn Point**: Drag your player spawn point Transform here

### 4. Configure GameManager

Select `GameManager` in Hierarchy and configure in Inspector:

- **Pause Menu UI**: Drag your pause menu Canvas/Panel here (if you have one)

### 5. Verify Hierarchy Structure

Your Hierarchy should look like this:

```
GameScene
├── TimeLoopManager          ← Root level (no parent)
├── GameManager              ← Root level (no parent)
├── SaveLoadManager          ← Root level (no parent)
├── Player
├── NPCs
│   ├── City Guard
│   ├── Loop Reporter
│   └── Street Vendor
├── Environment
└── UI
```

**IMPORTANT**: The three managers must be at the **root level** of the hierarchy, not nested under any other GameObject.

## Verification

1. **Run the game** in Unity Editor
2. **Check the Console** for warnings
3. You should **NOT** see any messages like:
   - ❌ "DontDestroyOnLoad only works for root GameObjects"
   - ❌ "There can be only one active Event System"

4. You **SHOULD** see:
   - ✅ "[TimeLoopManager] Loop 0 started"
   - ✅ "[SystemBootstrapper] Created missing EventSystem" (only once)

## Common Issues

### Issue: "DontDestroyOnLoad only works for root GameObjects"

**Solution**: The manager is a child of another GameObject. Move it to the root level:
1. Drag the manager GameObject to the top of the Hierarchy
2. Make sure it's not indented under anything

### Issue: Managers reset when changing scenes

**Solution**: Make sure the managers are root GameObjects and the scripts are properly attached.

### Issue: Multiple EventSystems warning

**Solution**: The SystemBootstrapper will automatically clean this up. If it persists:
1. Search Hierarchy for "EventSystem" objects
2. Delete any manually created EventSystem objects
3. Let SystemBootstrapper create it automatically

## Next Steps

After setting up managers, proceed to:
- [NavMesh Setup](NAVMESH_SETUP.md) - Configure NPC navigation
- [Camera Setup](CAMERA_SETUP.md) - Configure rendering
