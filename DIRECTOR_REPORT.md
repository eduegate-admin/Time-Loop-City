# Time Loop City - Director's Report

## 🎬 Vision Statement
**Time Loop City** should feel like a "playable mystery thriller." The player is trapped in a clockwork world. The atmosphere must shift from "Normal Morning" to "Apocalyptic Tension" over the course of the 5-minute loop.

---

## 🎨 Visual Polish Plan
1.  **Lighting Arc**:
    *   **Start (0:00)**: Golden Hour (Morning). Crisp shadows. Hopeful.
    *   **Mid (2:30)**: High Noon. Harsh light. "The Fire" event creates smoke haze.
    *   **End (4:30)**: Eclipse/Storm. The sky turns purple/dark. Shadows lengthen unnaturally.
2.  **The Glitch**:
    *   As the loop destabilizes, add **Chromatic Aberration** and **Film Grain**.
    *   Objects should "jitter" slightly (vertex displacement shader) near the end.

## 🔊 Audio Direction
1.  **The Ticking**:
    *   A subtle clock ticking sound should fade in at 4:00.
    *   At 4:50, it becomes a pounding heartbeat/industrial gear sound.
2.  **Music**:
    *   **Layer 1**: Ambient Synth (Always playing).
    *   **Layer 2**: Rhythmic Bass (Starts at 2:30).
    *   **Layer 3**: High Strings/Tension (Starts at 4:00).

---

## 🕵️ Mission Pacing ("The Golden Path")
To ensure a good player experience, the missions should flow as follows:

1.  **Loop 1: Confusion**
    *   Player explores. Sees the Fire. Sees the Reset.
    *   *Goal*: Just survive.
2.  **Loop 2: Discovery**
    *   Player finds the **Newspaper** (Clue).
    *   *Goal*: Realize the loop is caused by the Lab.
3.  **Loop 3: Contact**
    *   Player talks to **NPC_Scientist**.
    *   *Goal*: Get the mission "Find the Keycard".
4.  **Loop 4: Action**
    *   Player uses knowledge of the **Blackout** to sneak past security.
    *   *Goal*: Get the Keycard.
5.  **Loop 5: Resolution**
    *   Player enters the Lab.
    *   *Goal*: Fix the Core.

---

## ✅ QA & Polish Checklist
*   [ ] **Camera**: Ensure camera doesn't clip through buildings in narrow alleys.
*   [ ] **NavMesh**: Verify NPCs don't walk into the Fountain.
*   [ ] **UI**: "Loop Reset" fade should be perfectly timed with the player teleport.
*   [ ] **Performance**: Ensure no stutter when the City generates.
*   [ ] **Input**: "E" prompt should only appear when looking AT the object.

---

## 📝 Scripting Standards (Code Review)
*   **Namespaces**: All scripts must be in `TimeLoopCity.*`.
*   **Events**: Use `UnityEvent` for loose coupling.
*   **Singletons**: Only Managers should be Singletons.
*   **Performance**: No `FindObjectOfType` in `Update()`.

This project is on track for a high-quality vertical slice.
