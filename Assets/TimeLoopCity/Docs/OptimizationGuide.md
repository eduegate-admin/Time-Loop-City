# TimeLoopKochi Package - Optimization Guide

## Performance Optimization for Photoreal Kochi Pack

This guide helps you achieve **60 FPS @ 1080p** on mid-range GPUs while maintaining photorealistic quality.

---

## 1. Level of Detail (LOD) Configuration

### Building LODs
All procedural buildings should have 3 LOD levels:

- **LOD0 (0-50m)**: Full detail, all windows, balconies, decorations
- **LOD1 (50-150m)**: Simplified geometry, baked window textures
- **LOD2 (150m+)**: Box collider with normal map

**Setup:**
```csharp
// In ProceduralBuildingGenerator, add LOD group
LODGroup lodGroup = building.AddComponent<LODGroup>();
LOD[] lods = new LOD[3];
lods[0] = new LOD(0.5f, detailedRenderers);  // 50% screen height
lods[1] = new LOD(0.15f, simplifiedRenderers); // 15% screen height
lods[2] = new LOD(0.05f, boxRenderers);        // 5% screen height
lodGroup.SetLODs(lods);
```

### Vegetation LODs
- **LOD0 (0-30m)**: Full 3D mesh
- **LOD1 (30-100m)**: Billboard cross-fade
- **Billboard (100m+)**: Single quad with alpha

---

## 2. Occlusion Culling

### Bake Occlusion Data
1. **Window → Rendering → Occlusion Culling**
2. Mark large buildings as **Occluder Static**
3. **Bake Tab → Bake** (wait 5-15 minutes)

**Settings:**
- Smallest Occluder: 5 (buildings)
- Smallest Hole: 0.25 (windows)
- Backface Threshold: 100

**Result:** ~30-50% draw call reduction in dense urban areas

---

## 3. Texture Streaming

### Enable Texture Streaming
```
Edit → Project Settings → Quality
✅ Texture Streaming
Memory Budget: 2048 MB (adjust for your GPU)
```

### Texture Import Settings
For all PBR textures:
- **Max Size**: 2048 (hero), 1024 (standard), 512 (background)
- **Compression**: DXT5 (Normal maps), DXT1 (Albedo)
- ✅ **Generate Mip Maps**
- ✅ **Streaming Mip Maps**

**Result:** Reduces VRAM usage by 40-60%

---

## 4. Static Batching & GPU Instancing

### Static Batching
Mark all non-moving objects as **Static**:
- Buildings
- Roads
- Props
- Vegetation (if not using GPU instancing)

### GPU Instancing
For repeated objects (trees, street lights):
1. **Material → Enable GPU Instancing** (checkbox)
2. Use same material for all instances
3. Ensure mesh is identical

**Result:** Reduces draw calls from 5000+ to 500-1000

---

## 5. Shadow Optimization

### Quality Settings
```
Edit → Project Settings → Quality → Shadows
```

**Recommended (URP):**
- Shadow Distance: **150m** (Medium), **300m** (High)
- Shadow Cascades: **Two Cascades**
- Shadow Resolution: **2048** (High), **1024** (Medium)

**Performance Impact:**
- Reducing shadow distance from 500m → 150m: **+20 FPS**
- Reducing resolution 4096 → 2048: **+10 FPS**

---

## 6. Post-Processing Budget

### Disable Expensive Effects (if FPS < 60)
- ❌ Screen Space Ambient Occlusion (SSAO): -15 FPS
- ❌ Screen Space Reflections (SSR): -20 FPS
- ⚠️ Depth of Field: -5 FPS
- ✅ Bloom: -2 FPS (keep, looks great!)
- ✅ Tonemapping: -0 FPS (keep, essential)

### Recommended Minimal Profile
```
✅ Tonemapping (ACES)
✅ Color Grading (warm tropical)
✅ Bloom (intensity 0.15)
⚠️ Vignette (cheap, keep if desired)
❌ SSAO (disable for +15 FPS)
```

---

## 7. Lighting Optimization

### Baked Lighting (Best Performance)
1. Mark all buildings as **Lightmap Static**
2. **Window → Rendering → Lighting**
3. **Generate Lighting** (takes 30-60 min)

**Result:** Real-time lights → baked: **+30 FPS**

### Mixed Lighting (Balance)
- Sun: **Mixed** (baked shadows, realtime color)
- Street lights: **Baked**
- Player flashlight: **Realtime**

---

## 8. Mesh Optimization

### Reduce Poly Count
**Target Triangle Counts:**
- Building LOD0: 5,000-10,000 tris
- Building LOD1: 1,000-3,000 tris
- Building LOD2: 100-500 tris
- Vegetation: 500-2,000 tris (LOD0)

### Auto-Simplification
Use Unity's Mesh Simplifier:
```csharp
// In editor script
Mesh simplified = UnityMeshSimplifier.MeshSimplifier.SimplifyMesh(
    originalMesh, 
    quality: 0.5f // 50% reduction
);
```

---

## 9. Profiling & Bottleneck Identification

### Unity Profiler
```
Window → Analysis → Profiler
```

**Check:**
- **CPU**: Look for spikes in `Rendering.Mesh`, `Scripts.Update`
- **GPU**: Check `GPU Frame Time` (should be < 16ms for 60 FPS)
- **Memory**: Texture memory, mesh memory

### Common Bottlenecks
| Symptom | Cause | Solution |
|---------|-------|----------|
| High CPU rendering | Too many draw calls | Enable batching, combine meshes |
| High GPU time | Too many triangles | Reduce poly count, improve LODs |
| Stuttering | Texture loading | Enable texture streaming |
| Low FPS in vegetation | Too many trees | Use GPU instancing, billboards |

---

## 10. Platform-Specific Settings

### PC (Mid-Range: GTX 1060 / RX 580)
- Resolution: 1080p
- Quality: **Medium**
- Shadow Distance: 150m
- SSAO: ❌ Disabled
- Target: **60 FPS**

### PC (High-End: RTX 3060+)
- Resolution: 1080p / 1440p
- Quality: **High/Ultra**
- Shadow Distance: 300m
- SSAO: ✅ Enabled
- DLSS/FSR: ✅ Enabled (if available)
- Target: **60 FPS** @ 4K with upscaling

---

## 11. Memory Budget

**Total Budget (PC):**
- Textures: **2 GB** (streaming)
- Meshes: **500 MB**
- Audio: **100 MB**
- Scripts/Other: **400 MB**
- **Total: ~3 GB**

**Exceed budget?**
- ✂️ Reduce texture resolution (4K → 2K)
- ✂️ Compress audio (WAV → MP3 128kbps)
- ✂️ Remove unused assets

---

## 12. Quality Presets

### Low (30 FPS on integrated GPU)
```
Shadow Distance: 50m
Texture Quality: Half Res
LOD Bias: 0.5
SSAO: Off
Bloom: Off
VSync: Off
```

### Medium (60 FPS on GTX 1060)
```
Shadow Distance: 150m
Texture Quality: Full Res
LOD Bias: 1.0
SSAO: Off
Bloom: On (low intensity)
VSync: On
```

### High (60 FPS on RTX 3060)
```
Shadow Distance: 300m
Texture Quality: Full Res
LOD Bias: 2.0
SSAO: On
Bloom: On
Post-Processing: All enabled
VSync: On
```

### Ultra (60 FPS on RTX 4070+ with DLSS)
```
Shadow Distance: 500m
Texture Quality: Very High
LOD Bias: 2.0
SSAO: On (high quality)
SSR: On
All Post-FX: Max
DLSS: Quality Mode
```

---

## Quick Wins Summary

| Optimization | FPS Gain | Effort |
|--------------|----------|--------|
| Enable GPU Instancing | +15 FPS | Low |
| Reduce Shadow Distance (500→150m) | +20 FPS | Low |
| Disable SSAO | +15 FPS | Low |
| Enable Occlusion Culling (baked) | +10 FPS | Medium |
| Bake Lighting | +30 FPS | High |
| Improve Building LODs | +10 FPS | Medium |
| Enable Texture Streaming | +5 FPS | Low |

**Total Potential Gain: +105 FPS** (from unoptimized baseline)

---

**Target achieved:** 60 FPS @ 1080p on RTX 3060 / RX 6600 XT with Medium-High settings.
