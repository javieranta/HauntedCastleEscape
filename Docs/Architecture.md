# Haunted Castle Escape - Architecture Overview

## Project Structure

```
HauntedCastleEscape/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/           # Pure C# game logic
│   │   │   ├── GameState/  # Session management, progression
│   │   │   ├── Inventory/  # Inventory system logic
│   │   │   └── Progression/# Win conditions, scoring
│   │   ├── Services/       # Unity-dependent singletons
│   │   │   ├── GameManager.cs
│   │   │   ├── PixelPerfectSetup.cs
│   │   │   └── BootLoader.cs
│   │   ├── Data/           # ScriptableObject definitions
│   │   │   ├── CharacterData.cs
│   │   │   ├── ItemData.cs
│   │   │   ├── EnemyData.cs
│   │   │   └── RoomData.cs
│   │   ├── Player/         # Player controller, input handling
│   │   ├── Enemies/        # Enemy AI, behaviors
│   │   ├── Items/          # Pickup logic, item effects
│   │   ├── Rooms/          # Room management, transitions
│   │   └── UI/             # HUD, menus, inventory display
│   ├── Data/               # ScriptableObject instances
│   ├── Prefabs/            # Reusable game object templates
│   ├── Sprites/            # All 2D art assets
│   ├── Audio/              # Sound effects and music
│   ├── Scenes/             # Unity scene files
│   └── Settings/           # Input actions, render settings
├── ProjectSettings/        # Unity project configuration
├── Packages/               # Unity package dependencies
└── Docs/                   # Documentation
```

## Core Architecture Principles

### 1. Separation of Concerns

**Core Layer** (Pure C#):
- No Unity dependencies where possible
- Contains game rules, state management, logic
- Easily testable in isolation

**Services Layer** (Unity Singletons):
- GameManager: Scene transitions, global state
- AudioManager: Sound playback (to be added)
- InputManager: Input handling wrapper (to be added)
- RoomManager: Room loading and transitions (to be added)

**View Layer** (MonoBehaviours):
- PlayerController: Unity physics, input response
- EnemyController: AI behaviors, pathfinding
- UI components: Display only, no game logic

### 2. Data-Driven Design

All game content is defined in ScriptableObjects:

- **CharacterData**: Movement, combat, secret passage access
- **ItemData**: Effects, visuals, key properties
- **EnemyData**: Behavior, stats, spawning rules
- **RoomData**: Layout, connections, spawn points

This allows:
- Easy content iteration without code changes
- Designer-friendly editing in Unity Inspector
- Serializable for save/load

### 3. State Management

**GameSession** holds all run state:
- Lives, energy, score
- Inventory contents
- Key pieces collected
- Death marker positions
- Current room/floor

**GameManager** handles meta-state:
- Current game state (menu, playing, etc.)
- Selected character
- Scene transitions

### 4. Room System

Rooms are discrete screens with:
- 4 cardinal door connections (N/E/S/W)
- Floor transitions (stairs up/down, trapdoors)
- Secret passages (character-gated)
- Spawn points for player entry positions

Room transitions:
1. Player triggers door/transition
2. Fade out
3. Load new room data
4. Position player at correct spawn point
5. Spawn enemies/items
6. Fade in

### 5. Physics & Timing

- **FixedUpdate** for all physics and movement
- Fixed timestep ensures deterministic behavior
- Seedable RNG for procedural generation
- 2D physics only (no Z-axis gameplay)

## Key Systems

### Inventory System
- 3 slots maximum
- Pick up: E key / gamepad button
- Drop: Q key / gamepad button
- Automatic use for keys at doors

### Combat System
- Character-specific attack types
- Invulnerability frames after hit
- Knockback on damage
- Special enemies require special items

### Energy System
- Drains over time (configurable rate)
- Drains on movement (optional)
- Food items restore energy
- Energy reaching 0 = lose life

### Progression System
- 3 key pieces scattered across castle
- Collecting all 3 = Great Key
- Exit room accessible from start
- Great Key required to win

## Scene Flow

```
Boot → MainMenu → CharacterSelect → Game ←→ Pause
                                     ↓
                              GameOver / Victory
                                     ↓
                                 MainMenu
```

## Input System

Three control schemes:
1. **Keyboard & Mouse** (Modern): WASD + Space/E
2. **Gamepad**: Left stick + face buttons
3. **Classic**: QWRE + T/Z (Sinclair-style)

Full rebinding support via Unity Input System.
