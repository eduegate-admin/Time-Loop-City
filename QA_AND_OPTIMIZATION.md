# Time Loop City - QA & Optimization Report

## 1. Code Testing Automation

### **Time Loop System**
- **Test**: Verify `CurrentLoopCount` increments after `loopDuration`.
- **Test**: Verify `OnLoopReset` event fires exactly once per loop.
- **Test**: Verify Player position resets to `PlayerSpawnPoint`.
- **Fix**: Ensure `isResetting` flag prevents double-triggering of reset logic.

### **Persistence System**
- **Test**: Save a clue, restart game, verify clue exists.
- **Test**: Verify `SaveLoadManager` handles corrupted JSON gracefully.
- **Fix**: Added try-catch block in `LoadGame()` to prevent crash on bad save data.

### **Event System**
- **Test**: Spawn 100 events rapidly.
- **Test**: Verify events despawn on loop reset.
- **Fix**: Use object pooling for particles to avoid GC spikes.

---

## 2. Development Stage Workflow

### **Current Stage: Implementation (Phase 1)**
- [x] Core Systems (Time, World, Player)
- [x] Basic AI (Patrol)
- [x] UI Foundation
- [ ] **Next**: Mission System Integration
- [ ] **Next**: Advanced NPC Interactions

---

## 3. QA + Bug Finding

### **Potential Bugs**
1.  **Race Condition**: `TimeLoopManager` might reset before `WorldStateManager` is ready.
    *   *Fix*: Use `Script Execution Order` to ensure Managers initialize first.
2.  **NavMesh Issues**: NPCs might get stuck on dynamic obstacles (Events).
    *   *Fix*: Add `NavMeshObstacle` component to Event prefabs with "Carve" enabled.
3.  **UI Flicker**: Time display might flicker between loops.
    *   *Fix*: Update UI in `LateUpdate` or verify data validity before display.

### **Test Checklist**
- [ ] Play for 10 minutes (2 full loops).
- [ ] Interact with 5 different clues.
- [ ] Save/Load game.
- [ ] Trigger all 5 event types.
- [ ] Talk to an NPC in Loop 1 vs Loop 5.

---

## 4. Optimization Passes

### **Memory & GC**
- **String Concatenation**: Avoid `text.text = "Loop: " + count` in Update. Use `StringBuilder` or cache strings.
- **Linq Usage**: Removed `ToList()` calls in hot paths.

### **Physics & Rendering**
- **Raycasts**: Limited to `Interactable` layer (Layer 6).
- **Shadows**: Disable shadows on small props (clues).
- **Lighting**: Use Baked lighting for static city geometry, Realtime only for Sun.

---

## 5. Game Polish & Feel

### **Visuals**
- **Loop Transition**: Add a "glitch" effect shader during the fade.
- **Time of Day**: Smooth out the light color gradient (add dawn/dusk colors).

### **Audio**
- **Ambience**: Add city background noise that changes with time of day.
- **UI SFX**: Add "click" and "hover" sounds to menus.

---

## 6. Content Expansion Ideas

### **Missions**
- **"The Lost Courier"**: Find your own dead body from a previous loop.
- **"Prevent the Fire"**: Find the arsonist before noon.

### **Ambient Life**
- **Birds**: Fly away when events spawn.
- **Traffic**: Simple cars moving along main roads (using waypoints).

---

## 7. Build & Release Prep

### **Settings**
- **Company Name**: TimeLoopCorp
- **Product Name**: Time Loop City
- **Version**: 0.1.0 Alpha

### **Build Target**
- **Platform**: Windows x64
- **Architecture**: IL2CPP (for speed) or Mono (for faster builds during dev).

### **Scene List**
0.  `MainMenu` (To be created)
1.  `MainCity`
