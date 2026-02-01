# 🏰 **Stronghold Restoration — Planning Blueprint**

**Version 1.0 — Master Development Roadmap**

*Created: January 31, 2026*

This document serves as the comprehensive blueprint for "Stronghold Restoration" — our showcase game demonstrating the 11 Kalponic Studio sellable tool systems. It combines game design, system integration, development roadmap, and technical rules.

---

## 📋 **Executive Summary**

**Vision:** A systems-driven strategy-defense game that showcases modular Unity tools through gameplay.

**Goal:** Build a complete, marketable game while developing and testing 11 reusable tool systems.

**Timeline:** 8-12 weeks to MVP, 6 months to full release.

**Success Criteria:**
- All 11 systems integrated and functional
- Game demonstrates each system's capabilities
- Codebase serves as living documentation
- Ready for asset store packaging

---

## 🎮 **Game Concept**

### **Core Premise**
You are the leader of a ruined medieval stronghold in a hopepunk world. Rebuild civilization by defending against corruption waves, gathering resources, and closing dungeon sources of monsters.

### **Player Journey**
1. **Arrival:** Assess ruined stronghold
2. **Defense:** Survive initial waves with basic towers
3. **Rebuilding:** Repair structures, assign workers
4. **Exploration:** Send heroes on dungeon missions
5. **Climax:** Close major dungeons, stabilize the region

### **Win Condition**
Close 3 major dungeons + fully restore stronghold + survive final wave.

### **Game Loop**
```
Enemy waves spawn → Defend with towers/units → Gather resources → Rebuild structures → Send missions → Close dungeons → Reduced pressure → Progress
```

---

## 🧩 **System Architecture**

### **11 Core Systems (Football Team Model)**

Each system is independent but integrates seamlessly:

1. **Modular Stats & Slot System** — Hero progression, equipment
2. **Health & Combat Core** — Damage, shields, status effects  
3. **VFX Rules Automation** — Visual feedback system
4. **Audio & Ambience System** — Sound management
5. **Timer & Task Framework** — Time-based logic
6. **Mission / Dispatch Framework** — Dungeon missions
7. **Formation & Auto-Battle Framework** — Combat automation
8. **Construction & Building Framework** — Base building
9. **Wave & Threat Framework** — Enemy spawning
10. **Progression & Retention Pack** — Daily rewards, achievements
11. **Notifications & Markers** — Player feedback

### **Integration Rules**

- **Zero Mandatory Dependencies:** Systems work standalone
- **Interface-Based Coupling:** Optional integration via interfaces
- **Event-Driven Communication:** Loose coupling through events
- **SO Configuration:** Data-driven setup, no code changes

### **Layer Structure**
```
Domain (Pure Logic)
↑
Application (System Integration)  
↑
Presentation (Unity Components)
```

---

## 🗓️ **Development Roadmap**

### **Phase 1: Foundation (Weeks 1-4)**
**Goal:** Core combat loop functional

- **Week 1:** Polish Health & Combat Core for integration
- **Week 2:** Build Wave & Threat Framework (basic spawning)
- **Week 3:** Create Formation & Auto-Battle Framework  
- **Week 4:** Integrate combat systems, test basic defense

**Milestone:** Player can defend against waves with towers

### **Phase 2: World Building (Weeks 5-8)**
**Goal:** Town rebuilding and resource management

- **Week 5:** Build Construction & Building Framework
- **Week 6:** Add worker assignment and resource generation
- **Week 7:** Create Mission / Dispatch Framework
- **Week 8:** Integrate town systems, test rebuilding loop

**Milestone:** Player can repair buildings and send basic missions

### **Phase 3: Progression & Polish (Weeks 9-12)**
**Goal:** Hero progression and game completion

- **Week 9:** Build Modular Stats & Slot System
- **Week 10:** Add Progression & Retention Pack
- **Week 11:** Complete VFX Rules, Audio, Notifications
- **Week 12:** Polish, balance, and test full game loop

**Milestone:** Complete game with all systems integrated

### **Phase 4: Packaging & Launch (Weeks 13-16)**
**Goal:** Prepare for asset store and marketing

- **Week 13:** Extract standalone system packages
- **Week 14:** Create documentation and samples
- **Week 15:** Testing and bug fixes
- **Week 16:** Asset store submission and launch

**Milestone:** Systems available for sale, game published

---

## 📏 **Technical Rules**

### **Coding Standards**
- Follow [Unity Coding Rules V3.md](Unity Coding Rules V3.md)
- Three-layer architecture: Domain → Application → Presentation
- ScriptableObject-driven data
- Event-based communication

### **System Design Rules**
- **Independence:** Each system works in empty scene
- **Composition:** Systems enhance each other optionally
- **Simplicity:** No over-engineering
- **Performance:** Profile regularly, minimize allocations

### **Integration Patterns**
- **Interfaces:** `IHealthProvider`, `IStatsProvider`
- **Events:** `CombatEvents.OnDamage`, `MissionEvents.OnComplete`
- **SO Channels:** Shared event channels for cross-system communication

### **Asset Organization**
```
Assets/
├── Kalponic Studio/
│   ├── [SystemName]/
│   │   ├── Runtime/
│   │   ├── Editor/
│   │   ├── Tests/
│   │   └── Samples/
│   └── Shared/ (Common utilities)
├── Scenes/
├── Prefabs/
└── ScriptableObjects/
```

### **Testing Requirements**
- PlayMode tests for each system
- Integration tests for system combinations
- Performance benchmarks
- Cross-platform compatibility

---

## 🎯 **Game Features by System**

### **Health & Combat Core**
- Tower defense mechanics
- Hero health management
- Status effects on enemies
- Damage type resistances

### **Wave & Threat Framework**
- Timed enemy waves
- Difficulty scaling
- Base integrity tracking
- Alert system for incoming waves

### **Construction & Building Framework**
- Building repair mechanics
- Worker assignment
- Resource generation
- Upgrade paths

### **Mission / Dispatch Framework**
- Dungeon exploration missions
- Hero team dispatch
- Timer-based resolution
- Reward systems

### **Modular Stats & Slot System**
- Hero stat progression
- Equipment slots
- Module upgrades
- Rarity system

### **Formation & Auto-Battle Framework**
- Tower auto-targeting
- Hero combat automation
- Role-based positioning
- Battle result summaries

### **VFX Rules Automation**
- Combat impact effects
- Building construction visuals
- Mission completion celebrations
- Environmental feedback

### **Audio & Ambience System**
- Combat sound effects
- Ambient stronghold audio
- Music state changes
- Notification sounds

### **Timer & Task Framework**
- Mission timers
- Cooldowns on abilities
- Daily reward timers
- Construction progress

### **Progression & Retention Pack**
- Daily login rewards
- Achievement unlocks
- Milestone progress bars
- Title system

### **Notifications & Markers**
- Wave incoming alerts
- Mission completion toasts
- World space markers
- UI feedback

---

## 📊 **Success Metrics**

### **Technical**
- All systems load in empty scene
- <100ms system initialization
- Zero crashes in normal gameplay
- Clean separation of concerns

### **Gameplay**
- 30-60 minute play sessions
- Clear progression curve
- Intuitive controls
- Balanced difficulty

### **Business**
- Systems extracted as standalone packages
- Complete documentation
- Working sample scenes
- Ready for asset store

---

## 🚧 **Risk Mitigation**

### **Scope Creep**
- Stick to core loop: Defend → Rebuild → Explore → Progress
- No side features unless they demonstrate systems

### **System Complexity**
- Build MVP systems first, enhance later
- Test integration early and often

### **Performance Issues**
- Profile weekly
- Use object pooling for frequent spawns
- Optimize particle systems

### **Timeline Slips**
- Weekly milestones with demos
- Parallel system development where possible
- Buffer time for integration testing

---

## 📝 **Change Log**

- **v1.0 (Jan 31, 2026):** Initial planning document created
- Comprehensive roadmap and technical rules established

---

## 📞 **Contact & Resources**

- **Lead Developer:** [Kalponic Studio]
- **Technical Rules:** [Unity Coding Rules V3.md](Unity Coding Rules V3.md)
- **System Inventory:** See codebase analysis
- **Version Control:** Git with feature branches

This blueprint will evolve as development progresses. Regular reviews ensure we stay on track for delivering both a great game and sellable tool systems.