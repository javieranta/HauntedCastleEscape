# Milestone 3 — Inventory, Items & Door System

## Overview

This milestone implements the complete inventory system with a 3-slot limit, various item types (keys, food, key pieces, treasures), and integration with the door/lock system.

## Components Created

### Inventory System

#### PlayerInventory (`Assets/Scripts/Inventory/PlayerInventory.cs`)
Central inventory manager handling item storage, usage, and key piece collection.

**Features:**
- 3-slot inventory limit
- Key piece tracking (separate from slots - doesn't consume space)
- Great Key formation when all 3 pieces collected
- Item addition, removal, and usage
- Key checking for door unlocking
- Event-driven updates for UI

**Key Methods:**
```csharp
bool TryAddItem(ItemData item)      // Add item to inventory
ItemData RemoveItem(int slot)        // Remove item from slot
bool UseItem(int slot)               // Use item at slot
bool DropItem(int slot, position)    // Drop item in world
bool HasKey(KeyColor color)          // Check for key
bool TryUseKey(KeyColor color)       // Use and consume key
bool HasAllKeyPieces()               // Check Great Key ready
```

**Events:**
- `OnItemAdded(int slot, ItemData item)`
- `OnItemRemoved(int slot, ItemData item)`
- `OnItemUsed(int slot, ItemData item)`
- `OnKeyPieceCollected(int pieceIndex)`
- `OnGreatKeyFormed`
- `OnInventoryChanged`

### Item System

#### ItemPickup (`Assets/Scripts/Items/ItemPickup.cs`)
World representation of pickable items.

**Features:**
- Configurable pickup radius
- Auto-pickup option for certain items
- Visual bobbing animation
- Pickup animation and destruction
- Placeholder sprite generation based on item type

#### ItemSpawner (`Assets/Scripts/Items/ItemSpawner.cs`)
Handles spawning items in rooms based on RoomData.

**Features:**
- Room-based item spawning
- Persistent item tracking (key pieces stay collected)
- Runtime item creation
- Cleanup on room transitions

#### ItemDatabase (`Assets/Scripts/Data/ItemDatabase.cs`)
Runtime database providing ItemData without ScriptableObject assets.

**Item Categories:**

| Category | Items | Effect |
|----------|-------|--------|
| Keys | Red, Blue, Green, Yellow, Cyan, Magenta | Open matching doors (consumed) |
| Key Pieces | Crown, Scepter, Orb Fragments | Form Great Key (3/3 = escape) |
| Food | Chicken (+50), Bread (+30), Apple (+15), Potion (+100) | Restore energy |
| Treasures | Coin (100), Gem (250), Crown (500), Chalice (1000) | Score points |
| Special | Cross, Wreath, Spellbook, Amulet | Counter specific enemies |

### UI Components

#### InventoryUI (`Assets/Scripts/UI/InventoryUI.cs`)
HUD display for the 3-slot inventory.

**Features:**
- 3 item slots with keyboard shortcuts (1, 2, 3)
- Slot selection highlighting
- Item type indicators (F=Food, K=Key, T=Treasure, S=Special)
- Key piece progress display (3 indicators)
- Great Key formation notification
- Use (U) and Drop (G) controls

**Controls:**
- `1`, `2`, `3` - Select slot
- `U` - Use selected item
- `G` - Drop selected item

### Door System Updates

#### Door (`Assets/Scripts/Rooms/Door.cs`) - Updated
Enhanced door component with full inventory integration.

**New Features:**
- `CanPass()` - Checks PlayerInventory for keys
- `TryUnlockWithInventory()` - Auto-unlocks using player's key
- Audio support for lock/unlock sounds
- Automatic key consumption on unlock

## System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    GameSceneInitializer                      │
│  - Creates PlayerInventory                                   │
│  - Creates ItemSpawner                                       │
│  - Creates InventoryUI                                       │
└─────────────────────┬───────────────────────────────────────┘
                      │
         ┌────────────┼────────────┐
         ▼            ▼            ▼
┌─────────────┐ ┌──────────┐ ┌───────────┐
│PlayerInventory│ │ItemSpawner│ │InventoryUI│
│  - 3 slots    │ │ - Room    │ │ - Display │
│  - Keys       │ │   items   │ │ - Controls│
│  - Key pieces │ │ - Persist │ │           │
└───────┬───────┘ └─────┬────┘ └───────────┘
        │               │
        │      ┌────────┴────────┐
        │      ▼                 ▼
        │ ┌─────────┐      ┌──────────┐
        │ │ItemPickup│      │ItemDatabase│
        │ │ - World  │      │ - All items│
        │ │   items  │      │ - Runtime  │
        │ └────┬────┘      └───────────┘
        │      │
        ▼      ▼
    ┌───────────────┐
    │     Door      │
    │ - Key check   │
    │ - Auto unlock │
    └───────────────┘
```

## Item Flow

### Pickup Flow
```
1. Player walks to item (or presses E near item)
2. ItemPickup.TryPickup() called
3. PlayerInventory.TryAddItem() checks:
   - Key pieces: Auto-collect (no slot used)
   - Regular items: Check if slot available
4. If successful:
   - Add to inventory
   - Play pickup animation
   - Destroy world item
   - Fire OnItemAdded event
   - InventoryUI updates
```

### Door Unlock Flow
```
1. Player enters door trigger
2. Door.CanPass() checks:
   - Is door already unlocked? → Pass
   - Is door locked? → Check PlayerInventory.HasKey()
3. If has key:
   - Door.TryUnlockWithInventory()
   - PlayerInventory.TryUseKey() consumes key
   - Door unlocks, plays sound
   - Player passes through
4. If no key:
   - Play locked sound
   - Display "Need X key" message
```

### Great Key Formation
```
1. Player collects key piece (auto-pickup)
2. PlayerInventory.CollectKeyPiece() marks piece
3. Check if all 3 pieces collected
4. If yes:
   - Set HasGreatKey = true
   - Fire OnGreatKeyFormed event
   - UI shows "GREAT KEY FORMED"
5. Player can now exit via special door
```

## Controls Summary

| Key | Action |
|-----|--------|
| E | Interact / Pick up item |
| 1, 2, 3 | Select inventory slot |
| U | Use selected item |
| G | Drop selected item |

## Test Room Item Placements

Items are placed throughout the test rooms:

| Room | Items |
|------|-------|
| Kitchen (SW) | Chicken, Bread |
| Dungeon Cell | Red Key |
| Throne Room (F1) | Key Piece 1 |
| Secret Study | Key Piece 2 |
| Secret Tunnel | Key Piece 3 |
| Hidden Vault | Silver Crown |
| Treasure Chamber | Golden Chalice |

## Integration Points

### With Player System (Milestone 2)
- PlayerController.TryInteract() uses ItemPickup
- PlayerHealth receives energy from food items
- Input system handles inventory controls

### With Room System (Milestone 1)
- Door component uses PlayerInventory for keys
- ItemSpawner hooks into RoomManager events
- Items persist (or reset) across room visits

### With Future Enemy System (Milestone 4)
- Special items (Cross, Wreath, etc.) counter specific enemies
- ItemData.countersEnemyType field ready for use

## Files Created/Modified

### New Files
- `Assets/Scripts/Inventory/PlayerInventory.cs`
- `Assets/Scripts/Items/ItemPickup.cs`
- `Assets/Scripts/Items/ItemSpawner.cs`
- `Assets/Scripts/Data/ItemDatabase.cs`
- `Assets/Scripts/UI/InventoryUI.cs`
- `Docs/Milestone3_InventoryItems.md`

### Modified Files
- `Assets/Scripts/Rooms/Door.cs` - Added inventory integration
- `Assets/Scripts/Services/TestRoomSetup.cs` - Uses ItemDatabase
- `Assets/Scripts/Services/GameSceneInitializer.cs` - Creates new components
