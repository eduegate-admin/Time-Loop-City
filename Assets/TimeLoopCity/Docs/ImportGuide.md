# TimeLoopKochi - Import & Setup Guide

Complete step-by-step instructions for importing and configuring the Hyper-Photoreal Kochi asset pack.

---

## Prerequisites

### Unity Version
```
Minimum: Unity 2021.3 LTS
Recommended: Unity 2022.3 LTS
```

### Required Packages
Install via Window → Package Manager:
- ✅ **Universal Render Pipeline** (12.0+)
- ⚠️ **HDRP** (14.0+) - Optional for photore realism
- ✅ **Cinemachine** (2.8+)
- ✅ **ProBuilder** (5.0+) - Optional

---

## Installation

### Method 1: Unity Package
1. Download `TimeLoopKochi_v1.0.unitypackage`
2. Assets → Import Package → Custom Package
3. Select all files → Import
4. Wait for compilation

### Method 2: Folder Copy
1. Extract `TimeLoopKochi.zip`
2. Copy `TimeLoopKochi/` to `Assets/`
3. Unity will auto-import

---

## Step 1: Verify Installation

### Check Folders
Navigate to `Assets/TimeLoopKochi/` and verify:
```
✅ Materials/PBR/
✅ Terrain/Heightmaps/
✅ Roads/OSM_Data/
✅ Buildings/ProceduralPrefabs/
✅ EditorTools/
✅ Docs/
```

### Check Menu Items
```
Tools → TimeLoopKochi →
  ├── Generate Terrain Heightmaps
  ├── OSM Road Importer
  ├── Procedural Building Generator (coming)
  ├── Vegetation Painter (coming)
  └── Build Demo Scene (coming)
```

If menu items missing:
- **Assets → Reimport All**
- Check Console for script errors

---

## Step 2: Generate Terrain

### 2.1 Create Heightmaps
```
Tools → TimeLoopKochi → Generate Terrain Heightmaps
```

This creates:
- `Kochi_2048.raw` (High detail)
- `Kochi_1024.raw` (Medium)
- `Kochi_512.raw` (Low/preview)

**Location**: `Assets/TimeLoopKochi/Terrain/Heightmaps/`

### 2.2 Create Unity Terrain
1. **GameObject → 3D Object → Terrain**
2. **Select Terrain** in Hierarchy
3. **Inspector → Terrain Settings**:
   - Terrain Width: 2000m
   - Terrain Length: 2000m
   - Terrain Height: 50m
   - Heightmap Resolution: 2049

### 2.3 Import Heightmap
1. **Inspector → Terrain → ⚙ → Import RAW**
2. **Select**: `Kochi_2048.raw`
3. **Settings**:
   - Depth: 16 bit (Windows byte order)
   - Resolution: 2049 x 2049
   - Terrain Size X: 2000
   - Terrain Size Y: 50
   - Terrain Size Z: 2000
4. **Click Import**

### 2.4 Verify
- Terrain should show gradual slope (coast → inland)
- Lower areas (backwater channels) visible
- Zoom in to check detail

**Troubleshooting**:
- Terrain too flat? Check heightmap depth (must be 16-bit)
- Wrong scale? Adjust Terrain Height (0-100m range)

---

## Step 3: Import Road Network

### 3.1 Open OSM Importer
```
Tools → TimeLoopKochi → OSM Road Importer
```

### 3.2 Choose Data Source

**Option A: Bundled OSM File** (Recommended)
- Path: `Assets/TimeLoopKochi/Roads/OSM_Data/kochi_extract.osm`
- Already included (fallback)
- Click **Browse** if needed

**Option B: Overpass API** (Requires Internet)
- ☑ Use Overpass API
- Bounding Box: `9.85,76.15,10.05,76.40`
- Click **Import Roads**
- ⚠️ May timeout or fail → Use Option A

### 3.3 Import Settings
- ✅ Generate Sidewalks
- ✅ Detect Bridges
- World Scale: `100`

### 3.4 Import
Click **Import Roads**

**Expected Output**:
- `OSM_Roads` GameObject in Hierarchy
- Child objects: `Road_motorway`, `Road_primary`, `Road_residential`
- 100-500+ road segments

**Fallback**:
If OSM file missing, click **Generate Fallback Road Network**
- Creates procedural grid (10x10)
- Less realistic but functional

### 3.5 Verify
- Select `OSM_Roads` in Hierarchy
- Roads should align with terrain
- Zoom in: See lane markings (if `Road_Mat` applied)

**Troubleshooting**:
- Roads underground? Adjust Y position or re-import with terrain loaded
- No materials? Run `Tools → Fix Material References`

---

## Step 4: Generate Buildings

###⏳ *Coming Soon*

```
Tools → TimeLoopKochi → Procedural Building Generator
```

**Manual Placeholder**:
1. Use existing `KochiCityGenerator` for now
2. Buildings placed along roads automatically

---

## Step 5: Add Water

### 5.1 Create Water Plane
1. **GameObject → 3D Object → Quad**
2. **Name**: `Kochi_Backwaters`
3. **Transform**:
   - Position: `(0, 0, 0)` (sea level)
   - Rotation: `(90, 0, 0)` (horizontal)
   - Scale: `(2000, 2000, 1)` (match terrain size)

### 5.2 Apply Water Material
1. **Select** `Kochi_Backwaters`
2. **Inspector → Materials**:
   - Drag `Assets/TimeLoopKochi/Water/Materials/Water_Backwater.mat`

### 5.3 Adjust (Optional)
- Water too high? Lower Y position (-1 to -2)
- Want ocean waves? Use `Water_Ocean.mat` instead

---

## Step 6: Add Vegetation

### ⏳ *Coming Soon*

```
Tools → TimeLoopKochi → Vegetation Painter
```

**Manual Placeholder**:
1. Drag prefabs from `Assets/TimeLoopKochi/Vegetation/Prefabs/`
2. Place manually along shorelines, roads

---

## Step 7: Lighting & Post-Processing

### 7.1 Directional Light (Sun)
1. **Select** `Directional Light` (default in scene)
2. **Transform**:
   - Rotation: `(55, -30, 0)` (tropical sun angle)
3. **Light**:
   - Color: Warm white `(255, 250, 240)`
   - Intensity: `1.3`
   - Shadow Type: Soft Shadows

### 7.2 Post-Processing (URP)

**Create Volume**:
1. **GameObject → Volume → Global Volume**
2. **Inspector → Volume**:
   - Profile: Create New → `Kochi_PostProcess`

**Add Effects**:
1. **Add Override → Bloom**:
   - Intensity: 0.15
   - Threshold: 1.0

2. **Add Override → Tonemapping**:
   - Mode: ACES

3. **Add Override → Color Adjustments**:
   - Temperature: +10 (warm)
   - Saturation: +5 (vibrant)

4. **Add Override → Vignette**:
   - Intensity: 0.2

### 7.3 Fog
1. **Window → Rendering → Lighting**
2. **Environment Tab** scene.
   - Fog: ☑ Enabled
   - Color: Light blue-grey
   - Mode: Exponential
   - Density: 0.005

---

## Step 8: Camera & Cinemachine

### 8.1 Main Camera
1. **Select** Main Camera
2. **Transform**:
   - Position: `(0, 10, -50)` (overview)
   - Rotation: `(10, 0, 0)` (slight tilt)

### 8.2 Add Cinemachine (Optional)
1. **Cinemachine → Create FreeLook Camera**
2. **Assign** Follow Target (Player)
3. **Adjust** damping, FOV

---

## Step 9: Build Demo Scene

### ⏳ *Coming Soon*

```
Tools → TimeLoopKochi → Build Demo Scene
```

This will auto-assemble:
- ✅ Terrain
- ✅ Roads
- ✅ Buildings
- ✅ Water
- ✅ Vegetation
- ✅ Player spawn
- ✅ Missions
- ✅ Lighting

---

## Step 10: Performance Optimization

### 10.1 Quality Settings
```
Edit → Project Settings → Quality
```

**URP Medium**:
- Shadow Resolution: Medium (1024)
- Shadow Distance: 150m
- LOD Bias: 1.5
- Texture Quality: Half Res

**URP High**:
- Shadow Resolution: High (2048)
- Shadow Distance: 300m
- LOD Bias: 2.0
- Texture Quality: Full Res

### 10.2 Occlusion Culling (Optional)
1. **Window → Rendering → Occlusion Culling**
2. **Bake Tab → Bake**
3. Wait for bake (~5-10 min)

### 10.3 Static Batching
- Mark all buildings as **Static**
- Reduces draw calls

---

## Troubleshooting

### Issue: Missing Materials
**Solution**:
```
Tools → TimeLoopKochi → Fix Material References
```

### Issue: Low FPS
**Solutions**:
- Lower Shadow Distance (100m)
- Reduce Texture Quality
- Decrease LOD Bias
- Disable occlusion culling temporarily

### Issue: Roads Not Aligned
**Solution**:
- Delete `OSM_Roads`
- Ensure terrain loaded
- Re-run OSM Importer

### Issue: Terrain Too Flat
**Solution**:
- Re-import RAW with Terrain Height: 100m
- Check byte order (Windows vs Mac)

---

## Next Steps

1. **Test Play Mode** → Explore the scene
2. **Add Player** → Import FirstPersonController or Vehicle
3. **Add Missions** → Use existing TimeLoopCity mission system
4. **Polish** → Adjust colors, add props, refine lighting

---

## Advanced Topics

- **[HDRP Upgrade](HDRP_Upgrade.md)** - Migrate to photorealism
- **[Optimization Guide]( OptimizationGuide.md)** - Performance tuning
- **[UE5 Export](UE5_Export.md)** - Cross-engine workflow

---

**Import complete!** 🎉 You're ready to build realistic Kochi scenes.
