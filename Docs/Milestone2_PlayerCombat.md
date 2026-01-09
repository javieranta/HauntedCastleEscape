# Milestone 2 — Player & Combat System

## Overview

This milestone implements the complete player character system including movement, health/energy, combat, and input handling. The system is designed to support three distinct character types (Wizard, Knight, Serf) with different attack styles.

## Components Created

### Core Player Components

#### PlayerController (`Assets/Scripts/Player/PlayerController.cs`)
The main movement and interaction controller for the player character.

**Features:**
- 4-directional movement with smooth acceleration
- Collision handling via Rigidbody2D
- Knockback support for damage feedback
- Input callbacks for Unity Input System
- Integration with PlayerHealth for energy drain
- Integration with PlayerCombat for attacks
- Integration with PlayerAnimator for visual feedback

**Key Methods:**
- `OnMove(InputAction.CallbackContext)` - Movement input handler
- `OnAttack(InputAction.CallbackContext)` - Attack input handler
- `OnInteract(InputAction.CallbackContext)` - Interaction handler
- `ApplyKnockback(Vector2, float)` - Apply knockback force
- `SetCharacterData(CharacterData)` - Configure from character data

#### PlayerHealth (`Assets/Scripts/Player/PlayerHealth.cs`)
Manages the player's energy (health), lives, and damage handling.

**Features:**
- Energy system that drains over time (passive drain)
- Additional energy drain while moving
- 3-life system with respawn on death
- Invulnerability frames (i-frames) after taking damage
- Visual damage feedback (flash effect)
- Death marker placement
- Game over handling

**Events:**
- `OnEnergyChanged(float current, float max)` - Energy updates
- `OnLivesChanged(int lives)` - Life count changes
- `OnDamageTaken(float damage)` - Damage taken
- `OnDeath` - Player died
- `OnGameOver` - All lives lost

#### PlayerCombat (`Assets/Scripts/Player/PlayerCombat.cs`)
Handles all combat functionality including attacks and damage dealing.

**Attack Types:**
- **Projectile** (Wizard): Fires magical projectiles
- **Melee** (Knight): Close-range sword attacks with area damage
- **Thrown** (Serf): Throws objects with medium range

**Features:**
- Attack cooldown system
- Directional attacks
- Melee hit detection via Physics2D.OverlapCircle
- Projectile spawning and initialization
- Audio support for attack sounds
- Visual effects for melee attacks

#### Projectile (`Assets/Scripts/Player/Projectile.cs`)
Component for ranged attack projectiles.

**Features:**
- Configurable speed, damage, and range
- Automatic rotation to face movement direction
- Trigger-based collision detection
- Support for player and enemy projectiles
- Piercing capability (optional)
- Hit effects on impact
- Reflection support (for shields)

#### PlayerAnimator (`Assets/Scripts/Player/PlayerAnimator.cs`)
Handles player sprite animations.

**Features:**
- 4-directional idle sprites
- 4-directional walk animations
- Attack animation support
- Sprite flipping for left/right
- Color modification for effects
- CharacterData integration for sprites

#### PlayerInputHandler (`Assets/Scripts/Player/PlayerInputHandler.cs`)
Bridges Unity Input System with PlayerController.

**Supported Actions:**
- Move (Vector2)
- Attack (Button)
- Interact (Button)
- Pause (Button)
- DropItem (Button)

**Features:**
- Automatic action map binding
- Control scheme switching
- Input enable/disable

### Support Components

#### PlayerSetup (`Assets/Scripts/Player/PlayerSetup.cs`)
Factory component for creating fully-configured player GameObjects.

**Features:**
- Automatic component attachment
- CharacterData initialization
- Placeholder sprite generation
- Static factory method for runtime creation

#### PlayerSpawner (`Assets/Scripts/Services/PlayerSpawner.cs`)
Service for managing player spawning and positioning.

**Features:**
- Singleton pattern
- Spawn at position or spawn point
- Player control enable/disable
- Event subscription for death/game over

#### PlayerHUD (`Assets/Scripts/UI/PlayerHUD.cs`)
Development HUD for displaying player status.

**Displays:**
- Energy bar with percentage
- Life count (hearts)
- Character name and attack type

### Data Components

#### CharacterDatabase (`Assets/Scripts/Data/CharacterDatabase.cs`)
Runtime database providing CharacterData without ScriptableObject assets.

**Characters:**

| Character | Attack Type | Speed | Damage | Secret Passage |
|-----------|-------------|-------|--------|----------------|
| Wizard    | Projectile  | 4.5   | 1      | Bookcase       |
| Knight    | Melee       | 4.0   | 2      | Clock          |
| Serf      | Thrown      | 5.0   | 1      | Barrel         |

#### CharacterData Updates (`Assets/Scripts/Data/CharacterData.cs`)
Added fields:
- `characterName` - Display name
- `characterColor` - Character tint color

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      GameSceneInitializer                    │
│  - Creates PlayerSpawner                                     │
│  - Creates PlayerHUD                                         │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                       PlayerSpawner                          │
│  - Spawns Player GameObject                                  │
│  - Positions at spawn points                                 │
└─────────────────────┬───────────────────────────────────────┘
                      │
                      ▼
┌─────────────────────────────────────────────────────────────┐
│                    Player GameObject                         │
├─────────────────────────────────────────────────────────────┤
│  PlayerSetup          - Factory/initialization              │
│  PlayerController     - Movement & input                    │
│  PlayerHealth         - Energy, lives, damage               │
│  PlayerCombat         - Attacks & damage dealing            │
│  PlayerAnimator       - Sprite animations                   │
│  PlayerInputHandler   - Input System bridge                 │
│  PlayerInput          - Unity Input System                  │
│  Rigidbody2D          - Physics movement                    │
│  BoxCollider2D        - Collision detection                 │
│  SpriteRenderer       - Visual rendering                    │
└─────────────────────────────────────────────────────────────┘
```

## Input Configuration

The game uses the Unity Input System with the following default bindings:

### Keyboard (Modern - WASD)
- **Move**: WASD
- **Attack**: Space
- **Interact**: E
- **Pause**: Escape

### Keyboard (Classic - QWRE)
- **Move**: QWRE
- **Attack**: Space
- **Interact**: X
- **Pause**: Escape

### Gamepad
- **Move**: Left Stick / D-Pad
- **Attack**: South Button (A/Cross)
- **Interact**: West Button (X/Square)
- **Pause**: Start

## Testing

To test the player system:

1. Open the Game scene in Unity
2. Press Play
3. Use WASD to move the player
4. Press Space to attack
5. Press F1 to open debug panel
6. Try different room transitions to see player repositioning

### Debug Controls
- **F1**: Toggle room debug panel
- **Number keys (1-9)**: Teleport to specific rooms

## Integration Points

### With Room System (Milestone 1)
- Player spawns at room spawn points
- PlayerSpawner coordinates with RoomManager
- Room transitions reposition player

### With Inventory System (Milestone 3)
- PlayerInputHandler has DropItem action ready
- PlayerController.OnInteract calls future inventory methods

### With Enemy System (Milestone 4)
- IDamageable interface for damage dealing
- Projectile detects "Enemy" tag
- Melee uses enemyLayer for hit detection

## Files Created/Modified

### New Files
- `Assets/Scripts/Player/PlayerController.cs`
- `Assets/Scripts/Player/PlayerHealth.cs`
- `Assets/Scripts/Player/PlayerCombat.cs`
- `Assets/Scripts/Player/Projectile.cs`
- `Assets/Scripts/Player/PlayerAnimator.cs`
- `Assets/Scripts/Player/PlayerInputHandler.cs`
- `Assets/Scripts/Player/PlayerSetup.cs`
- `Assets/Scripts/Services/PlayerSpawner.cs`
- `Assets/Scripts/Data/CharacterDatabase.cs`
- `Assets/Scripts/UI/PlayerHUD.cs`
- `Docs/Milestone2_PlayerCombat.md`

### Modified Files
- `Assets/Scripts/Data/CharacterData.cs` - Added characterName, characterColor
- `Assets/Scripts/Services/GameSceneInitializer.cs` - Added PlayerSpawner, PlayerHUD
- `Assets/Scenes/Game.unity` - Updated with new components
