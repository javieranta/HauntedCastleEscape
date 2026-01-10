# Content Guide

## Adding New Rooms

Rooms are generated programmatically in `RoomDatabase.cs`. To add a new room:

### 1. Add Room to Generator Method

```csharp
// In RoomDatabase.cs, in the appropriate floor method
var myRoom = CreateRoom("unique_room_id", "Display Name", floorNumber, RoomType.Normal);

// Add doors
myRoom.northDoor = CreateDoor("destination_room_id", DoorType.Open);
myRoom.eastDoor = CreateDoor("other_room_id", DoorType.Locked, KeyColor.Red);

// Add spawns
AddEnemySpawn(myRoom, "Ghost", new Vector2(2, 3));
AddItemSpawn(myRoom, "Food_Apple", new Vector2(-2, 0));
AddHazard(myRoom, HazardType.Fire, new Vector2(0, -2), new Vector2(3, 2));

// Add secret passage
AddSecretPassage(myRoom, SecretPassageType.Bookcase, "secret_room_id", new Vector2(-5, 0));

// Register the room
floorRooms.Add(myRoom);
_allRooms[myRoom.roomId] = myRoom;
```

### 2. Door Connections

Always ensure bidirectional connections:
- If Room A has eastDoor → Room B
- Then Room B should have westDoor → Room A

## Adding New Enemies

### 1. Add Enemy Type to Enum

```csharp
// In EnemyData.cs or EnemyType enum
public enum EnemyType
{
    // ... existing types
    MyNewEnemy
}
```

### 2. Create Enemy Data in EnemyDatabase.cs

```csharp
private static void CreateAllEnemies()
{
    // ... existing enemies

    CreateEnemy(
        id: "my_new_enemy",
        name: "New Enemy Name",
        type: EnemyType.MyNewEnemy,
        health: 3,
        damage: 15,
        speed: 2.5f,
        detectionRange: 6f,
        behavior: EnemyBehavior.Chaser,
        persists: true,
        color: new Color(0.8f, 0.2f, 0.5f)
    );
}
```

### 3. Add Visual in PlaceholderSpriteGenerator.cs

```csharp
case EnemyType.MyNewEnemy:
    return GenerateCustomSprite(16, 16, myEnemyPixels, myEnemyColor);
```

## Adding New Items

### 1. Add Item to ItemDatabase.cs

```csharp
// Food items
CreateFood("food_newitem", "New Food", 30f, new Color(0.9f, 0.6f, 0.3f));

// Keys
CreateKey("key_newcolor", "New Key", KeyColor.NewColor, Color.cyan);

// Special items
CreateSpecialItem("special_item", "Counter Item", "CounteredEnemyType", Color.magenta);
```

### 2. Reference in Room Spawns

```csharp
AddItemSpawn(room, "food_newitem", new Vector2(x, y));
```

## Adding New Characters

### 1. Create Character Data in CharacterDatabase.cs

```csharp
CreateCharacter(
    type: CharacterType.NewCharacter,
    name: "New Hero",
    moveSpeed: 5.5f,
    acceleration: 22f,
    friction: 0.88f,
    attackType: AttackType.Melee,
    attackDamage: 15f,
    attackCooldown: 0.4f,
    attackRange: 1.2f,
    passageType: SecretPassageType.Bookcase,
    color: Color.cyan
);
```

### 2. Add Visual in PlaceholderSpriteGenerator.cs

```csharp
case CharacterType.NewCharacter:
    return GenerateHeroSprite(characterPixels, Color.cyan);
```

## Best Practices

1. **Room IDs**: Use format `floor_roomname` (e.g., `basement_crypt`, `castle_library`)
2. **Item IDs**: Use format `type_name` (e.g., `food_apple`, `key_red`, `acg_a`)
3. **Spawn Positions**: Keep within room bounds (-6 to 6 on X, -4 to 4 on Y)
4. **Door Placement**: North (0, 4), East (7, 0), South (0, -4), West (-7, 0)
5. **Key Piece Count**: Always exactly 3 (ACG_A, ACG_C, ACG_G)
