# AI Assistant Instructions for Incremental Pachinko

## Architecture Overview

This Unity incremental game uses a **multi-singleton architecture** with `PersistentSingleton<T>` base class providing auto-instantiation and DontDestroyOnLoad lifecycle management. Core systems communicate through events rather than direct dependencies.

### Core Systems (Initialize in Start(), not Awake())
- **DataController**: Game state, save/load via PlayerPrefs, prestige system  
- **UpgradeManager**: Manages `Upgrade` instances, syncs with DataController via events
- **FlyweightFactory**: Object pooling for performance-critical spawned objects

### Key Architectural Patterns

**BigDouble Integration**: All numeric values use `BreakInfinity.BigDouble` for large number support. UI displays via `.Notate()` method.

**ScriptableObject-Driven Design**: 
- `BigDoubleSO`: Reactive values with modifier chain system (`BaseValue` + modifiers = `FinalValue`)
- `UpgradeConfig`: Defines upgrade behavior, costs, formulas  
- `BuyAmountStrategy`: Pluggable x1/x10/x100/MAX purchase logic
- `FlyweightSettings`: Object pool configuration per type

**Event-Driven Communication**:
```csharp
// DataController events trigger UI updates
DataController.Instance.OnDataChanged += UpdateUI;
// UpgradeManager initialization
UpgradeManager.Instance.OnInitialized += () => { /* setup */ };
```

## Development Patterns

### Code Comments
All code comments must be written in English to maintain consistency and facilitate collaboration.

### BigDouble Usage
Always use `BigDouble` for game values. Convert via `BigDouble.Parse(string)` for serialization, display via `.Notate(precision)`.

### Upgrade System
- `Upgrade` class: Pure C# business logic, implements `IDisposable` for event cleanup
- UI components: `UpgradeUI` binds to single upgrade, `UpgradeSystemUI` manages lists
- Power calculation: `Config.powerFormula.Calculate(basePower, level)` * step multipliers

### Object Pooling (Performance Critical)
Use `FlyweightFactory.Spawn(settings)` for frequently created/destroyed objects:
- Balls in pachinko physics
- Floating damage text (`FloatingTextFlyweight` with DOTween animations)
- Any UI elements created >1 per second

### Animation & Performance
- DOTween for all UI/object animations - avoid creating new Sequences, reuse when possible
- Floating text: Kill existing sequence before creating new one to prevent leaks
- Physics objects: Use `Discrete` collision detection and `None` interpolation for WebGL performance

### Data Persistence
- **Save timing**: DataController batches saves, explicit `SaveData()` for critical moments
- **Serialization**: Custom string-based format for Dictionary<string, BigDouble> via `SerializableGameData`
- **WebGL considerations**: PlayerPrefs maps to localStorage, minimize save frequency

### UI Initialization
```csharp
// Wait for core systems before initializing UI
yield return new WaitUntil(() => 
    UpgradeManager.Instance != null && DataController.Instance != null);
```

## Performance Guidelines

### Critical for WebGL
- Batch PlayerPrefs saves (not per operation)
- Pool objects spawned >0.1s frequency  
- Cache `GetComponent` calls in performance paths
- Use `#if UNITY_EDITOR` for development-only logs

### Memory Management
- `Upgrade.Dispose()` must be called to prevent event subscription leaks
- DOTween sequences: Always `Kill(true)` before reassigning
- BigDoubleSO modifier chains: Remove listeners in OnDestroy

## Common Integration Points

**Adding New Upgrade Type**: Create UpgradeConfig ScriptableObject → Add to UpgradeManager's `_upgradeConfigs` → UI automatically generates

**Custom Buy Strategy**: Inherit from `BuyAmountStrategy`, implement `GetBuyAmount()` and `GetCost()` 

**Save Data Extension**: Modify `GameData` class and `SerializableGameData` with version migration

**Performance Optimization**: Check Counter.OnTriggerEnter() for GC patterns - this is the highest frequency code path

## File Organization
- `Assets/Scripts/Core/`: Singletons and main systems  
- `Assets/Scripts/Game/Upgrades/`: Business logic classes
- `Assets/Scripts/Patterns/`: Reusable patterns (Flyweight, Buy Amount strategies)
- `Assets/Scripts/UI/`: MonoBehaviour UI controllers
- `Assets/Scripts/Utility/`: BigDouble extensions, helper classes

Focus on event-driven design and ScriptableObject composition over inheritance when extending systems.