# Camera Setup Guide

This guide explains how to configure the camera for Time Loop City, including Universal Render Pipeline (URP) setup if applicable.

## Check Your Render Pipeline

First, determine which render pipeline your project uses:

1. **Go to** `Edit` → `Project Settings` → `Graphics`
2. **Look at** "Scriptable Render Pipeline Settings"
   - If it shows **"None"**: You're using **Built-in Render Pipeline** (skip URP steps)
   - If it shows a **URP asset**: You're using **Universal Render Pipeline** (follow URP steps)

## Built-in Render Pipeline Setup

If using Built-in Render Pipeline:

1. **Select** Main Camera in Hierarchy
2. **Verify** Camera component settings:
   - Clear Flags: `Skybox` or `Solid Color`
   - Culling Mask: `Everything`
   - Projection: `Perspective`
   - Field of View: `60`

3. **No additional setup needed** - the camera should work out of the box

## Universal Render Pipeline (URP) Setup

If using URP, you need to add the URP Camera Data component:

### 1. Add Universal Additional Camera Data

1. **Select** Main Camera in Hierarchy
2. **Click** "Add Component" in Inspector
3. **Search for** "Universal Additional Camera Data"
4. **Add** the component

This fixes the warning: "Camera Main Camera does not contain an additional camera data component"

### 2. Configure URP Camera Settings

With the camera selected, configure:

**Camera Component:**
- Clear Flags: `Skybox`
- Culling Mask: `Everything`
- Projection: `Perspective`
- Field of View: `60`

**Universal Additional Camera Data:**
- Render Type: `Base`
- Render Post Processing: ✓ (if using post-processing)
- Anti-aliasing: `SMAA` or `FXAA` (recommended)
- Stop NaN: ✓ (recommended)
- Dithering: ✓ (recommended)

### 3. Verify URP Asset

1. **Go to** `Edit` → `Project Settings` → `Graphics`
2. **Check** that a URP asset is assigned
3. **Go to** `Edit` → `Project Settings` → `Quality`
4. **Verify** URP asset is assigned for your quality level

## Post-Processing Setup (Optional)

If you want visual effects like bloom, color grading, etc.:

### For Built-in Pipeline:

1. **Select** Main Camera
2. **Add Component** → `Post Process Layer`
3. **Add Component** → `Post Process Volume`
4. Configure as needed

### For URP:

1. **Create** a Global Volume:
   - Right-click in Hierarchy → `Volume` → `Global Volume`
2. **Configure** the Volume:
   - Profile: Create new or assign existing
   - Add overrides (Bloom, Tonemapping, Color Adjustments, etc.)
3. **Enable** "Render Post Processing" on camera's Universal Additional Camera Data

## Camera Controller Setup

Your camera is controlled by `PlayerController.cs`. Verify settings:

1. **Select** Player in Hierarchy
2. **Find** `PlayerController` component
3. **Check** Camera Settings:
   - Camera Sensitivity: `2.0` (adjust to preference)
   - Min Look Angle: `-80` (how far down you can look)
   - Max Look Angle: `80` (how far up you can look)

## Common Issues

### Issue: "Camera does not contain an additional camera data component"

**Solution**: You're using URP. Add "Universal Additional Camera Data" component to camera (see URP Setup above).

### Issue: Camera not following player

**Solution**:
1. Check that camera is a child of Player GameObject
2. Verify `PlayerController` script is attached to Player
3. Check camera's local position is set correctly (e.g., `0, 1.6, 0` for eye level)

### Issue: Black screen or no rendering

**Solutions**:
- Check camera's Culling Mask includes all layers
- Verify camera's Clear Flags is set to Skybox or Solid Color
- Make sure camera is enabled
- Check that lighting is set up in the scene

### Issue: Post-processing not working

**For URP**:
- Enable "Render Post Processing" on Universal Additional Camera Data
- Make sure a Global Volume exists with a profile
- Verify URP asset has post-processing enabled

**For Built-in**:
- Add Post Process Layer component to camera
- Create a Post Process Volume in the scene
- Set volume to "Is Global"

## Testing

1. **Run the game** in Unity Editor
2. **Verify**:
   - ✅ Camera renders the scene correctly
   - ✅ No camera warnings in Console
   - ✅ Mouse look controls work
   - ✅ Post-processing effects appear (if enabled)

## Performance Tips

- Use **Occlusion Culling** for large scenes (Window → Rendering → Occlusion Culling)
- Adjust **Far Clip Plane** to only render what's visible
- Disable **MSAA** if targeting lower-end hardware (use FXAA instead)
- Limit **Shadow Distance** in Quality Settings

## Next Steps

After camera setup:
- Test all systems together
- Verify no console errors
- Proceed to final verification and testing
