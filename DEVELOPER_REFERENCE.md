# 🔧 DEVELOPER REFERENCE - TIME LOOP CITY

## Quick Navigation

### Core Systems
- **NPCs**: `Assets/Scripts/AI/`
- **Events**: `Assets/Scripts/Events/`
- **Dialogue**: `Assets/Scripts/Dialogue/`
- **Missions**: `Assets/Scripts/Managers/`
- **Audio**: `Assets/Scripts/Audio/`
- **UI**: `Assets/Scripts/UI/`

### Tools
- **NPC Spawner**: `Assets/Scripts/AI/NPCSpawner.cs`
- **Upgrade Tool**: `Assets/Editor/FullGameUpgradeTool.cs`
- **Validation**: `Assets/Editor/QAValidationTool.cs`
- **Content Gen**: `Assets/Editor/DefaultContentGenerator.cs`

### Documentation
- **Main Guide**: `FULL_GAME_UPGRADE_README.md`
- **Quick Start**: `UPGRADE_QUICKSTART.md`
- **Implementation**: `UPGRADE_IMPLEMENTATION_SUMMARY.md`
- **This File**: Developer Reference

---

## API Reference

### NPC System
```csharp
// Spawn single NPC (utility)
NPCController npc = spawnedObject.AddComponent<NPCController>();
npc.OnEventDetected("Fire");  // Alert to event
npc.SetSingleDestination(target);  // Override patrol

// Check if NPC remembers
bool remembers = npc.GetComponent<NPCMemoryComponent>().RemembersPreviousLoops();
```

### Event System
```csharp
// Create custom event
public class MyEvent : GameEvent
{
    protected override void OnEventStart() { AlertNearbyNPCs(); }
    protected override void UpdateEvent() { /* logic */ }
    protected override void OnEventEnd() { /* cleanup */ }
}

// Available event types
- FireEvent
- AccidentEvent
- RobberyEvent
- BlackoutEvent
- ParadeEvent
```

### Dialogue System
```csharp
// Start dialogue
DialogueManager.Instance.StartDialogue(dialogueData);

// Get all choices
dialogueData.choices.ForEach(c => Debug.Log(c.choiceText));

// Handle clue discovery
DialogueManager.Instance.EndDialogue();  // Triggers clue rewards
```

### Mission System
```csharp
// Get active missions
var missions = MissionManager.Instance.ActiveMissions;

// Check mission prerequisites
MissionManager.Instance.CheckMissionPrerequisites();

// Start specific mission
MissionManager.Instance.StartMission(missionData);
```

### Audio System
```csharp
// Play sound
AudioManager.Instance.PlaySFX(audioClip);

// Set ambience
AudioManager.Instance.SetAmbience(ambienceClip);

// Control music
AudioManager.Instance.PlayMusic(musicClip);
```

### UI System
```csharp
// Pause game
PauseMenuUI.Instance.Pause();
PauseMenuUI.Instance.Resume();

// Show dialogue
DialogueUI.Instance.ShowDialogue(npcName);
DialogueUI.Instance.ShowChoices(choices, onSelection);
```

---

## Creating Custom Content

### Custom Event Type

1. **Create Event Class**
```csharp
namespace TimeLoopCity.Events
{
    public class CustomEvent : GameEvent
    {
        protected override void OnEventStart()
        {
            Debug.Log("Event started!");
            AlertNearbyNPCs();
        }

        protected override void UpdateEvent()
        {
            // Called every frame
        }

        protected override void OnEventEnd()
        {
            Debug.Log("Event ended!");
        }
    }
}
```

2. **Create Prefab**
- Create empty GameObject
- Add CustomEvent component
- Configure serialized fields
- Save as prefab in `Assets/Resources/Events/`

3. **Register with RandomEventSpawner**
- Find RandomEventSpawner in scene
- Add prefab reference in inspector
- Add to EventType enum if needed

### Custom Dialogue

1. **Create Asset**
- Right-click > Create > TimeLoopCity > Dialogue Data
- Name: `Dialogue_NPCName_Topic`

2. **Configure Content**
```csharp
dialogue.npcName = "NPC Name";
dialogue.sentences.Add("Line 1");
dialogue.sentences.Add("Line 2");
dialogue.minLoopCount = 0;  // Available from loop X
dialogue.giveClueId = "clue_name";  // Reward
```

3. **Add Choices** (Optional)
```csharp
var choice = new DialogueChoice();
choice.choiceText = "Option A";
choice.nextDialogue = followUpDialogue;
dialogue.choices.Add(choice);
```

### Custom Mission

1. **Create Asset**
- Right-click > Create > TimeLoopCity > Mission Data
- Name: `Mission_ObjectiveName`

2. **Define Mission**
```csharp
mission.missionId = "mission_unique_id";
mission.missionName = "Find the X";
mission.description = "Long description";
mission.requiredLoopCount = 2;
mission.requiredClueIds.Add("clue_prerequisite");
```

3. **Add Objectives**
```csharp
var objective = new MissionObjective();
objective.description = "Do something";
objective.type = MissionObjectiveType.FindClue;
objective.targetId = "clue_name";
mission.objectives.Add(objective);
```

### Custom NPC

**Edit FullGameUpgradeTool.cs Step2_SpawnNPCs():**
```csharp
var npcSpawns = new List<(string id, string name, Vector3 pos, List<Vector3> waypoints)>
{
    ("npc_custom", "Custom NPC", new Vector3(x, y, z), new List<Vector3> 
    { 
        Vector3.zero,  // Start position
        new Vector3(5, 0, 5),  // Waypoint 1
        new Vector3(-5, 0, 5)  // Waypoint 2
    }),
    // Add more NPCs here
};
```

---

## Debugging Tips

### Enable Debug Logs
```csharp
// Find any system and check console
Debug.Log("[NPC] Message");  // Search in console
Debug.LogWarning("[System] Alert");
Debug.LogError("[Error] Problem");
```

### Visual Debugging
- Scene view shows NPC paths (cyan lines)
- Gizmos show waypoint connections
- Event positions marked with spheres

### Test Runtime
Add to any GameObject:
```csharp
var tester = gameObject.AddComponent<GameSystemsTester>();
tester.RunAllTests();  // Runs validation
```

---

## Performance Optimization

### NPC Pathing
```csharp
// Reduce update frequency
navMeshAgent.updatePosition = false;  // Update manually
navMeshAgent.updateRotation = false;
```

### Dialogue UI
```csharp
// Cache text components
private TextMeshProUGUI cachedText;

private void Start()
{
    cachedText = GetComponent<TextMeshProUGUI>();
}
```

### Events
```csharp
// Use object pooling for frequent spawns
ObjectPool.Instance.Get<FireEvent>();
ObjectPool.Instance.Return(fireEvent);
```

---

## Common Patterns

### Event Listener Pattern
```csharp
private void OnEnable()
{
    TimeLoopManager.Instance.OnLoopReset.AddListener(HandleReset);
}

private void OnDisable()
{
    TimeLoopManager.Instance.OnLoopReset.RemoveListener(HandleReset);
}

private void HandleReset()
{
    Debug.Log("Loop reset detected");
}
```

### Singleton Pattern
```csharp
public static MyManager Instance { get; private set; }

private void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
        return;
    }
    Instance = this;
}
```

### Factory Pattern
```csharp
private GameObject GetEventPrefab(EventType type)
{
    return type switch
    {
        EventType.Fire => firePrefab,
        EventType.Accident => accidentPrefab,
        _ => null
    };
}
```

---

## Testing Patterns

### Unit Test Example
```csharp
[Test]
public void TestNPCMovement()
{
    var npc = CreateTestNPC();
    npc.SetDestination(testTarget);
    
    Assert.AreEqual(testTarget, npc.CurrentDestination);
}
```

### Integration Test Example
```csharp
[Test]
public void TestEventSpawning()
{
    var spawner = FindObjectOfType<RandomEventSpawner>();
    int initialCount = spawner.GetActiveEvents().Count;
    
    spawner.SpawnRandomEvents();
    
    Assert.Greater(spawner.GetActiveEvents().Count, initialCount);
}
```

---

## Useful Commands

### Scene Setup
```csharp
// Create test NPC
var testNPC = new GameObject("TestNPC");
testNPC.AddComponent<NPCController>();
testNPC.AddComponent<UnityEngine.AI.NavMeshAgent>();
```

### Asset Creation
```csharp
// Create ScriptableObject
var asset = ScriptableObject.CreateInstance<DialogueData>();
AssetDatabase.CreateAsset(asset, "Assets/path/file.asset");
AssetDatabase.SaveAssets();
```

### NavMesh
```csharp
// Rebuild NavMesh at runtime
NavMesh.SetAreaCost(NavMesh.GetAreaFromName("Walkable"), 1.0f);
```

---

## Version History

| Version | Date | Changes |
|---------|------|---------|
| 1.0 | Dec 2025 | Initial release - Full game upgrade |
| 0.9 | Pre-release | Beta testing |
| 0.8 | Internal | Foundation systems |

---

## Contributing Guidelines

### Code Style
```csharp
// Use meaningful names
private int currentWaypointIndex;  // Good
private int cwp;  // Bad

// Document complex logic
/// <summary>
/// Selects waypoint list based on randomization setting
/// </summary>
private List<Transform> SelectWaypointList() { }

// Use regions for organization
#region Initialization
#region Update
#region Helpers
```

### Commit Messages
```
✨ Feature: Add custom event system
🐛 Fix: NPC waypoint assignment error
📝 Docs: Update README with examples
🧹 Refactor: Simplify dialogue manager
```

### Pull Request Template
```
## Description
What changes were made?

## Type of Change
- [ ] Feature
- [ ] Bug Fix
- [ ] Documentation

## Testing
How was this tested?

## Checklist
- [ ] Code compiles
- [ ] No new warnings
- [ ] Tests pass
- [ ] Documentation updated
```

---

## Additional Resources

### Learning Materials
- Unity NavMesh: https://docs.unity3d.com/Manual/nav-BuildingComponents.html
- UI System: https://docs.unity3d.com/Manual/UISystem.html
- Audio: https://docs.unity3d.com/Manual/AudioOverview.html

### External Tools
- Visual Studio Code (scripting)
- Blender (3D models)
- Audacity (audio editing)
- Aseprite (sprite creation)

### Community
- Unity Forums: forum.unity.com
- Reddit: r/Unity3D
- Discord: [Community links]

---

## FAQ

**Q: How do I add more NPCs?**  
A: Edit FullGameUpgradeTool.cs and add to npcSpawns list

**Q: Can I customize event behavior?**  
A: Yes, inherit from GameEvent and override methods

**Q: How do I add audio?**  
A: Assign AudioClips to AudioManager via inspector

**Q: What's the performance impact?**  
A: ~5-8ms per frame for 10 NPCs + events

**Q: Can I extend the missions?**  
A: Yes, create MissionData assets and add prerequisites

---

**Last Updated:** December 1, 2025  
**Maintained By:** Development Team  
**Status:** Active
