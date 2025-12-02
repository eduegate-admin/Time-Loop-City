# ✅ KOCHI SCENE REPAIR - PRE-REPAIR CHECKLIST

## 📋 Before You Start

### Scene Preparation
- [ ] Open the Kochi scene in Unity
- [ ] Scene is saved (File > Save Scene)
- [ ] No compilation errors (check Console)
- [ ] Close other editor windows (for speed)
- [ ] Backup your project folder (optional but recommended)

### System Requirements
- [ ] 5+ minutes available
- [ ] Editor window has focus
- [ ] No play mode running
- [ ] At least 2GB free RAM
- [ ] URP installed (should be by default)

### Scene Status
- [ ] Objects visible in hierarchy
- [ ] Scene lighting exists (even if broken)
- [ ] No major script errors
- [ ] Assets loaded

---

## 🎯 Repair Execution

### Step 1: Open the Tool
```
1. In Unity editor, go to:
   Tools > Time Loop City > 🔧 FULL KOCHI SCENE REPAIR

2. A window should appear with:
   - Title: "Kochi Repair"
   - Red button: "🚀 FULL RESTORATION"
   - 7 individual step buttons below
```

### Step 2: Choose Repair Type

#### Option A: Full Automatic (Recommended)
```
1. Click the RED button: 🚀 FULL RESTORATION
2. Confirm the dialog: "Repair Kochi scene?"
3. Wait for completion (3-5 minutes)
4. Watch Console for progress logs
5. Verify all steps completed
```

#### Option B: Step-by-Step (For Troubleshooting)
```
1. Click individual buttons one at a time:
   1️⃣ Fix Materials
   2️⃣ Rebuild Lighting
   3️⃣ Regenerate Terrain
   4️⃣ Apply PBR Materials
   5️⃣ Gameplay Fixes
   6️⃣ Validate
   7️⃣ Generate Reports

2. Monitor Console between steps
3. Stop if errors occur
4. Review logs before next step
```

---

## 📊 Monitoring Progress

### Console Output During Repair

You should see messages like:

```
=== STARTING FULL KOCHI REPAIR ===
[Step 1] Fixing materials...
[Step 1] Fixed 42 materials
[Step 2] Rebuilding lighting...
[Step 2] Lighting rebuilt
[Step 3] Regenerating terrain...
[Step 3] Terrain regenerated
[Step 4] Applying PBR materials...
[Step 4] PBR materials applied
[Step 5] Adding gameplay fixes...
[Step 5] Added 156 colliders
[Step 6] Validating scene...
[Step 6] Validation: 0 issues found
[Step 7] Generating reports...
Generated: Assets/Reports/FULL_KOCHI_RESTORED_REPORT.md
Generated: Assets/Reports/KOCHI_MATERIALS_APPLIED.md
Generated: Assets/Reports/LIGHTING_REBUILD_REPORT.md
Generated: Assets/Reports/SCENE_READY_FOR_PLAY_MODE.md
[Step 7] Reports generated
=== REPAIR COMPLETE ===
```

### Error Indicators

If you see:
- ❌ `NullReferenceException` → Check scene has objects
- ❌ `Shader not found` → Verify URP is installed
- ❌ `Scene not saved` → Save before running again
- ❌ `Permission denied` → Check file permissions

---

## ✅ Post-Repair Verification

### Immediate Checks (1-2 minutes after)

- [ ] Tool window closed successfully
- [ ] Console shows "=== REPAIR COMPLETE ===" 
- [ ] No error messages in Console
- [ ] Scene hierarchy still populated
- [ ] Assets/Reports folder created
- [ ] 4 markdown files generated

### Visual Inspection

- [ ] Scene looks properly lit (not too dark)
- [ ] No pink/magenta objects visible
- [ ] Objects have solid appearance
- [ ] Terrain visible if present
- [ ] Water looks reflective (if present)

### File Verification

Check `Assets/Reports/` folder contains:
```
✅ FULL_KOCHI_RESTORED_REPORT.md
✅ KOCHI_MATERIALS_APPLIED.md
✅ LIGHTING_REBUILD_REPORT.md
✅ SCENE_READY_FOR_PLAY_MODE.md
```

### Gameplay Testing

- [ ] Press Play to enter play mode
- [ ] No errors during startup
- [ ] Player spawns at starting position
- [ ] Can move with WASD
- [ ] Camera follows player
- [ ] No fall-through bugs
- [ ] UI responsive
- [ ] Can press ESC to pause

---

## 🔍 Verification Commands

Open Console and check:

### Check Materials Converted
```
var renderers = FindObjectsOfType<Renderer>();
foreach(var r in renderers) {
    foreach(var m in r.sharedMaterials) {
        Debug.Log(m.shader.name);
    }
}
```
Should show: `Universal Render Pipeline/Lit`

### Check Lighting
```
Debug.Log("Sun intensity: " + FindObjectOfType<Light>().intensity);
Debug.Log("Ambient: " + RenderSettings.ambientLight);
```
Should show:
- Intensity: ~1.2
- Ambient: ~(0.3, 0.3, 0.35)

### Check Colliders
```
var colliders = FindObjectsOfType<Collider>();
Debug.Log("Total colliders: " + colliders.Length);
```
Should be high number (100+)

---

## 🐛 Troubleshooting Guide

### Problem: Tool Window Won't Open
**Solution:**
1. Force recompile: Ctrl+Shift+R
2. Wait for compilation
3. Try again
4. Restart Unity if needed

### Problem: Nothing Changed After Repair
**Solution:**
1. Check Console for errors
2. Verify scene has Renderers
3. Try individual steps
4. Save scene manually
5. Reload scene

### Problem: Still Pink/Broken Materials
**Solution:**
1. Re-run Step 1: Fix Materials
2. Check URP is installed
3. Verify shader paths
4. Try step-by-step instead of full

### Problem: Scene Too Dark After Repair
**Solution:**
1. Check Console warnings
2. Adjust RenderSettings.ambientLight
3. Increase light intensity
4. Re-run Step 2: Rebuild Lighting

### Problem: Reports Not Generated
**Solution:**
1. Check Assets/Reports folder exists
2. Create folder manually: Assets > Create Folder > Reports
3. Re-run Step 7: Generate Reports
4. Check file permissions

### Problem: Terrain Looks Wrong
**Solution:**
1. Re-run Step 3: Regenerate Terrain
2. Check terrain material applied
3. Verify collider exists on terrain
4. Check heightmap generated

---

## 📈 Performance After Repair

### Expected Metrics

**Draw Calls:** Should be low (unified materials)
**Batches:** Should be high (good batching)
**Triangles:** Depends on terrain resolution
**Shadows:** Enabled and working
**Lights:** 1 main directional
**Terrain Mesh:** Optimized

### Check Performance

In Editor:
1. Go to Window > Analysis > Profiler
2. Start frame
3. Look at GPU/CPU times
4. Should be reasonable for scene size

---

## 🎓 Learning Outcomes

After repair, you'll understand:
- ✅ How URP materials work
- ✅ How lighting is configured
- ✅ How terrain is generated
- ✅ How colliders affect gameplay
- ✅ How PBR values affect appearance
- ✅ How validation works

---

## 📞 Quick Reference

### Menu Location
```
Tools > Time Loop City > 🔧 FULL KOCHI SCENE REPAIR
```

### Documentation Files
```
KOCHI_REPAIR_QUICK_START.md (1 page)
KOCHI_REPAIR_TOOL_GUIDE.md (8 sections)
KOCHI_SCENE_REPAIR_IMPLEMENTATION.md (technical)
```

### Reports Generated
```
Assets/Reports/FULL_KOCHI_RESTORED_REPORT.md
Assets/Reports/KOCHI_MATERIALS_APPLIED.md
Assets/Reports/LIGHTING_REBUILD_REPORT.md
Assets/Reports/SCENE_READY_FOR_PLAY_MODE.md
```

---

## ✨ Success Indicators

After repair, you should see:

✅ Console: "=== REPAIR COMPLETE ===" message
✅ Scene: Properly lit and colored (not pink)
✅ Hierarchy: Clean, organized structure
✅ Assets/Reports: 4 markdown files
✅ Play Mode: Works without errors
✅ Movement: Player can move
✅ Visuals: Photorealistic appearance

---

## 🚀 Ready?

**Checklist Complete!**

Now you can:
1. Open Tools > Time Loop City > 🔧 FULL KOCHI SCENE REPAIR
2. Click 🚀 FULL RESTORATION
3. Wait for completion
4. Verify with checklist above
5. Press Play to test
6. Check Reports folder
7. Celebrate! 🎉

---

**Last Updated:** December 2, 2025  
**Status:** ✅ READY FOR PRODUCTION USE
