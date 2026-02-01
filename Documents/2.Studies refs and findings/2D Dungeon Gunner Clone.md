# Reference Study: 2D Dungeon Gunner Clone

## Overview
**Reference Location:** `References/2D Dungeon Gunner Clone/Course 2D Action Game/`  
**Project Type:** Complete Top-Down Dungeon Exploration Game with Procedural Generation  
**Unity Version:** Unity 2021+ (URP, modern input system)  
**Genre:** Top-Down Roguelike Dungeon Crawler with Weapon Progression  
**Focus:** Procedural dungeon generation, weapon systems, enemy AI, room-based gameplay

## 📁 Project Structure Analysis

### Core Systems Identified

#### 🎮 **Player Systems (Modular Character)**
- **PlayerMovement.cs** - Top-down movement with dash ability
- **PlayerWeapon.cs** - Weapon equipping and management
- **PlayerHealth.cs** - Health system with damage interface
- **PlayerEnergy.cs** - Energy system for weapon usage
- **PlayerDetection.cs** - Enemy detection and targeting
- **PlayerConfig.cs** - Player character configuration

#### 🏗️ **Dungeon Generation (Procedural System)**
- **LevelManager.cs** - Central dungeon management and progression
- **DungeonLibrary.cs** - ScriptableObject configuration for levels
- **Room.cs** - Individual room management with doors and completion
- **RoomTemplate.cs** - Room layout templates
- **Door.cs** - Room transition doors
- **Portal.cs** - Level progression portals

#### 👹 **Enemy AI System (FSM-Based)**
- **EnemyBrain.cs** - Main enemy controller with FSM
- **EnemyPattern.cs** - Movement and behavior patterns
- **EnemyHealth.cs** - Enemy health and death system
- **EnemyWeapon.cs** - Enemy weapon systems
- **FSM/ folder** - Finite state machine implementation
- **Decisions/ folder** - AI decision-making components
- **Actions/ folder** - AI action implementations

#### 🎯 **Weapon System (Type-Based & Rarity)**
- **Weapon.cs** - Abstract base weapon class
- **GunWeapon.cs** - Ranged weapon implementation
- **MeleeWeapon.cs** - Close-combat weapon implementation
- **Projectile.cs** - Bullet physics and collision
- **ItemWeapon.cs** - Weapon item data with stats
- **CharacterWeapon.cs** - Player weapon attachment

#### 🎁 **Item & Loot System**
- **PickableItem.cs** - World item pickup
- **ItemData.cs** - Base item configuration
- **Chest.cs** - Treasure chest spawning
- **BonusBase.cs** - Experience/energy bonuses
- **CoinBonus.cs** - Currency collection
- **ChestItems.cs** - Level-specific loot tables

#### 🎮 **Manager Systems (Game State)**
- **GameManager.cs** - Global game state and configurations
- **UIManager.cs** - UI management and updates
- **MenuManager.cs** - Menu navigation
- **CoinManager.cs** - Currency management

#### 📦 **Data Management (ScriptableObject Architecture)**
- **DungeonLibrary.asset** - Complete dungeon configuration
- **RoomTemplates.asset** - Room layout configurations
- **ChestItems_Level_1.asset** - Loot table configurations
- **Player/ folder** - Player character data
- **Enemy/ folder** - Enemy configurations
- **Items/ folder** - Item definitions

## 🏗️ **Architecture Patterns Observed**

### **Finite State Machine (FSM) for AI**
- **EnemyBrain.cs:** State-based enemy behavior control
- **FSMState.cs:** State container with actions and transitions
- **FSMAction.cs:** Abstract base for executable behaviors
- **FSMDecision.cs:** Abstract base for conditional checks
- **FSMTransition.cs:** Decision-based state changes
### **Concrete FSM Actions**
- **ActionAttack:** Timed weapon usage against player
- **ActionWander:** Random movement within room bounds
- **ActionCirclePattern:** Circular patrol movement
- **ActionRandomPattern:** Random directional movement
- **ActionDetectPlayer:** Player detection setup

### **Concrete FSM Decisions**
- **DecisionPlayerInSight:** Raycast-based line-of-sight check with obstacle avoidance
- **State Configuration:** Inspector-configurable states with transitions

### **ScriptableObject Data Pattern**
- **DungeonLibrary:** Complete game configuration externalized
- **ItemWeapon:** Weapon stats and prefabs in assets
- **RoomTemplate:** Room layouts as reusable data
- **Runtime Instantiation:** Data drives prefab spawning

### **Event-Driven Room System**
- **Room Events:** Player enter, room completion
- **Door Control:** Automatic door opening/closing
- **Enemy Spawning:** Event-triggered enemy creation
- **Completion Logic:** Enemy counter-based room clearing

### **Component Composition Pattern**
- **Player Composition:** Multiple components for different aspects
- **Weapon Attachment:** Dynamic weapon component addition
- **Modular Enemies:** Composable enemy behaviors
- **Interface Usage:** ITakeDamage for flexible damage systems

### **Manager Singleton Pattern**
- **LevelManager:** Dungeon and level progression
- **GameManager:** Global game state
- **UIManager:** Centralized UI control
- **Service Architecture:** Managers provide services to components

### **Procedural Generation Pattern**
- **Dungeon Prefabs:** Pre-built dungeon layouts
- **Random Enemy Placement:** Available tile-based positioning
- **Loot Randomization:** Chest item randomization
- **Level Progression:** Sequential dungeon advancement

## 🎯 **Key Features for Anomaly Directive**

### **Advanced Procedural Dungeon Generation**
- **Room-Based Layouts:** Connected rooms with doors
- **Enemy Population:** Random enemy spawning per room
- **Boss Encounters:** Special boss rooms with portals
- **Loot Distribution:** Chests with randomized items
- **Level Progression:** Multi-dungeon level system

### **Sophisticated Weapon System**
- **Dual Weapon Types:** Melee and ranged weapons
- **Rarity System:** Common to Legendary progression
- **Stat-Based Damage:** Configurable weapon statistics
- **Energy Requirements:** Resource management for attacks
- **Spread Mechanics:** Accuracy variation systems

### **FSM-Based Enemy AI**
- **State-Driven Behavior:** Patrol, chase, attack states
- **Decision Trees:** Conditional AI transitions
- **Pattern Movement:** Predictable enemy movement
- **Room Awareness:** Door and room-based AI

### **Room Completion Mechanics**
- **Enemy Counter System:** Track remaining enemies
- **Door Locking:** Prevent early progression
- **Completion Rewards:** Chests and bonuses on clear
- **Boss Portals:** Special progression mechanics

### **Comprehensive Item System**
- **Pickable Items:** World-based item collection
- **Bonus Types:** Health, energy, coins
- **Weapon Drops:** Equipment progression
- **Loot Tables:** Level-specific item pools

## 🔧 **Technical Implementation Notes**

### **Dungeon Generation Architecture**
```csharp
public class LevelManager : Singleton<LevelManager>
{
    [SerializeField] private DungeonLibrary dungeonLibrary;
    
    private void CreateDungeon()
    {
        currentDungeonGO = Instantiate(dungeonLibrary.Levels[currentLevelIndex]
            .Dungeons[currentDungeonIndex], transform);
    }
    
    private void CreateEnemies()
    {
        int enemyAmount = GetEnemyAmount();
        for (int i = 0; i < enemyAmount; i++)
        {
            Vector3 tilePos = currentRoom.GetAvailableTilePos();
            EnemyBrain enemy = Instantiate(GetEnemy(), tilePos,
                Quaternion.identity, currentRoom.transform);
        }
    }
}
```

### **Weapon System Implementation**
```csharp
public enum WeaponType { Melee, Gun }
public enum WeaponRarity { Common, Rare, Epic, Legendary }

[CreateAssetMenu(menuName = "Items/Weapon")]
public class ItemWeapon : ItemData
{
    public WeaponType WeaponType;
    public WeaponRarity Rarity;
    public float Damage;
    public float RequiredEnergy;
    public Weapon WeaponPrefab;
    
    public override void PickUp()
    {
        LevelManager.Instance.SelectedPlayer
            .GetComponent<PlayerWeapon>().EquipWeapon(WeaponPrefab);
    }
}
```

### **Room Completion Logic**
```csharp
private void EnemyKilledCallback(Transform enemyTransform)
{
    enemyCounter--;
    CreateTombstonesInEnemyPos(enemyTransform);
    CreateBonusInEnemyPos(enemyTransform);
    
    if (enemyCounter <= 0)
    {
        currentRoom.SetRoomCompleted();
        CreateChestInsideRoom();
        if (currentRoom.RoomType == RoomType.RoomBoss)
        {
            Instantiate(dungeonLibrary.Portal, tilePos,
                Quaternion.identity, currentRoom.transform);
        }
    }
}
```

### **Player Movement with Dash**
```csharp
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.3f;
    
    private IEnumerator IEDash()
    {
        currentSpeed = dashSpeed;
        ModifySpriteRenderer(transperency);
        yield return new WaitForSeconds(dashTime);
        currentSpeed = speed;
        ModifySpriteRenderer(1f);
        usingDash = false;
    }
}
```

### **FSM Enemy AI Structure**
```csharp
public class EnemyBrain : MonoBehaviour
{
    [SerializeField] private string initialStateID;
    [SerializeField] private FSMState[] states;
    
    public FSMState CurrentState { get; set; }
    
    private void Start()
    {
        ChangeState(initialStateID);
    }
    
    private void Update()
    {
        CurrentState.ExecuteState(this);
    }
    
    public void ChangeState(string newStateID)
    {
        FSMState newState = GetState(newStateID);
        CurrentState = newState;
    }
}

[Serializable]
public class FSMState
{
    public string StateID;
    public FSMAction[] Actions;
    public FSMTransition[] Transitions;
    
    public void ExecuteState(EnemyBrain enemy)
    {
        ExecuteActions();
        ExecuteTransitions(enemy);
    }
    
    private void ExecuteActions()
    {
        foreach (var action in Actions)
            action.Act();
    }
    
    private void ExecuteTransitions(EnemyBrain enemy)
    {
        foreach (var transition in Transitions)
        {
            bool decision = transition.Decision.Decide();
            string nextState = decision ? transition.TrueState : transition.FalseState;
            if (!string.IsNullOrEmpty(nextState))
                enemy.ChangeState(nextState);
        }
    }
}

public abstract class FSMAction : MonoBehaviour
{
    public abstract void Act();
}

public abstract class FSMDecision : MonoBehaviour
{
    public abstract bool Decide();
}

[Serializable]
public class FSMTransition
{
    public FSMDecision Decision;
    public string TrueState;
    public string FalseState;
}
```

## 🎮 **Gameplay Mechanics Extracted**

### **Core Dungeon Crawler Loop**
1. **Enter Room** - Doors close, enemies spawn
2. **Combat Phase** - Fight enemies in room
3. **Room Clear** - All enemies defeated, doors open
4. **Loot Collection** - Open chests, collect items
5. **Progression** - Move to next room or level
6. **Boss Fights** - Special encounters with portals

### **Weapon Progression**
- **Weapon Types:** Melee for close combat, guns for ranged
- **Rarity Tiers:** Color-coded weapon quality
- **Stat Scaling:** Damage, energy cost, fire rate
- **Accuracy System:** Spread mechanics for guns

### **Enemy AI Patterns**
- **Patrol State:** Random movement in room
- **Chase State:** Pursue player when detected
- **Attack State:** Use weapons when in range
- **Flee State:** Retreat when low health

### **Procedural Elements**
- **Random Enemies:** Varied enemy counts per room
- **Loot Randomization:** Different items per chest
- **Bonus Drops:** Experience and currency from enemies
- **Level Variety:** Multiple dungeon layouts

### **Resource Management**
- **Health System:** Damage and healing mechanics
- **Energy System:** Weapon usage costs
- **Coin Economy:** Currency collection and spending
- **Inventory Limits:** Single weapon equipped

## 💡 **Lessons for Anomaly Directive**

### **What to Adopt**
- **Procedural Dungeon Generation:** Room-based level creation
- **FSM Enemy AI:** State-driven enemy behaviors
- **Weapon Rarity System:** Equipment progression tiers
- **Room Completion Mechanics:** Enemy counter systems
- **Event-Driven Architecture:** Room and enemy events

### **What to Adapt**
- **Top-Down Movement:** 2D exploration mechanics
- **Weapon Equipping:** Single weapon slot system
- **Door Systems:** Room transition locking
- **Chest Mechanics:** Randomized loot containers
- **Boss Encounters:** Special enemy types

### **What to Study**
- **ScriptableObject Configuration:** External game data management
- **State Machine Patterns:** Clean AI state transitions
- **Procedural Spawning:** Random enemy placement
- **Resource Systems:** Health, energy, currency balance
- **Level Progression:** Dungeon advancement logic

## 🔄 **Integration Opportunities**

### **TableForge Integration**
- **Weapon Stats:** Data-driven weapon configurations
- **Enemy Stats:** AI and combat balancing data
- **Loot Tables:** Randomized item drop systems
- **Dungeon Config:** Procedural generation parameters
- **Balance Analysis:** Statistical gameplay tuning

### **KS Character Controller Integration**
- **Player Movement:** Enhanced top-down controls
- **Dash Mechanics:** Advanced movement abilities
- **Weapon Systems:** Integrated combat mechanics
- **State Management:** Player behavior states

### **DOTween Integration**
- **UI Transitions:** Smooth menu animations
- **Screen Effects:** Damage feedback and effects
- **Door Animations:** Room transition visuals
- **Weapon Effects:** Attack animations

### **NaughtyAttributes Integration**
- **Inspector Enhancement:** Better ScriptableObject editing
- **Validation:** Data integrity checks
- **Dungeon Config:** Improved level setup
- **Weapon Stats:** Enhanced stat configuration

## 📊 **Asset Analysis**

### **Data Assets**
- **DungeonLibrary.asset:** Complete game configuration
- **RoomTemplates.asset:** Room layout templates
- **ChestItems:** Level-specific loot configurations
- **Player Configs:** Character customization data
- **Enemy Configs:** AI behavior settings

### **Prefabs**
- **Dungeon Prefabs:** Pre-built level layouts
- **Enemy Prefabs:** Different enemy types
- **Weapon Prefabs:** Equippable weapons
- **Room Elements:** Doors, chests, portals
- **UI Prefabs:** Interface components

### **Scenes**
- **Main Game Scene:** Complete dungeon crawler
- **Menu Scenes:** Character selection, settings
- **Modular Design:** Easy to extend levels

## 🎯 **Action Items for Anomaly Directive**

### **Immediate Adoption**
1. **Procedural Generation** - Room-based dungeon creation
2. **FSM Enemy AI** - State-driven enemy behaviors
3. **Weapon Rarity** - Equipment progression system
4. **Room Mechanics** - Door locking and completion
5. **Event System** - Room and enemy event handling

### **Short-term Integration**
1. **Top-Down Movement** - 2D exploration controls
2. **Weapon Equipping** - Single weapon management
3. **Chest System** - Randomized loot containers
4. **Boss Encounters** - Special enemy types
5. **Resource Management** - Health and energy systems

### **Long-term Goals**
1. **Advanced AI** - Complex enemy state machines
2. **Procedural Balance** - Statistical level tuning
3. **Weapon Variety** - Multiple weapon types
4. **Loot Economy** - Item drop and collection systems
5. **Level Variety** - Multiple dungeon themes

### **ScriptableObject Dungeon Config**
```csharp
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonLibrary", menuName = "Dungeon/Library")]
public class DungeonLibrary : ScriptableObject
{
    [System.Serializable]
    public class Level
    {
        public string Name;
        public GameObject[] Dungeons; // Prefabs
        public EnemyBrain[] Enemies;
        public EnemyBrain Boss;
        public int MinEnemiesPerRoom = 3;
        public int MaxEnemiesPerRoom = 6;
    }
    
    public Level[] Levels;
    public GameObject Chest;
    public GameObject Portal;
}
```
```csharp
using System;
using UnityEngine;

public enum RoomType { RoomEntrance, RoomEnemy, RoomBoss }

public class Room : MonoBehaviour
{
    public RoomType RoomType;
    public bool RoomCompleted { get; private set; }
    
    public static event Action<Room> OnPlayerEnterEvent;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            OnPlayerEnterEvent?.Invoke(this);
        }
    }
    
    public void SetRoomCompleted()
    {
        RoomCompleted = true;
        OpenDoors();
    }
    
    public void CloseDoors() { /* Close door logic */ }
    public void OpenDoors() { /* Open door logic */ }
    
    public Vector3 GetAvailableTilePos()
    {
        // Return random available position in room
        return transform.position + (Vector3)Random.insideUnitCircle * 5f;
    }
}
```

### **Weapon Rarity System**
```csharp
using UnityEngine;

public enum WeaponRarity { Common, Rare, Epic, Legendary }

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private Color commonColor = Color.white;
    [SerializeField] private Color rareColor = Color.blue;
    [SerializeField] private Color epicColor = Color.magenta;
    [SerializeField] private Color legendaryColor = Color.yellow;
    
    public Color GetWeaponNameColor(WeaponRarity rarity)
    {
        switch (rarity)
        {
            case WeaponRarity.Common: return commonColor;
            case WeaponRarity.Rare: return rareColor;
            case WeaponRarity.Epic: return epicColor;
            case WeaponRarity.Legendary: return legendaryColor;
        }
        return Color.white;
    }
}
```

### **Enemy Counter Room Completion**
```csharp
public class LevelManager : Singleton<LevelManager>
{
    private int enemyCounter;
    
    private void PlayerEnterEventCallback(Room room)
    {
        currentRoom = room;
        if (!currentRoom.RoomCompleted)
        {
            currentRoom.CloseDoors();
            if (currentRoom.RoomType == RoomType.RoomEnemy)
            {
                CreateEnemies();
            }
        }
    }
    
    private void EnemyKilledCallback(Transform enemyTransform)
    {
        enemyCounter--;
        if (enemyCounter <= 0)
        {
            currentRoom.SetRoomCompleted();
            CreateChestInsideRoom();
        }
    }
}
```

### **Concrete Action Implementation**
```csharp
public class ActionAttack : FSMAction
{
    [SerializeField] private float timeBtwAttacks;
    private EnemyBrain enemy;
    private EnemyWeapon enemyWeapon;
    private float attackTimer;
    
    private void Awake()
    {
        enemy = GetComponent<EnemyBrain>();
        enemyWeapon = GetComponent<EnemyWeapon>();
    }
    
    public override void Act()
    {
        if (enemy.Player == null) return;
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            enemyWeapon.UseWeapon();
            attackTimer = timeBtwAttacks;
        }
    }
}
```

### **Concrete Decision Implementation**
```csharp
public class DecisionPlayerInSight : FSMDecision
{
    [SerializeField] private LayerMask obstacleMask;
    private EnemyBrain enemy;
    
    private void Awake()
    {
        enemy = GetComponent<EnemyBrain>();
    }
    
    public override bool Decide()
    {
        if (enemy.Player == null) return false;
        Vector3 direction = enemy.Player.position - transform.position;
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            direction.normalized, direction.magnitude, obstacleMask);
        return hit.collider == null; // No obstacles = in sight
    }
}
```

### **Player Dash Movement**
```csharp
using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashTime = 0.3f;
    
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float currentSpeed;
    private bool usingDash;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = speed;
    }
    
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * currentSpeed * Time.fixedDeltaTime);
    }
    
    public void Dash()
    {
        if (usingDash) return;
        usingDash = true;
        StartCoroutine(DashCoroutine());
    }
    
    private IEnumerator DashCoroutine()
    {
        currentSpeed = dashSpeed;
        yield return new WaitForSeconds(dashTime);
        currentSpeed = speed;
        usingDash = false;
    }
}
```

## 📝 **Documentation Quality**
- **Code Structure:** Well-organized modular systems with excellent FSM architecture
- **Comments:** Basic inline documentation with clear component separation
- **Patterns:** Consistent design patterns throughout (FSM, ScriptableObjects, Events)
- **Extensibility:** Highly modular design for easy addition of new enemies, weapons, rooms
- **Completeness:** Comprehensive analysis of all major systems including detailed FSM implementation

## 🏷️ **Reference Rating**
**Relevance to Anomaly Directive:** ⭐⭐⭐⭐☆ (4/5)  
**Code Quality:** ⭐⭐⭐⭐☆ (4/5)  
**Architectural Value:** ⭐⭐⭐⭐⭐ (5/5)  
**Top-Down Focus:** ⭐⭐⭐⭐⭐ (5/5)

**Summary:** Excellent reference for top-down dungeon exploration mechanics with outstanding FSM AI architecture. Demonstrates professional procedural generation, modular weapon systems, and comprehensive enemy AI with state machines. Highly relevant for expanding Anomaly Directive with exploration, combat, and AI features. Complete analysis of all systems including concrete FSM implementations.