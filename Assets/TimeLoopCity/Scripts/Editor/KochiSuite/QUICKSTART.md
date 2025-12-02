# Kochi Development Suite - Quick Start Guide

## ⚡ 30-Second Startup

### Step 1: Open Suite
```
Unity Menu → Tools → Kochi Development Suite
```

### Step 2: Click One Button
Navigate to **"Build / Validate / Optimize"** tab and click:
```
🚀 FULL KOCHI BUILD PIPELINE
```

### Step 3: Wait & Watch
Progress bar shows build completion. Entire process takes 1-3 minutes.

### Result
✅ **Complete Kochi city generated and ready for gameplay**

---

## 🎮 What You Get

After Full Build Pipeline:
- ✅ Terrain with heightmaps and backwaters
- ✅ Road network (OSM or procedural)
- ✅ Procedural buildings by district
- ✅ Water systems and vegetation
- ✅ Traffic and boat spawners
- ✅ Metro system
- ✅ Missions and clues
- ✅ NPC navigation
- ✅ Photoreal materials
- ✅ Optimized and validated

---

## 🛠️ Individual Generators

Can't wait for full build? Run individual tabs:

| Tab | Purpose | Time |
|-----|---------|------|
| Terrain | Heightmaps + Backwaters | 30s |
| Roads | OSM or grid roads | 20s |
| Buildings | Procedural architecture | 45s |
| Environment | Water + vegetation + lighting | 30s |
| Traffic | Spawners + Metro + NavMesh | 25s |
| Gameplay | Missions + clues + NPCs | 20s |
| Photoreal | PBR materials + shaders | 15s |
| Build/Validate | Optimize + repair + validate | 30s |

---

## ⚠️ If Something Goes Wrong

1. **Check Console**: See specific error
2. **Click "Fix & Validate Scene"**: Auto-repairs 90% of issues
3. **Click "Optimize Scene"**: Reduces performance problems
4. **Retry Full Build**: Usually works after repair

---

## 🎨 Customize Output

### Before Running Full Build:

1. **Terrain Tab**: Adjust heightmap resolution
2. **Roads Tab**: Enable/disable sidewalks and bridges
3. **Buildings Tab**: Set district style and count
4. **Environment Tab**: Adjust vegetation density
5. **Traffic Tab**: Set traffic and boat density

Then run **Full Build Pipeline** with your settings.

---

## 📦 Scripts Location

All suite scripts are in:
```
Assets/Editor/KochiSuite/
```

Files:
- `KochiDevelopmentSuite.cs` - Main window
- `KochiGeneratorBase.cs` - Base class
- `TerrainGenerator.cs` - Terrain systems
- `RoadsAndOSMGenerator.cs` - Roads
- `BuildingsGenerator.cs` - Buildings
- `EnvironmentGenerator.cs` - Water, vegetation, lighting
- `TrafficAndBoatsGenerator.cs` - Transportation
- `GameplayMissionsGenerator.cs` - Gameplay
- `PhotorealUpgradeGenerator.cs` - Materials & shaders
- `BuildValidateOptimizeGenerator.cs` - Build pipeline

---

## 🚀 Next Steps

After build completes:

1. ✅ **Play Test**: Press Play in Unity and explore generated city
2. ✅ **Iterate**: Adjust settings and re-run individual tabs
3. ✅ **Customize**: Edit scripts to add custom elements
4. ✅ **Optimize**: Run "Optimize Scene" before final build
5. ✅ **Export**: Build for your target platform

---

## 💡 Pro Tips

- **Save Often**: Create backup scenes before major changes
- **Test Individually**: Run one generator at a time while learning
- **Tweak Materials**: Edit PBR materials after generation
- **Debug Issues**: Enable "Debug Logs" in footer for verbose output
- **Batch Operations**: Full build is most efficient for first-time setup

---

## 🎯 Common Workflows

### Workflow A: First-Time Setup
```
1. Open Kochi Development Suite
2. Run "Full Kochi Build Pipeline"
3. Play and enjoy! 🎉
```

### Workflow B: Iterative Design
```
1. Run "Full Build Pipeline"
2. Adjust settings in individual tabs
3. Run specific generators (e.g., just Buildings)
4. Re-run "Optimize Scene"
5. Repeat 2-4 until satisfied
```

### Workflow C: Quick Fix
```
1. Scene has issues
2. Run "Fix & Validate Scene"
3. Most common issues auto-repaired
4. Continue working
```

---

## ❓ FAQ

**Q: How long does full build take?**
A: Usually 1-3 minutes depending on hardware and density settings.

**Q: Can I customize generated content?**
A: Yes! Edit individual generator classes to add unique features.

**Q: Will it work with existing scenes?**
A: It can! Generators add content to current scene (won't overwrite).

**Q: Is it optimized for mobile?**
A: Yes! All generators create LODs and support mobile optimization.

**Q: Can I use the generators separately?**
A: Yes! Each tab is independent and can run without others.

---

## 📖 Full Documentation

See `README.md` in this folder for:
- Detailed tab descriptions
- Parameter explanations
- Developer guide
- Troubleshooting
- Performance tips
- Asset references

---

**Ready? Open the suite and start building!** 🏗️
