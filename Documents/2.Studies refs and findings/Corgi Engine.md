# Reference Study: Corgi Engine

## Overview
**Reference Location:** `References/Corgi/`  
**Project Type:** Complete 2D Platformer Framework with Modular Abilities System  
**Unity Version:** Unity 2019+ (URP compatible)  
**Genre:** 2D Platformer with Extensible Character Abilities  
**Focus:** Modular character system, ability components, physics controller, comprehensive platformer mechanics

## 📁 Project Structure Analysis

### Core Systems Identified

#### 🎮 **Character Core System (Agents/Core)**
- **Character.cs** - Central character orchestrator with state machines and ability management
- **CharacterStates.cs** - Movement and condition state definitions
- **CharacterEvents.cs** - Event system for character state changes
- **CharacterLevelBounds.cs** - Level boundary management

#### 🏃 **Character Abilities (Agents/CharacterAbilities)**
- **CharacterAbility.cs** - Abstract base class for all abilities
- **CharacterJump.cs** - Advanced jumping with double jumps, coyote time, input buffering
- **CharacterHorizontalMovement.cs** - Ground and air movement
- **CharacterDash.cs** - Dashing mechanics
- **CharacterWalljump.cs** - Wall jumping and climbing
- **CharacterRun.cs** - Running speed mechanics
- **CharacterCrouch.cs** - Crouching and crawling
- **CharacterLadder.cs** - Ladder climbing
- **CharacterSwim.cs** - Swimming mechanics
- **CharacterFly.cs** - Flying mechanics
- **CharacterJetpack.cs** - Jetpack propulsion
- **CharacterHandleWeapon.cs** - Weapon equipping and management
- **CharacterInventory.cs** - Item inventory system
- **CharacterPause.cs** - Pause functionality
- **CharacterTimeControl.cs** - Time manipulation abilities

#### 🎯 **Physics & Controller (Agents/CorgiController)**
- **CorgiController.cs** - 2D physics controller with raycasting
- **CorgiControllerParameters.cs** - Physics parameter configuration
- **CorgiControllerState.cs** - Controller state tracking

#### 👹 **AI System (Agents/AI)**
- **AIBrain.cs** - AI decision-making system
- **AIDecision.cs** - AI decision components
- **AIAction.cs** - AI action components
- **CharacterPathfinderAI.cs** - Pathfinding AI

#### 💥 **Damage & Health (Agents/Damage & Health)**
- **Health.cs** - Health management with damage/knockback
- **DamageOnTouch.cs** - Damage dealing on contact
- **CharacterDamageDash.cs** - Damage dash mechanics

#### 🎨 **Weapons System (Agents/Weapons)**
- **Weapon.cs** - Abstract weapon base class
- **ProjectileWeapon.cs** - Projectile-based weapons
- **MeleeWeapon.cs** - Melee combat weapons
- **WeaponAim.cs** - Weapon aiming systems

#### 🏗️ **Environment Systems (Environment)**
- **MovingPlatform.cs** - Moving platforms
- **ButtonActivated.cs** - Button-activated objects
- **Teleporter.cs** - Teleportation mechanics
- **AutoRespawn.cs** - Automatic respawning

#### 📦 **Items & Inventory (Items)**
- **PickableItem.cs** - Collectible items
- **InventoryItem.cs** - Inventory item definitions
- **ItemPicker.cs** - Item collection mechanics

#### 🎮 **Managers (Managers)**
- **GameManager.cs** - Global game state management
- **LevelManager.cs** - Level loading and management
- **InputManager.cs** - Input handling system
- **GUIManager.cs** - UI management
- **SoundManager.cs** - Audio management

#### 📷 **Camera System (Camera)**
- **CameraController.cs** - Camera following and effects
- **CinemachineCameraController.cs** - Cinemachine integration

#### 🎵 **Feedbacks & Effects (Feedbacks)**
- **MMFeedbacks** - Comprehensive feedback system
- **Particle systems** - Visual effects
- **Screen effects** - Camera and screen effects

## 🏗️ **Architecture Patterns Observed**

### **Component-Based Ability System**
- **CharacterAbility.cs:** Abstract base with permission system and processing pipeline
- **Modular Abilities:** Each ability is a separate component (Jump, Dash, Run, etc.)
- **Ability Permissions:** Blocking states prevent conflicting abilities
- **Processing Pipeline:** EarlyProcess, Process, LateProcess passes
- **Feedback Integration:** Start/stop feedbacks for each ability

### **State Machine Architecture**
- **MMStateMachine:** Generic state machine implementation
- **Movement States:** Idle, Walking, Running, Jumping, Falling, etc.
- **Condition States:** Normal, Dead, Frozen, Stunned, etc.
- **State Transitions:** Automatic state changes based on conditions
- **Event Emission:** State change events for system coordination

### **Raycast-Based Physics Controller**
- **CorgiController:** 2D physics with raycasting collision detection
- **Platform Support:** One-way platforms, moving platforms, slopes
- **Collision States:** Grounded, colliding directions tracking
- **Force Application:** Velocity and force management
- **Parameter Tuning:** Extensive physics parameter configuration

### **Event-Driven Communication**
- **Character Events:** Jump, dash, damage events
- **Health Events:** Hit, death, revive events
- **Level Events:** Room transitions, checkpoint events
- **Input Events:** Button press/release events

### **ScriptableObject Configuration**
- **Ability Parameters:** Jump height, dash speed as ScriptableObjects
- **Weapon Stats:** Damage, fire rate, ammo as data assets
- **Level Data:** Enemy placements, item spawns as configurations
- **Game Balance:** Centralized parameter management

### **Input Abstraction Layer**
- **InputManager:** Unified input handling across platforms
- **Player ID System:** Multi-player support with separate inputs
- **Button States:** Pressed, released, held state tracking
- **Axis Input:** Analog stick and trigger support

## 🎯 **Key Features for Anomaly Directive**

### **Modular Character Abilities**
- **Plug-and-Play:** Add abilities by attaching components
- **Permission System:** Automatic conflict resolution
- **Parameter Tuning:** Extensive customization options
- **Feedback Integration:** Visual/audio effects per ability
- **State Management:** Automatic state transitions

### **Advanced Platformer Physics**
- **Raycast Controller:** Precise collision detection
- **Platform Types:** Ground, one-way, moving, slopes
- **Force System:** Velocity, acceleration, gravity control
- **Collision States:** Comprehensive collision information
- **Parameter Flexibility:** Tunable physics for different characters

### **Comprehensive Input System**
- **Multi-Player Support:** Separate input for each player
- **Platform Agnostic:** Works across all Unity platforms
- **Button States:** Detailed input state tracking
- **Analog Support:** Full controller and keyboard support
- **Input Buffering:** Delayed input execution

### **Extensible AI Framework**
- **Decision Trees:** Conditional AI behavior
- **Action System:** Executable AI behaviors
- **Pathfinding:** Grid-based navigation
- **State Machines:** AI state management
- **Modular Brains:** Customizable AI personalities

### **Robust Health & Damage System**
- **Damage Types:** Physical, knockback, stun effects
- **Health States:** Invincibility frames, damage multipliers
- **Death Handling:** Respawn, game over logic
- **Feedback System:** Damage effects and animations
- **Health UI:** Automatic health bar updates

### **Flexible Weapon System**
- **Weapon Types:** Melee, ranged, projectile-based
- **Weapon States:** Idle, attacking, reloading
- **Ammo Management:** Magazine and total ammo tracking
- **Weapon Switching:** Multiple weapon inventory
- **Aim Systems:** Auto-aim, manual aim, lock-on

## 🔧 **Technical Implementation Notes**

### **Character Ability Base Class**
```csharp
public abstract class CharacterAbility : MonoBehaviour
{
    [Header("Permissions")]
    public bool AbilityPermitted = true;
    public CharacterStates.MovementStates[] BlockingMovementStates;
    public CharacterStates.CharacterConditions[] BlockingConditionStates;
    
    public virtual bool AbilityAuthorized
    {
        get
        {
            if (_character != null)
            {
                // Check blocking states
                foreach (var state in BlockingMovementStates)
                {
                    if (state == _character.MovementState.CurrentState)
                        return false;
                }
                // Check weapon states, etc.
            }
            return AbilityPermitted;
        }
    }
    
    protected Character _character;
    protected CorgiController _controller;
    protected InputManager _inputManager;
    
    protected virtual void Start()
    {
        Initialization();
    }
    
    protected virtual void Initialization()
    {
        _character = GetComponentInParent<Character>();
        _controller = GetComponentInParent<CorgiController>();
        _inputManager = _character.LinkedInputManager;
        BindAnimator();
        _abilityInitialized = true;
    }
    
    public virtual void EarlyProcessAbility() { }
    public virtual void ProcessAbility() { }
    public virtual void LateProcessAbility() { }
    public virtual void UpdateAnimator() { }
}
```

### **Advanced Jump Mechanics**
```csharp
public class CharacterJump : CharacterAbility
{
    public int NumberOfJumps = 2;
    public float JumpHeight = 3.025f;
    public float CoyoteTime = 0.1f; // Jump after leaving platform
    public float InputBufferDuration = 0.1f; // Buffered jump input
    
    protected int NumberOfJumpsLeft;
    protected float _lastTimeGrounded;
    
    public override void ProcessAbility()
    {
        // Reset jumps when grounded
        if (_controller.State.JustGotGrounded)
        {
            NumberOfJumpsLeft = NumberOfJumps;
        }
        
        // Coyote time check
        bool canJump = (_controller.State.IsGrounded) || 
                      (Time.time - _lastTimeGrounded <= CoyoteTime);
        
        // Input buffering
        if (_inputManager.JumpButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            if (canJump && NumberOfJumpsLeft > 0)
            {
                PerformJump();
                NumberOfJumpsLeft--;
            }
        }
    }
    
    protected virtual void PerformJump()
    {
        _movement.ChangeState(CharacterStates.MovementStates.Jumping);
        float jumpForce = Mathf.Sqrt(2f * JumpHeight * Mathf.Abs(_controller.Parameters.Gravity));
        _controller.SetVerticalForce(jumpForce);
        PlayAbilityStartFeedbacks();
    }
}
```

### **Raycast Physics Controller**
```csharp
public class CorgiController : MonoBehaviour
{
    [Header("Raycasting")]
    public int NumberOfHorizontalRays = 8;
    public int NumberOfVerticalRays = 8;
    public float RayOffset = 0.1f;
    
    public CorgiControllerState State { get; protected set; }
    public CorgiControllerParameters Parameters { get; protected set; }
    
    protected Vector2 _speed;
    protected Vector2 _externalForce;
    protected float _fallSlowFactor;
    
    protected virtual void FixedUpdate()
    {
        // Cast horizontal rays
        for (int i = 0; i < NumberOfHorizontalRays; i++)
        {
            Vector2 rayOrigin = transform.position + Vector3.up * (RayOffset + i * _verticalRaySpacing);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * _normalizedHorizontalSpeed, 
                Mathf.Abs(_speed.x * Time.deltaTime) + _skinWidth, _horizontalRayMask);
            
            if (hit)
            {
                // Handle collision
                _speed.x = 0;
                State.IsCollidingRight = (_normalizedHorizontalSpeed > 0);
                State.IsCollidingLeft = (_normalizedHorizontalSpeed < 0);
            }
        }
        
        // Apply forces
        _speed += _externalForce * Time.deltaTime;
        _speed.y += Parameters.Gravity * Time.deltaTime;
        
        // Move character
        transform.Translate(_speed * Time.deltaTime);
    }
}
```

### **State Machine Implementation**
```csharp
public class MMStateMachine<T> where T : struct, IComparable, IConvertible, IFormattable
{
    public T CurrentState { get; private set; }
    public delegate void OnStateChangeDelegate();
    public OnStateChangeDelegate OnStateChange;
    
    private MonoBehaviour _component;
    private bool _sendEvents;
    
    public MMStateMachine(MonoBehaviour component, bool sendEvents)
    {
        _component = component;
        _sendEvents = sendEvents;
    }
    
    public virtual void ChangeState(T newState)
    {
        if (CurrentState.Equals(newState)) return;
        
        CurrentState = newState;
        
        if (_sendEvents && OnStateChange != null)
        {
            OnStateChange();
        }
    }
    
    public virtual void RestorePreviousState()
    {
        // Implementation for state history
    }
}
```

### **Input Management System**
```csharp
public class InputManager : MonoBehaviour
{
    public string PlayerID = "Player1";
    
    [Header("Movement")]
    public MMInput.Axis PrimaryMovement;
    
    [Header("Buttons")]
    public MMInput.Button JumpButton;
    public MMInput.Button DashButton;
    public MMInput.Button RunButton;
    
    protected virtual void Update()
    {
        // Update axis inputs
        PrimaryMovement.Update();
        
        // Update button states
        JumpButton.TriggerButtonEvents();
        DashButton.TriggerButtonEvents();
        RunButton.TriggerButtonEvents();
    }
    
    public virtual void SetMovement(MMInput.Axis movement)
    {
        PrimaryMovement = movement;
    }
    
    public virtual void SetJumpButton(MMInput.Button button)
    {
        JumpButton = button;
    }
}
```

## 🎮 **Gameplay Mechanics Extracted**

### **Core Platformer Loop**
1. **Movement** - Horizontal movement with acceleration/deceleration
2. **Jumping** - Variable height jumps with double jumps
3. **Combat** - Weapon switching and attacking
4. **Platforming** - Wall jumps, dashes, special abilities
5. **Progression** - Level completion and checkpoint system

### **Advanced Jump System**
- **Coyote Time:** Jump after leaving platform edge
- **Input Buffering:** Jump input registered before landing
- **Jump Restrictions:** Ground-only, anywhere, ladder jumps
- **Proportional Jumps:** Height based on button hold time
- **Platform Handling:** One-way, moving platform support

### **Movement States**
- **Idle:** Standing still
- **Walking/Running:** Horizontal movement
- **Jumping:** Initial jump
- **Double Jumping:** Additional air jumps
- **Falling:** Descending
- **Dashing:** High-speed movement
- **Wall Clinging:** Wall attachment
- **Swimming:** Water movement

### **Ability Interactions**
- **Blocking States:** Abilities prevent conflicting actions
- **State Transitions:** Automatic state changes
- **Force Application:** Abilities modify character forces
- **Animation Triggers:** State-based animation control

### **Physics Interactions**
- **Collision Detection:** Raycast-based precise collisions
- **Platform Types:** Solid, one-way, moving, slippery
- **Slope Handling:** Automatic slope angle detection
- **Force Accumulation:** Multiple forces combine realistically

## 💡 **Lessons for Anomaly Directive**

### **What to Adopt**
- **Modular Ability System:** Component-based character customization
- **State Machine Architecture:** Clean state management
- **Raycast Physics:** Precise 2D collision detection
- **Input Abstraction:** Platform-agnostic input handling
- **Feedback System:** Comprehensive ability effects

### **What to Adapt**
- **Character Controller:** 2D physics with raycasting
- **Ability Permissions:** Conflict resolution system
- **Jump Mechanics:** Coyote time and input buffering
- **Weapon Management:** Equipping and switching system
- **AI Framework:** Decision-action AI structure

### **What to Study**
- **Component Architecture:** How to structure modular systems
- **State Management:** Complex state interactions
- **Physics Tuning:** Raycast parameter optimization
- **Input Systems:** Multi-platform input handling
- **Ability Design:** Balancing and interaction design

## 🔄 **Integration Opportunities**

### **TableForge Integration**
- **Ability Parameters:** Data-driven ability configurations
- **Weapon Stats:** Comprehensive weapon balancing data
- **Character Stats:** Health, speed, jump parameters
- **Level Design:** Enemy placement and difficulty data
- **Game Balance:** Statistical analysis and tuning

### **KS Character Controller Integration**
- **Enhanced Movement:** Advanced 2D movement mechanics
- **Ability System:** Modular character customization
- **State Management:** Complex character state handling
- **Physics Integration:** Combined physics systems
- **Animation Control:** Advanced animator parameter management

### **DOTween Integration**
- **Ability Feedbacks:** Smooth transitions and effects
- **Camera Movements:** Dynamic camera behaviors
- **UI Animations:** Interface state transitions
- **Particle Effects:** Enhanced visual feedback
- **Screen Effects:** Damage and ability effects

### **NaughtyAttributes Integration**
- **Ability Configuration:** Better inspector for ability parameters
- **State Debugging:** Runtime state inspection
- **Parameter Validation:** Data integrity checks
- **Editor Enhancement:** Improved workflow for ability setup
- **Documentation:** Help text for complex parameters

## 📊 **Asset Analysis**

### **ScriptableObjects**
- **Ability Configurations:** Jump height, dash speed parameters
- **Weapon Definitions:** Damage, range, ammo stats
- **Level Data:** Enemy waves, item placements
- **Character Presets:** Health, abilities configurations
- **Game Balance:** Difficulty and progression parameters

### **Prefabs**
- **Character Prefabs:** Player and enemy templates
- **Ability Components:** Pre-configured ability setups
- **Weapon Prefabs:** Equipped weapon objects
- **Environment Prefabs:** Platforms, hazards, collectibles
- **Effect Prefabs:** Particles and visual effects

### **Scenes**
- **Demo Scenes:** Platformer level examples
- **Test Scenes:** Individual mechanic testing
- **Tutorial Scenes:** Ability introduction levels
- **Boss Scenes:** Complex encounter designs

## 🎯 **Action Items for Anomaly Directive**

### **Immediate Adoption**
1. **Ability System Architecture** - Component-based character abilities
2. **State Machine Implementation** - Movement and condition states
3. **Raycast Physics Controller** - Precise 2D collision detection
4. **Input Management System** - Platform-agnostic input handling
5. **Jump Mechanics** - Advanced jumping with coyote time

### **Short-term Integration**
1. **Character Controller** - 2D physics with raycasting
2. **Ability Permissions** - Conflict resolution and blocking
3. **Weapon System** - Equipping and management framework
4. **Feedback System** - Visual and audio ability effects
5. **AI Framework** - Decision-action AI structure

### **Long-term Goals**
1. **Modular Characters** - Customizable character builds
2. **Advanced Physics** - Slope, platform, and force interactions
3. **Complex Abilities** - Wall jumping, dashing, flying mechanics
4. **Multi-Character Support** - Different character types
5. **Level Editor Tools** - Custom level creation tools

## 📝 **Code Snippets for Recreation**

### **Basic Ability Template**
```csharp
using UnityEngine;

public class CharacterDash : CharacterAbility
{
    [Header("Dash")]
    public float DashDistance = 3f;
    public float DashDuration = 0.2f;
    public float DashCooldown = 1f;
    
    protected bool _dashing;
    protected float _dashTimer;
    protected float _cooldownTimer;
    protected Vector2 _dashDirection;
    
    protected override void HandleInput()
    {
        if (_inputManager.DashButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            if (!_dashing && _cooldownTimer <= 0f && AbilityAuthorized)
            {
                StartDash();
            }
        }
    }
    
    public override void ProcessAbility()
    {
        if (_dashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                EndDash();
            }
        }
        else
        {
            _cooldownTimer -= Time.deltaTime;
        }
    }
    
    protected virtual void StartDash()
    {
        _dashing = true;
        _dashTimer = DashDuration;
        _cooldownTimer = DashCooldown;
        
        _dashDirection = _controller.Speed.normalized;
        if (_dashDirection == Vector2.zero)
        {
            _dashDirection = _character.IsFacingRight ? Vector2.right : Vector2.left;
        }
        
        _controller.SetForce(_dashDirection * (DashDistance / DashDuration));
        _movement.ChangeState(CharacterStates.MovementStates.Dashing);
        PlayAbilityStartFeedbacks();
    }
    
    protected virtual void EndDash()
    {
        _dashing = false;
        _controller.SetForce(Vector2.zero);
        _movement.ChangeState(CharacterStates.MovementStates.Idle);
        PlayAbilityStopFeedbacks();
    }
    
    public override void ResetAbility()
    {
        _dashing = false;
        _dashTimer = 0f;
        _cooldownTimer = 0f;
    }
}
```

### **State Machine Usage**
```csharp
public class Character : MonoBehaviour
{
    public MMStateMachine<CharacterStates.MovementStates> MovementState;
    
    protected virtual void Awake()
    {
        MovementState = new MMStateMachine<CharacterStates.MovementStates>(
            this.gameObject, true);
        MovementState.ChangeState(CharacterStates.MovementStates.Idle);
    }
    
    protected virtual void Update()
    {
        // Check conditions and change states
        if (_controller.Speed.x != 0 && _controller.State.IsGrounded)
        {
            MovementState.ChangeState(CharacterStates.MovementStates.Walking);
        }
        else if (!_controller.State.IsGrounded && _controller.Speed.y > 0)
        {
            MovementState.ChangeState(CharacterStates.MovementStates.Jumping);
        }
        else if (!_controller.State.IsGrounded && _controller.Speed.y < 0)
        {
            MovementState.ChangeState(CharacterStates.MovementStates.Falling);
        }
        else
        {
            MovementState.ChangeState(CharacterStates.MovementStates.Idle);
        }
    }
}

public enum CharacterStates
{
    public enum MovementStates
    {
        Idle,
        Walking,
        Running,
        Jumping,
        Falling,
        Dashing,
        WallClinging,
        Swimming
    }
}
```

### **Raycast Collision Detection**
```csharp
public class CorgiController : MonoBehaviour
{
    public int NumberOfHorizontalRays = 8;
    public int NumberOfVerticalRays = 8;
    public LayerMask PlatformMask;
    
    protected virtual void HandleHorizontalCollisions()
    {
        float rayLength = Mathf.Abs(_speed.x * Time.deltaTime) + _skinWidth;
        Vector2 rayDirection = _speed.x > 0 ? Vector2.right : Vector2.left;
        
        for (int i = 0; i < NumberOfHorizontalRays; i++)
        {
            Vector2 rayOrigin = transform.position + 
                Vector3.up * (_rayOffset + i * _verticalRaySpacing);
            
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, rayDirection, 
                rayLength, PlatformMask);
            
            if (hit)
            {
                // Handle collision
                _speed.x = 0;
                transform.position += (Vector3)(rayDirection * (hit.distance - _skinWidth));
                
                if (rayDirection == Vector2.right)
                    State.IsCollidingRight = true;
                else
                    State.IsCollidingLeft = true;
            }
        }
    }
}
```

## 📝 **Documentation Quality**
- **Code Structure:** Excellent modular architecture with clear separation of concerns
- **Comments:** Comprehensive inline documentation and help text
- **Patterns:** Consistent design patterns throughout the framework
- **Extensibility:** Highly extensible with abstract base classes and interfaces
- **Completeness:** Complete platformer framework with all essential systems

## 🏷️ **Reference Rating**
**Relevance to Anomaly Directive:** ⭐⭐⭐⭐⭐ (5/5)  
**Code Quality:** ⭐⭐⭐⭐⭐ (5/5)  
**Architectural Value:** ⭐⭐⭐⭐⭐ (5/5)  
**Modularity:** ⭐⭐⭐⭐⭐ (5/5)

**Summary:** Exceptional 2D platformer framework with outstanding modularity and extensibility. Demonstrates professional game architecture with component-based abilities, state machines, and raycast physics. Perfect foundation for building complex 2D games with clean, maintainable code and extensive customization options.