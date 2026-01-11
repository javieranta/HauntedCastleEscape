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

            // FLOOR NUMBERING:
            // Floor 0 = Basement/Dungeon (dark, dangerous) - RED test marker
            // Floor 1 = Castle Main Floor (starting area) - GREEN test marker
            // Floor 2 = Tower/Upper Floor - BLUE test marker

            // Center room (starting room) - Safe zone, no enemies - FLOOR 1 (CASTLE)
            var centerRoom = CreateRoomData("room_center", "Grand Hall", 1, true, false);
            centerRoom.northDoor = CreateDoor("room_north");
            centerRoom.southDoor = CreateDoor("room_south");
            centerRoom.eastDoor = CreateDoor("room_east");
            centerRoom.westDoor = CreateDoor("room_west");
            // Stairs UP to tower - positioned on the RIGHT side of room
            centerRoom.stairsUp = new FloorTransition
            {
                exists = true,
                destinationRoomId = "room_f2_center",
                position = new Vector2(5f, 2f)  // Right side
            };
            // Stairs DOWN to basement - positioned on the LEFT side of room
            centerRoom.stairsDown = new FloorTransition
            {
                exists = true,
                destinationRoomId = "room_dungeon_center",
                position = new Vector2(-5f, 2f)  // Left side
            };
            rooms.Add(centerRoom);

            // North room - Bats patrol here - FLOOR 1 (CASTLE)
            var northRoom = CreateRoomData("room_north", "Trophy Room", 1);
            northRoom.southDoor = CreateDoor("room_center");
            northRoom.eastDoor = CreateDoor("room_northeast");
            northRoom.westDoor = CreateDoor("room_northwest");
            northRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(-2f, 1f)));
            northRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(3f, -1f)));
            rooms.Add(northRoom);

            // South room - Exit area, light security - FLOOR 1 (CASTLE)
            var southRoom = CreateRoomData("room_south", "Entrance Hall", 1, false, true);
            southRoom.northDoor = CreateDoor("room_center");
            southRoom.eastDoor = CreateDoor("room_southeast");
            southRoom.westDoor = CreateDoor("room_southwest");
            southRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(0f, 2f), 0.5f));
            rooms.Add(southRoom);

            // East room - Armory has skeleton guards - FLOOR 1 (CASTLE)
            var eastRoom = CreateRoomData("room_east", "Armory", 1);
            eastRoom.westDoor = CreateDoor("room_center");
            eastRoom.northDoor = CreateDoor("room_northeast");
            eastRoom.southDoor = CreateDoor("room_southeast");
            // Add a locked door (red key)
            eastRoom.eastDoor = CreateLockedDoor("room_east_secret", KeyColor.Red);
            eastRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(3f, 0f)));
            eastRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(-2f, 2f), 0.6f));
            rooms.Add(eastRoom);

            // West room - Library haunted by ghosts - FLOOR 1 (CASTLE)
            var westRoom = CreateRoomData("room_west", "Library", 1);
            westRoom.eastDoor = CreateDoor("room_center");
            westRoom.northDoor = CreateDoor("room_northwest");
            westRoom.southDoor = CreateDoor("room_southwest");
            // Add secret bookcase passage (Wizard only)
            westRoom.secretPassages.Add(CreateSecretPassage(SecretPassageType.Bookcase, "room_hidden_study", new Vector2(-5f, 0f)));
            westRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(-3f, 1f)));
            westRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(2f, -1f), 0.5f));
            rooms.Add(westRoom);

            // Corner rooms - FLOOR 1 (CASTLE)
            var neRoom = CreateRoomData("room_northeast", "Tower Base", 1);
            neRoom.southDoor = CreateDoor("room_east");
            neRoom.westDoor = CreateDoor("room_north");
            // Tower has a witch!
            neRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Witch, new Vector2(0f, 0f), 0.7f));
            rooms.Add(neRoom);

            var nwRoom = CreateRoomData("room_northwest", "Clock Room", 1);
            nwRoom.southDoor = CreateDoor("room_west");
            nwRoom.eastDoor = CreateDoor("room_north");
            // Add clock passage (Knight only)
            nwRoom.secretPassages.Add(CreateSecretPassage(SecretPassageType.Clock, "room_hidden_vault", new Vector2(-4f, 2f)));
            // Mummies guard the clock
            nwRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Mummy, new Vector2(-2f, 0f)));
            rooms.Add(nwRoom);

            var seRoom = CreateRoomData("room_southeast", "Wine Cellar", 1);
            seRoom.northDoor = CreateDoor("room_east");
            seRoom.westDoor = CreateDoor("room_south");
            // Add barrel passage (Serf only)
            seRoom.secretPassages.Add(CreateSecretPassage(SecretPassageType.Barrel, "room_hidden_tunnel", new Vector2(4f, -2f)));
            seRoom.trapdoor = CreateFloorTransition("room_dungeon_2"); // Trapdoor to basement
            // Spiders infest the cellar
            seRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(2f, 1f)));
            seRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-1f, -2f)));
            seRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(3f, -1f), 0.4f));
            rooms.Add(seRoom);

            var swRoom = CreateRoomData("room_southwest", "Kitchen", 1);
            swRoom.northDoor = CreateDoor("room_west");
            swRoom.eastDoor = CreateDoor("room_south");
            // Add food spawn
            swRoom.itemSpawns.Add(CreateItemSpawn("food_chicken", new Vector2(2f, 1f)));
            swRoom.itemSpawns.Add(CreateItemSpawn("food_bread", new Vector2(-2f, -1f)));
            // Kitchen has rats (spiders work for now)
            swRoom.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-3f, 0f), 0.5f));
            rooms.Add(swRoom);

            // ============================================================
            // FLOOR 2 - TOWER (10 rooms in 3x3 grid + spire)
            // ============================================================
            // Layout:
            // [NW-Wizard] [N-Throne]  [NE-Belfry]
            // [W-Alchemy] [C-Upper]   [E-Observatory]
            // [SW-Gallery][S-Balcony] [SE-Chapel]
            //             [Spire] (from throne room)

            // Tower Center - Entry from castle
            var f2Center = CreateRoomData("room_f2_center", "Upper Hall", 2);
            f2Center.stairsDown = CreateFloorTransition("room_center");
            f2Center.northDoor = CreateDoor("room_f2_north");
            f2Center.southDoor = CreateDoor("room_f2_south");
            f2Center.eastDoor = CreateDoor("room_f2_east");
            f2Center.westDoor = CreateDoor("room_f2_west");
            f2Center.enemySpawns.Add(CreateEnemySpawn(EnemyType.Demon, new Vector2(2f, 1f), 0.6f));
            f2Center.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(-3f, 2f)));
            rooms.Add(f2Center);

            // Tower North - Throne Room
            var f2North = CreateRoomData("room_f2_north", "Throne Room", 2);
            f2North.southDoor = CreateDoor("room_f2_center");
            f2North.eastDoor = CreateDoor("room_f2_northeast");
            f2North.westDoor = CreateDoor("room_f2_northwest");
            // Stairs to the spire
            f2North.stairsUp = new FloorTransition
            {
                exists = true,
                destinationRoomId = "room_f2_spire",
                position = new Vector2(0f, 3f)
            };
            f2North.enemySpawns.Add(CreateEnemySpawn(EnemyType.Vampire, new Vector2(0f, 0f), 0.8f));
            f2North.itemSpawns.Add(CreateItemSpawn("special_cross", new Vector2(4f, -1f)));
            rooms.Add(f2North);

            // Tower South - Balcony
            var f2South = CreateRoomData("room_f2_south", "Balcony", 2);
            f2South.northDoor = CreateDoor("room_f2_center");
            f2South.eastDoor = CreateDoor("room_f2_southeast");
            f2South.westDoor = CreateDoor("room_f2_southwest");
            f2South.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(-2f, 1f)));
            f2South.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(2f, 1f)));
            f2South.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(0f, -1f)));
            rooms.Add(f2South);

            // Tower East - Observatory
            var f2East = CreateRoomData("room_f2_east", "Observatory", 2);
            f2East.westDoor = CreateDoor("room_f2_center");
            f2East.northDoor = CreateDoor("room_f2_northeast");
            f2East.southDoor = CreateDoor("room_f2_southeast");
            f2East.enemySpawns.Add(CreateEnemySpawn(EnemyType.Witch, new Vector2(0f, 0f), 0.7f));
            f2East.itemSpawns.Add(CreateItemSpawn("treasure_telescope", new Vector2(3f, 2f)));
            rooms.Add(f2East);

            // Tower West - Alchemy Lab
            var f2West = CreateRoomData("room_f2_west", "Alchemy Lab", 2);
            f2West.eastDoor = CreateDoor("room_f2_center");
            f2West.northDoor = CreateDoor("room_f2_northwest");
            f2West.southDoor = CreateDoor("room_f2_southwest");
            f2West.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Acid,
                position = new Vector2(-2f, 0f),
                size = new Vector2(1.5f, 1.5f),
                damagePerSecond = 10f
            });
            f2West.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(2f, 1f)));
            rooms.Add(f2West);

            // Tower Northeast - Belfry
            var f2Northeast = CreateRoomData("room_f2_northeast", "Belfry", 2);
            f2Northeast.southDoor = CreateDoor("room_f2_east");
            f2Northeast.westDoor = CreateDoor("room_f2_north");
            f2Northeast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(-1f, 1f)));
            f2Northeast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(1f, 1f)));
            f2Northeast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(0f, -1f)));
            f2Northeast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(-2f, -1f)));
            rooms.Add(f2Northeast);

            // Tower Northwest - Wizard's Study
            var f2Northwest = CreateRoomData("room_f2_northwest", "Wizard's Study", 2);
            f2Northwest.southDoor = CreateDoor("room_f2_west");
            f2Northwest.eastDoor = CreateDoor("room_f2_north");
            f2Northwest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Witch, new Vector2(0f, 0f), 0.8f));
            f2Northwest.itemSpawns.Add(CreateItemSpawn("special_spellbook", new Vector2(-3f, 2f)));
            rooms.Add(f2Northwest);

            // Tower Southeast - Chapel
            var f2Southeast = CreateRoomData("room_f2_southeast", "Chapel", 2);
            f2Southeast.northDoor = CreateDoor("room_f2_east");
            f2Southeast.westDoor = CreateDoor("room_f2_south");
            f2Southeast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(-2f, 0f)));
            f2Southeast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(2f, 0f)));
            f2Southeast.itemSpawns.Add(CreateItemSpawn("food_holy_water", new Vector2(0f, 2f)));
            rooms.Add(f2Southeast);

            // Tower Southwest - Gallery
            var f2Southwest = CreateRoomData("room_f2_southwest", "Portrait Gallery", 2);
            f2Southwest.northDoor = CreateDoor("room_f2_west");
            f2Southwest.eastDoor = CreateDoor("room_f2_south");
            f2Southwest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(0f, 0f), 0.6f));
            f2Southwest.itemSpawns.Add(CreateItemSpawn("treasure_painting", new Vector2(-3f, 1f)));
            rooms.Add(f2Southwest);

            // Tower Spire - Top of the tower (accessible from throne room)
            var f2Spire = CreateRoomData("room_f2_spire", "Tower Spire", 2);
            f2Spire.stairsDown = new FloorTransition
            {
                exists = true,
                destinationRoomId = "room_f2_north",
                position = new Vector2(0f, -3f)
            };
            // Key piece location!
            f2Spire.itemSpawns.Add(CreateItemSpawn("keypiece_0", new Vector2(0f, 2f), true));
            f2Spire.enemySpawns.Add(CreateEnemySpawn(EnemyType.Demon, new Vector2(-2f, 0f), 0.9f));
            f2Spire.enemySpawns.Add(CreateEnemySpawn(EnemyType.Demon, new Vector2(2f, 0f), 0.9f));
            rooms.Add(f2Spire);

            // Hidden rooms - FLOOR 1 (CASTLE - accessible from castle rooms)
            var hiddenStudy = CreateRoomData("room_hidden_study", "Secret Study", 1);
            hiddenStudy.secretPassages.Add(CreateSecretPassage(SecretPassageType.Bookcase, "room_west", new Vector2(5f, 0f)));
            hiddenStudy.itemSpawns.Add(CreateItemSpawn("keypiece_1", new Vector2(0f, 0f), true));
            // SPECIAL ENEMY: Frankenstein's Monster guards the study!
            hiddenStudy.enemySpawns.Add(CreateEnemySpawn(EnemyType.Reaper, new Vector2(2f, 1f), 0.7f));
            // Add Spellbook to counter Frankenstein
            hiddenStudy.itemSpawns.Add(CreateItemSpawn("special_spellbook", new Vector2(-3f, 1f)));
            rooms.Add(hiddenStudy);

            var hiddenVault = CreateRoomData("room_hidden_vault", "Hidden Vault", 1);
            hiddenVault.secretPassages.Add(CreateSecretPassage(SecretPassageType.Clock, "room_northwest", new Vector2(4f, -2f)));
            hiddenVault.itemSpawns.Add(CreateItemSpawn("treasure_crown", new Vector2(0f, 0f)));
            // Skeletons guard the vault
            hiddenVault.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(-2f, 0f)));
            hiddenVault.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(2f, 0f)));
            rooms.Add(hiddenVault);

            // ============================================================
            // FLOOR 0 - BASEMENT/DUNGEON (10 rooms in 3x3 grid + depths)
            // ============================================================
            // Layout:
            // [NW-Guard]   [N-Torture] [NE-Ritual]
            // [W-Ossuary]  [C-Entry]   [E-Crypt]
            // [SW-Sewer]   [S-Prison]  [SE-Pit]
            //              [Depths] (from prison)

            // Dungeon Center - Entry from castle stairs
            var dungeonCenter = CreateRoomData("room_dungeon_center", "Dungeon Entrance", 0);
            dungeonCenter.stairsUp = CreateFloorTransition("room_center");
            dungeonCenter.northDoor = CreateDoor("room_dungeon_north");
            dungeonCenter.southDoor = CreateDoor("room_dungeon_south");
            dungeonCenter.eastDoor = CreateDoor("room_dungeon_east");
            dungeonCenter.westDoor = CreateDoor("room_dungeon_west");
            dungeonCenter.itemSpawns.Add(CreateItemSpawn("key_red", new Vector2(3f, 0f)));
            dungeonCenter.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-2f, -1f)));
            dungeonCenter.enemySpawns.Add(CreateEnemySpawn(EnemyType.Bat, new Vector2(0f, 2f)));
            rooms.Add(dungeonCenter);

            // Dungeon North - Torture Chamber
            var dungeonNorth = CreateRoomData("room_dungeon_north", "Torture Chamber", 0);
            dungeonNorth.southDoor = CreateDoor("room_dungeon_center");
            dungeonNorth.eastDoor = CreateDoor("room_dungeon_northeast");
            dungeonNorth.westDoor = CreateDoor("room_dungeon_northwest");
            dungeonNorth.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Spikes,
                position = new Vector2(-2f, 0f),
                size = new Vector2(1.5f, 1.5f),
                damagePerSecond = 25f
            });
            dungeonNorth.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Spikes,
                position = new Vector2(2f, 0f),
                size = new Vector2(1.5f, 1.5f),
                damagePerSecond = 25f
            });
            dungeonNorth.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(0f, 1f)));
            rooms.Add(dungeonNorth);

            // Dungeon South - Prison Cells
            var dungeonSouth = CreateRoomData("room_dungeon_south", "Prison Cells", 0);
            dungeonSouth.northDoor = CreateDoor("room_dungeon_center");
            dungeonSouth.eastDoor = CreateDoor("room_dungeon_southeast");
            dungeonSouth.westDoor = CreateDoor("room_dungeon_southwest");
            // Stairs down to the depths
            dungeonSouth.stairsDown = new FloorTransition
            {
                exists = true,
                destinationRoomId = "room_dungeon_depths",
                position = new Vector2(0f, -3f)
            };
            dungeonSouth.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(-3f, 0f)));
            dungeonSouth.enemySpawns.Add(CreateEnemySpawn(EnemyType.Ghost, new Vector2(3f, 0f)));
            rooms.Add(dungeonSouth);

            // Dungeon East - Crypt
            var dungeonEast = CreateRoomData("room_dungeon_east", "Crypt", 0);
            dungeonEast.westDoor = CreateDoor("room_dungeon_center");
            dungeonEast.northDoor = CreateDoor("room_dungeon_northeast");
            dungeonEast.southDoor = CreateDoor("room_dungeon_southeast");
            dungeonEast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(-1f, 1f)));
            dungeonEast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(1f, 1f)));
            dungeonEast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(0f, -1f)));
            dungeonEast.itemSpawns.Add(CreateItemSpawn("treasure_skull", new Vector2(3f, 2f)));
            rooms.Add(dungeonEast);

            // Dungeon West - Ossuary
            var dungeonWest = CreateRoomData("room_dungeon_west", "Ossuary", 0);
            dungeonWest.eastDoor = CreateDoor("room_dungeon_center");
            dungeonWest.northDoor = CreateDoor("room_dungeon_northwest");
            dungeonWest.southDoor = CreateDoor("room_dungeon_southwest");
            dungeonWest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Mummy, new Vector2(0f, 0f), 0.8f));
            dungeonWest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(-2f, 1f)));
            rooms.Add(dungeonWest);

            // Dungeon Northeast - Ritual Chamber
            var dungeonNortheast = CreateRoomData("room_dungeon_northeast", "Ritual Chamber", 0);
            dungeonNortheast.southDoor = CreateDoor("room_dungeon_east");
            dungeonNortheast.westDoor = CreateDoor("room_dungeon_north");
            dungeonNortheast.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Fire,
                position = new Vector2(0f, 0f),
                size = new Vector2(2f, 2f),
                damagePerSecond = 15f
            });
            dungeonNortheast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Demon, new Vector2(-2f, 1f), 0.7f));
            dungeonNortheast.itemSpawns.Add(CreateItemSpawn("special_pentagram", new Vector2(0f, 2f)));
            rooms.Add(dungeonNortheast);

            // Dungeon Northwest - Guard Room
            var dungeonNorthwest = CreateRoomData("room_dungeon_northwest", "Guard Room", 0);
            dungeonNorthwest.southDoor = CreateDoor("room_dungeon_west");
            dungeonNorthwest.eastDoor = CreateDoor("room_dungeon_north");
            dungeonNorthwest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(-1f, 0f)));
            dungeonNorthwest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Skeleton, new Vector2(1f, 0f)));
            dungeonNorthwest.itemSpawns.Add(CreateItemSpawn("key_blue", new Vector2(0f, 2f)));
            dungeonNorthwest.itemSpawns.Add(CreateItemSpawn("food_bread", new Vector2(-3f, -1f)));
            rooms.Add(dungeonNorthwest);

            // Dungeon Southeast - The Pit
            var dungeonSoutheast = CreateRoomData("room_dungeon_southeast", "The Pit", 0);
            dungeonSoutheast.northDoor = CreateDoor("room_dungeon_east");
            dungeonSoutheast.westDoor = CreateDoor("room_dungeon_south");
            dungeonSoutheast.trapdoor = CreateFloorTransition("room_southeast"); // Back to wine cellar
            dungeonSoutheast.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Acid,
                position = new Vector2(0f, -1f),
                size = new Vector2(3f, 2f),
                damagePerSecond = 20f
            });
            dungeonSoutheast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-2f, 1f)));
            dungeonSoutheast.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(2f, 1f)));
            rooms.Add(dungeonSoutheast);

            // Dungeon Southwest - Sewer
            var dungeonSouthwest = CreateRoomData("room_dungeon_southwest", "Sewer", 0);
            dungeonSouthwest.northDoor = CreateDoor("room_dungeon_west");
            dungeonSouthwest.eastDoor = CreateDoor("room_dungeon_south");
            dungeonSouthwest.westDoor = CreateDoor("room_hidden_tunnel"); // Connect to secret tunnel
            dungeonSouthwest.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = HazardType.Poison,
                position = new Vector2(0f, 0f),
                size = new Vector2(4f, 1f),
                damagePerSecond = 8f
            });
            dungeonSouthwest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-2f, -1f)));
            dungeonSouthwest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(2f, -1f)));
            dungeonSouthwest.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(0f, 2f)));
            rooms.Add(dungeonSouthwest);

            // Dungeon Depths - Deepest level (accessible from prison)
            var dungeonDepths = CreateRoomData("room_dungeon_depths", "The Depths", 0);
            dungeonDepths.stairsUp = new FloorTransition
            {
                exists = true,
                destinationRoomId = "room_dungeon_south",
                position = new Vector2(0f, 3f)
            };
            // Key piece location!
            dungeonDepths.itemSpawns.Add(CreateItemSpawn("keypiece_2", new Vector2(0f, 0f), true));
            // SPECIAL ENEMY: Werewolf guards the depths!
            dungeonDepths.enemySpawns.Add(CreateEnemySpawn(EnemyType.Werewolf, new Vector2(-2f, 0f), 0.9f));
            dungeonDepths.enemySpawns.Add(CreateEnemySpawn(EnemyType.Reaper, new Vector2(2f, 0f), 0.7f));
            dungeonDepths.itemSpawns.Add(CreateItemSpawn("special_wreath", new Vector2(3f, 2f)));
            rooms.Add(dungeonDepths);

            // Hidden tunnel - Still accessible via secret passage from wine cellar
            var hiddenTunnel = CreateRoomData("room_hidden_tunnel", "Secret Tunnel", 0);
            hiddenTunnel.secretPassages.Add(CreateSecretPassage(SecretPassageType.Barrel, "room_southeast", new Vector2(-4f, 2f)));
            hiddenTunnel.eastDoor = CreateDoor("room_dungeon_southwest"); // Connect to sewer
            hiddenTunnel.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(-2f, 0f)));
            hiddenTunnel.enemySpawns.Add(CreateEnemySpawn(EnemyType.Spider, new Vector2(2f, 0f)));
            hiddenTunnel.itemSpawns.Add(CreateItemSpawn("food_chicken", new Vector2(0f, 2f)));
            rooms.Add(hiddenTunnel);

            // Secret east room (behind red door) - Treasure well guarded - FLOOR 1 (accessed from castle armory)
            var eastSecret = CreateRoomData("room_east_secret", "Treasure Chamber", 1);
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
