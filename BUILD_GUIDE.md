# Time Loop City - Build & Release Guide

## 🏗️ Build Settings

### 1. Platform Selection
1.  Go to **File > Build Settings**.
2.  Select your target platform:
    *   **PC/Mac/Linux Standalone**: Best for high-fidelity visuals and performance.
    *   **Android / iOS**: Requires optimization (see below).
3.  Click **Switch Platform** if needed.

### 2. Scene Management
Ensure scenes are added in the correct order:
1.  **0 - MainMenu** (If you created one, otherwise skip)
2.  **1 - MainCity** (The core game scene)

> **Tip**: Drag the `MainCity.unity` file from the Project window into the "Scenes In Build" list.

### 3. Player Settings
Go to **Edit > Project Settings > Player**.

#### **Company & Product Name**
*   **Company Name**: Your Name / Studio
*   **Product Name**: Time Loop City
*   **Version**: 1.0.0

#### **Resolution & Presentation**
*   **Fullscreen Mode**: Fullscreen Window
*   **Default Is Native Resolution**: Checked
*   **Run In Background**: Checked (prevents game pausing if player alt-tabs, optional)

#### **Other Settings (Critical)**
*   **Color Space**: Linear (Better lighting)
*   **Scripting Backend**: 
    *   **Windows/Mac**: Mono (Faster builds) or IL2CPP (Better performance/Security)
    *   **Mobile**: IL2CPP (Required for 64-bit stores)
*   **Api Compatibility Level**: .NET Standard 2.1

---

## ⚡ Optimization Settings

### 1. Quality Settings
Go to **Edit > Project Settings > Quality**.

*   **Pixel Light Count**: 4 (Balance visual fidelity vs performance)
*   **Texture Quality**: Full Res
*   **Anisotropic Textures**: Per Texture
*   **Anti-Aliasing**: 2x or 4x Multi Sampling
*   **Shadows**:
    *   **Shadow Distance**: 150 (City scale)
    *   **Shadow Cascades**: 2 Cascades
*   **VSync Count**: Don't Sync (Allow player to choose, or set target frame rate in code)

### 2. Physics Optimization
Go to **Edit > Project Settings > Physics**.

*   **Default Solver Iterations**: 6 (Default) -> Reduce to 4 if CPU bound.
*   **Auto Simulation**: Checked.
*   **Layer Collision Matrix**:
    *   Disable collisions between layers that don't interact (e.g., `Traffic` vs `Debris`).
    *   Ensure `Player` collides with `Interactable`, `Ground`, `Walls`.
    *   Ensure `NPC` collides with `Ground`, `Walls`, `Player`.

### 3. AI & Navigation
*   **NavMesh**:
    *   Use **NavMeshSurface** (if using AI Navigation package) for runtime baking if needed.
    *   Otherwise, ensure NavMesh is **Baked** before building.
    *   **Voxel Size**: Increase (0.1 -> 0.2) to reduce bake data size if precision isn't critical.

### 4. IL2CPP (Advanced)
If using IL2CPP:
*   **C++ Compiler Configuration**: Release
*   **Strip Engine Code**: Checked (Reduces build size significantly)

---

## ✅ Pre-Build Validation Checklist

Before clicking "Build", verify the following:

- [ ] **No Console Errors**: Clear the console and play the game. Ensure no red errors appear.
- [ ] **Bake Lighting**: Go to **Window > Rendering > Lighting > Generate Lighting**. Wait for it to finish.
- [ ] **Bake NavMesh**: Go to **Window > AI > Navigation > Bake**.
- [ ] **Content Generation**: Run `ContentGenerator` one last time to ensure all ScriptableObjects are up to date.
- [ ] **Starting State**: Ensure the Player is positioned at the **Spawn Point** in the scene, or that the `TimeLoopManager` handles spawning correctly on start.
- [ ] **UI Scaling**: Check Game View with "Scale" slider. Ensure UI anchors are correct for different aspect ratios (16:9, 21:9).

---

## 📦 Building the Game

1.  Open **Build Settings** (`Ctrl+Shift+B`).
2.  Click **Build**.
3.  Create a new folder named `Builds/` outside your Assets folder.
4.  Create a subfolder for the version, e.g., `Builds/Windows_v1.0/`.
5.  Name your executable (e.g., `TimeLoopCity.exe`).
6.  Click **Save**.

**Wait for the build process to complete.** This may take a few minutes.

---

## 🚀 Publishing & Distribution

### 1. Testing the Build
*   Run the generated `.exe` (or app).
*   **Critical Test**: Play through one full loop reset. Ensure the game doesn't crash and the world randomizes correctly.
*   **Check Logs**: If it crashes, check `Player.log` in the AppData folder (Windows).

### 2. Packaging
*   **Zip the Folder**: Compress the entire build folder (containing the `.exe` and the `_Data` folder). Do NOT just send the `.exe`.

### 3. Platforms
*   **Itch.io**: Upload the Zip file. Use Butler for command-line uploads if automating.
*   **Steam**: Requires Steamworks SDK integration (not covered here).
*   **Google Play**: Requires Android App Bundle (.aab) build and key signing.

---

## 🛠️ Troubleshooting Common Build Issues

*   **"Shader not supported"**: Ensure your materials use shaders compatible with the target platform (Standard or URP Lit).
*   **"Missing Script"**: You deleted a script but it's still attached to a GameObject. Check the console.
*   **"Lighting is dark"**: You forgot to Generate Lighting.
*   **"UI is tiny/huge"**: Check `Canvas Scaler` component. Set "UI Scale Mode" to "Scale With Screen Size".

---

**Congratulations! You are ready to ship Time Loop City!** 🕒🏙️
