# KS Health & Combat System - Complete Character State Framework

**Version 1.0.0** | Updated: January 31, 2026

A comprehensive, modular system for managing character health, shields, status effects, combat mechanics, and visual feedback in Unity games. Perfect for action, RPG, and combat-focused games.

---

## 🎯 What Makes This Special

**Complete Character State Management:**
- Health with damage/healing/regeneration
- Shield absorption system
- Status effects (buffs/debuffs)
- Invulnerability mechanics
- Downed/revive states

**Advanced Combat Features:**
- Damage type resistances
- Team-based damage rules
- Damage routing system
- Friendly fire controls

**Rich Visual Feedback:**
- Screen effects (flash, shake, particles)
- Sprite tinting and animations
- Audio integration
- Low-health warnings

**Professional UI System:**
- Health bars, text displays, icons
- World-space and screen-space options
- Animated transitions
- Boss health displays

**Save/Load Support:**
- JSON serialization
- State snapshots (health/shield/status)
- Versioned snapshots with basic validation

---

## 📦 What's Included

### Core Systems
- **HealthSystem**: Core vitality management with advanced features
- **ShieldSystem**: Damage absorption with regeneration
- **StatusEffectSystem**: Buffs/debuffs with stacking rules
- **HealthVisualSystem**: Visual feedback and effects

### UI Components
- **HealthBar**: Animated health progress bars
- **HealthText**: Numerical health displays
- **HealthIcon**: Visual health indicators
- **BossHealthBar**: Special boss health displays
- **WorldSpaceHealthBar**: 3D world-positioned bars

### Combat Extensions
- **DamageRouter**: Team-based damage handling
- **TeamComponent**: Faction and team management
- **DamageRules**: Combat relationship logic

### Persistence
- **HealthSnapshotComponent**: Save/load functionality
- **HealthSnapshot**: Serializable state data

### Utilities
- **HealthProfileSO**: Configuration presets
- **HealthEventChannelSO**: Decoupled event system
- **PlayMode tests for core systems**

---

## 🚀 Quick Start (3 Minutes)

1. **Add to Player:**
   ```csharp
   // Attach HealthSystem to your player
   HealthSystem health = player.AddComponent<HealthSystem>();
   health.SetMaxHealth(100);
   ```

2. **Add Visual Feedback:**
   ```csharp
   // Add visual effects
   player.AddComponent<HealthVisualSystem>();
   ```

3. **Add UI (Optional):**
   ```csharp
   // Create UI canvas and add components
   HealthUIController ui = player.AddComponent<HealthUIController>();
   // Connect UI elements in Inspector
   ```

4. **Combat Ready:**
   ```csharp
   // Deal damage
   health.TakeDamage(25);

   // Apply status effect
   statusSystem.ApplyPoison(5f, 2); // 5 seconds, 2 damage/sec
   ```

---

## 🎮 Advanced Features

### Damage Types & Mitigation
```csharp
// Set up damage resistances
DamageResistance[] resistances = new DamageResistance[] {
    new DamageResistance(DamageType.Fire, 0.5f), // 50% fire resistance
    new DamageResistance(DamageType.Poison, 1f)  // 100% poison immunity
};
health.ConfigureMitigation(5f, 0.1f, resistances); // 5 flat + 10% reduction
```

### Team Combat
```csharp
// Set up teams
TeamComponent teamA = playerA.AddComponent<TeamComponent>();
teamA.TeamId = 1;

TeamComponent teamB = playerB.AddComponent<TeamComponent>();
teamB.TeamId = 2;

// Damage only applies between different teams
damageRouter.ApplyDamage(damageInfo, sourceInfo);
```

### Save/Load System
```csharp
// Save state
HealthSnapshot snapshot = snapshotComponent.CaptureSnapshot();
string json = snapshot.ToJson();
PlayerPrefs.SetString("PlayerHealth", json);

// Load state
string json = PlayerPrefs.GetString("PlayerHealth");
HealthSnapshot snapshot = HealthSnapshot.FromJson(json);
snapshotComponent.RestoreSnapshot(snapshot);
```

---

## 📋 System Requirements

- **Unity Version**: 2020.3 or later
- **Platforms**: All Unity platforms supported
- **Dependencies**: None (uses only Unity core modules)

---

## 🏗️ Architecture

**Layered Folders:**
- **Core**: Health, shields, status effects (MonoBehaviours)
- **Visuals**: Visual feedback system
- **UI**: Display components and controller
- **Extensions**: Optional combat, persistence, and world-space UI

**Event-Driven:**
- C# events for programmatic control
- UnityEvents for Inspector wiring
- Optional ScriptableObject event channels

---

## 📚 Documentation

Most components include tooltips and summary comments. Key classes:

- `HealthSystem`: Core health management
- `ShieldSystem`: Damage absorption
- `StatusEffectSystem`: Buff/debuff management
- `HealthSnapshotComponent`: Persistence
- `DamageRouter`: Combat routing

---

## 🧪 Testing

Includes PlayMode tests covering:
- Health and shield interactions
- Event/channel firing
- Damage mitigation basics
- Low-health visual toggling

---

## 📄 License & Support

**License**: See LICENSE.md
**Support**: support@kalponicstudio.com
**Website**: https://www.kalponicstudio.com

---

## 🎯 Perfect For

- Action games with combat
- RPGs with status effects
- Battle royales with shields
- Platformers with health systems
- Any game needing character state management

4) (Optional) Create a `HealthProfileSO` and apply it at runtime or in a setup script.

That is enough to be fully functional.

---

## 3) Folder structure (audit)

Current structure is modular and easy to extend:

- `Runtime/Core` - health, shields, status effects, profiles, controllers
- `Runtime/Visuals` - visual feedback system
- `Runtime/UI` - UI components and UI docs
- `Extensions` - optional advanced features (safe to ignore)
- `Resources` - ScriptableObject assets (optional)
- `Tests` - playmode tests

---

## 4) Step-by-step setup (scene)

### Beginner Scene Setup (Hierarchy)
1) Create a Player GameObject
   - Hierarchy: Right-click -> Create Empty
   - Name: Player
   - Add components:
     - SpriteRenderer
     - Rigidbody2D (2D) or Rigidbody (3D)
     - Your movement script (optional)

2) Add the health system
   - Select Player
   - Add Component -> HealthSystem

3) Optional modules (add only what you need)
   - Add Component -> ShieldSystem
   - Add Component -> StatusEffectSystem
   - Add Component -> HealthVisualSystem

4) Create a UI Canvas (optional but common)
   - Hierarchy: Right-click -> UI -> Canvas
   - Unity will also create an EventSystem if missing

5) Add Health UI
   - Select the Canvas
   - Create an empty child: Right-click -> Create Empty (name: HealthUI)
   - Add Component -> HealthUIController on HealthUI
   - Create children under HealthUI:
     - UI -> Slider (add HealthBar script)
     - UI -> Text or TextMeshPro (add HealthText script)
     - UI -> Image (add HealthIcon script)
   - Assign the HealthSystem reference in HealthUIController

6) (Optional) Create an Event Channel
   - Project window: Right-click -> Create -> Kalponic Studio -> Health -> Event Channel
   - Assign it to HealthSystem and HealthVisualSystem if you prefer SO events

### Step 1: Create a unit
1) Create a GameObject (Player or Enemy).
2) Add `HealthSystem`.
3) Optional: Add `ShieldSystem` and `StatusEffectSystem`.

### Step 2: Add visuals (optional)
1) Add `HealthVisualSystem`.
2) Assign:
   - `Main Renderer` (SpriteRenderer)
   - UI Images for damage/heal flash (optional)
   - Particle systems (optional)
   - Audio source + clips (optional)

### Step 3: Create UI (optional)
1) Create a Canvas.
2) Add `HealthUIController` to a parent UI object.
3) Add one or more UI children:
   - `HealthBar`
   - `HealthText`
   - `HealthIcon`
4) Assign the `HealthSystem` to the controller.

### Step 4: Optional event channel
1) Create `HealthEventChannelSO`.
2) Assign it to any systems you want to drive via ScriptableObject events.

---

## 5) Code usage (common patterns)

### A) Subscribe to events (recommended)
```csharp
using UnityEngine;
using KalponicStudio.Health;

public class HealthListener : MonoBehaviour
{
    [SerializeField] private HealthSystem health;

    private void OnEnable()
    {
        health.HealthChanged += OnHealthChanged;
        health.DamageTaken += OnDamageTaken;
        health.Death += OnDeath;
    }

    private void OnDisable()
    {
        health.HealthChanged -= OnHealthChanged;
        health.DamageTaken -= OnDamageTaken;
        health.Death -= OnDeath;
    }

    private void OnHealthChanged(int current, int max) { }
    private void OnDamageTaken(int amount) { }
    private void OnDeath() { }
}
```

### B) Deal damage (simple)
```csharp
health.TakeDamage(25);
```

### C) Deal damage with type and options
```csharp
DamageInfo info = new DamageInfo
{
    Amount = 40,
    Type = DamageType.Fire,
    BypassShield = false,
    IgnoreMitigation = false,
    Source = gameObject,
    SourceTag = "Enemy"
};

health.TakeDamage(info);
```

### D) Heal or revive
```csharp
health.Heal(20);
health.Revive(50);
```

### E) Apply status effects
```csharp
statusEffectSystem.ApplyPoison(5f, 3);
statusEffectSystem.ApplyRegeneration(6f, 4);
statusEffectSystem.ApplySpeedBoost(8f, 1.5f);
```

### F) Apply a Health Profile
```csharp
public class HealthProfileApplier : MonoBehaviour
{
    [SerializeField] private HealthProfileSO profile;
    [SerializeField] private HealthSystem health;
    [SerializeField] private ShieldSystem shield;

    private void Awake()
    {
        profile.ApplyTo(health);
        profile.ApplyTo(shield);
    }
}
```

---

## 6) UI setup (step-by-step)

### Health bar + text + icon
1) Create a Canvas.
2) Add `HealthUIController` to a parent UI object.
3) Add children:
   - `HealthBar` (Slider/Image based)
   - `HealthText` (TextMeshPro or Text)
   - `HealthIcon` (Image)
4) Assign the `HealthSystem` to `HealthUIController`.

Tips:
- `HealthUIController` can auto-find children if enabled.
- Use `HealthUIManager` for multiple units.

---

## 7) Extensions (optional, Pro glue)

These live in `Extensions/` and can be added only when you need them.

Combat
- `TeamComponent` (team + faction + friendly fire flag)
- `DamageRouter` (team-aware damage application)
- `DamageSourceInfo` + `DamageRules`

Tip: Leave `FactionId` empty if you only want team-based checks.

Persistence
- `HealthSnapshotComponent` + `IHealthSerializable`
- `HealthSnapshot` (capture/restore health, shield, statuses)

UI
- `WorldSpaceHealthBar`
- `BossHealthBar`

---

## 8) Recommended setup by game type

Shmup (Sky Force style)
- HealthSystem + ShieldSystem + HealthVisualSystem
- UI: simple health bar + boss bar (if needed)

Metroidvania
- HealthSystem + StatusEffectSystem + optional downed/revive
- UI: health bar + text

Tower Defense
- HealthSystem + damage types + resistances
- UI: world-space bars (many units)

RTS
- HealthSystem + damage types + resistances + upgrades (external)
- UI: world-space bars + selection UI

Action RPG
- HealthSystem + StatusEffectSystem + downed/revive
- UI: health + status icons

---

## 9) How to use events

Code (C# events)
- HealthSystem: `HealthChanged`, `DamageTaken`, `Healed`, `Death`, `Downed`, `Revived`
- ShieldSystem: `ShieldChanged`, `ShieldAbsorbed`, `ShieldDepleted`, `ShieldRestored`
- StatusEffectSystem: `EffectApplied`, `EffectExpired`

Inspector (UnityEvents)
- UnityEvents are still available in each system for drag-and-drop wiring.

Event Channel (optional)
- `HealthEventChannelSO` can be assigned if you prefer ScriptableObject-based events.

---

## 10) Damage types and mitigation

- `DamageType` includes Generic, Physical, Fire, Ice, Poison, Electric, True.
- Flat and percent mitigation are applied unless damage is True or IgnoreMitigation is set.
- Damage can optionally bypass shields.

---

## 11) Status effects and stacking

Each effect supports:
- Duration and tick interval
- Amount per tick
- Stacking mode (Refresh, Extend, Stack)
- Max stacks

Default behavior:
- Poison: stacks (up to 5)
- Regeneration: refresh
- Speed Boost: refresh

---

## 12) Downed / Revive

Optional flow:
- On lethal damage: enter Downed state instead of Death
- Revive restores health and exits Downed
- If timer expires: Death fires

---

## 13) Health Profiles

`HealthProfileSO` can configure:
- Health settings
- Mitigation and resistances
- Downed / revive settings
- Invulnerability settings
- Shield settings (if present)

Use this for fast setup across many enemies/units.

---

## 14) Common issues

- If no events fire, check if you subscribed to C# events or assigned UnityEvents.
- If a shield exists but damage hits health, ensure `ShieldSystem` is on the same object.
- If low health warning does not show, ensure `HealthVisualSystem` has a low health overlay image.

---

## 15) Notes on modularity

You can use any subset:
- Health only
- Health + UI
- Health + Shield + UI
- Health + Status + Visuals
- Full setup

This is designed to remain clear, simple, and easy to debug.

---

## 16) Separation of Function vs Visuals (recommended)

Keep gameplay logic separate from visuals:
- Core logic: `HealthSystem`, `ShieldSystem`, `StatusEffectSystem`
- Visuals/feedback: `HealthVisualSystem`
- UI display: `HealthUIController` + UI components

Core systems should never depend on visuals or UI.
Visuals and UI should only listen to events.

---

## 17) Enhanced Serialization (Persistence Extension)

The persistence extension now supports comprehensive state capture and restoration:

### Features
- **Versioned Snapshots**: Includes version numbers and timestamps for compatibility
- **JSON Serialization**: Easy save/load to files or PlayerPrefs
- **Complete State Capture**: Health, shields, status effects, resistances, regeneration settings
- **Transform Data**: Optional position/rotation saving
- **Metadata**: Entity name and tag (optional)
- **Validation**: Automatic data integrity checks and repair

### Usage
```csharp
// Capture comprehensive snapshot
var snapshot = healthSnapshotComponent.CaptureSnapshot();

// Save to JSON
string json = snapshot.ToJson();
File.WriteAllText("save.json", json);

// Load from JSON
string json = File.ReadAllText("save.json");
var snapshot = HealthSnapshot.FromJson(json);
healthSnapshotComponent.RestoreSnapshot(snapshot);
```

### Context Menu Tools
- Right-click HealthSnapshotComponent → "Save Snapshot to PlayerPrefs"
- Right-click HealthSnapshotComponent → "Load Snapshot from PlayerPrefs"

---

## 18) Code Quality & Development Tools

### .editorconfig
The project includes a comprehensive `.editorconfig` file that enforces:
- Consistent indentation (4 spaces)
- UTF-8 encoding
- Trailing whitespace removal
- C# naming conventions
- Unity-specific code style rules

### Analyzer Configuration
- `.globalconfig`: Roslyn analyzer settings for performance and reliability
- `analyzers.ruleset`: XML rule set for Unity and .NET code quality rules
- `Directory.Build.props`: Build configuration for consistent compilation

### Recommended Setup
1. Ensure your IDE supports .editorconfig (Visual Studio, VS Code, Rider)
2. Install Unity's Roslyn analyzers package for enhanced Unity-specific rules
3. Enable "Code Analysis" in Unity preferences for real-time feedback

---
