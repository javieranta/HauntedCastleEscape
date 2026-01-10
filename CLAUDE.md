# CLAUDE.md - Haunted Castle Escape

## Project Overview
A clean-room remake inspired by the 1983 ZX Spectrum game "Atic Atac". Unity 2D game with procedural generation, multiple enemy types, and classic gameplay mechanics.

---

## 🚨 IMPORTANT: Read Learnings When Stuck

**Before spending hours debugging a problem, check `/Docs/LEARNINGS.md`**

This file contains solutions to hard problems already solved during development, including:
- Game freezes and their causes
- Physics corruption issues
- Performance problems
- Common Unity pitfalls

### When You Solve a Hard Problem

**Document it in `/Docs/LEARNINGS.md`** so future sessions don't waste time re-solving the same issue. Include:
1. The problem/symptom
2. The root cause
3. The solution with code examples
4. Files affected

---

## Key Technical Decisions

### Avoid CompareTag - Use String Comparison
```csharp
// DON'T - Freezes if tag doesn't exist
if (other.CompareTag("Wall"))

// DO - Safe even with missing tags
if (other.tag == "Wall")
```

### Always Use Unscaled Time in Coroutines
```csharp
// Use Time.unscaledDeltaTime instead of Time.deltaTime
// Use WaitForSecondsRealtime instead of WaitForSeconds
```

### Cache Runtime-Created Assets
All procedurally generated sprites/textures must be cached statically to avoid performance issues.

---

## Project Structure

```
/Assets
├── Scripts/
│   ├── Core/           # GameManager, ScoreManager, etc.
│   ├── Player/         # PlayerController, PlayerHealth
│   ├── Enemies/        # Enemy, EnemyAbilities, Projectiles
│   ├── Items/          # Pickups, Keys
│   ├── Rooms/          # Room generation, doors
│   ├── UI/             # HUD, menus
│   ├── Audio/          # AudioManager, ProceduralSound
│   ├── Effects/        # VisualEffectsManager
│   └── Data/           # ScriptableObject definitions
├── Data/               # ScriptableObject instances
├── Prefabs/
├── Sprites/
└── Scenes/
```

---

## Documentation Files

| File | Purpose |
|------|---------|
| `/Docs/LEARNINGS.md` | **Solutions to hard problems - READ FIRST when stuck** |
| `/Docs/CHANGES_FREEZE_FIX.md` | Detailed history of freeze bug fixes |
| `/Docs/Architecture.md` | System design (if exists) |

---

## Build & Run

- Unity 2022 LTS or later
- Platform: Windows x64
- Required packages: Input System, 2D Pixel Perfect, TextMeshPro

---

## Current Status

- ✅ Player movement and combat
- ✅ Enemy AI with multiple types
- ✅ Enemy projectiles (fixed freeze issues)
- ✅ Room transitions
- ✅ Visual effects system
- 🔧 Audio system (disabled for testing)
- 📋 Procedural castle generation
- 📋 Full game loop (keys, victory condition)
