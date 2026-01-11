using System.Collections.Generic;
using UnityEngine;
using HauntedCastle.Services;

namespace HauntedCastle.Data
{
    /// <summary>
    /// Comprehensive room database containing all 30 rooms across 3 floors.
    /// Floor 0 (Basement): Dark dungeon theme - 10 rooms
    /// Floor 1 (Main Castle): Living rooms theme - 10 rooms
    /// Floor 2 (Tower): Battlements theme - 10 rooms
    /// </summary>
    public class RoomDatabase : MonoBehaviour
    {
        public static RoomDatabase Instance { get; private set; }

        [Header("Room Collections")]
        public List<RoomData> basementRooms = new();
        public List<RoomData> castleRooms = new();
        public List<RoomData> towerRooms = new();

        [Header("Starting Room")]
        public RoomData startingRoom;

        private Dictionary<string, RoomData> _allRooms = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            GenerateAllRooms();
            RegisterWithRoomManager();
        }

        /// <summary>
        /// Generates all 30 rooms programmatically.
        /// </summary>
        public void GenerateAllRooms()
        {
            basementRooms.Clear();
            castleRooms.Clear();
            towerRooms.Clear();
            _allRooms.Clear();

            // Generate Basement Rooms (Floor 0)
            GenerateBasementRooms();

            // Generate Castle Rooms (Floor 1)
            GenerateCastleRooms();

            // Generate Tower Rooms (Floor 2)
            GenerateTowerRooms();

            // Set starting room (center of main castle floor)
            startingRoom = _allRooms.GetValueOrDefault("castle_hall");

            Debug.Log($"[RoomDatabase] Generated {_allRooms.Count} rooms total");
        }

        #region Basement Rooms (Floor 0) - Dark Dungeon Theme

        private void GenerateBasementRooms()
        {
            // B1: Dungeon Entrance - connects to castle via stairs
            var dungeonEntrance = CreateRoom("basement_entrance", "Dungeon Entrance", 0, RoomType.Dungeon);
            dungeonEntrance.stairsUp = new FloorTransition
            {
                exists = true,
                destinationRoomId = "castle_cellar",
                position = new Vector2(4, 2)
            };
            dungeonEntrance.eastDoor = CreateDoor("basement_cells", DoorType.Open);
            dungeonEntrance.southDoor = CreateDoor("basement_storage", DoorType.Open);
            AddSpawnPoint(dungeonEntrance, "stairs_down", new Vector2(4, 1));
            AddSpawnPoint(dungeonEntrance, "center", Vector2.zero);
            basementRooms.Add(dungeonEntrance);
            _allRooms[dungeonEntrance.roomId] = dungeonEntrance;

            // B2: Prison Cells
            var cells = CreateRoom("basement_cells", "Prison Cells", 0, RoomType.Dungeon);
            cells.westDoor = CreateDoor("basement_entrance", DoorType.Open);
            cells.eastDoor = CreateDoor("basement_torture", DoorType.Open);
            cells.southDoor = CreateDoor("basement_crypt", DoorType.Locked, KeyColor.Red);
            AddEnemySpawn(cells, "Skeleton", new Vector2(-3, 2));
            AddEnemySpawn(cells, "Skeleton", new Vector2(3, -2));
            basementRooms.Add(cells);
            _allRooms[cells.roomId] = cells;

            // B3: Torture Chamber
            var torture = CreateRoom("basement_torture", "Torture Chamber", 0, RoomType.Dungeon);
            torture.westDoor = CreateDoor("basement_cells", DoorType.Open);
            torture.northDoor = CreateDoor("basement_armory", DoorType.Open);
            AddHazard(torture, HazardType.Spikes, new Vector2(0, -2), new Vector2(4, 1));
            AddEnemySpawn(torture, "Hunchback", new Vector2(2, 2));
            AddItemSpawn(torture, "Food_Drumstick", new Vector2(-4, 3));
            basementRooms.Add(torture);
            _allRooms[torture.roomId] = torture;

            // B4: Underground Storage
            var storage = CreateRoom("basement_storage", "Underground Storage", 0, RoomType.Dungeon);
            storage.northDoor = CreateDoor("basement_entrance", DoorType.Open);
            storage.eastDoor = CreateDoor("basement_crypt", DoorType.Open);
            storage.southDoor = CreateDoor("basement_well", DoorType.Open);
            AddItemSpawn(storage, "Key_Yellow", new Vector2(3, 0));
            AddSecretPassage(storage, SecretPassageType.Barrel, "basement_treasure", new Vector2(-4, -2));
            basementRooms.Add(storage);
            _allRooms[storage.roomId] = storage;

            // B5: Crypt
            var crypt = CreateRoom("basement_crypt", "Ancient Crypt", 0, RoomType.Crypt);
            crypt.northDoor = CreateDoor("basement_cells", DoorType.Locked, KeyColor.Red);
            crypt.westDoor = CreateDoor("basement_storage", DoorType.Open);
            crypt.southDoor = CreateDoor("basement_tomb", DoorType.Open);
            AddEnemySpawn(crypt, "Ghost", new Vector2(0, 0));
            AddEnemySpawn(crypt, "Skeleton", new Vector2(-3, -2));
            basementRooms.Add(crypt);
            _allRooms[crypt.roomId] = crypt;

            // B6: Underground Armory
            var armory = CreateRoom("basement_armory", "Underground Armory", 0, RoomType.Chamber);
            armory.southDoor = CreateDoor("basement_torture", DoorType.Open);
            armory.eastDoor = CreateDoor("basement_tunnel", DoorType.Open);
            AddItemSpawn(armory, "Key_Red", new Vector2(0, 2));
            AddItemSpawn(armory, "Food_Bread", new Vector2(-3, -1));
            basementRooms.Add(armory);
            _allRooms[armory.roomId] = armory;

            // B7: Underground Well
            var well = CreateRoom("basement_well", "Underground Well", 0, RoomType.Dungeon);
            well.northDoor = CreateDoor("basement_storage", DoorType.Open);
            well.eastDoor = CreateDoor("basement_tomb", DoorType.Open);
            AddHazard(well, HazardType.Acid, new Vector2(0, 0), new Vector2(3, 3));
            AddEnemySpawn(well, "Spider", new Vector2(-4, 2));
            basementRooms.Add(well);
            _allRooms[well.roomId] = well;

            // B8: Tomb
            var tomb = CreateRoom("basement_tomb", "Ancient Tomb", 0, RoomType.Crypt);
            tomb.northDoor = CreateDoor("basement_crypt", DoorType.Open);
            tomb.westDoor = CreateDoor("basement_well", DoorType.Open);
            tomb.eastDoor = CreateDoor("basement_treasure", DoorType.Locked, KeyColor.Yellow);
            AddEnemySpawn(tomb, "Mummy", new Vector2(0, 0));
            AddEnemySpawn(tomb, "Ghost", new Vector2(3, 2));
            basementRooms.Add(tomb);
            _allRooms[tomb.roomId] = tomb;

            // B9: Secret Tunnel
            var tunnel = CreateRoom("basement_tunnel", "Secret Tunnel", 0, RoomType.Corridor);
            tunnel.westDoor = CreateDoor("basement_armory", DoorType.Open);
            tunnel.southDoor = CreateDoor("basement_treasure", DoorType.Hidden);
            AddEnemySpawn(tunnel, "Bat", new Vector2(-2, 1));
            AddEnemySpawn(tunnel, "Bat", new Vector2(2, -1));
            AddEnemySpawn(tunnel, "Spider", new Vector2(0, -3));
            basementRooms.Add(tunnel);
            _allRooms[tunnel.roomId] = tunnel;

            // B10: Treasure Vault
            var treasure = CreateRoom("basement_treasure", "Treasure Vault", 0, RoomType.Chamber);
            treasure.westDoor = CreateDoor("basement_tomb", DoorType.Locked, KeyColor.Yellow);
            treasure.northDoor = CreateDoor("basement_tunnel", DoorType.Hidden);
            AddItemSpawn(treasure, "ACG_A", new Vector2(0, 0)); // Key ACG piece!
            AddItemSpawn(treasure, "Food_Cheese", new Vector2(-3, 2));
            AddItemSpawn(treasure, "Food_Apple", new Vector2(3, 2));
            AddSecretPassage(treasure, SecretPassageType.Barrel, "basement_storage", new Vector2(4, -2));
            basementRooms.Add(treasure);
            _allRooms[treasure.roomId] = treasure;
        }

        #endregion

        #region Castle Rooms (Floor 1) - Main Castle Theme

        private void GenerateCastleRooms()
        {
            // C1: Grand Hall - Starting Room
            var hall = CreateRoom("castle_hall", "Grand Hall", 1, RoomType.Hall);
            hall.isStartRoom = true;
            hall.northDoor = CreateDoor("castle_throne", DoorType.Locked, KeyColor.Blue);
            hall.eastDoor = CreateDoor("castle_dining", DoorType.Open);
            hall.southDoor = CreateDoor("castle_foyer", DoorType.Open);
            hall.westDoor = CreateDoor("castle_library", DoorType.Open);
            AddSpawnPoint(hall, "start", Vector2.zero);
            AddSpawnPoint(hall, "center", Vector2.zero);
            castleRooms.Add(hall);
            _allRooms[hall.roomId] = hall;

            // C2: Throne Room
            var throne = CreateRoom("castle_throne", "Throne Room", 1, RoomType.Throne);
            throne.southDoor = CreateDoor("castle_hall", DoorType.Locked, KeyColor.Blue);
            throne.eastDoor = CreateDoor("castle_gallery", DoorType.Open);
            throne.stairsUp = new FloorTransition
            {
                exists = true,
                destinationRoomId = "tower_landing",
                position = new Vector2(-4, 2)
            };
            AddEnemySpawn(throne, "Devil", new Vector2(0, 2)); // Boss area
            AddItemSpawn(throne, "ACG_C", new Vector2(0, 3)); // Key ACG piece!
            castleRooms.Add(throne);
            _allRooms[throne.roomId] = throne;

            // C3: Dining Room
            var dining = CreateRoom("castle_dining", "Dining Room", 1, RoomType.Normal);
            dining.westDoor = CreateDoor("castle_hall", DoorType.Open);
            dining.northDoor = CreateDoor("castle_gallery", DoorType.Open);
            dining.eastDoor = CreateDoor("castle_kitchen", DoorType.Open);
            AddItemSpawn(dining, "Food_Drumstick", new Vector2(-2, 0));
            AddItemSpawn(dining, "Food_Bread", new Vector2(2, 0));
            AddEnemySpawn(dining, "Ghost", new Vector2(0, 2));
            castleRooms.Add(dining);
            _allRooms[dining.roomId] = dining;

            // C4: Library
            var library = CreateRoom("castle_library", "Library", 1, RoomType.Library);
            library.eastDoor = CreateDoor("castle_hall", DoorType.Open);
            library.northDoor = CreateDoor("castle_study", DoorType.Open);
            library.southDoor = CreateDoor("castle_cellar", DoorType.Open);
            AddSecretPassage(library, SecretPassageType.Bookcase, "castle_secret", new Vector2(-5, 0));
            AddEnemySpawn(library, "Ghost", new Vector2(3, 2));
            castleRooms.Add(library);
            _allRooms[library.roomId] = library;

            // C5: Foyer
            var foyer = CreateRoom("castle_foyer", "Castle Foyer", 1, RoomType.Normal);
            foyer.northDoor = CreateDoor("castle_hall", DoorType.Open);
            foyer.eastDoor = CreateDoor("castle_armory", DoorType.Open);
            foyer.westDoor = CreateDoor("castle_cellar", DoorType.Open);
            foyer.isExitRoom = true; // Exit from castle
            AddSpawnPoint(foyer, "exit", new Vector2(0, -3));
            castleRooms.Add(foyer);
            _allRooms[foyer.roomId] = foyer;

            // C6: Kitchen
            var kitchen = CreateRoom("castle_kitchen", "Kitchen", 1, RoomType.Kitchen);
            kitchen.westDoor = CreateDoor("castle_dining", DoorType.Open);
            kitchen.southDoor = CreateDoor("castle_armory", DoorType.Open);
            AddItemSpawn(kitchen, "Food_Apple", new Vector2(-3, 1));
            AddItemSpawn(kitchen, "Food_Cheese", new Vector2(3, 1));
            AddItemSpawn(kitchen, "Food_Drumstick", new Vector2(0, -2));
            AddHazard(kitchen, HazardType.Fire, new Vector2(0, 2), new Vector2(2, 2));
            castleRooms.Add(kitchen);
            _allRooms[kitchen.roomId] = kitchen;

            // C7: Art Gallery
            var gallery = CreateRoom("castle_gallery", "Art Gallery", 1, RoomType.Normal);
            gallery.westDoor = CreateDoor("castle_throne", DoorType.Open);
            gallery.southDoor = CreateDoor("castle_dining", DoorType.Open);
            gallery.eastDoor = CreateDoor("castle_secret", DoorType.Hidden);
            AddEnemySpawn(gallery, "Frankenstein", new Vector2(0, 0));
            AddItemSpawn(gallery, "Key_Blue", new Vector2(4, 3));
            castleRooms.Add(gallery);
            _allRooms[gallery.roomId] = gallery;

            // C8: Study
            var study = CreateRoom("castle_study", "Study", 1, RoomType.Normal);
            study.southDoor = CreateDoor("castle_library", DoorType.Open);
            study.eastDoor = CreateDoor("castle_secret", DoorType.Locked, KeyColor.Green);
            AddItemSpawn(study, "Key_Cyan", new Vector2(-2, 2));
            AddSecretPassage(study, SecretPassageType.Clock, "tower_observatory", new Vector2(4, 0));
            castleRooms.Add(study);
            _allRooms[study.roomId] = study;

            // C9: Cellar - connects to basement
            var cellar = CreateRoom("castle_cellar", "Wine Cellar", 1, RoomType.Normal);
            cellar.eastDoor = CreateDoor("castle_foyer", DoorType.Open);
            cellar.northDoor = CreateDoor("castle_library", DoorType.Open);
            cellar.stairsDown = new FloorTransition
            {
                exists = true,
                destinationRoomId = "basement_entrance",
                position = new Vector2(-4, -2)
            };
            AddItemSpawn(cellar, "Food_Bread", new Vector2(2, 0));
            AddEnemySpawn(cellar, "Spider", new Vector2(-2, -2));
            castleRooms.Add(cellar);
            _allRooms[cellar.roomId] = cellar;

            // C10: Armory
            var castleArmory = CreateRoom("castle_armory", "Castle Armory", 1, RoomType.Chamber);
            castleArmory.westDoor = CreateDoor("castle_foyer", DoorType.Open);
            castleArmory.northDoor = CreateDoor("castle_kitchen", DoorType.Open);
            AddItemSpawn(castleArmory, "Key_Green", new Vector2(0, 0));
            AddEnemySpawn(castleArmory, "Skeleton", new Vector2(-3, 2));
            AddEnemySpawn(castleArmory, "Skeleton", new Vector2(3, 2));
            castleRooms.Add(castleArmory);
            _allRooms[castleArmory.roomId] = castleArmory;

            // C11: Secret Room (bonus room)
            var secret = CreateRoom("castle_secret", "Secret Chamber", 1, RoomType.Chamber);
            secret.westDoor = CreateDoor("castle_study", DoorType.Locked, KeyColor.Green);
            secret.westDoor.exists = true; // Hidden but exists
            AddSecretPassage(secret, SecretPassageType.Bookcase, "castle_library", new Vector2(-4, 0));
            AddItemSpawn(secret, "Food_Apple", new Vector2(-2, 0));
            AddItemSpawn(secret, "Food_Drumstick", new Vector2(2, 0));
            castleRooms.Add(secret);
            _allRooms[secret.roomId] = secret;
        }

        #endregion

        #region Tower Rooms (Floor 2) - Battlements Theme

        private void GenerateTowerRooms()
        {
            // T1: Tower Landing - connects from throne room
            var landing = CreateRoom("tower_landing", "Tower Landing", 2, RoomType.Tower);
            landing.stairsDown = new FloorTransition
            {
                exists = true,
                destinationRoomId = "castle_throne",
                position = new Vector2(4, -2)
            };
            landing.northDoor = CreateDoor("tower_guardroom", DoorType.Open);
            landing.eastDoor = CreateDoor("tower_barracks", DoorType.Open);
            AddSpawnPoint(landing, "stairs_up", new Vector2(4, -1));
            towerRooms.Add(landing);
            _allRooms[landing.roomId] = landing;

            // T2: Guard Room
            var guardroom = CreateRoom("tower_guardroom", "Guard Room", 2, RoomType.Tower);
            guardroom.southDoor = CreateDoor("tower_landing", DoorType.Open);
            guardroom.eastDoor = CreateDoor("tower_armory", DoorType.Open);
            guardroom.northDoor = CreateDoor("tower_battlements", DoorType.Locked, KeyColor.Cyan);
            AddEnemySpawn(guardroom, "Skeleton", new Vector2(-2, 0));
            AddEnemySpawn(guardroom, "Skeleton", new Vector2(2, 0));
            towerRooms.Add(guardroom);
            _allRooms[guardroom.roomId] = guardroom;

            // T3: Tower Barracks
            var barracks = CreateRoom("tower_barracks", "Barracks", 2, RoomType.Normal);
            barracks.westDoor = CreateDoor("tower_landing", DoorType.Open);
            barracks.northDoor = CreateDoor("tower_armory", DoorType.Open);
            barracks.eastDoor = CreateDoor("tower_storage", DoorType.Open);
            AddItemSpawn(barracks, "Food_Drumstick", new Vector2(-3, 2));
            AddEnemySpawn(barracks, "Ghost", new Vector2(2, -1));
            towerRooms.Add(barracks);
            _allRooms[barracks.roomId] = barracks;

            // T4: Tower Armory
            var towerArmory = CreateRoom("tower_armory", "Tower Armory", 2, RoomType.Chamber);
            towerArmory.westDoor = CreateDoor("tower_guardroom", DoorType.Open);
            towerArmory.southDoor = CreateDoor("tower_barracks", DoorType.Open);
            towerArmory.northDoor = CreateDoor("tower_balcony", DoorType.Open);
            AddItemSpawn(towerArmory, "Key_Magenta", new Vector2(0, 0));
            towerRooms.Add(towerArmory);
            _allRooms[towerArmory.roomId] = towerArmory;

            // T5: Storage Room
            var towerStorage = CreateRoom("tower_storage", "Tower Storage", 2, RoomType.Normal);
            towerStorage.westDoor = CreateDoor("tower_barracks", DoorType.Open);
            towerStorage.northDoor = CreateDoor("tower_observatory", DoorType.Locked, KeyColor.Magenta);
            AddItemSpawn(towerStorage, "Food_Cheese", new Vector2(-2, 1));
            AddItemSpawn(towerStorage, "Food_Apple", new Vector2(2, 1));
            AddEnemySpawn(towerStorage, "Bat", new Vector2(0, -2));
            towerRooms.Add(towerStorage);
            _allRooms[towerStorage.roomId] = towerStorage;

            // T6: Battlements
            var battlements = CreateRoom("tower_battlements", "Battlements", 2, RoomType.Tower);
            battlements.southDoor = CreateDoor("tower_guardroom", DoorType.Locked, KeyColor.Cyan);
            battlements.eastDoor = CreateDoor("tower_balcony", DoorType.Open);
            battlements.northDoor = CreateDoor("tower_peak", DoorType.Open);
            AddEnemySpawn(battlements, "Bat", new Vector2(-3, 2));
            AddEnemySpawn(battlements, "Bat", new Vector2(3, 2));
            AddEnemySpawn(battlements, "Bat", new Vector2(0, -2));
            towerRooms.Add(battlements);
            _allRooms[battlements.roomId] = battlements;

            // T7: Balcony
            var balcony = CreateRoom("tower_balcony", "Tower Balcony", 2, RoomType.Tower);
            balcony.southDoor = CreateDoor("tower_armory", DoorType.Open);
            balcony.westDoor = CreateDoor("tower_battlements", DoorType.Open);
            balcony.northDoor = CreateDoor("tower_wizard", DoorType.Open);
            AddHazard(balcony, HazardType.Spikes, new Vector2(-4, 0), new Vector2(1, 4)); // Edge of balcony
            AddHazard(balcony, HazardType.Spikes, new Vector2(4, 0), new Vector2(1, 4));
            towerRooms.Add(balcony);
            _allRooms[balcony.roomId] = balcony;

            // T8: Observatory
            var observatory = CreateRoom("tower_observatory", "Observatory", 2, RoomType.Tower);
            observatory.southDoor = CreateDoor("tower_storage", DoorType.Locked, KeyColor.Magenta);
            observatory.northDoor = CreateDoor("tower_wizard", DoorType.Open);
            AddSecretPassage(observatory, SecretPassageType.Clock, "castle_study", new Vector2(-4, 0));
            AddItemSpawn(observatory, "Food_Bread", new Vector2(2, 2));
            AddEnemySpawn(observatory, "Ghost", new Vector2(0, 0));
            towerRooms.Add(observatory);
            _allRooms[observatory.roomId] = observatory;

            // T9: Wizard's Chamber
            var wizard = CreateRoom("tower_wizard", "Wizard's Chamber", 2, RoomType.Chamber);
            wizard.southDoor = CreateDoor("tower_balcony", DoorType.Open);
            wizard.westDoor = CreateDoor("tower_peak", DoorType.Open);
            wizard.eastDoor = CreateDoor("tower_observatory", DoorType.Open);
            AddEnemySpawn(wizard, "Devil", new Vector2(0, 0)); // Mini-boss
            AddItemSpawn(wizard, "Key_Yellow", new Vector2(3, 3)); // For treasure
            towerRooms.Add(wizard);
            _allRooms[wizard.roomId] = wizard;

            // T10: Tower Peak - Final ACG piece location
            var peak = CreateRoom("tower_peak", "Tower Peak", 2, RoomType.Tower);
            peak.southDoor = CreateDoor("tower_battlements", DoorType.Open);
            peak.eastDoor = CreateDoor("tower_wizard", DoorType.Open);
            AddItemSpawn(peak, "ACG_G", new Vector2(0, 2)); // Final ACG piece!
            AddEnemySpawn(peak, "Frankenstein", new Vector2(-2, -1));
            AddEnemySpawn(peak, "Mummy", new Vector2(2, -1));
            towerRooms.Add(peak);
            _allRooms[peak.roomId] = peak;
        }

        #endregion

        #region Helper Methods

        private RoomData CreateRoom(string id, string name, int floor, RoomType type)
        {
            var room = ScriptableObject.CreateInstance<RoomData>();
            room.roomId = id;
            room.displayName = name;
            room.floorNumber = floor;
            room.roomType = type;
            room.ambientColor = GetFloorAmbientColor(floor);
            room.playerSpawnPoints = new List<SpawnPoint>();
            room.enemySpawns = new List<EnemySpawn>();
            room.itemSpawns = new List<ItemSpawn>();
            room.hazardSpawns = new List<HazardSpawn>();
            room.secretPassages = new List<SecretPassage>();

            // Add default spawn points
            AddSpawnPoint(room, "north_door", new Vector2(0, 3));
            AddSpawnPoint(room, "south_door", new Vector2(0, -3));
            AddSpawnPoint(room, "east_door", new Vector2(5, 0));
            AddSpawnPoint(room, "west_door", new Vector2(-5, 0));

            return room;
        }

        private DoorConnection CreateDoor(string destinationId, DoorType type, KeyColor keyColor = KeyColor.None)
        {
            return new DoorConnection
            {
                exists = true,
                destinationRoomId = destinationId,
                doorType = type,
                requiredKeyColor = keyColor
            };
        }

        private void AddSpawnPoint(RoomData room, string id, Vector2 position)
        {
            room.playerSpawnPoints.Add(new SpawnPoint { spawnId = id, position = position });
        }

        private void AddEnemySpawn(RoomData room, string enemyType, Vector2 position)
        {
            // Get enemy data from database
            EnemyType type = ParseEnemyType(enemyType);
            EnemyData enemyData = EnemyDatabase.GetEnemy(type);

            if (enemyData == null)
            {
                Debug.LogWarning($"[RoomDatabase] Unknown enemy type: {enemyType}");
                return;
            }

            var spawn = new EnemySpawn
            {
                enemyData = enemyData,
                position = position,
                spawnChance = 0.85f,
                respawnsOnReentry = true
            };
            room.enemySpawns.Add(spawn);
        }

        private EnemyType ParseEnemyType(string typeName)
        {
            return typeName.ToLower() switch
            {
                "ghost" => EnemyType.Ghost,
                "skeleton" => EnemyType.Skeleton,
                "spider" => EnemyType.Spider,
                "bat" => EnemyType.Bat,
                "demon" or "devil" => EnemyType.Demon,
                "mummy" => EnemyType.Mummy,
                "witch" => EnemyType.Witch,
                "vampire" or "dracula" => EnemyType.Vampire,
                "werewolf" or "hunchback" => EnemyType.Werewolf,
                "reaper" or "frankenstein" => EnemyType.Reaper,
                _ => EnemyType.Ghost // Default
            };
        }

        private void AddItemSpawn(RoomData room, string itemType, Vector2 position)
        {
            var spawn = new ItemSpawn
            {
                position = position,
                persistsAcrossVisits = itemType.StartsWith("ACG_") || itemType.StartsWith("Key_")
            };
            room.itemSpawns.Add(spawn);
        }

        private void AddHazard(RoomData room, HazardType type, Vector2 position, Vector2 size)
        {
            room.hazardSpawns.Add(new HazardSpawn
            {
                hazardType = type,
                position = position,
                size = size,
                damagePerSecond = type == HazardType.Fire ? 15f : 10f
            });
        }

        private void AddSecretPassage(RoomData room, SecretPassageType type, string destinationId, Vector2 position)
        {
            room.secretPassages.Add(new SecretPassage
            {
                passageType = type,
                destinationRoomId = destinationId,
                position = position
            });
        }

        private Color GetFloorAmbientColor(int floor)
        {
            return floor switch
            {
                0 => new Color(0.75f, 0.68f, 0.8f),   // Basement - lighter purple tint for better visibility
                1 => new Color(0.9f, 0.85f, 0.75f),   // Castle - warm golden
                2 => new Color(0.7f, 0.8f, 0.95f),    // Tower - cool sky blue
                _ => Color.white
            };
        }

        #endregion

        /// <summary>
        /// Registers all generated rooms with the RoomManager.
        /// </summary>
        public void RegisterWithRoomManager()
        {
            if (RoomManager.Instance == null)
            {
                Debug.LogWarning("[RoomDatabase] RoomManager not found, rooms will be registered when available");
                return;
            }

            foreach (var room in _allRooms.Values)
            {
                RoomManager.Instance.RegisterRoom(room);
            }

            Debug.Log($"[RoomDatabase] Registered {_allRooms.Count} rooms with RoomManager");
        }

        /// <summary>
        /// Gets a room by its ID.
        /// </summary>
        public RoomData GetRoom(string roomId)
        {
            return _allRooms.GetValueOrDefault(roomId);
        }

        /// <summary>
        /// Gets all rooms on a specific floor.
        /// </summary>
        public List<RoomData> GetRoomsByFloor(int floor)
        {
            return floor switch
            {
                0 => basementRooms,
                1 => castleRooms,
                2 => towerRooms,
                _ => new List<RoomData>()
            };
        }

        /// <summary>
        /// Gets the total room count.
        /// </summary>
        public int TotalRoomCount => _allRooms.Count;

        /// <summary>
        /// Gets floor names for UI display.
        /// </summary>
        public static string GetFloorName(int floor)
        {
            return floor switch
            {
                0 => "Basement - The Dungeons",
                1 => "Main Floor - Castle Halls",
                2 => "Top Floor - The Towers",
                _ => "Unknown Floor"
            };
        }

        /// <summary>
        /// Gets floor description for UI display.
        /// </summary>
        public static string GetFloorDescription(int floor)
        {
            return floor switch
            {
                0 => "Dark, damp dungeons filled with prisoners' spirits and ancient horrors.",
                1 => "The main castle halls where nobles once walked. Now haunted by restless dead.",
                2 => "High towers reaching toward the stormy sky, home to the castle's darkest secrets.",
                _ => ""
            };
        }
    }
}
