using System.Collections.Generic;
using UnityEngine;
using HauntedCastle.Data;
using HauntedCastle.Enemies;
using HauntedCastle.Items;

namespace HauntedCastle.Services
{
    /// <summary>
    /// Creates test rooms at runtime for development.
    /// This is a temporary solution until proper room assets are created.
    /// </summary>
    public class TestRoomSetup : MonoBehaviour
    {
        [Header("Test Configuration")]
        [SerializeField] private bool createTestRooms = true;
        [SerializeField] private int testRoomCount = 9; // 3x3 grid

        private void Awake()
        {
            if (createTestRooms)
            {
                // Check if RoomDatabase already populated the rooms
                if (RoomDatabase.Instance != null && RoomDatabase.Instance.TotalRoomCount > 0)
                {
                    Debug.Log("[TestRoomSetup] RoomDatabase already has rooms, skipping test room creation");
                    return;
                }

                // Ensure RoomManager exists before creating rooms
                EnsureRoomManager();
                CreateTestRooms();
            }
        }

        private void EnsureRoomManager()
        {
            if (RoomManager.Instance == null)
            {
                var rmObj = new GameObject("RoomManager");
                rmObj.AddComponent<RoomManager>();
            }
        }

        private void CreateTestRooms()
        {
            if (RoomManager.Instance == null)
            {
                Debug.LogError("[TestRoomSetup] RoomManager not found!");
                return;
            }

            // Create a 3x3 grid of test rooms on floor 0
            // Room layout:
            // [NW] [N] [NE]
            // [W]  [C] [E]
            // [SW] [S] [SE]

            var rooms = new List<RoomData>();

            // Center room (starting room) - Safe zone, no enemies
            var centerRoom = CreateRoomData("room_center", "Grand Hall", 0, true, false);
            centerRoom.northDoor = CreateDoor("room_north");
            centerRoom.southDoor = CreateDoor("room_south");
            centerRoom.eastDoor = CreateDoor("room_east");
            centerRoom.westDoor = CreateDoor("room_west");
            centerRoom.stairsUp = CreateFloorTransition("room_f1_center");
            rooms.Add(centerRoom);

            // North room - Bats patrol here
            var northRoom = CreateRoomData("room_north", "Trophy Room", 0);
            northRoom.southDoor = CreateDoor("room_center");
            northRoom.eastDoor = CreateDoor("room_northeast");
            northRoom.westDoor = CreateDoor("room_northwest");
            northRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(-2f, 1f)));
            northRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(3f, -1f)));
            rooms.Add(northRoom);

            // South room - Exit area, light security
            var southRoom = CreateRoomData("room_south", "Entrance Hall", 0, false, true);
            southRoom.northDoor = CreateDoor("room_center");
            southRoom.eastDoor = CreateDoor("room_southeast");
            southRoom.westDoor = CreateDoor("room_southwest");
            southRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(0f, 2f), 0.5f));
            rooms.Add(southRoom);

            // East room - Armory has skeleton guards
            var eastRoom = CreateRoomData("room_east", "Armory", 0);
            eastRoom.westDoor = CreateDoor("room_center");
            eastRoom.northDoor = CreateDoor("room_northeast");
            eastRoom.southDoor = CreateDoor("room_southeast");
            // Add a locked door (red key)
            eastRoom.eastDoor = CreateLockedDoor("room_east_secret", KeyColor.Red);
            eastRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(3f, 0f)));
            eastRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(-2f, 2f), 0.6f));
            rooms.Add(eastRoom);

            // West room - Library haunted by ghosts
            var westRoom = CreateRoomData("room_west", "Library", 0);
            westRoom.eastDoor = CreateDoor("room_center");
            westRoom.northDoor = CreateDoor("room_northwest");
            westRoom.southDoor = CreateDoor("room_southwest");
            // Add secret bookcase passage (Wizard only)
            westRoom.secretPassages.Add(CreateSecretPassage(SecretPassageType.Bookcase, "room_hidden_study", new Vector2(-5f, 0f)));
            westRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(-3f, 1f)));
            westRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(2f, -1f), 0.5f));
            rooms.Add(westRoom);

            // Corner rooms
            var neRoom = CreateRoomData("room_northeast", "Tower Base", 0);
            neRoom.southDoor = CreateDoor("room_east");
            neRoom.westDoor = CreateDoor("room_north");
            // Tower has a witch!
            neRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Witch, new Vector2(0f, 0f), 0.7f));
            rooms.Add(neRoom);

            var nwRoom = CreateRoomData("room_northwest", "Clock Room", 0);
            nwRoom.southDoor = CreateDoor("room_west");
            nwRoom.eastDoor = CreateDoor("room_north");
            // Add clock passage (Knight only)
            nwRoom.secretPassages.Add(CreateSecretPassage(SecretPassageType.Clock, "room_hidden_vault", new Vector2(-4f, 2f)));
            // Mummies guard the clock
            nwRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Mummy, new Vector2(-2f, 0f)));
            rooms.Add(nwRoom);

            var seRoom = CreateRoomData("room_southeast", "Wine Cellar", 0);
            seRoom.northDoor = CreateDoor("room_east");
            seRoom.westDoor = CreateDoor("room_south");
            // Add barrel passage (Serf only)
            seRoom.secretPassages.Add(CreateSecretPassage(SecretPassageType.Barrel, "room_hidden_tunnel", new Vector2(4f, -2f)));
            seRoom.trapdoor = CreateFloorTransition("room_dungeon_1");
            // Spiders infest the cellar
            seRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(2f, 1f)));
            seRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-1f, -2f)));
            seRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(3f, -1f), 0.4f));
            rooms.Add(seRoom);

            var swRoom = CreateRoomData("room_southwest", "Kitchen", 0);
            swRoom.northDoor = CreateDoor("room_west");
            swRoom.eastDoor = CreateDoor("room_south");
            // Add food spawn
            swRoom.itemSpawns.Add(CreateItemSpawn("food_chicken", new Vector2(2f, 1f)));
            swRoom.itemSpawns.Add(CreateItemSpawn("food_bread", new Vector2(-2f, -1f)));
            // Kitchen has rats (spiders work for now)
            swRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-3f, 0f), 0.5f));
            rooms.Add(swRoom);

            // Floor 1 room (accessible via stairs from center)
            var f1Center = CreateRoomData("room_f1_center", "Upper Hall", 1);
            f1Center.stairsDown = CreateFloorTransition("room_center");
            f1Center.northDoor = CreateDoor("room_f1_north");
            // Demons guard the upper floor
            f1Center.enemySpawns.Add(CreateEnemySpawn(EnemyType.Demon, new Vector2(2f, 1f), 0.6f));
            f1Center.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(-3f, 2f)));
            rooms.Add(f1Center);

            var f1North = CreateRoomData("room_f1_north", "Throne Room", 1);
            f1North.southDoor = CreateDoor("room_f1_center");
            // Key piece location!
            f1North.itemSpawns.Add(CreateItemSpawn("keypiece_0", new Vector2(0f, 2f), true));
            // SPECIAL ENEMY: Dracula guards the throne room!
            f1North.enemySpawns.Add(CreateEnemySpawn(EnemyType.Vampire, new Vector2(0f, 0f), 0.8f));
            // Add Cross item to counter Dracula
            f1North.itemSpawns.Add(CreateItemSpawn("special_cross", new Vector2(4f, -1f)));
            rooms.Add(f1North);

            // Hidden rooms
            var hiddenStudy = CreateRoomData("room_hidden_study", "Secret Study", 0);
            hiddenStudy.secretPassages.Add(CreateSecretPassage(SecretPassageType.Bookcase, "room_west", new Vector2(5f, 0f)));
            hiddenStudy.itemSpawns.Add(CreateItemSpawn("keypiece_1", new Vector2(0f, 0f), true));
            // SPECIAL ENEMY: Frankenstein's Monster guards the study!
            hiddenStudy.enemySpawns.Add(CreateEnemySpawn(EnemyType.Reaper, new Vector2(2f, 1f), 0.7f));
            // Add Spellbook to counter Frankenstein
            hiddenStudy.itemSpawns.Add(CreateItemSpawn("special_spellbook", new Vector2(-3f, 1f)));
            rooms.Add(hiddenStudy);

            var hiddenVault = CreateRoomData("room_hidden_vault", "Hidden Vault", 0);
            hiddenVault.secretPassages.Add(CreateSecretPassage(SecretPassageType.Clock, "room_northwest", new Vector2(4f, -2f)));
            hiddenVault.itemSpawns.Add(CreateItemSpawn("treasure_crown", new Vector2(0f, 0f)));
            // Skeletons guard the vault
            hiddenVault.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(-2f, 0f)));
            hiddenVault.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(2f, 0f)));
            rooms.Add(hiddenVault);

            var hiddenTunnel = CreateRoomData("room_hidden_tunnel", "Secret Tunnel", 0);
            hiddenTunnel.secretPassages.Add(CreateSecretPassage(SecretPassageType.Barrel, "room_southeast", new Vector2(-4f, 2f)));
            hiddenTunnel.itemSpawns.Add(CreateItemSpawn("keypiece_2", new Vector2(0f, 0f), true));
            // SPECIAL ENEMY: Hunchback prowls the tunnel!
            hiddenTunnel.enemySpawns.Add(CreateEnemySpawn(EnemyType.Werewolf, new Vector2(-2f, 0f), 0.7f));
            // Add Garlic Wreath to counter Hunchback
            hiddenTunnel.itemSpawns.Add(CreateItemSpawn("special_wreath", new Vector2(3f, 0f)));
            rooms.Add(hiddenTunnel);

            // Dungeon room - dangerous! (Floor 0 = Basement/Dungeon level)
            var dungeon1 = CreateRoomData("room_dungeon_1", "Dungeon Cell", 0);
            dungeon1.trapdoor = CreateFloorTransition("room_southeast"); // Can go back up
            dungeon1.itemSpawns.Add(CreateItemSpawn("key_red", new Vector2(0f, 0f)));
            // Add hazards
            dungeon1.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Spikes,
                position = new Vector2(2f, 0f),
                size = new Vector2(1f, 1f),
                damagePerSecond = 20f
            });
            dungeon1.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Fire,
                position = new Vector2(-3f, 1f),
                size = new Vector2(1.5f, 1.5f),
                damagePerSecond = 15f
            });
            // Dungeon infested with enemies
            dungeon1.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-2f, -1f)));
            dungeon1.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(3f, 1f)));
            dungeon1.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(0f, 2f)));
            rooms.Add(dungeon1);

            // Secret east room (behind red door) - Treasure well guarded
            var eastSecret = CreateRoomData("room_east_secret", "Treasure Chamber", 0);
            eastSecret.westDoor = CreateLockedDoor("room_east", KeyColor.Red);
            eastSecret.itemSpawns.Add(CreateItemSpawn("treasure_chalice", new Vector2(0f, 0f)));
            // Demon guards the treasure!
            eastSecret.enemySpawns.Add(CreateEnemySpawn(EnemyType.Demon, new Vector2(2f, 0f)));
            eastSecret.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(-2f, 1f)));
            // Add poison hazard around treasure
            eastSecret.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Poison,
                position = new Vector2(0f, -2f),
                size = new Vector2(3f, 0.5f),
                damagePerSecond = 10f
            });
            rooms.Add(eastSecret);

            // Register all rooms
            foreach (var room in rooms)
            {
                RoomManager.Instance.RegisterRoom(room);
            }

            Debug.Log($"[TestRoomSetup] Created {rooms.Count} test rooms");

            // Trigger starting room load now that rooms are registered
            if (RoomManager.Instance != null && RoomManager.Instance.CurrentRoomData == null)
            {
                RoomManager.Instance.LoadStartingRoom();
            }
        }

        private RoomData CreateRoomData(string id, string name, int floor, bool isStart = false, bool isExit = false)
        {
            var room = ScriptableObject.CreateInstance<RoomData>();
            room.roomId = id;
            room.displayName = name;
            room.floorNumber = floor;
            room.isStartRoom = isStart;
            room.isExitRoom = isExit;
            room.roomType = RoomType.Normal;
            room.ambientColor = Color.white;
            room.playerSpawnPoints = CreateDefaultSpawnPoints();
            room.secretPassages = new List<SecretPassage>();
            room.enemySpawns = new List<EnemySpawn>();
            room.itemSpawns = new List<ItemSpawn>();
            room.hazardSpawns = new List<HazardSpawn>();
            return room;
        }

        private List<SpawnPoint> CreateDefaultSpawnPoints()
        {
            return new List<SpawnPoint>
            {
                new SpawnPoint { spawnId = "center", position = Vector2.zero },
                new SpawnPoint { spawnId = "start", position = Vector2.zero },
                new SpawnPoint { spawnId = "north_door", position = new Vector2(0, 3) },
                new SpawnPoint { spawnId = "south_door", position = new Vector2(0, -3) },
                new SpawnPoint { spawnId = "east_door", position = new Vector2(6, 0) },
                new SpawnPoint { spawnId = "west_door", position = new Vector2(-6, 0) },
                new SpawnPoint { spawnId = "stairs_up", position = new Vector2(3, 2) },
                new SpawnPoint { spawnId = "stairs_down", position = new Vector2(-3, 2) },
                new SpawnPoint { spawnId = "trapdoor_exit", position = new Vector2(0, -2) }
            };
        }

        private DoorConnection CreateDoor(string destinationId)
        {
            return new DoorConnection
            {
                exists = true,
                doorType = DoorType.Open,
                requiredKeyColor = KeyColor.None,
                destinationRoomId = destinationId
            };
        }

        private DoorConnection CreateLockedDoor(string destinationId, KeyColor keyColor)
        {
            return new DoorConnection
            {
                exists = true,
                doorType = DoorType.Locked,
                requiredKeyColor = keyColor,
                destinationRoomId = destinationId
            };
        }

        private FloorTransition CreateFloorTransition(string destinationId)
        {
            return new FloorTransition
            {
                exists = true,
                destinationRoomId = destinationId,
                position = new Vector2(3f, 2f)
            };
        }

        private SecretPassage CreateSecretPassage(SecretPassageType type, string destinationId, Vector2 position)
        {
            return new SecretPassage
            {
                passageType = type,
                destinationRoomId = destinationId,
                position = position
            };
        }

        private EnemySpawn CreateEnemySpawn(EnemyType type, Vector2 position, float spawnChance = 1f, bool respawns = true)
        {
            var enemyData = EnemyDatabase.GetEnemy(type);
            return new EnemySpawn
            {
                enemyData = enemyData,
                position = position,
                spawnChance = spawnChance,
                respawnsOnReentry = respawns
            };
        }

        private ItemSpawn CreateItemSpawn(string itemId, Vector2 position, bool persistent = false)
        {
            // Try to get item from ItemDatabase first
            var itemData = ItemDatabase.GetItem(itemId);

            // If not found in database, create a placeholder
            if (itemData == null)
            {
                itemData = ScriptableObject.CreateInstance<ItemData>();
                itemData.itemId = itemId;
                itemData.displayName = itemId.Replace("_", " ");

                // Set item type based on ID
                if (itemId.StartsWith("food"))
                {
                    itemData.itemType = ItemType.Food;
                    itemData.energyRestore = 25f;
                }
                else if (itemId.StartsWith("key_"))
                {
                    itemData.itemType = ItemType.Key;
                    itemData.keyColor = itemId switch
                    {
                        "key_red" => KeyColor.Red,
                        "key_blue" => KeyColor.Blue,
                        "key_green" => KeyColor.Green,
                        _ => KeyColor.None
                    };
                }
                else if (itemId.StartsWith("key_piece") || itemId.StartsWith("keypiece"))
                {
                    itemData.itemType = ItemType.KeyPiece;
                    // Parse index from "key_piece_0" or "keypiece_0"
                    string lastPart = itemId.Split('_')[^1];
                    if (int.TryParse(lastPart, out int index))
                    {
                        itemData.keyPieceIndex = index;
                    }
                }
                else if (itemId.StartsWith("treasure"))
                {
                    itemData.itemType = ItemType.Treasure;
                    itemData.scoreValue = 100;
                }
            }

            return new ItemSpawn
            {
                itemData = itemData,
                position = position,
                persistsAcrossVisits = persistent
            };
        }
    }
}
