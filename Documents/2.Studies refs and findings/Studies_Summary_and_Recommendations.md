# Studies Summary & Recommendations

**Created:** January 31, 2026

This report summarizes insights from the collected study notes in `Documents/2.Studies refs and findings/` and maps practical lessons to our 11 sellable systems. It highlights what we want, what already exists in our repo, and concrete recommendations we can apply immediately.

---

## Quick overview of studied projects

Files reviewed:
- Corgi Engine
- Animancer
- TwoBitMachines
- CodeMonkey Toolkit
- Cowsins 2D Platformer Engine
- 2D Action Platformer Kit
- 2D Dungeon Gunner Clone
- AdvancedRogueLikeandPuzzleSystem
- HappyHarvest
- Safire2DCamera-Analysis
- Udemy2DShooter
- UdemyKawaiiSurvivor
- UdemyTopDownShooter
- (and other course studies in the folder)

Common high-value patterns across studies:
- Component-based controllers and composition
- State machine architectures for AI and gameplay states
- ScriptableObject-driven data (SO assets for configs)
- Interface-driven loose coupling (IAgent/IHittable etc.)
- Object pooling for performance
- Data-driven wave/procedural content and loot
- Simple, lightweight utilities (timers, pools, tweening alternatives)
- Editor UX (SO inspectors, preview tools) makes assets sellable

---

## Map to our 11 sellable systems — what to adopt & prioritize

1) Modular Stats & Slot System
- Lessons: ScriptableObject stat and rarity patterns (Udemy courses, Cowsins)
- Apply: `StatContainer` SO + `ModifierStack` runtime API; `SlotContainer` SOs; UI slot/merge helpers.
- Priority: High (unlocks progression/productization)

2) Health & Combat Core (Already strong)
- Lessons: event-driven health, status effects, adapters for UI/FX (CodeMonkey, Udemy)
- Apply: polish API (IHealth, events), export small sample scenes and PlayMode tests.
- Priority: Ship-ready; finalize docs and examples.

3) VFX Rules Automation
- Lessons: Animancer/MMFeedbacks style event→feedback mapping; sequence builders (AnimatorPlus)
- Apply: `VFXRule` SO (event id → prefab/pool/anchor/cooldown/audio hook). Implement manager to auto-bind to CombatEvents.
- Priority: High (massive polish value for demos)

4) Audio & Ambience System
- Lessons: Master Audio patterns (channels, random clip selection, ducking)
- Apply: lightweight `AudioEventDispatcher` + `AmbientZone` + SO playlists. Hook to VFX rules.
- Priority: Medium

5) Timer & Task Framework (Already present)
- Lessons: FunctionTimer/CodeMonkey patterns — periodic and named timers.
- Apply: keep `Timer.Create` API; add persistence hooks for missions.
- Priority: Already solid

6) Mission / Dispatch Framework
- Lessons: Node-based mission definitions, timer-based resolution (courses)
- Apply: `MissionDefinition` SO, `MissionDispatcher` that uses Timer, reward application hooks.
- Priority: High for game loop

7) Formation & Auto-Battle Framework
- Lessons: A* and formation patterns for group behavior (A* references, TwoBitMachines)
- Apply: simple role tags + formation slots + `AutoBattleController` (tick-based simulation).
- Priority: Medium

8) Construction & Building Framework
- Lessons: Grid/tile systems (HappyHarvest, CodeMonkey), blueprint SOs
- Apply: `BuildSlot` registry, `Blueprint` SO, worker assignment + progress events
- Priority: High (core to stronghold theme)

9) Wave & Threat Framework
- Lessons: wave definitions as SO (Udemy Kawaii, Dungeon Gunner), scaling rules
- Apply: `WaveDefinition` SO + `WaveManager` (Timer + pool + spawn rules), notifications on incoming waves
- Priority: Highest for MVP gameplay loop

10) Progression & Retention Pack
- Lessons: daily rewards, merge/upgrade mechanics (Kawaii Survivor), shop patterns
- Apply: minimal `DailyRewardSO`, `MilestoneSO`, `AchievementSO` with save hooks
- Priority: Medium

11) Notifications & Markers
- Lessons: CodeMonkey UI patterns, toast systems, priority queue approaches
- Apply: `NotificationCenter` (priority queue + throttle) and `WorldMarker` prefabs
- Priority: Low-Medium (UX polish)

---

## Concrete, immediate actions (next 2 weeks)

1. Implement `WaveDefinition` SO + `WaveManager` MVP
   - Why: enables immediate playable loop (waves → defense → rebuild)
   - Deliverables: `Assets/Samples/WaveDemo` scene, simple enemy prefab using existing `HealthSystem`.

2. Prototype `VFXRule` SO + `VFXManager` (polled by Combat events)
   - Why: quick visual polish, shows system integration value
   - Deliverables: `VFXRule` SO, pooling integration, demo hooking to HealthSystem.OnDamage

3. Create `MissionDefinition` SO skeleton + dispatcher using `Timer` service
   - Why: mission/dispatch is central to dungeon layer and retention
   - Deliverables: mission SO, dispatcher, sample mission UI

4. Finish Health & Combat packaging
   - Why: flagship product to extract and sell
   - Deliverables: clean README, sample scenes, PlayMode tests, exportable package layout

---

## Editor & UX recommendations

- Use ScriptableObject assets for all data-heavy features.
- Provide sample scenes for each system (minimal runnable examples). 
- Create SO inspectors with validation (NaughtyAttributes or custom editors).
- Include short tutorial README per system and a single "Showcase" scene using multiple systems.

---

## Tech constraints & rules (from studies + our coding rules)

- Runtime assemblies must not reference `UnityEditor` (follow project rules).
- Domain logic should be pure C# where possible; Unity references only in Presentation layer.
- Keep Update() usage minimal—prefer `Timer` and tick services.
- Avoid reimplementing large subsystems (e.g., a full camera system); extend Unity packages (Cinemachine) instead.

---

## Risks & mitigations

- Scope creep: implement MVP features only; add optional flags for pro features.
- Maintenance: create automated tests and gather telemetry for performance-critical systems.
- Licensing: learn UX/IDE patterns from assets but do not copy proprietary code.

---

## Next steps (owner decisions)

- Choose the first prototype: `WaveManager` (recommended) or `VFXManager` (polish-first).
- Assign 1 developer to WaveManager (48–72h MVP). Create branch `feature/wave-manager`.
- Create sample enemy prefab that uses `HealthSystem` and a basic spawn point.

---

## Appendix: Short, actionable API ideas

- WaveDefinition SO fields: `List<WaveSegment>` where WaveSegment = {EnemyPrefab, Count, SpawnRate, SpawnRadius, Delay}
- VFXRule SO fields: `{EventId, Prefab, AnchorType(enum: HitPoint/World/UI), PoolSize, CooldownSeconds, OptionalAudioSO}`
- MissionDefinition SO: `{MissionName, Duration, RequiredUnits, RewardList(SO refs)}`

---

End of report.
