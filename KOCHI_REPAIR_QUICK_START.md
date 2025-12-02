# 🔧 KOCHI SCENE REPAIR - QUICK REFERENCE

## Access the Tool
```
Tools > Time Loop City > 🔧 FULL KOCHI SCENE REPAIR
```

## One-Click Repair
Click: **🚀 FULL RESTORATION**

---

## What Gets Fixed

| Issue | Solution | Time |
|-------|----------|------|
| 🔴 Pink materials | Convert to URP Lit | 30s |
| 🌑 Broken lighting | Rebuild URP system | 20s |
| 📉 Bad terrain | Regenerate with Perlin | 40s |
| 🎨 Ugly appearance | Apply PBR materials | 30s |
| 👻 Fall-through bugs | Add colliders | 50s |
| ❌ Unknown issues | Full validation | 20s |
| 📄 No docs | Generate reports | 10s |

---

## PBR Materials Applied

```
Road:      RGB(51, 51, 51)     - Metallic 0.1, Smoothness 0.4
Sidewalk:  RGB(179, 179, 179)  - Metallic 0.0, Smoothness 0.3
Building:  RGB(204, 191, 179)  - Metallic 0.05, Smoothness 0.2
Ground:    RGB(128, 115, 102)  - Metallic 0.0, Smoothness 0.2
Water:     RGB(26, 77, 128)    - Metallic 1.0, Smoothness 0.8
```

---

## Reports Generated

After repair, check `Assets/Reports/`:
- `FULL_KOCHI_RESTORED_REPORT.md` ← Main summary
- `KOCHI_MATERIALS_APPLIED.md` ← Material list
- `LIGHTING_REBUILD_REPORT.md` ← Lighting config
- `SCENE_READY_FOR_PLAY_MODE.md` ← Play readiness

---

## Pre-Repair Checklist

- [ ] Scene is saved
- [ ] No compile errors (run play test first)
- [ ] URP is in project
- [ ] 5 minutes available
- [ ] Editor window closed except scene/inspector

---

## Post-Repair Verification

1. **Press Play** - Should start without errors
2. **Check Console** - No errors/warnings
3. **Look Around** - Scene looks realistic
4. **Move Player** - No fall-through
5. **Check Reports** - All 4 generated

---

## Individual Step Buttons

Use these if you only need specific fixes:

- **1️⃣ Fix Materials** - Just convert materials
- **2️⃣ Rebuild Lighting** - Just fix lights
- **3️⃣ Regenerate Terrain** - Just rebuild terrain
- **4️⃣ Apply PBR** - Just apply materials
- **5️⃣ Gameplay Fixes** - Just add colliders
- **6️⃣ Validate** - Just check scene
- **7️⃣ Generate Reports** - Just create docs

---

## Lighting Setup

```
Sun (Directional Light)
├─ Intensity: 1.2
├─ Color: (255, 242, 217) - Warm
├─ Rotation: 50°, -30°
└─ Casts Shadows: Yes

Ambient Light: (77, 77, 90) @ 0.8 intensity
Skybox: Procedural (Dynamic)
```

---

## Terrain Specs

```
Size:        2000 × 2000 units
Height:      0 - 300 meters
Resolution:  513 × 513
Type:        Smooth Perlin noise
Shader:      URP Terrain/Lit
Collider:    Automatic
```

---

## Troubleshooting

| Problem | Fix |
|---------|-----|
| Tool not visible | Restart Unity, force recompile (Ctrl+Shift+R) |
| Nothing changed | Check scene has objects with Renderers |
| Still too dark | Adjust RenderSettings.ambientLight in lighting |
| Reports missing | Check Assets/Reports folder exists |
| Pink still visible | Re-run Step 1, verify URP installation |

---

## Speed Tips

- **Full repair: ~3-5 minutes**
- **Materials only: ~30 seconds**
- **Lighting only: ~20 seconds**
- **Terrain only: ~40 seconds**

---

## Performance After Repair

✅ Clean scene hierarchy
✅ Optimized materials (unified URP Lit)
✅ Professional lighting
✅ No missing references
✅ Proper colliders
✅ NavMesh ready

---

**Ready to repair? Go to Tools > Time Loop City > 🔧 FULL KOCHI SCENE REPAIR**
