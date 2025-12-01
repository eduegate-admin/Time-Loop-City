# Input System Migration Guide

This guide explains how to switch from the new Input System back to the legacy Input Manager to fix all input-related errors.

## The Problem

Your scripts use the legacy Input API (`Input.GetKeyDown()`, `Input.GetAxis()`, etc.), but Unity's Player Settings is configured to use the new Input System package. This causes errors like:

```
InvalidOperationException: You are trying to read Input using the UnityEngine.Input class, 
but you have switched active Input handling to Input System package in Player Settings.
```

## Solution: Switch to Legacy Input Manager

### Step 1: Open Player Settings

1. **Go to** `Edit` → `Project Settings`
2. **Select** `Player` in the left sidebar
3. **Scroll down** to "Other Settings" section

### Step 2: Change Active Input Handling

1. **Find** "Active Input Handling" setting
2. **Change from** `Input System Package (New)` 
3. **Change to** `Input Manager (Old)`

   **Alternative**: You can also select `Both` if you want to support both systems, but `Input Manager (Old)` is recommended for simplicity.

### Step 3: Apply and Restart

1. **Click "Apply"** when prompted
2. **Restart Unity Editor** (Unity will prompt you to do this)
3. **Wait** for Unity to recompile scripts

## Verification

After restarting Unity:

1. **Run the game** in Unity Editor
2. **Check the Console** - you should see:
   - ❌ NO "InvalidOperationException" errors
   - ✅ Input working correctly

3. **Test all input**:
   - Player movement (WASD/Arrow keys)
   - Camera rotation (Mouse)
   - Interaction (E key)
   - Pause menu (Escape)
   - UI navigation

## What This Changes

### Scripts Using Legacy Input (No Changes Needed):
- `PlayerController.cs` - Uses `Input.GetAxis()` for movement
- `GameManager.cs` - Uses `Input.GetKeyDown()` for pause
- `InteractionManager.cs` - Uses `Input.GetKeyDown()` for interaction
- `StandaloneInputModule` - Uses `Input.GetMousePosition()` for UI

All these scripts will work immediately with no code changes.

## Alternative: Migrate to New Input System (Advanced)

If you prefer to use the new Input System instead, you would need to:

1. Keep "Active Input Handling" as `Input System Package (New)`
2. Install Input System package (if not already installed)
3. Create Input Actions asset
4. Replace all `Input.GetKeyDown()` calls with new Input System API
5. Update `PlayerController`, `GameManager`, `InteractionManager`
6. Replace `StandaloneInputModule` with `InputSystemUIInputModule`

**This is significantly more work** and not recommended unless you specifically need new Input System features.

## Common Issues

### Issue: Still seeing Input errors after switching

**Solution**:
1. Make sure you **restarted Unity Editor** after changing settings
2. Check that setting is actually `Input Manager (Old)` or `Both`
3. Clear the Console and run again

### Issue: Input not working at all

**Solution**:
1. Go to `Edit` → `Project Settings` → `Input Manager`
2. Verify axes are defined:
   - Horizontal (A/D, Left/Right arrows)
   - Vertical (W/S, Up/Down arrows)
   - Mouse X
   - Mouse Y
3. If missing, Unity should have defaults - try resetting Input Manager

### Issue: "Both" mode not working

**Solution**:
- `Both` mode can sometimes cause conflicts
- Recommend using `Input Manager (Old)` exclusively
- If you need new Input System, fully migrate all scripts

## Input Manager Configuration

The legacy Input Manager should have these axes configured (Edit → Project Settings → Input Manager):

**Movement:**
- `Horizontal` - A/D keys and Left/Right arrows
- `Vertical` - W/S keys and Up/Down arrows

**Camera:**
- `Mouse X` - Mouse horizontal movement
- `Mouse Y` - Mouse vertical movement

**Actions:**
- `Fire1` - Left mouse button
- `Jump` - Space key
- `Submit` - Enter/Return key
- `Cancel` - Escape key

These are Unity's defaults and should already be configured.

## Next Steps

After fixing input:
1. Test all player controls
2. Test UI interactions
3. Test pause menu
4. Verify no input errors in Console
5. Proceed to [Manager Setup](MANAGER_SETUP.md)
