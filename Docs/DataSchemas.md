# Data Schemas

## RoomData

```csharp
[CreateAssetMenu(fileName = "Room", menuName = "HauntedCastle/RoomData")]
public class RoomData : ScriptableObject
{
    // Identity
    string roomId;              // Unique room identifier
    string displayName;         // Human-readable name
    int floorNumber;            // 0=Basement, 1=Castle, 2=Tower
    RoomType roomType;          // Normal, Corridor, Hall, etc.

    // Visual
    Color ambientColor;         // Room lighting tint

    // Doors (N/E/S/W)
    DoorConnection northDoor;
    DoorConnection eastDoor;
    DoorConnection southDoor;
    DoorConnection westDoor;

    // Floor Transitions
    FloorTransition stairsUp;
    FloorTransition stairsDown;
    FloorTransition trapdoor;

    // Spawns
    List<SpawnPoint> playerSpawnPoints;
    List<EnemySpawn> enemySpawns;
    List<ItemSpawn> itemSpawns;
    List<HazardSpawn> hazardSpawns;
    List<SecretPassage> secretPassages;

    // Flags
    bool isStartRoom;
    bool isExitRoom;
}

public class DoorConnection
{
    bool exists;
    string destinationRoomId;
    DoorType doorType;          // Open, Locked, Hidden
    KeyColor requiredKeyColor;  // For locked doors
}

public class FloorTransition
{
    bool exists;
    string destinationRoomId;
    Vector2 position;
}
```

## CharacterData

```csharp
[CreateAssetMenu(fileName = "Character", menuName = "HauntedCastle/CharacterData")]
public class CharacterData : ScriptableObject
{
    // Identity
    string characterId;
    string characterName;
    CharacterType characterType;    // Wizard, Knight, Serf

    // Movement
    float moveSpeed;                // Units per second
    float acceleration;             // How fast to reach max speed
    float friction;                 // Deceleration multiplier

    // Combat
    AttackType attackType;          // Melee, Ranged
    float attackDamage;
    float attackCooldown;
    float attackRange;

    // Special
    SecretPassageType accessiblePassageType;
    // Wizard: Bookcase, Knight: Clock, Serf: Barrel

    // Visuals
    Sprite idleSprite;
    Sprite[] walkSprites;
    Color tintColor;
}
```

## EnemyData

```csharp
[CreateAssetMenu(fileName = "Enemy", menuName = "HauntedCastle/EnemyData")]
public class EnemyData : ScriptableObject
{
    // Identity
    string enemyId;
    string displayName;
    EnemyType enemyType;

    // Stats
    int health;
    int damage;
    float moveSpeed;
    float detectionRange;
    float wanderRadius;

    // Behavior
    EnemyBehavior behavior;         // Patrol, Wanderer, Chaser, Ambush, Ranged
    bool persistsAfterContact;      // Destroy on hit or keep chasing?

    // Special
    bool isSpecialEnemy;            // Requires counter item
    string counteredByItem;         // Item ID that counters this enemy

    // Visuals
    Sprite sprite;
    Sprite[] animationSprites;
    Color tintColor;
}
```

## ItemData

```csharp
[CreateAssetMenu(fileName = "Item", menuName = "HauntedCastle/ItemData")]
public class ItemData : ScriptableObject
{
    // Identity
    string itemId;
    string displayName;
    ItemType itemType;              // Food, Key, KeyPiece, Treasure, Special

    // Effects
    float energyRestore;            // For food items
    KeyColor keyColor;              // For keys
    int keyPieceIndex;              // 0, 1, or 2 for ACG pieces
    int scoreValue;                 // For treasures
    string countersEnemyType;       // For special items

    // Behavior
    bool consumeOnUse;
    bool stackable;

    // Visuals
    Sprite sprite;
    Color glowColor;
}
```

## Enumerations

```csharp
public enum CharacterType { Wizard, Knight, Serf }
public enum RoomType { Normal, Corridor, Hall, Library, Kitchen, Throne, Tower, Dungeon, Crypt, Chamber }
public enum DoorType { Open, Locked, Hidden }
public enum KeyColor { None, Red, Blue, Green, Yellow, Cyan, Magenta }
public enum ItemType { Food, Key, KeyPiece, Treasure, Special }
public enum EnemyType { Ghost, Skeleton, Spider, Bat, Demon, Witch, Vampire, Mummy, Werewolf, Reaper }
public enum EnemyBehavior { Patrol, Wanderer, Chaser, Ambush, Ranged }
public enum SecretPassageType { Bookcase, Clock, Barrel }
public enum HazardType { Spikes, Fire, Poison, Acid, Electricity }
```
