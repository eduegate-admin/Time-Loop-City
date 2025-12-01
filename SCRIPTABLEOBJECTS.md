# Time Loop City - ScriptableObject Templates

This document explains how to create and use ScriptableObjects in Time Loop City.

---

## 1. Clue Database

### Creating the Database

1. In Unity Project window, navigate to `ScriptableObjects/`
2. Right-click > Create > TimeLoopCity > Clue Database
3. Name: `ClueDatabase`
4. Select it in Inspector

### Adding Clues

In the Inspector:
1. Increase `Clues` size
2. For each clue, fill in:
   - **Clue Id**: Unique identifier (e.g., "mysterious_note")
   - **Clue Name**: Display name (e.g., "Mysterious Note")
   - **Description**: Text describing the clue
   - **Icon**: Sprite image for UI
   - **Is Story Clue**: Check if important to story

**Example Clues:**
```
Clue 1:
- Id: "torn_photo"
- Name: "Torn Photograph"
- Description: "A photo of someone you don't recognize... yet."
- Icon: [photo sprite]
- Is Story Clue: ✓

Clue 2:
- Id: "strange_key"
- Name: "Strange Key"
- Description: "This key doesn't match any lock you've tried."
- Icon: [key sprite]
- Is Story Clue: ✓

Clue 3:
- Id: "coffee_stain"
- Name: "Coffee Stain"
- Description: "Always on the same table, every loop."
- Icon: [stain sprite]
- Is Story Clue: ✗
```

### Assigning to Scene

1. Select `PersistentClueSystem` GameObject in scene
2. Drag `ClueDatabase` to "Clue Database" field

---

## 2. World State Presets (Optional)

### Creating World State Data

You can create specific world states for certain loops.

**Create ScriptableObject:**

```csharp
// Add to WorldStateManager.cs or create new file
[CreateAssetMenu(fileName = "WorldStatePreset", menuName = "TimeLoopCity/World State Preset")]
public class WorldStatePresetSO : ScriptableObject
{
    public int loopNumber;
    public WeatherType weatherType;
    public float timeOfDay;
    public string description;
    public List<EventType> forcedEvents;
}
```

**Usage:**
1. Create asset: Right-click > Create > TimeLoopCity > World State Preset
2. Configure specific loop states (e.g., "Loop 5 is always stormy")
3. Reference in WorldStateManager

---

## 3. NPC Dialogue Database

### Creating NPC Dialogue System

**Create ScriptableObject:**

```csharp
// Save as: Scripts/AI/NPCDialogueData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NPCDialogue", menuName = "TimeLoopCity/NPC Dialogue")]
public class NPCDialogueData : ScriptableObject
{
    public string npcId;
    public List<DialogueLine> normalDialogue;
    public List<DialogueLine> memoryDialogue;
    public List<DialogueLine> clueResponses; // Dialogue after finding specific clue
}

[System.Serializable]
public class DialogueLine
{
    [TextArea]
    public string text;
    public string requiredClue; // Optional: only show if player has clue
}
```

**Usage:**
1. Create dialogue assets for each NPC
2. Assign to NPCMemoryComponent

---

## 4. Event Configuration

### Creating Event Data

**Create ScriptableObject:**

```csharp
// Save as: Scripts/Events/EventData.cs
using UnityEngine;

[CreateAssetMenu(fileName = "EventData", menuName = "TimeLoopCity/Event Data")]
public class EventData : ScriptableObject
{
    public EventType eventType;
    public string eventName;
    [TextArea]
    public string description;
    public GameObject prefab;
    public float duration = 60f; // How long event lasts
    public float npcReactionRange = 20f;
    public AudioClip eventSound;
}
```

**Usage:**
1. Create EventData for each event type
2. Configure in RandomEventSpawner
3. More flexible than direct prefab references

---

## 5. Game Settings

### Creating Global Settings

**Create ScriptableObject:**

```csharp
// Save as: Scripts/Core/GameSettings.cs
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "TimeLoopCity/Game Settings")]
public class GameSettings : ScriptableObject
{
    [Header("Time Loop")]
    public float loopDurationMinutes = 5f;
    public bool debugModeShortLoops = false;
    public float debugLoopDuration = 30f;

    [Header("Difficulty")]
    public float timeMultiplier = 1f;
    public int hintsAvailable = 3;
    public bool allowManualReset = true;

    [Header("UI")]
    public bool showDebugInfo = false;
    public bool show24HourClock = false;
}
```

**Usage:**
1. Create single `GameSettings` asset
2. Reference in managers for global config
3. Easy to adjust without touching code

---

## 6. Quest/Mission System (Expandable)

### Creating Mission Data

**Create ScriptableObject:**

```csharp
// Save as: Scripts/Managers/MissionData.cs
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Mission", menuName = "TimeLoopCity/Mission")]
public class MissionData : ScriptableObject
{
    public string missionId;
    public string missionName;
    [TextArea]
    public string description;
    
    [Header("Requirements")]
    public List<string> requiredClues;
    public int minimumLoopCount;
    
    [Header("Objectives")]
    public List<MissionObjective> objectives;
    
    [Header("Rewards")]
    public List<string> rewardClues;
    public string completionDialogue;
}

[System.Serializable]
public class MissionObjective
{
    public string description;
    public ObjectiveType type;
    public string targetId; // NPC id, location id, etc.
    public bool completed;
}

public enum ObjectiveType
{
    TalkToNPC,
    FindClue,
    VisitLocation,
    WitnessEvent
}
```

**Usage:**
1. Create mission assets
2. Build MissionManager system (future expansion)
3. Track across loops

---

## ScriptableObject Organization

```
ScriptableObjects/
├── ClueDatabase.asset
├── GameSettings.asset
├── WorldStates/
│   ├── Loop05_Stormy.asset
│   └── Loop10_Special.asset
├── NPCDialogue/
│   ├── Shopkeeper_Dialogue.asset
│   ├── Detective_Dialogue.asset
│   └── Rememberer_Dialogue.asset
├── Events/
│   ├── FireEvent_Config.asset
│   ├── AccidentEvent_Config.asset
│   └── ParadeEvent_Config.asset
└── Missions/
    ├── Mission_FindTheKey.asset
    └── Mission_SaveTheCity.asset
```

---

## Best Practices

### 1. Naming Convention
- Use descriptive names
- Include type suffix: `_Dialogue`, `_Config`, `_Data`
- Use consistent casing

### 2. Organization
- Group by type in folders
- One database per system
- Version control friendly

### 3. Data Integrity
- Use unique IDs for all items
- Document ID conventions
- Validate references

### 4. Iteration
- Easy to balance without code changes
- Designers can modify data
- Test different configurations quickly

---

## Quick Reference

| ScriptableObject | Menu Path | Used By |
|---|---|---|
| ClueDatabase | TimeLoopCity > Clue Database | PersistentClueSystem |
| NPCDialogue | TimeLoopCity > NPC Dialogue | NPCMemoryComponent |
| EventData | TimeLoopCity > Event Data | RandomEventSpawner |
| WorldStatePreset | TimeLoopCity > World State Preset | WorldStateManager |
| GameSettings | TimeLoopCity > Game Settings | All Managers |
| MissionData | TimeLoopCity > Mission | Mission System (future) |

---

## Next Steps

1. Create ClueDatabase first (required)
2. Add basic clues to test system
3. Expand with dialogue and missions as needed
4. Create editor tools for easier data entry (advanced)
