# UE5 Export Guide - Kochi Photoreal Pack

**Purpose**: Export Unity assets for use in Unreal Engine 5

---

## What Can Be Exported

### ✅ Exportable Assets
- **Heightmaps** (RAW files) → UE5 Landscape
- **Building Meshes** (FBX) → UE5 Static Meshes
- **PBR Textures** (PNG/TGA) → UE5 Materials
- **Road Splines** (positions) → UE5 Landscape Splines
- **Vegetation Prefabs** (FBX) → UE5 Foliage

### ❌ Non-Exportable (Unity-Specific)
- C# Scripts → Rewrite in Blueprint/C++
- Unity Materials → Recreate in UE5 Material Editor
- Prefab hierarchy → Manually rebuild in UE5

---

## Step 1: Export Terrain Heightmap

### From Unity
1. **Select Terrain** in scene
2. **Inspector → Terrain → ⚙ → Export Raw**
3. **Settings**:
   - Depth: 16-bit
   - Byte Order: Windows
   - Resolution: 2049 x 2049
4. **Save as**: `Kochi_Terrain_UE5.raw`

### Import to UE5
1. **Landscape Mode** → Create New  
2. **Import from File** → Select `Kochi_Terrain_UE5.raw`
3. **Settings**:
   - Scale: Z = 100 (adjust for height)
   - Sections: 127 x 127 quads
   - Component Count: 8 x 8
4. **Import** → Terrain created!

**Tip**: UE5 landscapes use different scale. Test with small area first.

---

## Step 2: Export Building Meshes

### From Unity (per building)
1. **Select building prefab**
2. **File → Export → FBX**
3. **Settings**:
   - ☑ Include: Meshes, Materials (as references)
   - ☐ Bake Animations: Disable
   - Scale: 1.0
4. **Export to**: `KochiBuildings_UE5/FortKochi_01.fbx`

### Batch Export Script (Unity Editor)
```csharp
// Place in Editor folder
using UnityEngine;
using UnityEditor;

public class UE5MeshExporter
{
    [MenuItem("Tools/UE5/Export All Buildings")]
    static void ExportBuildings()
    {
        string exportPath = "UE5_Export/Buildings/";
        System.IO.Directory.CreateDirectory(exportPath);
        
        var buildings = AssetDatabase.FindAssets("t:GameObject", 
            new[] { "Assets/TimeLoopKochi/Buildings/ProceduralPrefabs" });
        
        foreach (var guid in buildings)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            string filename = prefab.name + ".fbx";
            
            // Use Unity FBX Exporter package or external tool
            Debug.Log($"Would export: {filename}");
        }
    }
}
```

**Note**: Install **Unity FBX Exporter** package for best results.

### Import to UE5
1. **Content Browser → Import**
2. **Select** all FBX files
3. **Import Settings**:
   - Mesh: Static Mesh
   - Combine Meshes: Yes
   - Generate Lightmap UVs: Yes
   - LODs: Import (if embedded)
4. **Import All**

---

## Step 3: Export & Convert Textures

### Unity → UE5 Texture Mapping

| Unity (URP/HDRP) | UE5 Material |
|------------------|--------------|
| _BaseMap (Albedo) | Base Color |
| _NormalMap | Normal |
| _Metallic | Metallic |
| _Smoothness → **Invert!** | Roughness |
| _OcclusionMap | Ambient Occlusion |
| _HeightMap | Height (Displacement) |

### Export Textures
1. **Select texture** in Unity Project
2. **Right-click → Export Package** (individual)
3. Or copy directly from `Assets/TimeLoopKochi/Materials/PBR/Textures/`

### Convert Smoothness → Roughness
**Important**: UE5 uses Roughness, Unity uses Smoothness (inverted!)

**Conversion**:
- In Photoshop: `Image → Adjustments → Invert (Ctrl+I)`
- Or use UE5 Material Editor: `1 - Smoothness`

### Import to UE5
1. **Content Browser → Import**
2. **Texture Settings** (per texture type):
   - **Base Color**: sRGB ☑, Compression: Default
   - **Normal**: sRGB ☐, Compression: Normalmap
   - **Roughness**: sRGB ☐, Compression: Masks
   - **AO**: sRGB ☐, Compression: Masks

---

## Step 4: Create UE5 Materials

### Material Setup (Example: Building)
1. **Content Browser → Add → Material**
2. **Name**: `M_Building_Colonial`
3. **Material Editor**:
   - **Base Color** ← Texture Sample (Albedo)
   - **Normal** ← Texture Sample (Normal)
   - **Metallic** ← Texture Sample (Metallic, R channel)
   - **Roughness** ← Texture Sample (Roughness converted from Smoothness)
   - **Ambient Occlusion** ← Texture Sample (AO, R channel)
4. **Apply to mesh**

### Material Instance (for variations)
1. **Right-click material → Create Material Instance**
2. **Adjust** color tint, roughness multiplier
3. **Apply** to different building variants

---

## Step 5: Export Road Network

### Unity Roads → UE5 Landscape Splines

**Manual Method** (recommended for small cities):
1. **Document road positions** from Unity (X, Z coordinates)
2. **In UE5**: Use **Landscape Spline Tool**
3. **Place control points** matching Unity positions
4. **Adjust** width, falloff, materials

**Automated Method** (advanced):
1. Export road spline data as JSON from Unity
2. Write UE5 Python script to import spline points
3. Auto-generate landscape splines

**File**: `RoadExportData.json`
```json
{
  "roads": [
    {
      "type": "primary",
      "width": 10,
      "points": [[100, 0, 50], [200, 0, 50], [300, 0, 60]]
    }
  ]
}
```

---

## Step 6: Lighting Conversion

| Unity HDRP | UE5 Equivalent |
|------------|----------------|
| Directional Light | Directional Light (Sun) |
| HDRI Sky | Skylight (HDRI) |
| Volumetric Fog | Exponential Height Fog |
| Reflection Probes | Reflection Captures |

### Post-Processing (UE5)
1. **Place Post Process Volume** (infinite bounds)
2. **Settings**:
   - Bloom: Intensity 0.15
   - Tonemapper: ACES
   - Color Grading: Temp +10, Tint +5
   - Vignette: Intensity 0.2

---

## Step 7: Performance Settings (UE5)

### Lumen (UE5 GI)
- **Global Illumination**: Lumen (or baked lighting for performance)
- **Reflections**: Lumen SSR

### Nanite (UE5 Virtualized Geometry)
- **Enable Nanite** on high-poly building meshes
- **LODs**: Automatic with Nanite!

### Virtual Shadow Maps
- **Directional Light → Use Virtual Shadow Maps**: ☑
- **Better shadow quality** than cascaded shadow maps

**Performance Target**: 60 FPS @ 1080p on RTX 3060 (same as Unity)

---

## Step 8: Vegetation Export

### Unity → UE5 Foliage
1. **Export vegetation FBX** (palms, mangroves)
2. **Import to UE5**
3. **Create Foliage Type**:
   - Settings → Cull Distance: 5000
   - LOD setup: Use billboards at distance
4. **Paint with Foliage Tool**

**GPU Instancing**: Automatic in UE5 with Foliage Tool!

---

## Comparison: Unity vs UE5

| Feature | Unity HDRP | UE5 |
|---------|------------|-----|
| Global Illumination | Baked + Realtime Probes | Lumen (realtime) |
| LOD System | Manual LODs | Nanite (automatic) |
| Shadows | Cascaded SM | Virtual Shadow Maps |
| Reflections | SSR | Lumen SSR |
| Performance (RTX 3060) | 60 FPS | 60 FPS (with DLSS) |

---

## Troubleshooting

### Issue: Textures too dark in UE5
**Solution**: Check sRGB settings. Base Color should be sRGB ☑.

### Issue: Smoothness looks wrong
**Solution**: Invert texture (Smoothness → Roughness conversion).

### Issue: Terrain height mismatch
**Solution**: Adjust UE5 Landscape Z scale (try 50-200).

### Issue: Building scale wrong
**Solution**: FBX export scale should be 1.0. Adjust in UE5 import settings.

---

## Final Checklist

- [ ] Terrain heightmap imported and scaled correctly
- [ ] All building FBX files exported and imported
- [ ] Textures converted (Smoothness → Roughness)
- [ ] Materials created with PBR inputs
- [ ] Roads recreated manually or via script
- [ ] Lighting matches Unity (sun angle, fog, PP)
- [ ] Vegetation placed with Foliage Tool
- [ ] Performance optimized (Lumen, Nanite)

---

**Note**: UE5 has different workflow than Unity. Some manual recreation required. This guide focuses on asset transfer, not 1:1 scene recreation.

**Recommended**: Use Unity for rapid prototyping, UE5 for final cinematic quality (if target platform supports it).
