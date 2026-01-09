# Milestone 7 — Audio & Polish

## Overview

This milestone implements the audio system with sound effects and music support, completing the core game systems.

## AudioManager

### AudioManager (`Assets/Scripts/Audio/AudioManager.cs`)
Central singleton for all audio playback.

**Features:**
- Music playback with track switching
- Sound effect pool (8 sources by default)
- Volume controls (Master, Music, SFX)
- Persistent across scenes (DontDestroyOnLoad)
- Convenience methods for common sounds

### Volume Settings
| Setting | Default | Range |
|---------|---------|-------|
| Master Volume | 1.0 | 0-1 |
| Music Volume | 0.5 | 0-1 |
| SFX Volume | 0.8 | 0-1 |

### Music Tracks
```csharp
public enum MusicTrack
{
    None,
    Menu,      // Main menu background
    Game,      // Gameplay background
    Victory,   // Victory screen
    GameOver   // Game over screen
}
```

### Sound Effects
```csharp
public enum SoundEffect
{
    None,
    Pickup,          // Item collected
    DoorOpen,        // Door opened
    DoorLocked,      // Tried locked door
    KeyUnlock,       // Door unlocked with key
    PlayerHurt,      // Player took damage
    PlayerDeath,     // Player died
    EnemyHit,        // Enemy took damage
    EnemyDeath,      // Enemy killed
    Attack,          // Player attack
    MenuSelect,      // Menu navigation
    MenuConfirm,     // Menu selection confirmed
    KeyPiece,        // Key piece collected
    GreatKey,        // Great Key formed
    SecretPassage,   // Secret passage used
    Stairs           // Stairs used
}
```

## Usage

### Playing Music
```csharp
// Play specific track
AudioManager.Instance.PlayMusic(MusicTrack.Game);

// Control playback
AudioManager.Instance.StopMusic();
AudioManager.Instance.PauseMusic();
AudioManager.Instance.ResumeMusic();
```

### Playing Sound Effects
```csharp
// Using enum
AudioManager.Instance.PlaySFX(SoundEffect.Pickup);

// Using convenience methods
AudioManager.Instance.PlayPickupSound();
AudioManager.Instance.PlayDoorOpenSound();
AudioManager.Instance.PlayPlayerHurtSound();

// Play at specific position
AudioManager.Instance.PlaySFXAtPosition(SoundEffect.EnemyDeath, enemyPosition);
```

### Adjusting Volume
```csharp
AudioManager.Instance.MasterVolume = 0.8f;
AudioManager.Instance.MusicVolume = 0.5f;
AudioManager.Instance.SFXVolume = 1.0f;
```

## Integration Points

### GameSceneInitializer
- Creates AudioManager if not present
- AudioManager persists via DontDestroyOnLoad

### Recommended Integration (Future)
- PlayerHealth: PlayPlayerHurtSound(), PlayPlayerDeathSound()
- Enemy: PlayEnemyHitSound(), PlayEnemyDeathSound()
- ItemPickup: PlayPickupSound(), PlayKeyPieceSound()
- Door: PlayDoorOpenSound(), PlayDoorLockedSound(), PlayKeyUnlockSound()
- PlayerCombat: PlayAttackSound()
- Menu UIs: PlayMenuSelectSound(), PlayMenuConfirmSound()
- PlayerInventory: PlayGreatKeySound()
- SecretPassageTrigger: PlaySecretPassageSound()
- FloorTransitionTrigger: PlayStairsSound()

## Audio Asset Setup

AudioManager exposes serialized fields for audio clips:

```csharp
[Header("Music Tracks")]
[SerializeField] private AudioClip menuMusic;
[SerializeField] private AudioClip gameMusic;
[SerializeField] private AudioClip victoryMusic;
[SerializeField] private AudioClip gameOverMusic;

[Header("Sound Effects")]
[SerializeField] private AudioClip pickupSound;
[SerializeField] private AudioClip doorOpenSound;
// ... etc
```

To add audio:
1. Import audio files into Unity project
2. Select AudioManager GameObject
3. Assign clips to appropriate fields in Inspector

## Sound Effect Pool

The AudioManager uses an object pool for SFX playback:
- Default pool size: 8 AudioSource components
- Prevents audio source starvation under heavy use
- Round-robin allocation for concurrent sounds

## Project Completion Summary

With Milestone 7 complete, the game includes:

### Core Systems (Milestones 0-4)
- Room-based map with transitions
- Player movement and combat
- 3-slot inventory with items
- Enemies with AI behaviors
- Hazard zones

### Game Flow (Milestones 5-7)
- Three playable character classes
- Character-specific secret passages
- Complete menu system
- Win/lose conditions
- Audio framework

## Files Created

- `Assets/Scripts/Audio/AudioManager.cs`

## Files Modified

- `Assets/Scripts/Services/GameSceneInitializer.cs` - Creates AudioManager

## Future Enhancements

1. **Audio Assets**: Add actual audio files
2. **Sound Integration**: Call AudioManager from game events
3. **Music Crossfade**: Smooth transitions between tracks
4. **3D Audio**: Positional audio for enemies
5. **Audio Settings**: In-game volume sliders
