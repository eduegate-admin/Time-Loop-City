# TimeLoopKochi - Hyper-Photoreal Asset Pack

**Version**: 1.0  
**Target**: Unity 2021.3+ (URP/HDRP compatible)  
**Quality**: UE5-comparable photoreal visuals

---

## 📦 Package Contents

```
TimeLoopKochi/
├── Materials/PBR/          # 4K/2K/1K PBR material library
├── Terrain/Heightmaps/     # Kochi coastal heightmaps (3 LODs)
├── Roads/OSM_Data/         # OpenStreetMap integration
├── Buildings/              # Procedural building prefabs
├── Vegetation/             # Palm trees, mangroves, tropical plants
├── Water/                  # Backwater shader & materials
├── Audio/Music/            # Ambient Kerala-inspired music
├── Scenes/Demo/            # Playable demo scene
├── EditorTools/            # Generation & import tools
└── Docs/                   # This file + guides
```

---

## 🚀 Quick Start

### 1. Import Package
- Extract to `Assets/TimeLoopKochi/`
- Unity will auto-import assets

### 2. Generate Terrain
```
Tools → TimeLoopKochi → Generate Terrain Heightmaps
```
- Creates 2048/1024/512 RAW heightmaps
- Import via Unity Terrain Tools

### 3. Import Roads
```
Tools → TimeLoopKochi → OSM Road Importer
```
- **Option A**: Use bundled OSM extract (fallback)
- **Option B**: Fetch from Overpass API (requires internet)

### 4. Generate Buildings
```
Tools → TimeLoopKochi → Procedural Building Generator
```
- Select district style: Colonial, Modern, Industrial
- Batch-generate prefabs with LODs

### 5. Build Demo Scene
```
Tools → TimeLoopKochi → Build Demo Scene
```
- Auto-assembles terrain, roads, buildings, water
- Places player spawn & missions
- Configures lighting & post-processing

---

##  System Requirements

### Minimum
- **GPU**: GTX 1060 6GB
- **CPU**: i5-8400
- **RAM**: 16 GB
- **Unity**: 2021.3 LTS
- **Render Pipeline**: URP 12.0+

### Recommended (HDRP)
- **GPU**: RTX 3060 12GB
- **CPU**: i7-10700
- **RAM**: 32 GB
- **Unity**: 2022.3 LTS
- **Render Pipeline**: HDRP 14.0+

### Performance Targets
- **1080p High**: 60 FPS (URP)
- **1080p Ultra**: 60 FPS (HDRP + DLSS)
- **4K High**: 60 FPS (HDRP + DLSS)

---

## 🎨 Features

### Terrain System
- **Heightmaps**: Procedur ally generated Kochi coastal topology
- **LODs**: 3 resolution levels (2048/1024/512)
- **Streaming**: Texture streaming support
- **Format**: 16-bit RAW

### Road Network
- **Source**: OpenStreetMap (Kochi, India)
- **Types**: Motorway, Primary, Residential, Bridges
- **Geometry**: Sidewalks, curbs, lane markings
- **Alignment**: Auto-aligned to terrain heightmap

### Buildings
- **Styles**: Colonial Fort Kochi, Modern Marine Drive, Industrial Willingdon
- **LODs**: 3 levels per building (auto-generated)
- **Materials**: District-specific PBR materials
- **Features**: Balconies, windows, rooftop details

### PBR Materials
- **Textures**: Albedo, Normal, Metallic, Roughness, AO, Height
- **Resolutions**: 4K (hero), 2K (standard), 1K (LOD)
- **Library**: 12+ materials (roads, concrete, brick, glass, water, soil)
- **Format**: PNG (lossless)

### Water
- **Backwaters**: Calm reflective water shader
- **Ocean**: Tidal wave animation
- **Foam**: Shore line foam decals
- **Shader**: URP/HDRP compatible

### Vegetation
- **Types**: Palm, Coconut, Mangrove, Tropical shrubs
- **LODs**: Billboard crossfade at 50m+
- **Instancing**: GPU instanced for performance
- **Placement**: Brush-based painting tool

### Audio
- **Music**: 3x Kerala-inspired ambient tracks (30-90s loops)
- **SFX**: Seagulls, boat horns, market ambience, traffic
- **Format**: MP3/WAV
- **License**: Royalty-free

### Lighting (HDRP)
- **Volumetric Fog**: Tropical humidity effect
- **Contact Shadows**: Enhanced shadow detail
- **Bloom**: Sun glare & reflections
- **Color Grading**: Warm tropical color palette
- **Sky**: HDRI backdrops (sunset, overcast)

---

## 📚 Documentation

### Guides
- **[ImportGuide.md](ImportGuide.md)**: Step-by-step setup
- **[OptimizationGuide.md](OptimizationGuide.md)**: Performance tuning
- **[MaterialGuide.md](MaterialGuide.md)**: PBR workflow

### Troubleshooting
- Missing textures? Run `Tools → Fix Material References`
- Low FPS? Reduce LOD bias in Quality Settings
- Roads not aligned? Re-import OSM with terrain heightmap loaded

---

## 🔧 Editor Tools

### 1. Terrain Generator
```
Tools → TimeLoopKochi → Generate Terrain Heightmaps
```
Generates procedural coastal heightmaps (fallback for SRTM data).

### 2. OSM Importer
```
Tools → TimeLoopKochi → OSM Road Importer
```
Imports OpenStreetMap roads with lane classification and bridge detection.

### 3. Building Generator
```
Tools → TimeLoopKochi → Procedural Building Generator
```
Creates district-specific building prefabs with auto-LODs.

### 4. Vegetation Painter
```
Tools → TimeLoopKochi → Vegetation Painter
```
Brush-based vegetation placement with density control.

### 5. Scene Builder
```
Tools → TimeLoopKochi → Build Demo Scene
```
One-click demo scene assembly with all systems integrated.

---

## 📄 Licenses

### Assets
- **Textures**: CC0 (PoliigonTextures, AmbientCG)
- **Models**: Custom (free for non-commercial use)
- **Audio**: Royalty-free (FreeMusicArchive CC-BY)
- **Code**: MIT License

### Third-Party
- OpenStreetMap data: ODbL
- Unity packages: Unity Asset Store EULA

See **[LICENSE.txt](LICENSE.txt)** for full details.

---

## 🎯 Workflow

### Step-by-Step Scene Creation

1. **Create New Scene**
   - File → New Scene
   - Save as `MyKochiScene.unity`

2. **Add Terrain**
   - GameObject → 3D Object → Terrain
   - Import heightmap: `Kochi_2048.raw`

3. **Import Roads**
   - Run OSM Importer
   - Select terrain to align roads

4. **Generate Buildings**
   - Run Building Generator
   - Choose district presets
   - Place prefabs along roads

5. **Add Water**
   - Create plane at Y=0
   - Apply `Water_Backwater` material

6. **Paint Vegetation**
   - Run Vegetation Painter
   - Use shoreline & urban presets

7. **Configure Lighting**
   - Add Directional Light (sun)
   - Apply `Kochi_PostProcess` profile
   - Position camera for best angle

8. **Test Performance**
   - Stats panel: Target 60 FPS
   - Adjust LOD distances if needed

---

## 🌟 Advanced Features

### URP → HDRP Migration
See **[HDRP_Upgrade.md](HDRP_Upgrade.md)** for:
- Material conversion
- Lighting setup
- Post-processing profiles
- Performance comparison

### Runtime Streaming
For large cities:
- Use Addressables for district chunks
- Stream terrain patches
- Async building loading

### UE5 Export
Materials & meshes compatible with Unreal Engine 5:
- Export FBX models
- Convert textures to UE5 format
- See **[UE5_Export.md](UE5_Export.md)**

---

## 🐛 Known Issues

1. **OSM API requires internet** → Use bundled extract
2. **SRTM data unavailable** → Procedural fallback active
3. **HDRP requires powerful GPU** → Use URP for mid-range

---

## 📞 Support

- **Issues**: GitHub Issues
- **Documentation**: [Wiki](wiki_url)
- **Community**: Discord server

---

**Created with ❤️ for realistic Kochi city generation**
