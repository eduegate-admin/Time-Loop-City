# Release Preparation & Polish Plan

## 1. 🎨 Visual Polish
### Post-Processing (Unity Post-Processing Stack / Volume)
*   **Bloom**: Threshold 0.9, Intensity 1.2 (Soft glow for neon signs).
*   **Vignette**: Intensity 0.25, Smoothness 0.4 (Focus player attention).
*   **Color Grading**:
    *   **Tonemapping**: ACES.
    *   **Saturation**: +10 (Vibrant city).
    *   **Contrast**: +15 (Deep shadows).
*   **Film Grain**: Intensity 0.05 (Subtle cinematic feel).

### Lighting & Atmosphere
*   **Sun**: Realtime Directional Light with soft shadows.
*   **Ambient**: Gradient (Sky: Dark Blue, Equator: Purple, Ground: Black).
*   **Fog**: Exponential Squared, Density 0.002, Color: Dark Blue (Depth).

### UI Polish
*   **Animations**: Tween scale on hover (1.0 -> 1.1).
*   **Shadows**: Add `Shadow` component to text, distance (2, -2).
*   **Transparency**: Use CanvasGroup for fade in/out.

## 2. 🔊 Audio Polish
### Architecture
*   **AudioCue**: ScriptableObject defining a sound (Clip, Volume, Pitch Variation).
*   **Mixer Groups**: Master -> [Music, SFX, UI, Ambience].

### Asset List
*   `Ambience_CityLoop`: Distant traffic, wind.
*   `SFX_MissionComplete`: Positive chime.
*   `SFX_LoopReset`: Rising whoosh + heavy impact.
*   `SFX_Evidence`: Paper rustle / Camera shutter.
*   `UI_Hover`: High tick.
*   `UI_Click`: Mechanical click.
*   `Music_Main`: Mystery/Noir theme.
*   `Music_Ending`: Emotional piano.

## 3. ⚡ Performance Audit
### Optimization Targets
*   **Physics**: Reduce `Default Solver Iterations` to 4. Check Layer Collision Matrix.
*   **Garbage Collection**:
    *   `TrafficSystem`: Implement Object Pooling for cars.
    *   `NPCController`: Cache `WaitForSeconds`.
    *   `Update()`: Remove string concatenations in loops.
*   **Rendering**:
    *   **Static Batching**: Mark buildings/roads as Static.
    *   **Occlusion Culling**: Bake for the city.
    *   **Shadows**: Reduce distance to 80m on Mobile, 150m on PC.

## 4. 📢 Store Preparation
*   See `STORE_LISTING.md` for full copy.
