# Room System Documentation

## Overview

The room system manages discrete room screens with door transitions, floor changes via stairs/trapdoors, and character-gated secret passages.

## Core Components

### RoomManager (Service)

The central service managing all room operations.

**Responsibilities:**
- Room database and lookup
- Loading/unloading rooms
- Transition coordination
- Spawn position management

**Key Methods:**
```csharp
LoadRoom(string roomId, string spawnPointId)
TransitionThroughDoor(DoorDirection direction)
TransitionThroughFloor(FloorTransitionType type)
TransitionThroughSecretPassage(SecretPassageType type, string destinationId)
```

**Events:**
- `OnRoomLoadStarted` - Fired before room load begins
- `OnRoomLoadCompleted` - Fired after room is fully loaded
- `OnRoomTransition` - Fired on any room change

### TransitionManager (Service)

Handles screen fade effects during transitions.

**Methods:**
```csharp
FadeOut(float duration)  // Screen to black
FadeIn(float duration)   // Black to clear
FadeOutIn(Action midFadeAction, float duration)  // Full transition
Flash(Color color, float duration)  // Quick flash effect
```

### Room (Component)

Represents an instantiated room in the scene.

**Features:**
- Door registration and lookup
- Enemy/item spawning
- Visual setup from RoomData

### Door (Component)

Handles door interactions and room transitions.

**Door Types:**
- `Open` - Always passable
- `Locked` - Requires colored key
- `OneWay` - Single direction only
- `Hidden` - Must be revealed first

**Key Colors:**
- Red, Blue, Green, Yellow, Cyan, Magenta

### FloorTransitionTrigger (Component)

Handles vertical movement between floors.

**Transition Types:**
- `StairsUp` - Go to higher floor
- `StairsDown` - Go to lower floor
- `Trapdoor` - Fall to lower floor

### SecretPassageTrigger (Component)

Character-gated secret passages.

**Passage Types:**
| Type | Character | Disguise |
|------|-----------|----------|
| Bookcase | Wizard | Library bookshelf |
| Clock | Knight | Grandfather clock |
| Barrel | Serf | Wine barrel/vat |

### Hazard (Component)

Stationary damage zones.

**Hazard Types:**
- Spikes, Fire, Poison, Acid, Electricity

## Data Structures

### RoomData (ScriptableObject)

Defines room layout and contents:

```csharp
public class RoomData : ScriptableObject
{
    string roomId;
    string displayName;
    int floorNumber;
    RoomType roomType;
    bool isStartRoom;
    bool isExitRoom;

    DoorConnection northDoor, eastDoor, southDoor, westDoor;
    FloorTransition stairsUp, stairsDown, trapdoor;
    List<SecretPassage> secretPassages;

    List<SpawnPoint> playerSpawnPoints;
    List<EnemySpawn> enemySpawns;
    List<ItemSpawn> itemSpawns;
    List<HazardSpawn> hazardSpawns;
}
```

### DoorConnection

```csharp
public class DoorConnection
{
    bool exists;
    DoorType doorType;
    KeyColor requiredKeyColor;
    string destinationRoomId;
    Vector2 spawnOffset;
}
```

## Transition Flow

1. Player triggers transition (door/stairs/passage)
2. RoomManager validates transition is possible
3. TransitionManager fades out
4. Current room unloaded
5. New room instantiated from RoomData
6. Player positioned at appropriate spawn point
7. TransitionManager fades in
8. Gameplay resumes

## Test Level Layout

The TestRoomSetup creates a debug level:

```
Floor 0:
[NW Clock] [N Trophy] [NE Tower]
[W Library] [C Grand Hall] [E Armory] → [Red Door] → [Treasure]
[SW Kitchen] [S Entrance*] [SE Wine Cellar]
                              ↓ trapdoor
                           [Dungeon]

Floor 1:
         [N Throne - KeyPiece]
              ↑ stairs
         [C Upper Hall]

Hidden Rooms:
- Secret Study (via Bookcase from Library) - KeyPiece
- Hidden Vault (via Clock from Clock Room) - Treasure
- Secret Tunnel (via Barrel from Wine Cellar) - KeyPiece

* = Exit room
```

## Adding New Rooms

1. Create RoomData ScriptableObject
2. Configure doors, transitions, passages
3. Add spawn points for items/enemies
4. Register with RoomManager

Or use TestRoomSetup.CreateRoomData() as template for runtime creation.

## Debug Tools

Press F1 in Game scene to toggle debug UI showing:
- Current room info
- Available transitions (clickable)
- Character selection
- Game state
