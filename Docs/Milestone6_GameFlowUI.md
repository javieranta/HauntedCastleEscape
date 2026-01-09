# Milestone 6 — Game Flow & UI

## Overview

This milestone implements the complete game flow with main menu, character selection, pause functionality, game over screen, and victory screen.

## UI Components

### MainMenuUI (`Assets/Scripts/UI/MainMenuUI.cs`)
Main menu screen with game title and navigation.

**Features:**
- "HAUNTED CASTLE ESCAPE" title
- Menu options: Start Game, Character Select, Quit
- W/S or Arrow keys navigation
- Enter to confirm selection
- Credits line: "Inspired by Atic Atac (1983)"

### CharacterSelectUI (`Assets/Scripts/UI/CharacterSelectUI.cs`)
Character selection screen (see Milestone 5).

### PauseMenuUI (`Assets/Scripts/UI/PauseMenuUI.cs`)
In-game pause overlay.

**Features:**
- ESC to toggle pause
- Darkened overlay background
- Options: Resume, Main Menu, Quit
- Time.timeScale control (0 when paused)
- Syncs with GameManager.GameState

### GameOverUI (`Assets/Scripts/UI/GameOverUI.cs`)
Displayed when player loses all lives.

**Features:**
- "GAME OVER" text in red
- Final score display
- Options: Try Again, Main Menu, Quit
- Automatically shown by GameManager.TriggerGameOver()

### VictoryUI (`Assets/Scripts/UI/VictoryUI.cs`)
Displayed when player escapes with Great Key.

**Features:**
- "VICTORY!" text in gold
- Congratulations message
- Final score and time display
- Options: Play Again, Main Menu, Quit
- Triggered by WinConditionChecker

## Win Condition System

### WinConditionChecker (`Assets/Scripts/Services/WinConditionChecker.cs`)
Monitors victory conditions at exit doors.

**Requirements:**
1. Player must have Great Key (all 3 key pieces)
2. Player must reach exit door in Entrance Hall
3. Player must interact (E) with exit

**Flow:**
```
1. Player collects all 3 key pieces
2. PlayerInventory.OnGreatKeyFormed event fires
3. Exit door visual updates (red → green)
4. Player reaches exit and presses E
5. WinConditionChecker.TriggerVictory()
6. GameManager.TriggerVictory()
7. VictoryUI displayed
```

## Game State Flow

```
[Boot]
   ↓
[MainMenu] ←─────────────────────────┐
   ↓                                 │
[CharacterSelect]                    │
   ↓                                 │
[Playing] ←──────┐                   │
   ↓             │                   │
[Paused] ────────┘                   │
   ↓                                 │
[GameOver] ──────────────────────────┤
   ↓                                 │
[Victory] ───────────────────────────┘
```

## Controls Summary

### Menus
| Key | Action |
|-----|--------|
| W / ↑ | Move selection up |
| S / ↓ | Move selection down |
| A / ← | Previous character (select screen) |
| D / → | Next character (select screen) |
| Enter / Space | Confirm selection |
| Escape | Back / Pause toggle |

### Gameplay
| Key | Action |
|-----|--------|
| WASD / Arrows | Move |
| Space | Attack |
| E | Interact / Use exit |
| 1, 2, 3 | Select inventory slot |
| U | Use item |
| G | Drop item |
| ESC | Pause |

## Scene Structure

### Required Scenes
1. **Boot** - Initial loading, GameManager creation
2. **MainMenu** - MainMenuUI
3. **CharacterSelect** - CharacterSelectUI
4. **Game** - Main gameplay (GameSceneInitializer)
5. **GameOver** - GameOverUI
6. **Victory** - VictoryUI

### GameSceneInitializer Updates
Now creates:
- PauseMenuUI
- AudioManager
- All previously created managers

## Integration Points

### GameManager Methods
- `StartNewGame()` - Begin gameplay
- `GoToMainMenu()` - Return to main menu
- `GoToCharacterSelect()` - Show character selection
- `TriggerGameOver()` - Show game over screen
- `TriggerVictory()` - Show victory screen
- `QuitGame()` - Exit application

### PlayerHealth Integration
- `OnGameOver` event triggers GameManager.TriggerGameOver()
- Death marker system persists across respawns

### PlayerInventory Integration
- `OnGreatKeyFormed` event notifies WinConditionChecker
- `HasGreatKey` property checked for exit access

## Files Created

- `Assets/Scripts/UI/MainMenuUI.cs`
- `Assets/Scripts/UI/GameOverUI.cs`
- `Assets/Scripts/UI/VictoryUI.cs`
- `Assets/Scripts/UI/PauseMenuUI.cs`
- `Assets/Scripts/Services/WinConditionChecker.cs`

## Files Modified

- `Assets/Scripts/Services/GameSceneInitializer.cs` - Added PauseMenuUI, AudioManager
