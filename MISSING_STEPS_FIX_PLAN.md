# Missing Steps Fix Plan

The following steps were identified as missing or incomplete based on the Build Guide. I have executed the automated fixes.

## 🛠️ Automated Fixes Executed

### 1. Missing UI Scripts
*   **Issue**: The Build Guide referenced a Main Menu, but no script existed.
*   **Fix**: Created `Scripts/UI/MainMenuUI.cs`.
*   **Status**: ✅ FIXED

### 2. Missing Scene Management
*   **Issue**: No robust way to switch scenes or show loading screens.
*   **Fix**: Created `Scripts/Managers/SceneLoader.cs`.
*   **Status**: ✅ FIXED

### 3. Validation Tooling
*   **Issue**: User needs a way to verify settings without manually checking every menu.
*   **Fix**: Created `Scripts/Editor/BuildValidator.cs`.
*   **Status**: ✅ FIXED

## 📋 Remaining Manual Steps

These steps cannot be automated by an external agent and require Unity Editor interaction:

1.  **Scene Creation**:
    *   Create `MainMenu.unity`.
    *   Setup the UI Canvas with Buttons.
2.  **Build Settings Configuration**:
    *   Add scenes to the "Scenes in Build" list.
    *   Set Company Name and Product Name.
3.  **Baking**:
    *   Bake NavMesh.
    *   Bake Lighting.
