```instructions
---
description: 'Unity C# development guidelines for this repository'
applyTo: '**/*.cs'
---

# Unity C# Development

## C# instructions
- Use the highest C# language version supported by the project's Unity version. Prefer modern, allocation-friendly features (pattern matching, spans where applicable, switch expressions) but avoid features not supported by the current Unity Roslyn.
- Write clear, concise comments for each function explaining purpose, parameters, return, and important design decisions.

## General instructions
- Make only high-confidence suggestions when reviewing/rewriting code.
- Favor maintainability: small focused types, clear separation of concerns, and explanatory comments when a choice is non-obvious.
- Handle edge cases explicitly; prefer defensive programming at boundaries and avoid throwing exceptions in per-frame code paths.
- When adding libraries or external dependencies, mention their purpose and how they integrate (e.g., BreakInfinity.BigDouble for large-number arithmetic, DOTween for tweening).

## Naming conventions
- PascalCase for public types, methods, properties, and events.
- camelCase for private fields and locals. Use [SerializeField] on private fields instead of making fields public.
- Prefix interfaces with I (e.g., IUpgradeFormula, ISaveModule).
- Suffix ScriptableObjects with SO and editor-only utilities with Editor when helpful.

## Formatting
- Follow the `.editorconfig` settings. Prefer file-scoped namespaces and single-line `using` directives.
- Put a newline before an opening brace of any block (`if`, `for`, `while`, `foreach`, `using`, `try`, etc.). Ensure the final `return` of a method is on its own line.
- Prefer pattern matching and switch expressions. Use `nameof` for member references.
- Write XML doc comments for public/shared APIs (especially those consumed across assemblies). Include `<example>`/`<code>` when helpful.

## Unity project structure and conventions
- Keep runtime scripts under `Assets/Scripts/` and group by feature (e.g., Game, UI, Data). Place editor-only scripts under an `Editor/` folder or guard with the UNITY_EDITOR preprocessor directive.
- Use assembly definition files (`.asmdef`) where present to keep compile times low and enforce boundaries.
- Prefer ScriptableObjects for configuration data; avoid storing mutable runtime state on SOs unless clearly labeled as Runtime.

## MonoBehaviour lifecycle best practices
- Cache references in `Awake`/`Start`. Subscribe to events in `OnEnable`; unsubscribe in `OnDisable`. Release resources in `OnDestroy`.
- Avoid heavy work and allocations in `Update`/`FixedUpdate`/`LateUpdate`. Move long-running work off the hot path or batch it.
- Never call `GetComponent` repeatedly per frame; cache results. Prefer `TryGetComponent` when components may be missing.

## ScriptableObjects
- Use `[CreateAssetMenu]` for assets configurable in the Inspector. Keep SOs immutable at runtime unless explicitly marked as runtime containers (e.g., RuntimeSetSO, BigDoubleSO values).
- Prefer composition over inheritance for SO-based formulas and configuration.
- For polymorphic serialization, Unity serializes concrete types. Use `[SerializeReference]` only when necessary and with care.

## Events and decoupling
- Prefer C# events/delegates for performance-critical paths; use `UnityEvent` primarily for Inspector wiring.
- Use ScriptableObject “event channels” to decouple producers/consumers when scenes or lifetimes differ.
- Ensure every event subscription has a corresponding unsubscription. Implement `IDisposable` for listeners that outlive MonoBehaviours.

## Async operations: UniTask vs Coroutines vs Update
- Use `Update` for real-time input and counters; accumulate `Time.deltaTime` and act at intervals for repeated actions.
- Prefer UniTask over Coroutines for asynchronous workflows, delays, and sequencing. UniTask is allocation-free, supports cancellation tokens, and integrates with async/await.
- Use Coroutines only when UniTask is unavailable or for simple yield-based sequences. Keep them stoppable and avoid unbounded `while(true)` without yields.
- For physics, use `FixedUpdate` and `Time.fixedDeltaTime`. Read input in `Update`, store state, and act in `FixedUpdate`.

## UI (UGUI/TMP) and tooltips
- Use TextMesh Pro for text. Avoid per-frame string allocations; cache formatters and use `StringBuilder` in hot paths.
- Convert world to canvas space with `RectTransformUtility`. Clamp/pop tooltip to stay on-screen by adjusting pivot when near edges.
- Minimize layout rebuilds; call `LayoutRebuilder.ForceRebuildLayoutImmediate` only when needed.

## Memory and performance
- Avoid per-frame allocations (boxing, string concat, LINQ, `foreach` over non-struct enumerables). Pre-size lists; reuse collections.
- Use object pooling for frequently spawned objects (e.g., balls). Warm pools and return objects deterministically.
- Cache `Shader.PropertyToID` and static references. Avoid `FindObjectOfType` at runtime.
- Profile regularly (Profiler, Deep Profile sparingly). Fix the biggest offenders first.

## Object pooling and flyweight
- Keep pooled objects lightweight and reset state on reuse. Avoid `Instantiate`/`Destroy` in loops; use a factory/pool.
- Track active instances via a runtime set and fire events on add/remove to update UI efficiently.

## Saving and persistence
- Use the modular save system via `ISaveModule` and `SaveContainer`.
  - Save: serialize a container with module payloads.
  - Load: attempt modular format first; fall back to legacy, then migrate.
  - WebGL: prefer PlayerPrefs storage; compress payloads if size becomes a concern.
- Version save data and write migration code when schemas change.

## Large numbers (BreakInfinity.BigDouble)
- Use `BigDouble` for game economies to avoid overflow and maintain precision.
- Keep formatting user-friendly (trim trailing zeros, reasonable precision). Separate displayed value from internal value when temporary boosts apply.

## Error handling and logging
- Avoid throwing exceptions from hot paths like `Update`; guard and early-out instead.
- Use `Debug.Log*` for diagnostics; gate noisy logs behind developer flags or compile them only for Editor via the UNITY_EDITOR preprocessor directive.
- Fail fast in initialization with clear messages when required dependencies are missing.

## Testing
- Add Edit Mode tests for pure logic and Play Mode tests for behaviours. Prefer testing formulae, progression math, and save/load.
- Do not add "Arrange/Act/Assert" comments. Match existing naming and casing conventions in nearby tests.
- Mock time and inputs where feasible; design pure, side-effect-free functions for easy testing.

## Platform considerations (WebGL/Desktop)
- WebGL has tighter memory and threading limits. Avoid dynamic code paths that allocate heavily and prefer smaller textures/audio.
- Use PlayerPrefs and JSON for persistence on WebGL. Avoid synchronous file IO APIs at runtime.

## Editor-only code
- Place editor utilities under an `Editor/` folder or wrap with the UNITY_EDITOR preprocessor directive.
- Never reference `UnityEditor` types from runtime assemblies.

## Threading
- Only touch UnityEngine objects from the main thread. Use jobs/tasks for compute, then marshal results back to main thread.

## Nullable reference types
- Prefer non-nullable references. Validate `null` at entry points and when interfacing with Unity APIs that can return `null`.
- Use `is null` / `is not null` rather than `== null` / `!= null`.

```


