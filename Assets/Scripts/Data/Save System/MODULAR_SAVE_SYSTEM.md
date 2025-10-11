# Modular Save System Documentation

## Overview

The new modular save system follows the **Open-Closed Principle** - it's open for extension but closed for modification. You can easily add new save data types without changing existing save/load code.

## Architecture

### Core Components

1. **ISaveModule**: Interface that all save modules must implement
2. **PlayerPrefsDataRepository**: Main save/load manager that coordinates modules
3. **SaveContainer**: Container that holds all module data
4. **ModuleSaveData**: Individual module data structure

### How It Works

1. Each feature that needs save data implements `ISaveModule`
2. Modules are registered in `PlayerPrefsDataRepository` constructor
3. During save, each module serializes its own data
4. During load, each module deserializes its own data
5. Version migration is handled per-module

## Adding New Save Data (Example: Skill Tree)

### Step 1: Create Your Save Module

```csharp
public class SkillTreeSaveModule : ISaveModule
{
    public string ModuleName => "SkillTree";
    public int Version => 1;
    
    // Your data here
    private Dictionary<string, int> skillLevels = new Dictionary<string, int>();
    
    public string Serialize()
    {
        // Convert your data to JSON string
        var data = new SkillTreeData { skills = skillLevels };
        return JsonUtility.ToJson(data);
    }
    
    public void Deserialize(string data)
    {
        // Load your data from JSON string
        var skillData = JsonUtility.FromJson<SkillTreeData>(data);
        skillLevels = skillData.skills;
    }
    
    public void OnMigrate(int fromVersion, int toVersion)
    {
        // Handle version changes
        if (fromVersion < 1 && toVersion >= 1)
        {
            // Migration logic here
        }
    }
}
```

### Step 2: Register Your Module

In `PlayerPrefsDataRepository` constructor:

```csharp
public PlayerPrefsDataRepository()
{
    RegisterSaveModule(new CoreGameDataModule());
    RegisterSaveModule(new UpgradeSaveModule());
    RegisterSaveModule(new SkillTreeSaveModule()); // Add this line
}
```

### Step 3: Access Your Data

```csharp
// Get your module instance
var skillTree = dataRepository.GetModule<SkillTreeSaveModule>();
int level = skillTree.GetSkillLevel("fireball");
```

## Benefits

### ✅ Open-Closed Principle
- **Open for extension**: Add new features easily
- **Closed for modification**: Don't touch existing save code

### ✅ Separation of Concerns
- Each module handles its own data
- Clear boundaries between features
- Independent versioning per module

### ✅ Easy Testing
- Test each module in isolation
- Mock specific modules for unit tests
- Clear data contracts

### ✅ Maintainability
- Add new features without merge conflicts
- Easy to locate save-related bugs
- Modular code organization

## Migration Strategy

### Backward Compatibility
The system supports both old and new save formats:

1. Try to load new modular format first
2. Fall back to legacy format if needed
3. Automatic migration on first save

### Version Management
Each module has its own version number:
- Increment when data structure changes
- Handle migration in `OnMigrate()` method
- Per-module migration prevents conflicts

## Example: Before vs After

### Before (Violates Open-Closed Principle)
```csharp
// Adding skill tree requires modifying existing classes
public class GameData 
{
    // ... existing fields
    public Dictionary<string, int> skillLevels; // NEW - breaks existing code
}

public class SerializableGameData
{
    // ... existing fields  
    public SkillData[] skills; // NEW - breaks existing code
}
```

### After (Follows Open-Closed Principle)
```csharp
// Just register a new module - no existing code changes!
RegisterSaveModule(new SkillTreeSaveModule());
```

## Performance Considerations

- Modules serialize/deserialize independently
- Only affected modules need updates
- Lazy loading possible per module
- Memory usage optimized per feature

## Error Handling

- Module failures don't affect other modules
- Graceful degradation when modules fail
- Detailed error logging per module
- Automatic fallback to default values

This architecture makes the save system highly extensible and maintainable!