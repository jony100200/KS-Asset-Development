# Reference Study: Happy Harvest

## Overview
**Reference Location:** `References/HappyHarvest/`  
**Project Type:** Complete Farming Simulation Game with Polish Features  
**Unity Version:** Unity 2021+ (URP, modern features)  
**Genre:** Top-Down Farming Sim with RPG Elements  
**Focus:** Polish systems, data persistence, inventory management, scene transitions, spatial grids

## 📁 Project Structure Analysis

### Core Systems Identified

#### 🎮 **Player Systems**
- **PlayerController.cs** - Movement and interaction handling
- **InventorySystem.cs** - Fixed-size inventory with stacking
- **CharacterAnimationEventHandler.cs** - Animation event management

#### 💾 **Persistence System (SaveSystem/)**
- **SaveSystem.cs** - JSON-based game state serialization
- **SaveData structs** - Player, time, terrain data structures
- **Scene data management** - Per-scene save/load

#### 🏗️ **Terrain & Grid System**
- **TerrainManager.cs** - Grid-based farming simulation
- **Crop.cs** - Crop growth and harvesting logic
- **Tilemap integration** - Unity Tilemap for visual representation

#### 🌅 **Time & Weather Systems**
- **DayCycleHandler.cs** - Day/night cycle management
- **WeatherSystem.cs** - Weather effects and transitions
- **WeatherSystemElement.cs** - Individual weather components

#### 🎬 **Scene Management**
- **SceneTransition.cs** - Trigger-based scene loading
- **SpawnPoint.cs** - Player spawn location management
- **GameManager.cs** - Central game state coordinator

#### 🎨 **UI & Effects**
- **UI/ folder** - Comprehensive UI management
- **Effects/ folder** - Particle and visual effects
- **RendererFader.cs** - Screen transition effects

## 🏗️ **Architecture Patterns Observed**

### **Singleton Manager Pattern**
- **GameManager.cs:** Central hub for all game systems
- **Static Instance Access:** Global access to managers
- **Service Locator:** Systems register with GameManager
- **Initialization Order:** Awake() method handles setup

### **Data-Driven Item System**
- **Item.cs base class:** Abstract item functionality
- **ScriptableObject Items:** External item configuration
- **Polymorphic Usage:** Different item types with shared interface
- **Database Lookup:** ItemDatabase for ID-based retrieval

### **Grid-Based Spatial Management**
- **Dictionary<Vector3Int, Data>:** Cell-based data storage
- **Tilemap Visuals:** Unity Tilemap for rendering
- **State Machines:** Crop growth stages and transitions
- **Time-Based Updates:** Growth timers and water decay

### **Event-Driven Scene Management**
- **SceneManager.sceneLoaded:** Unity scene load callbacks
- **Spawn Registration:** Automatic spawn point management
- **Camera Following:** Dynamic camera positioning

### **JSON Persistence Pattern**
- **JsonUtility Serialization:** Unity's built-in JSON
- **Struct-Based Data:** Serializable save data structures
- **File I/O:** PersistentDataPath storage
- **Selective Saving:** Player, time, terrain data

## 🎯 **Key Features for Anomaly Directive**

### **Advanced Save System**
- **Multi-Data Persistence:** Player position, inventory, time, terrain
- **Scene-Specific Data:** Per-scene save states
- **JSON Serialization:** Human-readable save files
- **Auto-Save Triggers:** Game events trigger saves

### **Sophisticated Inventory Management**
- **Fixed-Size Inventory:** 9-slot system with stacking
- **Item Stacking Logic:** Max stack size per item type
- **Equipped Item System:** Active item selection
- **Consumable Items:** Use-and-deplete mechanics

### **Scene Transition System**
- **Trigger-Based Loading:** Collider triggers scene changes
- **Spawn Point Management:** Indexed spawn locations
- **Camera Continuity:** Seamless camera transitions
- **Fade Effects:** Smooth scene transitions

### **Spatial Grid System**
- **Tilemap Integration:** Unity Tilemap for visuals
- **Cell-Based Data:** Dictionaries for per-cell state
- **Growth Simulation:** Time-based crop progression
- **Interaction Queries:** Position-based terrain queries

### **Time Management**
- **Day/Night Cycles:** Time progression with events
- **Weather Systems:** Dynamic weather effects
- **Timer-Based Mechanics:** Water decay, crop growth
- **Event Scheduling:** Time-triggered game events

## 🔧 **Technical Implementation Notes**

### **Save System Architecture**
```csharp
public static void Save()
{
    GameManager.Instance.Player.Save(ref s_CurrentData.PlayerData);
    GameManager.Instance.DayCycleHandler.Save(ref s_CurrentData.TimeSaveData);
    
    string savefile = Application.persistentDataPath + "/save.sav";
    File.WriteAllText(savefile, JsonUtility.ToJson(s_CurrentData));
}
```

### **Inventory Stacking Logic**
```csharp
public bool AddItem(Item newItem, int amount = 1)
{
    // First fill existing stacks
    for (int i = 0; i < InventorySize; ++i)
    {
        if (Entries[i].Item == newItem && Entries[i].StackSize < newItem.MaxStackSize)
        {
            int fit = Mathf.Min(newItem.MaxStackSize - Entries[i].StackSize, remainingToFit);
            Entries[i].StackSize += fit;
            remainingToFit -= fit;
        }
    }
    
    // Then use empty slots
    for (int i = 0; i < InventorySize; ++i)
    {
        if (Entries[i].Item == null)
        {
            Entries[i].Item = newItem;
            int fit = Mathf.Min(newItem.MaxStackSize, remainingToFit);
            Entries[i].StackSize = fit;
            remainingToFit -= fit;
        }
    }
    
    return remainingToFit == 0;
}
```

### **Grid-Based Terrain Management**
```csharp
private Dictionary<Vector3Int, GroundData> m_GroundData = new();
private Dictionary<Vector3Int, CropData> m_CropData = new();

public bool IsTillable(Vector3Int target)
{
    return GroundTilemap.GetTile(target) == TilleableTile;
}

public void TillAt(Vector3Int target)
{
    GroundTilemap.SetTile(target, TilledTile);
    m_GroundData.Add(target, new GroundData());
}
```

### **Scene Transition Logic**
```csharp
[RequireComponent(typeof(Collider2D))]
public class SceneTransition : MonoBehaviour
{
    public int TargetSceneBuildIndex;
    public int TargetSpawnIndex;

    private void OnTriggerEnter2D(Collider2D col)
    {
        GameManager.Instance.MoveTo(TargetSceneBuildIndex, TargetSpawnIndex);
    }
}
```

### **Spawn Point System**
```csharp
[DefaultExecutionOrder(1000)]
public class SpawnPoint : MonoBehaviour
{
    public int SpawnIndex;

    private void OnEnable()
    {
        GameManager.Instance.RegisterSpawn(this);
    }

    public void SpawnHere()
    {
        var playerTransform = GameManager.Instance.Player.transform;
        playerTransform.position = transform.position;
        
        if (GameManager.Instance.MainCamera != null)
        {
            GameManager.Instance.MainCamera.Follow = playerTransform;
            GameManager.Instance.MainCamera.LookAt = playerTransform;
        }
    }
}
```

## 🎮 **Gameplay Mechanics Extracted**

### **Core Farming Loop**
1. **Till Soil** - Prepare ground for planting
2. **Plant Seeds** - Place crops in tilled soil
3. **Water Crops** - Maintain soil moisture
4. **Wait for Growth** - Time-based crop maturation
5. **Harvest Produce** - Collect mature crops
6. **Sell/Use Items** - Economic cycle

### **Time Management**
- **Day/Night Cycle:** 24-hour simulated time
- **Weather Impact:** Rain affects watering needs
- **Seasonal Changes:** Different crop availability
- **Event Scheduling:** Time-based game events

### **Inventory System**
- **9-Slot Inventory:** Fixed capacity management
- **Stacking System:** Efficient space usage
- **Item Types:** Tools, seeds, produce, consumables
- **Equipment System:** Active tool selection

### **Persistence**
- **Auto-Save:** Critical moments saved automatically
- **Manual Save:** Player-initiated saves
- **Load on Start:** Resume from last save
- **Scene States:** Per-location data preservation

## 💡 **Lessons for Anomaly Directive**

### **What to Adopt**
- **JSON Save System:** Simple, reliable persistence
- **Inventory Stacking:** Space-efficient item management
- **Scene Transitions:** Smooth level changes
- **Grid-Based Systems:** Spatial data management
- **Time-Based Mechanics:** Dynamic progression

### **What to Adapt**
- **Tilemap Integration:** Unity Tilemap for 2D grids
- **Data Dictionaries:** Position-based state storage
- **Manager Architecture:** Centralized system coordination
- **Event Callbacks:** Unity scene load events
- **Timer Systems:** Time.deltaTime based updates

### **What to Study**
- **Data Serialization:** Choosing right serialization method
- **Grid Performance:** Dictionary vs array for grids
- **State Persistence:** What data to save/load
- **UI Updates:** Efficient inventory UI refresh
- **Camera Management:** Following vs fixed cameras

## 🔄 **Integration Opportunities**

### **TableForge Integration**
- **Item Data:** Data-driven item definitions
- **Crop Stats:** Growth time, yield, value data
- **Inventory Balance:** Statistical analysis of economy
- **Save Data:** Structured persistence data

### **KS Character Controller Integration**
- **Player Movement:** Enhanced movement in farming areas
- **Interaction System:** Tool usage and crop interaction
- **Animation States:** Farming-specific animations
- **State Management:** Farming activity states

### **DOTween Integration**
- **Scene Transitions:** Smooth fade effects
- **UI Animations:** Inventory and menu transitions
- **Growth Effects:** Visual crop growth feedback
- **Weather Transitions:** Smooth weather changes

### **NaughtyAttributes Integration**
- **Inspector Enhancement:** Better item configuration
- **Validation:** Data integrity checks
- **Organization:** Cleaner ScriptableObject editors
- **Debugging:** Runtime data inspection

## 📊 **Asset Analysis**

### **Data Assets**
- **Items/:** Complete item database with stats
- **Crops/:** Growth stages, times, yields
- **Weather/:** Weather effect configurations
- **UI Prefabs:** Interface component templates

### **Prefabs**
- **Player Prefab:** Complete player setup
- **Crop Prefabs:** Instantiable crop objects
- **Tool Prefabs:** Farm tool implementations
- **Effect Prefabs:** Particle systems

### **Scenes**
- **Farm Scene:** Main gameplay area
- **Interior Scenes:** Houses, shops
- **Modular Design:** Reusable scene templates

## 🎯 **Action Items for Anomaly Directive**

### **Immediate Adoption**
1. **Save System Implementation** - JSON-based persistence
2. **Inventory System** - Stacking inventory management
3. **Scene Transitions** - Trigger-based scene loading
4. **Grid System** - Spatial data management
5. **Time Management** - Day/night cycles

### **Short-term Integration**
1. **Spawn Points** - Player positioning system
2. **Data Persistence** - Game state saving
3. **Item Management** - Consumable and stackable items
4. **Camera System** - Dynamic camera following
5. **UI Management** - Inventory and menu systems

### **Long-term Goals**
1. **Advanced Persistence** - Multi-scene save states
2. **Complex Grids** - Multi-layer spatial systems
3. **Time-Based Events** - Scheduled game occurrences
4. **Economic Balance** - Item value and trading systems
5. **Content Pipeline** - Easy item/crop creation

## 📝 **Code Snippets for Recreation**

### **Basic Save System**
```csharp
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    [System.Serializable]
    public struct SaveData
    {
        public Vector3 PlayerPosition;
        public int PlayerHealth;
        // Add more fields as needed
    }
    
    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/save.json", json);
    }
    
    public static SaveData LoadGame()
    {
        string path = Application.persistentDataPath + "/save.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<SaveData>(json);
        }
        return new SaveData(); // Default
    }
}
```

### **Inventory with Stacking**
```csharp
using System;
using UnityEngine;

[Serializable]
public class Inventory
{
    public const int Size = 8;
    
    [Serializable]
    public class Slot
    {
        public Item Item;
        public int Count;
    }
    
    public Slot[] Slots = new Slot[Size];
    
    public bool AddItem(Item item, int amount = 1)
    {
        // Fill existing stacks first
        for (int i = 0; i < Size; i++)
        {
            if (Slots[i].Item == item && Slots[i].Count < item.MaxStack)
            {
                int space = item.MaxStack - Slots[i].Count;
                int add = Mathf.Min(space, amount);
                Slots[i].Count += add;
                amount -= add;
                if (amount <= 0) return true;
            }
        }
        
        // Use empty slots
        for (int i = 0; i < Size; i++)
        {
            if (Slots[i].Item == null)
            {
                Slots[i].Item = item;
                int add = Mathf.Min(item.MaxStack, amount);
                Slots[i].Count = add;
                amount -= add;
                if (amount <= 0) return true;
            }
        }
        
        return false; // No space
    }
}
```

### **Scene Manager with Fades**
```csharp
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneManager : PersistentSingleton<SceneManager>
{
    public Image FadeImage;
    
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }
    
    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        // Fade out
        yield return StartCoroutine(Fade(1f));
        
        // Load scene
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        while (!operation.isDone)
        {
            yield return null;
        }
        
        // Fade in
        yield return StartCoroutine(Fade(0f));
    }
    
    private IEnumerator Fade(float targetAlpha)
    {
        float startAlpha = FadeImage.color.a;
        float duration = 0.5f;
        float time = 0;
        
        while (time < duration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, time / duration);
            FadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }
}
```

### **Grid System for Spatial Queries**
```csharp
using System.Collections.Generic;
using UnityEngine;

public class GridSystem : PersistentSingleton<GridSystem>
{
    private Dictionary<Vector2Int, List<IGridElement>> grid = new();
    
    public void RegisterElement(IGridElement element, Vector2Int position)
    {
        if (!grid.ContainsKey(position))
            grid[position] = new List<IGridElement>();
        
        grid[position].Add(element);
    }
    
    public void UnregisterElement(IGridElement element, Vector2Int position)
    {
        if (grid.ContainsKey(position))
        {
            grid[position].Remove(element);
            if (grid[position].Count == 0)
                grid.Remove(position);
        }
    }
    
    public List<IGridElement> GetElementsInRadius(Vector2Int center, int radius)
    {
        List<IGridElement> result = new();
        
        for (int x = center.x - radius; x <= center.x + radius; x++)
        {
            for (int y = center.y - radius; y <= center.y + radius; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                if (Vector2Int.Distance(center, pos) <= radius)
                {
                    if (grid.ContainsKey(pos))
                    {
                        result.AddRange(grid[pos]);
                    }
                }
            }
        }
        
        return result;
    }
}

public interface IGridElement
{
    Vector2Int GridPosition { get; }
}
```

## 📝 **Documentation Quality**
- **Code Structure:** Clean, modular architecture
- **Comments:** Good inline documentation
- **Patterns:** Consistent design patterns
- **Extensibility:** Easy to extend and modify

## 🏷️ **Reference Rating**
**Relevance to Anomaly Directive:** ⭐⭐⭐⭐⭐ (5/5)  
**Code Quality:** ⭐⭐⭐⭐⭐ (5/5)  
**Architectural Value:** ⭐⭐⭐⭐⭐ (5/5)  
**Polish Level:** ⭐⭐⭐⭐⭐ (5/5)

**Summary:** Exceptional reference for game polish systems. Demonstrates professional Unity development with robust save systems, inventory management, scene transitions, and spatial grids. Perfect foundation for implementing sophisticated game mechanics with clean, maintainable code.