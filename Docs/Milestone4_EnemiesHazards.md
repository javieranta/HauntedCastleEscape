# Milestone 4 — Enemies & Hazards

## Overview

This milestone implements the complete enemy system with AI behaviors, regular and special enemies, hazard zones, and integration with the inventory counter-item system.

## Components Created

### Enemy System

#### Enemy (`Assets/Scripts/Enemies/Enemy.cs`)
Base enemy controller with AI state machine and combat handling.

**Features:**
- State machine AI: Idle, Patrol, Chase, Attack, Wander, Stunned
- Multiple behavior patterns (Chaser, Wanderer, Patrol, Ambush, Ranged)
- Implements IDamageable interface for receiving damage
- Special enemy handling with instant kill mechanics
- Counter-item checking via PlayerInventory
- Knockback and stun mechanics
- Death animation and cleanup

**AI States:**
| State | Description |
|-------|-------------|
| Idle | Standing still, watching for player |
| Patrol | Moving between waypoints |
| Chase | Pursuing the player |
| Attack | Striking the player |
| Wander | Random movement within radius |
| Stunned | Temporarily disabled |

**Key Methods:**
```csharp
void Initialize(EnemyData data, Vector2 position)  // Setup enemy
void TakeDamage(int damage, Vector2 knockback)     // IDamageable
void Stun(float duration)                          // Disable temporarily
```

#### EnemySpawner (`Assets/Scripts/Enemies/EnemySpawner.cs`)
Room-based enemy spawning and lifecycle management.

**Features:**
- Spawns enemies based on RoomData configuration
- Tracks killed enemies for respawn control
- Configurable spawn delay and max enemies per room
- Enemy type counting per room
- Range-based enemy queries
- Cleanup on room transitions

**Events:**
- `OnEnemySpawned(Enemy enemy)`
- `OnEnemyKilled(Enemy enemy)`

#### EnemyDatabase (`Assets/Scripts/Data/EnemyDatabase.cs`)
Runtime database providing enemy data without ScriptableObject assets.

**Regular Enemies:**
| Type | Behavior | Speed | Damage | HP | Notes |
|------|----------|-------|--------|-----|-------|
| Ghost | Chaser | 3.5 | 10 | 1 | Fast, chases player |
| Skeleton | Patrol | 2.5 | 15 | 2 | Guards areas |
| Spider | Wanderer | 2.0 | 10 | 1 | Dies on contact |
| Bat | Wanderer | 4.0 | 5 | 1 | Fast, erratic |
| Demon | Chaser | 3.0 | 20 | 3 | Strong, aggressive |
| Mummy | Patrol | 1.5 | 15 | 4 | Slow but tough |
| Witch | Ranged | 2.0 | 10 | 2 | Keeps distance |

**Special Enemies (Instant Kill without Counter Item):**
| Type | Display Name | Counter Item | Notes |
|------|--------------|--------------|-------|
| Vampire | Dracula | Holy Cross | Guards throne room |
| Werewolf | Hunchback | Garlic Wreath | Prowls tunnels |
| Reaper | Frankenstein | Spellbook | Guards study |

### Hazard System

#### Hazard (`Assets/Scripts/Rooms/Hazard.cs`) - Updated
Stationary damage zones with visual indicators.

**Hazard Types:**
| Type | Color | Typical Damage |
|------|-------|----------------|
| Spikes | Gray | 20/sec |
| Fire | Orange | 15/sec |
| Poison | Green | 10/sec |
| Acid | Yellow-Green | 15/sec |
| Electricity | Blue | 25/sec |

**Features:**
- Damage over time while player is in contact
- Animation support
- Visual color coding by type
- Integration with PlayerHealth.DrainEnergy()

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    GameSceneInitializer                      │
│  - Creates EnemySpawner                                      │
│  - Initializes enemy system                                  │
└─────────────────────┬───────────────────────────────────────┘
                      │
         ┌────────────┼────────────┐
         ▼            ▼            ▼
┌─────────────┐ ┌──────────┐ ┌───────────┐
│EnemySpawner │ │  Enemy   │ │EnemyDatabase│
│ - Room spawn│ │ - AI FSM │ │ - All types│
│ - Lifecycle │ │ - Combat │ │ - Runtime  │
└──────┬──────┘ └─────┬────┘ └───────────┘
       │              │
       │              ▼
       │      ┌──────────────┐
       │      │PlayerInventory│
       │      │ - HasItem()  │
       │      │ - Counter    │
       │      │   items      │
       │      └──────────────┘
       ▼
┌─────────────┐
│   Hazard    │
│ - Damage    │
│   zones     │
└─────────────┘
```

## Enemy AI Flow

### Chase Behavior
```
1. Enemy in Idle state
2. Player enters detection range
3. Switch to Chase state
4. Move toward player at moveSpeed
5. When close (< 0.8 units), switch to Attack
6. Attack triggers collision damage
7. If player escapes (> 1.5x detection range), return to Idle
```

### Special Enemy Flow
```
1. Special enemy collides with player
2. Check if player has counter item:
   - PlayerInventory.HasItem(counteredByItem)
3. If has counter item:
   - Enemy is stunned for 2 seconds
   - Player passes safely
4. If NO counter item:
   - PlayerHealth.InstantKill()
   - Game Over
```

## Combat Integration

### Player Attacking Enemies
- Player uses melee attack (from PlayerCombat)
- Enemy.TakeDamage() called
- Enemy knockback applied
- Brief stun state
- If HP <= 0, enemy dies

### Enemy Attacking Player
- Contact damage on collision
- PlayerHealth.TakeDamage() called
- Invulnerability frames prevent repeated damage
- Knockback applied to player

### Hazard Damage
- Continuous damage while player overlaps hazard trigger
- Uses PlayerHealth.DrainEnergy() for gradual damage
- No knockback from hazards

## Test Room Enemy Placements

| Room | Enemies | Special Notes |
|------|---------|---------------|
| Grand Hall (Center) | None | Safe starting room |
| Trophy Room (North) | 2x Bat | Basic introduction |
| Entrance Hall (South) | 1x Ghost (50%) | Light security |
| Armory (East) | 2x Skeleton | Guards the path |
| Library (West) | 2x Ghost | Haunted atmosphere |
| Tower Base (NE) | 1x Witch (70%) | Ranged threat |
| Clock Room (NW) | 1x Mummy | Slow guardian |
| Wine Cellar (SE) | 3x Spider | Infested |
| Kitchen (SW) | 1x Spider (50%) | Light threat |
| Upper Hall (F1) | 1x Demon, 1x Bat | Difficult |
| Throne Room (F1) | **Vampire** | Key piece location |
| Secret Study | **Reaper** | Key piece location |
| Secret Tunnel | **Werewolf** | Key piece location |
| Hidden Vault | 2x Skeleton | Treasure guard |
| Dungeon Cell | Spider, Ghost, Bat | Dangerous |
| Treasure Chamber | Demon, Ghost | Behind locked door |

## Hazard Placements

| Room | Hazard | Damage/sec |
|------|--------|------------|
| Dungeon Cell | Spikes | 20 |
| Dungeon Cell | Fire | 15 |
| Treasure Chamber | Poison | 10 |

## Controls Summary

| Key | Action |
|-----|--------|
| Arrow Keys / WASD | Move |
| Space | Attack (when enemies nearby) |
| E | Interact |
| 1, 2, 3 | Select inventory slot |
| U | Use selected item |

## Files Created/Modified

### New Files
- `Assets/Scripts/Enemies/Enemy.cs`
- `Assets/Scripts/Enemies/EnemySpawner.cs`
- `Assets/Scripts/Data/EnemyDatabase.cs`
- `Docs/Milestone4_EnemiesHazards.md`

### Modified Files
- `Assets/Scripts/Rooms/Hazard.cs` - Player damage integration
- `Assets/Scripts/Services/TestRoomSetup.cs` - Enemy spawns added
- `Assets/Scripts/Services/GameSceneInitializer.cs` - EnemySpawner creation

## Integration Points

### With Player System (Milestone 2)
- PlayerHealth receives damage from enemies
- PlayerCombat damages enemies via IDamageable
- Knockback and invulnerability frames

### With Inventory System (Milestone 3)
- Special items counter specific enemies
- PlayerInventory.HasItem() checks counter items
- Cross counters Vampire, Wreath counters Werewolf, Spellbook counters Reaper

### With Room System (Milestone 1)
- EnemySpawner hooks into RoomManager events
- Enemies cleared on room exit
- Spawn data defined in RoomData

## Game Balance Notes

- Starting room has no enemies (safe zone)
- Difficulty increases with distance from center
- Upper floors have stronger enemies
- Special enemies guard key pieces (risk/reward)
- Counter items placed near special enemy locations
- Food items in Kitchen provide recovery option
- Hazards add environmental danger to dungeon areas
