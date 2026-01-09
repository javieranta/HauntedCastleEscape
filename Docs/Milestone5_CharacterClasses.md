# Milestone 5 — Character Classes

## Overview

This milestone implements the three playable character classes from Atic Atac, each with unique abilities, attack styles, and access to different secret passages.

## Character Classes

### Wizard
- **Name**: Dorian the Wizard
- **Color**: Purple
- **Attack Type**: Projectile (Magic)
- **Move Speed**: 4.5
- **Attack Range**: 8 (longest)
- **Secret Passage**: Bookcases

**Description**: A powerful mage who attacks with magical projectiles. Can pass through bookcases to access secret areas.

### Knight
- **Name**: Sir Dorian the Knight
- **Color**: Silver/Gray
- **Attack Type**: Melee (Sword)
- **Move Speed**: 4.0
- **Attack Damage**: 2 (highest)
- **Attack Range**: 1.2 (shortest)
- **Secret Passage**: Grandfather Clocks

**Description**: A brave warrior who attacks with a sword. Can pass through grandfather clocks to access secret areas.

### Serf
- **Name**: Dorian the Serf
- **Color**: Brown
- **Attack Type**: Thrown
- **Move Speed**: 5.0 (fastest)
- **Attack Range**: 5 (medium)
- **Secret Passage**: Barrels

**Description**: A resourceful peasant who throws objects. Can pass through barrels to access secret areas.

## Components

### CharacterData (`Assets/Scripts/Data/CharacterData.cs`)
ScriptableObject defining character attributes.

**Properties:**
- `characterType` - Wizard, Knight, or Serf
- `characterName`, `displayName`, `description`
- `moveSpeed`, `acceleration`, `friction`
- `attackType`, `attackCooldown`, `attackDamage`, `projectileSpeed`, `attackRange`
- `accessiblePassageType` - Which secret passages this character can use
- Visual and audio properties

### CharacterDatabase (`Assets/Scripts/Data/CharacterDatabase.cs`)
Runtime database providing character data without ScriptableObject assets.

**Static Methods:**
- `GetCharacter(CharacterType type)` - Get character data by type
- `GetAllCharacters()` - Get array of all characters
- `Wizard`, `Knight`, `Serf` - Direct accessors

### CharacterSelectUI (`Assets/Scripts/UI/CharacterSelectUI.cs`)
Character selection screen for choosing which hero to play.

**Features:**
- Visual preview with character color
- Stats display (speed, attack type, passages)
- Description text
- A/D or Arrow keys to cycle characters
- Enter to confirm, Escape to go back

## Secret Passage System

### SecretPassageTrigger (`Assets/Scripts/Rooms/SecretPassageTrigger.cs`)
Handles character-specific secret passage access.

**Passage Types:**
| Type | Accessible By | Locations |
|------|---------------|-----------|
| Bookcase | Wizard | Library → Secret Study |
| Clock | Knight | Clock Room → Hidden Vault |
| Barrel | Serf | Wine Cellar → Secret Tunnel |

**Flow:**
1. Player approaches secret passage
2. Check if player's character type matches passage type
3. If match, highlight passage and show interaction prompt
4. Press E to use passage and transition to secret room

## Integration Points

### GameSession.SelectedCharacter
Static property storing the selected character type across scenes.

### PlayerSetup Integration
- Reads `GameSession.SelectedCharacter` on spawn
- Configures PlayerController, PlayerCombat, PlayerAnimator
- Sets character-specific visual appearance

### GameManager Integration
- `SelectedCharacter` property synced with GameSession
- Character select scene flow integrated

## Character Balance

| Aspect | Wizard | Knight | Serf |
|--------|--------|--------|------|
| Speed | Medium | Slow | Fast |
| Range | Long | Short | Medium |
| Damage | Low | High | Low |
| Safety | Safest | Riskiest | Balanced |
| Passages | Bookcase | Clock | Barrel |

## Files Created/Modified

### New Files
- `Assets/Scripts/UI/CharacterSelectUI.cs`

### Modified Files
- `Assets/Scripts/Core/GameState/GameSession.cs` - Added static SelectedCharacter

### Existing (Pre-built)
- `Assets/Scripts/Data/CharacterData.cs`
- `Assets/Scripts/Data/CharacterDatabase.cs`
- `Assets/Scripts/Player/PlayerSetup.cs`
- `Assets/Scripts/Rooms/SecretPassageTrigger.cs`
