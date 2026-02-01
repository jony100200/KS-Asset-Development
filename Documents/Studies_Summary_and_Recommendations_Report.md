# Studies Summary & Recommendations — Top-Level Report

**Created:** January 31, 2026

This top-level report summarizes findings from the `2.Studies refs and findings` folder and points to the detailed study report.

- Detailed study report: `Documents/2.Studies refs and findings/Studies_Summary_and_Recommendations.md`

## Executive summary
- We reviewed ~15 study documents (engine kits, toolkits, courses). Key patterns: component-based controllers, state machines, ScriptableObject-driven data, event-driven architecture, object pooling, and data-driven wave/progression systems.
- Our codebase already contains a strong Health & Combat Core, Timer service, ObjectPool, and UI systems. Major gaps: Modular Stats & Slot System, Mission/Dispatch, Formation/Auto-battle, Construction/Building, Wave/Threat frameworks, and Progression/Retention pack.

## Immediate priorities (recommended)
1. Implement `WaveDefinition` SO + `WaveManager` MVP to enable playable defense loop.
2. Prototype `VFXRule` SO + `VFXManager` to provide visual polish and show system integration.
3. Create `MissionDefinition` SO skeleton + `MissionDispatcher` using existing `Timer` service.
4. Package Health & Combat Core for release: docs, samples, PlayMode tests.

## Next steps
- Choose which prototype to begin: `WaveManager` (recommended) or `VFXManager` (polish-first).
- Assign a developer and create a feature branch, e.g., `feature/wave-manager`.

---

File locations:
- Detailed studies folder: `Documents/2.Studies refs and findings/`
- Detailed consolidated report: `Documents/2.Studies refs and findings/Studies_Summary_and_Recommendations.md`

If you want, I can: create the `WaveDefinition` SO stub and `WaveManager` script next, or generate the `VFXRule` SO and manager. Which should I start?